using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public class DynamicFieldDefinition
    {
        public string Name { get; private set; }
        public string Expression { get; private set; }
        public SourceToExpressionDelegate ExpressionGenerator { get; set; }

        public DynamicFieldDefinition(string expression) : this(expression, expression) { }
        public DynamicFieldDefinition(string expression, string name)
        {
            Expression = expression;
            Name = name;
        }
    }
}
