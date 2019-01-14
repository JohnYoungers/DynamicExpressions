using System;
using System.Collections.Generic;
using System.Text;
using DynamicExpressions.Query;
using DynamicExpressions.Query.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicExpressions.Tests.Query
{
    [TestClass]
    public class QueryTermParserValidTests
    {
        [TestMethod]
        public void TermExpression()
        {
            var result = QueryTermParser.Parse("a b c");
            Assert.AreEqual("a b c", result.Term);

            Assert.AreEqual("a b c", (result.Expression as TermExpression).Value);
        }

        [TestMethod]
        public void SimpleComparisons()
        {
            void verify(string query, string field, ComparisonOperation op, string value)
            {
                var result = QueryTermParser.Parse(query);

                var exp = result.Expression as ComparisonExpression;
                Assert.AreEqual(field, exp.Field);
                Assert.AreEqual(op, exp.Operation);
                Assert.AreEqual(value, exp.Value);
            }

            verify("A = 5", "A", ComparisonOperation.Equals, "5");
            verify("A < 5", "A", ComparisonOperation.LessThan, "5");
            verify("A <= 5", "A", ComparisonOperation.LessThanEquals, "5");
            verify("A > 5", "A", ComparisonOperation.GreaterThan, "5");
            verify("A >= 5", "A", ComparisonOperation.GreaterThanEquals, "5");
            verify("a b = c d", "a b", ComparisonOperation.Equals, "c d");
        }

        [TestMethod]
        public void BetweenExpression()
        {
            var result = QueryTermParser.Parse("a b between 1 and 2");

            var exp = result.Expression as BetweenExpression;
            Assert.AreEqual("a b", exp.Field);
            Assert.AreEqual("1", exp.ValueFrom);
            Assert.AreEqual("2", exp.ValueTo);
        }

        [TestMethod]
        public void CompoundExpression()
        {
            var result = QueryTermParser.Parse("a between 1/1/10 and 1/1/11 or c = 2");

            var compoundExpression = result.Expression as CompoundExpression;
            Assert.AreEqual(CompoundOperation.Or, compoundExpression.Operation);
            Assert.AreEqual(2, compoundExpression.Expressions.Count);

            var betweenExpression = compoundExpression.Expressions[0] as BetweenExpression;
            Assert.AreEqual("a", betweenExpression.Field);
            Assert.AreEqual("1/1/10", betweenExpression.ValueFrom);
            Assert.AreEqual("1/1/11", betweenExpression.ValueTo);

            var fieldExpression = compoundExpression.Expressions[1] as ComparisonExpression;
            Assert.AreEqual("c", fieldExpression.Field);
            Assert.AreEqual(ComparisonOperation.Equals, fieldExpression.Operation);
            Assert.AreEqual("2", fieldExpression.Value);
        }

        [TestMethod]
        public void StackedExpression()
        {
            var result = QueryTermParser.Parse("(c = 3 and (a = 1 or b = 2)) or d = 4");

            var level0CompoundExpression = result.Expression as CompoundExpression;
            Assert.AreEqual(CompoundOperation.Or, level0CompoundExpression.Operation);
            Assert.AreEqual(2, level0CompoundExpression.Expressions.Count);
            Assert.AreEqual("d Equals 4", level0CompoundExpression.Expressions[1].ToString());

            var level1CompoundExpression = level0CompoundExpression.Expressions[0] as CompoundExpression;
            Assert.AreEqual(CompoundOperation.And, level0CompoundExpression.Operation);
            Assert.AreEqual(2, level1CompoundExpression.Expressions.Count);
            Assert.AreEqual("c Equals 3", level1CompoundExpression.Expressions[0].ToString());

            var level2CompoundExpression = level1CompoundExpression.Expressions[1] as CompoundExpression;
            Assert.AreEqual(CompoundOperation.Or, level0CompoundExpression.Operation);
            Assert.AreEqual(2, level2CompoundExpression.Expressions.Count);

            var level2Exp1 = level2CompoundExpression.Expressions[0] as ComparisonExpression;
            Assert.AreEqual("a", level2Exp1.Field);
            Assert.AreEqual(ComparisonOperation.Equals, level2Exp1.Operation);
            Assert.AreEqual("1", level2Exp1.Value);


            var level2Exp2 = level2CompoundExpression.Expressions[1] as ComparisonExpression;
            Assert.AreEqual("b", level2Exp2.Field);
            Assert.AreEqual(ComparisonOperation.Equals, level2Exp2.Operation);
            Assert.AreEqual("2", level2Exp2.Value);
        }
    }

}
