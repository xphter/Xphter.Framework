using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WeChatEventHandlerAttribute : Attribute {
        public WeChatEventHandlerAttribute(string eventType) {
            this.EventType = eventType;
        }

        public string EventType {
            get;
            private set;
        }
    }
}
