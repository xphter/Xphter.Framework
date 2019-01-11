using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatMessageHandler {
        WeChatMessageHandlerPriority Priority {
            get;
        }

        IAsyncResult BeginProcessMessage(IWeChatMessage message, IOfficialAccountService service, AsyncCallback callback, object userState);

        IWeChatMessageResult EndProcessMessage(IAsyncResult asyncResult);
    }

    public enum WeChatMessageHandlerPriority {
        [Description("禁用")]
        Disabled = 0x00,

        [Description("最低优先级")]
        None = 0x01,

        [Description("低优先级")]
        Low = 0x04,

        [Description("普通优先级")]
        Normal = 0x08,

        [Description("高优先级")]
        High = 0x10,

        [Description("最高优先级")]
        Ultimate = 0x80,
    }
}
