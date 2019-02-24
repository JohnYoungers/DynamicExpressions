using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Tests.Linq.Models
{
    public class ComplexChildEntity
    {
        public int Index { get; set; }
        public int Level { get; set; }
        public string Value { get; set; }

        public List<int> SubChildren { get; set; }
    }
}
