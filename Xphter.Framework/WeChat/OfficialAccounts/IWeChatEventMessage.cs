using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatEventMessage : IWeChatMessage {
        string Event {
            get;
        }

        string EventKey {
            get;
        }
    }
}
