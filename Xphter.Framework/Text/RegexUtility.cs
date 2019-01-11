using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Text {
    /// <summary>
    /// Provide a utility for processing regular expression.
    /// </summary>
    public static class RegexUtility {
        /// <summary>
        /// Encode the specified string which is a constant component of  a regural expression.
        /// </summary>
        /// <param name="value">A string.</param>
        /// <returns>The string after encoded.</returns>
        public static string Encode(string value) {
            if(string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            StringBuilder s = new StringBuilder(value.Length);
            foreach(char c in value) {
                switch(c) {
                    case '\'':
                    case '\\':
                    case '/':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case '|':
                    case '^':
                    case '$':
                    case '.':
                    case '-':
                    case '*':
                    case '?':
                    case '+':
                    case '=':
                    case '!':
                    case ':':
                    case '#':
                        s.Append("\\" + c);
                        break;
                    default:
                        s.Append(c);
                        break;
                }
            }

            return s.ToString();
        }
    }
}
