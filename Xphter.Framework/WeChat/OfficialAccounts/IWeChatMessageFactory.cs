﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatMessageFactory {
        IWeChatMessage GetMessage(string data, IOfficialAccountService service);
    }
}
