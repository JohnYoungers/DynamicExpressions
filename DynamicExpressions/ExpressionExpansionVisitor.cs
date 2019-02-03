using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DynamicExpressions.Mapping;

namespace DynamicExpressions
{
    public class ExpressionExpansionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, Expression> FlattenedExpressions;

        public ExpressionExpansionVisitor() : this(new Dictionary<ParameterExpression, Expression>()) { }

        private ExpressionExpansionVisitor(Dictionary<ParameterExpression, Expression> flattenedExpressions)
        {
            FlattenedExpressions = flattenedExpressions;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return FlattenedExpressions.ContainsKey(node) ? FlattenedExpressions[node] : base.VisitParameter(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            var target = node.Expression;
            if (target is MemberExpression)
            {
                target = DoSomething((MemberExpression)target);
            }
            if (target is ConstantExpression)
            {
                target = ((ConstantExpression)target).Value as Expression;
            }

            var lambda = (LambdaExpression)target;

            var fe = new Dictionary<ParameterExpression, Expression>(FlattenedExpressions);
            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    fe.Add(lambda.Parameters[i], Visit(node.Arguments[i]));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not flatten Invocation", ex);
            }

            return new ExpressionExpansionVisitor(fe).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(ExpressionMapExtensions) && node.Method.Name == "Invoke")
            {
                var target = node.Arguments[0];
                if (target is MemberExpression)
                {
                    target = DoSomething((MemberExpression)target);
                }
                if (target is ConstantExpression)
                {
                    target = ((ConstantExpression)target).Value as Expression;
                }
                if (target is UnaryExpression)
                {
                    target = ((UnaryExpression)target).Operand;
                }

                var lambda = (LambdaExpression)target;
                if (lambda != null)
                {
                    var fe = new Dictionary<ParameterExpression, Expression>(FlattenedExpressions);
                    try
                    {
                        for (int i = 0; i < lambda.Parameters.Count; i++)
                        {
                            fe.Add(lambda.Parameters[i], Visit(node.Arguments[i + 1]));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Could not flatten Invoke", ex);
                    }

                    return new ExpressionExpansionVisitor(fe).Visit(lambda.Body);
                }
            }

            return base.VisitMethodCall(node);
        }

        Expression DoSomething(MemberExpression input)
        {
            if (input == null)
            {
                return null;
            }

            var field = input.Member as FieldInfo;

            if (field == null)
            {
                if (input.Expression is ParameterExpression && FlattenedExpressions.ContainsKey(input.Expression as ParameterExpression))
                {
                    return base.VisitMember(input);
                }

                return input;
            }

            var expression = input.Expression as ConstantExpression;
            if (expression != null)
            {
                var obj = expression.Value;
                if (obj == null)
                {
                    return input;
                }

                var t = obj.GetType();
                if (!t.GetTypeInfo().IsNestedPrivate || !t.Name.StartsWith("<>"))
                {
                    return input;
                }

                var fi = (FieldInfo)input.Member;
                var result = fi.GetValue(obj);
                var exp = result as Expression;
                if (exp != null)
                {
                    return Visit(exp);
                }
            }

            var propertyInfo = input.Member as PropertyInfo;
            if (field.FieldType.GetTypeInfo().IsSubclassOf(typeof(Expression)) || propertyInfo != null && propertyInfo.PropertyType.GetTypeInfo().IsSubclassOf(typeof(Expression)))
            {
                return Visit(Expression.Lambda<Func<Expression>>(input).Compile()());
            }

            return input;
        }
    }
}
