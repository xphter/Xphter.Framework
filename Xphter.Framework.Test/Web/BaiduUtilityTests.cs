using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Web.Tests {
    [TestClass()]
    public class BaiduUtilityTests {
        [TestMethod()]
        public void PingTest() {
            Assert.IsTrue(BaiduUtility.Ping("XX软件工作室", "http://www.xx.com", "http://www.xx.com/a/index.html", "http://www.xx.com/rss/site.xml"));
        }

        [TestMethod()]
        public void BeignPingTest() {
            bool result = false;

            using(AutoResetEvent are = new AutoResetEvent(false)) {
                BaiduUtility.BeginPing("XX软件工作室", "http://www.xx.com", "http://www.xx.com/a/index.html", "http://www.xx.com/rss/site.xml", (ar) => {
                    try {
                        result = BaiduUtility.EndPing(ar);
                    } finally {
                        are.Set();
                    }
                }, false);

                are.WaitOne();
            }

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void PushTest() {
            IBaiduPushResult result = BaiduUtility.Push("www.beianm.com", "cMsCtRF1P1XullOH", false, new string[] {
                "http://www.beianm.com/b/21/802/XINICPBEI13001563HAO.html",
                "http://www.beianm.com/b/21/802/YUEICPBEI11053162HAO.html",
            });
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void BeginPushTest() {
            IBaiduPushResult result = null;

            using(AutoResetEvent are = new AutoResetEvent(false)) {
                BaiduUtility.BeginPush("www.beianm.com", "cMsCtRF1P1XullOH", false, new string[] {
                    "http://www.beianm.com/b/21/802/XINICPBEI13003823HAO.html",
                    "http://www.beianm.com/b/21/802/XINICPBEI15000645HAO.html",
                }, (ar) => {
                    try {
                        result = BaiduUtility.EndPush(ar);
                    } finally {
                        are.Set();
                    }
                }, false);
                are.WaitOne();
            }

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetIncludeInfoTest() {
            IBaiduIncludeInfo info1 = BaiduUtility.GetIncludeInfo("www.beianm.com");
            IBaiduIncludeInfo info2 = BaiduUtility.GetIncludeInfo("www.xphter.com");

            Assert.IsTrue(info1.IndexCount > 0);
            Assert.IsTrue(info1.IncludeCount > 0);
            Assert.IsTrue(info2.IndexCount > 0);
            Assert.IsTrue(info2.IncludeCount > 0);
        }

        [TestMethod()]
        public void BeginGetIncludeInfoTest() {
            IBaiduIncludeInfo info1 = null, info2 = null;

            using(CountdownEvent ce = new CountdownEvent(2)) {
                BaiduUtility.BeginGetIncludeInfo("www.beianm.com", (ar) => {
                    try {
                        info1 = BaiduUtility.EndGetIncludeInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                BaiduUtility.BeginGetIncludeInfo("www.xphter.com", (ar) => {
                    try {
                        info2 = BaiduUtility.EndGetIncludeInfo(ar);
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
        }

        [TestMethod()]
        public void GetChinazBaiduRankTest() {
            IBaiduRankInfo info1 = BaiduUtility.GetChinazBaiduRank("www.beianm.com");
            IBaiduRankInfo info2 = BaiduUtility.GetChinazBaiduRank("news.qq.com");
            IBaiduRankInfo info3 = BaiduUtility.GetChinazBaiduRank("www.xphter.com");

            Assert.IsTrue(info1.RankValue > 0);
            Assert.IsTrue(info1.ExpectedFlow > 0);
            Assert.IsTrue(info2.RankValue > 0);
            Assert.IsTrue(info2.ExpectedFlow > 0);
            Assert.IsTrue(info3.RankValue == 0);
            Assert.IsTrue(info3.ExpectedFlow == 0);
        }

        [TestMethod()]
        public void BeginGetChinazBaiduRankTest() {
            IBaiduRankInfo info1 = null, info2 = null, info3 = null;

            using(CountdownEvent ce = new CountdownEvent(3)) {
                BaiduUtility.BeginGetChinazBaiduRank("www.beianm.com", (ar) => {
                    try {
                        info1 = BaiduUtility.EndGetChinazBaiduRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                BaiduUtility.BeginGetChinazBaiduRank("news.qq.com", (ar) => {
                    try {
                        info2 = BaiduUtility.EndGetChinazBaiduRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                BaiduUtility.BeginGetChinazBaiduRank("www.xphter.com", (ar) => {
                    try {
                        info3 = BaiduUtility.EndGetChinazBaiduRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                ce.Wait();
            }

            Assert.IsTrue(info1.RankValue > 0);
            Assert.IsTrue(info1.ExpectedFlow > 0);
            Assert.IsTrue(info2.RankValue > 0);
            Assert.IsTrue(info2.ExpectedFlow > 0);
            Assert.IsTrue(info3.RankValue == 0);
            Assert.IsTrue(info3.ExpectedFlow == 0);
        }

        [TestMethod()]
        public void GetAiZhanBaiduRankTest() {
            IBaiduRankInfo info1 = BaiduUtility.GetAiZhanBaiduRank("www.beianm.com");
            IBaiduRankInfo info2 = BaiduUtility.GetAiZhanBaiduRank("news.qq.com");
            IBaiduRankInfo info3 = BaiduUtility.GetAiZhanBaiduRank("www.xphter.com");

            Assert.IsTrue(info1.RankValue > 0);
            Assert.IsTrue(info1.ExpectedFlow > 0);
            Assert.IsTrue(info2.RankValue > 0);
            Assert.IsTrue(info2.ExpectedFlow > 0);
            Assert.IsTrue(info3.RankValue == 0);
            Assert.IsTrue(info3.ExpectedFlow == 0);
        }

        [TestMethod()]
        public void BeginGetAiZhanBaiduRankTest() {
            IBaiduRankInfo info1 = null, info2 = null, info3 = null;

            using(CountdownEvent ce = new CountdownEvent(3)) {
                BaiduUtility.BeginGetAiZhanBaiduRank("www.beianhao.net", (ar) => {
                    try {
                        info1 = BaiduUtility.EndGetAiZhanBaiduRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                BaiduUtility.BeginGetAiZhanBaiduRank("news.qq.com", (ar) => {
                    try {
                        info2 = BaiduUtility.EndGetAiZhanBaiduRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                BaiduUtility.BeginGetAiZhanBaiduRank("www.xphter.com", (ar) => {
                    try {
                        info3 = BaiduUtility.EndGetAiZhanBaiduRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                ce.Wait();
            }

            Assert.IsTrue(info1.RankValue > 0);
            Assert.IsTrue(info1.ExpectedFlow > 0);
            Assert.IsTrue(info2.RankValue > 0);
            Assert.IsTrue(info2.ExpectedFlow > 0);
            Assert.IsTrue(info3.RankValue == 0);
            Assert.IsTrue(info3.ExpectedFlow == 0);
        }

        [TestMethod()]
        public void GetKeywordRankTest() {
            IEnumerable<int> rank1 = BaiduUtility.GetKeywordRank("寻美图", "www.xunmeitu.com");
            IEnumerable<int> rank2 = BaiduUtility.GetKeywordRank("腾讯网", "www.xphter.com");

            Assert.IsTrue(rank1.Count() > 0);
            Assert.IsTrue(rank2.Count() == 0);
        }

        [TestMethod()]
        public void BeginGetKeywordRankTest() {
            IEnumerable<int> rank1 = null, rank2 = null;

            using(CountdownEvent ce = new CountdownEvent(2)) {
                BaiduUtility.BeginGetKeywordRank("备案查询", "www.beianm.com", (ar) => {
                    try {
                        rank1 = BaiduUtility.EndGetKeywordRank(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                BaiduUtility.BeginGetKeywordRank("腾讯网", "www.xphter.com", (ar) => {
                    try {
                        rank2 = BaiduUtility.EndGetKeywordRank(ar);
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
