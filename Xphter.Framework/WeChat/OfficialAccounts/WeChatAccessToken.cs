using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatAccessToken : WeChatReturnValue, IWeChatAccessToken {
        [ObfuscationAttribute]
        public string access_token {
            get;
            set;
        }

        [ObfuscationAttribute]
        public int expires_in {
            get;
            set;
        }

        public override string ToString() {
            return this.access_token;
        }

        #region IWeChatAccessToken Members

        string IWeChatAccessToken.AccessToken {
            get {
                return this.access_token;
            }
        }

        int IWeChatAccessToken.ExpiresIn {
            get {
                return this.expires_in;
            }
        }

        #endregion
    }
}
