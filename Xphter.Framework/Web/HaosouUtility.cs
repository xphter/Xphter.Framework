using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Xphter.Framework.Collections;
using Xphter.Framework.Net;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provides functions of haosou.com.
    /// </summary>
    public static class HaosouUtility {
        static HaosouUtility() {
            g_bufferManager = BufferManager.CreateBufferManager(10, BUFFER_SIZE);

            g_includeCountRegex = new Regex(@"找到相关结果约\s*([\d\,]+)\s*个", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            g_rankPointRegex = new Regex(@"class\=\""res\-list\""", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private const int BUFFER_SIZE = 1024;
        private const int TIMEOUT = 30000;

        private static BufferManager g_bufferManager;

        private static HttpWebRequest CreateNormalRequest(string url) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers["Accept-Encoding"] = "gzip";
            request.Headers["Accept-Language"] = "en-US,en;q=0.8,ja;q=0.6,zh-CN;q=0.4,zh;q=0.2,zh-TW;q=0.2,ms;q=0.2,de;q=0.2,tr;q=0.2";
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Http.Get;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = TIMEOUT;
            request.ReadWriteTimeout = TIMEOUT;
            return request;
        }

        #region INCLUDE INFO

        private static Regex g_includeCountRegex;

        private static int ParseIncludeCount(string content) {
            if(string.IsNullOrWhiteSpace(content)) {
                return 0;
            }

            Match match = g_includeCountRegex.Match(content);
            if(match != null && match.Success) {
                return int.Parse(match.Groups[1].Value, NumberStyles.AllowThousands);
            }

            return 0;
        }

        /// <summary>
        /// Gets the URI to query include info of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string GetIncludeInfoQueryUri(string domainName) {
            return string.Format("http://www.haosou.com/s?q={0}", HttpUtility.UrlEncode("site:" + domainName));
        }

        /// <summary>
        /// Gets include info of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static IHaosouIncludeInfo GetIncludeInfo(string domainName) {
            HttpWebRequest request = CreateNormalRequest(GetIncludeInfoQueryUri(domainName));
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }

                return new IncludeInfo {
                    IncludeCount = ParseIncludeCount(content),
                };
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// Starts an asynchronous operation to get include info. 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginGetIncludeInfo(string domainName, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreateNormalRequest(GetIncludeInfoQueryUri(domainName));
            AsyncResult<IHaosouIncludeInfo> result = new AsyncResult<IHaosouIncludeInfo>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IHaosouIncludeInfo info = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }

                    info = new IncludeInfo {
                        IncludeCount = ParseIncludeCount(content),
                    };
                } catch(Exception ex) {
                    error = ex;
                } finally {
                    g_bufferManager.ReturnBuffer(buffer);
                }

                result.MarkCompleted(error, false, info);
            }, request), request, TIMEOUT);

            return result;
        }

        /// <summary>
        /// Ends an asynchronous operation to get include info.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IHaosouIncludeInfo EndGetIncludeInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IHaosouIncludeInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IHaosouIncludeInfo>) asyncResult).End();
        }

        private class IncludeInfo : IHaosouIncludeInfo {
            #region IHaosouIncludeInfo Members

            public int IncludeCount {
                get;
                set;
            }

            #endregion
        }

        #endregion

        #region KEYWORD RANK

        /// <summary>
        /// The maxmum number of rank.
        /// </summary>
        public const int MAX_RANK = 10;

        private static Regex g_rankPointRegex;

        private static IEnumerable<int> ParseKeywordRank(string content, string domainName) {
            if(string.IsNullOrWhiteSpace(content)) {
                return Enumerable.Empty<int>();
            }

            ICollection<int> rank = new List<int>();
            int[] points = g_rankPointRegex.Matches(content).Cast<Match>().Select((item) => item.Index).ToArray();

            if(points.Length > 0) {
                for(int i = 0; i < points.Length; i++) {
                    if(content.IndexOf(
                        domainName,
                        points[i],
                        (i < points.Length - 1 ? points[i + 1] : content.Length) - points[i],
                        StringComparison.OrdinalIgnoreCase) >= 0) {
                        rank.Add(i + 1);
                        break;
                    }
                }
            }

            return rank;
        }

        /// <summary>
        /// Gets the URI to query rank of the specified keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string GetKeywordRankQueryUri(string keyword) {
            return string.Format("https://www.haosou.com/s?q={0}", HttpUtility.UrlEncode(keyword));
        }

        /// <summary>
        /// Gets keyword rank of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetKeywordRank(string keyword, string domainName) {
            HttpWebRequest request = CreateNormalRequest(GetKeywordRankQueryUri(keyword));
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }

                return ParseKeywordRank(content, domainName);
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// Starts an asynchronous operation to get keyword rank. 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginGetKeywordRank(string keyword, string domainName, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreateNormalRequest(GetKeywordRankQueryUri(keyword));
            AsyncResult<IEnumerable<int>> result = new AsyncResult<IEnumerable<int>>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IEnumerable<int> rank = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }

                    rank = ParseKeywordRank(content, domainName);
                } catch(Exception ex) {
                    error = ex;
                } finally {
                    g_bufferManager.ReturnBuffer(buffer);
                }

                result.MarkCompleted(error, false, rank);
            }, request), request, TIMEOUT);

            return result;
        }

        /// <summary>
        /// Ends an asynchronous operation to get keyword rank.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IEnumerable<int> EndGetKeywordRank(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IEnumerable<int>>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IEnumerable<int>>) asyncResult).End();
        }

        #endregion
    }

    public interface IHaosouIncludeInfo {
        int IncludeCount {
            get;
        }
    }
}
