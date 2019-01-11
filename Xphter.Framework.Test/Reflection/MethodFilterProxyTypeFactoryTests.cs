using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Reflection.Tests {
    [TestClass()]
    public class MethodFilterProxyTypeFactoryTests {
        [TestMethod()]
        public void CreateProxyTypeTest() {
            MethodFilterProxyTypeFactory proxyFactory = new MethodFilterProxyTypeFactory("Xphter.Framework.Reflection.Tests.Proxies");

            try {
                TestClass3<double> obj1 = new TestClass3<double>();
                TestClass3<float> obj2 = (TestClass3<float>) Activator.CreateInstance(proxyFactory.CreateProxyType(typeof(TestClass3<>)).MakeGenericType(typeof(float)));
                TestClass3<double> obj3 = (TestClass3<double>) Activator.CreateInstance(proxyFactory.CreateProxyType(typeof(TestClass3<double>)));

                Assert.AreEqual(obj1.TestMethod1(), obj2.TestMethod1());
                Assert.AreEqual(obj1.TestMethod1(13), obj2.TestMethod1(13));
                Assert.AreEqual(obj1.TestMethod3<Attribute, ProgressChangedEventArgs, ArrayList>(null, null, null), obj2.TestMethod3<Attribute, ProgressChangedEventArgs, ArrayList>(null, null, null));

                Assert.AreEqual(obj1.TestMethod1(), obj3.TestMethod1());
                Assert.AreEqual(obj1.TestMethod1(13), obj3.TestMethod1(13));
                Assert.AreEqual(obj1.TestMethod3<Attribute, ProgressChangedEventArgs, ArrayList>(null, null, null), obj3.TestMethod3<Attribute, ProgressChangedEventArgs, ArrayList>(null, null, null));

                obj1.TestProperty1 = 13;
                obj2.TestProperty1 = 13;
                obj3.TestProperty1 = 13;
                Assert.AreEqual(obj1.TestProperty1, obj2.TestProperty1);
                Assert.AreEqual(obj1.TestProperty1, obj3.TestProperty1);
            } finally {
#if DEBUG
                proxyFactory.SaveAssembly(null);
#endif
            }
        }

        [MethodFilter1]
        public class TestClass1<Z> {
            [MethodFilter1]
            protected virtual int InternalTestMethod1() {
                return 1;
            }

            [MethodFilter1]
            public virtual int TestMethod1() {
                return this.InternalTestMethod1();
            }

            [MethodFilter1]
            public virtual int TestMethod1(int value) {
                return value;
            }

            [FilterIgnore]
            [MethodFilter1]
            public virtual void TestMethod2() {
            }

            [MethodFilter1]
            public virtual int TestMethod3() {
                return this.GetHashCode();
            }

            public virtual Z TestMethod4() {
                return default(Z);
            }

            [MethodFilter1]
            public virtual int TestMethod3<T, U, V>(T p1, U p2, V p3)
                where T : class
                where U : EventArgs
                where V : IEnumerable, ICollection {
                return typeof(T).GetHashCode() + typeof(U).GetHashCode() + typeof(V).GetHashCode();
            }

            private int m_testProperty1;

            [MethodFilter1]
            public virtual int TestProperty1 {
                get {
                    return this.m_testProperty1;
                }
                set {
                    this.m_testProperty1 = value;
                }
            }

            [MethodFilter1(PropertyMethods = MethodFilterActOnPropertyMethods.SetMethod)]
            public virtual int TestProperty2 {
                get;
                protected set;
            }

            [MethodFilter1]
            protected virtual int TestProperty3 {
                get;
                private set;
            }

            [MethodFilter1(PropertyMethods = MethodFilterActOnPropertyMethods.GetMethod)]
            public virtual Z TestProperty4 {
                get;
                private set;
            }
        }

        public class TestClass2<Z> : TestClass1<Z> {
            public override int TestMethod1() {
                return this.InternalTestMethod1();
            }

            public override int TestMethod1(int value) {
                return value * 2;
            }
        }

        public class TestClass3<Z> : TestClass2<Z> {
            protected override int InternalTestMethod1() {
                return 13;
            }

            public override int TestMethod1() {
                return this.InternalTestMethod1() * 2;
            }

            public override int TestMethod1(int value) {
                return value * 3;
            }

            public override int TestMethod3() {
                return base.TestMethod3();
            }

            public override int TestMethod3<T, U, V>(T p1, U p2, V p3) {
                return base.TestMethod3<T, U, V>(p1, p2, p3);
            }

            public override Z TestMethod4() {
                return base.TestMethod4();
            }

            public override int TestProperty1 {
                get {
                    return base.TestProperty1;
                }
                set {
                    base.TestProperty1 = value;
                }
            }
        }
    }
}
