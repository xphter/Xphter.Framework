using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xphter.Framework.Collections;
using Xphter.Framework.IO;
using Xphter.Framework.Reflection;
using Xphter.Framework.Text;
using Xphter.Framework.Web;
using Xphter.Framework.Web.Mvc;

namespace Xphter.Framework.Xtml {
    /// <summary>
    /// Represents the range in the original XTML document.
    /// </summary>
    internal interface IXtmlRange : IComparable<IXtmlRange> {
        /// <summary>
        /// Gets the original XTML text.
        /// </summary>
        string XtmlText {
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
        /// Gets or sets the parent range of this range.
        /// </summary>
        IXtmlRange ParentRange {
            get;
        }

        /// <summary>
        /// Gets child ranges of this range.
        /// </summary>
        IEnumerable<IXtmlRange> ChildRanges {
            get;
        }

        /// <summary>
        /// Transfers this range to a IXtmlNode object.
        /// </summary>
        /// <returns></returns>
        IXtmlNode ToNode();
    }

    /// <summary>
    /// Provides a default implementation of IXtmlRange interface.
    /// </summary>
    internal class XtmlRange : IXtmlRange {
        /// <summary>
        /// Initialize a new instance of XtmlRange.
        /// </summary>
        private XtmlRange() {
            this.m_childRanges = new ChildCollection<XtmlRange, XtmlRange>(
                this,
                (parent, child) => child.ParentRange = parent,
                (parent, child) => child.ParentRange = null);
        }

        /// <summary>
        /// Gets or sets the IXtmlRangeToNode interface.
        /// </summary>
        public IXtmlRangeToNode RangeToNode {
            get;
            set;
        }

        /// <summary>
        /// Appends the specified item to child ranges list.
        /// </summary>
        /// <param name="item"></param>
        public void AppendChild(XtmlRange item) {
            this.m_childRanges.Add(item);
        }

        /// <summary>
        /// Sets the child nodes to the correct order.
        /// </summary>
        public void Normalize() {
            IEnumerable<XtmlRange> childRanges = new List<XtmlRange>(this.m_childRanges).OrderBy((item) => item);
            this.m_childRanges.Clear();
            foreach(XtmlRange item in childRanges) {
                this.m_childRanges.Add(item);
            }
        }

        #region IXtmlRange Members

        /// <inheritdoc />
        public virtual string XtmlText {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual Range RangeValue {
            get;
            set;
        }

        protected string m_rangeText;
        /// <inheritdoc />
        public virtual string RangeText {
            get {
                return this.m_rangeText ?? (this.m_rangeText = this.XtmlText.Substring(Convert.ToInt32(this.RangeValue.StartIndex), Convert.ToInt32(this.RangeValue.Length)));
            }
        }

        /// <inheritdoc />
        public virtual IXtmlRange ParentRange {
            get;
            set;
        }

        protected ChildCollection<XtmlRange, XtmlRange> m_childRanges;
        /// <inheritdoc />
        public virtual IEnumerable<IXtmlRange> ChildRanges {
            get {
                return this.m_childRanges;
            }
        }

        /// <inheritdoc />
        public IXtmlNode ToNode() {
            return this.RangeToNode != null ? this.RangeToNode.ToNode(this) : null;
        }

        #endregion

        #region IComparable<IXtmlRange> Members

        public int CompareTo(IXtmlRange other) {
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
            if(!(obj is XtmlRange)) {
                return false;
            }

            return this.RangeValue.Equals(((XtmlRange) obj).RangeValue);
        }

        public override string ToString() {
            return this.RangeText;
        }

        #endregion

        /// <summary>
        /// Creates a range object for declaration node.
        /// </summary>
        /// <returns>Returns the created IXtmlRange object.</returns>
        public static XtmlRange CreateDeclarationRange(string xtmlText, Range nodeRange) {
            return new XtmlRange {
                XtmlText = xtmlText,
                RangeValue = nodeRange,
                RangeToNode = XtmlRangeToDeclaration.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for text node.
        /// </summary>
        /// <returns>Returns the created IXtmlRange object.</returns>
        public static XtmlRange CreateTextRange(string xtmlText, Range nodeRange) {
            return new XtmlRange {
                XtmlText = xtmlText,
                RangeValue = nodeRange,
                RangeToNode = XtmlRangeToText.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for attribute node.
        /// </summary>
        /// <returns>Returns the created IXtmlRange object.</returns>
        public static XtmlRange CreateAttributeRange(string xtmlText, Range nodeRange) {
            return new XtmlRange {
                XtmlText = xtmlText,
                RangeValue = nodeRange,
                RangeToNode = XtmlRangeToAttribute.Instance,
            };
        }

        /// <summary>
        /// Creates a range object for element node.
        /// </summary>
        /// <returns>Returns the created IXtmlRange object.</returns>
        public static XtmlRange CreateElementRange(string xtmlText, Range nodeRange, Range openRange, Range nameRange, Range closeRange, XtmlNodePrefix prefix) {
            return new XtmlRange {
                XtmlText = xtmlText,
                RangeValue = nodeRange,
                RangeToNode = new XtmlRangeToElement(openRange, nameRange, closeRange, prefix),
            };
        }

        /// <summary>
        /// Creates a range object for a XTML document.
        /// </summary>
        /// <returns>Returns the created IXtmlRange object.</returns>
        public static XtmlRange CreateDocumentRange(string xtmlText) {
            return new XtmlRange {
                XtmlText = xtmlText,
                RangeValue = new Range(0, xtmlText.Length),
                RangeToNode = XtmlRangeToDocument.Instance,
            };
        }
    }

    /// <summary>
    /// Provides the information of the XTML node prefix.
    /// </summary>
    public class XtmlNodePrefix {
        /// <summary>
        /// The char between node prefix and node name.
        /// </summary>
        public const char SEPARATOR_CHAR = ':';

        /// <summary>
        /// Gets or sets the descriptive name of the node prefix.
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of the node prefix.
        /// </summary>
        public string Description {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actual prefix string.
        /// </summary>
        public string Value {
            get;
            set;
        }

        private string m_prefixString;
        /// <summary>
        /// Gets the prefix string used in a XTML document.
        /// </summary>
        public string PrefixString {
            get {
                return m_prefixString ?? (m_prefixString = string.IsNullOrWhiteSpace(this.Value) ? string.Empty : this.Value + SEPARATOR_CHAR);
            }
        }

        /// <summary>
        /// Gets a qualified name from the specified local name.
        /// </summary>
        /// <param name="localName"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="localName"/> is null or empty.</exception>
        public string GetQualifiedName(string localName) {
            if(string.IsNullOrEmpty(localName)) {
                throw new ArgumentException("The local name is null or empty.", "localName");
            }

            return this.PrefixString + localName;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is XtmlNodePrefix)) {
                return false;
            }

            return string.Equals(this.Value, ((XtmlNodePrefix) obj).Value);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.PrefixString;
        }

        #endregion
    }

    /// <summary>
    /// Represents the context for markuping XTML documents.
    /// </summary>
    public class XtmlMarkupContext {
        /// <summary>
        /// Gets or sets the char to mark bengin of a XTML tag.
        /// </summary>
        public char OpenChar {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of open char entity.
        /// </summary>
        public string OpenCharEntityName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the char to mark end of a XTML tag.
        /// </summary>
        public char CloseChar {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of close char entity.
        /// </summary>
        public string CloseCharEntityName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the char to mark a XTML tag that is the end XTML tag.
        /// </summary>
        public char EndChar {
            get;
            set;
        }

        private ICollection<XtmlEntity> m_otherEntities = new List<XtmlEntity>();
        /// <summary>
        /// Gets all other available entities excepts the OpenEntity and CloseEntity.
        /// </summary>
        public ICollection<XtmlEntity> OtherEntities {
            get {
                return this.m_otherEntities;
            }
        }

        /// <summary>
        /// Gets all defined entities.
        /// </summary>
        public IEnumerable<XtmlEntity> Entities {
            get {
                IEnumerable<XtmlEntity> entities = new XtmlEntity[] {
                    new XtmlEntity(this.OpenCharEntityName, Convert.ToString(this.OpenChar)),
                    new XtmlEntity(this.CloseCharEntityName, Convert.ToString(this.CloseChar)),
                };
                if(this.OtherEntities.Count > 0) {
                    entities = entities.Concat(this.OtherEntities);
                }
                return entities;
            }
        }

        /// <summary>
        /// Gets text of the open tag from the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public string GetOpenTag(string qualifiedName, params string[] attributes) {
            if(attributes != null) {
                attributes = attributes.Where((item) => !string.IsNullOrWhiteSpace(item)).ToArray();
            }
            StringBuilder tag = new StringBuilder(this.OpenChar + qualifiedName);
            if(attributes.Length > 0) {
                foreach(string item in attributes) {
                    tag.AppendFormat(" {0}=''", item);
                }
            }
            tag.Append(this.CloseChar);
            return tag.ToString();
        }

        /// <summary>
        /// Gets text of the close tag from the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public string GetCloseTag(string qualifiedName) {
            return string.Format("{0}{1}{2}{3}", this.OpenChar, this.EndChar, qualifiedName, this.CloseChar);
        }

        /// <summary>
        /// Gets text of the self-closing tag from the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public string GetCloselfTag(string qualifiedName, params string[] attributes) {
            if(attributes != null) {
                attributes = attributes.Where((item) => !string.IsNullOrWhiteSpace(item)).ToArray();
            }
            StringBuilder tag = new StringBuilder(this.OpenChar + qualifiedName);
            if(attributes.Length > 0) {
                foreach(string item in attributes) {
                    tag.AppendFormat(" {0}=''", item);
                }
            }
            tag.Append((" " + this.EndChar) + this.CloseChar);
            return tag.ToString();
        }

        /// <summary>
        /// Replaces all XTML entities by their values in the specified XTML string.
        /// </summary>
        /// <param name="xtml"></param>
        /// <returns></returns>
        public string Decode(string xtml) {
            return xtml.Replace(this.Entities.ToDictionary(
                (item) => item.Text,
                (item) => item.Value), false);
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return string.Format("{0}{2}{1}", this.OpenChar, this.CloseChar, this.EndChar).GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is XtmlMarkupContext)) {
                return false;
            }

            XtmlMarkupContext context = (XtmlMarkupContext) obj;
            return this.OpenChar == context.OpenChar &&
                this.CloseChar == context.CloseChar &&
                this.EndChar == context.EndChar;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0}Name{1}{0}{2}Name{1}", this.OpenChar, this.CloseChar, this.EndChar);
        }

        #endregion
    }

    /// <summary>
    /// Provides options for analyze a XTML document.
    /// </summary>
    public class XtmlDocumentOption {
        public XtmlDocumentOption() {
            this.Prefixs = Enumerable.Empty<XtmlNodePrefix>();
        }

        public XtmlDocumentOption(XtmlMarkupContext context, params XtmlNodePrefix[] prefixes) {
            this.Context = context;
            this.Prefixs = prefixes;
        }

        /// <summary>
        /// Gets or sets the XMTL markup context.
        /// </summary>
        public XtmlMarkupContext Context {
            get;
            set;
        }

        /// <summary>
        /// Gets the XTML node prefixs.
        /// </summary>
        public IEnumerable<XtmlNodePrefix> Prefixs {
            get;
            set;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Context.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is XtmlDocumentOption)) {
                return false;
            }

            XtmlDocumentOption option = (XtmlDocumentOption) obj;
            return this.Context.Equals(option.Context) &&
                this.Prefixs.Equals<XtmlNodePrefix>(option.Prefixs);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Context.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents a entity in a XTML document, such as &amp;.
    /// </summary>
    public class XtmlEntity {
        /// <summary>
        /// Initialize XtmlEntity class.
        /// </summary>
        static XtmlEntity() {
            g_ampersand = new XtmlEntity("amp", "&");
        }

        /// <summary>
        /// Intialize a new instance of XtmlEntity.
        /// </summary>
        /// <param name="name">Entity name.</param>
        /// <param name="value">Entity value.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public XtmlEntity(string name, string value) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("XTML entity name is null or empty.", "name");
            }

            this.Name = name;
            this.Value = value;
            this.Text = string.Format("{0}{1}{2}", BEGIN_CHAR, name, END_CHAR);
        }

        public const char BEGIN_CHAR = '&';
        public const char END_CHAR = ';';

        private static XtmlEntity g_ampersand;
        /// <summary>
        /// Gets the predefined entity for '&' character.
        /// </summary>
        public static XtmlEntity Ampersand {
            get {
                return g_ampersand;
            }
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the entity value.
        /// </summary>
        public string Value {
            get;
            private set;
        }

        /// <summary>
        /// Gets text of this entity.
        /// </summary>
        public string Text {
            get;
            private set;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is XtmlEntity)) {
                return false;
            }

