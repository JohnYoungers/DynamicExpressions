using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DynamicExpressions.Mapping
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

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(ExpressionMapExtensions)
                && node.Method.Name == "Invoke"
                && node.Arguments[0] is MemberExpression me
                && me.Expression is ConstantExpression ce)
            {
                var result = ((FieldInfo)me.Member).GetValue(ce.Value);

                var lambda = Visit(result as Expression) as LambdaExpression;

                var fe = new Dictionary<ParameterExpression, Expression>(FlattenedExpressions);
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    fe.Add(lambda.Parameters[i], Visit(node.Arguments[i + 1]));
                }

                return new ExpressionExpansionVisitor(fe).Visit(lambda.Body);
            }
            else
            {
                return base.VisitMethodCall(node);
            }
        }
    }
}
