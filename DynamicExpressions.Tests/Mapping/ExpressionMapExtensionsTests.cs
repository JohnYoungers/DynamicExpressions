using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DynamicExpressions.Mapping;

namespace DynamicExpressions.Tests.Mapping
{
    [TestClass]
    public class ExpressionMapExtensionsTests
    {
        public class FooEntity
        {
            public string FieldA { get; set; }
            public string Address1 { get; set; }
        }
        public class FooSummaryViewModel
        {
            public string Prop1 { get; set; }

            public static Expression<Func<FooEntity, FooSummaryViewModel>> Map()
            {
                return i => new FooSummaryViewModel { Prop1 = i.FieldA };
            }
        }
        public class FooViewModel : FooSummaryViewModel
        {
            public string Addr { get; set; }

            public new static Expression<Func<FooEntity, FooViewModel>> Map()
            {
                return FooSummaryViewModel.Map().Concat(i => new FooViewModel { Addr = i.Address1 });
            }
        }

        public class FlattenExample
        {
            public FooSummaryViewModel Summary { get; set; }
            public FooViewModel Full { get; set; }

            public static Expression<Func<FooEntity, FlattenExample>> Map()
            {
                var fooSummaryMap = FooSummaryViewModel.Map();
                var fooMap = FooViewModel.Map();

                Expression<Func<FooEntity, FlattenExample>> map = i => new FlattenExample
                {
                    Summary = fooSummaryMap.Invoke(i),
                    Full = fooMap.Invoke(i)
                };

                var x = map.Flatten();
                return x;
            }
        }

        [TestMethod]
        public void Concat()
        {
            var result = FooViewModel.Map().Compile().Invoke(new FooEntity { FieldA = "A", Address1 = "B" });

            Assert.AreEqual("A", result.Prop1);
            Assert.AreEqual("B", result.Addr);
        }

        [TestMethod]
        public void Flatten()
        {
            var result = FlattenExample.Map().Compile().Invoke(new FooEntity { FieldA = "A", Address1 = "B" });

            Assert.AreEqual("A", result.Full.Prop1);
            Assert.AreEqual("A", result.Summary.Prop1);
        }
    }

}
