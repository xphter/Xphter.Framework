using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for HtmlRangeAnalyzerTest and is intended
    ///to contain all HtmlRangeAnalyzerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HtmlRangeAnalyzerTest {


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
        public void AnalyzeTest_Declaration() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(1, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>("<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">", range.RangeText);
            Assert.AreEqual<string>("html", range.ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>("PUBLIC", range.ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(" -//W3C//DTD XHTML 1.0 Transitional//EN", range.ChildRanges.ElementAt(2).RangeText);
            Assert.AreEqual<string>("http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd", range.ChildRanges.ElementAt(3).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Comment() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<!-- this is a HTML comment -->";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(1, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>(text, range.RangeText);
            Assert.AreEqual<string>(" this is a HTML comment ", range.ChildRanges.ElementAt(0).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_One() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<div class=\"block\">123</div>";
            List<IHtmlRange> ranges = new List<IHtmlRange>(target.Analyze(text));
            ranges.Sort();

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<string>(text, ranges[0].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<div class=\"block\"><a href=\"1.html\"><span>123<span>456</span><span>789</span></span></a></div>";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(1, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>(text, range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<a href=\"1.html\"><span>123<span>456</span><span>789</span></span></a>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<span>123<span>456</span><span>789</span></span>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<span>789</span>", range.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting_Comment() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<div class=\"block\"><!--<div></div>--><a href=\"1.html\"><!--<a />--><span>123<span>456</span><!--<div></div>--><span>789<!--123--></span></span><!--<a />--></a></div>";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(1, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>(text, range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<!--<div></div>-->", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(2);
            Assert.AreEqual<string>("<a href=\"1.html\"><!--<a />--><span>123<span>456</span><!--<div></div>--><span>789<!--123--></span></span><!--<a />--></a>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<!--<a />-->", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(2);
            Assert.AreEqual<string>("<span>123<span>456</span><!--<div></div>--><span>789<!--123--></span></span>", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(3);
            Assert.AreEqual<string>("<!--<a />-->", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(2).ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<!--<div></div>-->", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(2);
            Assert.AreEqual<string>("<span>789<!--123--></span>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<!--123-->", range.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Error() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<div class=\"block\"><a href='1.html'><span>123<span>456</span></span></a>spacing<span>789</span>";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(3, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>("<div class=\"block\">", range.RangeText);

            range = ranges.ElementAt(1);
            Assert.AreEqual<string>("<a href='1.html'><span>123<span>456</span></span></a>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<span>123<span>456</span></span>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", range.RangeText);

            range = ranges.ElementAt(2);
            Assert.AreEqual<string>("<span>789</span>", range.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Multiple() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">jsie.sjie<div class=\"block\"><a href=\"1.html\"><span>123<span>456</span></span></a></div>sjie.sieji<div style=\"margin: 0px;\"><p>789<p>101112<span>131415</span></p>161718</p></div>sjiea.ehisi";
            IEnumerable<IHtmlRange> ranges = null;
            ranges = target.Analyze(text);
            //ranges = target.Analyze(System.IO.File.ReadAllText(@"d:\1\beian.html", System.Text.Encoding.UTF8));

            Assert.AreEqual<int>(3, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>("<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">", range.RangeText);

            range = ranges.ElementAt(1);
            Assert.AreEqual<string>("<div class=\"block\"><a href=\"1.html\"><span>123<span>456</span></span></a></div>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<a href=\"1.html\"><span>123<span>456</span></span></a>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<span>123<span>456</span></span>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<span>456</span>", range.RangeText);

            range = ranges.ElementAt(2);
            Assert.AreEqual<string>("<div style=\"margin: 0px;\"><p>789<p>101112<span>131415</span></p>161718</p></div>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<p>789<p>101112<span>131415</span></p>161718</p>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<p>101112<span>131415</span></p>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<span>131415</span>", range.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelf() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<br /><img href=\"1.gif\" /><input type=\"text\" value='123' title=haha />";
            List<IHtmlRange> ranges = new List<IHtmlRange>(target.Analyze(text));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<string>("<br />", ranges[0].RangeText);
            Assert.AreEqual<string>("<img href=\"1.gif\" />", ranges[1].RangeText);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha />", ranges[2].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelf_Comment() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<br /><img href=\"1.gif\" /><!--<a href=\"\" target=\"_blank\" />--><input type=\"text\" value='123' title=haha />";
            List<IHtmlRange> ranges = new List<IHtmlRange>(target.Analyze(text));

            Assert.AreEqual<int>(4, ranges.Count);
            Assert.AreEqual<string>("<br />", ranges[0].RangeText);
            Assert.AreEqual<string>("<img href=\"1.gif\" />", ranges[1].RangeText);
            Assert.AreEqual<string>("<!--<a href=\"\" target=\"_blank\" />-->", ranges[2].RangeText);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha />", ranges[3].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelfNesting() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "ajies.<img href=\"1.gif\" />sji<div><input type=\"text\" value='123' title=haha /><a href=\"1.html\">1.html</a><br /></div>sjiehji18h";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(2, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>("<img href=\"1.gif\" />", range.RangeText);

            range = ranges.ElementAt(1);
            Assert.AreEqual<string>("<div><input type=\"text\" value='123' title=haha /><a href=\"1.html\">1.html</a><br /></div>", range.RangeText);

            range = range.ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha />", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<a href=\"1.html\">1.html</a>", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(2);
            Assert.AreEqual<string>("<br />", range.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelfNesting_Comment() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "ajies.<!--<div style=\"\"></div>--><img href=\"1.gif\" />sj<!--<input type=\"text\" />-->i<div><input type=\"text\" value='123' title=haha /><a href=\"1.html\">1.html<!--Test--></a><br /></div>sjieh<!--456-->ji18h";
            IEnumerable<IHtmlRange> ranges = target.Analyze(text);

            Assert.AreEqual<int>(5, ranges.Count());

            IHtmlRange range = ranges.ElementAt(0);
            Assert.AreEqual<string>("<!--<div style=\"\"></div>-->", range.RangeText);

            range = ranges.ElementAt(1);
            Assert.AreEqual<string>("<img href=\"1.gif\" />", range.RangeText);

            range = ranges.ElementAt(2);
            Assert.AreEqual<string>("<!--<input type=\"text\" />-->", range.RangeText);

            range = ranges.ElementAt(3);
            Assert.AreEqual<string>("<div><input type=\"text\" value='123' title=haha /><a href=\"1.html\">1.html<!--Test--></a><br /></div>", range.RangeText);

            range = ranges.ElementAt(4);
            Assert.AreEqual<string>("<!--456-->", range.RangeText);

            range = ranges.ElementAt(3).ChildRanges.ElementAt(0);
            Assert.AreEqual<string>("<input type=\"text\" value='123' title=haha />", range.RangeText);

            range = range.ParentRange.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<a href=\"1.html\">1.html<!--Test--></a>", range.RangeText);

            range = range.ChildRanges.ElementAt(1);
            Assert.AreEqual<string>("<!--Test-->", range.RangeText);

            range = range.ParentRange.ParentRange.ChildRanges.ElementAt(2);
            Assert.AreEqual<string>("<br />", range.RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Attribute() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<a href=\"http://www.163.com\" target='_blank' title=网易></a>";

            IHtmlRange tag = target.Analyze(text).ElementAt(0);
            ICollection<IHtmlAttribute> attributes = ((IHtmlElement) tag.ToNode()).Attributes;

            Assert.AreEqual<int>(3, attributes.Count);
            Assert.AreEqual<string>("href", attributes.ElementAt(0).Name);
            Assert.AreEqual<string>("http://www.163.com", attributes.ElementAt(0).Value);
            Assert.AreEqual<string>("target", attributes.ElementAt(1).Name);
            Assert.AreEqual<string>("_blank", attributes.ElementAt(1).Value);
            Assert.AreEqual<string>("title", attributes.ElementAt(2).Name);
            Assert.AreEqual<string>("网易", attributes.ElementAt(2).Value);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_NormalizeAttribute() {
            HtmlRangeAnalyzer target = new HtmlRangeAnalyzer();
            string text = "<div>\r\n<input   readonly   type   =    \"text\" autofocus value=   '123' disabled placeholder   =占位符   />\r\n<a href=\"\"  ></a></div>";
            text = target.Normalize(text);
            Assert.AreEqual("<div>\r\n<input readonly=\"readonly\"   type   =    \"text\" autofocus=\"autofocus\" value=   '123' disabled=\"disabled\" placeholder   =占位符   />\r\n<a href=\"\"  ></a></div>", text);

            IHtmlRange tag = target.Analyze(text).ElementAt(0);
            ICollection<IHtmlAttribute> attributes = ((IHtmlElement) (tag.ToNode().ChildNodes[1])).Attributes;

            Assert.AreEqual<int>(6, attributes.Count);
            Assert.AreEqual<string>("readonly", attributes.ElementAt(0).Name);
            Assert.AreEqual<string>("readonly", attributes.ElementAt(0).Value);

            Assert.AreEqual<string>("type", attributes.ElementAt(1).Name);
            Assert.AreEqual<string>("text", attributes.ElementAt(1).Value);

            Assert.AreEqual<string>("autofocus", attributes.ElementAt(2).Name);
            Assert.AreEqual<string>("autofocus", attributes.ElementAt(2).Value);

            Assert.AreEqual<string>("value", attributes.ElementAt(3).Name);
            Assert.AreEqual<string>("123", attributes.ElementAt(3).Value);

            Assert.AreEqual<string>("disabled", attributes.ElementAt(4).Name);
            Assert.AreEqual<string>("disabled", attributes.ElementAt(4).Value);

            Assert.AreEqual<string>("placeholder", attributes.ElementAt(5).Name);
            Assert.AreEqual<string>("占位符", attributes.ElementAt(5).Value);
        }
    }
}
