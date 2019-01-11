using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xtml;

namespace Xphter.Framework.Xtml.Tests {
    [TestClass()]
    public class XtmlNumericalAggregateFunctionTests {
        [TestMethod()]
        public void ComputeTest() {
            IXtmlAggregateFunction target = null;
            PropertyInfo p1 = typeof(TestClass).GetProperty("P1");
            PropertyInfo p2 = typeof(TestClass).GetProperty("P2");
            PropertyInfo p3 = typeof(TestClass).GetProperty("P3");
            PropertyInfo p4 = typeof(TestClass).GetProperty("P4");
            PropertyInfo p5 = typeof(TestClass).GetProperty("P5");
            PropertyInfo p6 = typeof(TestClass).GetProperty("P6");

            target = new XtmlSumAggregateFunction();
            Assert.AreEqual(15, target.Compute(null, null, null, p1, new List<object> {
                1, 2, 3, 4, 5,
            }));
            Assert.AreEqual(12, target.Compute(null, null, null, p2, new List<object> {
                1, 2, null, 4, 5,
            }));
            Assert.AreEqual(16.5M, target.Compute(null, null, null, p3, new List<object> {
                1.1M, 2.2M, 3.3M, 4.4M, 5.5M,
            }));
            Assert.AreEqual(13.2M, target.Compute(null, null, null, p4, new List<object> {
                1.1M, 2.2M, null, 4.4M, 5.5M,
            }));
            Assert.AreEqual(16.5F, target.Compute(null, null, null, p5, new List<object> {
                1.1F, 2.2F, 3.3F, 4.4F, 5.5F,
            }));
            Assert.AreEqual(13.2F, target.Compute(null, null, null, p6, new List<object> {
                1.1F, 2.2F, null, 4.4F, 5.5F,
            }));

            target = new XtmlAverageAggregateFunction();
            Assert.AreEqual(3D, target.Compute(null, null, null, p1, new List<object> {
                1, 2, 3, 4, 5,
            }));
            Assert.AreEqual(3D, target.Compute(null, null, null, p2, new List<object> {
                1, 2, null, 4, 5,
            }));
            Assert.AreEqual(3.3M, target.Compute(null, null, null, p3, new List<object> {
                1.1M, 2.2M, 3.3M, 4.4M, 5.5M,
            }));
            Assert.AreEqual(3.3M, target.Compute(null, null, null, p4, new List<object> {
                1.1M, 2.2M, null, 4.4M, 5.5M,
            }));
            Assert.AreEqual(3.3F, target.Compute(null, null, null, p5, new List<object> {
                1.1F, 2.2F, 3.3F, 4.4F, 5.5F,
            }));
            Assert.AreEqual(3.3F, target.Compute(null, null, null, p6, new List<object> {
                1.1F, 2.2F, null, 4.4F, 5.5F,
            }));

            target = new XtmlMaxAggregateFunction();
            Assert.AreEqual(5, target.Compute(null, null, null, p1, new List<object> {
                1, 2, 3, 4, 5,
            }));
            Assert.AreEqual(5, target.Compute(null, null, null, p2, new List<object> {
                1, 2, null, 4, 5,
            }));
            Assert.AreEqual(5.5M, target.Compute(null, null, null, p3, new List<object> {
                1.1M, 2.2M, 3.3M, 4.4M, 5.5M,
            }));
            Assert.AreEqual(5.5M, target.Compute(null, null, null, p4, new List<object> {
                1.1M, 2.2M, null, 4.4M, 5.5M,
            }));
            Assert.AreEqual(5.5F, target.Compute(null, null, null, p5, new List<object> {
                1.1F, 2.2F, 3.3F, 4.4F, 5.5F,
            }));
            Assert.AreEqual(5.5F, target.Compute(null, null, null, p6, new List<object> {
                1.1F, 2.2F, null, 4.4F, 5.5F,
            }));

            target = new XtmlMinAggregateFunction();
            Assert.AreEqual(1, target.Compute(null, null, null, p1, new List<object> {
                1, 2, 3, 4, 5,
            }));
            Assert.AreEqual(1, target.Compute(null, null, null, p2, new List<object> {
                1, 2, null, 4, 5,
            }));
            Assert.AreEqual(1.1M, target.Compute(null, null, null, p3, new List<object> {
                1.1M, 2.2M, 3.3M, 4.4M, 5.5M,
            }));
            Assert.AreEqual(1.1M, target.Compute(null, null, null, p4, new List<object> {
                1.1M, 2.2M, null, 4.4M, 5.5M,
            }));
            Assert.AreEqual(1.1F, target.Compute(null, null, null, p5, new List<object> {
                1.1F, 2.2F, 3.3F, 4.4F, 5.5F,
            }));
            Assert.AreEqual(1.1F, target.Compute(null, null, null, p6, new List<object> {
                1.1F, 2.2F, null, 4.4F, 5.5F,
            }));
        }

        private class TestClass {
            public int P1 {
                get;
                set;
            }

            public int? P2 {
                get;
                set;
            }

            public decimal P3 {
                get;
                set;
            }

            public decimal? P4 {
                get;
                set;
            }

            public float P5 {
                get;
                set;
            }

            public float? P6 {
                get;
                set;
            }
        }
    }
}