            return this.Name.Equals(((XtmlEntity) obj).Name);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Text;
        }

        #endregion
    }

    /// <summary>
    /// Provides function to analyze the original XTML text.
    /// </summary>
    internal class XtmlRangeAnalyzer {
        /// <summary>
        /// Initialize a new instance of XtmlRangeAnalyzer class.
        /// </summary>
        /// <param name="option">A XtmlDocumentOption object.</param>
        public XtmlRangeAnalyzer(XtmlDocumentOption option) {
            if(option == null) {
                throw new ArgumentException("The XTML document option is null.", "option");
            }

            this.DocumentOption = option;
            this.InitializePatterns();
        }

        private string m_noCapturePrefixPattern;
        private string m_noCaptureAttributePattern;
        private string m_noCaptureCloseSelfPattern;

        private string m_prefixPattern;
        private string m_attributePattern;
        private string m_declarationPattern;
        private string m_closeExplicitPattern;
        private string m_closeSelfPattern;

        private Regex m_declarationRegex;
        private Regex m_closeExplicitRegex;
        private Regex m_closeSelfRegex;

        private IDictionary<string, XtmlNodePrefix> m_prefixsMap;

        /// <summary>
        /// Gets the XTML document option to analyze the original XTML text.
        /// </summary>
        public XtmlDocumentOption DocumentOption {
            get;
            private set;
        }

        /// <summary>
        /// Initializes all patterns.
        /// </summary>
        private void InitializePatterns() {
            this.m_prefixsMap = this.DocumentOption.Prefixs.ToIDictionary((item) => item.Value);

            this.m_noCapturePrefixPattern = this.DocumentOption.Prefixs.Count() > 0 ?
                "(?:" + this.DocumentOption.Prefixs.StringJoin('|', (item) => RegexUtility.Encode(item.Value)) + @")\" + XtmlNodePrefix.SEPARATOR_CHAR :
                string.Empty;
            this.m_noCaptureAttributePattern = string.Format(
                @"\s+[\w\-]+\s*\=\s*(?:\""[^\""]*\""|\'[^\']*\'|\`[^\`]*\`)",
                this.DocumentOption.Context.OpenChar,
                this.DocumentOption.Context.CloseChar);
            this.m_noCaptureCloseSelfPattern = string.Format(
                @"\{0}{3}[\w\-]+(?:{4})*\s*\{2}\{1}",
                this.DocumentOption.Context.OpenChar,
                this.DocumentOption.Context.CloseChar,
                this.DocumentOption.Context.EndChar,
                this.m_noCapturePrefixPattern,
                this.m_noCaptureAttributePattern);

            this.m_prefixPattern = this.DocumentOption.Prefixs.Count() > 0 ?
                "(?'nodePrefix'(?'prefix'" + this.DocumentOption.Prefixs.StringJoin('|', (item) => RegexUtility.Encode(item.Value)) + @"))\" + XtmlNodePrefix.SEPARATOR_CHAR :
                string.Empty;
            this.m_attributePattern = string.Format(
                @"\s+(?'attribute'(?'attributeName'[\w\-]+)\s*\=\s*(?:\""(?'attributeValue'[^\""]*)\""|\'(?'attributeValue'[^\']*)\'|\`(?'attributeValue'[^\`]*)\`))",
                this.DocumentOption.Context.OpenChar,
                this.DocumentOption.Context.CloseChar);
            this.m_declarationPattern = string.Format(
                @"\{0}\?xtml(?:{2})*\s*\?\{1}",
                this.DocumentOption.Context.OpenChar,
                this.DocumentOption.Context.CloseChar,
                this.m_attributePattern);
            this.m_closeExplicitPattern = string.Format(
                "(?>(?:" +
                    "(?:" +
                        "(?'begin'" +
                            "(?'open'" +
                                @"\{0}{3}" +
                                "(?'nodeName'" +
                                    @"(?'name'[\w\-]+)" +
                                ")" +
                                "(?:{4})*" +
                                @"\s*\{1}" +
                            ")" +
                        @")(?:[^\{0}\{1}]|(?:{5}))*" +
                    ")+" +
                    "(?:" +
                        "(?'inner-open'" +
                            @"\{0}\{2}" +
                            (this.DocumentOption.Prefixs.Count() > 0 ?
                                @"(?'-prefix'\k'prefix')" + XtmlNodePrefix.SEPARATOR_CHAR :
                                string.Empty) +
                            @"(?'-name'\k'name')" +
                            @"\{1}" +
                        @")(?:[^\{0}\{1}]|(?:{5}))*" +
                    ")+" +
                ")+)" +
                "(?(open)(?!))",
                this.DocumentOption.Context.OpenChar,
                this.DocumentOption.Context.CloseChar,
                this.DocumentOption.Context.EndChar,
                this.m_prefixPattern,
                this.m_attributePattern,
                this.m_noCaptureCloseSelfPattern);
            this.m_closeSelfPattern = string.Format(
                @"\{0}{3}(?'nodeName'[\w\-]+)(?:{4})*\s*\{2}\{1}",
                this.DocumentOption.Context.OpenChar,
                this.DocumentOption.Context.CloseChar,
                this.DocumentOption.Context.EndChar,
                this.m_prefixPattern,
                this.m_attributePattern);

            this.m_declarationRegex = new Regex(this.m_declarationPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            this.m_closeExplicitRegex = new Regex(this.m_closeExplicitPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            this.m_closeSelfRegex = new Regex(this.m_closeSelfPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        }

        private XtmlRange AnalyzeDeclarationRange(string xtmlText, int startIndex) {
            Capture attributeCapture = null;
            Capture attributeNameCapture = null;
            Capture attributeValueCapture = null;
            CaptureCollection attributeCaptures = null;
            CaptureCollection attributeNameCaptures = null;
            CaptureCollection attributeValueCaptures = null;

            XtmlRange range = null, attributeRange = null;
            Match match = this.m_declarationRegex.Match(xtmlText, startIndex);
            if(match != null && match.Success) {
                attributeCaptures = match.Groups["attribute"].Captures;
                attributeNameCaptures = match.Groups["attributeName"].Captures;
                attributeValueCaptures = match.Groups["attributeValue"].Captures;

                range = XtmlRange.CreateDeclarationRange(
                    xtmlText,
                    new Range(match.Index, match.Length));

                for(int i = 0; i < attributeCaptures.Count; i++) {
                    attributeCapture = attributeCaptures[i];
                    attributeNameCapture = attributeNameCaptures[i];
                    attributeValueCapture = attributeValueCaptures[i];

                    range.AppendChild(attributeRange = XtmlRange.CreateAttributeRange(
                        xtmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length)));
                    attributeRange.AppendChild(XtmlRange.CreateTextRange(
                        xtmlText,
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length)));
                    attributeRange.AppendChild(XtmlRange.CreateTextRange(
                        xtmlText,
                        new Range(attributeValueCapture.Index, attributeValueCapture.Length)));
                }
            }

            return range;
        }

        private IEnumerable<XtmlRange> AnalyzeCloseExplicitRanges(string xtmlText, int startIndex, out IEnumerable<XtmlRange> allRanges) {
            Capture beginCapture = null;
            XtmlNodePrefix prefix = null;
            Capture nodeNameCapture = null;
            Capture innerCapture = null;
            Capture attributeCapture = null;
            Capture attributeNameCapture = null;
            Capture attributeValueCapture = null;
            CaptureCollection beginCaptures = null;
            CaptureCollection nodePrefixCaptures = null;
            CaptureCollection nodeNameCaptures = null;
            IEnumerable<Capture> innerCaptures = null;
            CaptureCollection attributeCaptures = null;
            CaptureCollection attributeNameCaptures = null;
            CaptureCollection attributeValueCaptures = null;

            Range openRange = null;
            XtmlRange range = null, previousRange = null, attributeRange = null;
            IList<XtmlRange> currentRanges = new List<XtmlRange>();
            IList<XtmlRange> attributeRanges = new List<XtmlRange>();
            ICollection<XtmlRange> rootRanges = new List<XtmlRange>();
            ICollection<XtmlRange> allRangesInternal = new List<XtmlRange>();
            foreach(Match match in this.m_closeExplicitRegex.Matches(xtmlText, startIndex)) {
                beginCaptures = match.Groups["begin"].Captures;
                nodePrefixCaptures = match.Groups["nodePrefix"].Success ? match.Groups["nodePrefix"].Captures : null;
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

                    attributeRange = attributeRanges.AddItem(XtmlRange.CreateAttributeRange(
                        xtmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length)));
                    attributeRange.AppendChild(XtmlRange.CreateTextRange(
                        xtmlText,
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length)));
                    attributeRange.AppendChild(XtmlRange.CreateTextRange(
                        xtmlText,
                        new Range(attributeValueCapture.Index, attributeValueCapture.Length)));
                }

                for(int i = 0; i < beginCaptures.Count; i++) {
                    beginCapture = beginCaptures[i];
                    prefix = nodePrefixCaptures != null ? this.m_prefixsMap[nodePrefixCaptures[i].Value] : null;
                    nodeNameCapture = nodeNameCaptures[i];
                    innerCapture = innerCaptures.Where((item) => item.Index == beginCapture.Index + beginCapture.Length).First();

                    range = XtmlRange.CreateElementRange(
                        xtmlText,
                        new Range(beginCapture.Index, beginCapture.Length + innerCapture.Length + nodeNameCapture.Length + (prefix != null ? prefix.PrefixString.Length : 0) + 3),
                        openRange = new Range(beginCapture.Index, beginCapture.Length),
                        new Range(nodeNameCapture.Index, nodeNameCapture.Length),
                        new Range(innerCapture.Index + innerCapture.Length, nodeNameCapture.Length + (prefix != null ? prefix.PrefixString.Length : 0) + 3),
                        prefix);
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

        private IEnumerable<XtmlRange> AnalyzeCloseSelfRanges(string xtmlText, int startIndex) {
            Group nodeGroup = null;
            Group nodeNameGroup = null;
            Capture attributeCapture = null;
            Capture attributeNameCapture = null;
            Capture attributeValueCapture = null;
            CaptureCollection attributeCaptures = null;
            CaptureCollection attributeNameCaptures = null;
            CaptureCollection attributeValueCaptures = null;

            XtmlRange range = null, attributeRange = null;
            ICollection<XtmlRange> ranges = new List<XtmlRange>();
            foreach(Match match in this.m_closeSelfRegex.Matches(xtmlText, startIndex)) {
                nodeGroup = match.Groups[0];
                nodeNameGroup = match.Groups["nodeName"];
                attributeCaptures = match.Groups["attribute"].Captures;
                attributeNameCaptures = match.Groups["attributeName"].Captures;
                attributeValueCaptures = match.Groups["attributeValue"].Captures;

                ranges.Add(range = XtmlRange.CreateElementRange(
                    xtmlText,
                    new Range(nodeGroup.Index, nodeGroup.Length),
                    new Range(nodeGroup.Index, nodeGroup.Length),
                    new Range(nodeNameGroup.Index, nodeNameGroup.Length),
                    Range.Empty,
                    match.Groups["nodePrefix"].Success ? this.m_prefixsMap[match.Groups["nodePrefix"].Value] : null));

                for(int i = 0; i < attributeCaptures.Count; i++) {
                    attributeCapture = attributeCaptures[i];
                    attributeNameCapture = attributeNameCaptures[i];
                    attributeValueCapture = attributeValueCaptures[i];

                    range.AppendChild(attributeRange = XtmlRange.CreateAttributeRange(
                        xtmlText,
                        new Range(attributeCapture.Index, attributeCapture.Length)));
                    attributeRange.AppendChild(XtmlRange.CreateTextRange(
                        xtmlText,
                        new Range(attributeNameCapture.Index, attributeNameCapture.Length)));
                    attributeRange.AppendChild(XtmlRange.CreateTextRange(
                        xtmlText,
                        new Range(attributeValueCapture.Index, attributeValueCapture.Length)));
                }
            }

            return ranges;
        }

        /// <summary>
        /// Analyzes the original XTML text.
        /// </summary>
        /// <param name="xtmlText">The text of a XTML document.</param>
        /// <returns>A IXtmlRange collection.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="xtmlText"/> is null.</exception>
        public virtual IEnumerable<IXtmlRange> Analyze(string xtmlText) {
            return this.Analyze(xtmlText, 0);
        }

        /// <summary>
        /// Analyzes the original XTML text.
        /// </summary>
        /// <param name="xtmlText">The text of a XTML document.</param>
        /// <param name="startIndex">The position start to analyze XTML nodes.</param>
        /// <returns>A IXtmlRange collection.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="xtmlText"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is out of range.</exception>
        public virtual IEnumerable<IXtmlRange> Analyze(string xtmlText, int startIndex) {
            if(xtmlText == null) {
                throw new ArgumentException("The original XTML text is null.", "xtmlText");
            }
            if(startIndex < 0 || startIndex > xtmlText.Length) {
                throw new ArgumentOutOfRangeException("startIndex", "The start index is out of range.");
            }
            if(string.IsNullOrWhiteSpace(xtmlText)) {
                return Enumerable.Empty<IXtmlRange>();
            }

            XtmlRange parentRange = null;
            List<IXtmlRange> rootRanges = new List<IXtmlRange>();

            XtmlRange declarationRange = this.AnalyzeDeclarationRange(xtmlText, startIndex);
            if(declarationRange != null) {
                rootRanges.Add(declarationRange);
                startIndex += Convert.ToInt32(declarationRange.RangeValue.Length);
            }

            IEnumerable<XtmlRange> closeExplicitRanges = null;
            rootRanges.AddRange(this.AnalyzeCloseExplicitRanges(xtmlText, startIndex, out closeExplicitRanges));

            closeExplicitRanges = closeExplicitRanges.Reverse();
            foreach(XtmlRange item in this.AnalyzeCloseSelfRanges(xtmlText, startIndex)) {
                parentRange = closeExplicitRanges.Where((range) => range.RangeValue.Contains(item.RangeValue)).FirstOrDefault();
                if(parentRange != null) {
                    parentRange.AppendChild(item);
                } else {
                    rootRanges.Add(item);
                }
            }

            foreach(XtmlRange item in closeExplicitRanges) {
                item.Normalize();
            }
            return rootRanges.OrderBy((item) => item);
        }
    }

    /// <summary>
    /// Represents a node in a XTML document.
    /// </summary>
    public interface IXtmlNode {
        /// <summary>
        /// Gets the associated XTML document.
        /// </summary>
        IXtmlDocument Document {
            get;
        }

        /// <summary>
        /// Gets the node prefix.
        /// </summary>
        XtmlNodePrefix Prefix {
            get;
            set;
        }

        /// <summary>
        /// Gets parent node.
        /// </summary>
        IXtmlNode ParentNode {
            get;
        }

        /// <summary>
        /// Gets child nodes.
        /// </summary>
        IEnumerable<IXtmlNode> ChildNodes {
            get;
        }

        /// <summary>
        /// Gets the name of the node with the prefix removed.
        /// </summary>
        string LocalName {
            get;
        }

        /// <summary>
        /// Gets the name of the node includes the prefix.
        /// </summary>
        string QualifiedName {
            get;
        }

        /// <summary>
        /// Gets XtmlNodeType of this node.
        /// </summary>
        XtmlNodeType NodeType {
            get;
        }

        /// <summary>
        /// Gets the XTML text of this node.
        /// </summary>
        string NodeXtml {
            get;
        }

        /// <summary>
        /// Gets or sets a object attached this node.
        /// </summary>
        object AttachedObject {
            get;
            set;
        }

        /// <summary>
        /// Renders this node to a TextWriter with the specified data source and rendering context.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        void Render(TextWriter writer, object dataSource, IXtmlRenderingContext context);
    }

    /// <summary>
    /// Represents a XTML declaration node.
    /// </summary>
    public interface IXtmlDeclaration : IXtmlNode {
        /// <summary>
        /// Gets the document version.
        /// </summary>
        string Version {
            get;
        }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        string Type {
            get;
        }

        /// <summary>
        /// Gets using encoding when rendering this document.
        /// </summary>
        string Encoding {
            get;
        }
    }

    /// <summary>
    /// Represents a XTML document.
    /// </summary>
    public interface IXtmlDocument : IXtmlNode {
        /// <summary>
        /// Gets document declaration.
        /// </summary>
        IXtmlDeclaration Declaration {
            get;
        }

        /// <summary>
        /// Gets document option.
        /// </summary>
        XtmlDocumentOption DocumentOption {
            get;
        }
    }

    /// <summary>
    /// Provides a interface to render a IXmlNode object.
    /// </summary>
    public interface IXtmlNodeRenderer {
        /// <summary>
        /// Renders the specified XTML node to the TextWriter with the specified data source.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="writer"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context);
    }

    /// <summary>
    /// Represents a XTML attribute node.
    /// </summary>
    public interface IXtmlAttribute : IXtmlNode {
        /// <summary>
        /// Gets attribute value.
        /// </summary>
        /// <param name="dataSource">Current data source.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetValue(object dataSource, IXtmlRenderingContext context);

        /// <summary>
        /// Sets attribute value.
        /// </summary>
        /// <param name="value"></param>
        void SetValue(string value);
    }

    /// <summary>
    /// Represents a XTML element node.
    /// </summary>
    public interface IXtmlElement : IXtmlNode {
        /// <summary>
        /// Gets attributes of this element.
        /// </summary>
        IEnumerable<IXtmlAttribute> Attributes {
            get;
        }
    }

    /// <summary>
    /// Provides a default implementation of IXtmlNode interface.
    /// </summary>
    internal abstract class XtmlNode : IXtmlNode {
        /// <summary>
        /// Initialize a new instance of XtmlNode.
        /// </summary>
        public XtmlNode() {
            this.m_childNodes = new ChildCollection<XtmlNode, XtmlNode>(
                this,
                (parent, child) => {
                    child.ParentNode = parent;
                    child.Document = parent.Document;
                },
                (parent, child) => {
                    child.ParentNode = null;
                    child.Document = null;
                });
        }

        /// <summary>
        /// Gets or sets a IXtmlRange object associated with this node.
        /// </summary>
        public virtual IXtmlRange Range {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a IXtmlNodeRenderer object used to render this node.
        /// </summary>
        public virtual IXtmlNodeRenderer NodeRenderer {
            get;
            set;
        }

        /// <summary>
        /// Appends the specified node to child nodes list.
        /// </summary>
        /// <param name="item"></param>
        public virtual void AppendChild(XtmlNode item) {
            this.m_childNodes.Add(item);
        }

        /// <summary>
        /// Repairs the document chain of all child nodes.
        /// </summary>
        public virtual void RepairDocumentChain() {
            foreach(XtmlNode item in this.m_childNodes) {
                item.Document = this.Document;
                item.RepairDocumentChain();
            }
        }

        #region IXtmlNode Members

        /// <inheritdoc />
        public virtual IXtmlDocument Document {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual XtmlNodePrefix Prefix {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual IXtmlNode ParentNode {
            get;
            set;
        }

        protected ChildCollection<XtmlNode, XtmlNode> m_childNodes;
        /// <inheritdoc />
        public virtual IEnumerable<IXtmlNode> ChildNodes {
            get {
                return this.m_childNodes;
            }
        }

        /// <inheritdoc />
        public virtual string LocalName {
            get;
            set;
        }

        protected string m_qualifiedName;
        /// <inheritdoc />
        public virtual string QualifiedName {
            get {
                return this.m_qualifiedName ?? (this.m_qualifiedName = this.Prefix != null ? this.Prefix.GetQualifiedName(this.LocalName) : this.LocalName);
            }
        }

        /// <inheritdoc />
        public abstract XtmlNodeType NodeType {
            get;
        }

        /// <inheritdoc />
        public virtual string NodeXtml {
            get {
                return this.Range != null ? this.Range.RangeText : null;
            }
        }

        /// <inheritdoc />
        public object AttachedObject {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual void Render(TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(this.NodeRenderer == null) {
                this.NodeRenderer = TagSystem.Instance.GetTag(this.QualifiedName);
            }

            if(this.NodeRenderer != null) {
                this.NodeRenderer.Render(this, writer, dataSource, context);
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
            return this.QualifiedName;
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IXtmlDeclaration interface.
    /// </summary>
    internal class XtmlDeclaration : XtmlNode, IXtmlDeclaration {
        /// <summary>
        /// Initialize a new instance of XtmlDeclaration class.
        /// </summary>
        public XtmlDeclaration() {
            this.Version = VERSION_DEFAULT_VALUE;
            this.Type = TYPE_DEFAULT_VALUE;
            this.Encoding = ENCODING_DEFAULT_VALUE;
        }

        public const string DEFAULT_NAME = "#declaration";
        public const string VERSION_ATTRIBUTE_NAME = "version";
        public const string TYPE_ATTRIBUTE_NAME = "type";
        public const string ENCODING_ATTRIBUTE_NAME = "encoding";
        public const string VERSION_DEFAULT_VALUE = "1.0";
        public const string TYPE_DEFAULT_VALUE = "text/html";
        public const string ENCODING_DEFAULT_VALUE = "UTF-8";

        /// <summary>
        /// Gets the default document declaration.
        /// </summary>
        public static IXtmlDeclaration CreateDefaultDeclaration(IXtmlDocument document) {
            return new XtmlDeclaration {
                Document = document,
                ParentNode = document,
                Version = VERSION_DEFAULT_VALUE,
                Type = TYPE_DEFAULT_VALUE,
                Encoding = ENCODING_DEFAULT_VALUE,
            };
        }

        /// <inheritdoc />
        public override string LocalName {
            get {
                return DEFAULT_NAME;
            }
            set {
            }
        }

        /// <inheritdoc />
        public override XtmlNodeType NodeType {
            get {
                return XtmlNodeType.Declaration;
            }
        }

        /// <inheritdoc />
        public override void Render(TextWriter writer, object dataSource, IXtmlRenderingContext context) {
        }

        #region IXtmlDeclaration Members

        protected string m_version;
        /// <inheritdoc />
        public virtual string Version {
            get {
                return this.m_version;
            }
            set {
                this.m_version = string.IsNullOrWhiteSpace(value) ? VERSION_DEFAULT_VALUE : value;
            }
        }

        protected string m_type;
        /// <inheritdoc />
        public virtual string Type {
            get {
                return this.m_type;
            }
            set {
                this.m_type = string.IsNullOrWhiteSpace(value) ? TYPE_DEFAULT_VALUE : value;
            }
        }

        protected string m_encoding;
        /// <inheritdoc />
        public virtual string Encoding {
            get {
                return this.m_encoding;
            }
            set {
                this.m_encoding = string.IsNullOrWhiteSpace(value) ? ENCODING_DEFAULT_VALUE : value;
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0}={3};{1}={4};{2}={5};", VERSION_ATTRIBUTE_NAME, TYPE_ATTRIBUTE_NAME, ENCODING_ATTRIBUTE_NAME, this.Version, this.Type, this.Encoding);
        }

        #endregion
    }

    /// <summary>
    /// Represents a XTML text node.
    /// </summary>
    internal class XtmlText : XtmlNode, IXtmlText {
        /// <summary>
        /// Initialize a new instance of XtmlText class.
        /// </summary>
        public XtmlText() {
        }

        public const string DEFAULT_NAME = "#text";

        /// <inheritdoc />
        public override string LocalName {
            get {
                return DEFAULT_NAME;
            }
            set {
            }
        }

        /// <inheritdoc />
        public override XtmlNodeType NodeType {
            get {
                return XtmlNodeType.Text;
            }
        }

        /// <inheritdoc />
        public override IXtmlNodeRenderer NodeRenderer {
            get {
                return base.NodeRenderer;
            }
            set {
                base.NodeRenderer = value ?? DefaultXtmlTextRenderer.Instance;
            }
        }

        #region IXtmlText Members

        protected string m_text;
        /// <inheritdoc />
        public virtual string Text {
            get {
                return this.m_text ?? (this.m_text = this.Document.DocumentOption.Context.Decode(this.NodeXtml));
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override string ToString() {
            return this.Text;
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IXtmlAttribute interface.
    /// </summary>
    internal class XtmlAttribute : XtmlNode, IXtmlAttribute {
        private object m_lock = new object();

        /// <inheritdoc />
        public override string QualifiedName {
            get {
                return this.LocalName;
            }
        }

        /// <inheritdoc />
        public override XtmlNodeType NodeType {
            get {
                return XtmlNodeType.Attribute;
            }
        }

        /// <summary>
        /// Gets or sets the XTML text of value of this attribute.
        /// </summary>
        public string ValueXtml {
            get;
            set;
        }

        /// <inheritdoc />
        public override void Render(TextWriter writer, object dataSource, IXtmlRenderingContext context) {
        }

        #region IXtmlAttribute Members

        private string m_textValue;
        private IXtmlDocument m_documentValue;
        /// <inheritdoc />
        public string GetValue(object dataSource, IXtmlRenderingContext context) {
            if(this.m_textValue != null) {
                return this.m_textValue;
            } else if(this.m_documentValue != null) {
                StringBuilder value = new StringBuilder(this.ValueXtml.Length);
                using(TextWriter writer = new StringWriter(value)) {
                    this.m_documentValue.Render(writer, dataSource, context);
                }
                return value.ToString();
            } else {
                lock(this.m_lock) {
                    if(this.m_textValue == null && this.m_documentValue == null) {
                        XtmlDocument document = XtmlDocument.Load(this.ValueXtml, this.Document.DocumentOption, null);
                        if(document.ChildNodes.Count() == 1 && document.ChildNodes.First().NodeType == XtmlNodeType.Text) {
                            this.m_textValue = ((IXtmlText) document.ChildNodes.First()).Text;
                        } else {
                            document.ParentNode = this;
                            this.m_documentValue = document;
                        }
                    }
                }
                return this.GetValue(dataSource, context);
            }
        }

        /// <inheritdoc />
        public void SetValue(string value) {
            this.ValueXtml = value ?? string.Empty;
            this.m_textValue = null;
            this.m_documentValue = null;
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0}='{1}'", this.LocalName, this.ValueXtml);
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IXtmlElement interface.
    /// </summary>
    internal class XtmlElement : XtmlNode, IXtmlElement {
        /// <summary>
        /// Initialize a new instance of XtmlElement class.
        /// </summary>
        public XtmlElement() {
            this.m_attributes = new ChildCollection<XtmlElement, XtmlAttribute>(
                this,
                (parent, child) => child.ParentNode = parent,
                (parent, child) => child.ParentNode = null);
        }

        /// <inheritdoc />
        public override XtmlNodeType NodeType {
            get {
                return XtmlNodeType.Element;
            }
        }

        /// <inheritdoc />
        public override IXtmlNodeRenderer NodeRenderer {
            get {
                return base.NodeRenderer;
            }
            set {
                base.NodeRenderer = value ?? DefaultXtmlComplexNodeRenderer.Instance;
            }
        }

        /// <summary>
        /// Appends the specified attribute to attributes list.
        /// </summary>
        /// <param name="item"></param>
        public virtual void AppendAttribute(XtmlAttribute item) {
            this.m_attributes.Add(item);
        }

        /// <inheritdoc />
        public override void RepairDocumentChain() {
            foreach(XtmlAttribute item in this.m_attributes) {
                item.Document = this.Document;
            }
            base.RepairDocumentChain();
        }

        #region IXtmlElement Members

        protected ChildCollection<XtmlElement, XtmlAttribute> m_attributes;
        /// <inheritdoc />
        public virtual IEnumerable<IXtmlAttribute> Attributes {
            get {
                return this.m_attributes;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of IXtmlDocument interface.
    /// </summary>
    internal class XtmlDocument : XtmlNode, IXtmlDocument {
        /// <summary>
        /// Initialize a new instance of XtmlDocument class.
        /// </summary>
        public XtmlDocument() {
            this.LocalName = DEFAULT_NAME;
            this.Document = this;
            this.Declaration = XtmlDeclaration.CreateDefaultDeclaration(this);
        }

        public const string DEFAULT_NAME = "#document";

        /// <summary>
        /// A XtmlRangeAnalyzer cache, key is a XtmlDocumentOption object.
        /// </summary>
        private static IDictionary<XtmlDocumentOption, XtmlRangeAnalyzer> g_analyzerCache = new Dictionary<XtmlDocumentOption, XtmlRangeAnalyzer>();

        private string m_localName;
        /// <inheritdoc />
        public override string LocalName {
            get {
                return m_localName;
            }
            set {
                m_localName = string.IsNullOrWhiteSpace(value) ? DEFAULT_NAME : value;
            }
        }

        /// <inheritdoc />
        public override XtmlNodeType NodeType {
            get {
                return XtmlNodeType.Document;
            }
        }

        /// <inheritdoc />
        public override IXtmlNodeRenderer NodeRenderer {
            get {
                return base.NodeRenderer;
            }
            set {
                base.NodeRenderer = value ?? DefaultXtmlComplexNodeRenderer.Instance;
            }
        }

        #region IXtmlDocument Members

        protected IXtmlDeclaration m_declaration;
        /// <inheritdoc />
        public virtual IXtmlDeclaration Declaration {
            get {
                return this.m_declaration;
            }
            set {
                this.m_declaration = value ?? XtmlDeclaration.CreateDefaultDeclaration(this);
            }
        }

        /// <inheritdoc />
        public virtual XtmlDocumentOption DocumentOption {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Loads a XTML document from the specified XTML string.
        /// </summary>
        /// <param name="xtml"></param>
        /// <param name="option"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XtmlDocument Load(string xtml, XtmlDocumentOption option, string name) {
            if(xtml == null) {
                throw new ArgumentException("The XTML text is null.", "xtml");
            }
            if(option == null) {
                throw new ArgumentException("The XTML document option is null.", "option");
            }

            lock(((ICollection) g_analyzerCache).SyncRoot) {
                if(!g_analyzerCache.ContainsKey(option)) {
                    g_analyzerCache[option] = new XtmlRangeAnalyzer(option);
                }
            }

            XtmlRangeAnalyzer analyzer = g_analyzerCache[option];
            XtmlRange range = XtmlRange.CreateDocumentRange(xtml);
            foreach(XtmlRange item in analyzer.Analyze(xtml)) {
                range.AppendChild(item);
            }
            XtmlDocument document = (XtmlDocument) range.ToNode();
            document.LocalName = name;
            document.DocumentOption = option;
            document.Declaration = (IXtmlDeclaration) document.ChildNodes.Where((item) => item is IXtmlDeclaration).FirstOrDefault();
            return document;
        }
    }

    /// <summary>
    /// Transfers a IXtmlRange to a IXtmlNode.
    /// </summary>
    internal interface IXtmlRangeToNode {
        IXtmlNode ToNode(IXtmlRange range);
    }

    /// <summary>
    /// Transfers a IXtmlRange to a complex XTML node, for example: a element or a document.
    /// </summary>
    internal abstract class XtmlRangeToComplex : IXtmlRangeToNode {
        /// <summary>
        /// Creates a XtmlNode object.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public abstract XtmlNode CreateNode(IXtmlRange range);

        /// <summary>
        /// Gets the range of all child nodes.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public abstract Range GetChildNodesRange(IXtmlRange range);

        /// <summary>
        /// Creates child nodes.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public virtual IEnumerable<XtmlNode> CreateChildNodes(IXtmlRange range, Range childrenRange) {
            if(childrenRange.Length == 0) {
                return Enumerable.Empty<XtmlNode>();
            }

            Range rangeValue = null;
            int previousIndex = Convert.ToInt32(childrenRange.StartIndex);
            ICollection<XtmlNode> childNodes = new List<XtmlNode>();

            foreach(IXtmlRange child in range.ChildRanges) {
                if(!childrenRange.Contains(rangeValue = child.RangeValue)) {
                    continue;
                }

                if(rangeValue.StartIndex > previousIndex) {
                    childNodes.Add(new XtmlText {
                        Range = XtmlRange.CreateTextRange(range.XtmlText, new Range(previousIndex, rangeValue.StartIndex - previousIndex)),
                    });
                }
                childNodes.Add((XtmlNode) child.ToNode());
                previousIndex = Convert.ToInt32(rangeValue.StartIndex + rangeValue.Length);
            }
            if(previousIndex <= childrenRange.EndIndex) {
                childNodes.Add(new XtmlText {
                    Range = XtmlRange.CreateTextRange(range.XtmlText, new Range(previousIndex, childrenRange.EndIndex - previousIndex + 1)),
                });
            }

            return childNodes;
        }

        #region IXtmlRangeToNode Members

        /// <inheritdoc />
        public virtual IXtmlNode ToNode(IXtmlRange range) {
            XtmlNode node = this.CreateNode(range);
            if(node != null) {
                foreach(XtmlNode item in this.CreateChildNodes(range, this.GetChildNodesRange(range))) {
                    node.AppendChild(item);
                }
            }
            return node;
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IXtmlRange to a XTML text node.
    /// </summary>
    internal class XtmlRangeToText : IXtmlRangeToNode {
        #region Singleton

        private XtmlRangeToText() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new XtmlRangeToText();
            }

            public static XtmlRangeToText Instance;
        }

        /// <summary>
        /// Gets the unique instance of XtmlRangeToText class.
        /// </summary>
        public static IXtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IXtmlRangeToNode Members

        /// <inheritdoc />
        public virtual IXtmlNode ToNode(IXtmlRange range) {
            return new XtmlText {
                Range = range,
            };
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IXtmlRange to a XTML declaration node.
    /// </summary>
    internal class XtmlRangeToDeclaration : IXtmlRangeToNode {
        #region Singleton

        private XtmlRangeToDeclaration() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new XtmlRangeToDeclaration();
            }

            public static XtmlRangeToDeclaration Instance;
        }

        /// <summary>
        /// Gets the unique instance of XtmlRangeToDeclaration class.
        /// </summary>
        public static IXtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IXtmlRangeToNode Members

        /// <inheritdoc />
        public IXtmlNode ToNode(IXtmlRange range) {
            XtmlDeclaration node = new XtmlDeclaration {
                Range = range,
            };

            foreach(XtmlAttribute attribute in range.ChildRanges.Select((item) => item.ToNode())) {
                if(string.Equals(attribute.LocalName, XtmlDeclaration.VERSION_ATTRIBUTE_NAME, StringComparison.OrdinalIgnoreCase)) {
                    node.Version = attribute.ValueXtml;
                }
                if(string.Equals(attribute.LocalName, XtmlDeclaration.TYPE_ATTRIBUTE_NAME, StringComparison.OrdinalIgnoreCase)) {
                    node.Type = attribute.ValueXtml;
                }
                if(string.Equals(attribute.LocalName, XtmlDeclaration.ENCODING_ATTRIBUTE_NAME, StringComparison.OrdinalIgnoreCase)) {
                    node.Encoding = attribute.ValueXtml;
                }
            }

            return node;
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IXtmlRange to a XTML attribute node.
    /// </summary>
    internal class XtmlRangeToAttribute : IXtmlRangeToNode {
        #region Singleton

        private XtmlRangeToAttribute() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new XtmlRangeToAttribute();
            }

            public static XtmlRangeToAttribute Instance;
        }

        /// <summary>
        /// Gets the unique instance of XtmlRangeToAttribute class.
        /// </summary>
        public static IXtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IXtmlRangeToNode Members

        /// <inheritdoc />
        public IXtmlNode ToNode(IXtmlRange range) {
            return new XtmlAttribute {
                Range = range,
                LocalName = range.ChildRanges.First().RangeText,
                ValueXtml = range.ChildRanges.ElementAt(1).RangeText,
            };
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IXtmlRange to a XTML element node.
    /// </summary>
    internal class XtmlRangeToElement : XtmlRangeToComplex {
        /// <summary>
        /// Initialize a new instance of XtmlRangeToElement class.
        /// </summary>
        /// <param name="openRange"></param>
        /// <param name="nameRange"></param>
        /// <param name="closeRange"></param>
        /// <param name="prefix"></param>
        public XtmlRangeToElement(Range openRange, Range nameRange, Range closeRange, XtmlNodePrefix prefix) {
            this.OpenRange = openRange;
            this.NameRange = nameRange;
            this.CloseRange = closeRange;
            this.Prefix = prefix;
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
        /// Gets the range of close tag in this element.
        /// </summary>
        public Range CloseRange {
            get;
            private set;
        }

        /// <summary>
        /// Gets the node prefix.
        /// </summary>
        public XtmlNodePrefix Prefix {
            get;
            private set;
        }

        #region XtmlRangeToComplex Members

        /// <inheritdoc />
        public override XtmlNode CreateNode(IXtmlRange range) {
            XtmlElement node = new XtmlElement {
                Prefix = this.Prefix,
                Range = range,
                LocalName = range.XtmlText.Substring(Convert.ToInt32(this.NameRange.StartIndex), Convert.ToInt32(this.NameRange.Length)),
            };

            /*
             * Notice: The child ranges in open rang may be contains close-self rangs in attribute value ranges.
             */
            foreach(XtmlAttribute item in range.ChildRanges.Where((item) => this.OpenRange.Contains(item.RangeValue)).Select((item) => item.ToNode()).Where((item) => item is XtmlAttribute)) {
                node.AppendAttribute(item);
            }
            return node;
        }

        /// <inheritdoc />
        public override Range GetChildNodesRange(IXtmlRange range) {
            return new Range(this.OpenRange.StartIndex + this.OpenRange.Length, range.RangeValue.Length - this.OpenRange.Length - this.CloseRange.Length);
        }

        #endregion
    }

    /// <summary>
    /// Transfers a IXtmlRange to a XTML document.
    /// </summary>
    internal class XtmlRangeToDocument : XtmlRangeToComplex {
        #region Singleton

        private XtmlRangeToDocument() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new XtmlRangeToDocument();
            }

            public static XtmlRangeToDocument Instance;
        }

        /// <summary>
        /// Gets the unique instance of XtmlRangeToDocument class.
        /// </summary>
        public static IXtmlRangeToNode Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region XtmlRangeToComplex Members

        /// <inheritdoc />
        public override XtmlNode CreateNode(IXtmlRange range) {
            XtmlDocument node = new XtmlDocument {
                Range = range,
            };
            return node;
        }

        /// <inheritdoc />
        public override Range GetChildNodesRange(IXtmlRange range) {
            return range.RangeValue;
        }

        #endregion

        /// <inheritdoc />
        public override IXtmlNode ToNode(IXtmlRange range) {
            XtmlNode document = (XtmlNode) base.ToNode(range);
            document.RepairDocumentChain();
            return document;
        }
    }

    /// <summary>
    /// Represents a XTML text node.
    /// </summary>
    public interface IXtmlText : IXtmlNode {
        /// <summary>
        /// Gets text of this node.
        /// </summary>
        string Text {
            get;
        }
    }

    /// <summary>
    /// Represents a tag in a XTML template.
    /// The implementation must ensure the tag is reusable.
    /// </summary>
    public interface ITemplateTag : IXtmlNodeRenderer {
        /// <summary>
        /// Gets the descriptive name of this tag.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets the qualified name of this tag.
        /// </summary>
        string TagName {
            get;
        }

        /// <summary>
        /// Gets category of this tag.
        /// </summary>
        string Category {
            get;
        }

        /// <summary>
        /// Gets description of this tag.
        /// </summary>
        string Description {
            get;
        }

        /// <summary>
        /// Gets whether display this tag to user.
        /// </summary>
        bool IsVisible {
            get;
        }

        /// <summary>
        /// Gets shorthand of this tag.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetShorthand(XtmlMarkupContext context);

        /// <summary>
        /// Gets the default pattern of this node when used it in a template.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetPattern(XtmlMarkupContext context);
    }

    /// <summary>
    /// Represents the type of a XTML node.
    /// </summary>
    public enum XtmlNodeType {
        /// <summary>
        /// Text node.
        /// </summary>
        Text = 0x00,

        /// <summary>
        /// Declaration node.
        /// </summary>
        Declaration = 0x01,

        /// <summary>
        /// Attribute node.
        /// </summary>
        Attribute = 0x02,

        /// <summary>
        /// Element node.
        /// </summary>
        Element = 0x03,

        /// <summary>
        /// XTML document.
        /// </summary>
        Document = 0x04,
    }

    /// <summary>
    /// Represents the global context when render a XTML document.
    /// </summary>
    public interface IXtmlRenderingContext {
    }

    /// <summary>
    /// Provides a default implementation of IXtmlNodeRenderer interface for text node.
    /// This class renders IXtmlText.Text property to the TextWriter object.
    /// </summary>
    internal class DefaultXtmlTextRenderer : IXtmlNodeRenderer {
        #region Singleton

        private DefaultXtmlTextRenderer() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new DefaultXtmlTextRenderer();
            }

            public static DefaultXtmlTextRenderer Instance;
        }

        /// <summary>
        /// Gets the unique instance of DefaultXtmlTextRenderer class.
        /// </summary>
        public static IXtmlNodeRenderer Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Text) {
                throw new XtmlException("The rendered node is not a text node.");
            }

            writer.Write(((IXtmlText) node).Text);
        }

        #endregion
    }

    /// <summary>
    /// Represents errors that occur during analyzing or rendering a XTML document.
    /// </summary>
    public class XtmlException : Exception {
        public XtmlException()
            : base() {
        }

        public XtmlException(string message)
            : base(message) {
        }

        public XtmlException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public XtmlException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }

    /// <summary>
    /// Provides a default implementation of IXtmlNodeRenderer interface for rendering a XTML element node or a XTML document.
    /// This class renders all child nodes to the TextWriter object.
    /// </summary>
    internal class DefaultXtmlComplexNodeRenderer : IXtmlNodeRenderer {
        #region Singleton

        private DefaultXtmlComplexNodeRenderer() {
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new DefaultXtmlComplexNodeRenderer();
            }

            public static DefaultXtmlComplexNodeRenderer Instance;
        }

        /// <summary>
        /// Gets the unique instance of DefaultXtmlElementRenderer class.
        /// </summary>
        public static IXtmlNodeRenderer Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Element &&
                node.NodeType != XtmlNodeType.Document) {
                throw new XtmlException("The rendered node is not a element node or a document.");
            }

            foreach(IXtmlNode child in node.ChildNodes) {
                child.Render(writer, dataSource, context);
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a implementation of ITemplateTag interface which contains a IXtmlNodeRenderer object.
    /// </summary>
    public class XtmlNodeRendererWrapper : ITemplateTag {
        /// <summary>
        /// Initialize a new instance of XtmlNodeRendererWrapper class.
        /// </summary>
        /// <param name="renderer"></param>
        /// <exception cref="System.ArgumentException"><paramref name="renderer"/> is null.</exception>
        public XtmlNodeRendererWrapper(IXtmlNodeRenderer renderer) {
            if(renderer == null) {
                throw new ArgumentException("The IXtmlNodeRenderer object is null.", "renderer");
            }

            this.IsVisible = true;
            this.NodeRenderer = renderer;
        }

        /// <summary>
        /// Gets the IXtmlNodeRenderer object used by this tag.
        /// </summary>
        public IXtmlNodeRenderer NodeRenderer {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the delegate to get the shorthand of this tag.
        /// </summary>
        public Func<XtmlMarkupContext, string, string> ShorthandSelector {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delegate to get the default pattern of this tag.
        /// </summary>
        public Func<XtmlMarkupContext, string, string> PatternSelector {
            get;
            set;
        }

        #region ITemplateTag Members

        /// <inheritdoc />
        public virtual string Name {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual string TagName {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual string Category {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual string Description {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual bool IsVisible {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual string GetShorthand(XtmlMarkupContext context) {
            return this.ShorthandSelector != null ? this.ShorthandSelector(context, this.TagName) : null;
        }

        /// <inheritdoc />
        public virtual string GetPattern(XtmlMarkupContext context) {
            return this.PatternSelector != null ? this.PatternSelector(context, this.TagName) : null;
        }

        #endregion

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(this.NodeRenderer != null) {
                this.NodeRenderer.Render(node, writer, dataSource, context);
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.TagName.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is ITemplateTag)) {
                return false;
            }

            return string.Equals(this.TagName, ((ITemplateTag) obj).TagName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.TagName;
        }

        #endregion
    }

    /// <summary>
    /// Provides a implementation of IXtmlNodeRenderer interface which renders a property of the data source.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class DataSourcePropertyXtmlElementRenderer<TSource> : IXtmlNodeRenderer where TSource : class {
        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName)
            : this(propertyName, (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector)
            : this(propertyName, dataSourceSelector, (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object> formater)
            : this(propertyName, null, formater) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="formater"></param>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, IDataSourceProvider<TSource> dataSourceProvider, IDataSourcePropertyValueFormater<TSource, object> formater)
            : this(propertyName, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, formater != null ? formater.Format : (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="formater"></param>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, IDataSourceProvider<TSource> dataSourceProvider, string format)
            : this(propertyName, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, format != null ? (element, dataSource, source, context, name, value, isMultiple) => string.Format(string.Format("{{0:{0}}}", format), value) : (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyNameSelector"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector, Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object> formater)
            : this((element, source, context) => propertyName, dataSourceSelector, formater) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="formater"></param>
        public DataSourcePropertyXtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, string> propertyNameSelector, IDataSourceProvider<TSource> dataSourceProvider, IDataSourcePropertyValueFormater<TSource, object> formater)
            : this(propertyNameSelector, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, formater != null ? formater.Format : (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyNameSelector"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, string> propertyNameSelector, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector, Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object> formater) {
            this.m_propertyNameSelector = propertyNameSelector ?? ((element, dataSource, context) => {
                IXtmlAttribute attribute = element.GetAttributesByName(PROPERTY_ATTRIBUTE_NAME).FirstOrDefault();
                return attribute != null ? attribute.GetValue(dataSource, context) : null;
            });
            this.m_dataSourceSelector = dataSourceSelector;
            this.m_dataSourceFinder = DataSourceFinder.Instance;
            this.m_formater = formater;
        }

        /// <summary>
        /// The name of property name attribute.
        /// </summary>
        public const string PROPERTY_ATTRIBUTE_NAME = "Property";

        /// <summary>
        /// The property dictionary.
        /// </summary>
        protected IDictionary<string, Element<PropertyInfo, bool>> m_propertiesCache = new Dictionary<string, Element<PropertyInfo, bool>>();
        protected volatile int m_isAddingPropertyCache;

        /// <summary>
        /// The delegate to get property name.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, string> m_propertyNameSelector;

        /// <summary>
        /// The delegate to get data source.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, TSource> m_dataSourceSelector;
        private DataSourceFinder m_dataSourceFinder;

        /// <summary>
        /// The formater for property value.
        /// </summary>
        protected Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object> m_formater;

        /// <summary>
        /// Gets the property name.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual string GetPropertyName(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return this.m_propertyNameSelector(element, dataSource, context);
        }

        /// <summary>
        /// Gets the actual data source.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual TSource GetSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            TSource source = null;

            if(this.m_dataSourceSelector != null) {
                source = this.m_dataSourceSelector(element, dataSource, context);
            }
            if(source == null) {
                if(dataSource is TSource) {
                    source = (TSource) dataSource;
                } else if(dataSource != null) {
                    source = this.m_dataSourceFinder.Find<TSource>(dataSource);
                }
            }

            return source;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="isMultiple"></param>
        /// <returns></returns>
        protected virtual object GetPropertyValue(IXtmlElement element, object dataSource, TSource source, IXtmlRenderingContext context, string propertyName, out bool isMultiple) {
            isMultiple = false;
            Type type = typeof(TSource);
            PropertyInfo property = null;

            if(!this.m_propertiesCache.ContainsKey(propertyName)) {
                lock(((ICollection) this.m_propertiesCache).SyncRoot) {
                    if(!this.m_propertiesCache.ContainsKey(propertyName)) {
                        property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if(property == null || property.GetIndexParameters().Length > 0) {
                            throw new XtmlException(string.Format("{0} do not contains a property named {1}.", type.FullName, propertyName));
                        }

                        isMultiple = Attribute.GetCustomAttributes(property, typeof(MultipleAttribute), true).Length > 0;

                        this.m_isAddingPropertyCache = 1;
                        this.m_propertiesCache[propertyName] = new Element<PropertyInfo, bool>(property, isMultiple);
                        this.m_isAddingPropertyCache = 0;
                    }
                }
            }

            if(property == null) {
                SpinWait.SpinUntil(() => this.m_isAddingPropertyCache == 0);

                Element<PropertyInfo, bool> cacheValue = this.m_propertiesCache[propertyName];
                property = cacheValue.Component1;
                isMultiple = cacheValue.Component2;
            }

            return property.GetValue(source, null);
        }

        /// <summary>
        /// Formats the property value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="isMultiple"></param>
        /// <returns></returns>
        protected virtual object FormatPropertyValue(IXtmlElement element, object dataSource, TSource source, IXtmlRenderingContext context, string propertyName, object propertyValue, bool isMultiple) {
            object formatedValue = propertyValue;
            if(this.m_formater != null) {
                formatedValue = this.m_formater(element, dataSource, source, context, propertyName, propertyValue, isMultiple);
            }
            return formatedValue;
        }

        /// <summary>
        /// Renders the property value to a TextWriter.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="source"></param>
        /// <param name="propertyValue"></param>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        protected virtual void RenderPropertyValue(IXtmlElement element, object dataSource, TSource source, IXtmlRenderingContext context, string propertyName, object propertyValue, bool isMultiple, TextWriter writer) {
            if(propertyValue == null) {
                return;
            }

            if(isMultiple && propertyValue is IEnumerable<object>) {
                new XtmlElementIterationRenderer<object>().Render(element, writer, propertyValue, context);
            } else {
                writer.Write(propertyValue);
            }
        }

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public virtual void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Element) {
                throw new XtmlException("The rendered node is not a XTML element node.");
            }

            IXtmlElement element = (IXtmlElement) node;
            string propertyName = this.GetPropertyName(element, dataSource, context);
            if(string.IsNullOrWhiteSpace(propertyName)) {
                throw new XtmlException("Property name is empty.");
            }

            TSource source = this.GetSource(element, dataSource, context);
            if(source == null) {
                throw new XtmlException(string.Format("Can not find data source of {0} type.", typeof(TSource).Name));
            }

            bool isMultiple = false;
            object propertyValue = this.GetPropertyValue(element, dataSource, source, context, propertyName, out isMultiple);
            propertyValue = this.FormatPropertyValue(element, dataSource, source, context, propertyName, propertyValue, isMultiple);
            this.RenderPropertyValue(element, dataSource, source, context, propertyName, propertyValue, isMultiple, writer);
        }

        #endregion
    }

    /// <summary>
    /// Provides a implementation of IXtmlNodeRenderer interface which renders a property of the data source.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DataSourcePropertyXtmlElementRenderer<TSource, TValue> : DataSourcePropertyXtmlElementRenderer<TSource> where TSource : class {
        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName)
            : this(propertyName, (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, TValue, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector)
            : this(propertyName, dataSourceSelector, (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, TValue, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, TValue, bool, object> formater)
            : this(propertyName, (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, formater) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="formater"></param>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, IDataSourceProvider<TSource> dataSourceProvider, IDataSourcePropertyValueFormater<TSource, TValue> formater)
            : this(propertyName, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, formater != null ? formater.Format : (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, TValue, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="formater"></param>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, IDataSourceProvider<TSource> dataSourceProvider, string format)
            : this(propertyName, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, format != null ? (element, dataSource, source, context, name, value, isMultiple) => string.Format(string.Format("{{0:{0}}}", format), value) : (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, TValue, bool, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of DataSourcePropertyXtmlElementRenderer class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
        public DataSourcePropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector, Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, TValue, bool, object> formater)
            : base(propertyName, dataSourceSelector, formater != null ? (element, dataSource, source, context, name, value, isMultiple) => formater(element, dataSource, source, context, name, (TValue) value, isMultiple) : (Func<IXtmlElement, object, TSource, IXtmlRenderingContext, string, object, bool, object>) null) {
        }
    }

    /// <summary>
    /// Provides a implementation of IXtmlNodeRenderer interface which renders the uri of a MVC action.
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    public class MvcActionUriXtmlElementRenderer<TController> : IXtmlNodeRenderer
        where TController : ControllerBase {
        /// <summary>
        /// Initialize a instance of MvcActionUriXtmlElementRenderer class.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dataSourceSelector"></param>
        /// <param name="arguments"></param>
        public MvcActionUriXtmlElementRenderer(Expression<Func<TController, object>> expression, IDictionary<string, string> arguments) {
            if(expression == null) {
                throw new ArgumentException("The action expression is null.", "expression");
            }

            this.m_actionExpression = expression;
            this.m_arguments = arguments;
        }

        /// <summary>
        /// Initialize a instance of MvcActionUriXtmlElementRenderer class.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="dataSourceSelector"></param>
        /// <param name="arguments"></param>
        public MvcActionUriXtmlElementRenderer(string actionName, IDictionary<string, string> arguments) {
            if(string.IsNullOrWhiteSpace(actionName)) {
                throw new ArgumentException("The action name is null or empty.", "actionName");
            }

            this.m_actionName = actionName;
            this.m_arguments = arguments;
        }

        protected string m_actionName;
        protected Expression<Func<TController, object>> m_actionExpression;
        protected IDictionary<string, string> m_arguments;

        /// <summary>
        /// Finds argument value form element attributes.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual string FindArgumentValue(IXtmlElement element, object dataSource, IXtmlRenderingContext context, string name) {
            IXtmlAttribute attribute = element.GetAttributesByName(name).FirstOrDefault();
            return attribute != null ? attribute.GetValue(dataSource, context) : null;
        }

        /// <summary>
        /// Finds argument value form data source properties.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual object FindArgumentValue(object dataSource, string name) {
            if(dataSource == null) {
                return null;
            }

            PropertyInfo property = dataSource.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if(property == null || property.GetIndexParameters().Length > 0) {
                return null;
            }

            return property.GetValue(dataSource, null);
        }

        /// <summary>
        /// Gets the dictionary of the route values.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IDictionary<string, object> GetRouteValues(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return this.m_arguments == null ? null : this.m_arguments.ToDictionary(
                (item) => item.Key,
                (item) => this.FindArgumentValue(element, dataSource, context, item.Value) ?? this.FindArgumentValue(dataSource, item.Value));
        }

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public virtual void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Element) {
                throw new XtmlException("The rendered node is not a XTML element node.");
            }

            IDictionary<string, object> dictionary = this.GetRouteValues((IXtmlElement) node, dataSource, context);
            if(this.m_actionExpression != null) {
                writer.Write(WebHelper.CreateUrlHelper().ModularAction<TController>(this.m_actionExpression, dictionary));
            } else {
                writer.Write(WebHelper.CreateUrlHelper().ModularAction<TController>(this.m_actionName, dictionary));
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a implementation of IXtmlNodeRenderer interface which renders the uri of a MVC action.
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public class MvcActionUriXtmlElementRenderer<TController, TSource> : MvcActionUriXtmlElementRenderer<TController>
        where TController : ControllerBase
        where TSource : class {
        /// <summary>
        /// Initialize a instance of MvcActionUriXtmlElementRenderer class.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="arguments"></param>
        public MvcActionUriXtmlElementRenderer(Expression<Func<TController, object>> expression, IDataSourceProvider<TSource> dataSourceProvider, IDictionary<string, string> arguments)
            : this(expression, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, arguments) {
        }

        /// <summary>
        /// Initialize a instance of MvcActionUriXtmlElementRenderer class.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="dataSourceProvider"></param>
        /// <param name="arguments"></param>
        public MvcActionUriXtmlElementRenderer(string actionName, IDataSourceProvider<TSource> dataSourceProvider, IDictionary<string, string> arguments)
            : this(actionName, dataSourceProvider != null ? dataSourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, arguments) {
        }

        /// <summary>
        /// Initialize a instance of MvcActionUriXtmlElementRenderer class.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dataSourceSelector"></param>
        /// <param name="arguments"></param>
        public MvcActionUriXtmlElementRenderer(Expression<Func<TController, object>> expression, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector, IDictionary<string, string> arguments)
            : base(expression, arguments) {
            this.m_dataSourceSelector = dataSourceSelector;
            this.m_dataSourceFinder = DataSourceFinder.Instance;
        }

        /// <summary>
        /// Initialize a instance of MvcActionUriXtmlElementRenderer class.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="dataSourceSelector"></param>
        /// <param name="arguments"></param>
        public MvcActionUriXtmlElementRenderer(string actionName, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> dataSourceSelector, IDictionary<string, string> arguments)
            : base(actionName, arguments) {
            this.m_dataSourceSelector = dataSourceSelector;
            this.m_dataSourceFinder = DataSourceFinder.Instance;
        }

        /// <summary>
        /// Gets the delegate to get data source.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, TSource> m_dataSourceSelector;
        private DataSourceFinder m_dataSourceFinder;

        /// <summary>
        /// Gets the actual data source.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual TSource GetSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            TSource source = null;

            if(this.m_dataSourceSelector != null) {
                source = this.m_dataSourceSelector(element, dataSource, context);
            }
            if(source == null) {
                if(dataSource is TSource) {
                    source = (TSource) dataSource;
                } else if(dataSource != null) {
                    source = this.m_dataSourceFinder.Find<TSource>(dataSource);
                }
            }

            return source;
        }

        /// <summary>
        /// Finds argument value form data source properties.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual object FindArgumentValue(TSource dataSource, string name) {
            if(dataSource == null) {
                return null;
            }

            PropertyInfo property = typeof(TSource).GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if(property == null || property.GetIndexParameters().Length > 0) {
                return null;
            }

            return property.GetValue(dataSource, null);
        }

        /// <summary>
        /// Gets the dictionary of the route values.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected override IDictionary<string, object> GetRouteValues(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            TSource source = this.GetSource(element, dataSource, context);
            if(source == null) {
                throw new XtmlException(string.Format("Can not find data source of {0} type.", typeof(TSource).Name));
            }

            return this.m_arguments == null ? null : this.m_arguments.ToDictionary(
                (item) => item.Key,
                (item) => this.FindArgumentValue(element, dataSource, context, item.Value) ?? this.FindArgumentValue(source, item.Value));
        }
    }

    /// <summary>
    /// Provides the actual data source of a XTML element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataSourceProvider<out T> {
        /// <summary>
        /// Gets the actual data source.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        T GetDataSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context);
    }

    /// <summary>
    /// Formats the property value for DataSourcePropertyXtmlElementRenderer class.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IDataSourcePropertyValueFormater<in TSource, in TValue> {
        /// <summary>
        /// Gets the formated value of the specified property value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        object Format(IXtmlElement element, object dataSource, TSource source, IXtmlRenderingContext context, string propertyName, TValue value, bool isMultiple);
    }

    /// <summary>
    /// Represents a IPropertyValueFormater which encodes property value in a HTML page.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HtmlEncodedStringFormater<TSource, TValue> : IDataSourcePropertyValueFormater<TSource, TValue> {
        #region IDataSourcePropertyValueFormater<TSource,TValue> Members

        /// <inheritdoc />
        public virtual object Format(IXtmlElement element, object dataSource, TSource source, IXtmlRenderingContext context, string propertyName, TValue value, bool isMultiple) {
            return HttpUtility.HtmlEncode(value);
        }

        #endregion
    }

    /// <summary>
    /// Renders all child nodes for each item in the data source of a XTML element node which may represents a sub template.
    /// </summary>
    public class SubTemplateXtmlElementRenderer<T> : XtmlElementIterationRenderer<T> {
        /// <summary>
        /// Initialize a new instance of XtmlComplexNodeRenderer class.
        /// </summary>
        /// <param name="dataSourceGetter"></param>
        /// <param name="subTemplateGetter"></param>
        public SubTemplateXtmlElementRenderer()
            : base() {
        }

        /// <summary>
        /// Initialize a new instance of XtmlComplexNodeRenderer class.
        /// </summary>
        /// <param name="dataSourceProvider"></param>
        /// <param name="subTemplateSelector"></param>
        public SubTemplateXtmlElementRenderer(
            IDataSourceProvider<IEnumerable<T>> dataSourceProvider,
            Func<IXtmlElement, object, IXtmlRenderingContext, IXtmlDocument> subTemplateSelector)
            : base(dataSourceProvider) {
            this.m_subTemplateSelector = subTemplateSelector;
        }

        /// <summary>
        /// Initialize a new instance of XtmlComplexNodeRenderer class.
        /// </summary>
        /// <param name="dataSourceSelector"></param>
        /// <param name="subTemplateSelector"></param>
        public SubTemplateXtmlElementRenderer(
            Func<IXtmlElement, object, IXtmlRenderingContext, IEnumerable<T>> dataSourceSelector,
            Func<IXtmlElement, object, IXtmlRenderingContext, IXtmlDocument> subTemplateSelector)
            : base(dataSourceSelector) {
            this.m_subTemplateSelector = subTemplateSelector;
        }

        /// <summary>
        /// The delegate to get sub template.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, IXtmlDocument> m_subTemplateSelector;

        /// <inheritdoc />
        protected override IEnumerable<IXtmlNode> GetChildNodes(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            IEnumerable<IXtmlNode> childNodes = null;

            if(this.m_subTemplateSelector != null) {
                IXtmlDocument document = this.m_subTemplateSelector(element, dataSource, context);
                if(document != null) {
                    childNodes = document.ChildNodes;
                } else {
                    childNodes = element.ChildNodes;
                }
            } else {
                childNodes = element.ChildNodes;
            }

            return childNodes;
        }
    }

    /// <summary>
    /// Represents a template tag which renders the data source for a XTML node.
    /// </summary>
    /// <typeparam name="T">The type of data source.</typeparam>
    public abstract class XtmlNodeTag<T> : TemplateTag {
        /// <summary>
        /// Gets data source from the specified XTML node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected virtual T GetDataSource(IXtmlNode node, object dataSource, IXtmlRenderingContext context) {
            return (T) dataSource;
        }

        /// <summary>
        /// Renders the data source.
        /// </summary>
        /// <param name="element"></param>        
        /// <param name="dataSource"></param>
        /// <param name="writer"></param>
        protected virtual void RenderDataSource(IXtmlNode node, TextWriter writer, T dataSource, object originalSource, IXtmlRenderingContext context) {
            if(dataSource != null) {
                writer.Write(dataSource.ToString());
            }
        }

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public override void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            T source = this.GetDataSource(node, dataSource, context);
            if(source != null) {
                this.RenderDataSource(node, writer, source, dataSource, context);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a template tag which renders the data source for each child nodes of a XTML element.
    /// </summary>
    /// <typeparam name="T">The type of data source.</typeparam>
    public abstract class XtmlElementTag<T> : TemplateTag {
        /// <summary>
        /// Gets data source from the specified XTML element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected virtual T GetDataSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return (T) dataSource;
        }

        /// <summary>
        /// Renders the data source.
        /// </summary>
        /// <param name="element"></param>        
        /// <param name="dataSource"></param>
        /// <param name="writer"></param>
        protected virtual void RenderDataSource(IXtmlElement element, TextWriter writer, T dataSource, object originalSource, IXtmlRenderingContext context) {
            foreach(IXtmlNode node in element.ChildNodes) {
                node.Render(writer, dataSource, context);
            }
        }

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public override void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Element) {
                throw new XtmlException("The rendered node is not a XTML element node.");
            }

            IXtmlElement element = (IXtmlElement) node;
            T source = this.GetDataSource(element, dataSource, context);
            if(source != null) {
                this.RenderDataSource(element, writer, source, dataSource, context);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a template tag which renders the attached object of parent node.
    /// </summary>
    /// <typeparam name="T">The data type of attached object.</typeparam>
    public abstract class XtmlElementAttachedObjectTag<T> : XtmlElementTag<T> {
        /// <inheritdoc />
        protected override T GetDataSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            // find parent node with not null AttachedObject property
            IXtmlNode parentNode = element;
            while((parentNode = parentNode.ParentNode) != null) {
                if(parentNode.AttachedObject != null) {
                    break;
                }
            }

            if(parentNode == null || !(parentNode.AttachedObject is T)) {
                throw new XtmlException(string.Format("The data type of attached object of parent node is not {0}", typeof(T).FullName));
            }
            return (T) parentNode.AttachedObject;
        }
    }

    /// <summary>
    /// Represents a template tag which renders a property of the data source.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public abstract class DataSourcePropertyTag<TSource> : XtmlElementTag<TSource> where TSource : class {
        protected DataSourcePropertyTag() {
            this.m_dataSourceFinder = DataSourceFinder.Instance;
        }

        /// <summary>
        /// The name of property name attribute.
        /// </summary>
        public const string PROPERTY_ATTRIBUTE_NAME = "Property";

        private DataSourceFinder m_dataSourceFinder;

        /// <summary>
        /// The property dictionary.
        /// </summary>
        protected IDictionary<string, PropertyInfo> m_propertiesCache = new Dictionary<string, PropertyInfo>();
        protected volatile int m_isAddingPropertyCache;

        /// <summary>
        /// Gets name of the property that will be rendered.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual string GetPropertyName(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            IXtmlAttribute attribute = element.GetAttributesByName(PROPERTY_ATTRIBUTE_NAME).FirstOrDefault();
            return attribute != null ? attribute.GetValue(dataSource, context) : null;
        }

        /// <summary>
        /// Gets formated value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract string FormatValue(IXtmlElement element, TSource source, IXtmlRenderingContext context, string propertyName, object value);

        /// <inheritdoc />
        protected override TSource GetDataSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            TSource source = null;

            if(dataSource is TSource) {
                source = (TSource) dataSource;
            } else if(dataSource != null) {
                source = this.m_dataSourceFinder.Find<TSource>(dataSource);
            }

            return source;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual object GetPropertyValue(IXtmlElement element, object dataSource, IXtmlRenderingContext context, TSource source, string propertyName) {
            Type type = typeof(TSource);
            PropertyInfo property = null;

            if(!this.m_propertiesCache.ContainsKey(propertyName)) {
                lock(((ICollection) this.m_propertiesCache).SyncRoot) {
                    if(!this.m_propertiesCache.ContainsKey(propertyName)) {
                        property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if(property == null || property.GetIndexParameters().Length > 0) {
                            throw new XtmlException(string.Format("{0} do not contains a property named {1}.", type.Name, propertyName));
                        }

                        this.m_isAddingPropertyCache = 1;
                        this.m_propertiesCache[propertyName] = property;
                        this.m_isAddingPropertyCache = 0;
                    }
                }
            }

            if(property == null) {
                SpinWait.SpinUntil(() => this.m_isAddingPropertyCache == 0);

                property = this.m_propertiesCache[propertyName];
            }

            return property.GetValue(source, null);
        }

        /// <summary>
        /// Renders the property value to a TextWriter.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="writer"></param>
        protected virtual void RenderPropertyValue(IXtmlElement element, TSource source, IXtmlRenderingContext context, string propertyName, object value, TextWriter writer) {
            writer.Write(this.FormatValue(element, source, context, propertyName, value));
        }

        /// <inheritdoc />
        protected override void RenderDataSource(IXtmlElement element, TextWriter writer, TSource dataSource, object originalSource, IXtmlRenderingContext context) {
            string propertyName = this.GetPropertyName(element, dataSource, context);
            if(string.IsNullOrWhiteSpace(propertyName)) {
                throw new XtmlException("Property name is empty.");
            }

            this.RenderPropertyValue(element, dataSource, context, propertyName, this.GetPropertyValue(element, originalSource, context, dataSource, propertyName), writer);
        }
    }

    /// <summary>
    /// Represents a template tag which renders a property of the data source.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class DataSourcePropertyTag<TSource, TValue> : DataSourcePropertyTag<TSource> where TSource : class {
        /// <summary>
        /// Gets formated value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract string FormatValue(IXtmlElement element, TSource source, IXtmlRenderingContext context, string propertyName, TValue value);

        /// <inheritdoc />
        protected override string FormatValue(IXtmlElement element, TSource source, IXtmlRenderingContext context, string propertyName, object value) {
            return this.FormatValue(element, source, context, propertyName, (TValue) value);
        }
    }

    /// <summary>
    /// Represents a template tag which renders child nodes of a XTML element for each item in the data source.
    /// </summary>
    /// <typeparam name="T">The type of item in the data source.</typeparam>
    public abstract class XtmlElementIterationTag<T> : XtmlElementTag<IEnumerable<T>> {
        public const string HEADER_TEMPLATE_TAG_NAME = "Header";
        public const string FOOTER_TEMPLATE_TAG_NAME = "Footer";
        public const string ITEM_TEMPLATE_TAG_NAME = "Item";
        public const string ALTERNATING_ITEM_TEMPLATE_TAG_NAME = "AlternatingItem";
        public const string SEPARATOR_TEMPLATE_TAG_NAME = "Separator";
        public const string ONE_BY_ONE_ATTRIBUTE_NAME = "OneByOne";

        public const string ITEM_INDEX_ATTRIBUTE_NAME = "Index";

        protected object m_lock = new object();

        /// <inheritdoc />
        public override string Description {
            get {
                return string.Format("备注：若需要设置数据项具体的显示模板，请使用下面的子标签{0}",
                    new string[] {
                        string.Format("\r\n{0}：必选的数据项模板，{1}属性设置从零开始的数据项序号，未定义{1}属性或{1}属性值为空表示是默认项模板", ITEM_TEMPLATE_TAG_NAME, ITEM_INDEX_ATTRIBUTE_NAME),
                        string.Format("\r\n{0}：可选的交替项模板", ALTERNATING_ITEM_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：可选的分隔符模板，实现华丽的分隔符", SEPARATOR_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：可选的标头模板", HEADER_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：可选的注脚模板", FOOTER_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：值为true表示禁用循环渲染，而是依次渲染每个子标签一次", ONE_BY_ONE_ATTRIBUTE_NAME),
                    }.StringJoin(string.Empty));
            }
        }

        protected IEnumerable<IXtmlElement> FindElements(IEnumerable<IXtmlNode> childNodes, string localName) {
            return from item in childNodes
                   where item.NodeType == XtmlNodeType.Element && string.Equals(item.LocalName, localName, StringComparison.OrdinalIgnoreCase)
                   select (IXtmlElement) item;
        }

        /// <summary>
        /// Gets child nodes.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IXtmlNode> GetChildNodes(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return element.ChildNodes;
        }

        /// <summary>
        /// Determines the render mode.
        /// True:  render all child nodes for each item in data source.
        /// False: render one child by one data source item.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="originalSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool IsCycled(IXtmlElement element, object originalSource, IXtmlRenderingContext context) {
            bool oneByOne = false;
            IXtmlAttribute attribute = element.GetAttributesByName(ONE_BY_ONE_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute != null) {
                bool.TryParse(attribute.GetValue(originalSource, context), out oneByOne);
            }

            return !oneByOne;
        }

        /// <summary>
        /// Renders cycled.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="writer"></param>
        /// <param name="dataSource"></param>
        /// <param name="childNodes"></param>
        /// <param name="context"></param>
        protected virtual void RenderCycled(IXtmlElement element, TextWriter writer, IEnumerable<T> dataSource, IXtmlRenderingContext context, IEnumerable<IXtmlNode> childNodes) {
            // find default item template and consturct templates map
            IXtmlElement itemTemplate = null;
            IDictionary<int, IXtmlElement> templatesMap = new Dictionary<int, IXtmlElement>();

            int indexValue = 0;
            string indexAttributeValue = null;
            IXtmlAttribute indexAttribute = null;
            foreach(IXtmlElement item in this.FindElements(childNodes, ITEM_TEMPLATE_TAG_NAME)) {
                if((indexAttribute = item.GetAttributesByName(ITEM_INDEX_ATTRIBUTE_NAME).FirstOrDefault()) == null) {
                    if(itemTemplate == null) {
                        itemTemplate = item;
                    }
                } else {
                    if(string.IsNullOrWhiteSpace(indexAttributeValue = indexAttribute.GetValue(dataSource, context))) {
                        if(itemTemplate == null) {
                            itemTemplate = item;
                        }
                    } else {
                        if(int.TryParse(indexAttributeValue, out indexValue)) {
                            if(indexValue < 0) {
                                throw new XtmlException(string.Format("The value of {1} attribute in {0} tag is less than zero.", ITEM_TEMPLATE_TAG_NAME, ITEM_INDEX_ATTRIBUTE_NAME, indexAttributeValue));
                            }

                            templatesMap[indexValue] = item;
                        } else {
                            throw new XtmlException(string.Format("The value of {1} attribute in {0} tag is not a number.", ITEM_TEMPLATE_TAG_NAME, ITEM_INDEX_ATTRIBUTE_NAME, indexAttributeValue));
                        }
                    }
                }
            }

            IXtmlElement alternatingTemplate = this.FindElements(childNodes, ALTERNATING_ITEM_TEMPLATE_TAG_NAME).FirstOrDefault();
            IXtmlElement separatorTemplate = this.FindElements(childNodes, SEPARATOR_TEMPLATE_TAG_NAME).FirstOrDefault();
            if(itemTemplate == null && (alternatingTemplate != null || separatorTemplate != null)) {
                throw new XtmlException("The default item template is not defined.");
            }

            if(itemTemplate != null) {
                int index = 0;
                IXtmlElement template = null;

                lock(this.m_lock) {
                    foreach(T item in dataSource) {
                        // render separator if not first item
                        if(index > 0 && separatorTemplate != null) {
                            foreach(IXtmlNode child in separatorTemplate.ChildNodes) {
                                child.Render(writer, item, context);
                            }
                        }

                        // render current item
                        if(templatesMap.ContainsKey(index)) {
                            template = templatesMap[index];
                        } else if(index % 2 == 1 && alternatingTemplate != null) {
                            template = alternatingTemplate;
                        } else {
                            template = itemTemplate;
                        }

                        element.AttachedObject = index++;
                        foreach(IXtmlNode child in template.ChildNodes) {
                            child.Render(writer, item, context);
                        }
                    }
                }
            } else {
                int index = 0;

                lock(this.m_lock) {
                    foreach(T item in dataSource) {
                        element.AttachedObject = index++;
                        foreach(IXtmlNode child in childNodes) {
                            child.Render(writer, item, context);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders one by one.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="writer"></param>
        /// <param name="dataSource"></param>
        /// <param name="childNodes"></param>
        /// <param name="context"></param>
        protected virtual void RenderOneByOne(IXtmlElement element, TextWriter writer, IEnumerable<T> dataSource, IXtmlRenderingContext context, IEnumerable<IXtmlNode> childNodes) {
            int dataIndex = -1, elementIndex = -1;
            IEnumerator<T> source = dataSource.GetEnumerator();
            IEnumerator<IXtmlNode> nodes = childNodes.GetEnumerator();
            if(!source.MoveNext()) {
                return;
            }

            dataIndex = 0;

            lock(this.m_lock) {
                while(nodes.MoveNext()) {
                    if(nodes.Current.NodeType == XtmlNodeType.Element) {
                        if(++elementIndex > dataIndex) {
                            if(source.MoveNext()) {
                                ++dataIndex;
                            } else {
                                break;
                            }
                        }
                    }

                    element.AttachedObject = dataIndex;
                    nodes.Current.Render(writer, source.Current, context);
                }
            }
        }

        /// <inheritdoc />
        protected override void RenderDataSource(IXtmlElement element, TextWriter writer, IEnumerable<T> dataSource, object originalSource, IXtmlRenderingContext context) {
            ICollection<IXtmlNode> childNodes = new List<IXtmlNode>(this.GetChildNodes(element, originalSource, context));

            IXtmlElement headerTemplate = this.FindElements(childNodes, HEADER_TEMPLATE_TAG_NAME).FirstOrDefault();
            IXtmlElement footerTemplate = this.FindElements(childNodes, FOOTER_TEMPLATE_TAG_NAME).FirstOrDefault();
            if(headerTemplate != null) {
                childNodes.Remove(headerTemplate);
            }
            if(footerTemplate != null) {
                childNodes.Remove(footerTemplate);
            }

            if(headerTemplate != null) {
                headerTemplate.Render(writer, dataSource, context);
            }
            if(dataSource != null) {
                if(this.IsCycled(element, originalSource, context)) {
                    this.RenderCycled(element, writer, dataSource, context, childNodes);
                } else {
                    this.RenderOneByOne(element, writer, dataSource, context, childNodes);
                }
            } else {
                foreach(IXtmlNode child in childNodes) {
                    child.Render(writer, null, context);
                }
            }
            if(footerTemplate != null) {
                footerTemplate.Render(writer, dataSource, context);
            }
        }
    }

    /// <summary>
    /// Provides a default implementation of IXtmlNodeRenderer interface.
    /// </summary>
    public class XtmlNodeRenderer : IXtmlNodeRenderer {
        /// <summary>
        /// Initialize a new instance of XtmlNodeRenderer class.
        /// </summary>
        public XtmlNodeRenderer()
            : this(null) {
        }

        /// <summary>
        /// Initialize a new instance of XtmlNodeRenderer class.
        /// </summary>
        /// <param name="value"></param>
        public XtmlNodeRenderer(object value)
            : this((node, writer, dataSource, context) => writer.Write(value)) {
        }

        /// <summary>
        /// Initialize a new instance of XtmlNodeRenderer class.
        /// </summary>
        /// <param name="renderAction"></param>
        public XtmlNodeRenderer(Action<IXtmlNode, TextWriter, object, IXtmlRenderingContext> renderAction) {
            if(renderAction == null) {
                renderAction = (node, writer, dataSource, context) => {
                    if(dataSource != null) {
                        writer.Write(dataSource.ToString());
                    }
                };
            }

            this.m_renderAction = renderAction;
        }

        /// <summary>
        /// Gets the action when render node.
        /// </summary>
        protected Action<IXtmlNode, TextWriter, object, IXtmlRenderingContext> m_renderAction;

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            this.m_renderAction(node, writer, dataSource, context);
        }

        #endregion
    }

    /// <summary>
    /// Renders the data source of a XTML element node.
    /// </summary>
    /// <typeparam name="T">The type of data source.</typeparam>
    public class XtmlElementRenderer<T> : IXtmlNodeRenderer where T : class {
        /// <summary>
        /// Initialize a new instance of XtmlElementRenderer class.
        /// </summary>
        public XtmlElementRenderer() {
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementRenderer class.
        /// </summary>
        /// <param name="dataSourceProvider"></param>
        /// <param name="renderAction"></param>
        public XtmlElementRenderer(IDataSourceProvider<T> dataSourceProvider, Action<IXtmlElement, TextWriter, T, IXtmlRenderingContext> renderAction) {
            this.m_dataSourceSelector = dataSourceProvider.GetDataSource;
            this.m_renderAction = renderAction;
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementRenderer class.
        /// </summary>
        /// <param name="dataSourceSelector"></param>
        /// <param name="renderAction"></param>
        public XtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, T> dataSourceSelector, Action<IXtmlElement, TextWriter, T, IXtmlRenderingContext> renderAction) {
            this.m_dataSourceSelector = dataSourceSelector;
            this.m_renderAction = renderAction;
        }

        /// <summary>
        /// The delegate to get data source.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, T> m_dataSourceSelector;

        /// <summary>
        /// The delegate used to render data source.
        /// </summary>
        protected Action<IXtmlElement, TextWriter, T, IXtmlRenderingContext> m_renderAction;

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Element) {
                throw new XtmlException("The rendered node is not a XTML element node.");
            }

            IXtmlElement element = (IXtmlElement) node;
            T source = this.m_dataSourceSelector != null ? this.m_dataSourceSelector(element, dataSource, context) : null;
            if(source != null && this.m_renderAction != null) {
                this.m_renderAction(element, writer, source, context);
            }
        }

        #endregion
    }

    /// <summary>
    /// Renders all child nodes for each item in the data source of a XTML element node.
    /// </summary>
    /// <typeparam name="T">The type of item in the data source.</typeparam>
    public class XtmlElementIterationRenderer<T> : IXtmlNodeRenderer {
        /// <summary>
        /// Initialize a new instance of XtmlElementIterationRenderer class.
        /// </summary>
        public XtmlElementIterationRenderer() {
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementIterationRenderer class.
        /// </summary>
        /// <param name="dataSourceProvider"></param>
        public XtmlElementIterationRenderer(IDataSourceProvider<IEnumerable<T>> dataSourceProvider) {
            this.m_dataSourceSelector = dataSourceProvider.GetDataSource;
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementIterationRenderer class.
        /// </summary>
        /// <param name="dataSourceSelector"></param>
        public XtmlElementIterationRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, IEnumerable<T>> dataSourceSelector) {
            this.m_dataSourceSelector = dataSourceSelector;
        }

        public const string HEADER_TEMPLATE_TAG_NAME = "Header";
        public const string FOOTER_TEMPLATE_TAG_NAME = "Footer";
        public const string ITEM_TEMPLATE_TAG_NAME = "Item";
        public const string ALTERNATING_ITEM_TEMPLATE_TAG_NAME = "AlternatingItem";
        public const string SEPARATOR_TEMPLATE_TAG_NAME = "Separator";
        public const string ONE_BY_ONE_ATTRIBUTE_NAME = "OneByOne";

        public const string ITEM_INDEX_ATTRIBUTE_NAME = "Index";

        public static string Description {
            get {
                return string.Format("备注：若需要设置数据项具体的显示模板，请使用下面的子标签{0}",
                    new string[] {
                        string.Format("\r\n{0}：必选的数据项模板，{1}属性设置从零开始的数据项序号，未定义{1}属性或{1}属性值为空表示是默认项模板", ITEM_TEMPLATE_TAG_NAME, ITEM_INDEX_ATTRIBUTE_NAME),
                        string.Format("\r\n{0}：可选的交替项模板", ALTERNATING_ITEM_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：可选的分隔符模板，实现华丽的分隔符", SEPARATOR_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：可选的标头模板", HEADER_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：可选的注脚模板", FOOTER_TEMPLATE_TAG_NAME),
                        string.Format("\r\n{0}：值为true表示禁用循环渲染，而是依次渲染每个子标签一次", ONE_BY_ONE_ATTRIBUTE_NAME),
                    }.StringJoin(string.Empty));
            }
        }

        protected object m_lock = new object();

        /// <summary>
        /// The delegate to get data source.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, IEnumerable<T>> m_dataSourceSelector;

        /// <summary>
        /// Gets the actual data for iteration.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetIterationData(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return this.m_dataSourceSelector != null ? this.m_dataSourceSelector(element, dataSource, context) : (IEnumerable<T>) dataSource;
        }

        /// <summary>
        /// Gets child nodes.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IXtmlNode> GetChildNodes(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return element.ChildNodes;
        }

        protected IEnumerable<IXtmlElement> FindElements(IEnumerable<IXtmlNode> childNodes, string localName) {
            return from item in childNodes
                   where item.NodeType == XtmlNodeType.Element && string.Equals(item.LocalName, localName, StringComparison.OrdinalIgnoreCase)
                   select (IXtmlElement) item;
        }

        /// <summary>
        /// Determines the render mode.
        /// True:  render all child nodes for each item in data source.
        /// False: render one child by one data source item.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="originalSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool IsCycled(IXtmlElement element, object originalSource, IXtmlRenderingContext context) {
            bool oneByOne = false;
            IXtmlAttribute attribute = element.GetAttributesByName(ONE_BY_ONE_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute != null) {
                bool.TryParse(attribute.GetValue(originalSource, context), out oneByOne);
            }

            return !oneByOne;
        }

        /// <summary>
        /// Renders cycled.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="writer"></param>
        /// <param name="dataSource"></param>
        /// <param name="childNodes"></param>
        /// <param name="context"></param>
        protected virtual void RenderCycled(IXtmlElement element, TextWriter writer, IEnumerable<T> dataSource, IXtmlRenderingContext context, IEnumerable<IXtmlNode> childNodes) {
            // find default item template and consturct templates map
            IXtmlElement itemTemplate = null;
            IDictionary<int, IXtmlElement> templatesMap = new Dictionary<int, IXtmlElement>();

            int indexValue = 0;
            string indexAttributeValue = null;
            IXtmlAttribute indexAttribute = null;
            foreach(IXtmlElement item in this.FindElements(childNodes, ITEM_TEMPLATE_TAG_NAME)) {
                if((indexAttribute = item.GetAttributesByName(ITEM_INDEX_ATTRIBUTE_NAME).FirstOrDefault()) == null) {
                    if(itemTemplate == null) {
                        itemTemplate = item;
                    }
                } else {
                    if(string.IsNullOrWhiteSpace(indexAttributeValue = indexAttribute.GetValue(dataSource, context))) {
                        if(itemTemplate == null) {
                            itemTemplate = item;
                        }
                    } else {
                        if(int.TryParse(indexAttributeValue, out indexValue)) {
                            if(indexValue < 0) {
                                throw new XtmlException(string.Format("The value of {1} attribute in {0} tag is less than zero.", ITEM_TEMPLATE_TAG_NAME, ITEM_INDEX_ATTRIBUTE_NAME, indexAttributeValue));
                            }

                            templatesMap[indexValue] = item;
                        } else {
                            throw new XtmlException(string.Format("The value of {1} attribute in {0} tag is not a number.", ITEM_TEMPLATE_TAG_NAME, ITEM_INDEX_ATTRIBUTE_NAME, indexAttributeValue));
                        }
                    }
                }
            }

            IXtmlElement alternatingTemplate = this.FindElements(childNodes, ALTERNATING_ITEM_TEMPLATE_TAG_NAME).FirstOrDefault();
            IXtmlElement separatorTemplate = this.FindElements(childNodes, SEPARATOR_TEMPLATE_TAG_NAME).FirstOrDefault();
            if(itemTemplate == null && (alternatingTemplate != null || separatorTemplate != null)) {
                throw new XtmlException("The default item template is not defined.");
            }

            if(itemTemplate != null) {
                int index = 0;
                IXtmlElement template = null;

                lock(this.m_lock) {
                    foreach(T item in dataSource) {
                        // render separator if not first item
                        if(index > 0 && separatorTemplate != null) {
                            foreach(IXtmlNode child in separatorTemplate.ChildNodes) {
                                child.Render(writer, item, context);
                            }
                        }

                        // render current item
                        if(templatesMap.ContainsKey(index)) {
                            template = templatesMap[index];
                        } else if(index % 2 == 1 && alternatingTemplate != null) {
                            template = alternatingTemplate;
                        } else {
                            template = itemTemplate;
                        }

                        element.AttachedObject = index++;
                        foreach(IXtmlNode child in template.ChildNodes) {
                            child.Render(writer, item, context);
                        }
                    }
                }
            } else {
                int index = 0;

                lock(this.m_lock) {
                    foreach(T item in dataSource) {
                        element.AttachedObject = index++;
                        foreach(IXtmlNode child in childNodes) {
                            child.Render(writer, item, context);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders one by one.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="writer"></param>
        /// <param name="dataSource"></param>
        /// <param name="childNodes"></param>
        /// <param name="context"></param>
        protected virtual void RenderOneByOne(IXtmlElement element, TextWriter writer, IEnumerable<T> dataSource, IXtmlRenderingContext context, IEnumerable<IXtmlNode> childNodes) {
            IXtmlNode currentNode = null;
            int dataIndex = -1, elementIndex = -1;
            IEnumerator<T> source = dataSource.GetEnumerator();
            IEnumerator<IXtmlNode> nodes = childNodes.GetEnumerator();
            if(!source.MoveNext()) {
                return;
            }

            dataIndex = 0;

            lock(this.m_lock) {
                while(nodes.MoveNext()) {
                    currentNode = nodes.Current;
                    if(currentNode.NodeType == XtmlNodeType.Element) {
                        if(++elementIndex > dataIndex) {
                            if(source.MoveNext()) {
                                ++dataIndex;
                            } else {
                                break;
                            }
                        }
                    }

                    element.AttachedObject = dataIndex;
                    currentNode.Render(writer, source.Current, context);
                }
            }
        }

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            if(node.NodeType != XtmlNodeType.Element) {
                throw new XtmlException("The rendered node is not a XTML element node.");
            }

            IXtmlElement element = (IXtmlElement) node;
            /*
             * don't modify the original DOM tree, so create a new child nodes collection.
             */
            ICollection<IXtmlNode> childNodes = new List<IXtmlNode>(this.GetChildNodes(element, dataSource, context));
            IEnumerable<T> source = this.GetIterationData(element, dataSource, context);

            IXtmlElement headerTemplate = this.FindElements(childNodes, HEADER_TEMPLATE_TAG_NAME).FirstOrDefault();
            IXtmlElement footerTemplate = this.FindElements(childNodes, FOOTER_TEMPLATE_TAG_NAME).FirstOrDefault();
            if(headerTemplate != null) {
                childNodes.Remove(headerTemplate);
            }
            if(footerTemplate != null) {
                childNodes.Remove(footerTemplate);
            }

            if(headerTemplate != null) {
                headerTemplate.Render(writer, source, context);
            }
            if(source != null) {
                if(this.IsCycled(element, dataSource, context)) {
                    this.RenderCycled(element, writer, source, context, childNodes);
                } else {
                    this.RenderOneByOne(element, writer, source, context, childNodes);
                }
            } else {
                foreach(IXtmlNode child in childNodes) {
                    child.Render(writer, null, context);
                }
            }
            if(footerTemplate != null) {
                footerTemplate.Render(writer, source, context);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a sub template tag which renders all child nodes of a XTML element for each item in the data source.
    /// </summary>
    /// <typeparam name="T">The type of item in the data source.</typeparam>
    public abstract class SubTemplateXtmlElementTag<T> : XtmlElementIterationTag<T> {
        /// <summary>
        /// Gets the sub template correspounds the specified XTML element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract IXtmlDocument GetSubTemplate(IXtmlElement element, object dataSource, IXtmlRenderingContext context);

        /// <inheritdoc />
        protected override IEnumerable<IXtmlNode> GetChildNodes(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            IEnumerable<IXtmlNode> childNodes = null;

            IXtmlDocument document = this.GetSubTemplate(element, dataSource, context);
            if(document != null) {
                childNodes = document.ChildNodes;
            } else {
                childNodes = element.ChildNodes;
            }

            return childNodes;
        }
    }

    /// <summary>
    /// Defines the contract that class factories must implement to create new ITemplateTag objects.
    /// </summary>
    public interface ITemplateTagFactory {
        /// <summary>
        /// Gets a instance of class that implements the ITemplateTag interface.
        /// If the tag is not found, then return null.
        /// </summary>
        /// <param name="tagName">The name of tag.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="tagName"/> is null or empty.</exception>
        ITemplateTag GetTag(string tagName);

        /// <summary>
        /// Gets all template tags that is created by this factory.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITemplateTag> GetTags();
    }

    /// <summary>
    /// Provides a pool of template tags.
    /// </summary>
    public class TemplateTagPool : ITemplateTagFactory {
        /// <summary>
        /// Initialize a new instance of TemplateTagPool class.
        /// </summary>
        public TemplateTagPool() {
            this.Initialize();
        }

        /// <summary>
        /// The pool of tags, key is tag name, value is a ITemplateTag object.
        /// </summary>
        protected IDictionary<string, ITemplateTag> m_pool = new ConcurrentDictionary<string, ITemplateTag>();

        /// <summary>
        /// Initialize this pool.
        /// </summary>
        protected virtual void Initialize() {
        }

        /// <summary>
        /// Registers the specified tag to this pool.
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <param name="tag"></param>
        /// <returns>The registered tag.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="tag"/> is null.</exception>
        public ITemplateTag Register(ITemplateTag tag) {
            if(tag == null) {
                throw new ArgumentException("The tag will be registered is null.", "tag");
            }

            return this.m_pool[tag.TagName] = tag;
        }

        #region ITemplateTagFactory Members

        /// <inheritdoc />
        public ITemplateTag GetTag(string tagName) {
            if(string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException("The tag name is null or empty.", "tagName");
            }

            return this.m_pool.ContainsKey(tagName) ? this.m_pool[tagName] : null;
        }

        /// <inheritdoc />
        public IEnumerable<ITemplateTag> GetTags() {
            return this.m_pool.Values;
        }

        #endregion
    }

    /// <summary>
    /// Loads template tags from some assemblies.
    /// </summary>
    public class AssembliesTemplateTagFactory : ITemplateTagFactory {
        /// <summary>
        /// Initialize a new instance of AssembliesTemplateTagFactory class.
        /// </summary>
        /// <param name="assemblies"></param>
        public AssembliesTemplateTagFactory(params Assembly[] assemblies)
            : this((IEnumerable<Assembly>) assemblies) {
        }

        /// <summary>
        /// Initialize a new instance of AssembliesTemplateTagFactory class.
        /// </summary>
        /// <param name="assemblies"></param>
        public AssembliesTemplateTagFactory(IEnumerable<Assembly> assemblies) {
            this.m_assemblies = assemblies;
        }

        protected object m_lock = new object();
        protected IEnumerable<Assembly> m_assemblies;
        protected IDictionary<string, ITemplateTag> m_tags;

        protected virtual void Initialize() {
            if(this.m_tags == null) {
                lock(this.m_lock) {
                    if(this.m_tags == null) {
                        this.m_tags = this.LoadTags(this.m_assemblies).ToDictionary((item) => item.TagName);
                    }
                }
            }
        }

        /// <summary>
        /// Loads template tags from the specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ITemplateTag> LoadTags(IEnumerable<Assembly> assemblies) {
            return TypeUtility.LoadInstances<ITemplateTag>(assemblies);
        }

        #region ITemplateTagFactory Members

        public ITemplateTag GetTag(string tagName) {
            if(string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException("The tag name is null or empty.", "tagName");
            }

            this.Initialize();

            return this.m_tags.ContainsKey(tagName) ? this.m_tags[tagName] : null;
        }

        public IEnumerable<ITemplateTag> GetTags() {
            this.Initialize();

            return this.m_tags.Values;
        }

        #endregion
    }

    /// <summary>
    /// Provides a default implementation of ITemplateTag interface.
    /// </summary>
    public abstract class TemplateTag : ITemplateTag {
        #region ITemplateTag Members

        /// <inheritdoc />
        public abstract string Name {
            get;
        }

        /// <inheritdoc />
        public abstract string TagName {
            get;
        }

        /// <inheritdoc />
        public abstract string Category {
            get;
        }

        /// <inheritdoc />
        public abstract string Description {
            get;
        }

        /// <inheritdoc />
        public virtual bool IsVisible {
            get {
                return true;
            }
        }

        /// <inheritdoc />
        public abstract string GetShorthand(XtmlMarkupContext context);

        /// <inheritdoc />
        public abstract string GetPattern(XtmlMarkupContext context);

        #endregion

        #region IXtmlNodeRenderer Members

        /// <inheritdoc />
        public abstract void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context);

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.TagName.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is ITemplateTag)) {
                return false;
            }

            return string.Equals(this.TagName, ((ITemplateTag) obj).TagName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.TagName;
        }

        #endregion
    }

    /// <summary>
    /// Provides system functions of handle templates.
    /// </summary>
    public class TemplateSystem {
        #region Singleton

        private TemplateSystem() {
            this.m_textBufferSize = DEFAULT_TEXT_BUFFER_SIZE;
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new TemplateSystem();
            }

            public static TemplateSystem Instance;
        }

        /// <summary>
        /// Gets the unique instance of TemplateSystem class.
        /// </summary>
        internal static TemplateSystem Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        public const int MIN_TEXT_BUFFER_SIZE = 1024;
        public const int DEFAULT_TEXT_BUFFER_SIZE = 1024 * 1024;

        /// <summary>
        /// Gets the buffer size of stream writer which writes text to a file.
        /// </summary>
        private int m_textBufferSize;
        public int TextBufferSize {
            get {
                return this.m_textBufferSize;
            }
            set {
                this.m_textBufferSize = value < MIN_TEXT_BUFFER_SIZE ? MIN_TEXT_BUFFER_SIZE : value;
            }
        }

        /// <summary>
        /// Loads a template from the specified TextReader object.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="option"></param>
        /// <param name="documentName"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="reader"/> or <paramref name="option"/> is null. <paramref name="documentName"/> is null or empty.</exception>
        public IXtmlDocument Load(TextReader reader, XtmlDocumentOption option, string documentName) {
            if(reader == null) {
                throw new ArgumentException("The TextReader is null.", "reader");
            }
            if(option == null) {
                throw new ArgumentException("Document option is null.", "option");
            }

            return this.Load(reader.ReadToEnd(), option, documentName);
        }

        /// <summary>
        /// Loads a template from the specified XTML string.
        /// </summary>
        /// <param name="xtml"></param>
        /// <param name="option"></param>
        /// <param name="documentName"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="xtml"/> or <paramref name="option"/> is null. <paramref name="documentName"/> is null or empty.</exception>
        public IXtmlDocument Load(string xtml, XtmlDocumentOption option, string documentName) {
            if(xtml == null) {
                throw new ArgumentException("The XTML string is null.", "xtml");
            }
            if(option == null) {
                throw new ArgumentException("Document option is null.", "option");
            }

            return XtmlDocument.Load(xtml, option, documentName);
        }

        /// <summary>
        /// Renders the specified template to a file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="filePath"></param>
        /// <exception cref="System.ArgumentException"><paramref name="document"/> is null. <paramref name="filePath"/> is null or empty or not a valid local path.</exception>
        public void Render(IXtmlDocument document, object dataSource, IXtmlRenderingContext context, string filePath) {
            if(document == null) {
                throw new ArgumentException("Document is null.", "document");
            }
            if(string.IsNullOrWhiteSpace(filePath)) {
                throw new ArgumentException("The file path is null or empty.", "filePath");
            }
            if(!PathUtility.IsValidLocalPath(filePath)) {
                throw new ArgumentException("The file path is not a valid local path.", "filePath");
            }

            string folder = Path.GetDirectoryName(filePath);
            if(!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            using(StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding(document.Declaration.Encoding), this.m_textBufferSize)) {
                document.Render(writer, dataSource, context);
            }
        }

        /// <summary>
        /// Renders the specified template to HttpResponse.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="response"></param>
        /// <exception cref="System.ArgumentException"><paramref name="document"/> or <paramref name="response"/> is null.</exception>
        public void Render(IXtmlDocument document, object dataSource, IXtmlRenderingContext context, HttpResponse response) {
            if(document == null) {
                throw new ArgumentException("Document is null.", "document");
            }
            if(response == null) {
                throw new ArgumentException("The http response is null.", "response");
            }

            response.ContentType = document.Declaration.Type;
            response.ContentEncoding = Encoding.GetEncoding(document.Declaration.Encoding);
            response.ClearContent();
            document.Render(response.Output, dataSource, context);
            response.Flush();
        }

        /// <summary>
        /// Renders the specified template to HttpResponse.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="response"></param>
        /// <exception cref="System.ArgumentException"><paramref name="document"/> or <paramref name="response"/> is null.</exception>
        public void Render(IXtmlDocument document, object dataSource, IXtmlRenderingContext context, HttpResponseBase response) {
            if(document == null) {
                throw new ArgumentException("Document is null.", "document");
            }
            if(response == null) {
                throw new ArgumentException("The http response is null.", "response");
            }

            response.ContentType = document.Declaration.Type;
            response.ContentEncoding = Encoding.GetEncoding(document.Declaration.Encoding);
            response.ClearContent();
            document.Render(response.Output, dataSource, context);
            response.Flush();
        }

        /// <summary>
        /// Get the output content of the specified template.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <exception cref="System.ArgumentException"><paramref name="document"/> is null.</exception>
        public string GetOutputContent(IXtmlDocument document, object dataSource, IXtmlRenderingContext context) {
            if(document == null) {
                throw new ArgumentException("Document is null.", "document");
            }

            StringBuilder content = new StringBuilder(document.NodeXtml.Length * 2);
            using(TextWriter writer = new StringWriter(content)) {
                document.Render(writer, dataSource, context);
            }
            return content.ToString();
        }
    }

    /// <summary>
    /// Provides system functions of handle template tags.
    /// </summary>
    public class TagSystem : ITemplateTagFactory {
        #region Singleton

        private TagSystem() {
            this.m_factories.Add(this.m_pool);
            this.SetDefaultTextRender(null);
            this.SetDefaultDocumentRender(null);
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new TagSystem();
            }

            public static TagSystem Instance;
        }

        /// <summary>
        /// Gets the unique instance of TagSystem class.
        /// </summary>
        internal static TagSystem Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        /// <summary>
        /// A tag pool.
        /// </summary>
        private TemplateTagPool m_pool = new TemplateTagPool();

        /// <summary>
        /// All registered tag factories.
        /// </summary>
        private ICollection<ITemplateTagFactory> m_factories = new List<ITemplateTagFactory>();

        /// <summary>
        /// The tag cache, key is tag name, value is a ITemplateTag object.
        /// </summary>
        private IDictionary<string, ITemplateTag> m_tagCache = new Dictionary<string, ITemplateTag>();
        protected volatile int m_isAddingTagCache;

        /// <summary>
        /// Sets the default render logic of IXtmlText nodes.
        /// </summary>
        /// <param name="render"></param>
        public void SetDefaultTextRender(IXtmlNodeRenderer render) {
            lock(((ICollection) this.m_tagCache).SyncRoot) {
                this.m_isAddingTagCache = 1;
                this.m_tagCache[XtmlText.DEFAULT_NAME] = new XtmlNodeRendererWrapper(render ?? DefaultXtmlTextRenderer.Instance) {
                    TagName = XtmlText.DEFAULT_NAME,
                };
                this.m_isAddingTagCache = 0;
            }
        }

        /// <summary>
        /// Sets the default render logic of XTML documents.
        /// </summary>
        /// <param name="render"></param>
        public void SetDefaultDocumentRender(IXtmlNodeRenderer render) {
            lock(((ICollection) this.m_tagCache).SyncRoot) {
                this.m_isAddingTagCache = 1;
                this.m_tagCache[XtmlDocument.DEFAULT_NAME] = new XtmlNodeRendererWrapper(render ?? DefaultXtmlComplexNodeRenderer.Instance) {
                    TagName = XtmlDocument.DEFAULT_NAME,
                };
                this.m_isAddingTagCache = 0;
            }
        }

        /// <summary>
        /// Registers a template tag factory.
        /// </summary>
        /// <param name="factory"></param>
        /// <exception cref="System.ArgumentException"><paramref name="factory"/> is null.</exception>
        public void RegisterFactory(ITemplateTagFactory factory) {
            if(factory == null) {
                throw new ArgumentException("The factory will be registered is null.", "factory");
            }

            lock(((ICollection) this.m_factories).SyncRoot) {
                if(!this.m_factories.Contains(factory)) {
                    this.m_factories.Add(factory);
                }
            }
        }

        /// <summary>
        /// Registers a template tag whose priority is greater than the tags created by registered tag factories.
        /// </summary>
        /// <param name="tag"></param>
        /// <exception cref="System.ArgumentException"><paramref name="tag"/> is null.</exception>
        public void RegisterTag(ITemplateTag tag) {
            if(tag == null) {
                throw new ArgumentException("The tag will be registered is null.", "factory");
            }

            lock(((ICollection) this.m_tagCache).SyncRoot) {
                this.m_isAddingTagCache = 1;
                this.m_tagCache[tag.TagName] = this.m_pool.Register(tag);
                this.m_isAddingTagCache = 0;
            }
        }

        #region ITemplateTagFactory Members

        /// <inheritdoc />
        public ITemplateTag GetTag(string tagName) {
            if(string.IsNullOrWhiteSpace(tagName)) {
                throw new ArgumentException("The tag name is null or empty.", "tagName");
            }

            ITemplateTag tag = null;

            if(!this.m_tagCache.ContainsKey(tagName)) {
                lock(((ICollection) this.m_tagCache).SyncRoot) {
                    if(!this.m_tagCache.ContainsKey(tagName)) {
                        foreach(ITemplateTagFactory factory in this.m_factories) {
                            if((tag = factory.GetTag(tagName)) != null) {
                                this.m_isAddingTagCache = 1;
                                this.m_tagCache[tagName] = tag;
                                this.m_isAddingTagCache = 0;
                                break;
                            }
                        }
                    } else {
                        tag = this.m_tagCache[tagName];
                    }
                }
            } else {
                SpinWait.SpinUntil(() => this.m_isAddingTagCache == 0);

                tag = this.m_tagCache[tagName];
            }

            return tag;
        }

        /// <inheritdoc />
        public IEnumerable<ITemplateTag> GetTags() {
            IEnumerable<ITemplateTag> tags = Enumerable.Empty<ITemplateTag>();
            foreach(ITemplateTagFactory factory in this.m_factories) {
                tags = tags.Concat(factory.GetTags());
            }

            lock(((ICollection) this.m_tagCache).SyncRoot) {
                this.m_isAddingTagCache = 1;
                foreach(ITemplateTag tag in tags) {
                    this.m_tagCache[tag.TagName] = tag;
                }
                this.m_isAddingTagCache = 0;
            }

            return tags;
        }

        #endregion
    }

    /// <summary>
    /// Provides all functions of template engine.
    /// </summary>
    public static class TemplateEngine {
        /// <summary>
        /// Initialize TemplateEngine class.
        /// </summary>
        static TemplateEngine() {
        }

        /// <summary>
        /// Gets function interface of template system.
        /// </summary>
        public static TemplateSystem TemplateSystem {
            get {
                return TemplateSystem.Instance;
            }
        }

        /// <summary>
        /// Gets function interface of tag system.
        /// </summary>
        public static TagSystem TagSystem {
            get {
                return TagSystem.Instance;
            }
        }
    }

    /// <summary>
    /// Defines extension methods of IXtmlDocument.
    /// </summary>
    public static class XtmlExtension {
        /// <summary>
        /// Gets all elements from a XTML document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static IEnumerable<IXtmlElement> GetElements(this IXtmlDocument document) {
            IXtmlNode current = null;
            Queue<IXtmlNode> queue = new Queue<IXtmlNode>(new IXtmlNode[] { document });
            while(queue.Count > 0) {
                if((current = queue.Dequeue()).ChildNodes == null) {
                    continue;
                }

                foreach(IXtmlNode node in current.ChildNodes) {
                    if(node.NodeType == XtmlNodeType.Element) {
                        yield return (IXtmlElement) node;
                    }

                    queue.Enqueue(node);
                }
            }

            yield break;
        }

        /// <summary>
        /// Gets all elements by the specified local name from a XTML document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static IEnumerable<IXtmlElement> GetElementsByLocalName(this IXtmlDocument document, string localName) {
            return GetElements(document).Where((item) => string.Equals(item.LocalName, localName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all elements by the specified qualified name from a XTML document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="qualifieName"></param>
        /// <returns></returns>
        public static IEnumerable<IXtmlElement> GetElementsByQualifiedName(this IXtmlDocument document, string qualifieName) {
            return GetElements(document).Where((item) => string.Equals(item.QualifiedName, qualifieName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets child elements by the specified local name from a XTML element.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static IEnumerable<IXtmlElement> GetChildElementsByLocalName(this IXtmlElement element, string localName) {
            return from item in element.ChildNodes
                   where item.NodeType == XtmlNodeType.Element && string.Equals(item.LocalName, localName, StringComparison.OrdinalIgnoreCase)
                   select (IXtmlElement) item;
        }

        /// <summary>
        /// Gets all attributes by the specified name from a XTML element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<IXtmlAttribute> GetAttributesByName(this IXtmlElement element, string name) {
            return element.Attributes.Where((item) => string.Equals(item.LocalName, name, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Represents a XtmlElementIterationRenderer object which renders all values of a property of the data source.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    [Obsolete]
    public class DataSourceMultipartPropertyXtmlElementRenderer<TSource> : XtmlElementIterationRenderer<IDataSourceMultipartPropertyValuesItem<TSource>> {
        public DataSourceMultipartPropertyXtmlElementRenderer()
            : base() {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(string propertyName, Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>> propertyValuesSelector)
            : this(propertyName, null, propertyValuesSelector) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(string propertyName, IDataSourceMultipartPropertyValuesProvider<TSource> propertyValuesProvider)
            : this(propertyName, null, propertyValuesProvider != null ? propertyValuesProvider.GetPropertyValues : (Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>>) null) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(string propertyName, IDataSourceProvider<TSource> sourceProvider, IDataSourceMultipartPropertyValuesProvider<TSource> propertyValuesProvider)
            : this(propertyName, sourceProvider != null ? sourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, propertyValuesProvider != null ? propertyValuesProvider.GetPropertyValues : (Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>>) null) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(string propertyName, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> sourceSelector, Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>> propertyValuesSelector)
            : this((element, dataSource, context) => propertyName, sourceSelector, propertyValuesSelector) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, string> propertyNameSelector, Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>> propertyValuesSelector)
            : this(propertyNameSelector, null, propertyValuesSelector) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, string> propertyNameSelector, IDataSourceMultipartPropertyValuesProvider<TSource> propertyValuesProvider)
            : this(propertyNameSelector, null, propertyValuesProvider != null ? propertyValuesProvider.GetPropertyValues : (Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>>) null) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, string> propertyNameSelector, IDataSourceProvider<TSource> sourceProvider, IDataSourceMultipartPropertyValuesProvider<TSource> propertyValuesProvider)
            : this(propertyNameSelector, sourceProvider != null ? sourceProvider.GetDataSource : (Func<IXtmlElement, object, IXtmlRenderingContext, TSource>) null, propertyValuesProvider != null ? propertyValuesProvider.GetPropertyValues : (Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>>) null) {
        }

        public DataSourceMultipartPropertyXtmlElementRenderer(Func<IXtmlElement, object, IXtmlRenderingContext, string> propertyNameSelector, Func<IXtmlElement, object, IXtmlRenderingContext, TSource> sourceSelector, Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>> propertyValuesSelector) {
            if(propertyNameSelector == null) {
                throw new ArgumentException("propertyNameSelector is null.", "propertyNameSelector");
            }

            this.m_propertyNameSelector = propertyNameSelector;
            this.m_sourceSelector = sourceSelector;
            this.m_propertyValuesSelector = propertyValuesSelector;

            this.m_dataSourceSelector = (element, dataSource, context) => {
                string propertyName = this.m_propertyNameSelector(element, dataSource, context);
                if(string.IsNullOrWhiteSpace(propertyName)) {
                    throw new XtmlException("Property name is empty.");
                }

                IEnumerable<object> values = null;
                TSource source = this.GetSource(element, dataSource, context);
                if(source != null && (values = this.GetPropertyValues(source, context, propertyName, this.GetPropertyValue(element, dataSource, source, propertyName))) != null) {
                    return values.Select((item) => new DataSourceMultipartPropertyValuesItem<TSource>(source, propertyName, item));
                } else {
                    return Enumerable.Empty<IDataSourceMultipartPropertyValuesItem<TSource>>();
                }
            };
        }

        /// <summary>
        /// The property dictionary.
        /// </summary>
        protected IDictionary<string, PropertyInfo> m_propertiesCache = new Dictionary<string, PropertyInfo>();
        protected volatile int m_isAddPropertyCache;

        /// <summary>
        /// The delegate to get property name.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, string> m_propertyNameSelector;

        /// <summary>
        /// The delegate to get data source.
        /// </summary>
        protected Func<IXtmlElement, object, IXtmlRenderingContext, TSource> m_sourceSelector;

        /// <summary>
        /// The delegate to get property values.
        /// </summary>
        protected Func<TSource, IXtmlRenderingContext, string, object, IEnumerable<object>> m_propertyValuesSelector;

        /// <summary>
        /// Gets the actual data source.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected virtual TSource GetSource(IXtmlElement element, object dataSource, IXtmlRenderingContext context) {
            return this.m_sourceSelector != null ? this.m_sourceSelector(element, dataSource, context) : (TSource) dataSource;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected virtual object GetPropertyValue(IXtmlElement element, object dataSource, TSource source, string propertyName) {
            Type type = typeof(TSource);
            PropertyInfo property = null;

            if(!this.m_propertiesCache.ContainsKey(propertyName)) {
                lock(((ICollection) this.m_propertiesCache).SyncRoot) {
                    if(!this.m_propertiesCache.ContainsKey(propertyName)) {
                        property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if(property == null || property.GetIndexParameters().Length > 0) {
                            throw new XtmlException(string.Format("{0} do not contains a property named {1}.", type.Name, propertyName));
                        }

                        this.m_isAddPropertyCache = 1;
                        this.m_propertiesCache[propertyName] = property;
                        this.m_isAddPropertyCache = 0;
                    }
                }
            }

            if(property == null) {
                SpinWait.SpinUntil(() => this.m_isAddPropertyCache == 0);

                property = this.m_propertiesCache[propertyName];
            }

            return property.GetValue(source, null);
        }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual IEnumerable<object> GetPropertyValues(TSource source, IXtmlRenderingContext context, string propertyName, object value) {
            return this.m_propertyValuesSelector != null ? this.m_propertyValuesSelector(source, context, propertyName, value) : (IEnumerable<object>) value;
        }
    }

    [Obsolete]
    public class DataSourceMultipartPropertyXtmlElementRenderer<TSource, TValue> : DataSourceMultipartPropertyXtmlElementRenderer<TSource> {
    }

    /// <summary>
    /// Represents a item in data source of DataSourceMultipartPropertyXtmlElementRenderer class.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    [Obsolete]
    public interface IDataSourceMultipartPropertyValuesItem<out TSource> {
        /// <summary>
        /// Gets the data source.
        /// </summary>
        TSource DataSource {
            get;
        }

        /// <summary>
        /// Gets property name.
        /// </summary>
        string PropertyName {
            get;
        }

        /// <summary>
        /// Gets current value which is a member of property values.
        /// </summary>
        object PropertyValue {
            get;
        }
    }

    /// <summary>
    /// Provides a default implementation of IDataSourceMultipartPropertyValuesItem interface.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    [Obsolete]
    public class DataSourceMultipartPropertyValuesItem<TSource> : IDataSourceMultipartPropertyValuesItem<TSource> {
        /// <summary>
        /// Initialize a instance of DataSourceMultipartPropertyValueItem class.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public DataSourceMultipartPropertyValuesItem(TSource dataSource, string propertyName, object propertyValue) {
            this.DataSource = dataSource;
            this.PropertyName = propertyName;
            this.PropertyValue = propertyValue;
        }

        /// <inheritdoc />
        public TSource DataSource {
            get;
            private set;
        }

        /// <inheritdoc />
        public string PropertyName {
            get;
            private set;
        }

        /// <inheritdoc />
        public object PropertyValue {
            get;
            private set;
        }
    }

    /// <summary>
    /// Provides values of a multipart property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete]
    public interface IDataSourceMultipartPropertyValuesProvider<T> {
        /// <summary>
        /// Gets all values of the specified property and value.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<object> GetPropertyValues(T source, IXtmlRenderingContext context, string propertyName, object value);
    }

    /// <summary>
    /// Represents a template tag which renders the aggregate value of the data source.
    /// </summary>
    public abstract class XtmlElementAggregationTag : XtmlElementTag<IEnumerable<object>> {
        /// <summary>
        /// Initialize a new instance of XtmlElementAggregationTag class.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="formater"></param>
        public XtmlElementAggregationTag()
            : this(null, (Func<IXtmlElement, IEnumerable<object>, IXtmlRenderingContext, object, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementAggregationTag class.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="formater"></param>
        public XtmlElementAggregationTag(IEnumerable<IXtmlAggregateFunction> functions, string format)
            : this(functions, format != null ? (element, dataSource, context, value) => string.Format(string.Format("{{0:{0}}}", format), value) : (Func<IXtmlElement, IEnumerable<object>, IXtmlRenderingContext, object, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementAggregationTag class.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="formater"></param>
        public XtmlElementAggregationTag(IEnumerable<IXtmlAggregateFunction> functions, IXtmlAggregateValueFormater formater)
            : this(functions, formater != null ? formater.Format : (Func<IXtmlElement, IEnumerable<object>, IXtmlRenderingContext, object, object>) null) {
        }

        /// <summary>
        /// Initialize a new instance of XtmlElementAggregationTag class.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="formater"></param>
        public XtmlElementAggregationTag(IEnumerable<IXtmlAggregateFunction> functions, Func<IXtmlElement, IEnumerable<object>, IXtmlRenderingContext, object, object> formater) {
            this.m_propertiesCache = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
            if(functions != null) {
                this.m_functions = functions.ToDictionary((item) => item.Name);
            } else {
                this.m_functions = new Dictionary<string, IXtmlAggregateFunction>();
            }
            this.m_formater = formater ?? ((element, dataSource, context, value) => {
                string format = null;
                IXtmlAttribute attribute = element.GetAttributesByName(FORMAT_ATTRIBUTE_NAME).FirstOrDefault();
                if(attribute != null) {
                    format = attribute.GetValue(dataSource, context);
                }

                if(format != null) {
                    return string.Format(string.Format("{{0:{0}}}", format), value);
                } else {
                    return value;
                }
            });
        }

        public const string PROPERTY_ATTRIBUTE_NAME = "Property";
        public const string FUNCTION_ATTRIBUTE_NAME = "Function";
        public const string FORMAT_ATTRIBUTE_NAME = "Format";

        protected volatile int m_isAddingPropertyCache;
        protected IDictionary<Type, IDictionary<string, PropertyInfo>> m_propertiesCache;
        protected IDictionary<string, IXtmlAggregateFunction> m_functions;
        protected Func<IXtmlElement, IEnumerable<object>, IXtmlRenderingContext, object, object> m_formater;

        /// <summary>
        /// Gets property name.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual string GetPropertyName(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context) {
            string propertyName = null;
            IXtmlAttribute attribute = element.GetAttributesByName(PROPERTY_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute != null) {
                propertyName = attribute.GetValue(dataSource, context);
            }
            return propertyName;
        }

        /// <summary>
        /// Gets property values collection and property type.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        protected virtual IEnumerable<object> GetPropertyValues(IEnumerable<object> dataSource, string propertyName, out PropertyInfo propertyInfo) {
            PropertyInfo property = null;
            Type type = TypeUtility.GetEnumerableElementType(dataSource);
            if(type == null) {
                throw new XtmlException(string.Format("Unable to determine the element type of {0}.", dataSource.GetType().FullName));
            }

            if(!this.m_propertiesCache.ContainsKey(type) || !this.m_propertiesCache[type].ContainsKey(propertyName)) {
                lock(((ICollection) this.m_propertiesCache).SyncRoot) {
                    if(!this.m_propertiesCache.ContainsKey(type) || !this.m_propertiesCache[type].ContainsKey(propertyName)) {
                        property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if(property == null || property.GetIndexParameters().Length > 0) {
                            throw new XtmlException(string.Format("{0} do not contains a property named {1}.", type.Name, propertyName));
                        }

                        this.m_isAddingPropertyCache = 1;
                        if(!this.m_propertiesCache.ContainsKey(type)) {
                            this.m_propertiesCache[type] = new Dictionary<string, PropertyInfo>();
                        }
                        this.m_propertiesCache[type][propertyName] = property;
                        this.m_isAddingPropertyCache = 0;
                    }
                }
            }

            if(property == null) {
                SpinWait.SpinUntil(() => this.m_isAddingPropertyCache == 0);

                property = this.m_propertiesCache[type][propertyName];
            }
            propertyInfo = property;

            /*
             * Must be use a object collection: (new List<int> is IEnumerable<object>) is false.
             */
            ICollection<object> values = new List<object>();
            foreach(object item in dataSource) {
                values.Add(property.GetValue(item, null));
            }
            return values;
        }

        /// <summary>
        /// Gets the aggregate function.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected IXtmlAggregateFunction GetAggregateFunction(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context) {
            string functionName = null;
            IXtmlAttribute attribute = element.GetAttributesByName(FUNCTION_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute == null) {
                throw new XtmlException(string.Format("The {0} tag is not specified {1} attribute.", this.TagName, FUNCTION_ATTRIBUTE_NAME));
            }
            if(string.IsNullOrWhiteSpace(functionName = attribute.GetValue(dataSource, context))) {
                throw new XtmlException(string.Format("The value of {1} attribute in {0} tag is empty.", this.TagName, FUNCTION_ATTRIBUTE_NAME));
            }
            if(!this.m_functions.ContainsKey(functionName)) {
                throw new XtmlException(string.Format("The aggreate function named {0} is not existing.", functionName));
            }

            return this.m_functions[functionName];
        }

        /// <summary>
        /// Formats aggregate value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="aggregateValue"></param>
        /// <returns></returns>
        protected virtual object FormatAggregateValue(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context, object aggregateValue) {
            object formatedValue = aggregateValue;
            if(this.m_formater != null) {
                formatedValue = this.m_formater(element, dataSource, context, aggregateValue);
            }
            return formatedValue;
        }

        /// <inheritdoc />
        protected override void RenderDataSource(IXtmlElement element, TextWriter writer, IEnumerable<object> dataSource, object originalSource, IXtmlRenderingContext context) {
            if(!dataSource.Any()) {
                return;
            }

            string propertyName = null;
            PropertyInfo propertyInfo = null;
            IEnumerable<object> propertyValues = null;
            IXtmlAggregateFunction aggregateFunction = null;
            object aggregateValue = null, formatedValue = null;

            if(!string.IsNullOrWhiteSpace(propertyName = this.GetPropertyName(element, dataSource, context))) {
                propertyValues = this.GetPropertyValues(dataSource, propertyName, out propertyInfo);
            }
            aggregateFunction = this.GetAggregateFunction(element, dataSource, context);
            aggregateValue = aggregateFunction.Compute(element, dataSource, context, propertyInfo, propertyValues);
            formatedValue = this.FormatAggregateValue(element, dataSource, context, aggregateValue);

            if(formatedValue != null) {
                writer.Write(formatedValue);
            }
        }
    }

    /// <summary>
    /// Represents a aggregate function used by XtmlElementAggregationTag class.
    /// </summary>
    public interface IXtmlAggregateFunction {
        /// <summary>
        /// Gets the unique function name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets the function description.
        /// </summary>
        string Description {
            get;
        }

        /// <summary>
        /// Computes the aggregate value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <param name="propertyValues"></param>
        /// <returns></returns>
        object Compute(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context, PropertyInfo propertyInfo, IEnumerable<object> propertyValues);
    }

    /// <summary>
    /// Formats the aggregate value for XtmlElementAggregationTag class.
    /// </summary>
    public interface IXtmlAggregateValueFormater {
        /// <summary>
        /// Formats the specified aggregate value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dataSource"></param>
        /// <param name="context"></param>
        /// <param name="aggregateValue"></param>
        /// <returns></returns>
        object Format(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context, object aggregateValue);
    }

    /// <summary>
    /// Provides the base class which aggregate numerical property values by extended methods defined in Enumerable class.
    /// </summary>
    public abstract class XtmlNumericalAggregateFunction : IXtmlAggregateFunction {
        private readonly string CAST_METHOD_NAME = TypeUtility.GetMemberName<IEnumerable<object>, IEnumerable<object>>((obj) => obj.Cast<object>());
        private readonly string TO_ARRAY_METHOD_NAME = TypeUtility.GetMemberName<IEnumerable<object>, object[]>((obj) => obj.ToArray());
        private readonly string ELEMENT_AT_METHOD_NAME = TypeUtility.GetMemberName<IEnumerable<object>, object>((obj) => obj.ElementAt(0));
        private readonly string ADD_METHOD_NAME = TypeUtility.GetMemberName<ICollection<object>>((obj) => obj.Add(null));

        /// <summary>
        /// Gets the aggregate method name in Enumerable class.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetAggregateMethodName();

        #region IXtmlAggregateFunction Members

        /// <inheritdoc />
        public abstract string Name {
            get;
        }

        /// <inheritdoc />
        public abstract string Description {
            get;
        }

        /// <inheritdoc />
        public virtual object Compute(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context, PropertyInfo propertyInfo, IEnumerable<object> propertyValues) {
            if(propertyValues == null) {
                return null;
            }

            Type elementType = propertyInfo.PropertyType;
            if(elementType.IsNullable()) {
                elementType = elementType.GetGenericArguments()[0];
                propertyValues = propertyValues.Where((item) => item != null);
            }
            if(!elementType.IsNumber()) {
                throw new XtmlException(string.Format("{0} is not a numerical type.", elementType.FullName));
            }

            object castValues = typeof(Enumerable).GetMethod(CAST_METHOD_NAME).MakeGenericMethod(elementType).Invoke(null, new object[] { propertyValues });
            castValues = typeof(Enumerable).GetMethod(TO_ARRAY_METHOD_NAME).MakeGenericMethod(elementType).Invoke(null, new object[] { castValues });

            Type castType = null;
            if(elementType.IsInteger() && !elementType.IsInt32() && !elementType.IsInt64()) {
                castType = typeof(int);
            } else {
                castType = elementType;
            }
            object aggregateValues = Activator.CreateInstance(typeof(List<>).MakeGenericType(castType));

            MethodInfo addMethod = aggregateValues.GetType().GetMethod(ADD_METHOD_NAME);
            MethodInfo elementAtMethod = typeof(Enumerable).GetMethod(ELEMENT_AT_METHOD_NAME).MakeGenericMethod(elementType);
            int count = (int) castValues.GetType().GetProperty("Length").GetValue(castValues, null);
            for(int i = 0; i < count; i++) {
                addMethod.Invoke(aggregateValues, new object[] { elementAtMethod.Invoke(null, new object[] { castValues, i }) });
            }

            object result = typeof(Enumerable).GetMethod(this.GetAggregateMethodName(), new Type[] { typeof(IEnumerable<>).MakeGenericType(castType) }).Invoke(null, new object[] { aggregateValues });
            return result;
        }

        #endregion
    }

    /// <summary>
    /// Aggregate method: Enumerable.Sum.
    /// </summary>
    public class XtmlSumAggregateFunction : XtmlNumericalAggregateFunction {
        /// <inheritdoc />
        protected override string GetAggregateMethodName() {
            return TypeUtility.GetMemberName<IEnumerable<int>, int>((obj) => obj.Sum());
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "sum";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return "计算指定属性的值之和";
            }
        }
    }

    /// <summary>
    /// Aggregate method: Enumerable.Average.
    /// </summary>
    public class XtmlAverageAggregateFunction : XtmlNumericalAggregateFunction {
        /// <inheritdoc />
        protected override string GetAggregateMethodName() {
            return TypeUtility.GetMemberName<IEnumerable<int>, double>((obj) => obj.Average());
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "avg";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return "计算指定属性的值的平均值";
            }
        }
    }

    /// <summary>
    /// Aggregate method: Enumerable.Max.
    /// </summary>
    public class XtmlMaxAggregateFunction : XtmlNumericalAggregateFunction {
        /// <inheritdoc />
        protected override string GetAggregateMethodName() {
            return TypeUtility.GetMemberName<IEnumerable<int>, int>((obj) => obj.Max());
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "max";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return "计算指定属性的值的最大值";
            }
        }
    }

    /// <summary>
    /// Aggregate method: Enumerable.Min.
    /// </summary>
    public class XtmlMinAggregateFunction : XtmlNumericalAggregateFunction {
        /// <inheritdoc />
        protected override string GetAggregateMethodName() {
            return TypeUtility.GetMemberName<IEnumerable<int>, int>((obj) => obj.Min());
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "min";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return "计算指定属性的值的最小值";
            }
        }
    }

    /// <summary>
    /// Computes the elements number in data source.
    /// </summary>
    public class XtmlCountAggregateFunction : IXtmlAggregateFunction {
        private const string DISTINCT_ATTRIBUTE_NAME = "Distinct";

        #region IXtmlAggregateFunction Members

        /// <inheritdoc />
        public string Name {
            get {
                return "count";
            }
        }

        /// <inheritdoc />
        public string Description {
            get {
                return string.Format("计算数据源或指定属性中包含的项的数量，{0}属性为true表示只计算不重复的项", DISTINCT_ATTRIBUTE_NAME);
            }
        }

        /// <inheritdoc />
        public object Compute(IXtmlElement element, IEnumerable<object> dataSource, IXtmlRenderingContext context, PropertyInfo propertyInfo, IEnumerable<object> propertyValues) {
            int count = 0;
            bool distinct = false;
            IXtmlAttribute attribute = element.GetAttributesByName(DISTINCT_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute != null) {
                bool.TryParse(attribute.GetValue(dataSource, context), out distinct);
            }

            if(propertyInfo != null && Attribute.GetCustomAttributes(propertyInfo, typeof(MultipleAttribute), true).Length > 0 && propertyInfo.PropertyType.IsEnumerable()) {
                if(distinct) {
                    object current = null;
                    IEnumerator enumerator = null;
                    ICollection<object> data = new HashSet<object>();

                    foreach(IEnumerable item in propertyValues) {
                        try {
                            enumerator = item.GetEnumerator();
                            while(enumerator.MoveNext()) {
                                if(!data.Contains(current = enumerator.Current)) {
                                    data.Add(current);
                                }
                            }
                        } finally {
                            enumerator.Reset();
                        }
                    }

                    count = data.Count;
                } else {
                    foreach(IEnumerable item in propertyValues) {
                        count += item.Number();
                    }
                }
            } else {
                if(distinct) {
                    count = dataSource.Distinct().Count();
                } else {
                    count = dataSource.Count();
                }
            }

            return count;
        }

        #endregion
    }

    /// <summary>
    /// Provides function to find appropriate data source from current data source.
    /// </summary>
    internal class DataSourceFinder {
        #region Singleton

        /// <summary>
        /// Initialize a new instance of DataSourceFinder class.
        /// </summary>
        protected DataSourceFinder() {
            this.m_graph = new DirectedGraph<Type, PropertyInfo>();
            this.m_pathsCache = new Dictionary<CacheKey, IEnumerable<IEnumerable<DirectedGraphEdge<Type, PropertyInfo>>>>();
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new DataSourceFinder();
            }

            public static DataSourceFinder Instance;
        }

        /// <summary>
        /// Gets the unique instance of DataSourceFinder class.
        /// </summary>
        public static DataSourceFinder Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        private DirectedGraph<Type, PropertyInfo> m_graph;

        private volatile int m_isAddingPathCache;
        private IDictionary<CacheKey, IEnumerable<IEnumerable<DirectedGraphEdge<Type, PropertyInfo>>>> m_pathsCache;

        private bool IsDataSource(Type type) {
            return type.GetCustomAttributes(typeof(DataSourceAttribute), true).Length > 0;
        }

        private void CreateGraph(Type type) {
            if(type.BaseType != null && this.IsDataSource(type.BaseType)) {
                if(!this.m_graph.ContainsVertex(type.BaseType)) {
                    this.CreateGraph(type.BaseType);
                }
                this.m_graph.AddEdge(type, type.BaseType, null);
            }

            foreach(PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where((item) => item.GetIndexParameters().Length == 0)) {
                if(!this.IsDataSource(property.PropertyType) || property.PropertyType.Equals(type)) {
                    continue;
                }

                if(!type.IsAssignableFrom(property.PropertyType) && !this.m_graph.ContainsVertex(property.PropertyType)) {
                    this.CreateGraph(property.PropertyType);
                }
                if(property.PropertyType.Equals(type.BaseType)) {
                    this.m_graph.RemoveEdge(type, type.BaseType);
                }
                this.m_graph.AddEdge(type, property.PropertyType, property);
            }
        }

        /// <summary>
        /// Finds data source from <paramref name="dataSource"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public TSource Find<TSource>(object dataSource) {
            if(dataSource == null) {
                throw new ArgumentNullException("dataSource");
            }

            CacheKey key = new CacheKey {
                From = dataSource.GetType(),
                To = typeof(TSource),
            };
            IEnumerable<IEnumerable<DirectedGraphEdge<Type, PropertyInfo>>> paths = null;

            if(!this.m_pathsCache.ContainsKey(key)) {
                lock(((ICollection) this.m_pathsCache).SyncRoot) {
                    if(!this.m_pathsCache.ContainsKey(key)) {
                        if(!this.m_graph.ContainsVertex(key.From)) {
                            this.CreateGraph(key.From);
                            this.m_pathsCache.Clear();
                        }

                        this.m_isAddingPathCache = 1;
                        if(!this.m_graph.ContainsVertex(key.From)) {
                            this.m_pathsCache[key] = paths = Enumerable.Empty<IEnumerable<DirectedGraphEdge<Type, PropertyInfo>>>();
                        } else if(key.To.IsInterface) {
                            this.m_pathsCache[key] = paths = this.m_graph.FindOutPaths(key.From, (item) => key.To.IsAssignableFrom(item));
                        } else {
                            this.m_pathsCache[key] = paths = this.m_graph.FindOutPaths(key.From, key.To);
                        }
                        this.m_isAddingPathCache = 0;
                    }
                }
            }

            if(paths == null) {
                SpinWait.SpinUntil(() => this.m_isAddingPathCache == 0);

                paths = this.m_pathsCache[key];
            }

            object source = null;
            foreach(IEnumerable<DirectedGraphEdge<Type, PropertyInfo>> path in paths) {
                source = dataSource;

                foreach(DirectedGraphEdge<Type, PropertyInfo> edge in path) {
                    if(edge.Data != null) {
                        source = edge.Data.GetValue(source, null);
                    }

                    if(source == null) {
                        break;
                    }
                }

                if(source != null) {
                    break;
                }
            }

            return (TSource) source;
        }

        #region CacheKey

        private class CacheKey {
            public Type From;
            public Type To;

            public override int GetHashCode() {
                return this.ToString().GetHashCode();
            }

            public override bool Equals(object obj) {
                if(obj == null) {
                    return false;
                }
                if(object.ReferenceEquals(this, obj)) {
                    return true;
                }
                if(!(obj is CacheKey)) {
                    return false;
                }

                CacheKey key = (CacheKey) obj;
                return this.From.Equals(key.From) && this.To.Equals(key.To);
            }

            public override string ToString() {
                return string.Format("{0} & {1}", this.From.AssemblyQualifiedName, this.To.AssemblyQualifiedName);
            }
        }

        #endregion
    }

    /// <summary>
    /// Specifies the target can be a datasource in XTML.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public sealed class DataSourceAttribute : Attribute {
    }

    /// <summary>
    /// Provides a tag to show the string representation of current data source
    /// </summary>
    public abstract class XtmlElementStringTag : XtmlNodeTag<object> {
        /// <inheritdoc />
        public override string Name {
            get {
                return "字符串";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return "显示当前数据源的字符串表现形式";
            }
        }

        /// <inheritdoc />
        public override string GetShorthand(XtmlMarkupContext context) {
            return context.GetCloselfTag(this.TagName);
        }

        /// <inheritdoc />
        public override string GetPattern(XtmlMarkupContext context) {
            return context.GetCloselfTag(this.TagName);
        }
    }

    /// <summary>
    /// Provides a tag to show the ordinal of item in the data source.
    /// </summary>
    public abstract class XtmlElementItemOrdinalTag : XtmlElementAttachedObjectTag<int> {
        public const string START_ATTRIBUTE_NAME = "Start";
        public const string FORMAT_ATTRIBUTE_NAME = "Format";

        /// <inheritdoc />
        protected override void RenderDataSource(IXtmlElement element, TextWriter writer, int dataSource, object originalSource, IXtmlRenderingContext context) {
            int start = 0;
            IXtmlAttribute attribute = element.GetAttributesByName(START_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute != null) {
                int.TryParse(attribute.GetValue(originalSource, context), out start);
            }

            string format = null;
            attribute = element.GetAttributesByName(FORMAT_ATTRIBUTE_NAME).FirstOrDefault();
            if(attribute != null && !string.IsNullOrEmpty(format = attribute.GetValue(originalSource, context))) {
                writer.Write(string.Format(string.Format("{{0:{0}}}", format), dataSource + start));
            } else {
                writer.Write(dataSource + start);
            }
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "数据项序号";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return string.Format("显示数据源中数据项的序号\r\n\r\n可选参数{0}",
                    new string[] {
                        string.Format("\r\n{0}：起始序号，默认为0", START_ATTRIBUTE_NAME),
                        string.Format("\r\n{0}：格式化字符串", FORMAT_ATTRIBUTE_NAME),
                    }.StringJoin(string.Empty));
            }
        }

        /// <inheritdoc />
        public override string GetShorthand(XtmlMarkupContext context) {
            return context.GetOpenTag(this.TagName);
        }

        /// <inheritdoc />
        public override string GetPattern(XtmlMarkupContext context) {
            return context.GetCloselfTag(this.TagName);
        }
    }

    /// <summary>
    /// Provides a tag to execute conditional.
    /// </summary>
    public abstract class ConditionalTemplateTag : TemplateTag {
        public const string TRUE_ATTRIBUTE_NAME = "True";
        public const string FALSE_ATTRIBUTE_NAME = "False";
        public const string EXPRESSION_ATTRIBUTE_NAME = "Expression";

        /// <summary>
        /// Gets the boolean value of the specified attribute value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetBooleanValue(string value) {
            if(string.IsNullOrWhiteSpace(value)) {
                return false;
            }

            decimal number = 0;
            if(decimal.TryParse(value, out number)) {
                return number != 0;
            }

            bool trueOrFalse = false;
            if(bool.TryParse(value, out trueOrFalse)) {
                return trueOrFalse;
            }

            return true;
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "条件判断标签";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return string.Format(
                    "根据{0}、{1}、{2}属性的值来决定是否渲染包含的子标签，{0}、{1}和{2}属性只允许同时出现一个。\r\n\r\n可选标签：\r\n{0}：属性值解析为真时，渲染子标签\r\n{1}：属性值解析为假时，渲染子标签\r\n{2}：属性值作为运算表达式进行计算，值解析为真时，渲染子标签\r\n\r\n目前按照以下规则顺序解析属性值的真假性：\r\n{3}\r\n\r\n运算表达式中支持以下运算数：\r\n{4}\r\n\r\n运算表达式中支持以下运算符：\r\n{5}",
                    TRUE_ATTRIBUTE_NAME,
                    FALSE_ATTRIBUTE_NAME,
                    EXPRESSION_ATTRIBUTE_NAME,
                    new string[] {
                        "空白字符串：假",
                        "数字0：假",
                        "非数字0：真",
                        "字符串true：真",
                        "字符串false：假",
                        "非空白字符串：真",
                    }.StringJoin("\r\n"),
                    new string[] {
                        "数字",
                        "true或false",
                        string.Format("用字符 {0} 和 {1} 改变计算顺序", OperationExpressionCalculator.LEFT_BRACKET, OperationExpressionCalculator.RIGHT_BRACKET),
                        string.Format("前后用字符 {0} 界定的字符串", OperationExpressionCalculator.STRING_DELIMITER),
                    }.StringJoin("\r\n"),
                    MathUtility.RegisteredOperationRules.Select((item) => string.Format("{0}：{1}", item.OperatorSymbol, item.Name)).StringJoin("\r\n"));
            }
        }

        /// <inheritdoc />
        public override string GetShorthand(XtmlMarkupContext context) {
            return context.GetOpenTag(this.TagName);
        }

        /// <inheritdoc />
        public override string GetPattern(XtmlMarkupContext context) {
            return context.GetOpenTag(this.TagName, TRUE_ATTRIBUTE_NAME) + "\r\n" + context.GetCloseTag(this.TagName);
        }

        /// <inheritdoc />
        public override void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            IXtmlElement element = null;
            IXtmlAttribute trueAttribute = null, falseAttribute = null, expressionAttribute = null;

            element = (IXtmlElement) node;
            trueAttribute = element.GetAttributesByName(TRUE_ATTRIBUTE_NAME).FirstOrDefault();
            falseAttribute = element.GetAttributesByName(FALSE_ATTRIBUTE_NAME).FirstOrDefault();
            expressionAttribute = element.GetAttributesByName(EXPRESSION_ATTRIBUTE_NAME).FirstOrDefault();
            switch((trueAttribute != null ? 1 : 0) + (falseAttribute != null ? 1 : 0) + (expressionAttribute != null ? 1 : 0)) {
                case 0:
                    throw new XtmlException(string.Format("标签未提供 {0} 属性 或 {1} 属性 或 {2} 属性 中的任何一个", TRUE_ATTRIBUTE_NAME, FALSE_ATTRIBUTE_NAME, EXPRESSION_ATTRIBUTE_NAME));
                case 1:
                    break;
                default:
                    throw new XtmlException(string.Format("标签同时提供了 {0} 属性、{1} 属性 和 {2} 属性", TRUE_ATTRIBUTE_NAME, FALSE_ATTRIBUTE_NAME, EXPRESSION_ATTRIBUTE_NAME));
            }

            string attributeValue = (trueAttribute ?? falseAttribute ?? expressionAttribute).GetValue(dataSource, context);
            if(expressionAttribute != null) {
                attributeValue = MathUtility.ComputeExpression(attributeValue).ToString();
            }

            if((trueAttribute != null || expressionAttribute != null) && this.GetBooleanValue(attributeValue) ||
                falseAttribute != null && !this.GetBooleanValue(attributeValue)) {
                foreach(IXtmlNode child in element.ChildNodes) {
                    child.Render(writer, dataSource, context);
                }
            }
        }
    }

    /// <summary>
    /// Provides a tag to show the length of input data.
    /// </summary>
    public abstract class LengthTemplateTag : TemplateTag {
        public const string INPUT_ATTRIBUTE_NAME = "Input";

        /// <inheritdoc />
        public override string Name {
            get {
                return "长度标签";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return string.Format("渲染 {0} 属性值的长度", INPUT_ATTRIBUTE_NAME);
            }
        }

        /// <inheritdoc />
        public override string GetShorthand(XtmlMarkupContext context) {
            return context.GetOpenTag(this.TagName);
        }

        /// <inheritdoc />
        public override string GetPattern(XtmlMarkupContext context) {
            return context.GetCloselfTag(this.TagName, INPUT_ATTRIBUTE_NAME);
        }

        /// <inheritdoc />
        public override void Render(IXtmlNode node, TextWriter writer, object dataSource, IXtmlRenderingContext context) {
            IXtmlElement element = (IXtmlElement) node;
            IXtmlAttribute inputAttribute = element.GetAttributesByName(INPUT_ATTRIBUTE_NAME).FirstOrDefault();
            if(inputAttribute != null) {
                string value = inputAttribute.GetValue(dataSource, context);
                if(value != null) {
                    writer.Write(value.Length);
                } else {
                    writer.Write(0);
                }
            }
        }
    }

    /// <summary>
    /// Provides a tag to list enumerable data source.
    /// </summary>
    public abstract class XtmlElementListTag : XtmlElementIterationTag<object> {
        /// <inheritdoc />
        public override string Name {
            get {
                return "数据列表";
            }
        }

        /// <inheritdoc />
        public override string Description {
            get {
                return string.Format("显示{0}，数据源必须是可枚举的数据列表\r\n\r\n{1}",
                    this.Name,
                    base.Description);
            }
        }

        /// <inheritdoc />
        public override string GetShorthand(XtmlMarkupContext context) {
            return context.GetOpenTag(this.TagName);
        }

        /// <inheritdoc />
        public override string GetPattern(XtmlMarkupContext context) {
            return context.GetOpenTag(this.TagName) + "\r\n" + context.GetCloseTag(this.TagName);
        }
    }
}
