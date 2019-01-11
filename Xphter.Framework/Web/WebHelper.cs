using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Serialization;
using Microsoft.Win32;
using Xphter.Framework.Collections;
using Xphter.Framework.Net;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provides the encoder and decoder methods used to process HTTP request.
    /// </summary>
    public static class WebHelper {
        static WebHelper() {
            g_applicationRootPath = HttpRuntime.AppDomainAppVirtualPath ?? string.Empty;
            if(!g_applicationRootPath.EndsWith(@"/")) {
                g_applicationRootPath += @"/";
            }

            g_bufferManager = BufferManager.CreateBufferManager(10, BUFFER_SIZE);

            g_htmlTitleRegex = new Regex(@"\<title[^\<\>]*\>([^\<\>]*)\<\/title\>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_metaRegex = new Regex(@"\<meta(?:\s+(?'key'[\w\-]+)\s*\=\s*(?:(?:\""(?'value'[^\""]*)\"")|(?:\'(?'value'[^\']*)\')|(?'value'[^\'\""\<\>\s]+)))+\s*\/?\>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private const int BUFFER_SIZE = 1024;
        private const int TIMEOUT = 30000;

        /// <summary>
        /// The application root path.
        /// </summary>
        private static string g_applicationRootPath = "/";

        private static BufferManager g_bufferManager;

        #region Mobile and Tablet UserAgent Patterns

        private const string MOBILE_KEYWORD = "mobile";

        private static readonly string[] KNOWN_MOBILE_USER_AGENT_PREFIXES = new String[] {
            "w3c ", "w3c-", "acs-", "alav", "alca", "amoi", "audi", "avan",
            "benq", "bird", "blac", "blaz", "brew", "cell", "cldc", "cmd-",
            "dang",  "doco", "eric", "hipt", "htc_", "inno", "ipaq", "ipod",
            "jigs", "kddi", "keji", "leno", "lg-c", "lg-d", "lg-g", "lge-",
            "lg/u", "maui", "maxo", "midp", "mits", "mmef", "mobi", "mot-",
            "moto", "mwbp", "nec-", "newt", "noki", "palm", "pana", "pant",
            "phil", "play", "port", "prox", "qwap", "sage", "sams", "sany",  
            "sch-", "sec-", "send", "seri", "sgh-", "shar", "sie-", "siem",
            "smal", "smar", "sony", "sph-", "symb", "t-mo", "teli", "tim-",
            "tosh", "tsm-", "upg1", "upsi", "vk-v", "voda", "wap-", "wapa",
            "wapi", "wapp", "wapr", "webc", "winw", "winw", "xda ", "xda-",
        };

        private static readonly string[] KNOWN_MOBILE_USER_AGENT_KEYWORDS = new String[] {
            "blackberry", "webos", "ipod", "lge vx", "midp", "maemo",
            "mmp", "mobile", "netfront", "hiptop", "nintendo DS",
            "novarra", "openweb", "opera mobi", "opera mini",
            "palm", "psp", "phone", "smartphone", "symbian",
            "up.browser", "up.link", "wap", "windows ce",
        };

        private static readonly String[] KNOWN_TABLET_USER_AGENT_KEYWORDS = new String[] {
            "ipad", "playbook", "tablet", "kindle"
        };

        #endregion

        /// <summary>
        /// Initializes the application root path from the specified http context.
        /// </summary>
        /// <param name="context"></param>
        [Obsolete("There is no need to call this method, class will auto initialize the root application virtual path.")]
        public static void InitializeApplicationRootPath(HttpContextBase context) {
            g_applicationRootPath = new UrlHelper(context.Request.RequestContext, RouteTable.Routes).Content("~");
        }

        /// <summary>
        /// Gets the application absolute path of the specified relative path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string GetApplicationAsbolutePath(string relativePath) {
            if(string.IsNullOrWhiteSpace(relativePath)) {
                return g_applicationRootPath;
            }

            relativePath = relativePath.Replace('\\', '/');
            if(relativePath.StartsWith(@"/")) {
                relativePath = relativePath.Remove(0, 1);
            }
            return g_applicationRootPath + relativePath;
        }

        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        /// <remarks>Be differ from System.Web.HttpUtility.HtmlEncode method, this method encodes space character.</remarks>
        /// <param name="value">A string.</param>
        /// <returns>The string after encoded.</returns>
        public static string HtmlEncode(string value) {
            if(string.IsNullOrEmpty(value)) {
                return value;
            }

            StringBuilder result = new StringBuilder(value.Length);
            foreach(char c in value) {
                if(c <= '>') {
                    switch(c) {
                        case '>':
                            result.Append("&gt;");
                            break;
                        case '<':
                            result.Append("&lt;");
                            break;
                        case '&':
                            result.Append("&amp;");
                            break;
                        case ' ':
                            result.Append("&nbsp;");
                            break;
                        case '\"':
                            result.Append("&quot;");
                            break;
                        case '\'':
                            result.Append("'");
                            break;
                        default:
                            result.Append(c);
                            break;
                    }
                } else {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts a string to an JSON-encoded string.
        /// </summary>
        /// <param name="value">A string.</param>
        /// <returns>The string after encoded.</returns>
        public static string JsonEncode(string value) {
            if(string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            StringBuilder result = new StringBuilder(value.Length);
            foreach(char c in value) {
                switch(c) {
                    case '\'':
                    case '\"':
                    case '\r':
                    case '\n':
                    case '\\':
                        result.Append(@"\" + c);
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets ClientDeviceType from the HttpContextBase object.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>This method can not recognise the WeChat server callback request, because the UserAgent is "Mozilla/4.0" only.</remarks>
        public static ClientDeviceType GetClientDeviceType(HttpContextBase context) {
            if(context == null) {
                throw new ArgumentNullException("context");
            }

            HttpRequestBase request = context.Request;
            string userAgent = request.UserAgent != null ? request.UserAgent.ToLower() : null;
            if(string.IsNullOrWhiteSpace(userAgent)) {
                return ClientDeviceType.Unknown;
            }

            string value = null;
            NameValueCollection headers = request.Headers;
            string userAgentPrefix = userAgent.Substring(0, Math.Min(4, userAgent.Length));

            if(headers["x-wap-profile"] != null || headers["profile"] != null) {
                return ClientDeviceType.Phone;
            }
            value = headers["Accept"];
            if(value != null && value.Contains("wap")) {
                return ClientDeviceType.Phone;
            }

            if(userAgent.Contains(OperatingSystem.Names.Android) && !userAgent.Contains(MOBILE_KEYWORD)) {
                return ClientDeviceType.Tablet;
            }
            if(userAgent.Contains("silk") && !userAgent.Contains(MOBILE_KEYWORD)) {
                return ClientDeviceType.Tablet;
            }
            if(KNOWN_TABLET_USER_AGENT_KEYWORDS.Any((item) => userAgent.Contains(item))) {
                return ClientDeviceType.Tablet;
            }
            if(KNOWN_MOBILE_USER_AGENT_PREFIXES.Contains(userAgentPrefix)) {
                return ClientDeviceType.Phone;
            }
            if(KNOWN_MOBILE_USER_AGENT_KEYWORDS.Any((item) => userAgent.Contains(item))) {
                return ClientDeviceType.Phone;
            }
            if(headers.Keys.Cast<string>().Any((item) => item.Contains("OperaMini"))) {
                return ClientDeviceType.Phone;
            }

            return ClientDeviceType.PersonalComputer;
        }

        /// <summary>
        /// Gets a value to indicate whether the specified device is a mobile device.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static bool IsMobileDevice(ClientDeviceType deviceType) {
            switch(deviceType) {
                case ClientDeviceType.Phone:
                case ClientDeviceType.Tablet:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates a new UrlHelper instance.
        /// </summary>
        /// <returns></returns>
        public static UrlHelper CreateUrlHelper() {
            UrlHelper helper = null;

            if(HttpContext.Current != null) {
                HttpContext context = HttpContext.Current;
                if(context.Request.RequestContext != null) {
                    helper = new UrlHelper(HttpContext.Current.Request.RequestContext, RouteTable.Routes);
                } else {
                    helper = new UrlHelper(new System.Web.Routing.RequestContext(new HttpContextWrapper(context), new RouteData()), RouteTable.Routes);
                }
            } else {
                HttpRequest request = new HttpRequest("/", "http://localhost", string.Empty);
                HttpResponse response = new HttpResponse(new StringWriter());
                HttpContext context = new HttpContext(request, response);

                helper = new UrlHelper(new System.Web.Routing.RequestContext(new HttpContextWrapper(context), new RouteData()), RouteTable.Routes);
            }

            return helper;
        }

        /// <summary>
        /// Gets version of installed IIS.
        /// </summary>
        /// <returns>The IIS version or null if uninstalled.</returns>
        public static int? GetIISVersion() {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"software\microsoft\inetstp");
            if(key == null) {
                return null;
            }

            object value = key.GetValue("MajorVersion");
            if(value == null) {
                return null;
            }

            int version = 0;
            if(int.TryParse(value.ToString(), out version)) {
                return version;
            } else {
                return null;
            }
        }

        #region Web Site Info

        private const string META_NAME = "name";
        private const string META_CONTENT = "content";
        private const string META_KEYWORDS_NAME = "keywords";
        private const string META_DESCRIPTION_NAME = "description";
        private static readonly char[] SiteKeywordsSeparators = new char[] { '_', ',', ';', '|', '，', '、', '—', '丨' };

        private static Regex g_htmlTitleRegex;
        private static Regex g_metaRegex;

        private static HttpWebRequest CreateGetWebSiteInfoRequest(string url, IWebProxy proxy) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers["Accept-Encoding"] = "gzip";
            request.Headers["Accept-Language"] = "en-US,en;q=0.8,ja;q=0.6,zh-CN;q=0.4,zh;q=0.2,zh-TW;q=0.2,ms;q=0.2,de;q=0.2,tr;q=0.2";
            request.KeepAlive = false;
            request.Proxy = proxy;
            request.Method = WebRequestMethods.Http.Get;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = TIMEOUT;
            request.ReadWriteTimeout = TIMEOUT;
            return request;
        }

        private static IWebSiteInfo ParseWebSiteInfo(string url, HttpWebResponse response, string content) {
            WebSiteInfo info = new WebSiteInfo {
                SiteUri = url,
                ProtocolType = string.Format("HTTP{0} {1} {2}", response.ProtocolVersion, (int) response.StatusCode, response.StatusDescription),
                PageType = response.ContentType,
                ServerType = response.Server,
                EncodingType = response.ContentEncoding,
            };

            Match match = g_htmlTitleRegex.Match(content);
            if(match != null && match.Success) {
                info.HtmlTitle = match.Groups[1].Value;
            }

            int? nameIndex = null, contentIndex = null;
            Capture[] keyCaptures = null, valueCaptures = null;
            foreach(Match item in g_metaRegex.Matches(content)) {
                nameIndex = contentIndex = null;
                keyCaptures = item.Groups["key"].Captures.Cast<Capture>().ToArray();
                valueCaptures = item.Groups["value"].Captures.Cast<Capture>().ToArray();

                for(int i = 0; i < keyCaptures.Length; i++) {
                    switch(keyCaptures[i].Value.ToLower()) {
                        case META_NAME:
                            nameIndex = i;
                            break;
                        case META_CONTENT:
                            contentIndex = i;
                            break;
                    }
                }
                if(!nameIndex.HasValue || !contentIndex.HasValue) {
                    continue;
                }

                switch(valueCaptures[nameIndex.Value].Value.ToLower()) {
                    case META_KEYWORDS_NAME:
                        info.MetaKeywords = valueCaptures[contentIndex.Value].Value;
                        break;
                    case META_DESCRIPTION_NAME:
                        info.MetaDescription = valueCaptures[contentIndex.Value].Value;
                        break;
                }
            }

            info.SiteKeywords = info.MetaKeywords != null ? info.MetaKeywords.Split(SiteKeywordsSeparators, StringSplitOptions.RemoveEmptyEntries).Select((item) => item.Trim()).Where((item) => item.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() : Enumerable.Empty<string>();
            return info;
        }

        /// <summary>
        /// Gets web site info of the specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IWebSiteInfo GetWebSiteInfo(string url, IWebProxy proxy) {
            HttpWebRequest request = CreateGetWebSiteInfoRequest(url, proxy);
            byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

            try {
                string content = null;
                using(HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
                    content = response.GetResponseString(buffer);
                    return ParseWebSiteInfo(url, response, content);
                }
            } finally {
                g_bufferManager.ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// Starts an asynchronous operation to get web site info. 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginGetWebSiteInfo(string url, IWebProxy proxy, AsyncCallback callback, object userState) {
            HttpWebRequest request = CreateGetWebSiteInfoRequest(url, proxy);
            AsyncResult<IWebSiteInfo> result = new AsyncResult<IWebSiteInfo>(callback, userState);

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                IWebSiteInfo info = null;
                Exception error = null;
                byte[] buffer = g_bufferManager.TakeBuffer(BUFFER_SIZE);

                try {
                    string content = null;
                    using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                        info = ParseWebSiteInfo(url, response, content);
                    }
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
        /// Ends an asynchronous operation to get web site info.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static IWebSiteInfo EndGetWebSiteInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWebSiteInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWebSiteInfo>) asyncResult).End();
        }

        private class WebSiteInfo : IWebSiteInfo {
            #region IWebSiteInfo Members

            public string SiteUri {
                get;
                set;
            }

            public string ProtocolType {
                get;
                set;
            }

            public string PageType {
                get;
                set;
            }

            public string ServerType {
                get;
                set;
            }

            public string EncodingType {
                get;
                set;
            }

            public string HtmlTitle {
                get;
                set;
            }

            public string MetaKeywords {
                get;
                set;
            }

            public string MetaDescription {
                get;
                set;
            }

            public IEnumerable<string> SiteKeywords {
                get;
                set;
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// Represents info of a web site.
    /// </summary>
    public interface IWebSiteInfo {
        /// <summary>
        /// Gets the site URI.
        /// </summary>
        string SiteUri {
            get;
        }

        /// <summary>
        /// Gets the protocol type.
        /// </summary>
        string ProtocolType {
            get;
        }

        /// <summary>
        /// Gets the page type.
        /// </summary>
        string PageType {
            get;
        }

        /// <summary>
        /// Gets the server type.
        /// </summary>
        string ServerType {
            get;
        }

        /// <summary>
        /// Gets the encoding type.
        /// </summary>
        string EncodingType {
            get;
        }

        /// <summary>
        /// Gets the HTML title.
        /// </summary>
        string HtmlTitle {
            get;
        }

        /// <summary>
        /// Gets the meta keywords
        /// </summary>
        string MetaKeywords {
            get;
        }

        /// <summary>
        /// Gets the meta description.
        /// </summary>
        string MetaDescription {
            get;
        }

        /// <summary>
        /// Gets all keywords of this web site.
        /// </summary>
        IEnumerable<string> SiteKeywords {
            get;
        }
    }
}