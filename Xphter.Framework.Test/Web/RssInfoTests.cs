using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
namespace Xphter.Framework.Web.Tests {
    [TestClass()]
    public class RssInfoTests {
        [TestMethod()]
        public void RssInfoTest() {
            RssInfo info = new RssInfo {
                Channel = new RssChannelInfo {
                    Title = "首页",
                    Link = "http://www.xphter.com",
                    Description = "科锐软件工作室",
                    Categories = new List<RssCategoryInfo> {
                        new RssCategoryInfo {
                            Domain = "syndic8",
                            Value = "IT",
                        },
                        new RssCategoryInfo {
                            Domain = "syndic8",
                            Value = "News",
                        },
                    },
                    Copyright = "xphter.com 2014",
                    Docs = "http://www.xphter.com/rss",
                    Generator = "Xphter SiteBuilder",
                    Ttl = 60,
                    Image = new RssImageInfo {
                        Url = "http://www.xphter.com/logo.png",
                        Title = "首页",
                        Link = "http://www.xphter.com",
                        Description = "科锐软件工作室",
                        Width = 100,
                        Height = 120,
                    },
                    Language = "zh-cn",
                    LastBuildDate = DateTime.Today,
                    PubDate = DateTime.Today,
                    ManagingEditor = "editor@xphter.com",
                    WebMaster = "rss@xphter.com",
                    Items = new List<RssItemInfo> {
                        new RssItemInfo {
                            Title = "新闻1",
                            Link = "http://www.xphter.com/news/1.html",
                            Description = "新闻1描述",
                            PubDate = DateTime.Today,
                            Guid = "1",
                            Author = "editor@xphter.com",
                            Source = "新闻1来源",
                            Categories = new List<RssCategoryInfo> {
                                new RssCategoryInfo {
                                    Domain = "syndic8",
                                    Value = "IT",
                                },
                                new RssCategoryInfo {
                                    Domain = "syndic8",
                                    Value = "News",
                                },
                            },
                            Comments = "新闻1备注",
                            Enclosure = new RssEnclosureInfo {
                                Url = "http://www.xphter.com/news/1.jpg",
                                Type = "image/jepg",
                                Length = 100,
                            },
                        },
                        new RssItemInfo {
                            Title = "新闻2",
                            Link = "http://www.xphter.com/news/2.html",
                            Description = "新闻2描述",
                            PubDate = DateTime.Today,
                            Guid = "2",
                            Author = "editor@xphter.com",
                            Source = "新闻2来源",
                            Categories = new List<RssCategoryInfo> {
                                new RssCategoryInfo {
                                    Domain = "syndic8",
                                    Value = "IT",
                                },
                                new RssCategoryInfo {
                                    Domain = "syndic8",
                                    Value = "News",
                                },
                            },
                            Comments = "新闻2备注",
                            Enclosure = new RssEnclosureInfo {
                                Url = "http://www.xphter.com/news/2.jpg",
                                Type = "image/jepg",
                                Length = 200,
                            },
                        },
                    },
                },
            };

            XmlSerializer serializer = new XmlSerializer(typeof(RssInfo));
            using(StreamWriter writer = new StreamWriter("rss.xml", false, Encoding.UTF8)) {
                serializer.Serialize(writer, info);
            }
        }
    }
}
