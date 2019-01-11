using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Tests {
    [TestClass()]
    public class IcpNumberTests {
        [TestMethod()]
        public void IsWellFormedIcpNumberStringTest() {
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("粤A2-20044005号   \r\n"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("辽B-2-4-20080039"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("\t 粤B2-20090059  \r\n\t"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("蒙B2-20090059号"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("京B2-20090059-13"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("沪B2-20090059号-13"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("皖B1.B2-20070020 "));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("\t 津ICP备11041704  \r\n\t"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("冀ICP备11041704号"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("晋ICP备11041704-13"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("辽ICP备11041704号-13"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("\t 吉ICP证000007  \r\n\t"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("黑ICP证000007号"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("苏ICP证000007-13"));
            Assert.IsTrue(IcpNumber.IsWellFormedIcpNumberString("浙ICP证000007号-13"));

            Assert.IsFalse(IcpNumber.IsWellFormedIcpNumberString("我B2-20090059"));
            Assert.IsFalse(IcpNumber.IsWellFormedIcpNumberString("粤B1-20090059"));
            Assert.IsFalse(IcpNumber.IsWellFormedIcpNumberString("津ICP11041704"));
            Assert.IsFalse(IcpNumber.IsWellFormedIcpNumberString("浙ICP000007号-13"));
        }

        [TestMethod()]
        public void IcpNumberTest() {
            IcpNumber number = null;

            number = new IcpNumber("\t 粤A2-20044005  \r\n\t");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("粤A2-20044005号", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("粤A2-20044005-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("粤A2-20044005号", number.SubjectNumber);
            Assert.AreEqual("粤A2-20044005号-13", number.SiteNumber);
            number = new IcpNumber("粤A2-20044005号");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("粤A2-20044005号", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("粤A2-20044005号-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("粤A2-20044005号", number.SubjectNumber);
            Assert.AreEqual("粤A2-20044005号-13", number.SiteNumber);

            number = new IcpNumber("\t 辽B-2-4-20080039  \r\n\t");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("辽B-2-4-20080039", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("辽B-2-4-20080039-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("辽B-2-4-20080039", number.SubjectNumber);
            Assert.AreEqual("辽B-2-4-20080039-13", number.SiteNumber);
            number = new IcpNumber("辽B-2-4-20080039号");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("辽B-2-4-20080039", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("辽B-2-4-20080039号-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("辽B-2-4-20080039", number.SubjectNumber);
            Assert.AreEqual("辽B-2-4-20080039-13", number.SiteNumber);

            number = new IcpNumber("\t 粤B2-20090059  \r\n\t");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("粤B2-20090059", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("粤B2-20090059-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("粤B2-20090059", number.SubjectNumber);
            Assert.AreEqual("粤B2-20090059-13", number.SiteNumber);
            number = new IcpNumber("粤B2-20090059号");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("粤B2-20090059", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("粤B2-20090059号-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("粤B2-20090059", number.SubjectNumber);
            Assert.AreEqual("粤B2-20090059-13", number.SiteNumber);

            number = new IcpNumber("\t 皖B1.B2-20070020   \r\n\t");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("皖B1.B2-20070020", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("皖B1.B2-20070020-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("皖B1.B2-20070020", number.SubjectNumber);
            Assert.AreEqual("皖B1.B2-20070020-13", number.SiteNumber);
            number = new IcpNumber("皖B1.B2-20070020号");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("皖B1.B2-20070020", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("皖B1.B2-20070020号-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("皖B1.B2-20070020", number.SubjectNumber);
            Assert.AreEqual("皖B1.B2-20070020-13", number.SiteNumber);

            number = new IcpNumber("\t 京ICP备11041704  \r\n\t");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("京ICP备11041704号", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("京ICP备11041704-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("京ICP备11041704号", number.SubjectNumber);
            Assert.AreEqual("京ICP备11041704号-13", number.SiteNumber);
            number = new IcpNumber("京ICP备11041704号");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("京ICP备11041704号", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("京ICP备11041704号-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("京ICP备11041704号", number.SubjectNumber);
            Assert.AreEqual("京ICP备11041704号-13", number.SiteNumber);

            number = new IcpNumber("\t 京ICP证000007  \r\n\t");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("京ICP证000007号", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("京ICP证000007-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("京ICP证000007号", number.SubjectNumber);
            Assert.AreEqual("京ICP证000007号-13", number.SiteNumber);
            number = new IcpNumber("京ICP证000007号");
            Assert.AreEqual(IcpNumberType.SubjectNumber, number.NumberType);
            Assert.AreEqual("京ICP证000007号", number.SubjectNumber);
            Assert.IsNull(number.SiteNumber);
            number = new IcpNumber("京ICP证000007号-13");
            Assert.AreEqual(IcpNumberType.SiteNumber, number.NumberType);
            Assert.AreEqual("京ICP证000007号", number.SubjectNumber);
            Assert.AreEqual("京ICP证000007号-13", number.SiteNumber);
        }

        [TestMethod()]
        public void FromPinyinTest() {
            Assert.AreEqual("藏ICP备1234号", IcpNumber.FromPinyin("CANGICPBEI1234HAO"));
            Assert.AreEqual("藏ICP备1234号", IcpNumber.FromPinyin("ZANGICPBEI1234HAO"));
            Assert.AreEqual("京ICP证030173号", IcpNumber.FromPinyin("JINGICPZHENG030173HAO"));
            Assert.AreEqual("浙B2-20080224", IcpNumber.FromPinyin("ZHEB2-20080224"));
        }

        [TestMethod()]
        public void ToPinyinTest() {
            Assert.AreEqual("ZANGICPBEI1234HAO", IcpNumber.ToPinyin("藏ICP备1234号"));
            Assert.AreEqual("JINGICPZHENG030173HAO", IcpNumber.ToPinyin("京ICP证030173号"));
            Assert.AreEqual("ZHEB2-20080224", IcpNumber.ToPinyin("浙B2-20080224"));
        }
    }
}
