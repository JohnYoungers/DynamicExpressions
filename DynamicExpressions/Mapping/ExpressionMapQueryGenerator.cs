using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions.Mapping
{
    /// <summary>
    /// Untested until Query pieces are complete
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public class ExpressionMapQueryGenerator<TSource, TTarget>
    {
        public Expression<Func<TSource, TTarget>> Map { get; set; }
        public ParameterExpression ExpressionParameter { get; set; }
        public List<Expression> QueryBindings { get; set; }

        public string Term { get; private set; }
        public DateTime? AsDateTime { get; private set; }
        public Guid? AsGuid { get; private set; }
        public bool? AsBool { get; private set; }
        public decimal? AsDecimal { get; private set; }
        public long? AsLong { get; private set; }
        public int? AsInt { get; private set; }
        public short? AsShort { get; private set; }

        public ExpressionMapQueryGenerator(Expression<Func<TSource, TTarget>> map)
        {
            Map = map;
            QueryBindings = new List<Expression>();
            ExpressionParameter = Expression.Parameter(typeof(TSource), "i");
        }

        public ExpressionMapQueryGenerator(Expression<Func<TSource, TTarget>> map, string query) : this(map)
        {
            ParseQuery(query);
            BuildBindingFilters();
        }

        void AddQueryBinding(Expression exp)
        {
            QueryBindings.Add(new ParameterReplaceVisitor(Map.Parameters[0], ExpressionParameter).VisitAndConvert(exp, "Combine"));
        }

        public Expression<Func<TSource, bool>> ToFilter()
        {
            return Expression.Lambda<Func<TSource, bool>>(
                QueryBindings.Skip(1).Aggregate(QueryBindings[0], Expression.OrElse),
                ExpressionParameter
            );
        }

        private void ParseQuery(string query)
        {
            Term = query;
            AsBool = string.Equals(query, "yes", StringComparison.CurrentCultureIgnoreCase) || string.Equals(query, "true", StringComparison.CurrentCultureIgnoreCase)
                ? true
                : (string.Equals(query, "no", StringComparison.CurrentCultureIgnoreCase) || string.Equals(query, "false", StringComparison.CurrentCultureIgnoreCase)
                    ? (bool?)false
                    : null);

            AsDateTime = DateTime.TryParse(query, out var d) ? (DateTime?)d : null;
            AsGuid = Guid.TryParse(query, out var g) ? (Guid?)g : null;

            if (decimal.TryParse(query, out var m))
            {
                AsDecimal = m;
                if (long.TryParse(query, out var l))
                {
                    AsLong = l;
                    if (int.TryParse(query, out var i))
                    {
                        AsInt = i;
                        if (short.TryParse(query, out var s))
                        {
                            AsShort = s;
                        }
                    }
                }
            }
        }

        private void BuildBindingFilters()
        {
            var expr = Map.Body as MemberInitExpression ?? throw new ArgumentException("Expected map to be MemberInitExpression");

            void AddEqualsExpression(MemberAssignment binding, object value)
            {
                var valueType = value.GetType();
                var targetValue = binding.Expression.Type.IsAssignableFrom(valueType) ? value : Convert.ChangeType(value, binding.Expression.Type);

                AddQueryBinding(Expression.Equal(Expression.Constant(targetValue, binding.Expression.Type), binding.Expression));
            }

            foreach (var binding in expr.Bindings.OfType<MemberAssignment>())
            {
                var expressionResultType = Nullable.GetUnderlyingType(binding.Expression.Type) ?? binding.Expression.Type;

                if (AsDateTime != null && expressionResultType == typeof(DateTime))
                {
                    AddEqualsExpression(binding, AsDateTime);
                }
                else if (AsGuid != null && expressionResultType == typeof(Guid))
                {
                    AddEqualsExpression(binding, AsGuid);
                }
                else if (AsBool != null && expressionResultType == typeof(bool))
                {
                    AddEqualsExpression(binding, AsBool);
                }
                else if (AsDecimal != null && expressionResultType == typeof(decimal))
                {
                    AddEqualsExpression(binding, AsDecimal);
                }
                else if (AsLong != null && expressionResultType == typeof(long))
                {
                    AddEqualsExpression(binding, AsLong);
                }
                else if (AsInt != null && expressionResultType == typeof(int))
                {
                    AddEqualsExpression(binding, AsInt);
                }
                else if (AsShort != null && expressionResultType == typeof(short))
                {
                    AddEqualsExpression(binding, AsShort);
                }
                else if (Term != null && expressionResultType == typeof(string))
                {
                    var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    AddQueryBinding(Expression.Call(binding.Expression, method, Expression.Constant(Term)));
                }
            }
        }
    }
}
