using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xtml;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for DataSourcePropertyXtmlElementRendererTest and is intended
    ///to contain all DataSourcePropertyXtmlElementRendererTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DataSourcePropertyXtmlElementRendererTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private XtmlDocumentOption m_prefixOption = new XtmlDocumentOption {
            Context = new XtmlMarkupContext {
                OpenChar = '[',
                OpenCharEntityName = "lte",
                CloseChar = ']',
                CloseCharEntityName = "gte",
                EndChar = '/',
            },
            Prefixs = new XtmlNodePrefix[]{
                new XtmlNodePrefix {
                    Value = "dst",
                },
            },
        };

        private class ArticleInfo {
            public string Name {
                get;
                set;
            }
            public string Author {
                get;
                set;
            }
            public DateTime CreateTime {
                get;
                set;
            }
        }

        /// <summary>
        ///A test for Render
        ///</summary>
        [TestMethod()]
        public void RenderTest() {
            string xtml = "文章信息\r\n名称：[dst:Name /]\r\n作者：[dst:Author /]\r\n时间：[dst:CreateTime /]";
            TemplateEngine.TagSystem.RegisterTag(new XtmlNodeRendererWrapper(new DataSourcePropertyXtmlElementRenderer<ArticleInfo, string>("Name")) {
                Name = "名称",
                TagName = "dst:Name",
            });
            TemplateEngine.TagSystem.RegisterTag(new XtmlNodeRendererWrapper(new DataSourcePropertyXtmlElementRenderer<ArticleInfo, string>("Author")) {
                Name = "作者",
                TagName = "dst:Author",
            });
            TemplateEngine.TagSystem.RegisterTag(new XtmlNodeRendererWrapper(new DataSourcePropertyXtmlElementRenderer<ArticleInfo, DateTime>("CreateTime", null, "yyyy-MM-dd HH:mm:ss")) {
                Name = "时间",
                TagName = "dst:CreateTime",
            });
            string filePath = Guid.NewGuid() + ".txt";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_prefixOption, null);
            TemplateEngine.TemplateSystem.Render(document, new ArticleInfo {
                Name = "名称",
                Author = "作者",
                CreateTime = new DateTime(2012, 12, 7, 8, 10, 30),
            }, null, filePath);

            Assert.AreEqual<string>("文章信息\r\n名称：名称\r\n作者：作者\r\n时间：2012-12-07 08:10:30", File.ReadAllText(filePath, Encoding.GetEncoding(document.Declaration.Encoding)));
        }
    }
}
