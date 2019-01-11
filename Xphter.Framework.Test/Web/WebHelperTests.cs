using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Web.Tests {
    [TestClass()]
    public class WebHelperTests {
        /// <summary>
        ///A test for HtmlEncode
        ///</summary>
        [TestMethod()]
        public void HtmlEncodeTest() {
            string value = "<>&\"\' ";
            string expected = "&lt;&gt;&amp;&quot;'&nbsp;"; // TODO: Initialize to an appropriate value
            string actual = WebHelper.HtmlEncode(value);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SaveSitemapTest() {
            SitemapInfo info = new SitemapInfo(new SitemapItemInfo[] {
                new SitemapItemInfo {
                    Location = "http://www.xphter.com/1.html",
                    ItemType = SitemapItemType.PersonalComputer,
                    LastModifyTime = DateTime.Now,
                    ChangeFrequency = SitemapItemChangeFrequency.Daily,
                    Priority = 0.8F,
                },
                new SitemapItemInfo {
                    Location = "http://www.xphter.com/2.html",
                    ItemType = SitemapItemType.Mobile,
                    LastModifyTime = DateTime.Today,
                },
                new SitemapItemInfo {
                    Location = "http://www.xphter.com/3.html",
                    ItemType = SitemapItemType.AutoAdaptive,
                },
                new SitemapItemInfo {
                    Location = "http://www.xphter.com/4.html",
                    ItemType = SitemapItemType.HtmlAdaptive,
                },
            });

            new DefaultSitemapSerializer().Serialize(info, "default_sitemap.xml");
            new BaiduSitemapSerializer().Serialize(info, "baidu_sitemap.xml");
        }

        [TestMethod()]
        public void SaveSitemapIndexTest() {
            SitemapIndexInfo info = new SitemapIndexInfo(new SitemapIndexItemInfo[] {
                new SitemapIndexItemInfo {
                    Location = "http://www.xphter.com/1.html",
                    LastModifyTime = DateTime.Now,
                },
                new SitemapIndexItemInfo {
                    Location = "http://www.xphter.com/2.html",
                },
            });

            new DefaultSitemapSerializer().Serialize(info, "default_sitemap_index.xml");
            new BaiduSitemapSerializer().Serialize(info, "baidu_sitemap_index.xml");
        }

        [TestMethod()]
        public void GetWebSiteInfoTest() {
            IWebSiteInfo info = WebHelper.GetWebSiteInfo("http://www.huazhang.com", null);
            IWebSiteInfo info1 = WebHelper.GetWebSiteInfo("http://www.beianm.com", null);
            IWebSiteInfo info2 = WebHelper.GetWebSiteInfo("http://www.qq.com", null);
            IWebSiteInfo info3 = WebHelper.GetWebSiteInfo("http://www.163.com", null);
            IWebSiteInfo info4 = WebHelper.GetWebSiteInfo("http://www.sohu.com", null);

            Assert.IsNotNull(info1);
            Assert.IsNotNull(info2);
            Assert.IsNotNull(info3);
            Assert.IsNotNull(info4);
        }

        [TestMethod()]
        public void BeginGetWebSiteInfoTest() {
            IWebSiteInfo info1 = null, info2 = null, info3 = null, info4 = null;

            using(CountdownEvent ce = new CountdownEvent(4)) {
                WebHelper.BeginGetWebSiteInfo("http://www.beianm.com", null, (ar) => {
                    try {
                        info1 = WebHelper.EndGetWebSiteInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                WebHelper.BeginGetWebSiteInfo("http://www.qq.com", null, (ar) => {
                    try {
                        info2 = WebHelper.EndGetWebSiteInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                WebHelper.BeginGetWebSiteInfo("http://www.163.com", null, (ar) => {
                    try {
                        info3 = WebHelper.EndGetWebSiteInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                WebHelper.BeginGetWebSiteInfo("http://www.sohu.com", null, (ar) => {
                    try {
                        info4 = WebHelper.EndGetWebSiteInfo(ar);
                    } finally {
                        ce.Signal();
                    }
                }, null);
                ce.Wait();
            }

            Assert.IsNotNull(info1);
            Assert.IsNotNull(info2);
            Assert.IsNotNull(info3);
            Assert.IsNotNull(info4);
        }

        [TestMethod()]
        public void GetIISVersionTest() {
            Assert.IsTrue(WebHelper.GetIISVersion().HasValue);
        }
    }
}
