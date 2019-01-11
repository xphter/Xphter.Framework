using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatLocationMessage : IWeChatCommonMessage {
        float Location_X {
            get;
        }

        string Location_Y {
            get;
        }

        int Scale {
            get;
        }

        string Label {
            get;
        }
    }
}
