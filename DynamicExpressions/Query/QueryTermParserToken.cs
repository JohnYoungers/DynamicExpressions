using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Query
{
    public enum QueryTermParserToken
    {
        None,
        Expression,
        Quote,
        Between,
        And,
        Or,
        Equals
    }
}
