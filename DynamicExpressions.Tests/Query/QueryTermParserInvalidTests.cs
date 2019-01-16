using DynamicExpressions.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Tests.Query
{
    [TestClass]
    public class QueryTermParserInvalidTests
    {
        private void VerifyException(string query, string exceptionMessage)
        {
            var ex = Assert.ThrowsException<ParseException>(() => QueryTermParser.Parse(query));
            Assert.AreEqual(exceptionMessage, ex.Message);
        }

        [TestMethod]
        public void UnterminatedQuote()
        {
            VerifyException("a = \"ABC", "Expression contained an opening quote symbol without a closing quote symbol");
        }

        [TestMethod]
        public void AndOrCombo()
        {
            VerifyException("a = 1 or b = 2 and c = 3", "The term \"a = 1 or b = 2 and c = 3\" contains a mixture of \"And\" and \"Or\": try adding parentheses to clarify the expression");
        }

        [TestMethod]
        public void BetweenSyntax()
        {
            VerifyException("a between 1 or 2", "The term \"a between 1 or 2\" does not appear to be a valid \"between\" expression");
        }

        [TestMethod]
        public void Partial()
        {
            VerifyException("a = ", "The term \"a = \" could not be parsed");
        }

        [TestMethod]
        public void AccurateNestedExceptionMessage()
        {
            VerifyException("a = 2 and (b = )", "The term \"b = \" could not be parsed");
            VerifyException("(a = 2 and (b = 2 and c = ))", "The term \"c = \" could not be parsed");
        }
    }
}
