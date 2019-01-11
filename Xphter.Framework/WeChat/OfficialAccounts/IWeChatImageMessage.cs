using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatImageMessage : IWeChatCommonMessage {
        string PicUrl {
            get;
        }

        string MediaId {
            get;
        }
    }
}
