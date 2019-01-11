using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for XmlTextAnalyzerTest and is intended
    ///to contain all XmlTextAnalyzerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HtmlTextAnalyzerTest {


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
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_One() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "<div class=\"block\">123</div>";
            List<HtmlTag> tags = new List<HtmlTag>(target.Analyze(text));
            tags.Sort();

            Assert.AreEqual<int>(1, tags.Count);
            Assert.AreEqual<string>(text, tags[0].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "<div class=\"block\"><a href=\"1.html\"><span>123<span>456</span><span>789</span></span></a></div>";
            IEnumerable<HtmlTag> tags = target.Analyze(text);

            Assert.AreEqual<int>(1, tags.Count());

            HtmlTag tag = tags.ElementAt(0);
            Assert.AreEqual<string>(text, tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<a href=\"1.html\"><span>123<span>456</span><span>789</span></span></a>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>123<span>456</span><span>789</span></span>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", tag.RangeText);

            tag = tag.Parent.ChildTags.ElementAt(1);
            Assert.AreEqual<string>("<span>789</span>", tag.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Error() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "<div class=\"block\"><a href='1.html'><span>123<span>456</span></span></a>spacing<span>789</span>";
            IEnumerable<HtmlTag> tags = target.Analyze(text);

            Assert.AreEqual<int>(3, tags.Count());

            HtmlTag tag = tags.ElementAt(0);
            Assert.AreEqual<string>("<div class=\"block\">", tag.RangeText);

            tag = tags.ElementAt(1);
            Assert.AreEqual<string>("<a href='1.html'><span>123<span>456</span></span></a>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>123<span>456</span></span>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", tag.RangeText);

            tag = tags.ElementAt(2);
            Assert.AreEqual<string>("<span>789</span>", tag.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Multiple() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "jsie.sjie<div class=\"block\"><a href=\"1.html\"><span>123<span>456</span></span></a></div>sjie.sieji<div style=\"margin: 0px;\"><p>789<p>101112<span>131415</span></p>161718</p></div>sjiea.ehisi";
            IEnumerable<HtmlTag> tags = target.Analyze(text);

            Assert.AreEqual<int>(2, tags.Count());

            HtmlTag tag = tags.ElementAt(0);
            Assert.AreEqual<string>("<div class=\"block\"><a href=\"1.html\"><span>123<span>456</span></span></a></div>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<a href=\"1.html\"><span>123<span>456</span></span></a>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>123<span>456</span></span>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", tag.RangeText);

            tag = tags.ElementAt(1);
            Assert.AreEqual<string>("<div style=\"margin: 0px;\"><p>789<p>101112<span>131415</span></p>161718</p></div>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<p>789<p>101112<span>131415</span></p>161718</p>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<p>101112<span>131415</span></p>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<span>131415</span>", tag.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelf() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "<br /><img href=\"1.gif\" /><input type=\"text\" value='123' title=haha />";
            List<HtmlTag> tags = new List<HtmlTag>(target.Analyze(text));

            Assert.AreEqual<int>(3, tags.Count);
            Assert.AreEqual<string>("<br />", tags[0].RangeText);
            Assert.AreEqual<string>("<img href=\"1.gif\" />", tags[1].RangeText);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha />", tags[2].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelfNesting() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "ajies.<img href=\"1.gif\" />sji<div><input type=\"text\" value='123' title=haha /><a href=\"1.html\">1.html</a><br /></div>sjiehji18h";
            IEnumerable<HtmlTag> tags = target.Analyze(text);

            Assert.AreEqual<int>(2, tags.Count());

            HtmlTag tag = tags.ElementAt(0);
            Assert.AreEqual<string>("<img href=\"1.gif\" />", tag.RangeText);

            tag = tags.ElementAt(1);
            Assert.AreEqual<string>("<div><input type=\"text\" value='123' title=haha /><a href=\"1.html\">1.html</a><br /></div>", tag.RangeText);

            tag = tag.ChildTags.ElementAt(0);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha />", tag.RangeText);

            tag = tag.Parent.ChildTags.ElementAt(1);
            Assert.AreEqual<string>("<a href=\"1.html\">1.html</a>", tag.RangeText);

            tag = tag.Parent.ChildTags.ElementAt(2);
            Assert.AreEqual<string>("<br />", tag.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_BeginOnly() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "<br><div><font><span>123</span></div><input type=\"text\" value='123' title=haha >";
            IEnumerable<HtmlTag> tags = target.Analyze(text);

            Assert.AreEqual<int>(5, tags.Count());

            HtmlTag tag = tags.ElementAt(0);
            Assert.AreEqual<string>("<br>", tag.RangeText);

            tag = tags.ElementAt(1);
            Assert.AreEqual<string>("<div>", tag.RangeText);

            tag = tags.ElementAt(2);
            Assert.AreEqual<string>("<font>", tag.RangeText);

            tag = tags.ElementAt(3);
            Assert.AreEqual<string>("<span>123</span>", tag.RangeText);

            tag = tags.ElementAt(4);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha >", tag.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Attribute() {
            HtmlTextAnalyzer target = new HtmlTextAnalyzer();
            string text = "<a href=\"http://www.163.com\" target='_blank' title=网易></a>";
            HtmlTag tag = target.Analyze(text).ElementAt(0);
            IEnumerable<HtmlAttribute> attributes = tag.Attributes;

            Assert.AreEqual<int>(3, attributes.Count());
            Assert.AreEqual<string>("href", attributes.ElementAt(0).Name);
            Assert.AreEqual<string>("http://www.163.com", attributes.ElementAt(0).Value);
            Assert.AreEqual<string>("target", attributes.ElementAt(1).Name);
            Assert.AreEqual<string>("_blank", attributes.ElementAt(1).Value);
            Assert.AreEqual<string>("title", attributes.ElementAt(2).Name);
            Assert.AreEqual<string>("网易", attributes.ElementAt(2).Value);
        }
    }
}
