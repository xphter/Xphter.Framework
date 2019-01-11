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
    /// Provides functions of sogou.com.
    /// </summary>
    public static class SougouUtility {
        static SougouUtility() {
            g_bufferManager = BufferManager.CreateBufferManager(10, BUFFER_SIZE);

            g_includeCountRegex1 = new Regex(@"找到约([\s\<\/\>\=\""\-\,\w]+)条结果", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_includeCountRegex2 = new Regex(@"[\d\,]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            g_rankPointRegex = new Regex(@"RANK\:(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

        private static Regex g_includeCountRegex1;
        private static Regex g_includeCountRegex2;

        private static ISougouIncludeInfo ParseIncludeInfo(string content) {
            IncludeInfo info = new IncludeInfo();
            if(string.IsNullOrWhiteSpace(content)) {
                return info;
            }

            Match[] matches = g_includeCountRegex1.Matches(content).Cast<Match>().ToArray();
            if(matches.Length > 0) {
                info.IndexCount = int.Parse(g_includeCountRegex2.Match(matches[0].Groups[1].Value).Value, NumberStyles.AllowThousands);
                if(matches.Length > 1) {
                    info.IncludeCount = int.Parse(g_includeCountRegex2.Match(matches[1].Groups[1].Value).Value, NumberStyles.AllowThousands);
                } else {
                    info.IncludeCount = info.IndexCount;
                }
            }

            return info;
        }

        /// <summary>
        /// Gets the URI to query include info of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string GetIncludeInfoQueryUri(string domainName) {
            return string.Format("http://www.sogou.com/web?query={0}", HttpUtility.UrlEncode("site:" + domainName));
        }

        /// <summary>
        /// Gets include info of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static ISougouIncludeInfo GetIncludeInfo(string domainName) {
            HttpWebRequest request = CreateNormalRequest(GetIncludeInfoQueryUri(domainName));
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }

                return ParseIncludeInfo(content);
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
            AsyncResult<ISougouIncludeInfo> result = new AsyncResult<ISougouIncludeInfo>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                ISougouIncludeInfo info = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }

                    info = ParseIncludeInfo(content);
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
        public static ISougouIncludeInfo EndGetIncludeInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<ISougouIncludeInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<ISougouIncludeInfo>) asyncResult).End();
        }

        private class IncludeInfo : ISougouIncludeInfo {
            #region ISougouIncludeInfo Members

            public int IndexCount {
                get;
                set;
            }

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
        public const int MAX_RANK = 100;

        private static Regex g_rankPointRegex;

        private static IEnumerable<int> ParseKeywordRank(string content, string domainName) {
            if(string.IsNullOrWhiteSpace(content)) {
                return Enumerable.Empty<int>();
            }

            ICollection<int> rank = new List<int>();
            Match[] points = g_rankPointRegex.Matches(content).Cast<Match>().ToArray();

            if(points.Length > 0) {
                for(int i = 0; i < points.Length; i++) {
                    if(content.IndexOf(
                        domainName,
                        i == 0 ? 0 : points[i - 1].Index,
                        points[i].Index - (i == 0 ? 0 : points[i - 1].Index),
                        StringComparison.OrdinalIgnoreCase) >= 0) {
                        rank.Add(int.Parse(points[i].Groups[1].Value) + 1);
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
            return string.Format("http://www.sogou.com/web?query={0}&num=100", HttpUtility.UrlEncode(keyword));
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

    public interface ISougouIncludeInfo {
        int IndexCount {
            get;
        }

        int IncludeCount {
            get;
        }
    }
}
