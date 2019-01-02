using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public class DynamicTypeDefinition
    {
        public List<DynamicFieldDefinition> Fields { get; set; } = new List<DynamicFieldDefinition>();
        public List<DynamicListDefinition> Lists { get; set; } = new List<DynamicListDefinition>();
    }
}
