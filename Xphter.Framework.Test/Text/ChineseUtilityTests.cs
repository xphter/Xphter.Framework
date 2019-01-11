using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Text;

namespace Xphter.Framework.Test {
    [TestClass()]
    public class ChineseUtilityTests {
        [TestMethod()]
        public void GetPinyinTest() {
            Assert.AreEqual("WoShiXphter SiteBuilderDeZuoZhe：DuPeng", ChineseUtility.GetPinyin("我是Xphter SiteBuilder的作者：杜彭"));
        }

        [TestMethod()]
        public void GetFirstLetterOfPinyin() {
            Assert.AreEqual('a', ChineseUtility.GetFirstLetterOfPinyin('a'));
            Assert.AreEqual('：', ChineseUtility.GetFirstLetterOfPinyin('：'));
            Assert.AreEqual('W', ChineseUtility.GetFirstLetterOfPinyin('我'));
            Assert.AreEqual('P', ChineseUtility.GetFirstLetterOfPinyin('彭'));
        }

        [TestMethod()]
        public void GetFirstLettersOfPinyin() {
            Assert.AreEqual("WSXphter SiteBuilderDZZ：DP", ChineseUtility.GetFirstLettersOfPinyin("我是Xphter SiteBuilder的作者：杜彭"));
        }

        [TestMethod()]
        public void SimplifiedToTraditionalTest() {
            Assert.AreEqual("1a時間b時間", ChineseUtility.SimplifiedToTraditional("1a時間b时间"));
        }

        [TestMethod()]
        public void TraditionalToSimplifiedTest() {
            Assert.AreEqual("1a时间b时间", ChineseUtility.TraditionalToSimplified("1a时间b時間"));
        }
    }
}
