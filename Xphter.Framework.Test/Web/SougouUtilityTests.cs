using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Web.Tests {
    [TestClass()]
    public class SougouUtilityTests {
        [TestMethod()]
        public void GetIncludeInfoTest() {
            ISougouIncludeInfo info1 = SougouUtility.GetIncludeInfo("www.beianm.com");
            ISougouIncludeInfo info2 = SougouUtility.GetIncludeInfo("www.xphter.com");
            ISougouIncludeInfo info3 = SougouUtility.GetIncludeInfo("news.qq.com");

            Assert.IsTrue(info1.IndexCount > 0);
            Assert.IsTrue(info1.IncludeCount > 0);
            Assert.IsTrue(info2.IndexCount > 0);
            Assert.IsTrue(info2.IncludeCount > 0);
            Assert.IsTrue(info3.IndexCount > 0);
            Assert.IsTrue(info3.IncludeCount > 0);
        }

        [TestMethod()]
        public void BeginGetIncludeInfoTest() {
            ISougouIncludeInfo info1 = null, info2 = null, info3 = null;

            using(CountdownEvent ce = new CountdownEvent(3)) {
                SougouUtility.BeginGetIncludeInfo("www.beianm.com", (ar) => {
                    try {
                        info1 = SougouUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                SougouUtility.BeginGetIncludeInfo("www.xphter.com", (ar) => {
                    try {
                        info2 = SougouUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                SougouUtility.BeginGetIncludeInfo("news.qq.com", (ar) => {
                    try {
                        info3 = SougouUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                ce.Wait();
            }

            Assert.IsTrue(info1.IndexCount > 0);
            Assert.IsTrue(info1.IncludeCount > 0);
            Assert.IsTrue(info2.IndexCount > 0);
            Assert.IsTrue(info2.IncludeCount > 0);
            Assert.IsTrue(info3.IndexCount > 0);
            Assert.IsTrue(info3.IncludeCount > 0);
        }

        [TestMethod()]
        public void GetKeywordRankTest() {
            IEnumerable<int> rank1 = SougouUtility.GetKeywordRank("寻美图", "www.xunmeitu.com");
            IEnumerable<int> rank2 = SougouUtility.GetKeywordRank("腾讯网", "www.xphter.com");

            Assert.IsTrue(rank1.Count() > 0);
            Assert.IsTrue(rank2.Count() == 0);
        }

        [TestMethod()]
        public void BeginGetKeywordRankTest() {
            IEnumerable<int> rank1 = null, rank2 = null;

            using(CountdownEvent ce = new CountdownEvent(2)) {
                SougouUtility.BeginGetKeywordRank("备案查询", "www.beianm.com", (ar) => {
                    try {
                        rank1 = SougouUtility.EndGetKeywordRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                SougouUtility.BeginGetKeywordRank("腾讯网", "www.xphter.com", (ar) => {
                    try {
                        rank2 = SougouUtility.EndGetKeywordRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                ce.Wait();
            }

            Assert.IsTrue(rank1.Count() > 0);
            Assert.IsTrue(rank2.Count() == 0);
        }
    }
}
