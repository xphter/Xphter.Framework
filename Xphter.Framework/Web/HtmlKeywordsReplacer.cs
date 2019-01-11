using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Collections;
using Xphter.Framework.Text;

namespace Xphter.Framework.Web {
    /// <summary>
    /// A tool used to replace keywords in a HTML snippet.
    /// </summary>
    public class HtmlKeywordsReplacer {
        /// <summary>
        /// Initialize a new instance of HtmlKeywordsReplacer class.
        /// </summary>
        /// <param name="providers">Keywords providers.</param>
        /// <exception cref="System.ArgumentException"><paramref name="providers"/> is null or empty.</exception>
        public HtmlKeywordsReplacer(params IHtmlKeywordReplacementProvider[] providers) {
            if(providers == null || providers.Length == 0) {
                throw new ArgumentException("Keywords providers is null or empty.", "providers");
            }

            StringBuilder pattern = new StringBuilder();
            foreach(IHtmlKeywordReplacementProvider provider in providers) {
                if(provider == null) {
                    continue;
                }

                foreach(HtmlKeywordReplacementInfo info in provider.Keywords) {
                    pattern.AppendFormat("|{0}", RegexUtility.Encode(info.Keyword));
                    this.m_keywords[info.Keyword.ToLower()] = info;
                }

                this.m_providers.Add(provider);
            }
            if(pattern.Length > 0) {
                this.m_regex = new Regex(pattern.Remove(0, 1).ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// The regular expression used to match keywords.
        /// </summary>
        private Regex m_regex;

        /// <summary>
        /// The keyword providers.
        /// </summary>
        private ICollection<IHtmlKeywordReplacementProvider> m_providers = new List<IHtmlKeywordReplacementProvider>();

        /// <summary>
        /// The keywords dictionary.
        /// </summary>
        private IDictionary<string, HtmlKeywordReplacementInfo> m_keywords = new Dictionary<string, HtmlKeywordReplacementInfo>();

        /// <summary>
        /// Replace the specified text.
        /// </summary>
        /// <param name="text">A string.</param>
        /// <returns>The result string after replacing.</returns>
        public string Replace(string text) {
            if(string.IsNullOrWhiteSpace(text)) {
                return text;
            }
            if(this.m_regex == null) {
                return text;
            }

            //find all tags
            List<HtmlTag> tags = new List<HtmlTag>(HtmlTextAnalyzer.GetAllTags(text));
            tags.Sort();
            tags.Reverse();

            //find keywords      
            Range range = null;
            HtmlTag tag = null;
            int previousIndex = -1;
            HtmlKeywordReplacementInfo previousKeyword = null, keyword = null;
            IDictionary<int, HtmlKeywordReplacementInfo> foundKeywords = new Dictionary<int, HtmlKeywordReplacementInfo>();
            foreach(Match match in this.m_regex.Matches(text)) {
                if(!match.Success || match.Length == 0) {
                    continue;
                }
                range = new Range(match.Index, match.Length);

                //check position
                tag = (from t in tags
                       where t.RangeValue.Contains(range)
                       select t).FirstOrDefault();
                if(tag != null && (tag.OpenRange.Contains(range) || tag.CloseRange.Contains(range))) {
                    continue;
                }

                //check replacement
                keyword = this.m_keywords[match.Value.ToLower()];
                if(!keyword.Provider.Checker.Check(foundKeywords.Count, keyword, match.Index, tag, previousKeyword, previousIndex)) {
                    continue;
                }

                //save replacement
                previousIndex = match.Index;
                previousKeyword = keyword;
                foundKeywords[match.Index] = keyword;
            }

            //replace keywords
            int offset = 0, index = 0;
            StringBuilder result = new StringBuilder(text);
            foreach(KeyValuePair<int, HtmlKeywordReplacementInfo> pair in foundKeywords) {
                index = pair.Key + offset;
                keyword = pair.Value;

                result.Remove(index, keyword.Keyword.Length);
                offset -= keyword.Keyword.Length;

                result.Insert(index, keyword.Replacement);
                offset += keyword.Replacement.Length;
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Represents information of a HTML keyword and it's replacement.
    /// </summary>
    public class HtmlKeywordReplacementInfo : IComparable<HtmlKeywordReplacementInfo> {
        /// <summary>
        /// Initialize a new instance of HtmlKeywordReplacementInfo class.
        /// </summary>
        /// <param name="provider">The keyword provider.</param>
        /// <param name="keyword">Keyword.</param>
        /// <param name="description">Description.</param>
        /// <param name="replacement">Replacement.</param>
        /// <exception cref="System.ArgumentException"><paramref name="keyword"/> is null or empty.</exception>
        public HtmlKeywordReplacementInfo(IHtmlKeywordReplacementProvider provider, string keyword, string description, string replacement) {
            if(string.IsNullOrEmpty(keyword)) {
                throw new ArgumentException("Keyword is null or empty.", "keyword");
            }

            this.Provider = provider;
            this.Keyword = keyword;
            this.Description = description;
            this.Replacement = replacement ?? string.Empty;
        }

        /// <summary>
        /// Gets keywords provider.
        /// </summary>
        public IHtmlKeywordReplacementProvider Provider {
            get;
            private set;
        }

        /// <summary>
        /// Gets keyword.
        /// </summary>
        public string Keyword {
            get;
            private set;
        }

        /// <summary>
        /// Gets description of this keyword.
        /// </summary>
        public string Description {
            get;
            private set;
        }

        /// <summary>
        /// Gets the replacement.
        /// </summary>
        public string Replacement {
            get;
            private set;
        }

        #region IComparable Members

        /// <inheritdoc />
        public int CompareTo(HtmlKeywordReplacementInfo other) {
            if(other == null) {
                return 1;
            }
            if(this.Keyword.Length > other.Keyword.Length) {
                return 1;
            } else if(this.Keyword.Length < other.Keyword.Length) {
                return -1;
            } else {
                return this.Keyword.CompareTo(other.Keyword);
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Keyword.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is HtmlKeywordReplacementInfo)) {
                return false;
            }
            return this.Keyword.Equals(((HtmlKeywordReplacementInfo) obj).Keyword);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Description ?? this.Keyword;
        }

        #endregion
    }

    /// <summary>
    /// Provide keyword information.
    /// </summary>
    public interface IHtmlKeywordReplacementProvider {
        /// <summary>
        /// Gets keywords.
        /// </summary>
        IEnumerable<HtmlKeywordReplacementInfo> Keywords {
            get;
        }

        /// <summary>
        /// Gets the IHtmlKeywordReplacementChecker object used to check keyword replacement.
        /// </summary>
        /// <returns>A IHtmlKeywordReplacementChecker object.</returns>
        IHtmlKeywordReplacementChecker Checker {
            get;
        }
    }

    /// <summary>
    /// Represents a tool to checking keyword replacement.
    /// </summary>
    public interface IHtmlKeywordReplacementChecker {
        /// <summary>
        /// Check whether can perform this replacement operation.
        /// </summary>
        /// <param name="count">The number of replaced keyword.</param>
        /// <param name="keyword">The keyword info.</param>
        /// <param name="position">The keyword position.</param>
        /// <param name="tag">The tag which contains this keyword.</param>
        /// <param name="previousKeyword">The previous replaced keyword info.</param>
        /// <param name="previousPosition">The position of previous replaced keyword.</param>
        /// <returns>Return true if allow this replacement operation, otherwise return false.</returns>
        bool Check(int count, HtmlKeywordReplacementInfo keyword, int position, HtmlTag tag, HtmlKeywordReplacementInfo previousKeyword, int previousPosition);
    }
}
