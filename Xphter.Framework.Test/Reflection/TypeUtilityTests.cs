using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Reflection.Tests {
    [TestClass()]
    public class TypeUtilityTests {
        [TestMethod()]
        public void IsTest() {
            Assert.IsTrue(TypeUtility.Is(typeof(IEnumerable), typeof(IEnumerable)));
            Assert.IsTrue(TypeUtility.Is(typeof(IEnumerable<>), typeof(IEnumerable)));
            Assert.IsTrue(TypeUtility.Is(typeof(List<>), typeof(IEnumerable)));
            Assert.IsTrue(TypeUtility.Is(typeof(List<int>), typeof(IEnumerable)));
            Assert.IsTrue(TypeUtility.Is(typeof(ArrayList), typeof(IEnumerable)));

            Assert.IsTrue(TypeUtility.Is(typeof(IEnumerable<>), typeof(IEnumerable<>)));
            Assert.IsTrue(TypeUtility.Is(typeof(ICollection<>), typeof(IEnumerable<>)));
            Assert.IsTrue(TypeUtility.Is(typeof(List<>), typeof(IEnumerable<>)));
            Assert.IsTrue(TypeUtility.Is(typeof(List<int>), typeof(IEnumerable<>)));

            Assert.IsTrue(TypeUtility.Is(typeof(IEnumerable<int>), typeof(IEnumerable<int>)));
            Assert.IsTrue(TypeUtility.Is(typeof(ICollection<int>), typeof(IEnumerable<int>)));
            Assert.IsTrue(TypeUtility.Is(typeof(List<int>), typeof(IEnumerable<int>)));

            Assert.IsFalse(TypeUtility.Is(typeof(ICollection<>), typeof(IEnumerable<int>)));
            Assert.IsFalse(TypeUtility.Is(typeof(List<>), typeof(IEnumerable<int>)));
            

            Assert.IsTrue(TypeUtility.Is(typeof(IEnumerable<int>), typeof(IEnumerable)));
            Assert.IsTrue(TypeUtility.Is(typeof(IEnumerable<int>), typeof(IEnumerable<int>)));
            Assert.IsTrue(TypeUtility.Is(typeof(ICollection<int>), typeof(IEnumerable<int>)));

            Assert.IsTrue(TypeUtility.Is(typeof(G1<>), typeof(T0)));
            Assert.IsTrue(TypeUtility.Is(typeof(G1<int>), typeof(T0)));
            Assert.IsTrue(TypeUtility.Is(typeof(G2<>), typeof(T0)));
            Assert.IsTrue(TypeUtility.Is(typeof(G2<int>), typeof(T0)));

            Assert.IsTrue(TypeUtility.Is(typeof(G2<>), typeof(G1<>)));
            Assert.IsTrue(TypeUtility.Is(typeof(G2<int>), typeof(G1<>)));

            Assert.IsTrue(TypeUtility.Is(typeof(G2<int>), typeof(G1<int>)));

            Assert.IsFalse(TypeUtility.Is(typeof(G2<>), typeof(G1<int>)));
        }

        private class T0 {
        }

        private class G1<T> : T0 {
        }

        private class G2<T> : G1<T> {
        }
    }
}
