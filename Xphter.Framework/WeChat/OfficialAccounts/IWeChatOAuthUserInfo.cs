using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatOAuthUserInfo : IWeChatReturnValue {
        string OpenID {
            get;
        }

        string UnionID {
            get;
        }

        string Nickname {
            get;
        }

        bool? Sex {
            get;
        }

        string Country {
            get;
        }

        string Province {
            get;
        }

        string City {
            get;
        }

        IEnumerable<string> Privileges {
            get;
        }

        string GetAvatarImageUri(WeChatAvatarImageSize size);
    }
}
