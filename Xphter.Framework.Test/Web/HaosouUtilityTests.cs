using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Web.Tests {
    [TestClass()]
    public class HaosouUtilityTests {
        [TestMethod()]
        public void GetIncludeInfoTest() {
            IHaosouIncludeInfo info1 = HaosouUtility.GetIncludeInfo("www.beianhao.net");
            IHaosouIncludeInfo info2 = HaosouUtility.GetIncludeInfo("www.xphter.com");
            IHaosouIncludeInfo info3 = HaosouUtility.GetIncludeInfo("news.qq.com");

            Assert.IsTrue(info1.IncludeCount > 0);
            Assert.IsTrue(info2.IncludeCount > 0);
            Assert.IsTrue(info3.IncludeCount > 0);
        }

        [TestMethod()]
        public void BeginGetIncludeInfoTest() {
            IHaosouIncludeInfo info1 = null, info2 = null, info3 = null;

            using(CountdownEvent ce = new CountdownEvent(3)) {
                HaosouUtility.BeginGetIncludeInfo("www.beianm.com", (ar) => {
                    try {
                        info1 = HaosouUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                HaosouUtility.BeginGetIncludeInfo("www.xphter.com", (ar) => {
                    try {
                        info2 = HaosouUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                HaosouUtility.BeginGetIncludeInfo("news.qq.com", (ar) => {
                    try {
                        info3 = HaosouUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                ce.Wait();
            }

            Assert.IsTrue(info1.IncludeCount > 0);
            Assert.IsTrue(info2.IncludeCount > 0);
            Assert.IsTrue(info3.IncludeCount > 0);
        }

        [TestMethod()]
        public void GetKeywordRankTest() {
            IEnumerable<int> rank1 = HaosouUtility.GetKeywordRank("备案查询", "www.beianbeian.com");
            IEnumerable<int> rank2 = HaosouUtility.GetKeywordRank("腾讯网", "www.xphter.com");

            Assert.IsTrue(rank1.Count() > 0);
            Assert.IsTrue(rank2.Count() == 0);
        }

        [TestMethod()]
        public void BeginGetKeywordRankTest() {
            IEnumerable<int> rank1 = null, rank2 = null;

            using(CountdownEvent ce = new CountdownEvent(2)) {
                HaosouUtility.BeginGetKeywordRank("备案查询", "www.beianbeian.com", (ar) => {
                    try {
                        rank1 = HaosouUtility.EndGetKeywordRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                HaosouUtility.BeginGetKeywordRank("腾讯网", "www.xphter.com", (ar) => {
                    try {
                        rank2 = HaosouUtility.EndGetKeywordRank(ar);
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
