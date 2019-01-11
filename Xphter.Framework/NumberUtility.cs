using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides utilities to process numbers.
    /// </summary>
    public static class NumberUtility {
        /// <summary>
        /// Parses the specified text to a integer.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Parse(string s, int defaultValue) {
            int.TryParse(s, out defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Parses the specified text to a long integer.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long Parse(string s, long defaultValue) {
            long.TryParse(s, out defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Parses the specified text to a float number.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float Parse(string s, float defaultValue) {
            float.TryParse(s, out defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Parses the specified text to a double-float number.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double Parse(string s, double defaultValue) {
            double.TryParse(s, out defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Parses the specified text to a decimal number.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal Parse(string s, decimal defaultValue) {
            decimal.TryParse(s, out defaultValue);
            return defaultValue;
        }
    }
}
