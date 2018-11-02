using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions.Mapping
{
    public class MappedType<TSource, TEntity>
    {
        public Type GeneratedType { get; set; }
        public Func<TSource, Expression<Func<TEntity, object>>> Map { get; set; }
    }
}
