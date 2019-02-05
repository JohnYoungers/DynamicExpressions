using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DynamicExpressions.Mapping;
using DynamicExpressions.Tests.Mapping.Models;

namespace DynamicExpressions.Tests.Mapping
{
    [TestClass]
    public class ExpressionMapExtensionsTests
    {
        [TestMethod]
        public void Concat()
        {
            var result = FooViewModel.Map().Compile().Invoke(new FooEntity { FieldA = "A", Address1 = "B" });

            Assert.AreEqual("A", result.Prop1);
            Assert.AreEqual("B", result.Addr);
        }

        [TestMethod]
        public void Flatten_InvalidInvoke()
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(() => FooViewModel.Map().Invoke(new FooEntity { FieldA = "A", Address1 = "B" }));
            Assert.AreEqual("This method is intended to be used in conjunction with Flatten", ex.Message);
        }

        [TestMethod]
        public void Flatten()
        {
            var map = FlattenExample.Map();
            var result = map.Compile().Invoke(new FooEntity { FieldA = "A", Address1 = "B" });

            // Verify map worked
            Assert.AreEqual("A", result.Summary.Prop1);
            Assert.AreEqual("B", result.Full.Addr);

            // Verify map was flattened
            Assert.AreEqual("i => new FlattenExample() {Summary = new FooSummaryViewModel() {Prop1 = i.FieldA}, Full = new FooViewModel() {Prop1 = i.FieldA, Addr = i.Address1}}",
                            map.ToString());
        }
    }

}
