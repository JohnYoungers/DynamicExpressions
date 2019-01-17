using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions
{
    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression original;
        private readonly ParameterExpression updated;

        public ParameterReplaceVisitor(ParameterExpression original, ParameterExpression updated)
        {
            this.original = original;
            this.updated = updated;
        }

        protected override Expression VisitParameter(ParameterExpression node) => node == original ? updated : base.VisitParameter(node);
    }
}
