using DynamicExpressions.Tests.Mapping.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpressions.Tests
{
    [TestClass]
    public class ExpressionBuilderTests
    {
        [TestMethod]
        public void Or()
        {
            List<Expression<Func<Blog, bool>>> expressions = new List<Expression<Func<Blog, bool>>>
            {
                b => b.Name == "A",
                b => b.Id == 2
            };

            Assert.AreEqual("b => ((b.Name == \"A\") OrElse (b.Id == 2))", ExpressionBuilder.Or(expressions).ToString());
        }

        [TestMethod]
        public void And()
        {
            List<Expression<Func<Blog, bool>>> expressions = new List<Expression<Func<Blog, bool>>>
            {
                b => b.Name == "A",
                b => b.Id == 2
            };

            Assert.AreEqual("b => ((b.Name == \"A\") AndAlso (b.Id == 2))", ExpressionBuilder.And(expressions).ToString());
        }
    }
}
