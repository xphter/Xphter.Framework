using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Collections;
using Xphter.Framework.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides utilities to process string.
    /// </summary>
    public static class StringUtility {
        /// <summary>
        /// Returns a new string in which all occurrences of the keys of the specified dictionary in the current instance are replaced with the values of the specifid dictionary.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string Replace(this string text, IDictionary<string, string> replacement, bool ignoreCase) {
            return Replace(text, replacement.Keys, (arg) => replacement[arg], ignoreCase);
        }

        /// <summary>
        /// Returns a new string in which all occurrences of the specified strings in the current instance are replaced with another specified strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="oldValues"></param>
        /// <param name="newValueSelector"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string Replace(this string text, IEnumerable<string> oldValues, Func<string, string> newValueSelector, bool ignoreCase) {
            if(string.IsNullOrWhiteSpace(text)) {
                return text;
            }
            oldValues = new List<string>(oldValues.Where((item) => !string.IsNullOrEmpty(item)));
            if(oldValues.Count() == 0) {
                return text;
            }

            if(oldValues.Count() == 1) {
                string oldValue = oldValues.First();
                return Regex.Replace(text, RegexUtility.Encode(oldValue), newValueSelector(oldValue), ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            }

            //search all occurrences of each old value
            int groupIndex = 0;
            StringBuilder pattern = new StringBuilder();
            IDictionary<Range, string> replacement = new Dictionary<Range, string>();
            foreach(string item in oldValues) {
                pattern.AppendFormat("({0})|", RegexUtility.Encode(item));
            }
            foreach(Match match in Regex.Matches(text, pattern.Remove(pattern.Length - 1, 1).ToString(), ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)) {
                groupIndex = -1;
                for(int i = 1; i < match.Groups.Count; i++) {
                    if(match.Groups[i].Success) {
                        groupIndex = i;
                        break;
                    }
                }

                replacement[new Range(match.Index, match.Length)] = newValueSelector(oldValues.ElementAt(groupIndex - 1));
            }

            //replace all old values
            int offset = 0;
            Range key = null;
            string value = null;
            int index = 0, length = 0;
            StringBuilder result = new StringBuilder(text);
            foreach(KeyValuePair<Range, string> item in replacement) {
                key = item.Key;
                value = item.Value;
                index = Convert.ToInt32(key.StartIndex) + offset;
                length = Convert.ToInt32(key.Length);

                result.Remove(index, length);
                result.Insert(index, value);
                offset += value.Length - length;
            }

            return result.ToString();
        }

        /// <summary>
        /// Determines whether this string represents a integer.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsInteger(this string text) {
            long value = 0;
            return !string.IsNullOrWhiteSpace(text) ? long.TryParse(text, out value) : false;
        }

        /// <summary>
        /// Determines whether this string represents a number.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNumber(this string text) {
            double value = 0;
            return !string.IsNullOrWhiteSpace(text) ? double.TryParse(text, out value) : false;
        }

        /// <summary>
        /// Determines whether this string represents a DateTime.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string text) {
            DateTime value;
            return !string.IsNullOrWhiteSpace(text) ? DateTime.TryParse(text, out value) : false;
        }

        /// <summary>
        /// Finds all parts between <paramref name="startValue"/> and <paramref name="endValue"/> in <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="startValue"/> or <paramref name="endValue"/> is null or empty.</exception>
        public static IEnumerable<string> Between(this string text, string startValue, string endValue) {
            return Between(text, 0, startValue, endValue, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Finds all parts between <paramref name="startValue"/> and <paramref name="endValue"/> in <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than zero or not less than length of <paramref name="text"/>.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="startValue"/> or <paramref name="endValue"/> is null or empty.</exception>
        public static IEnumerable<string> Between(this string text, int startIndex, string startValue, string endValue) {
            return Between(text, startIndex, startValue, endValue, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Finds all parts between <paramref name="startValue"/> and <paramref name="endValue"/> in <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="startValue"/> or <paramref name="endValue"/> is null or empty.</exception>
        public static IEnumerable<string> Between(this string text, string startValue, string endValue, StringComparison comparisonType) {
            return Between(text, 0, startValue, endValue, comparisonType);
        }

        /// <summary>
        /// Finds all parts between <paramref name="startValue"/> and <paramref name="endValue"/> in <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than zero or not less than length of <paramref name="text"/>.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="startValue"/> or <paramref name="endValue"/> is null or empty.</exception>
        public static IEnumerable<string> Between(this string text, int startIndex, string startValue, string endValue, StringComparison comparisonType) {
            if(startIndex < 0 || !string.IsNullOrEmpty(text) && startIndex >= text.Length) {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if(string.IsNullOrEmpty(startValue)) {
                throw new ArgumentException("startValue is null or empty.", "startValue");
            }
            if(string.IsNullOrEmpty(endValue)) {
                throw new ArgumentException("endValue is null or empty.", "startValue");
            }

            if(string.IsNullOrEmpty(text)) {
                return Enumerable.Empty<string>();
            }

            int endIndex = 0;
            int startLength = startValue.Length;
            int endLength = endValue.Length;
            ICollection<string> result = new List<string>();
            while(startIndex < text.Length && (startIndex = text.IndexOf(startValue, startIndex, comparisonType)) >= 0) {
                if((startIndex += startLength) >= text.Length) {
                    break;
                }
                if((endIndex = text.IndexOf(endValue, startIndex, comparisonType)) < 0) {
                    break;
                }

                result.Add(text.Substring(startIndex, endIndex - startIndex));
                startIndex = endIndex + endLength;
            }

            return result;
        }

        /// <summary>
        /// Finds the first string which is not null, empty, or consists only of white-space characters from <paramref name="strings"/>.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string FindNonEmpty(params string[] strings) {
            if(strings == null || strings.Length == 0) {
                return null;
            }

            return strings.Where((item) => !string.IsNullOrWhiteSpace(item)).FirstOrDefault();
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the specified string object.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Trim(string s) {
            return s != null ? s.Trim() : s;
        }

        /// <summary>
        /// Parses the specified text to a boolean.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool? ToBoolean(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            bool value = false;
            if(bool.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Parses the specified text to a integer.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int? ToInt32(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            int value = 0;
            if(int.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Parses the specified text to a long integer.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long? ToInt64(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            long value = 0;
            if(long.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Parses the specified text to a float number.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float? ToSingle(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            float value = 0;
            if(float.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Parses the specified text to a double-float number.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double? ToDouble(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            double value = 0;
            if(double.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Parses the specified text to a decimal number.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal? ToDecimal(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            decimal value = 0;
            if(decimal.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Parses the specified text to a DateTime.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return null;
            }

            DateTime value = DateTime.MinValue;
            if(DateTime.TryParse(s, out value)) {
                return value;
            } else {
                return null;
            }
        }
    }
}
