using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatAccessToken : IWeChatReturnValue {
        string AccessToken {
            get;
        }

        int ExpiresIn {
            get;
        }
    }
}
