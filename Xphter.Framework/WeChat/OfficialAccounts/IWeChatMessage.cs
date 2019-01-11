using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatMessage {
        string ToUserName {
            get;
        }

        string FromUserName {
            get;
        }

        int CreateTime {
            get;
        }

        string MsgType {
            get;
        }
    }
}
