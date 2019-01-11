using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Defines extension methods of HttpResponseBase class.
    /// </summary>
    public static class HttpResponseExtension {
        private const string CONTENT_LENGTH_HEADER_NAME = "Content-Length";

        /// <summary>
        /// Sets content length of the specified HTTP response.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="length"></param>
        public static void SetContentLength(this HttpResponseBase response, long length) {
            response.Headers[CONTENT_LENGTH_HEADER_NAME] = length.ToString();
        }

        /// <summary>
        /// Sets content length of the specified HTTP response.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="length"></param>
        public static void SetContentLength(this HttpResponse response, long length) {
            response.Headers[CONTENT_LENGTH_HEADER_NAME] = length.ToString();
        }
    }
}
