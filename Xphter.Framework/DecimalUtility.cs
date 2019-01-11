using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xphter.Framework {
    /// <summary>
    /// Provides utilities to process decimal.
    /// </summary>
    public static class DecimalUtility {
        /// <summary>
        /// Determines whether the specified value has decimal places.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Return true if <paramref name="value"/> has decimal places, otherwise return false.</returns>
        public static bool HasDecimalPlaces(this decimal value) {
            return value > Math.Floor(value);
        }

        /// <summary>
        /// Gets the number of decimal places of the speicifed decimal value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Number of decimal places of <paramref name="value"/>.</returns>
        public static int GetDecimalPlaces(this decimal value) {
            return (decimal.GetBits(value)[3] >> 16) & 0x00FF;
        }

        /// <summary>
        /// Gets the number of valid decimal places of the specified decimal value.
        /// The first valid decimal place is the first nonzero decimal place from right to left.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetValidDecimalPlaces(this decimal value) {
            int count = value.GetDecimalPlaces();
            if(count == 0) {
                return 0;
            }

            return Regex.Match(value.ToString("F" + count), "^\\-?\\d+\\.(\\d*?)0*$").Groups[1].Length;
        }

        /// <summary>
        /// Converts the specified decimal to representation of uppercase RMB.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUppercaseRMB(this decimal value) {
            string result = null;
            string text = value.ToString("0.########");
            int dotIndex = text.IndexOf('.');

            result = IntegerUtility.ToUppercaseChineseNumber(long.Parse(dotIndex > 0 ? text.Substring(0, dotIndex) : text)) + "元";
            if(dotIndex > 0) {
                int digit = int.Parse(text[dotIndex + 1].ToString());
                if(digit != 0) {
                    result += IntegerUtility.ToUppercaseChineseDigit(digit) + "角";
                }
                if(dotIndex + 2 < text.Length) {
                    digit = int.Parse(text[dotIndex + 2].ToString());
                    if(digit != 0) {
                        result += IntegerUtility.ToUppercaseChineseDigit(digit) + "分";
                    }
                }
            } else {
                result += "整";
            }

            return result;
        }
    }
}
