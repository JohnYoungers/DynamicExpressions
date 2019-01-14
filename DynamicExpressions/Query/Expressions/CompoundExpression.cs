using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpressions.Query.Expressions
{
    public class CompoundExpression : QueryExpression
    {
        public CompoundOperation Operation { get; set; }
        public List<QueryExpression> Expressions { get; } = new List<QueryExpression>();

        public override string ToString()
        {
            return string.Join($" {Operation} ", Expressions.Select(e => e.ToString()));
        }
    }
}
