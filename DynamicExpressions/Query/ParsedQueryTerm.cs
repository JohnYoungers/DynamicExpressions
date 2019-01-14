using System;
using System.Collections.Generic;
using System.Text;
using DynamicExpressions.Query.Expressions;

namespace DynamicExpressions.Query
{
    public class ParsedQueryTerm
    {
        public string Term { get; set; }
        public QueryExpression Expression { get; set; }
    }
}
