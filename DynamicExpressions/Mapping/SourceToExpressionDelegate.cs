using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public delegate Expression SourceToExpressionDelegate(ParameterExpression sourceParam, ParameterExpression entityParam);
}
