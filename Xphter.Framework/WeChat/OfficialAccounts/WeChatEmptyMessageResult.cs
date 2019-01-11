using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatEmptyMessageResult : IWeChatMessageResult {
        #region Singleton

        private WeChatEmptyMessageResult() {
        }

        private static class InstanceContainer {
            static InstanceContainer() {
                Instance = new WeChatEmptyMessageResult();
            }

            public static WeChatEmptyMessageResult Instance;
        }

        public static IWeChatMessageResult Instance {
            get {
                return InstanceContainer.Instance;
            }
        }

        #endregion

        #region IWeChatMessageResult Members

        public string ContentType {
            get {
                return "text/html";
            }
        }

        public string ResultContent {
            get {
                return string.Empty;
            }
        }

        #endregion
    }
}
