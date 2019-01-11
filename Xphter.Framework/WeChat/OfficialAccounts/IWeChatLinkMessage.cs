using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatLinkMessage : IWeChatCommonMessage {
        float Title {
            get;
        }

        string Description {
            get;
        }

        string Url {
            get;
        }
    }
}
