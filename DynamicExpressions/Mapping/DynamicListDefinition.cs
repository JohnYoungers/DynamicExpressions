using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public class DynamicListDefinition : DynamicTypeDefinition
    {
        public string Name { get; private set; }
        public string Expression { get; private set; }
        public SourceToExpressionDelegate ExpressionGenerator { get; set; }

        public DynamicListDefinition(string expression) : this(expression, expression) { }
        public DynamicListDefinition(string expression, string name)
        {
            Expression = expression;
            Name = name;
        }
    }
}
