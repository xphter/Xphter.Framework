using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Xphter.Framework.WeChat {
    /// <summary>
    /// Provides a utility for WeChat library.
    /// </summary>
    public static class WeChatHelper {
        static WeChatHelper() {
            g_oAuthStateRegex = new Regex("^[a-z0-9]{0,128}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private static Regex g_oAuthStateRegex;

        public const string OAUTH_SCOPE_BASE = "snsapi_base";
        public const string OAUTH_SCOPE_USERINFO = "snsapi_userinfo";

        /// <summary>
        /// Gets the URI for OAuth in WeChat.
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="redirectUri"></param>
        /// <param name="scope"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string GetOAuthUri(string appID, string redirectUri, string scope, string state) {
            if(string.IsNullOrWhiteSpace(appID)) {
                throw new ArgumentException("appid is null or empty.", "appID");
            }
            if(string.IsNullOrWhiteSpace(redirectUri)) {
                throw new ArgumentException("redirect uri is null or empty.", "redirectUri");
            }
            if(state != null && !g_oAuthStateRegex.IsMatch(state)) {
                throw new ArgumentException("state rules: a-z, 0-9, 128 bytes max.", "state");
            }

            return string.Format(
                "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect",
                appID,
                HttpUtility.UrlEncode(redirectUri),
                !string.IsNullOrEmpty(scope) ? scope : OAUTH_SCOPE_BASE,
                state);
        }

        /// <summary>
        /// Converts the timestamp to local DateTime.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TimestampToLocalTime(int timestamp) {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp));
        }
    }
}
