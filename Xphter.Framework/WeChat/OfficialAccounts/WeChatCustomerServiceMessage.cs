using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Web.JavaScript;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public abstract class WeChatCustomerServiceMessage : IWeChatMessageResult {
        public const int AVAILABLE_INTERVAL_SECONDS = 48 * 3600; // 48 hours

        [JsonPropertyName("touser")]
        public virtual string ToUserName {
            get;
            set;
        }

        [JsonPropertyName("msgtype")]
        public abstract string MsgType {
            get;
        }

        #region IWeChatMessageResult Members

        [JsonIgnore]
        public virtual string ContentType {
            get {
                return "application/json";
            }
        }

        [JsonIgnore]
        public virtual string ResultContent {
            get {
                return new JsonSerializer().Serialize(this);
            }
        }

        #endregion
    }
}
