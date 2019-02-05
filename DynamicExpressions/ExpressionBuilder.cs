using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<TSource, bool>> Or<TSource>(IEnumerable<Expression<Func<TSource, bool>>> expressions)
        {
            return Or(expressions?.ToArray());
        }
        public static Expression<Func<TSource, bool>> Or<TSource>(params Expression<Func<TSource, bool>>[] expressions)
        {
            return Combine(Expression.OrElse, expressions);
        }

        public static Expression<Func<TSource, bool>> And<TSource>(IEnumerable<Expression<Func<TSource, bool>>> expressions)
        {
            return And(expressions?.ToArray());
        }
        public static Expression<Func<TSource, bool>> And<TSource>(params Expression<Func<TSource, bool>>[] expressions)
        {
            return Combine(Expression.AndAlso, expressions);
        }

        private static Expression<Func<TSource, bool>> Combine<TSource>(Func<Expression, Expression, BinaryExpression> combiner, params Expression<Func<TSource, bool>>[] expressions)
        {
            return expressions == null || expressions.Length == 0
                ? i => true
                : expressions.Skip(1).Aggregate(expressions[0], (acc, exp) =>
                {
                    return Expression.Lambda<Func<TSource, bool>>(
                        combiner(acc.Body, new ParameterReplaceVisitor(exp.Parameters[0], acc.Parameters[0]).Visit(exp.Body)),
                        acc.Parameters);
                });
        }
    }
}
