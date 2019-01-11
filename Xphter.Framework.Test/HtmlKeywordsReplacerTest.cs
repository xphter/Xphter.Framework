using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for XmlKeywordsReplacerTest and is intended
    ///to contain all XmlKeywordsReplacerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HtmlKeywordsReplacerTest {


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


        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_None() {
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(new XmlKeywordsReplacementProvider());
            string text = "heihei-haha";
            Assert.AreEqual<string>(text, target.Replace(text));
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_NoneHasTag() {
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(new XmlKeywordsReplacementProvider());
            string text = "heihei<div>123<span>456</span></div>-haha";
            Assert.AreEqual<string>(text, target.Replace(text));
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_OneNotTag() {
            IHtmlKeywordReplacementProvider provider = new XmlKeywordsReplacementProvider();
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(provider);
            string text = "heihei-dumohan-haha";
            Assert.AreEqual<string>(string.Format("heihei-{0}-haha", provider.Keywords.ElementAt(0).Replacement), target.Replace(text));
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_MultipleNotTag() {
            IHtmlKeywordReplacementProvider provider = new XmlKeywordsReplacementProvider();
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(provider);
            string text = "heihei-dumohan-haha-dumohan-123";
            Assert.AreEqual<string>(string.Format("heihei-{0}-haha-{0}-123", provider.Keywords.ElementAt(0).Replacement), target.Replace(text));
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_BeginTag() {
            IHtmlKeywordReplacementProvider provider = new XmlKeywordsReplacementProvider();
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(provider);
            string text = "heihei-dumohan<div title=\"dumohan\">456dumohan</div>";
            Assert.AreEqual<string>(string.Format("heihei-{0}<div title=\"dumohan\">456{0}</div>", provider.Keywords.ElementAt(0).Replacement), target.Replace(text));
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_IgnoreTag_Nesting() {
            IHtmlKeywordReplacementProvider provider = new XmlKeywordsReplacementProvider();
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(provider);
            string text = "heihei-<a><span>dumohan</span></a><div title=\"dumohan\">456dumohan</div>dumohan";
            Assert.AreEqual<string>(string.Format("heihei-<a><span>dumohan</span></a><div title=\"dumohan\">456{0}</div>{0}", provider.Keywords.ElementAt(0).Replacement), target.Replace(text));
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        public void ReplaceTest_IgnoreTag() {
            IHtmlKeywordReplacementProvider provider = new XmlKeywordsReplacementProvider();
            HtmlKeywordsReplacer target = new HtmlKeywordsReplacer(provider);
            string text = "heihei-<a>dumohan</a><div title=\"dumohan\">456dumohan</div>dumohan";
            Assert.AreEqual<string>(string.Format("heihei-<a>dumohan</a><div title=\"dumohan\">456{0}</div>{0}", provider.Keywords.ElementAt(0).Replacement), target.Replace(text));
        }

        private class XmlKeywordsReplacementProvider : IHtmlKeywordReplacementProvider {
            public XmlKeywordsReplacementProvider() {
                this.m_keywords = new List<HtmlKeywordReplacementInfo> {
                    new HtmlKeywordReplacementInfo(this,"dumohan","杜墨涵个人网站","<a href=\"http://www.dumohan.info\" target=\"_blank\" title=\"杜墨涵个人网站\">杜墨涵</a>"),
                };
                this.m_checker = new ReplacementChecker();
            }

            private List<HtmlKeywordReplacementInfo> m_keywords;

            public IEnumerable<HtmlKeywordReplacementInfo> Keywords {
                get {
                    return this.m_keywords.AsReadOnly();
                }
            }

            private IHtmlKeywordReplacementChecker m_checker;
            public IHtmlKeywordReplacementChecker Checker {
                get {
                    return this.m_checker;
                }
            }

            private class ReplacementChecker : IHtmlKeywordReplacementChecker {
                public bool Check(int count, HtmlKeywordReplacementInfo keyword, int position, HtmlTag tag, HtmlKeywordReplacementInfo previousKeyword, int previousPosition) {
                    bool allow = true;

                    while(tag != null) {
                        if(tag.Name.Equals("a", StringComparison.InvariantCultureIgnoreCase)) {
                            allow = false;
                            break;
                        }
                        tag = tag.Parent;
                    }

                    return allow;
                }
            }
        }
    }
}
