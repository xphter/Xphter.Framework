using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatVoiceMessage : IWeChatCommonMessage {
        string MediaId {
            get;
        }

        string Format {
            get;
        }

        string Recognition {
            get;
        }
    }
}
