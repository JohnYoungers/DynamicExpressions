using DynamicExpressions.Mapping;
using DynamicExpressions.Tests.Mapping.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Tests.Mapping
{
    [TestClass]
    public class MapperExceptions
    {
        [TestMethod]
        public void ShouldGenerateAndMap()
        {
            var typeDefinition = new DynamicTypeDefinition
            {
                Lists = new List<DynamicListDefinition>
                {
                    new DynamicListDefinition("Id")
                }
            };

            var ex = Assert.ThrowsException<ArgumentException>(() => Mapper.GenerateMappedType<Context, Blog>(typeDefinition));
            Assert.AreEqual("List Id must result in a type that implements IEnumerable<>", ex.Message);
        }
    }
}
