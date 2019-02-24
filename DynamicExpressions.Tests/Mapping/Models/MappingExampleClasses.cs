using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DynamicExpressions.Mapping;

namespace DynamicExpressions.Tests.Mapping.Models
{
    public class FooEntity
    {
        public string FieldA { get; set; }
        public string Address1 { get; set; }
    }
    public class FooSummaryViewModel
    {
        public string Prop1 { get; set; }

        // Using as field
        public static Expression<Func<FooEntity, FooSummaryViewModel>> Map = i => new FooSummaryViewModel
        {
            Prop1 = i.FieldA
        };
    }

    public class FooViewModel : FooSummaryViewModel
    {
        public string Addr { get; set; }

        // Concat + using as property
        public new static Expression<Func<FooEntity, FooViewModel>> Map()
        {
            return FooSummaryViewModel.Map.Concat(i => new FooViewModel { Addr = i.Address1 });
        }
    }

    public class FlattenExample
    {
        public FooSummaryViewModel Summary { get; set; }
        public FooViewModel Full { get; set; }

        public static Expression<Func<FooEntity, FlattenExample>> Map()
        {
            var fooMap = FooViewModel.Map();

            Expression<Func<FooEntity, FlattenExample>> map = i => new FlattenExample
            {
                Summary = FooSummaryViewModel.Map.Invoke(i),
                Full = fooMap.Invoke(i)
            };

            var x = map.Flatten();
            return x;
        }
    }
}
