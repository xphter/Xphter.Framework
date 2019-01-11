using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provides function to analyze the original HTML text.
    /// </summary>
    internal class HtmlRangeAnalyzer {
        /// <summary>
        /// Initializ the HtmlRangeAnalyzer class.
        /// </summary>
        static HtmlRangeAnalyzer() {
            g_openTagPattern = string.Format(@"\<[\w\-\:]+([^\<\>]*)\>");

            g_noCaptureAttributePattern =
                @"\s+[\w\-\:]+\s*\=\s*" +
                "(?:" +
                    @"\""[^\""]*\""" +
                    "|" +
                    @"\'[^\']*\'" +
                    "|" +
                    @"[^\s\<\/\>]*" +
                ")";
            g_noCaptureCommentPattern = @"\<\!\-\-.*?\-\-\>";
            g_noCaptureCloseSelfPattern = string.Format(@"\<[\w\-\:]+(?:{0})*\s*\/\>", g_noCaptureAttributePattern);

            g_declarationPattern = @"\<\!DOCTYPE(?:\s+(?:(?'component'[^\s\""\<\>]+)|\""(?'component'[^\""\<\>]+)\""))*\s*\>";
            g_attributePattern =
                @"\s+(?'attribute'(?'attributeName'[\w\-\:]+)\s*\=\s*" +
                "(?'attributeValue'" +
                    @"\""[^\""]*\""" +
                    "|" +
                    @"\'[^\']*\'" +
                    "|" +
                    @"[^\s\<\/\>]*" +
                "))";
            g_closeExplicitPattern = string.Format(
                "(?>(?:" +
                    "(?:" +
                        "(?'begin'" +
                            "(?'open'" +
                                @"\<" +
                                "(?'nodeName'" +
                                    @"(?'name'[\w\-\:]+)" +
                                ")" +
                                "(?:{0})*" +
                                @"\s*\>" +
                            ")" +
                        @")(?:[^\<\>]|(?:{1})|(?:{2}))*" +
                    ")+" +
                    "(?:" +
                        "(?'inner-open'" +
                            @"\<\/" +
                            @"(?'-name'\k'name')" +
                            @"\>" +
                        @")(?:[^\<\>]|(?:{1})|(?:{2}))*" +
                    ")+" +
                ")+)" +
                "(?(open)(?!))",
                g_attributePattern,
                g_noCaptureCommentPattern,
                g_noCaptureCloseSelfPattern);
            g_commentPattern = @"\<\!\-\-(?'comment'.*?)\-\-\>";
            g_closeSelfPattern = string.Format(@"\<(?'nodeName'[\w\-\:]+)(?:{0})*\s*\/\>", g_attributePattern);
            g_openOnlyPattern = string.Format(@"\<(?'nodeName'[\w\-\:]+)(?:{0})*\s*\>", g_attributePattern);

            g_whitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

            g_openTagRegex = new Regex(g_openTagPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            g_noCaptureAttributeRegex = new Regex(g_noCaptureAttributePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            g_declarationRegex = new Regex(g_declarationPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            g_closeExplicitRegex = new Regex(g_closeExplicitPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            g_commentRegex = new Regex(g_commentPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            g_closeSelfRegex = new Regex(g_closeSelfPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            g_openOnlyRegex = new Regex(g_openOnlyPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        private static string g_openTagPattern;

        private static string g_noCaptureAttributePattern;
        private static string g_noCaptureCommentPattern;
        private static string g_noCaptureCloseSelfPattern;

        private static string g_declarationPattern;
        private static string g_attributePattern;
        private static string g_closeExplicitPattern;
        private static string g_commentPattern;
        private static string g_closeSelfPattern;
        private static string g_openOnlyPattern;

        private static Regex g_whitespaceRegex;

        private static Regex g_openTagRegex;
        private static Regex g_noCaptureAttributeRegex;

        private static Regex g_declarationRegex;
        private static Regex g_closeExplicitRegex;
        private static Regex g_commentRegex;
        private static Regex g_closeSelfRegex;
        private static Regex g_openOnlyRegex;

        private Range GetAttributeValueRange(string htmlText, Capture capture) {
            int startIndex = capture.Index;
            char c = htmlText[startIndex];

            return c == '"' || c == '\'' ? new Range(startIndex + 1, capture.Length - 2) : new Range(startIndex, capture.Length);
        }

        private string NormalizeAttribute(string htmlText) {
            return g_openTagRegex.Replace(htmlText, (openMatch) => {
                if(!openMatch.Groups[1].Success) {
                    return openMatch.Value;
                }

                string attributesText = openMatch.Groups[1].Value;
                if(string.IsNullOrWhiteSpace(attributesText)) {
                    return openMatch.Value;
                }

                if(attributesText.EndsWith("/")) {
                    attributesText = attributesText.Substring(0, attributesText.Length - 1);
                }
                if(string.IsNullOrWhiteSpace(attributesText)) {
                    return openMatch.Value;
                }

                int offset = 0;
                int startIndex = 0;
                bool modified = false;
                string replacement = null;
                string openText = openMatch.Value;
                int originalIndex = openMatch.Groups[1].Index - openMatch.Index;
                foreach(Match match in g_noCaptureAttributeRegex.Matches(attributesText)) {
                    if(match.Index > startIndex) {
                        // may be have multiple alone attribute names
                        replacement = g_whitespaceRegex.Split(attributesText.Substring(startIndex, match.Index - startIndex).Trim()).Select((item) => string.Format(" {0}=\"{0}\"", item)).StringJoin(string.Empty);
                        openText = openText.Remove(originalIndex + startIndex + offset, match.Index - startIndex).Insert(originalIndex + startIndex + offset, replacement);
                        offset += replacement.Length - (match.Index - startIndex);

                        modified = true;
                    }

                    startIndex = match.Index + match.Length;
                }
                if(startIndex < attributesText.Length - 1) {
                    string tail = attributesText.Substring(startIndex).Trim();
                    if(!string.IsNullOrEmpty(tail)) {
                        replacement = g_whitespaceRegex.Split(tail).Select((item) => string.Format(" {0}=\"{0}\"", item)).StringJoin(string.Empty);
                        openText = openText.Remove(originalIndex + startIndex + offset, attributesText.Length - startIndex).Insert(originalIndex + startIndex + offset, replacement);

                        modified = true;
                    }
                }

                return modified ? openText : openMatch.Value;
            });
        }

        private HtmlRange AnalyzeDeclarationRange(string htmlText, int startIndex) {
            Capture componentCapture = null;
            CaptureCollection componentCaptures = null;

            HtmlRange range = null;
            Match match = g_declarationRegex.Match(htmlText, startIndex);
            if(match != null && match.Success) {
                componentCaptures = match.Groups["component"].Captures;

                range = HtmlRange.CreateDeclarationRange(
                    htmlText,
                    new Range(match.Index, match.Length));

                for(int i = 0; i < componentCaptures.Count; i++) {
                    componentCapture = componentCaptures[i];

                    range.AppendChild(HtmlRange.CreateTextRange(
                        htmlText,
                        new Range(componentCapture.Index, componentCapture.Length)));
                }
            }

            return range;
        }

        private IEnumerable<HtmlRange> AnalyzeCommentRanges(string htmlText, int startIndex) {
            Group commentGroup = null;
            HtmlRange range = null;
            ICollection<HtmlRange> ranges = new List<HtmlRange>();
            foreach(Match match in g_commentRegex.Matches(htmlText, startIndex)) {
                commentGroup = match.Groups["comment"];

                range = ranges.AddItem(HtmlRange.CreateCommentRange(
                    htmlText,
                    new Range(match.Index, match.Length)));
                range.AppendChild(HtmlRange.CreateTextRange(
                    htmlText,
                    new Range(commentGroup.Index, commentGroup.Length)));
            }

            return ranges;
        }

        /*
         * Warnning: Can not analyze this case:
         * 
         * <td><a href="/yuming/g4wl.com.cn/">g4wl.com.cn</a><br><span>g4wl.cn</span><br><span>g4wl.com</span><br><span>g4wl.net.cn</span><br><span>g4wl.net</span><br></td>
         * 
         */
        private IEnumerable<HtmlRange> AnalyzeCloseExplicitRanges(string htmlText, int startIndex, IEnumerable<IHtmlRange> commentRanges, out IEnumerable<HtmlRange> allRanges) {
            Capture beginCapture = null;
            Capture nodeNameCapture = null;
            Capture innerCapture = null;
            Capture attributeCapture = null;
            Capture attributeNameCapture = null;
            Capture attributeValueCapture = null;
            CaptureCollection beginCaptures = null;
            CaptureCollection nodeNameCaptures = null;
            IEnumerable<Capture> innerCaptures = null;
            CaptureCollection attributeCaptures = null;
            CaptureCollection attributeNameCaptures = null;
            CaptureCollection attributeValueCaptures = null;

            Range openRange = null;
            HtmlRange range = null, previousRange = null, attributeRange = null;
            IList<HtmlRange> currentRanges = new List<HtmlRange>();
            IList<HtmlRange> attributeRanges = new List<HtmlRange>();
            ICollection<HtmlRange> rootRanges = new List<HtmlRange>();
            ICollection<HtmlRange> allRangesInternal = new List<HtmlRange>();
            foreach(Match match in g_closeExplicitRegex.Matches(htmlText, startIndex)) {
                if(commentRanges.Where((item) => item.RangeValue.Contains(new Range(match.Index, match.Length))).FirstOrDefault() != null) {
                    continue;
                }

                beginCaptures = match.Groups["begin"].Captures;
                nodeNameCaptures = match.Groups["nodeName"].Captures;
                attributeCaptures = match.Groups["attribute"].Captures;
                attributeNameCaptures = match.Groups["attributeName"].Captures;
                attributeValueCaptures = match.Groups["attributeValue"].Captures;

                currentRanges.Clear();
                attributeRanges.Clear();
                innerCaptures = match.Groups["inner"].Captures.Cast<Capture>();
                for(int i = 0; i < attributeCaptures.Count; i++) {
                    attributeCapture = attributeCaptures[i];
                    attributeNameCapture = attributeNameCaptures[i];
                    attributeValueCapture = attributeValueCaptures[i];

                    attributeRange = attributeRanges.AddItem(HtmlRange.CreateAttributeRange(
                        htmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length)));
                    attributeRange.AppendChild(HtmlRange.CreateTextRange(
                        htmlText,
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length)));
                    attributeRange.AppendChild(HtmlRange.CreateTextRange(
                        htmlText,
                        this.GetAttributeValueRange(htmlText, attributeValueCapture)));
                }

                for(int i = 0; i < beginCaptures.Count; i++) {
                    beginCapture = beginCaptures[i];
                    nodeNameCapture = nodeNameCaptures[i];
                    innerCapture = innerCaptures.Where((item) => item.Index == beginCapture.Index + beginCapture.Length).First();

                    range = HtmlRange.CreateElementRange(
                        htmlText,
                        new Range(beginCapture.Index, beginCapture.Length + innerCapture.Length + nodeNameCapture.Length + 3),
                        openRange = new Range(beginCapture.Index, beginCapture.Length),
                        new Range(nodeNameCapture.Index, nodeNameCapture.Length),
                        HtmlElementTagFormat.Long);
                    for(int j = currentRanges.Count - 1; j >= 0; j--) {
                        if((previousRange = currentRanges[j]).RangeValue.Contains(range.RangeValue)) {
                            previousRange.AppendChild(range);
                            break;
                        }
                    }
                    for(int j = 0; j < attributeRanges.Count; j++) {
                        if(openRange.Contains((attributeRange = attributeRanges[j]).RangeValue)) {
                            range.AppendChild(attributeRange);
                            attributeRanges.RemoveAt(j--);
                        } else {
                            break;
                        }
                    }

                    allRangesInternal.Add(currentRanges.AddItem(range));
                    if(range.ParentRange == null) {
                        rootRanges.Add(range);
                    }
                }
            }

            allRanges = allRangesInternal;
            return rootRanges;
        }

        private IEnumerable<HtmlRange> AnalyzeNotCloseExplicitRanges(Regex regex, HtmlElementTagFormat tagFormat, string htmlText, int startIndex, IEnumerable<IHtmlRange> commentRanges) {
            Group nodeGroup = null;
            Group nodeNameGroup = null;
            Capture attributeCapture = null;
            Capture attributeNameCapture = null;
            Capture attributeValueCapture = null;
            CaptureCollection attributeCaptures = null;
            CaptureCollection attributeNameCaptures = null;
            CaptureCollection attributeValueCaptures = null;

            HtmlRange range = null, attributeRange = null;
            ICollection<HtmlRange> ranges = new List<HtmlRange>();
            foreach(Match match in regex.Matches(htmlText, startIndex)) {
                if(commentRanges.Where((item) => item.RangeValue.Contains(new Range(match.Index, match.Length))).FirstOrDefault() != null) {
                    continue;
                }

                nodeGroup = match.Groups[0];
                nodeNameGroup = match.Groups["nodeName"];
                attributeCaptures = match.Groups["attribute"].Captures;
                attributeNameCaptures = match.Groups["attributeName"].Captures;
                attributeValueCaptures = match.Groups["attributeValue"].Captures;

                ranges.Add(range = HtmlRange.CreateElementRange(
                    htmlText,
                    new Range(nodeGroup.Index, nodeGroup.Length),
                    new Range(nodeGroup.Index, nodeGroup.Length),
                    new Range(nodeNameGroup.Index, nodeNameGroup.Length),
                    tagFormat));

                for(int i = 0; i < attributeCaptures.Count; i++) {
                    attributeCapture = attributeCaptures[i];
                    attributeNameCapture = attributeNameCaptures[i];
                    attributeValueCapture = attributeValueCaptures[i];

                    range.AppendChild(attributeRange = HtmlRange.CreateAttributeRange(
                        htmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length)));
                    attributeRange.AppendChild(HtmlRange.CreateTextRange(
                        htmlText,
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length)));
                    attributeRange.AppendChild(HtmlRange.CreateTextRange(
                        htmlText,
                        this.GetAttributeValueRange(htmlText, attributeValueCapture)));
                }
            }

            return ranges;
        }

        /// <summary>
        /// Normalizes the specified HTML text.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public string Normalize(string htmlText) {
            htmlText = this.NormalizeAttribute(htmlText);
            return htmlText;
        }

        /// <summary>
        /// Analyze the specified HTML text.
        /// </summary>
        /// <param name="htmlText">A HTML string.</param>
        /// <returns>All ranges found in <paramref name="htmlText"/>.</returns>
        public IEnumerable<IHtmlRange> Analyze(string htmlText, int startIndex) {
            if(htmlText == null) {
                throw new ArgumentException("The original HTML text is null.", "htmlText");
            }
            if(startIndex < 0 || startIndex > htmlText.Length) {
                throw new ArgumentOutOfRangeException("startIndex", "The start index is out of range.");
            }
            if(string.IsNullOrWhiteSpace(htmlText)) {
                return Enumerable.Empty<IHtmlRange>();
            }

            HtmlRange parentRange = null;
            List<IHtmlRange> rootRanges = new List<IHtmlRange>();

            HtmlRange declarationRange = this.AnalyzeDeclarationRange(htmlText, startIndex);
            if(declarationRange != null) {
                rootRanges.Add(declarationRange);
                startIndex += Convert.ToInt32(declarationRange.RangeValue.Length);
            }

            IEnumerable<HtmlRange> commentRanges = this.AnalyzeCommentRanges(htmlText, startIndex);

            IEnumerable<HtmlRange> closeExplicitRanges = null;
            rootRanges.AddRange(this.AnalyzeCloseExplicitRanges(htmlText, startIndex, commentRanges, out closeExplicitRanges));
            closeExplicitRanges = closeExplicitRanges.Reverse().ToArray();

            foreach(HtmlRange item in commentRanges) {
                if((parentRange = closeExplicitRanges.Where((tag) => tag.RangeValue.Contains(item.RangeValue)).FirstOrDefault()) != null) {
                    parentRange.AppendChild(item);
                } else {
                    rootRanges.Add(item);
                }
            }

            foreach(HtmlRange item in this.AnalyzeNotCloseExplicitRanges(g_closeSelfRegex, HtmlElementTagFormat.Short, htmlText, startIndex, commentRanges)) {
                if((parentRange = closeExplicitRanges.Where((tag) => tag.RangeValue.Contains(item.RangeValue)).FirstOrDefault()) != null) {
                    parentRange.AppendChild(item);
                } else {
                    rootRanges.Add(item);
                }
            }

            foreach(HtmlRange item in this.AnalyzeNotCloseExplicitRanges(g_openOnlyRegex, HtmlElementTagFormat.Error, htmlText, startIndex, commentRanges)) {
                if((parentRange = closeExplicitRanges.Where((tag) => tag.RangeValue.Contains(item.RangeValue)).FirstOrDefault()) != null) {
                    if(parentRange.ContentRange.Contains(item.RangeValue)) {
                        parentRange.AppendChild(item);
                    }
                } else {
                    rootRanges.Add(item);
                }
            }

            foreach(HtmlRange item in closeExplicitRanges) {
                item.Normalize();
            }

            return rootRanges.OrderBy((item) => item);
        }

        /// <summary>
        /// Analyze the specified HTML text.
        /// </summary>
        /// <param name="htmlText">A HTML string.</param>
        /// <returns>All ranges found in <paramref name="htmlText"/>.</returns>
        public IEnumerable<IHtmlRange> Analyze(string htmlText) {
            return this.Analyze(htmlText, 0);
        }
    }

    /// <summary>
    /// Represents the range in the original HTML document.
    /// </summary>
    internal interface IHtmlRange : IComparable<IHtmlRange> {
        /// <summary>
        /// Gets the original HTML text.
        /// </summary>
        string HtmlText {
            get;
        }

        /// <summary>
        /// Gets the range value.
        /// </summary>
        Range RangeValue {
            get;
        }

        /// <summary>
        /// Gets the range text.
        /// </summary>
        string RangeText {
            get;
        }

        /// <summary>
        /// Gets the range value of HTML content.
        /// </summary>
        Range ContentRange {
            get;
        }

        /// <summary>
        /// Gets or sets the parent range of this range.
        /// </summary>
        IHtmlRange ParentRange {
            get;
        }

        /// <summary>
        /// Gets child ranges of this range.
        /// </summary>
        IEnumerable<IHtmlRange> ChildRanges {
            get;
        }

        /// <summary>
        /// Transfers this range to a IHtmlNode object.
        /// </summary>
        /// <returns></returns>
        IHtmlNode ToNode();
    }

    /// <summary>
    /// Provides a default implementation of IHtmlRange interface.
    /// </summary>
    internal class HtmlRange : IHtmlRange {
        /// <summary>
        /// Initialize a new instance of HtmlRange.
        /// </summary>
        private HtmlRange() {
            this.m_childRanges = new ChildCollection<HtmlRange, HtmlRange>(
                this,
                (parent, child) => child.ParentRange = parent,
                (parent, child) => child.ParentRange = null);
        }

        /// <summary>
        /// Gets or sets the IHtmlRangeToNode interface.
        /// </summary>
        public IHtmlRangeToNode RangeToNode {
            get;
            set;
        }

        /// <summary>
        /// Appends the specified item to child ranges list.
        /// </summary>
        /// <param name="item"></param>
        public void AppendChild(HtmlRange item) {
            this.m_childRanges.Add(item);
        }

        /// <summary>
        /// Sets the child nodes to the correct order.
        /// </summary>
        public void Normalize() {
            IEnumerable<HtmlRange> childRanges = new List<HtmlRange>(this.m_childRanges).OrderBy((item) => item);
            this.m_childRanges.Clear();
            foreach(HtmlRange item in childRanges) {
                this.m_childRanges.Add(item);
            }
        }

        #region IHtmlRange Members

        /// <inheritdoc />
        public string HtmlText {
            get;
            set;
        }

        /// <inheritdoc />
        public Range RangeValue {
            get;
            set;
        }

        protected string m_rangeText;
        /// <inheritdoc />
        public virtual string RangeText {
            get {
                return this.m_rangeText ?? (this.m_rangeText = this.HtmlText.Substring(Convert.ToInt32(this.RangeValue.StartIndex), Convert.ToInt32(this.RangeValue.Length)));
            }
        }

        /// <inheritdoc />
        public Range ContentRange {
            get;
            set;
        }

        /// <inheritdoc />
        public IHtmlRange ParentRange {
            get;
            set;
        }

        protected ChildCollection<HtmlRange, HtmlRange> m_childRanges;
        /// <inheritdoc />
        public IEnumerable<IHtmlRange> ChildRanges {
            get {
                return this.m_childRanges;
            }
        }

        /// <inheritdoc />
        public IHtmlNode ToNode() {
            return this.RangeToNode != null ? this.RangeToNode.ToNode(this) : null;
        }

        #endregion

        #region IComparable<IHtmlRange> Members

        /// <inheritdoc />
        public int CompareTo(IHtmlRange other) {
            return this.RangeValue.CompareTo(other.RangeValue);
        }

        #endregion

        #region Object Members

        public override int GetHashCode() {
            return this.RangeValue.GetHashCode();
        }

        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is HtmlRange)) {
                return false;
            }

            return this.RangeValue.Equals(((HtmlRange) obj).RangeValue);
        }

        public override string ToString() {
            return this.RangeText;
        }

        #endregion

        /// <summary>
        /// Creates a range object for declaration node.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="nodeRange"></param>
        /// <returns>Returns the created IHtmlRange object.</returns>
        public static HtmlRange CreateDeclarationRange(string htmlText, Range nodeRange) {
            return new HtmlRange {
                HtmlText = htmlText,
                RangeValue = nodeRange,
                ContentRange = nodeRange,
                RangeToNode = HtmlRangeToDeclaration.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for text node.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="nodeRange"></param>
        /// <returns>Returns the created IHtmlRange object.</returns>
        public static HtmlRange CreateTextRange(string htmlText, Range nodeRange) {
            return new HtmlRange {
                HtmlText = htmlText,
                RangeValue = nodeRange,
                ContentRange = nodeRange,
                RangeToNode = HtmlRangeToText.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for comment node.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="nodeRange"></param>
        /// <returns>Returns the created IHtmlRange object.</returns>
        public static HtmlRange CreateCommentRange(string htmlText, Range nodeRange) {
            return new HtmlRange {
                HtmlText = htmlText,
                RangeValue = nodeRange,
                ContentRange = nodeRange,
                RangeToNode = HtmlRangeToComment.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for attribute node.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="nodeRange"></param>
        /// <returns>Returns the created IHtmlRange object.</returns>
        public static HtmlRange CreateAttributeRange(string htmlText, Range nodeRange) {
            return new HtmlRange {
                HtmlText = htmlText,
                RangeValue = nodeRange,
                ContentRange = nodeRange,
                RangeToNode = HtmlRangeToAttribute.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for element node.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="nodeRange"></param>
        /// <param name="openRange"></param>
        /// <param name="nameRange"></param>
        /// <param name="tagFormat"></param>
        /// <returns>Returns the created IHtmlRange object.</returns>
        public static HtmlRange CreateElementRange(string htmlText, Range nodeRange, Range openRange, Range nameRange, HtmlElementTagFormat tagFormat) {
            Range contentRange = null;
            switch(tagFormat) {
                case HtmlElementTagFormat.Long:
                    contentRange = new Range(openRange.StartIndex + openRange.Length, nodeRange.Length - openRange.Length - nameRange.Length - 3);
                    break;
                default:
                    contentRange = Range.Empty;
                    break;
            }

            return new HtmlRange {
                HtmlText = htmlText,
                RangeValue = nodeRange,
                ContentRange = contentRange,
                RangeToNode = new HtmlRangeToElement(openRange, nameRange, tagFormat),
            };
        }

        /// <summary>
        /// Creates a range object for a HTML document.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns>Returns the created IHtmlRange object.</returns>
        public static HtmlRange CreateDocumentRange(string htmlText) {
            Range range = new Range(0, htmlText.Length);
            return new HtmlRange {
                HtmlText = htmlText,
                RangeValue = range,
                ContentRange = range,
                RangeToNode = HtmlRangeToDocument.Instance,
            };
        }
    }

    /// <summary>
    /// Transfers a IHtmlRange to a IHtmlNode.
    /// </summary>
    internal interface IHtmlRangeToNode {
        IHtmlNode ToNode(IHtmlRange range);
    }

    /// <summary>
    /// Transfers a IHtmlRange to a HTML declaration node.
    /// </summary>
    internal class HtmlRangeToDeclaration : IHtmlRangeToNode {
        #region Singleton

        private HtmlRangeToDeclaration() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new HtmlRangeToDeclaration();
            }

            public static HtmlRangeToDeclaration Instance;
        }

        /// <summary>
        /// Gets the unique instance of HtmlRangeToDeclaration class.
        /// </summary>
        public static IHtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IHtmlRangeToNode Members

        /// <inheritdoc />
        public IHtmlNode ToNode(IHtmlRange range) {
            IHtmlDeclaration node = new HtmlDeclaration();
            IList<IHtmlRange> childRanges = new List<IHtmlRange>(range.ChildRanges);
            if(childRanges.Count > 0) {
                node.RootElementName = childRanges[0].RangeText;
            }
            if(childRanges.Count > 1) {
                node.AccessModifier = childRanges[1].RangeText;
            }
            if(childRanges.Count > 2) {
                node.DtdDefinition = childRanges[2].RangeText;
            }
            if(childRanges.Count > 3) {
                node.DtdUri = childRanges[3].RangeText;
            }
            return node;
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IHtmlRange to a HTML text node.
    /// </summary>
    internal class HtmlRangeToText : IHtmlRangeToNode {
        #region Singleton

        private HtmlRangeToText() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new HtmlRangeToText();
            }

            public static HtmlRangeToText Instance;
        }

        /// <summary>
        /// Gets the unique instance of HtmlRangeToText class.
        /// </summary>
        public static IHtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IHtmlRangeToNode Members

        /// <inheritdoc />
        public virtual IHtmlNode ToNode(IHtmlRange range) {
            return new HtmlText {
                Text = range.RangeText,
            };
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IHtmlRange to a HTML comment node.
    /// </summary>
    internal class HtmlRangeToComment : IHtmlRangeToNode {
        #region Singleton

        private HtmlRangeToComment() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new HtmlRangeToComment();
            }

            public static HtmlRangeToComment Instance;
        }

        /// <summary>
        /// Gets the unique instance of HtmlRangeToComment class.
        /// </summary>
        public static IHtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IHtmlRangeToNode Members

        /// <inheritdoc />
        public virtual IHtmlNode ToNode(IHtmlRange range) {
            return new HtmlComment {
                Comment = range.ChildRanges.First().RangeText,
            };
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IHtmlRange to a HTML attribute node.
    /// </summary>
    internal class HtmlRangeToAttribute : IHtmlRangeToNode {
        #region Singleton

        private HtmlRangeToAttribute() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new HtmlRangeToAttribute();
            }

            public static HtmlRangeToAttribute Instance;
        }

        /// <summary>
        /// Gets the unique instance of HtmlRangeToAttribute class.
        /// </summary>
        public static IHtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IHtmlRangeToNode Members

        /// <inheritdoc />
        public IHtmlNode ToNode(IHtmlRange range) {
            IHtmlRange nameRange = range.ChildRanges.First();
            IHtmlRange valueRange = range.ChildRanges.ElementAt(1);
            return new HtmlAttribute(nameRange.RangeText) {
                Value = valueRange.RangeText,
            };
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IHtmlRange to a complex HTML node, for example: a element or a document.
    /// </summary>
    internal abstract class HtmlRangeToComplex : IHtmlRangeToNode {
        /// <summary>
        /// Creates a HtmlNode object.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public abstract HtmlNode CreateNode(IHtmlRange range);

        /// <summary>
        /// Gets the range of all child nodes.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public abstract Range GetChildNodesRange(IHtmlRange range);

        /// <summary>
        /// Creates child nodes.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public virtual IEnumerable<HtmlNode> CreateChildNodes(IHtmlRange range, Range childrenRange) {
            if(childrenRange.Length == 0) {
                return Enumerable.Empty<HtmlNode>();
            }

            Range rangeValue = null;
            long previousIndex = childrenRange.StartIndex;
            ICollection<HtmlNode> childNodes = new List<HtmlNode>();

            foreach(IHtmlRange child in range.ChildRanges) {
                if(!childrenRange.Contains(rangeValue = child.RangeValue)) {
                    continue;
                }

                if(rangeValue.StartIndex > previousIndex) {
                    childNodes.Add(new HtmlText {
                        Text = range.HtmlText.Substring((int) previousIndex, (int) (rangeValue.StartIndex - previousIndex)),
                    });
                }
                childNodes.Add((HtmlNode) child.ToNode());
                previousIndex = rangeValue.StartIndex + rangeValue.Length;
            }
            if(previousIndex <= childrenRange.EndIndex) {
                childNodes.Add(new HtmlText {
                    Text = range.HtmlText.Substring((int) previousIndex, (int) (childrenRange.EndIndex - previousIndex + 1)),
                });
            }

            return childNodes;
        }

        #region IHtmlRangeToNode Members

        /// <inheritdoc />
        public virtual IHtmlNode ToNode(IHtmlRange range) {
            HtmlNode node = this.CreateNode(range);
            if(node != null) {
                foreach(HtmlNode item in this.CreateChildNodes(range, this.GetChildNodesRange(range))) {
                    node.ChildNodes.Add(item);
                }
            }
            return node;
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IHtmlRange to a HTML element node.
    /// </summary>
    internal class HtmlRangeToElement : HtmlRangeToComplex {
        /// <summary>
        /// Initialize a new instance of HtmlRangeToElement class.
        /// </summary>
        /// <param name="openRange"></param>
        /// <param name="nameRange"></param>
        /// <param name="tagFormat"></param>
        public HtmlRangeToElement(Range openRange, Range nameRange, HtmlElementTagFormat tagFormat) {
            this.OpenRange = openRange;
            this.NameRange = nameRange;
            this.TagFormat = tagFormat;
        }

        /// <summary>
        /// Gets the range of open tag in this element.
        /// </summary>
        public Range OpenRange {
            get;
            private set;
        }

        /// <summary>
        /// Gets the range of name in OpenRange.
        /// </summary>
        public Range NameRange {
            get;
            private set;
        }

        /// <summary>
        /// Gets the element tag format of this range.
        /// </summary>
        public HtmlElementTagFormat TagFormat {
            get;
            private set;
        }

        #region HtmlRangeToComplex Members

        /// <inheritdoc />
        public override HtmlNode CreateNode(IHtmlRange range) {
            HtmlElement node = new HtmlElement(range.HtmlText.Substring(Convert.ToInt32(this.NameRange.StartIndex), Convert.ToInt32(this.NameRange.Length)), this.TagFormat);
            foreach(HtmlAttribute item in range.ChildRanges.Where((item) => this.OpenRange.Contains(item.RangeValue)).Select((item) => item.ToNode()).Where((item) => item is HtmlAttribute)) {
                node.Attributes.Add(item);
            }
            return node;
        }

        /// <inheritdoc />
        public override Range GetChildNodesRange(IHtmlRange range) {
            switch(this.TagFormat) {
                case HtmlElementTagFormat.Short:
                    return Range.Empty;
                case HtmlElementTagFormat.Long:
                    return new Range(this.OpenRange.StartIndex + this.OpenRange.Length, range.RangeValue.Length - this.OpenRange.Length - this.NameRange.Length - 3);
                default:
                    return Range.Empty;
            }
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IHtmlRange to a HTML document.
    /// </summary>
    internal class HtmlRangeToDocument : HtmlRangeToComplex {
        #region Singleton

        private HtmlRangeToDocument() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new HtmlRangeToDocument();
            }

            public static HtmlRangeToDocument Instance;
        }

        /// <summary>
        /// Gets the unique instance of HtmlRangeToDocument class.
        /// </summary>
        public static HtmlRangeToDocument Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region HtmlRangeToComplex Members

        /// <inheritdoc />
        public override HtmlNode CreateNode(IHtmlRange range) {
            return new HtmlDocument();
        }

        /// <inheritdoc />
        public override Range GetChildNodesRange(IHtmlRange range) {
            return range.RangeValue;
        }

        #endregion
    }

    /// <summary>
    /// Represents a node in a HTML document.
    /// </summary>
    public interface IHtmlNode {
        /// <summary>
        /// Gets node name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets node type.
        /// </summary>
        HtmlNodeType NodeType {
            get;
        }

        /// <summary>
        /// Gets or sets the concatenated text values of the node and all its child nodes.
        /// </summary>
        string NodeText {
            get;
        }

        /// <summary>
        /// Gets or sets the markup representing this node and all it's child nodes.
        /// </summary>
        string NodeHtml {
            get;
        }

        /// <summary>
        /// Gets the IHtmlDocument to which this node belongs.
        /// </summary>
        IHtmlDocument Document {
            get;
        }

        /// <summary>
        /// Gets the parent of this node (for nodes that can have parents).
        /// </summary>
        IHtmlNode ParentNode {
            get;
        }

        /// <summary>
        /// Gets all the child nodes of the node.
        /// </summary>
        IList<IHtmlNode> ChildNodes {
            get;
        }
    }

    /// <summary>
    /// Represents the type of a HTML node.
    /// </summary>
    public enum HtmlNodeType {
        /// <summary>
        /// Declaration node.
        /// </summary>
        Declaration = 0x00,

        /// <summary>
        /// Text node.
        /// </summary>
        Text = 0x01,

        /// <summary>
        /// Comment node.
        /// </summary>
        Comment = 0x02,

        /// <summary>
        /// Attribute node.
        /// </summary>
        Attribute = 0x03,

        /// <summary>
        /// Element node.
        /// </summary>
        Element = 0x04,

        /// <summary>
        /// HTML document.
        /// </summary>
        Document = 0x05,
    }

    /// <summary>
    /// Represents a HTML document.
    /// </summary>
    public interface IHtmlDocument : IHtmlNode {
        /// <summary>
        /// Gets document type declaration.
        /// </summary>
        IHtmlDeclaration Declaration {
            get;
        }

        /// <summary>
        /// Creates an IHtmlDeclaration node.
        /// </summary>
        /// <returns></returns>
        IHtmlDeclaration CreateDeclaration();

        /// <summary>
        /// Creates an IHtmlText node.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        IHtmlText CreateTextNode(string text);

        /// <summary>
        /// Creates an IHtmlComment node.
        /// </summary>
        /// <returns></returns>
        IHtmlComment CreateComment();

        /// <summary>
        /// Creates an IHtmlAttribute node with the specified attribute name.
        /// </summary>
        /// <returns></returns>
        IHtmlAttribute CreateAttribute(string name);

        /// <summary>
        /// Creates an IHtmlElement node with the specified element name.
        /// </summary>
        /// <returns></returns>
        IHtmlElement CreateElement(string name, bool isCloself);
    }

    /// <summary>
    /// Represents a HTML declaration node.
    /// </summary>
    public interface IHtmlDeclaration : IHtmlNode {
        /// <summary>
        /// Gets or sets the name of top-level element.
        /// </summary>
        string RootElementName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the access modifier of this resource.
        /// </summary>
        string AccessModifier {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the definition of HTML DTD.
        /// </summary>
        string DtdDefinition {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URI of a HTML DTD file.
        /// </summary>
        string DtdUri {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a HTML text node.
    /// </summary>
    public interface IHtmlText : IHtmlNode {
        /// <summary>
        /// Gets or sets text value of this node.
        /// </summary>
        string Text {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a comment node in a  HTML document.
    /// </summary>
    public interface IHtmlComment : IHtmlNode {
        /// <summary>
        /// Gets or sets the comment content.
        /// </summary>
        string Comment {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a attribute in a HTML element.
    /// </summary>
    public interface IHtmlAttribute : IHtmlNode {
        /// <summary>
        /// Gets or sets attribute value.
        /// </summary>
        string Value {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a HTML element node.
    /// </summary>
    public interface IHtmlElement : IHtmlNode {
        /// <summary>
        /// Gets the tag format of this element.
        /// </summary>
        HtmlElementTagFormat TagFormat {
            get;
        }

        /// <summary>
        /// Gets attributes of this element.
        /// </summary>
        IList<IHtmlAttribute> Attributes {
            get;
        }
    }

    /// <summary>
    /// Represents the tag format of a HTML element.
    /// </summary>
    public enum HtmlElementTagFormat {
        /// <summary>
        /// This HTML element does not close properly.
        /// </summary>
        Error = 0x00,

        /// <summary>
        /// This HTML element close in the short format: "<item />".
        /// </summary>
        Short = 0x01,

        /// <summary>
        /// This HTML element close in the long format: "<item></item>".
        /// </summary>
        Long = 0x02,
    }

    /// <summary>
    /// Provides a default implementation of IHtmlNode interface.
    /// </summary>
    internal abstract class HtmlNode : IHtmlNode {
        #region IHtmlNode Members

        /// <inheritdoc />
        public abstract string Name {
            get;
        }

        /// <inheritdoc />
        public abstract HtmlNodeType NodeType {
            get;
        }

        /// <inheritdoc />
        public virtual string NodeText {
            get {
                if(this.ChildNodes == null) {
                    return null;
                }

                return this.ChildNodes.Select((item) => item.NodeText).StringJoin(string.Empty);
            }
        }

        /// <inheritdoc />
        public virtual string NodeHtml {
            get {
                if(this.ChildNodes == null) {
                    return null;
                }

                return this.ChildNodes.Select((item) => item.NodeHtml).StringJoin(string.Empty);
            }
        }

        protected IHtmlDocument m_document;
        /// <inheritdoc />
        public IHtmlDocument Document {
            get {
                return this.m_document;
            }
            set {
                this.m_document = value;
                if(this.ChildNodes != null) {
                    foreach(HtmlNode item in this.ChildNodes) {
                        item.Document = value;
                    }
                }
            }
        }

        /// <inheritdoc />
        public virtual IHtmlNode ParentNode {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual IList<IHtmlNode> ChildNodes {
            get {
                return null;
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return object.ReferenceEquals(this, obj);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Name;
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IHtmlDeclaration interface.
    /// </summary>
    internal class HtmlDeclaration : HtmlNode, IHtmlDeclaration {
        /// <inheritdoc />
        public override string Name {
            get {
                return "#declaration";
            }
        }

        /// <inheritdoc />
        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Declaration;
            }
        }

        /// <inheritdoc />
        public override string NodeHtml {
            get {
                StringBuilder html = new StringBuilder("<!DOCTYPE");
                if(this.RootElementName != null) {
                    html.AppendFormat(" {0}", this.RootElementName);
                }
                if(this.AccessModifier != null) {
                    html.AppendFormat(" {0}", this.AccessModifier);
                }
                if(this.DtdDefinition != null) {
                    html.AppendFormat(" \"{0}\"", this.DtdDefinition);
                }
                if(this.DtdUri != null) {
                    html.AppendFormat(" \"{0}\"", this.DtdUri);
                }
                html.Append('>');

                return html.ToString();
            }
        }

        #region IHtmlDeclaration Members

        /// <inheritdoc />
        public string RootElementName {
            get;
            set;
        }

        /// <inheritdoc />
        public string AccessModifier {
            get;
            set;
        }

        /// <inheritdoc />
        public string DtdDefinition {
            get;
            set;
        }

        /// <inheritdoc />
        public string DtdUri {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Represents a node in a HTML document which only contains text content.
    /// </summary>
    internal abstract class HtmlCharacter : HtmlNode {
        /// <summary>
        /// the text content.
        /// </summary>
        protected string m_content;

        /// <inheritdoc />
        public override string NodeText {
            get {
                return this.m_content;
            }
        }

        /// <inheritdoc />
        public override string NodeHtml {
            get {
                return this.m_content;
            }
        }
    }

    /// <summary>
    /// Provides a default implementation of IHtmlText interface.
    /// </summary>
    internal class HtmlText : HtmlCharacter, IHtmlText {
        /// <inheritdoc />
        public override string Name {
            get {
                return "#text";
            }
        }

        /// <inheritdoc />
        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Text;
            }
        }

        #region IHtmlText Members

        /// <inheritdoc />
        public string Text {
            get {
                return this.m_content;
            }
            set {
                this.m_content = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IHtmlComment interface.
    /// </summary>
    internal class HtmlComment : HtmlCharacter, IHtmlComment {
        /// <inheritdoc />
        public override string Name {
            get {
                return "#comment";
            }
        }

        /// <inheritdoc />
        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Comment;
            }
        }

        /// <inheritdoc />
        public override string NodeText {
            get {
                return null;
            }
        }

        /// <inheritdoc />
        public override string NodeHtml {
            get {
                return string.Format("<!--{0}-->", this.m_content);
            }
        }

        #region IHtmlComment Members

        /// <inheritdoc />
        public string Comment {
            get {
                return this.m_content;
            }
            set {
                this.m_content = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IHtmlAttribute interface.
    /// </summary>
    internal class HtmlAttribute : HtmlNode, IHtmlAttribute {
        /// <summary>
        /// Initialize a instance of HtmlAttribute class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public HtmlAttribute(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("argument name is null or empty.", "name");
            }

            this.m_name = name;
        }

        private string m_name;
        /// <inheritdoc />
        public override string Name {
            get {
                return this.m_name;
            }
        }

        /// <inheritdoc />
        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Attribute;
            }
        }

        /// <inheritdoc />
        public override string NodeHtml {
            get {
                if(this.Value != null && !this.Value.Contains('"')) {
                    return string.Format("{0}=\"{1}\"", this.m_name, this.Value);
                } else {
                    return string.Format("{0}='{1}'", this.m_name, this.Value);
                }
            }
        }

        #region IHtmlAttribute Members

        /// <inheritdoc />
        public string Value {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IHtmlElement interface.
    /// </summary>
    internal class HtmlElement : HtmlNode, IHtmlElement {
        /// <summary>
        /// Initialize a instance of HtmlElement class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="tagformat">Tag format.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public HtmlElement(string name, HtmlElementTagFormat tagformat) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("element name is null or empty.", "name");
            }

            this.m_name = name;
            switch(this.m_tagFormat = tagformat) {
                case HtmlElementTagFormat.Long:
                    this.m_childNodes = new ChildList<IHtmlElement, IHtmlNode>(
                        this,
                        (parent, child) => {
                            ((HtmlNode) child).Document = this.Document;
                            ((HtmlNode) child).ParentNode = parent;
                        },
                        (parent, child) => {
                            ((HtmlNode) child).Document = null;
                            ((HtmlNode) child).ParentNode = null;
                        });
                    break;
            }
            this.m_attributes = new ChildList<IHtmlElement, IHtmlAttribute>(
                this,
                (parent, child) => {
                    ((HtmlNode) child).Document = this.Document;
                    ((HtmlAttribute) child).ParentNode = parent;
                },
                (parent, child) => {
                    ((HtmlNode) child).Document = null;
                    ((HtmlAttribute) child).ParentNode = null;
                });
        }

        private string m_name;
        /// <inheritdoc />
        public override string Name {
            get {
                return this.m_name;
            }
        }

        /// <inheritdoc />
        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Element;
            }
        }

        /// <inheritdoc />
        public override string NodeHtml {
            get {
                switch(this.m_tagFormat) {
                    case HtmlElementTagFormat.Short:
                        return this.GetOpenHtml() + " />";
                    case HtmlElementTagFormat.Long:
                        return string.Format("{0}>{1}</{2}>", this.GetOpenHtml(), base.NodeHtml, this.m_name);
                    default:
                        return this.GetOpenHtml() + ">";
                }
            }
        }

        /// <inheritdoc />
        public override IHtmlNode ParentNode {
            get {
                return base.ParentNode;
            }
            set {
                foreach(HtmlAttribute item in this.m_attributes) {
                    item.Document = this.Document;
                }
                base.ParentNode = value;
            }
        }

        private ChildList<IHtmlElement, IHtmlNode> m_childNodes;
        /// <inheritdoc />
        public override IList<IHtmlNode> ChildNodes {
            get {
                return this.m_childNodes;
            }
        }

        /// <summary>
        /// Gets HTML text of open tag of this element.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOpenHtml() {
            StringBuilder html = new StringBuilder("<" + this.m_name);
            foreach(IHtmlAttribute item in this.m_attributes) {
                html.AppendFormat(" {0}", item.NodeHtml);
            }

            return html.ToString();
        }

        #region IHtmlElement Members

        private HtmlElementTagFormat m_tagFormat;
        /// <inheritdoc />
        public HtmlElementTagFormat TagFormat {
            get {
                return this.m_tagFormat;
            }
        }

        private ChildList<IHtmlElement, IHtmlAttribute> m_attributes;
        /// <inheritdoc />
        public IList<IHtmlAttribute> Attributes {
            get {
                return this.m_attributes;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IHtmlDocument interface.
    /// </summary>
    internal class HtmlDocument : HtmlNode, IHtmlDocument {
        /// <summary>
        /// Initialize a instance of HtmlDocument class.
        /// </summary>
        public HtmlDocument() {
            this.Document = this;
            this.m_childNodes = new ChildList<IHtmlDocument, IHtmlNode>(
                this,
                (parent, child) => {
                    ((HtmlNode) child).Document = parent;
                    ((HtmlNode) child).ParentNode = parent;
                },
                (parent, child) => {
                    ((HtmlNode) child).Document = null;
                    ((HtmlNode) child).ParentNode = null;
                });
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "#document";
            }
        }

        /// <inheritdoc />
        public override HtmlNodeType NodeType {
            get {
                return HtmlNodeType.Document;
            }
        }

        private ChildList<IHtmlDocument, IHtmlNode> m_childNodes;
        /// <inheritdoc />
        public override IList<IHtmlNode> ChildNodes {
            get {
                return this.m_childNodes;
            }
        }

        #region IHtmlDocument Members

        /// <inheritdoc />
        public IHtmlDeclaration Declaration {
            get {
                return (IHtmlDeclaration) this.ChildNodes.Where((item) => item is IHtmlDeclaration).FirstOrDefault();
            }
        }

        /// <inheritdoc />
        public IHtmlDeclaration CreateDeclaration() {
            return new HtmlDeclaration();
        }

        /// <inheritdoc />
        public IHtmlText CreateTextNode(string text) {
            return new HtmlText {
                Text = text,
            };
        }

        /// <inheritdoc />
        public IHtmlComment CreateComment() {
            return new HtmlComment();
        }

        /// <inheritdoc />
        public IHtmlAttribute CreateAttribute(string name) {
            return new HtmlAttribute(name);
        }

        /// <inheritdoc />
        public IHtmlElement CreateElement(string name, bool isCloself) {
            return new HtmlElement(name, isCloself ? HtmlElementTagFormat.Short : HtmlElementTagFormat.Long);
        }

        #endregion
    }

    /// <summary>
    /// Analyzes a HTML text and create a IHtmlDocument form it.
    /// </summary>
    public static class HtmlAnalyzer {
        /// <summary>
        /// Loads a HTML document from the specified HTML text.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="htmlText"/> is null.</exception>
        public static IHtmlDocument Load(string htmlText) {
            if(htmlText == null) {
                throw new ArgumentException("htmlText is null", "htmlText");
            }

            HtmlRangeAnalyzer analyzer = new HtmlRangeAnalyzer();
            htmlText = analyzer.Normalize(htmlText);
            HtmlRange range = HtmlRange.CreateDocumentRange(htmlText);
            foreach(IHtmlRange item in analyzer.Analyze(htmlText)) {
                range.AppendChild((HtmlRange) item);
            }
            return (IHtmlDocument) range.ToNode();
        }
    }

    /// <summary>
    /// Defines extension methods of IHtmlDocument.
    /// </summary>
    public static class HtmlExtension {
        /// <summary>
        /// Gets all elements by the specified tag name from a HTML document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static IEnumerable<IHtmlElement> GetElementsByTagName(this IHtmlDocument document, string tagName) {
            IHtmlNode current = null, node = null;
            Queue<IHtmlNode> queue = new Queue<IHtmlNode>(new IHtmlNode[] { document });
            while(queue.Count > 0) {
                if((current = queue.Dequeue()).ChildNodes == null) {
                    continue;
                }

                for(int i = 0; i < current.ChildNodes.Count; i++) {
                    node = current.ChildNodes[i];
                    if(node.NodeType == HtmlNodeType.Element &&
                        string.Equals(node.Name, tagName, StringComparison.OrdinalIgnoreCase)) {
                        yield return (IHtmlElement) node;
                    }

                    queue.Enqueue(node);
                }
            }

            yield break;
        }

        /// <summary>
        /// Gets all descendant elements by the specified tag name from a HTML element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static IEnumerable<IHtmlElement> GetElementsByTagName(this IHtmlElement element, string tagName) {
            IHtmlNode current = null, node = null;
            Queue<IHtmlNode> queue = new Queue<IHtmlNode>(new IHtmlNode[] { element });
            while(queue.Count > 0) {
                if((current = queue.Dequeue()).ChildNodes == null) {
                    continue;
                }

                for(int i = 0; i < current.ChildNodes.Count; i++) {
                    node = current.ChildNodes[i];
                    if(node.NodeType == HtmlNodeType.Element &&
                        string.Equals(node.Name, tagName, StringComparison.OrdinalIgnoreCase)) {
                        yield return (IHtmlElement) node;
                    }

                    queue.Enqueue(node);
                }
            }

            yield break;
        }

        /// <summary>
        /// Gets the direct child elements by the specified tag Name of a HTML element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static IEnumerable<IHtmlElement> GetChildElementsByTagName(this IHtmlElement element, string tagName) {
            return element.ChildNodes.Where((item) => item.NodeType == HtmlNodeType.Element && string.Equals(item.Name, tagName, StringComparison.OrdinalIgnoreCase)).Cast<IHtmlElement>();
        }

        /// <summary>
        /// Gets all attributes by the specified name from a HTML element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<IHtmlAttribute> GetAttributesByName(this IHtmlElement element, string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("attribute name is null or empty.", "name");
            }

            return element.Attributes.Where((item) => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a value to indicate whether a HTML element has the specified attribute.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttribute(this IHtmlElement element, string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("attribute name is null or empty.", "name");
            }

            return element.Attributes.Any((item) => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
