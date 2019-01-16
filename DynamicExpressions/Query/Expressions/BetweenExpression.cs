using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Query.Expressions
{
    public class BetweenExpression : QueryExpression
    {
        public string Field { get; set; }
        public string ValueFrom { get; set; }
        public string ValueTo { get; set; }

        public override string ToString()
        {
            return $"{Field} Between {ValueFrom} and {ValueTo}";
        }
    }
}
