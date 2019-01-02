using DynamicExpressions.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpressions.Tests.Linq
{
    [TestClass]
    public class SelectTests : LinqTestBase
    {
        [TestMethod]
        public void Simple()
        {
            var results = mockData.Select("Letter").Cast<char>().ToList();

            Assert.AreEqual('A', results[0]);
        }

        [TestMethod]
        public void Ternary()
        {
            var results = mockData.Select("Letter = 'A' ? \"Yes\" : \"No\"").Cast<string>().ToList();

            Assert.AreEqual("Yes", results[0]);
            Assert.AreEqual("No", results[1]);
        }

        [TestMethod]
        public void Coalesce()
        {
            mockData[1].NullableLetter = 'Z';
            var results = mockData.Select("NullableLetter ?? Letter").Cast<char>().ToList();

            Assert.AreEqual('A', results[0]);
            Assert.AreEqual('Z', results[1]);
        }

        [TestMethod]
        public void Any()
        {
            var results = mockData.Select("ComplexProperty.Any(Index == 91)").Cast<bool>().ToList();

            Assert.AreEqual(false, results[0]);
            Assert.AreEqual(true, results[1]);
        }

        [TestMethod]
        public void Count()
        {
            var results = mockData.Select("ComplexProperty.Count(Index == 91)").Cast<int>().ToList();

            Assert.AreEqual(0, results[0]);
            Assert.AreEqual(1, results[1]);
        }
    }
}
