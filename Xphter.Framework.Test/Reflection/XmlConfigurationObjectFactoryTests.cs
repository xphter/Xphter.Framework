using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Reflection.Tests {
    [TestClass()]
    public class XmlConfigurationObjectFactoryTests {
        public const string CONFIG_FILE_NAME = "factory.xml";

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            File.WriteAllText(CONFIG_FILE_NAME, Xphter.Framework.Test.Properties.Resources.ObjectFactory, Encoding.UTF8);
        }

        [TestMethod()]
        public void SuccessTest_WithProxyFactory() {
            MethodFilterProxyTypeFactory proxyFactory = new MethodFilterProxyTypeFactory("Xphter.Framework.Reflection.Tests.Proxies");

            IObjectFactory objectFactory = new XmlConfigurationObjectFactory(CONFIG_FILE_NAME, new Assembly[] {
                Assembly.GetExecutingAssembly(),
            }, proxyFactory);

            try {
                IClassA obj1 = objectFactory.CreateInstance<IClassA>();
                string name1 = obj1.Test(13);

                IClassA obj2 = objectFactory.CreateInstance<IClassA>();
                string name2 = obj1.Test(18);
            } finally {
#if DEBUG
                proxyFactory.SaveAssembly(null);
#endif
            }
        }

        [TestMethod()]
        public void SuccessTest_NoProxyFactory() {
            IObjectFactory objectFactory = new XmlConfigurationObjectFactory(CONFIG_FILE_NAME, new Assembly[] {
                Assembly.GetExecutingAssembly(),
            }, null);

            IClassA obj1 = objectFactory.CreateInstance<IClassA>();
            string name1 = obj1.Test(13);

            IClassA obj2 = objectFactory.CreateInstance<IClassA>();
            string name2 = obj1.Test(18);
        }

        [TestMethod()]
        [ExpectedException(typeof(ObjectFacgoryException))]
        public void CycleTest() {
            MethodFilterProxyTypeFactory proxyFactory = new MethodFilterProxyTypeFactory("Xphter.Framework.Reflection.Tests.Proxies");

            IObjectFactory objectFactory = new XmlConfigurationObjectFactory(CONFIG_FILE_NAME, new Assembly[] {
                Assembly.GetExecutingAssembly(),
            }, proxyFactory);

            objectFactory.CreateInstance<ClassD5>();
        }
    }

    public interface IClassA {
        string Test(int k);
    }

    public class ClassA1 : IClassA {
        public ClassA1(bool p1, char p2, string p3, int p4, float p5, double p6,
            decimal p7, DateTime p8, IClassB p9, IClassC p10, IClassD p11, object p12,
            IEnumerable<object> p13, byte p14, short p15, long p16, EnumForA p17) {
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
            this.P4 = p4;
            this.P5 = p5;
            this.P6 = p6;
            this.P7 = p7;
            this.P8 = p8;
            this.P9 = p9;
            this.P10 = p10;
            this.P11 = p11;
            this.P12 = p12;
            this.P13 = p13;
            this.P14 = p14;
            this.P15 = p15;
            this.P16 = p16;
            this.P17 = p17;
        }

        public bool P1 {
            get;
            set;
        }

        public char P2 {
            get;
            set;
        }

        public string P3 {
            get;
            set;
        }

        public int P4 {
            get;
            set;
        }

        public float P5 {
            get;
            set;
        }

        public double P6 {
            get;
            set;
        }

        public decimal P7 {
            get;
            set;
        }

        public DateTime P8 {
            get;
            set;
        }

        public IClassB P9 {
            get;
            set;
        }

        public IClassC P10 {
            get;
            set;
        }

        public IClassD P11 {
            get;
            set;
        }

        public object P12 {
            get;
            set;
        }

        public IEnumerable<object> P13 {
            get;
            set;
        }

        public byte P14 {
            get;
            set;
        }

        public short P15 {
            get;
            set;
        }

        public long P16 {
            get;
            set;
        }

        public EnumForA P17 {
            get;
            set;
        }

        [MethodFilter1]
        public virtual string Test(int k) {
            for(int i = 0; i < 10; i++) {
                for(int j = 0; j < 10; j++) {
                    ++k;
                }
            }
            return this.GetType().FullName;
        }
    }

    public enum EnumForA {
        None = 0x00,

        Value1 = 0x01,

        Value2 = 0x02,

        Value3 = 0x03,
    }

    public interface IClassB {
    }

    public class ClassB1 : IClassB {
        public ClassB1(bool value) {
            this.P1 = value;
        }

        public bool P1 {
            get;
            set;
        }
    }

    public class ClassB2 : IClassB {
        public ClassB2(IEnumerable<IClassD> p1) {
            this.P1 = p1;
        }

        public IEnumerable<IClassD> P1 {
            get;
            set;
        }
    }

    public interface IClassC {
    }

    public class ClassC1 : IClassC {
    }

    public class ClassC2 : ClassC1 {
        public ClassC2(byte p1) {
            this.P1 = p1;
        }

        public byte P1 {
            get;
            set;
        }
    }

    public class ClassC3 : ClassC2 {
        public ClassC3(byte p1, ulong p2)
            : base(p1) {
            this.P2 = p2;
        }

        public ulong P2 {
            get;
            set;
        }
    }

    public interface IClassD {
    }

    public class ClassD1 : IClassD {
    }

    [MethodFilter1]
    [MethodFilter2]
    public class ClassD2 : IClassD {
    }

    public class ClassD3 : IClassD {
        internal ClassD3() {
        }
    }

    public class ClassD4 : IClassD {
        internal ClassD4(int p1, string p2) {
            this.P1 = p1;
            this.P2 = p2;
        }

        public int P1 {
            get;
            set;
        }

        public string P2 {
            get;
            set;
        }
    }

    public class ClassD5 : IClassD {
        public ClassD5(IEnumerable<IClassC> data) {
        }
    }


    public class MethodFilter1Attribute : MethodFilterAttribute {
        public override void OnPreExecuting(IPreMethodExecutingContext context) {
        }

        public override void OnPostExecuting(IPostMethodExecutingContext context) {
        }
    }

    public class MethodFilter2Attribute : MethodFilterAttribute {
        public override void OnPreExecuting(IPreMethodExecutingContext context) {
        }

        public override void OnPostExecuting(IPostMethodExecutingContext context) {
        }
    }
}
