using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatMessageHandlerFactory {
        IWeChatMessageHandler GetHandler(IWeChatMessage message, IOfficialAccountService service);
    }
}
