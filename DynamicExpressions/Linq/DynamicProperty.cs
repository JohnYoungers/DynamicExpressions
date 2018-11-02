using System;

namespace DynamicExpressions.Linq
{
    public class DynamicProperty
    {
        public DynamicProperty(string name, Type type)
        {
            this.Name = name ?? throw new ArgumentNullException("name");
            this.Type = type ?? throw new ArgumentNullException("type");
        }

        public string Name { get; }
        public Type Type { get; }
    }
}
