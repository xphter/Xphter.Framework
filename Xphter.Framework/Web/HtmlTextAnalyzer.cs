using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Represents a tool for analyzing HTML text.
    /// </summary>
    public class HtmlTextAnalyzer {
        /// <summary>
        /// Initializ the HtmlTextAnalyzer class.
        /// </summary>
        static HtmlTextAnalyzer() {
            g_noCaptureAttributePattern =
                @"\s+\w+\s*\=\s*" +
                "(?:" +
                    @"\""[^\""\<\>]*\""" +
                    "|" +
                    @"\'[^\'\<\>]*\'" +
                    "|" +
                    @"[^\s\<\>]*" +
                ")";
            g_noCaptureCloseSelfPattern = string.Format(@"\<[\w\-\:]+(?:{0})*\s*\/\>", g_noCaptureAttributePattern);

            g_attributePattern =
                @"\s+(?'attribute'(?'attributeName'\w+)\s*\=\s*" +
                "(?'attributeValue'" +
                    @"\""[^\""\<\>]*\""" +
                    "|" +
                    @"\'[^\'\<\>]*\'" +
                    "|" +
                    @"[^\s\<\>]*" +
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
                        @")(?:[^\<\>]|(?:{1}))*" +
                    ")+" +
                    "(?:" +
                        "(?'inner-open'" +
                            @"\<\/" +
                            @"(?'-name'\k'name')" +
                            @"\>" +
                        @")(?:[^\<\>]|(?:{1}))*" +
                    ")+" +
                ")+)" +
                "(?(open)(?!))",
                g_attributePattern,
                g_noCaptureCloseSelfPattern);
            g_closeSelfPattern = string.Format(@"\<(?'nodeName'[\w\-\:]+)(?:{0})*\s*\/\>", g_attributePattern);
            g_openOnlyPattern = string.Format(@"\<(?'nodeName'[\w\-\:]+)(?:{0})*\s*\>", g_attributePattern);

            g_closeExplicitRegex = new Regex(g_closeExplicitPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            g_closeSelfRegex = new Regex(g_closeSelfPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            g_openOnlyRegex = new Regex(g_openOnlyPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        private static string g_noCaptureAttributePattern;
        private static string g_noCaptureCloseSelfPattern;

        private static string g_attributePattern;
        private static string g_closeExplicitPattern;
        private static string g_closeSelfPattern;
        private static string g_openOnlyPattern;

        private static Regex g_closeExplicitRegex;
        private static Regex g_closeSelfRegex;
        private static Regex g_openOnlyRegex;

        private Range GetAttributeValueRange(string htmlText, Capture capture) {
            int startIndex = capture.Index;
            char c = htmlText[startIndex];

            return c == '"' || c == '\'' ? new Range(startIndex + 1, capture.Length - 2) : new Range(startIndex, capture.Length);
        }

        /// <summary>
        /// Find close-explicit tags.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="startIndex"></param>
        /// <param name="allTags"></param>
        /// <returns></returns>
        private IEnumerable<HtmlTag> FindCloseExplicitTags(string htmlText, int startIndex, out IEnumerable<HtmlTag> allTags) {
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
            HtmlTag tag = null, previousTag = null;
            HtmlAttribute attribute = null;
            IList<HtmlTag> currentTags = new List<HtmlTag>();
            IList<HtmlAttribute> attributes = new List<HtmlAttribute>();
            ICollection<HtmlTag> rootTags = new List<HtmlTag>();
            ICollection<HtmlTag> allTagsInternal = new List<HtmlTag>();
            foreach(Match match in g_closeExplicitRegex.Matches(htmlText, startIndex)) {
                beginCaptures = match.Groups["begin"].Captures;
                nodeNameCaptures = match.Groups["nodeName"].Captures;
                attributeCaptures = match.Groups["attribute"].Captures;
                attributeNameCaptures = match.Groups["attributeName"].Captures;
                attributeValueCaptures = match.Groups["attributeValue"].Captures;

                currentTags.Clear();
                attributes.Clear();
                innerCaptures = match.Groups["inner"].Captures.Cast<Capture>();
                for(int i = 0; i < attributeCaptures.Count; i++) {
                    attributeCapture = attributeCaptures[i];
                    attributeNameCapture = attributeNameCaptures[i];
                    attributeValueCapture = attributeValueCaptures[i];

                    attributes.Add(new HtmlAttribute(
                        htmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length),
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length),
                        this.GetAttributeValueRange(htmlText, attributeValueCapture)));
                }

                for(int i = 0; i < beginCaptures.Count; i++) {
                    beginCapture = beginCaptures[i];
                    nodeNameCapture = nodeNameCaptures[i];
                    innerCapture = innerCaptures.Where((item) => item.Index == beginCapture.Index + beginCapture.Length).First();

                    tag = new HtmlTag(
                        htmlText,
                        new Range(beginCapture.Index, beginCapture.Length + innerCapture.Length + nodeNameCapture.Length + 3),
                        openRange = new Range(beginCapture.Index, beginCapture.Length),
                        new Range(nodeNameCapture.Index, nodeNameCapture.Length),
                        new Range(innerCapture.Index + innerCapture.Length, nodeNameCapture.Length + 3));

                    for(int j = currentTags.Count - 1; j >= 0; j--) {
                        if((previousTag = currentTags[j]).RangeValue.Contains(tag.RangeValue)) {
                            previousTag.AppendChild(tag);
                            break;
                        }
                    }
                    for(int j = 0; j < attributes.Count; j++) {
                        if(openRange.Contains((attribute = attributes[j]).RangeValue)) {
                            tag.AppendAttribute(attribute);
                            attributes.RemoveAt(j--);
                        } else {
                            break;
                        }
                    }

                    allTagsInternal.Add(currentTags.AddItem(tag));
                    if(tag.Parent == null) {
                        rootTags.Add(tag);
                    }
                }
            }

            allTags = allTagsInternal;
            return rootTags;
        }

        /// <summary>
        /// Find close-self or open-only tags.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private IEnumerable<HtmlTag> FindNotCloseExplicitTags(Regex regex, string htmlText, int startIndex) {
            Group nodeGroup = null;
            Group nodeNameGroup = null;
            Capture attributeCapture = null;
            Capture attributeNameCapture = null;
            Capture attributeValueCapture = null;
            CaptureCollection attributeCaptures = null;
            CaptureCollection attributeNameCaptures = null;
            CaptureCollection attributeValueCaptures = null;

            HtmlTag tag = null;
            ICollection<HtmlTag> tags = new List<HtmlTag>();
            foreach(Match match in regex.Matches(htmlText, startIndex)) {
                nodeGroup = match.Groups[0];
                nodeNameGroup = match.Groups["nodeName"];
                attributeCaptures = match.Groups["attribute"].Captures;
                attributeNameCaptures = match.Groups["attributeName"].Captures;
                attributeValueCaptures = match.Groups["attributeValue"].Captures;

                tags.Add(tag = new HtmlTag(
                    htmlText,
                    new Range(nodeGroup.Index, nodeGroup.Length),
                    new Range(nodeGroup.Index, nodeGroup.Length),
                    new Range(nodeNameGroup.Index, nodeNameGroup.Length),
                    Range.Empty));

                for(int i = 0; i < attributeCaptures.Count; i++) {
                    attributeCapture = attributeCaptures[i];
                    attributeNameCapture = attributeNameCaptures[i];
                    attributeValueCapture = attributeValueCaptures[i];

                    tag.AppendAttribute(new HtmlAttribute(
                        htmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length),
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length),
                        this.GetAttributeValueRange(htmlText, attributeValueCapture)));
                }
            }

            return tags;
        }

        /// <summary>
        /// Analyze the specified HTML text.
        /// </summary>
        /// <param name="htmlText">A HTML string.</param>
        /// <returns>All tags found in <paramref name="htmlText"/>.</returns>
        public IEnumerable<HtmlTag> Analyze(string htmlText, int startIndex) {
            if(htmlText == null) {
                throw new ArgumentException("The original HTML text is null.", "htmlText");
            }
            if(startIndex < 0 || startIndex > htmlText.Length) {
                throw new ArgumentOutOfRangeException("startIndex", "The start index is out of range.");
            }
            if(string.IsNullOrWhiteSpace(htmlText)) {
                return new HtmlTag[0];
            }

            HtmlTag parentTag = null;
            List<HtmlTag> rootTags = new List<HtmlTag>();

            IEnumerable<HtmlTag> closeExplicitTags = null;
            rootTags.AddRange(this.FindCloseExplicitTags(htmlText, startIndex, out closeExplicitTags));
            closeExplicitTags = closeExplicitTags.Reverse();

            foreach(HtmlTag item in this.FindNotCloseExplicitTags(g_closeSelfRegex, htmlText, startIndex)) {
                if((parentTag = closeExplicitTags.Where((tag) => tag.RangeValue.Contains(item.RangeValue)).FirstOrDefault()) != null) {
                    parentTag.AppendChild(item);
                } else {
                    rootTags.Add(item);
                }
            }

            foreach(HtmlTag item in this.FindNotCloseExplicitTags(g_openOnlyRegex, htmlText, startIndex)) {
                if((parentTag = closeExplicitTags.Where((tag) => tag.RangeValue.Contains(item.RangeValue)).FirstOrDefault()) != null) {
                    if(!parentTag.OpenRange.Contains(item.RangeValue)) {
                        parentTag.AppendChild(item);
                    }
                } else {
                    rootTags.Add(item);
                }
            }

            foreach(HtmlTag item in closeExplicitTags) {
                item.Normalize();
            }

            return rootTags.OrderBy((item) => item);
        }

        /// <summary>
        /// Analyze the specified HTML text.
        /// </summary>
        /// <param name="htmlText">A HTML string.</param>
        /// <returns>All tags found in <paramref name="htmlText"/>.</returns>
        public IEnumerable<HtmlTag> Analyze(string htmlText) {
            return this.Analyze(htmlText, 0);
        }

        /// <summary>
        /// Gets all HTML tags of the specified HTML text.
        /// </summary>
        /// <param name="htmlText">A HTML string.</param>
        /// <returns></returns>
        public static IEnumerable<HtmlTag> GetAllTags(string htmlText) {
            HtmlTag tag = null;
            ICollection<HtmlTag> result = new List<HtmlTag>();
            Queue<HtmlTag> queue = new Queue<HtmlTag>(new HtmlTextAnalyzer().Analyze(htmlText));
            while(queue.Count > 0) {
                result.Add(tag = queue.Dequeue());
                tag.ChildTags.ForEach((item) => queue.Enqueue(item));
            }

            return result.OrderBy((item) => item);
        }
    }

    /// <summary>
    /// Represents a range in the HTML document.
    /// </summary>
    public abstract class HtmlRange : IComparable<HtmlRange> {
        /// <summary>
        /// Initialize a new instance of HtmlRange class.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="rangeValue"></param>
        protected HtmlRange(string htmlText, Range rangeValue) {
            this.HtmlText = htmlText;
            this.RangeValue = rangeValue;
        }

        /// <summary>
        /// Gets the orginal HTML string.
        /// </summary>
        public string HtmlText {
            get;
            private set;
        }

        /// <summary>
        /// Gets the range value.
        /// </summary>
        public Range RangeValue {
            get;
            private set;
        }

        private string m_rangeText;
        /// <summary>
        /// Gets the range text.
        /// </summary>
        public string RangeText {
            get {
                return this.m_rangeText ?? (this.m_rangeText = this.HtmlText.Substring((int) this.RangeValue.StartIndex, (int) this.RangeValue.Length));
            }
        }

        #region IComparable Members

        /// <inheritdoc />
        public int CompareTo(HtmlRange other) {
            if(other == null) {
                return 1;
            }

            return this.RangeValue.CompareTo(other.RangeValue);
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.RangeValue.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is HtmlRange)) {
                return false;
            }
            return this.RangeValue.Equals(((HtmlRange) obj).RangeValue);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.RangeText;
        }

        #endregion
    }

    /// <summary>
    /// Represents a HTML tag.
    /// </summary>
    public class HtmlTag : HtmlRange {
        /// <summary>
        /// Initialize a new instance of HtmlTag class.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="rangeValue"></param>
        /// <param name="openRange"></param>
        /// <param name="nameRange"></param>
        /// <param name="closeRange"></param>
        internal HtmlTag(string htmlText, Range rangeValue, Range openRange, Range nameRange, Range closeRange)
            : base(htmlText, rangeValue) {
            this.OpenRange = openRange;
            this.NameRange = nameRange;
            this.CloseRange = closeRange;

            long innerStartIndex = this.OpenRange.StartIndex + this.OpenRange.Length;
            this.InnerRange = new Range(innerStartIndex, this.CloseRange.StartIndex - innerStartIndex);
        }

        /// <summary>
        /// Child tags.
        /// </summary>
        private ICollection<HtmlTag> m_childTags = new List<HtmlTag>();

        /// <summary>
        /// Attributes.
        /// </summary>
        private ICollection<HtmlAttribute> m_attributes = new List<HtmlAttribute>();

        /// <summary>
        /// Get the range of open tag.
        /// </summary>
        public Range OpenRange {
            get;
            private set;
        }

        private string m_openText;
        /// <summary>
        /// Gets the text of open tag.
        /// </summary>
        public string OpenText {
            get {
                return m_openText ?? (m_openText = this.HtmlText.Substring((int) this.OpenRange.StartIndex, (int) this.OpenRange.Length));
            }
        }

        /// <summary>
        /// Gets the range of tag name.
        /// </summary>
        public Range NameRange {
            get;
            private set;
        }

        private string m_name;
        /// <summary>
        /// Gets tag name.
        /// </summary>
        public string Name {
            get {
                return this.m_name ?? (this.m_name = this.HtmlText.Substring((int) this.NameRange.StartIndex, (int) this.NameRange.Length));
            }
        }

        /// <summary>
        /// Gets the range between open tag and close tag.
        /// </summary>
        public Range InnerRange {
            get;
            private set;
        }

        private string m_innerText;
        /// <summary>
        /// Gets the text between open tag and close tag.
        /// </summary>
        public string InnerText {
            get {
                return this.m_innerText ?? (this.m_innerText = this.HtmlText.Substring((int) this.InnerRange.StartIndex, (int) this.InnerRange.Length));
            }
        }

        /// <summary>
        /// Gets the range of close tag.
        /// </summary>
        public Range CloseRange {
            get;
            private set;
        }

        private string m_closeText;
        /// <summary>
        /// Gets the text of close tag.
        /// </summary>
        public string CloseText {
            get {
                return this.m_closeText ?? (this.m_closeText = this.HtmlText.Substring((int) this.CloseRange.StartIndex, (int) this.CloseRange.Length));
            }
        }

        /// <summary>
        /// Gets the parent tag.
        /// </summary>
        public HtmlTag Parent {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the child tags list.
        /// </summary>
        public IEnumerable<HtmlTag> ChildTags {
            get {
                return this.m_childTags;
            }
        }

        /// <summary>
        /// Gets the attributes list.
        /// </summary>
        public IEnumerable<HtmlAttribute> Attributes {
            get {
                return this.m_attributes;
            }
        }

        /// <summary>
        /// Append the specified tag into child tags list of this tag.
        /// </summary>
        /// <param name="child">Child tag.</param>
        internal void AppendChild(HtmlTag child) {
            if(child == null) {
                return;
            }

            child.Parent = this;
            this.m_childTags.Add(child);
        }

        /// <summary>
        /// Append the specified attribute into attributes list of this tag.
        /// </summary>
        /// <param name="attribute"></param>
        internal void AppendAttribute(HtmlAttribute attribute) {
            if(attribute == null) {
                return;
            }

            attribute.Tag = this;
            this.m_attributes.Add(attribute);
        }

        /// <summary>
        /// Sets the child nodes to the correct order.
        /// </summary>
        internal void Normalize() {
            IEnumerable<HtmlTag> childTags = new List<HtmlTag>(this.m_childTags).OrderBy((item) => item);
            this.m_childTags.Clear();
            foreach(HtmlTag item in childTags) {
                this.m_childTags.Add(item);
            }
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Name;
        }
    }

    /// <summary>
    /// Represents a attribute in the HTML tag.
    /// </summary>
    public class HtmlAttribute : HtmlRange {
        /// <summary>
        /// Initialize a new instance of HtmlAttribute.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="rangeValue"></param>
        /// <param name="nameRange"></param>
        /// <param name="valueRange"></param>
        internal HtmlAttribute(string htmlText, Range rangeValue, Range nameRange, Range valueRange)
            : base(htmlText, rangeValue) {
            this.NameRange = nameRange;
            this.ValueRange = valueRange;
        }

        /// <summary>
        /// Gets the range of attribute name.
        /// </summary>
        public Range NameRange {
            get;
            private set;
        }

        private string m_name;
        /// <summary>
        /// Gets attribute name.
        /// </summary>
        public string Name {
            get {
                return this.m_name ?? (this.m_name = this.HtmlText.Substring((int) this.NameRange.StartIndex, (int) this.NameRange.Length));
            }
        }

        /// <summary>
        /// Gets the range of attribute value.
        /// </summary>
        public Range ValueRange {
            get;
            private set;
        }

        private string m_value;
        /// <summary>
        /// Gets attribute value.
        /// </summary>
        public string Value {
            get {
                return this.m_value ?? (this.m_value = this.HtmlText.Substring((int) this.ValueRange.StartIndex, (int) this.ValueRange.Length));
            }
        }

        /// <summary>
        /// Gets the tag which contains the attribute.
        /// </summary>
        public HtmlTag Tag {
            get;
            internal set;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0} = \"{1}\"", this.Name, this.Value);
        }
    }
}
