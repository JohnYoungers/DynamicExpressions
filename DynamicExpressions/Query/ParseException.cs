using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Query
{
    public class ParseException : Exception
    {
        public int Position { get; }

        public ParseException(string message, int position) : base(message)
        {
            Position = position;
        }
    }
}
