using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Defines extension methods of HttpRequestBase class.
    /// </summary>
    public static class HttpRequestExtension {
        private const string FORWARDED_HEADER_NAME = "X-FORWARDED-FOR";
        private static readonly char[] ProxyIPSeperators = new char[] { ',' };

        /// <summary>
        /// Finds the real IP address form forwarded IP and default IP.
        /// </summary>
        /// <param name="forwardedIP"></param>
        /// <param name="defaultIP"></param>
        /// <returns></returns>
        private static string FindClientIP(string forwardedIP, string defaultIP) {
            string ip = forwardedIP;

            if(!string.IsNullOrWhiteSpace(ip)) {
                IPAddress address = null;
                string[] parts = ip.Split(ProxyIPSeperators, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length > 0 && IPAddress.TryParse(parts[0], out address)) {
                    ip = address.ToString();
                } else {
                    ip = defaultIP;
                }
            } else {
                ip = defaultIP;
            }

            return ip;
        }

        /// <summary>
        /// Sets the forwarded IP of <paramref name="request"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="forwardedIP"></param>
        /// <param name="currentIP"></param>
        private static void SetForwardedIP(HttpWebRequest request, string forwardedIP, string currentIP) {
            string ip = forwardedIP;
            if(string.IsNullOrWhiteSpace(ip)) {
                ip = currentIP;
            } else {
                ip += "," + currentIP;
            }

            request.Headers[FORWARDED_HEADER_NAME] = ip;
        }

        /// <summary>
        /// Gets the real IP address of HTTP client.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIP(this HttpRequestBase request) {
            return FindClientIP(request.Headers[FORWARDED_HEADER_NAME], request.UserHostAddress);
        }

        /// <summary>
        /// Gets the real IP address of HTTP client.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIP(this HttpRequest request) {
            return FindClientIP(request.Headers[FORWARDED_HEADER_NAME], request.UserHostAddress);
        }

        /// <summary>
        /// Sets the forwarded IP of <paramref name="webRequst"/> form <paramref name="httpRequest"/>.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="httpRequest"></param>
        public static void SetForwardedIP(this HttpWebRequest webRequest, HttpRequestBase httpRequest) {
            SetForwardedIP(webRequest, httpRequest.Headers[FORWARDED_HEADER_NAME], httpRequest.UserHostAddress);
        }

        /// <summary>
        /// Sets the forwarded IP of <paramref name="webRequst"/> form <paramref name="httpRequest"/>.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="httpRequest"></param>
        public static void SetForwardedIP(this HttpWebRequest webRequest, HttpRequest httpRequest) {
            SetForwardedIP(webRequest, httpRequest.Headers[FORWARDED_HEADER_NAME], httpRequest.UserHostAddress);
        }
    }
}
