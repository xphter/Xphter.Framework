using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatOAuthAccessToken : IWeChatAccessToken {
        string RefreshToken {
            get;
        }

        string OpenID {
            get;
        }

        string Scope {
            get;
        }

        string Unionid {
            get;
        }
    }
}
