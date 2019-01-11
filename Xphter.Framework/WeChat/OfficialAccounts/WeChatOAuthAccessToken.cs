using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatOAuthAccessToken : WeChatAccessToken, IWeChatOAuthAccessToken {
        [ObfuscationAttribute]
        public string refresh_token {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string openid {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string scope {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string unionid {
            get;
            set;
        }

        #region IWeChatOAuthAccessToken Members

        string IWeChatOAuthAccessToken.RefreshToken {
            get {
                return this.refresh_token;
            }
        }

        string IWeChatOAuthAccessToken.OpenID {
            get {
                return this.openid;
            }
        }

        string IWeChatOAuthAccessToken.Scope {
            get {
                return this.scope;
            }
        }

        string IWeChatOAuthAccessToken.Unionid {
            get {
                return this.unionid;
            }
        }

        #endregion
    }
}
