using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xtml;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for XtmlRangeAnalyzerTest and is intended
    ///to contain all XtmlRangeAnalyzerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XtmlRangeAnalyzerTest {


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
            },
        };

        private XtmlDocumentOption m_onePrefixOption = new XtmlDocumentOption {
            Context = new XtmlMarkupContext {
                OpenChar = '[',
                OpenCharEntityName = "lte",
                CloseChar = ']',
                CloseCharEntityName = "gte",
                EndChar = '/',
            },
            Prefixs = new XtmlNodePrefix[] {
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
            Prefixs = new XtmlNodePrefix[] {
                new XtmlNodePrefix {
                    Value = "x",
                },
                new XtmlNodePrefix {
                    Value = "y",
                },
            },
        };

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_One() {
            string xtmlText = @"holder[a]123[/a]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<string>(@"[a]123[/a]", ranges[0].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_One_Attribute() {
            string xtmlText = @"holder[a a0=""val'ue0"" a1='val""ue1']123[/a]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<int>(2, ranges[0].ChildRanges.Count());
            Assert.AreEqual<string>(@"[a a0=""val'ue0"" a1='val""ue1']123[/a]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"a0=""val'ue0""", ranges[0].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"a1='val""ue1'", ranges[0].ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Multiple() {
            string xtmlText = @"holder[a]123[b]456[c]789[/c][/b][/a]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<int>(1, ranges[0].ChildRanges.Count());
            Assert.AreEqual<int>(1, ranges[0].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[a]123[b]456[c]789[/c][/b][/a]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[b]456[c]789[/c][/b]", ranges[0].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[c]789[/c]", ranges[0].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Multiple_Prefix() {
            string xtmlText = @"holder[x:a]123[x:b]456[x:c]789[/x:c][/x:b][/x:a]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_onePrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<int>(1, ranges[0].ChildRanges.Count());
            Assert.AreEqual<int>(1, ranges[0].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[x:a]123[x:b]456[x:c]789[/x:c][/x:b][/x:a]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:b]456[x:c]789[/x:c][/x:b]", ranges[0].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:c]789[/x:c]", ranges[0].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Multiple_Attribute() {
            string xtmlText = @"holder[a a0=""value0""]123[b b0='value0' b1=""value1""]456[c]789[/c][/b][/a]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<int>(2, ranges[0].ChildRanges.Count());
            Assert.AreEqual<int>(3, ranges[0].ChildRanges.ElementAt(1).ChildRanges.Count());
            Assert.AreEqual<string>(@"[a a0=""value0""]123[b b0='value0' b1=""value1""]456[c]789[/c][/b][/a]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[b b0='value0' b1=""value1""]456[c]789[/c][/b]", ranges[0].ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(@"[c]789[/c]", ranges[0].ChildRanges.ElementAt(1).ChildRanges.ElementAt(2).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Multiple_Attribute_Prefix() {
            string xtmlText = @"holder[x:a a0=""value0""]123[x:b b0=""value0"" b1='value1']456[x:c]789[/x:c][/x:b][/x:a]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_onePrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(1, ranges.Count);
            Assert.AreEqual<int>(2, ranges[0].ChildRanges.Count());
            Assert.AreEqual<int>(3, ranges[0].ChildRanges.ElementAt(1).ChildRanges.Count());
            Assert.AreEqual<string>(@"[x:a a0=""value0""]123[x:b b0=""value0"" b1='value1']456[x:c]789[/x:c][/x:b][/x:a]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:b b0=""value0"" b1='value1']456[x:c]789[/x:c][/x:b]", ranges[0].ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(@"[x:c]789[/x:c]", ranges[0].ChildRanges.ElementAt(1).ChildRanges.ElementAt(2).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Error() {
            string xtmlText = @"holder[a][b][/b]holder[[c][/c][/d]holder[e][f /][/e]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<string>(@"[b][/b]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[c][/c]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[e][f /][/e]", ranges[2].RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelf() {
            string xtmlText = @"holder[a /]holder[b a0=""123""/][c a0='123' a1=""456"" /]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<string>(@"[a /]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[b a0=""123""/]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[c a0='123' a1=""456"" /]", ranges[2].RangeText);

            Assert.AreEqual<int>(0, ranges[0].ChildRanges.Count());

            Assert.AreEqual<int>(1, ranges[1].ChildRanges.Count());
            Assert.AreEqual<string>(@"a0=""123""", ranges[1].ChildRanges.ElementAt(0).RangeText);

            Assert.AreEqual<int>(2, ranges[2].ChildRanges.Count());
            Assert.AreEqual<string>(@"a0='123'", ranges[2].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"a1=""456""", ranges[2].ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_CloseSelf_Prefix() {
            string xtmlText = @"holder[x:a /]holder[x:b a0=""123""/][x:c a0=""123"" a1='456' /]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_onePrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<string>(@"[x:a /]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:b a0=""123""/]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[x:c a0=""123"" a1='456' /]", ranges[2].RangeText);

            Assert.AreEqual<int>(0, ranges[0].ChildRanges.Count());

            Assert.AreEqual<int>(1, ranges[1].ChildRanges.Count());
            Assert.AreEqual<string>(@"a0=""123""", ranges[1].ChildRanges.ElementAt(0).RangeText);

            Assert.AreEqual<int>(2, ranges[2].ChildRanges.Count());
            Assert.AreEqual<string>(@"a0=""123""", ranges[2].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"a1='456'", ranges[2].ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting() {
            string xtmlText = @"holder[a a0=""value0"" /][e][b]123[c /]456[d d0=""value0""]123[/d][/b][/e]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(2, ranges.Count);
            Assert.AreEqual<int>(1, ranges[1].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[1].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[a a0=""value0"" /]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[e][b]123[c /]456[d d0=""value0""]123[/d][/b][/e]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[b]123[c /]456[d d0=""value0""]123[/d][/b]", ranges[1].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[c /]", ranges[1].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[d d0=""value0""]123[/d]", ranges[1].ChildRanges.ElementAt(0).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting_Prefix() {
            string xtmlText = @"holder[x:a a0=""value0"" /][x:e][x:b]123[x:c /]456[x:d d0='value0' d1=""value1""]123[/x:d][/x:b][/x:e]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_onePrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(2, ranges.Count);
            Assert.AreEqual<int>(1, ranges[1].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[1].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[x:a a0=""value0"" /]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:e][x:b]123[x:c /]456[x:d d0='value0' d1=""value1""]123[/x:d][/x:b][/x:e]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[x:b]123[x:c /]456[x:d d0='value0' d1=""value1""]123[/x:d][/x:b]", ranges[1].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:c /]", ranges[1].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:d d0='value0' d1=""value1""]123[/x:d]", ranges[1].ChildRanges.ElementAt(0).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting_Declaration() {
            string xtmlText = @"[?xtml version='1.0' encoding=""UTF-8"" ?]holder[a a0=""value0"" /][e][b][c /][d d0='value0']123[/d][/b][/e]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<int>(1, ranges[2].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[2].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[?xtml version='1.0' encoding=""UTF-8"" ?]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[a a0=""value0"" /]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[e][b][c /][d d0='value0']123[/d][/b][/e]", ranges[2].RangeText);
            Assert.AreEqual<string>(@"[b][c /][d d0='value0']123[/d][/b]", ranges[2].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[c /]", ranges[2].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[d d0='value0']123[/d]", ranges[2].ChildRanges.ElementAt(0).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting_Declaration_Prefix() {
            string xtmlText = @"[?xtml version=""1.0"" encoding='UTF-8' ?]holder[x:a a0='value0' /][x:e][x:b][x:c /][x:d d0=""value0""]123[/x:d][/x:b][/x:e]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_onePrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<int>(1, ranges[2].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[2].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[?xtml version=""1.0"" encoding='UTF-8' ?]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:a a0='value0' /]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[x:e][x:b][x:c /][x:d d0=""value0""]123[/x:d][/x:b][/x:e]", ranges[2].RangeText);
            Assert.AreEqual<string>(@"[x:b][x:c /][x:d d0=""value0""]123[/x:d][/x:b]", ranges[2].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:c /]", ranges[2].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:d d0=""value0""]123[/x:d]", ranges[2].ChildRanges.ElementAt(0).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_Nesting_Declaration_MultiplePrefix() {
            string xtmlText = @"[?xtml version='1.0' encoding='UTF-8' ?]holder[x:a a0=""value0"" /][x:e][y:b][x:c /][y:d d0=""value0""]123[/y:d][/y:b][y:f name=""xphter"" /][/x:e]holder";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_twoPrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(3, ranges.Count);
            Assert.AreEqual<int>(2, ranges[2].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[2].ChildRanges.ElementAt(0).ChildRanges.Count());
            Assert.AreEqual<string>(@"[?xtml version='1.0' encoding='UTF-8' ?]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:a a0=""value0"" /]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"[x:e][y:b][x:c /][y:d d0=""value0""]123[/y:d][/y:b][y:f name=""xphter"" /][/x:e]", ranges[2].RangeText);
            Assert.AreEqual<string>(@"[y:b][x:c /][y:d d0=""value0""]123[/y:d][/y:b]", ranges[2].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[y:f name=""xphter"" /]", ranges[2].ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(@"[x:c /]", ranges[2].ChildRanges.ElementAt(0).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[y:d d0=""value0""]123[/y:d]", ranges[2].ChildRanges.ElementAt(0).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_AttributeValueNesting() {
            string xtmlText = @"[?xtml version='1.0' encoding='UTF-8' ?][x ax0='[z az=""1"" /]' ax1='[z az=""2""][/z]'][y ay0='[z az=""3"" /]' ay1='[z az=`4`][/z]' /][/x]";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_option);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(2, ranges.Count);
            Assert.AreEqual<int>(4, ranges[1].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[1].ChildRanges.ElementAt(3).ChildRanges.Count());
            Assert.AreEqual<string>(@"[?xtml version='1.0' encoding='UTF-8' ?]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x ax0='[z az=""1"" /]' ax1='[z az=""2""][/z]'][y ay0='[z az=""3"" /]' ay1='[z az=`4`][/z]' /][/x]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"ax0='[z az=""1"" /]'", ranges[1].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[z az=""1"" /]", ranges[1].ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(@"ax1='[z az=""2""][/z]'", ranges[1].ChildRanges.ElementAt(2).RangeText);
            Assert.AreEqual<string>(@"[y ay0='[z az=""3"" /]' ay1='[z az=`4`][/z]' /]", ranges[1].ChildRanges.ElementAt(3).RangeText);
            Assert.AreEqual<string>(@"ay0='[z az=""3"" /]'", ranges[1].ChildRanges.ElementAt(3).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"ay1='[z az=`4`][/z]'", ranges[1].ChildRanges.ElementAt(3).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_AttributeValueNesting_Prefix() {
            string xtmlText = @"[?xtml version='1.0' encoding='UTF-8' ?][x:x ax0='[x:z az=""1"" /]' ax1='[x:z az=""2""][/x:z]'][x:y ay0='[x:z az=""3"" /]' ay1='[x:z az=`4`][/x:z]' /][/x:x]";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_onePrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(2, ranges.Count);
            Assert.AreEqual<int>(4, ranges[1].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[1].ChildRanges.ElementAt(3).ChildRanges.Count());
            Assert.AreEqual<string>(@"[?xtml version='1.0' encoding='UTF-8' ?]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:x ax0='[x:z az=""1"" /]' ax1='[x:z az=""2""][/x:z]'][x:y ay0='[x:z az=""3"" /]' ay1='[x:z az=`4`][/x:z]' /][/x:x]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"ax0='[x:z az=""1"" /]'", ranges[1].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:z az=""1"" /]", ranges[1].ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(@"ax1='[x:z az=""2""][/x:z]'", ranges[1].ChildRanges.ElementAt(2).RangeText);
            Assert.AreEqual<string>(@"[x:y ay0='[x:z az=""3"" /]' ay1='[x:z az=`4`][/x:z]' /]", ranges[1].ChildRanges.ElementAt(3).RangeText);
            Assert.AreEqual<string>(@"ay0='[x:z az=""3"" /]'", ranges[1].ChildRanges.ElementAt(3).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"ay1='[x:z az=`4`][/x:z]'", ranges[1].ChildRanges.ElementAt(3).ChildRanges.ElementAt(1).RangeText);
        }

        /// <summary>
        ///A test for Analyze
        ///</summary>
        [TestMethod()]
        public void AnalyzeTest_AttributeValueNesting_MultiplePrefix() {
            string xtmlText = @"[?xtml version='1.0' encoding='UTF-8' ?][x:x ax0='[x:z az=""1"" /]' ax1='[y:z az=""2""][/y:z]'][y:y ay0='[y:z az=""3"" /]' ay1='[x:z az=`4`][/x:z]' /][/x:x]";
            XtmlRangeAnalyzer target = new XtmlRangeAnalyzer(this.m_twoPrefixOption);
            IList<IXtmlRange> ranges = new List<IXtmlRange>(target.Analyze(xtmlText));

            Assert.AreEqual<int>(2, ranges.Count);
            Assert.AreEqual<int>(4, ranges[1].ChildRanges.Count());
            Assert.AreEqual<int>(2, ranges[1].ChildRanges.ElementAt(3).ChildRanges.Count());
            Assert.AreEqual<string>(@"[?xtml version='1.0' encoding='UTF-8' ?]", ranges[0].RangeText);
            Assert.AreEqual<string>(@"[x:x ax0='[x:z az=""1"" /]' ax1='[y:z az=""2""][/y:z]'][y:y ay0='[y:z az=""3"" /]' ay1='[x:z az=`4`][/x:z]' /][/x:x]", ranges[1].RangeText);
            Assert.AreEqual<string>(@"ax0='[x:z az=""1"" /]'", ranges[1].ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"[x:z az=""1"" /]", ranges[1].ChildRanges.ElementAt(1).RangeText);
            Assert.AreEqual<string>(@"ax1='[y:z az=""2""][/y:z]'", ranges[1].ChildRanges.ElementAt(2).RangeText);
            Assert.AreEqual<string>(@"[y:y ay0='[y:z az=""3"" /]' ay1='[x:z az=`4`][/x:z]' /]", ranges[1].ChildRanges.ElementAt(3).RangeText);
            Assert.AreEqual<string>(@"ay0='[y:z az=""3"" /]'", ranges[1].ChildRanges.ElementAt(3).ChildRanges.ElementAt(0).RangeText);
            Assert.AreEqual<string>(@"ay1='[x:z az=`4`][/x:z]'", ranges[1].ChildRanges.ElementAt(3).ChildRanges.ElementAt(1).RangeText);
        }
    }
}
