using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Web.Script.Serialization;
using Xphter.Framework;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Provides functions to communicate under IP protocol.
    /// </summary>
    public static class IpHelper {
        /// <summary>
        /// The JavaScriptSerializer object used to parse JSON string.
        /// </summary>
        private static JavaScriptSerializer g_jsSerializer = new JavaScriptSerializer();

        private const int TIMEOUT = 30000;

        #region Get IP Location

        /// <summary>
        /// The BufferManager for GetLocation operation.
        /// </summary>
        private static BufferManager g_ipLocationBufferManager = BufferManager.CreateBufferManager(10, 1024);

        /// <summary>
        /// Creates a HttpWebRequest of the specified IP address for GetLocation operation.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="proxy"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateGetLocationRequest(string ip, IWebProxy proxy) {
            if(!string.IsNullOrWhiteSpace(ip)) {
                IPAddress address = null;
                if(!IPAddress.TryParse(ip, out address)) {
                    throw new ArgumentException("ip not represents a valid IP address", "ip");
                }
            }

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(string.Format("http://int.dpool.sina.com.cn/iplookup/?ip={0}&format=json", ip));
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers["Accept-Encoding"] = "gzip,deflate,sdch";
            request.Headers["Accept-Language"] = "en-US,en;q=0.8,zh-CN;q=0.6,zh;q=0.4,zh-TW;q=0.2";
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.Timeout = TIMEOUT;
            request.ReadWriteTimeout = TIMEOUT;
            request.Method = WebRequestMethods.Http.Get;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Proxy = proxy;
            return request;
        }

        /// <summary>
        /// Parses the HTTP response of the specified HTTP request for GetLocation operation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static IEnumerable<string> ParseGetLocationResponse(HttpWebRequest request, HttpWebResponse response) {
            byte[] buffer = g_ipLocationBufferManager.TakeBuffer(1024);

            try {
                string content = response.GetResponseString(buffer);
                SinaLocationServiceReturnValue value = g_jsSerializer.Deserialize<SinaLocationServiceReturnValue>(content);
                return new string[] { value.country, value.province, value.city, };
            } finally {
                g_ipLocationBufferManager.ReturnBuffer(buffer);
                response.Close();
                request.Abort();
            }
        }

        /// <summary>
        /// Begins a async operation to get location info of the specified IP address.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="ip"/> not represents a valid IP address.</exception>
        public static IAsyncResult BeginGetLocation(string ip, AsyncCallback callback, object userState) {
            return BeginGetLocation(ip, null, callback, userState);
        }

        /// <summary>
        /// Begins a async operation to get location info of the specified IP address.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="proxy"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="ip"/> not represents a valid IP address.</exception>
        public static IAsyncResult BeginGetLocation(string ip, IWebProxy proxy, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreateGetLocationRequest(ip, proxy);
            AsyncResult<IEnumerable<string>> result = new AsyncResult<IEnumerable<string>>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IEnumerable<string> locations = null;
                try {
                    locations = ParseGetLocationResponse(request, (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar));
                } catch(Exception ex) {
                    result.MarkCompleted(ex, false);
                    return;
                }

                result.MarkCompleted(null, locations);
            }, request), request, TIMEOUT);

            return result;
        }

        /// <summary>
        /// Ends the async operation to get location info of a IP address.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IEnumerable<string> EndGetLocation(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IEnumerable<string>>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IEnumerable<string>>) asyncResult).End();
        }

        /// <summary>
        /// Gets location info of the the specified IP address.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="ip"/> not represents a valid IP address.</exception>
        public static IEnumerable<string> GetLocation(string ip) {
            return GetLocation(ip, null);
        }

        /// <summary>
        /// Gets location info of the the specified IP address.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="ip"/> not represents a valid IP address.</exception>
        public static IEnumerable<string> GetLocation(string ip, IWebProxy proxy) {
            HttpWebRequest request = CreateGetLocationRequest(ip, proxy);
            return ParseGetLocationResponse(request, (HttpWebResponse) request.GetResponse());
        }

        private class SinaLocationServiceReturnValue {
            [ObfuscationAttribute]
            public int ret = 0;

            [ObfuscationAttribute]
            public string start = null;
            
            [ObfuscationAttribute]
            public string end = null;

            [ObfuscationAttribute]
            public string country = null;

            [ObfuscationAttribute]
            public string province = null;

            [ObfuscationAttribute]
            public string city = null;

            [ObfuscationAttribute]
            public string district = null;

            [ObfuscationAttribute]
            public string isp = null;

            [ObfuscationAttribute]
            public string type = null;

            [ObfuscationAttribute]
            public string desc = null;
        }

        #endregion
    }
}
