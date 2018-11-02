using DynamicExpressions.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpressions.Tests.Linq
{
    [TestClass]
    public class WhereTests : LinqTestBase
    {
        [TestMethod]
        public void Simple()
        {
            var match = mockData.Where("Letter == 'A'").ToList();

            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(0, match[0].Counter);
        }

        [TestMethod]
        public void SubProperty()
        {
            var match = mockData.Where("SubProperty.IsOdd == true").ToList();

            Assert.AreEqual(5, match.Count);
            Assert.AreEqual(true, match[0].SubProperty.IsOdd);
        }

        [TestMethod]
        public void InList()
        {
            var letters = new[] { 'A', 'B', 'C' };
            var match = mockData.Where($"Letter in ('{String.Join("','", letters)}')").ToList();

            Assert.AreEqual(3, match.Count);
            Assert.IsTrue(letters.Contains(match[0].Letter));
        }

        [TestMethod]
        public void WithFilter()
        {
            var letter = 'B';
            var match = mockData.Where("Letter == _LetterVariable", new Dictionary<string, object>() { { "_LetterVariable", letter } }).ToList();

            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(letter, match[0].Letter);
        }
    }
}
