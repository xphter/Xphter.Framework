using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Xphter.Framework.Web;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Provides functions to communicate under HTTP protocol.
    /// </summary>
    public static class HttpHelper {
        private static Regex g_contentTypeHeaderRegex = new Regex(@"(?:charset|encoding)\s*\=\s*([\w\-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets the MIME type of the speicified image format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="format"/> is null.</exception>
        public static string GetContentType(ImageFormat format) {
            if(format == null) {
                throw new ArgumentException("The image format is null.", "format");
            }

            string imageType = null;
            if(format.Equals(ImageFormat.Bmp)) {
                imageType = "bmp";
            } else if(format.Equals(ImageFormat.Jpeg)) {
                imageType = "jpeg";
            } else if(format.Equals(ImageFormat.Png)) {
                imageType = "png";
            } else if(format.Equals(ImageFormat.Gif)) {
                imageType = "gif";
            } else if(format.Equals(ImageFormat.Icon)) {
                imageType = "icon";
            } else {
                throw new ArgumentException("Can not determine the MIME type of the spicified image format.", "format");
            }

            return "image/" + imageType;
        }

        /// <summary>
        /// Gets the MIME type of the specified file extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetContentType(string extension) {
            string contentType = HttpContentTypes.DEFAULT_CONTENT_TYPE;
            if(!string.IsNullOrWhiteSpace(extension) && HttpContentTypes.ContentTypes.ContainsKey(extension)) {
                contentType = HttpContentTypes.ContentTypes[extension].First();
            }
            return contentType;
        }

        /// <summary>
        /// Gets the text encoding of the specified HTTP response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is null.</exception>
        public static Encoding GetTextEncoding(HttpWebResponse response) {
            if(response == null) {
                throw new ArgumentNullException("response");
            }

            string charset = null;
            Encoding encoding = null;

            Match match = g_contentTypeHeaderRegex.Match(response.ContentType);
            if(match != null && match.Success) {
                charset = match.Groups[1].Value;
            }

            if(!string.IsNullOrWhiteSpace(charset)) {
                try {
                    encoding = Encoding.GetEncoding(charset);
                } catch(ArgumentException) {
                }
            }

            return encoding;
        }

        #region HTTP Get Methods

        /// <summary>
        /// Gets the response of the specified HTTP resource.
        /// </summary>
        /// <param name="uri">A http address.</param>
        /// <param name="proxy">Web proxy.</param>
        /// <param name="timeout">Timeout value in miliseconds.</param>
        /// <returns>Web server response.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="uri"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="uri"/> is not a valid HTTP URI.</exception>
        public static HttpWebResponse GetRequest(string uri, IWebProxy proxy, int timeout) {
            if(string.IsNullOrWhiteSpace(uri)) {
                throw new ArgumentException("HTTP URI string is null or empty.", "uri");
            }
            if(!Uri.IsWellFormedUriString(uri, UriKind.Absolute)) {
                throw new ArgumentException("HTTP URI string is not a valid absolute URI string.", "uri");
            }

            return GetRequest(new Uri(uri), proxy, timeout);
        }

        /// <summary>
        /// Gets the response of the specified HTTP resource.
        /// </summary>
        /// <param name="uri">A http URI.</param>
        /// <param name="proxy">Web proxy.</param>
        /// <param name="timeout">Timeout value in miliseconds.</param>
        /// <returns>Web server response.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="uri"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="uri"/> is not a HTTP URI.</exception>
        public static HttpWebResponse GetRequest(Uri uri, IWebProxy proxy, int timeout) {
            if(uri == null) {
                throw new ArgumentException("HTTP URI string is null.", "uri");
            }
            if(!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
              !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)) {
                throw new ArgumentException("URI is not a HTTP URI.", "uri");
            }

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.Accept = "*/*";
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;
            request.Proxy = proxy;
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = WebRequestMethods.Http.Get;
            request.CookieContainer = new CookieContainer();

            return GetRequest(request);
        }

        /// <summary>
        /// Gets the response of the specified HTTP resource.
        /// </summary>
        /// <param name="uri">A HttpWebRequest object.</param>
        /// <returns>Web server response.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="request"/> is null.</exception>
        public static HttpWebResponse GetRequest(HttpWebRequest request) {
            if(request == null) {
                throw new ArgumentException("HTTP request is null.", "request");
            }

            return (HttpWebResponse) request.GetResponse();
        }

        #endregion

        #region HTTP Post Methods

        /// <summary>
        /// Gets a random string use in "multipart/form-data" post.
        /// </summary>
        /// <returns></returns>
        private static string GetRandomBoundary() {
            return string.Format("{0}{1}", new string('-', new Random(Environment.TickCount).Next(8, 16)), Environment.TickCount);
        }

        /// <summary>
        /// Post data to a HTTP resource and return the response.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <returns></returns>
        public static HttpWebResponse PostRequest(HttpWebRequest request) {
            return PostRequest(request, null, null, null);
        }

        /// <summary>
        /// Post data to a HTTP resource and return the response.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="data">The data to post.</param>
        /// <returns></returns>
        public static HttpWebResponse PostRequest(HttpWebRequest request, object data) {
            return PostRequest(request, null, data, null);
        }

        /// <summary>
        /// Post data to a HTTP resource and return the response.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="serializer">A FormValuesSerializer object to serialize data to HTTP request stream.</param>
        /// <param name="data">The data to post.</param>
        /// <returns></returns>
        public static HttpWebResponse PostRequest(HttpWebRequest request, FormValuesSerializer serializer, object data) {
            return PostRequest(request, serializer, data, null);
        }

        /// <summary>
        /// Post data to a HTTP resource and return the response.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="serializer">A FormValuesSerializer object to serialize data to HTTP request stream.</param>
        /// <param name="data">The data to post.</param>
        /// <param name="name">If data is not complex type, use to name it.</param>
        /// <returns></returns>
        public static HttpWebResponse PostRequest(HttpWebRequest request, FormValuesSerializer serializer, object data, string name) {
            if(request == null) {
                throw new ArgumentNullException("request");
            }

            string boundary = null;
            serializer = serializer ?? new FormValuesSerializer();
            bool needEncode = FormValuesSerializer.CheckEncodeRequirement(data);
            request.Method = WebRequestMethods.Http.Post;
            if(needEncode) {
                request.ContentType = "application/x-www-form-urlencoded";
            } else {
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary = GetRandomBoundary());
            }
            if(data != null) {
                using(Stream writer = request.GetRequestStream()) {
                    if(needEncode) {
                        serializer.Serialize(data, name, writer);
                    } else {
                        serializer.Serialize(data, name, boundary, writer);
                    }
                }
            } else {
                //avoid sending 411 server error
                request.ContentLength = 0;
            }
            return (HttpWebResponse) request.GetResponse();
        }

        /// <summary>
        /// Begins a async operation to post data to a HTTP resource.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="callback">The callback mehtod to invoke when async operation has completed.</param>
        /// <param name="userState">A user specified data.</param>
        /// <returns></returns>
        public static IAsyncResult BeginPostRequest(HttpWebRequest request, AsyncCallback callback, object userState) {
            return BeginPostRequest(request, null, null, null, callback, userState);
        }

        /// <summary>
        /// Begins a async operation to post data to a HTTP resource.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="data">The data to post.</param>
        /// <param name="callback">The callback mehtod to invoke when async operation has completed.</param>
        /// <param name="userState">A user specified data.</param>
        /// <returns></returns>
        /// <remarks>If you want to upload a file, <paramref name="data"/> must has a member of HttpPostingFile type.</remarks>
        public static IAsyncResult BeginPostRequest(HttpWebRequest request, object data, AsyncCallback callback, object userState) {
            return BeginPostRequest(request, null, data, null, callback, userState);
        }

        /// <summary>
        /// Begins a async operation to post data to a HTTP resource.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="serializer">A FormValuesSerializer object to serialize data to HTTP request stream.</param>
        /// <param name="data">The data to post.</param>
        /// <param name="callback">The callback mehtod to invoke when async operation has completed.</param>
        /// <param name="userState">A user specified data.</param>
        /// <returns></returns>
        /// <remarks>If you want to upload a file, <paramref name="data"/> must has a member of HttpPostingFile type.</remarks>
        public static IAsyncResult BeginPostRequest(HttpWebRequest request, FormValuesSerializer serializer, object data, AsyncCallback callback, object userState) {
            return BeginPostRequest(request, serializer, data, null, callback, userState);
        }

        /// <summary>
        /// Begins a async operation to post data to a HTTP resource.
        /// </summary>
        /// <param name="request">A HttpWebRequest object.</param>
        /// <param name="serializer">A FormValuesSerializer object to serialize data to HTTP request stream.</param>
        /// <param name="data">The data to post.</param>
        /// <param name="name">If data is not complex type, use to name it.</param>
        /// <param name="callback">The callback mehtod to invoke when async operation has completed.</param>
        /// <param name="userState">A user specified data.</param>
        /// <returns></returns>
        /// <remarks>If you want to upload a file, <paramref name="data"/> must has a member of HttpPostingFile type.</remarks>
        public static IAsyncResult BeginPostRequest(HttpWebRequest request, FormValuesSerializer serializer, object data, string name, AsyncCallback callback, object userState) {
            if(request == null) {
                throw new ArgumentNullException("request");
            }

            string boundary = null;
            serializer = serializer ?? new FormValuesSerializer();
            bool needEncode = FormValuesSerializer.CheckEncodeRequirement(data);
            AsyncResult<HttpWebResponse> result = new AsyncResult<HttpWebResponse>(callback, userState);

            request.Method = WebRequestMethods.Http.Post;
            if(needEncode) {
                request.ContentType = "application/x-www-form-urlencoded";
            } else {
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary = GetRandomBoundary());
            }
            if(data != null) {
                NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetRequestStream((streamResult) => {
                    HttpWebRequest httpRequest = (HttpWebRequest) streamResult.AsyncState;
                    try {
                        using(Stream writer = httpRequest.EndGetRequestStream(streamResult)) {
                            if(needEncode) {
                                serializer.Serialize(data, name, writer);
                            } else {
                                serializer.Serialize(data, name, boundary, writer);
                            }
                        }

                        NetworkRequestAsyncTimeout.RegisterRequest(httpRequest.BeginGetResponse((responseResult) => {
                            Exception exception = null;
                            HttpWebResponse response = null;
                            try {
                                response = (HttpWebResponse) ((HttpWebRequest) responseResult.AsyncState).EndGetResponse(responseResult);
                            } catch(Exception ex) {
                                exception = ex;
                            }

                            result.MarkCompleted(exception, response);
                        }, httpRequest), httpRequest, httpRequest.Timeout);
                    } catch(WebException ex) {
                        result.MarkCompleted(ex, (HttpWebResponse) ex.Response);
                    } catch(Exception ex) {
                        result.MarkCompleted(ex);
                    }
                }, request), request, request.Timeout);
            } else {
                //avoid sending 411 server error
                request.ContentLength = 0;
                NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((responseResult) => {
                    Exception exception = null;
                    HttpWebResponse response = null;
                    try {
                        response = (HttpWebResponse) ((HttpWebRequest) responseResult.AsyncState).EndGetResponse(responseResult);
                    } catch(Exception ex) {
                        exception = ex;
                    }

                    result.MarkCompleted(exception, response);
                }, request), request, request.Timeout);
            }
            return result;
        }

        /// <summary>
        /// Ends a async HTTP-POST operation.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        public static HttpWebResponse EndPostRequest(IAsyncResult ar) {
            if(!(ar is AsyncResult<HttpWebResponse>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<HttpWebResponse>) ar).End();
        }

        #endregion

        #region Net Request/Response vs Web Request/Response

        /// <summary>
        /// Creates a HttpCookie from the specified Cookie.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static HttpCookie NetCookieToWebCookie(Cookie cookie) {
            HttpCookie hc = new HttpCookie(cookie.Name, cookie.Value) {
                Expires = cookie.Expires,
                HttpOnly = cookie.HttpOnly,
                Path = cookie.Path,
                Secure = cookie.Secure,
            };
            if(!string.IsNullOrWhiteSpace(cookie.Domain)) {
                hc.Domain = cookie.Domain;
            }
            return hc;
        }

        /// <summary>
        /// Creates a Cookie from the specified HttpCookie.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Cookie WebCookieToNetCookie(HttpCookie cookie) {
            Cookie nc = new Cookie(cookie.Name, cookie.Value, cookie.Path) {
                Expires = cookie.Expires,
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
            };
            if(!string.IsNullOrWhiteSpace(cookie.Domain)) {
                nc.Domain = cookie.Domain;
            }
            return nc;
        }

        /// <summary>
        /// Creates a HttpWebRequest form the specified HttpRequestBase object.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebRequest CreateWebRequest(HttpRequestBase request, Uri url) {
            string value = null;
            HttpWebRequest web = (HttpWebRequest) WebRequest.Create(url ?? request.Url);

            NameValueCollection headers = request.Headers;
            foreach(string name in headers.Keys) {
                if(!WebHeaderCollection.IsRestricted(name)) {
                    try {
                        web.Headers[name] = headers[name];
                    } catch(ArgumentException) {
                    }
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Accept"])) {
                try {
                    web.Accept = value;
                } catch(ArgumentException) {
                    web.Accept = "*/*";
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Connection"])) {
                value = value.ToLower();
                try {
                    if(value.Contains("keep-alive")) {
                        web.KeepAlive = true;
                    } else if(value.Contains("close")) {
                        web.KeepAlive = false;
                    } else {
                        web.Connection = value;
                    }
                } catch(ArgumentException) {
                    web.KeepAlive = true;
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Content-Length"])) {
                long length = 0;
                if(long.TryParse(value, out length)) {
                    web.ContentLength = length;
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Content-Type"])) {
                web.ContentType = value;
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Date"])) {
                DateTime date;
                if(DateTime.TryParse(value, out date)) {
                    web.Date = date;
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Expect"])) {
                if(!(web.ServicePoint.Expect100Continue = value.ToLower().Contains("100-continue"))) {
                    web.Expect = value;
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["If-Modified-Since"])) {
                DateTime time;
                if(DateTime.TryParse(value, out time)) {
                    web.IfModifiedSince = time;
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Referer"])) {
                try {
                    web.Referer = Uri.EscapeUriString(value);
                } catch(ArgumentException) {
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Transfer-Encoding"])) {
                try {
                    web.TransferEncoding = value;
                } catch(ArgumentException) {
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["User-Agent"])) {
                try {
                    web.UserAgent = value;
                } catch(ArgumentException) {
                    web.UserAgent = "MSIE9.0";
                }
            }

            if(!string.IsNullOrWhiteSpace(value = headers["Transfer-Encoding"])) {
                switch(value.ToLower()) {
                    case "chunked":
                        web.SendChunked = true;
                        break;
                    default:
                        web.SendChunked = false;
                        break;
                }
            }

            Cookie cookie = null;
            CookieContainer cookieContainer = web.CookieContainer = new CookieContainer();
            foreach(string name in request.Cookies.AllKeys) {
                try {
                    cookie = WebCookieToNetCookie(request.Cookies[name]);
                    cookie.Domain = web.RequestUri.Host;
                    cookieContainer.Add(cookie);
                } catch(CookieException) {
                }
            }

            web.SetForwardedIP(request);
            web.ServicePoint.Expect100Continue = false;
            web.ProtocolVersion = HttpVersion.Version11;
            web.Method = request.HttpMethod;

            return web;
        }

        /// <summary>
        /// Stuffs current response from a HttpWebResponse object.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="response"></param>
        public static void StuffFromHttpWebResponse(this HttpResponseBase web, HttpWebResponse response) {
            foreach(string name in response.Headers) {
                web.AppendHeader(name, response.Headers[name]);
            }

            web.ContentType = response.ContentType;
            web.Charset = response.CharacterSet;
            if(!string.IsNullOrWhiteSpace(response.CharacterSet)) {
                Encoding encoding = Encoding.GetEncoding(response.CharacterSet);
                web.ContentEncoding = encoding;
                web.HeaderEncoding = encoding;
            }
            web.StatusCode = (int) response.StatusCode;
            web.StatusDescription = response.StatusDescription;

            web.ClearContent();
            if(!string.IsNullOrWhiteSpace(response.Headers["Transfer-Encoding"]) || response.ContentLength < 0) {
                int count = 0;
                byte[] buffer = new byte[1024 * 1024];
                using(Stream reader = response.GetResponseStream()) {
                    using(Stream writer = web.OutputStream) {
                        while((count = reader.Read(buffer, 0, buffer.Length)) > 0) {
                            writer.Write(buffer, 0, count);
                        }
                    }
                }
            } else {
                byte[] data = new byte[response.ContentLength];
                using(Stream reader = response.GetResponseStream()) {
                    reader.Read(data, 0, data.Length);
                }
                using(Stream writer = web.OutputStream) {
                    writer.Write(data, 0, data.Length);
                }
            }

            web.Flush();
        }

        #endregion
    }
}
