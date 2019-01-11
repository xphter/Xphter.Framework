using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Xphter.Framework.Collections;
using Xphter.Framework.Net;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provides functions of baidu.com.
    /// </summary>
    public static class BaiduUtility {
        static BaiduUtility() {
            g_pingRequestFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<methodCall>" +
                    "<methodName>weblogUpdates.extendedPing</methodName>" +
                    "<params>" +
                        "<param>" +
                            "<value><string>{0}</string></value>" +
                        "</param>" +
                        "<param>" +
                            "<value><string>{1}</string></value>" +
                        "</param>" +
                        "<param>" +
                            "<value><string>{2}</string></value>" +
                        "</param>" +
                        "<param>" +
                            "<value><string>{3}</string></value>" +
                        "</param>" +
                    "</params>" +
                "</methodCall>";
            g_bufferManager = BufferManager.CreateBufferManager(10, BUFFER_SIZE);

            g_includeCountRegex = new Regex(@"百度为您找到相关结果约\s*([\d\,]+)\s*个", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_indexCountRegex1 = new Regex(@"该网站共有\s*\<b[^\<\>]*\>\s*([\d\,]+)\s*\<\/b\>\s*个网页被百度收录", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_indexCountRegex2 = new Regex(@"找到相关结果数约\s*([\d\,]+)\s*个", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            g_chinazRankRegex = new Regex(@"百度权重：[^\d]*(\d{1,2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_chinazFlowRegex = new Regex(@"预估百度流量：[^\d]*(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_aiZhanRankRegex = new Regex(@"http\:\/\/static\.aizhan\.com\/images\/brs\/(\d{1,2})\.gif", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_aiZhanFlowRegex = new Regex(@"预计来路[^\d]*\d+\s*~\s*(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            g_rankPointRegex = new Regex(@"\sid\=\""\d{1,2}\""\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private const int BUFFER_SIZE = 1024;
        private const int TIMEOUT = 30000;

        private static string g_pingRequestFormat;
        private static BufferManager g_bufferManager;

        private static HttpWebRequest CreatePingRequest() {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://ping.baidu.com/ping/RPC2");
            request.UserAgent = "request";
            request.ContentType = "text/xml";
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Http.Post;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = TIMEOUT;
            request.ReadWriteTimeout = TIMEOUT;
            return request;
        }

        private static HttpWebRequest CreatePushRequest(string site, string token, bool isOriginal) {
            string url = null;
            if(isOriginal) {
                url = string.Format("http://data.zz.baidu.com/urls?site={0}&token={1}&type=original", HttpUtility.UrlEncode(site), HttpUtility.UrlEncode(token));
            } else {
                url = string.Format("http://data.zz.baidu.com/urls?site={0}&token={1}", HttpUtility.UrlEncode(site), HttpUtility.UrlEncode(token));
            }

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.UserAgent = "curl/7.12.1";
            request.ContentType = "text/plain";
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Http.Post;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = TIMEOUT;
            request.ReadWriteTimeout = TIMEOUT;
            return request;
        }

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

        #region PING

        /// <summary>
        /// Invokes the update notification service of baidu.com(Baidu PING).
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="homepageUri"></param>
        /// <param name="articleUri"></param>
        /// <param name="rssUri"></param>
        /// <returns></returns>
        public static bool Ping(string siteName, string homepageUri, string articleUri, string rssUri) {
            string content = null;
            HttpWebRequest request = CreatePingRequest();
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            byte[] data = Encoding.UTF8.GetBytes(string.Format(g_pingRequestFormat, siteName, homepageUri, articleUri, rssUri));
            request.ContentLength = data.Length;
            try {
                using(Stream writer = request.GetRequestStream()) {
                    writer.Write(data, 0, data.Length);
                }
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }
            } catch(WebException) {
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }

            return !string.IsNullOrWhiteSpace(content) && content.Contains("<int>0</int>");
        }

        /// <summary>
        /// Begins an asynchronous operation to invoke the update notification service of baidu.com(Baidu PING).
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="homepageUri"></param>
        /// <param name="articleUri"></param>
        /// <param name="rssUri"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginPing(string siteName, string homepageUri, string articleUri, string rssUri, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreatePingRequest();
            AsyncResult<bool> result = new AsyncResult<bool>(callback, userState);

            byte[] data = Encoding.UTF8.GetBytes(string.Format(g_pingRequestFormat, siteName, homepageUri, articleUri, rssUri));
            request.ContentLength = data.Length;
            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetRequestStream((streamResult) => {
                try {
                    using(Stream writer = ((HttpWebRequest) streamResult.AsyncState).EndGetRequestStream(streamResult)) {
                        writer.Write(data, 0, data.Length);
                    }
                } catch(WebException) {
                    result.MarkCompleted(null, false, false);
                    return;
                } catch(Exception ex) {
                    result.MarkCompleted(ex, false, false);
                    return;
                }

                NetworkRequestAsyncTimeout.RegisterRequest(((HttpWebRequest) streamResult.AsyncState).BeginGetResponse((contentResult) => {
                    string content = null;
                    Exception error = null;
                    byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                    try {
                        using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) contentResult.AsyncState).EndGetResponse(contentResult)) {
                            content = response.GetResponseString(buffer);
                        }
                    } catch(WebException) {
                    } catch(Exception ex) {
                        error = ex;
                    } finally {
                        g_bufferManager.ReturnBuffer(buffer);
                    }

                    result.MarkCompleted(error, false, !string.IsNullOrWhiteSpace(content) && content.Contains("<int>0</int>"));
                }, request), request, TIMEOUT);
            }, request), request, TIMEOUT);

            return result;
        }

        /// <summary>
        /// Ends an asynchronous operation to invoke the update notification service of baidu.com(Baidu PING).
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static bool EndPing(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<bool>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<bool>) asyncResult).End();
        }

        #endregion

        #region PUSH

        /// <summary>
        /// Push urls to baidu.com.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="token"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static IBaiduPushResult Push(string site, string token, bool isOriginal, IEnumerable<string> urls) {
            string content = null;
            PushResult pushResult = null;
            HttpWebRequest request = CreatePushRequest(site, token, isOriginal);
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            byte[] data = Encoding.UTF8.GetBytes(urls.StringJoin("\r\n"));
            request.ContentLength = data.Length;
            try {
                using(Stream writer = request.GetRequestStream()) {
                    writer.Write(data, 0, data.Length);
                }
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    if(!string.IsNullOrWhiteSpace(content = response.GetResponseString(buffer))) {
                        pushResult = new JavaScriptSerializer().Deserialize<PushResult>(content);
                    }
                }
            } catch(WebException ex) {
                try {
                    using(HttpWebResponse response = (HttpWebResponse) ex.Response) {
                        if(response != null) {
                            if(!string.IsNullOrWhiteSpace(content = response.GetResponseString(buffer))) {
                                pushResult = new JavaScriptSerializer().Deserialize<PushResult>(content);
                            }
                        } else {
                            throw ex;
                        }
                    }
                } catch {
                    throw ex;
                }
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }

            return pushResult;
        }

        /// <summary>
        /// Starts an asynchronous operation to push urls to baidu.com.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="token"></param>
        /// <param name="urls"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginPush(string site, string token, bool isOriginal, IEnumerable<string> urls, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreatePushRequest(site, token, isOriginal);
            AsyncResult<IBaiduPushResult> result = new AsyncResult<IBaiduPushResult>(callback, userState);

            byte[] data = Encoding.UTF8.GetBytes(urls.StringJoin("\r\n"));
            request.ContentLength = data.Length;
            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetRequestStream((streamResult) => {
                try {
                    using(Stream writer = ((HttpWebRequest) streamResult.AsyncState).EndGetRequestStream(streamResult)) {
                        writer.Write(data, 0, data.Length);
                    }
                } catch(Exception ex) {
                    result.MarkCompleted(ex, false, null);
                    return;
                }

                NetworkRequestAsyncTimeout.RegisterRequest(((HttpWebRequest) streamResult.AsyncState).BeginGetResponse((contentResult) => {
                    string content = null;
                    Exception error = null;
                    PushResult pushResult = null;
                    byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                    try {
                        using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) contentResult.AsyncState).EndGetResponse(contentResult)) {
                            if(!string.IsNullOrWhiteSpace(content = response.GetResponseString(buffer))) {
                                pushResult = new JavaScriptSerializer().Deserialize<PushResult>(content);
                            }
                        }
                    } catch(WebException ex) {
                        try {
                            using(HttpWebResponse response = (HttpWebResponse) ex.Response) {
                                if(response != null) {
                                    if(!string.IsNullOrWhiteSpace(content = response.GetResponseString(buffer))) {
                                        pushResult = new JavaScriptSerializer().Deserialize<PushResult>(content);
                                    }
                                } else {
                                    error = ex;
                                }
                            }
                        } catch {
                            error = ex;
                        }
                    } catch(Exception ex) {
                        error = ex;
                    } finally {
                        g_bufferManager.ReturnBuffer(buffer);
                    }

                    result.MarkCompleted(error, false, pushResult);
                }, request), request, TIMEOUT);
            }, request), request, TIMEOUT);

            return result;
        }

        /// <summary>
        /// Ends an asynchronous operation to push urls to baidu.com.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IBaiduPushResult EndPush(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IBaiduPushResult>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IBaiduPushResult>) asyncResult).End();
        }

        public class PushResult : IBaiduPushResult {
            [ObfuscationAttribute]
            public int error;

            [ObfuscationAttribute]
            public string message;

            [ObfuscationAttribute]
            public int success;

            [ObfuscationAttribute]
            public int remain;

            [ObfuscationAttribute]
            public string[] not_same_site;

            [ObfuscationAttribute]
            public string[] not_valid;

            #region IBaiduPushResult Members

            int IBaiduPushResult.ErrorCode {
                get {
                    return this.error;
                }
            }

            string IBaiduPushResult.Message {
                get {
                    return this.message;
                }
            }

            int IBaiduPushResult.SuccessCount {
                get {
                    return this.success;
                }
            }

            int IBaiduPushResult.RemainCount {
                get {
                    return this.remain;
                }
            }

            IEnumerable<string> IBaiduPushResult.ForeignUrls {
                get {
                    return this.not_same_site;
                }
            }

            IEnumerable<string> IBaiduPushResult.InvalidUrls {
                get {
                    return this.not_valid;
                }
            }

            #endregion
        }

        #endregion

        #region INCLUDE INFO

        private static Regex g_includeCountRegex;
        private static Regex g_indexCountRegex1;
        private static Regex g_indexCountRegex2;

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

        private static int ParseIndexCount(string content) {
            if(string.IsNullOrWhiteSpace(content)) {
                return 0;
            }

            Match match = g_indexCountRegex1.Match(content);
            if(match != null && match.Success) {
                return int.Parse(match.Groups[1].Value, NumberStyles.AllowThousands);
            }

            match = g_indexCountRegex2.Match(content);
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
            return string.Format("http://www.baidu.com/s?wd={0}", HttpUtility.UrlEncode("site:" + domainName));
        }

        /// <summary>
        /// Gets include info of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static IBaiduIncludeInfo GetIncludeInfo(string domainName) {
            HttpWebRequest request = CreateNormalRequest(GetIncludeInfoQueryUri(domainName));
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }

                return new IncludeInfo {
                    IncludeCount = ParseIncludeCount(content),
                    IndexCount = ParseIndexCount(content),
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
            AsyncResult<IBaiduIncludeInfo> result = new AsyncResult<IBaiduIncludeInfo>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IBaiduIncludeInfo info = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }

                    info = new IncludeInfo {
                        IncludeCount = ParseIncludeCount(content),
                        IndexCount = ParseIndexCount(content),
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
        public static IBaiduIncludeInfo EndGetIncludeInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IBaiduIncludeInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IBaiduIncludeInfo>) asyncResult).End();
        }

        private class IncludeInfo : IBaiduIncludeInfo {
            #region IBaiduIncludeInfo Members

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

        #region BAIDU RANK

        #region chinaz.com

        private static Regex g_chinazRankRegex;
        private static Regex g_chinazFlowRegex;

        private static IBaiduRankInfo ParseChinazBaiduRank(string content) {
            BaiduRankInfo rank = new BaiduRankInfo();
            if(string.IsNullOrWhiteSpace(content)) {
                return rank;
            }

            Match match = g_chinazRankRegex.Match(content);
            if(match != null && match.Success) {
                rank.RankValue = int.Parse(match.Groups[1].Value);
            }

            match = g_chinazFlowRegex.Match(content);
            if(match != null && match.Success) {
                rank.ExpectedFlow = int.Parse(match.Groups[1].Value);
            }

            return rank;
        }

        /// <summary>
        /// Gets the URI to query baidu rank in chinaz.com of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string GetChinazBaiduRankQueryUri(string domainName) {
            return string.Format("http://mytool.chinaz.com/baidusort.aspx?host={0}", HttpUtility.UrlEncode(domainName));
        }

        /// <summary>
        /// Gets baidu rank in chinaz.com of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static IBaiduRankInfo GetChinazBaiduRank(string domainName) {
            HttpWebRequest request = CreateNormalRequest(GetChinazBaiduRankQueryUri(domainName));
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }

                return ParseChinazBaiduRank(content);
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// Starts an asynchronous operation to get baidu rank in chinaz.com. 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginGetChinazBaiduRank(string domainName, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreateNormalRequest(GetChinazBaiduRankQueryUri(domainName));
            AsyncResult<IBaiduRankInfo> result = new AsyncResult<IBaiduRankInfo>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IBaiduRankInfo rank = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }

                    rank = ParseChinazBaiduRank(content);
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
        /// Ends an asynchronous operation to get baidu rank in chinaz.com.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IBaiduRankInfo EndGetChinazBaiduRank(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IBaiduRankInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IBaiduRankInfo>) asyncResult).End();
        }

        #endregion

        #region aizhan.com

        private static Regex g_aiZhanRankRegex;
        private static Regex g_aiZhanFlowRegex;

        private static IBaiduRankInfo ParseAiZhanBaiduRank(string content) {
            BaiduRankInfo rank = new BaiduRankInfo();
            if(string.IsNullOrWhiteSpace(content)) {
                return rank;
            }

            Match match = g_aiZhanRankRegex.Match(content);
            if(match != null && match.Success) {
                rank.RankValue = int.Parse(match.Groups[1].Value);
            }

            match = g_aiZhanFlowRegex.Match(content);
            if(match != null && match.Success) {
                rank.ExpectedFlow = int.Parse(match.Groups[1].Value);
            }

            return rank;
        }

        /// <summary>
        /// Gets the URI to query baidu rank in aizhan.com of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static string GetAiZhanBaiduRankQueryUri(string domainName) {
            return string.Format("http://baidurank.aizhan.com/baidu/{0}/", HttpUtility.UrlEncode(domainName));
        }

        /// <summary>
        /// Gets baidu rank in aizhan.com of the specified domain name.
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static IBaiduRankInfo GetAiZhanBaiduRank(string domainName) {
            HttpWebRequest request = CreateNormalRequest(GetAiZhanBaiduRankQueryUri(domainName));
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                }

                return ParseAiZhanBaiduRank(content);
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// Starts an asynchronous operation to get baidu rank in aizhan.com. 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginGetAiZhanBaiduRank(string domainName, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreateNormalRequest(GetAiZhanBaiduRankQueryUri(domainName));
            AsyncResult<IBaiduRankInfo> result = new AsyncResult<IBaiduRankInfo>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IBaiduRankInfo rank = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }

                    rank = ParseAiZhanBaiduRank(content);
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
        /// Ends an asynchronous operation to get baidu rank in aizhan.com.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IBaiduRankInfo EndGetAiZhanBaiduRank(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IBaiduRankInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IBaiduRankInfo>) asyncResult).End();
        }

        #endregion

        private class BaiduRankInfo : IBaiduRankInfo {
            #region IBaiduRankInfo Members

            public int RankValue {
                get;
                set;
            }

            public int ExpectedFlow {
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
        public const int MAX_RANK = 50;

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
            return string.Format("http://www.baidu.com/s?wd={0}&rn=50", HttpUtility.UrlEncode(keyword));
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

    public interface IBaiduPushResult {
        int ErrorCode {
            get;
        }

        string Message {
            get;
        }

        int SuccessCount {
            get;
        }

        int RemainCount {
            get;
        }

        IEnumerable<string> ForeignUrls {
            get;
        }

        IEnumerable<string> InvalidUrls {
            get;
        }
    }

    public interface IBaiduIncludeInfo {
        int IndexCount {
            get;
        }

        int IncludeCount {
            get;
        }
    }

    public interface IBaiduRankInfo {
        int RankValue {
            get;
        }

        int ExpectedFlow {
            get;
        }
    }

    public interface IBaiduIndexInfo {
        int TotalIndex {
            get;
        }

        int MobileIndex {
            get;
        }
    }
}
