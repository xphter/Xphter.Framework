using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;

namespace Xphter.Framework.Text {
    /// <summary>
    /// Provides function to handle chinese text.
    /// </summary>
    public static class ChineseUtility {
        /// <summary>
        /// Gets pinyin text of <paramref name="s"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetPinyin(string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return s;
            }

            string pinyin = null;
            StringBuilder result = new StringBuilder(s.Length * 5);
            foreach(char c in s) {
                try {
                    if(ChineseChar.IsValidChar(c)) {
                        pinyin = new ChineseChar(c).Pinyins[0].ToLower();
                        result.Append(char.ToUpper(pinyin[0]));
                        if(pinyin.Length > 2) {
                            result.Append(pinyin, 1, pinyin.Length - 2);
                        }
                    } else {
                        result.Append(c);
                    }
                } catch(NotSupportedException) {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the first letter of pinyin of <paramref name="c"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static char GetFirstLetterOfPinyin(char c) {
            char result = c;
            try {
                if(ChineseChar.IsValidChar(c)) {
                    result = char.ToUpper(new ChineseChar(c).Pinyins[0][0]);
                }
            } catch(NotSupportedException) {
            }
            return result;
        }

        /// <summary>
        /// Gets the first letters of pinyin of <paramref name="s"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFirstLettersOfPinyin(string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return s;
            }

            return new string(s.Select((item) => GetFirstLetterOfPinyin(item)).ToArray());
        }

        /// <summary>
        /// Converts <paramref name="s"/> from simplified chinese to traditional chinese.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SimplifiedToTraditional(string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return s;
            }

            return ChineseConverter.Convert(s, ChineseConversionDirection.SimplifiedToTraditional);
        }

        /// <summary>
        /// Converts <paramref name="s"/> from traditional chinese to simplified chinese.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TraditionalToSimplified(string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return s;
            }

            return ChineseConverter.Convert(s, ChineseConversionDirection.TraditionalToSimplified);
        }
    }
}
