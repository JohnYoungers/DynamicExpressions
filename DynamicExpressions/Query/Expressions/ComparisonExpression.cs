using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Query.Expressions
{
    public class ComparisonExpression : TermExpression
    {
        public ComparisonExpression Operation { get; set; }
        public string Field { get; set; }

        public override string ToString()
        {
            return $"{Field} {Operation} {base.ToString()}";
        }
    }
}
