using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Tests {
    [TestClass()]
    public class OperationExpressionCalculatorTests {
        [ClassInitialize]
        public static void Initialize(TestContext context) {
            ObjectManager<IOperationRule>.Instance.Register(typeof(OperationExpressionCalculator).Assembly);
        }

        [TestMethod()]
        public void ComputeTest() {
            OperationExpressionCalculator target = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
            Assert.AreEqual(false, target.Compute("!true"));

            Assert.AreEqual(2L, target.Compute("+2"));
            Assert.AreEqual(2.1M, target.Compute("+2.1"));

            Assert.AreEqual(-2L, target.Compute("-2"));
            Assert.AreEqual(-2.1M, target.Compute("-2.1"));

            Assert.AreEqual(3L, target.Compute("1 + 2"));
            Assert.AreEqual(3.3M, target.Compute("1.1 + 2.2"));

            Assert.AreEqual(-1L, target.Compute("1 - 2"));
            Assert.AreEqual(-1.1M, target.Compute("1.1 - 2.2"));

            Assert.AreEqual(6L, target.Compute("2 * 3"));
            Assert.AreEqual(7.26M, target.Compute("2.2 * 3.3"));

            Assert.AreEqual(2.5M, target.Compute("5 / 2"));
            Assert.AreEqual(2.6M, target.Compute("5.2 / 2"));

            Assert.AreEqual(1L, target.Compute("5 % 2"));

            Assert.AreEqual(false, target.Compute("1 > 2"));
            Assert.AreEqual(true, target.Compute("5.2 > 2"));

            Assert.AreEqual(false, target.Compute("2 < 1"));
            Assert.AreEqual(true, target.Compute("2 < 5.2"));

            Assert.AreEqual(false, target.Compute("1 >= 2"));
            Assert.AreEqual(true, target.Compute("5.2 >= 2"));

            Assert.AreEqual(false, target.Compute("2 <= 1"));
            Assert.AreEqual(true, target.Compute("2 <= 5.2"));

            Assert.AreEqual(false, target.Compute("2.1 = 1"));
            Assert.AreEqual(true, target.Compute("1 = 1"));
            Assert.AreEqual(false, target.Compute("\"2\" = 2"));
            Assert.AreEqual(false, target.Compute("\"ab\" = 2"));
            Assert.AreEqual(true, target.Compute("\"ab\" = \"AB\""));

            Assert.AreEqual(true, target.Compute("2.1 != 1"));
            Assert.AreEqual(false, target.Compute("1 != 1"));
            Assert.AreEqual(true, target.Compute("\"2\" != 2"));
            Assert.AreEqual(true, target.Compute("\"ab\" != 2"));
            Assert.AreEqual(false, target.Compute("\"ab\" != \"AB\""));

            Assert.AreEqual(false, target.Compute("2.1 > 1 && 2 > 3"));
            Assert.AreEqual(true, target.Compute("2.1 > 1 || 2 > 3"));
            Assert.AreEqual(true, target.Compute(" !(1 + -2 * +3 > 5.2 / (1 + 1)) && \"a\" != 3 && 1.2 + 3.4 = 4.6  "));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationExpressionException))]
        public void ComputeTest_InvalidCharacter() {
            OperationExpressionCalculator target = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
            target.Compute("1 + 2 b 3 - 1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationExpressionException))]
        public void ComputeTest_MissingBracket() {
            OperationExpressionCalculator target = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
            target.Compute("3.2 * (1.3 + 2 + (7 / 3)");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationExpressionException))]
        public void ComputeTest_MissingOperand() {
            OperationExpressionCalculator target = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
            target.Compute("3 * (4 + 5) > 2 &&");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationExpressionException))]
        public void ComputeTest_InvalidOperand() {
            OperationExpressionCalculator target = new OperationExpressionCalculator(ObjectManager<IOperationRule>.Instance.Objects);
            target.Compute("8 + !3");
        }
    }
}
