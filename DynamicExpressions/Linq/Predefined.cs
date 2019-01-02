using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Linq
{
    public static class Predefined
    {
        /// <summary>
        /// List of known types for use in dynamic linq queries.  Add or remove types as needed.
        /// i.e. `Predefined.Types.Add(typeof(System.Data.Entity.DbFunctions))
        /// </summary>
        public static List<Type> Types { get; private set; } = new List<Type>
        {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };

        public static List<string> GenericMethods { get; private set; } = new List<string>
        {
            "Min",
            "Max",
            "Select",
            "OrderBy",
            "OrderByDescending",
            "ThenBy",
            "ThenByDescending"
        };
    }
}
