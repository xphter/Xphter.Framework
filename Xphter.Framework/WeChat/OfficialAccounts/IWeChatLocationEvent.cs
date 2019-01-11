using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatLocationEvent : IWeChatEventMessage {
        float Latitude {
            get;
        }

        float Longitude {
            get;
        }

        float Precision {
            get;
        }
    }
}
