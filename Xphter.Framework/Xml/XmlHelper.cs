using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Xml {
    /// <summary>
    /// Provides the encoder and decoder methods used to process XML document.
    /// </summary>
    public static class XmlHelper {
        /// <summary>
        /// Converts a string to an XML-encoded string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string XmlEncode(string s) {
            StringBuilder result = new StringBuilder(s.Length);
            foreach(char c in s) {
                switch(c) {
                    case '<':
                        result.Append("&lt;");
                        break;
                    case '>':
                        result.Append("&gt;");
                        break;
                    case '&':
                        result.Append("&amp;");
                        break;
                    case '\'':
                        result.Append("&apos;");
                        break;
                    case '"':
                        result.Append("&quot;");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }

            return result.ToString();
        }
    }
}
