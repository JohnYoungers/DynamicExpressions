using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public static class ExpressionMapExtensions
    {
        public static TTarget Invoke<TSource, TTarget>(this Expression<Func<TSource, TTarget>> map, TSource arg)
        {
            throw new InvalidOperationException($"This method is intended to be used in conjunction with {nameof(Flatten)}");
        }

        public static Expression<Func<TSource, TTarget>> Flatten<TSource, TTarget>(this Expression<Func<TSource, TTarget>> map)
        {
            return (Expression<Func<TSource, TTarget>>)new ExpressionExpansionVisitor().Visit(map);
        }

        public static Expression<Func<TSource, TTargetB>> Concat<TSource, TTargetA, TTargetB>(
            this Expression<Func<TSource, TTargetA>> mapA, Expression<Func<TSource, TTargetB>> mapB)
            where TTargetB : TTargetA
        {
            var param = Expression.Parameter(typeof(TSource), "i");

            return Expression.Lambda<Func<TSource, TTargetB>>(
                Expression.MemberInit(
                    ((MemberInitExpression)mapB.Body).NewExpression,
                    (new LambdaExpression[] { mapA, mapB }).SelectMany(e =>
                    {
                        var bindings = (e.Body as MemberInitExpression ?? throw new ArgumentException("Expected map to be MemberInitExpression")).Bindings.OfType<MemberAssignment>();
                        return bindings.Select(b =>
                        {
                            var paramReplacedExp = new ParameterReplaceVisitor(e.Parameters[0], param).VisitAndConvert(b.Expression, "Combine");
                            return Expression.Bind(b.Member, paramReplacedExp);
                        });
                    })),
                param);
        }
    }
}
