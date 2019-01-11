using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Test {

    /// <summary>
    ///This is a test class for HtmlAnalyzerTest and is intended
    ///to contain all HtmlAnalyzerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HtmlAnalyzerTest {


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
        ///A test for Load
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadTest_Null() {
            string htmlText = null;
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_Empty() {
            string htmlText = string.Empty;
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<int>(0, document.ChildNodes.Count);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_WhiteSpace() {
            string htmlText = "  \t  \r\n   \t\r\n  ";
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<int>(1, document.ChildNodes.Count);
            Assert.AreEqual<string>(htmlText, document.ChildNodes[0].NodeText);
            Assert.AreEqual<string>(htmlText, document.ChildNodes[0].NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_HtmlEncode() {
            string htmlText = "  \t&nbsp;&lt;123&gt;&nbsp;\t  ";
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<int>(1, document.ChildNodes.Count);
            Assert.AreEqual<string>(htmlText, document.ChildNodes[0].NodeText);
            Assert.AreEqual<string>(htmlText, document.ChildNodes[0].NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_HtmlText() {
            string htmlText = "<!--123--><p>这是<a href=\"\" target=blank>段落</a>，<!--456-->它中间包含一个注释。</p>";
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<string>("这是段落，它中间包含一个注释。", document.NodeText);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_Entire() {
            string htmlText = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
"<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n" +
"<head>\r\n" +
"<title>{$metaTitle}</title>\r\n" +
"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n" +
"<meta name=\"keywords\" content=\"{$metaKeyword}\" />\r\n" +
"<meta name=\"description\" content=\"{$metaDescription}\" />\r\n" +
"<script type=\"text/javascript\" src=\"site.js\"></script>\r\n" +
"<link href=\"site.css\" rel=\"stylesheet\" type=\"text/css\" />\r\n" +
"</head>\r\n" +
"<body id=\"body\">\r\n" +
"<!-- body start -->\r\n" +
"<div id=\"head\">\r\n" +
"<!-- page header -->\r\n" +
"Page &lt;Header&gt; <input type=\"text\" value=\"123\" />\r\n" +
"</div>\r\n" +
"<div id=\"footer\">\r\n" +
"<!-- page header -->\r\n" +
"Page &lt;Footer&gt;\r\n" +
"</div>\r\n" +
"<!-- body end -->\r\n" +
"</body>\r\n" +
"</html>\r\n";

            //htmlText = File.ReadAllText(@"d:\4\html.txt", Encoding.UTF8);
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<string>(htmlText, document.NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_Entire_Error() {
            string htmlText = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
"<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n" +
"<head>\r\n" +
"<title>{$metaTitle}</title>\r\n" +
"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n" +
"<meta name=\"keywords\" content=\"{$metaKeyword}\" />\r\n" +
"<meta name=\"description\" content=\"{$metaDescription}\" />\r\n" +
"<script type=\"text/javascript\" src=\"site.js\"></script>\r\n" +
"<link href=\"site.css\" rel=\"stylesheet\" type=\"text/css\">\r\n" +
"</head>\r\n" +
"<body id=\"body\">\r\n" +
"<!-- body start -->\r\n" +
"<div id=\"head\">\r\n" +
"<!-- page header -->\r\n" +
"Page &lt;Header&gt; <input type=\"text\" value=\"123\" />\r\n" +
"</div>\r\n" +
"<div id=\"footer\">\r\n" +
"<!-- page header -->\r\n" +
"Page &lt;Footer&gt;\r\n" +
"</div>\r\n" +
"<!-- body end -->\r\n" +
"</body>\r\n" +
"</html>\r\n";
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<string>(htmlText, document.NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_Fragment() {
            string htmlText = "<p>\r\n" +
    "&nbsp; &nbsp;<strong><a href=\"http://www.nyautohome.com/\">南阳汽车网</a></strong>编辑从<a " +
        "href=\"http://www.nyautohome.com/store/xinkaidi\">凯迪拉克汽车南阳新凯迪4S店</a>获悉，凯迪拉克针对旗下全新豪华轿车XTS全系28T车型推出&ldquo;5050气球金融贷款计划&rdquo;。此项金融贷款服务拥有低首付、低月供、灵活还款三大优势。</p>\r\n" +
"<p style=\"text-align: center;\">\r\n" +
    "<img alt=\"\" src=\"/upload/images/2013/09/09/20130909_091956.jpg\" style=\"width: 500px;\r\n" +
        "height: 283px;\" /></p>\r\n" +
"<p>\r\n" +
    "以热销的<a href=\"http://www.nyautohome.com/auto/12274\" target=\"_blank\" class=\"contentTextLink\">凯迪拉克XTS</a>28T豪华型为例(官方指导价41.99万元)，消费者仅需率先支付车款的50%，便可以不足21万元的价格即刻购得一款拥有真皮座椅、全景天窗、BOSE&reg;音响、10安全气囊、智能三区空调、GPS导航及倒车影像、ESS强化安全策略、配备蓝光播放器的双屏影音娱乐系统等豪华配置的<a " +
        "href=\"http://www.nyautohome.com/auto/12274\" target=\"_blank\" class=\"contentTextLink\">凯迪拉克XTS</a>。购车当年内，每月支付698元贷款利息，日供仅需20元左右，即可于一年后还清剩余50%车款。如还款时资金紧张，凯迪拉克还提供一年展期服务，延长一年还款期限。对于升级置换车主，可用现有车型抵扣部分首付款项，轻松置换一台拥有更多科技配置的豪华轿车。</p>\r\n" +
"<p>\r\n" +
    "<a href=\"http://www.nyautohome.com/auto/12274\" target=\"_blank\" class=\"contentTextLink\">\r\n" +
        "凯迪拉克XTS</a> 28T车型搭载荣获&ldquo;沃德十佳发动机&rdquo;的2.0T涡轮增压发动机，通过凯迪拉克CUE移动互联体验、Bose\r\n" +
    "ANC主动降噪静音科技、MRC主动电磁感应悬挂、ESS强化安全策略等业界领先的创新科技，为消费者带来了革命性驾乘体验。凭借极具竞争力的配置与价格优势，<a href=\"http://www.nyautohome.com/auto/12274\" " +
        "target=\"_blank\" class=\"contentTextLink\">凯迪拉克XTS</a>上市2个多月来，销量已突破4,000辆，获得了消费者的广泛认可，成为国内豪车市场新锐。</p>\r\n" +
"<p>\r\n" +
    "若需了解更多信息，敬请莅临凯迪拉克南阳新凯迪展厅品鉴试驾。</p>\r\n" +
"<p>\r\n" +
    "[stl.store:58]</p>\r\n";

            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<string>(htmlText, document.NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_SetAttributeValue() {
            string htmlText1 = "<p>123<a target=\"_blank\" href=\"../../1.html\">000</a>456</p>";
            string htmlText2 = "<p>123<a target=\"_self\" href=\"../../2.html\">000</a>456</p>";
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText1);
            IHtmlAttribute attribute = document.GetElementsByTagName("a").First().GetAttributesByName("target").First();
            attribute.Value = "_self";
            attribute = document.GetElementsByTagName("a").First().GetAttributesByName("href").First();
            attribute.Value = "../../2.html";
            Assert.AreEqual<string>(htmlText2, document.NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_AloneAttributeName() {
            string htmlText = "<input type=\"check\" autofocus readonly class=a checked value='123' disabled />";
            IHtmlDocument document = HtmlAnalyzer.Load(htmlText);
            Assert.AreEqual<string>("<input type=\"check\" autofocus=\"autofocus\" readonly=\"readonly\" class=\"a\" checked=\"checked\" value=\"123\" disabled=\"disabled\" />", document.NodeHtml);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest_OnlyJavaScript() {
            IHtmlDocument document = HtmlAnalyzer.Load(Properties.Resources.HtmlAnalyzer_OnlyJavaScript);
            // there is a bug has not repaired: can not parse js contents.
        }
    }
}
