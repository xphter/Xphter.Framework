using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Text;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Defines extension methods of WebResponse class.
    /// </summary>
    public static class WebResponseExtension {
        static WebResponseExtension() {
            g_charsetRegex = new Regex(@"\<meta[^\<\>]+?charset\=(?:\""|\')?([\w\-]+)(?:\""|\')?[^\<\>]*\>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            DefaultContentEncoding = Encoding.Default;
        }

        private static Regex g_charsetRegex;

        /// <summary>
        /// Gets or sets the default encoding when can not determine the encoding type of a HttpWebResponse object.
        /// </summary>
        public static Encoding DefaultContentEncoding {
            get;
            set;
        }

        /// <summary>
        /// Gets the raw content as a byte array.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] GetRawContent(this WebResponse response, int bufferSize) {
            return GetRawContent(response, new byte[bufferSize]);
        }

        /// <summary>
        /// Gets the raw content as a byte array.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        public static byte[] GetRawContent(this WebResponse response, byte[] buffer) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }

            int count = 0;
            using(MemoryStream writer = new MemoryStream()) {
                using(Stream reader = response.GetResponseStream()) {
                    while((count = reader.Read(buffer, 0, buffer.Length)) > 0) {
                        writer.Write(buffer, 0, count);
                    }
                }

                return writer.ToArray();
            }
        }

        /// <summary>
        /// Gets the response content as a byte array.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] GetResponseContent(this HttpWebResponse response, int bufferSize) {
            return GetResponseContent(response, new byte[bufferSize]);
        }

        /// <summary>
        /// Gets the response content as a byte array.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.NotSupportedException">Not supported content-encoding.</exception>
        public static byte[] GetResponseContent(this HttpWebResponse response, byte[] buffer) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }
            if(!string.IsNullOrWhiteSpace(response.ContentEncoding) && !string.Equals(response.ContentEncoding, "gzip", StringComparison.OrdinalIgnoreCase)) {
                throw new NotSupportedException(string.Format("Not supported content-encoding: {0}", response.ContentEncoding));
            }

            int count = 0;
            using(MemoryStream writer = new MemoryStream()) {
                using(Stream reader = string.IsNullOrWhiteSpace(response.ContentEncoding) ? response.GetResponseStream() : new GZipStream(response.GetResponseStream(), CompressionMode.Decompress, false)) {
                    while((count = reader.Read(buffer, 0, buffer.Length)) > 0) {
                        writer.Write(buffer, 0, count);
                    }
                }

                return writer.ToArray();
            }
        }

        /// <summary>
        /// Gets the response content as a text.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        /// <exception cref="System."><paramref name="bufferSize"/> not greater than zero.</exception>
        /// <exception cref="System.NotSupportedException">Not supported character set.</exception>        
        public static string GetResponseString(this HttpWebResponse response, int bufferSize) {
            return GetResponseString(response, null, bufferSize);
        }

        /// <summary>
        /// Gets the response content as a text.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.NotSupportedException">Not supported character set.</exception>
        public static string GetResponseString(this HttpWebResponse response, byte[] buffer) {
            return GetResponseString(response, null, buffer);
        }

        /// <summary>
        /// Gets the response content as a byte array.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="buffer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <exception cref="System."><paramref name="bufferSize"/> not greater than zero.</exception>
        /// <exception cref="System.NotSupportedException">Not supported character set.</exception>
        public static string GetResponseString(this HttpWebResponse response, Encoding encoding, int bufferSize) {
            if(bufferSize <= 0) {
                throw new ArgumentOutOfRangeException("bufferSize", "bufferSize not greater than zero.");
            }

            return GetResponseString(response, encoding, new byte[bufferSize]);
        }

        /// <summary>
        /// Gets the response content as a byte array.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="buffer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.NotSupportedException">Not supported character set.</exception>
        public static string GetResponseString(this HttpWebResponse response, Encoding encoding, byte[] buffer) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }

            if(encoding == null) {
                encoding = HttpHelper.GetTextEncoding(response);
            }

            byte[] data = GetResponseContent(response, buffer);
            if(encoding == null) {
                if((encoding = EncodingUtility.DetectUnicodeEncoding(data)) == null) {
                    string content = DefaultContentEncoding.GetString(data);
                    Match match = g_charsetRegex.Match(content);
                    if(match != null && match.Success) {
                        switch(match.Groups[1].Value.ToLower()) {
                            case "utf8":
                            case "utf-8":
                                encoding = Encoding.UTF8;
                                break;
                            case "utf16":
                            case "utf-16":
                                encoding = Encoding.Unicode;
                                break;
                            case "utf32":
                            case "utf-32":
                                encoding = Encoding.UTF32;
                                break;
                            default:
                                try {
                                    encoding = Encoding.GetEncoding(match.Groups[1].Value);
                                } catch(ArgumentException) {
                                }
                                break;
                        }
                    }
                    if(encoding == null) {
                        foreach(string item in content.Between("<meta ", ">", StringComparison.OrdinalIgnoreCase).Select((item) => item.ToLower())) {
                            if(item.Contains("utf8") || item.Contains("utf-8")) {
                                encoding = Encoding.UTF8;
                            } else if(item.Contains("utf16") || item.Contains("utf-16")) {
                                encoding = Encoding.Unicode;
                            } else if(item.Contains("utf32") || item.Contains("utf-32")) {
                                encoding = Encoding.UTF32;
                            } else if(item.Contains("gb2312")) {
                                encoding = Encoding.GetEncoding("gb2312");
                            } else if(item.Contains("gbk")) {
                                encoding = Encoding.GetEncoding("gbk");
                            }

                            if(encoding != null) {
                                break;
                            }
                        }
                    }


                    /*
                     * if can not determine the encoding type, we use default encoding.
                     */
                    if(encoding == null || DefaultContentEncoding.Equals(encoding)) {
                        return content;
                    }
                }
            }

            return encoding.GetString(data);
        }
    }
}
