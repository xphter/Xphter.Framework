using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Test {
    [TestClass()]
    public class MethodUtilityTests {
        [TestMethod()]
        public void GetCallingMethodTest() {
            MethodBase method = new A().FA();
            Assert.IsNotNull(method);
            Assert.IsTrue(typeof(B).IsAssignableFrom(method.ReflectedType));
            Assert.AreEqual<string>(method.Name, TypeUtility.GetMemberName<B, MethodBase>((obj) => obj.FB()));
        }

        public class A {
            public MethodBase FA() {
                return new B().FB();
            }
        }

        public class B {
            public MethodBase FB() {
                return new C().FC();
            }
        }

        public class C {
            public MethodBase FC() {
                return MethodUtility.GetCallingMethod();
            }
        }
    }
}
