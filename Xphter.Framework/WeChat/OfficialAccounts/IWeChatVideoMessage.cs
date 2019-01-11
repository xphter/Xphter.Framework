﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatVideoMessage : IWeChatCommonMessage {
        string MediaId {
            get;
        }

        string ThumbMediaId {
            get;
        }
    }
}
