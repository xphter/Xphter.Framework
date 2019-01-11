using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xtml;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for TemplateSystemTest and is intended
    ///to contain all TemplateSystemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TemplateSystemTest {


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

        private XtmlDocumentOption m_option = new XtmlDocumentOption {
            Context = new XtmlMarkupContext {
                OpenChar = '[',
                OpenCharEntityName = "lte",
                CloseChar = ']',
                CloseCharEntityName = "gte",
                EndChar = '/',
            }
        };

        private XtmlDocumentOption m_onePrefixOption = new XtmlDocumentOption {
            Context = new XtmlMarkupContext {
                OpenChar = '[',
                OpenCharEntityName = "lte",
                CloseChar = ']',
                CloseCharEntityName = "gte",
                EndChar = '/',
            },
            Prefixs = new XtmlNodePrefix[]{
                new XtmlNodePrefix {
                    Value = "x",
                },
            },
        };

        private XtmlDocumentOption m_twoPrefixOption = new XtmlDocumentOption {
            Context = new XtmlMarkupContext {
                OpenChar = '[',
                OpenCharEntityName = "lte",
                CloseChar = ']',
                CloseCharEntityName = "gte",
                EndChar = '/',
            },
            Prefixs = new XtmlNodePrefix[]{
                new XtmlNodePrefix {
                    Value = "x",
                },
                new XtmlNodePrefix {
                    Value = "y",
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
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_Empty() {
            string xtml = string.Empty;
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_option, null);
            Assert.AreEqual<int>(0, document.ChildNodes.Count());
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_NoChildren() {
            string xtml = "holder&lte;占位符&gte;holder";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_option, null);
            Assert.AreEqual<int>(1, document.ChildNodes.Count());
            Assert.AreEqual<string>(xtml, document.ChildNodes.ElementAt(0).NodeXtml);
            Assert.AreEqual<string>(this.m_option.Context.Decode(xtml), ((IXtmlText) document.ChildNodes.ElementAt(0)).Text);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_NoChildren_HasDeclaration() {
            string xtml = "[?xtml version=\"1.1\" type=\"text/javascript\" encoding=\"gb2312\" ?]holder占位符holder";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_option, null);
            Assert.AreEqual<int>(2, document.ChildNodes.Count());
            Assert.AreEqual<string>("1.1", document.Declaration.Version);
            Assert.AreEqual<string>("text/javascript", document.Declaration.Type);
            Assert.AreEqual<string>("gb2312", document.Declaration.Encoding);
            Assert.AreEqual<string>("holder占位符holder", document.ChildNodes.ElementAt(1).NodeXtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_OneElement() {
            string xtml = "[?xtml version=\"1.1\" encoding=\"gb2312\" ?]\r\n[name a0=\"value0\"]holder[/name]holder";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_option, null);
            Assert.AreEqual<int>(4, document.ChildNodes.Count());
            Assert.AreEqual<string>("name", document.ChildNodes.ElementAt(2).LocalName);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_MutipleElement() {
            string xtml = "[?xtml version=\"1.1\" encoding=\"gb2312\" ?]\r\n[a a0=\"&lte;value&lte;0&gte;\"]holder\r\n[b /][/a]holder";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_option, null);
            Assert.AreEqual<int>(4, document.ChildNodes.Count());
            Assert.AreEqual<string>("a", document.ChildNodes.ElementAt(2).LocalName);
            Assert.AreEqual<string>("[value[0]", ((IXtmlElement) document.ChildNodes.ElementAt(2)).Attributes.ElementAt(0).GetValue(null, null));
            Assert.AreEqual<string>("holder\r\n", document.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).NodeXtml);
            Assert.AreEqual<string>("b", document.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1).LocalName);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_MutipleElement_Prefix() {
            string xtml = "[?xtml version=\"1.1\" encoding=\"gb2312\" ?]\r\n[x:a a0=\"&lte;value0&gte;\"]holder\r\n[x:b /][/x:a]holder";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_onePrefixOption, "abc");
            Assert.AreEqual<int>(4, document.ChildNodes.Count());
            Assert.AreEqual<string>("x:a", ((IXtmlElement) document.ChildNodes.ElementAt(2)).QualifiedName);
            Assert.AreEqual<string>("[value0]", ((IXtmlElement) document.ChildNodes.ElementAt(2)).Attributes.ElementAt(0).GetValue(null, null));
            Assert.AreEqual<string>("holder\r\n", document.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).NodeXtml);
            Assert.AreEqual<string>("x:b", ((IXtmlElement) document.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1)).QualifiedName);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_MutipleElement_MultiplePrefix() {
            string xtml = "[?xtml version=\"1.1\" encoding=\"gb2312\" ?]\r\n[x:a a0=\"&lte;value0&gte;\"]holder\r\n[y:b /][/x:a]holder";
            IXtmlDocument document = TemplateEngine.TemplateSystem.Load(xtml, this.m_twoPrefixOption, "abc");
            Assert.AreEqual<int>(4, document.ChildNodes.Count());
            Assert.AreEqual<string>("x:a", ((IXtmlElement) document.ChildNodes.ElementAt(2)).QualifiedName);
            Assert.AreEqual<string>("[value0]", ((IXtmlElement) document.ChildNodes.ElementAt(2)).Attributes.ElementAt(0).GetValue(null, null));
            Assert.AreEqual<string>("holder\r\n", document.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).NodeXtml);
            Assert.AreEqual<string>("y:b", ((IXtmlElement) document.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1)).QualifiedName);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod]
        public void LoadTest_File() {
            string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\Template.html");
            using(TextReader reader = new StreamReader(file, Encoding.UTF8)) {
                IXtmlDocument document = TemplateEngine.TemplateSystem.Load(reader, this.m_onePrefixOption, null);
            }
        }

        /// <summary>
        ///A test for Render
        ///</summary>
        [TestMethod()]
        public void RenderTest() {
            string xtml = "holder&lte;[x:Copyright /]&gte;&lte;[x:Author name=\"[x:AuthorName /]\" /]&gte;holder";
            TemplateEngine.TagSystem.RegisterTag(new XtmlNodeRendererWrapper(new XtmlNodeRenderer((node, writer, dataSource, context) => writer.Write("Xphter 2012"))) {
                Name = "版权",
                TagName = "x:Copyright",
            });
            TemplateEngine.TagSystem.RegisterTag(new XtmlNodeRendererWrapper(new XtmlNodeRenderer((node, writer, dataSource, context) => writer.Write(((IXtmlElement) node).GetAttributesByName("name").First().GetValue(dataSource, context)))) {
                Name = "作者",
                TagName = "x:Author",
            });
            TemplateEngine.TagSystem.RegisterTag(new XtmlNodeRendererWrapper(new DataSourcePropertyXtmlElementRenderer<ArticleInfo, string>("Name")) {
                Name = "作者名称",
                TagName = "x:AuthorName",
            });
            string filePath = Guid.NewGuid() + ".txt";
            TemplateEngine.TemplateSystem.Render(TemplateEngine.TemplateSystem.Load(xtml, this.m_onePrefixOption, null), new ArticleInfo {
                Name = "DuPeng",
            }, null, filePath);

            Assert.AreEqual<string>("holder[Xphter 2012][DuPeng]holder", File.ReadAllText(filePath));
        }
    }
}
