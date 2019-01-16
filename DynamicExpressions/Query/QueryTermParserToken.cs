using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Query
{
    public enum QueryTermParserToken
    {
        Literal,
        Expression,
        Quote,
        Between,
        And,
        Or,
        Equals,
        LessThan,
        LessThanEquals,
        GreaterThan,
        GreaterThanEquals
    }
}
