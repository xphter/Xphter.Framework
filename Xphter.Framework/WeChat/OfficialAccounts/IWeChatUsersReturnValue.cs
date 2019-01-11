using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatUsersReturnValue : IWeChatReturnValue {
        int TotalCount {
            get;
        }

        int CurrentCount {
            get;
        }

        IEnumerable<string> OpenIDList {
            get;
        }

        string NextOpenID {
            get;
        }
    }
}
