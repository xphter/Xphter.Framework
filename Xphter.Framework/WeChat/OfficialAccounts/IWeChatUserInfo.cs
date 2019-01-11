using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public　interface IWeChatUserInfo : IWeChatReturnValue {
        bool IsSubscribed {
            get;
        }

        DateTime SubscribeTime {
            get;
        }

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

        WeChatLanguage Language {
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

        string Remark {
            get;
        }

        int GroupID {
            get;
        }

        string AvatarImageUri {
            get;
        }

        string GetAvatarImageUri(WeChatAvatarImageSize size);
    }
}
