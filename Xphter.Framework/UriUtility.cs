using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides utility of operate a URI.
    /// </summary>
    public static class UriUtility {
        static UriUtility() {
            HttpLocalhost = new Uri("http://localhost/");
        }

        /// <summary>
        /// The HTTP URI of localhost.
        /// </summary>
        public static readonly Uri HttpLocalhost;

        /// <summary>
        /// Gets a value to indicate whether the specified URI is a HTTP address.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">This instance represents a relative URI, and this extension method is valid only for absolute URIs.</exception>
        public static bool IsHttp(this Uri uri) {
            return string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets path component of a URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetPath(this Uri uri) {
            if(uri.IsAbsoluteUri) {
                return uri.AbsolutePath;
            } else {
                return new Uri(HttpLocalhost, uri).AbsolutePath;
            }
        }

        /// <summary>
        /// Gets a new URI without fragment from this URI object.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Uri ExceptFragment(this Uri uri) {
            if(!string.IsNullOrEmpty(uri.Fragment)) {
                UriBuilder builder = new UriBuilder(uri);
                builder.Fragment = null;
                return builder.Uri;
            } else {
                return uri;
            }
        }
    }
}
