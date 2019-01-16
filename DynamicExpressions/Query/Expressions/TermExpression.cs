using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Query.Expressions
{
    public class TermExpression : QueryExpression
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
