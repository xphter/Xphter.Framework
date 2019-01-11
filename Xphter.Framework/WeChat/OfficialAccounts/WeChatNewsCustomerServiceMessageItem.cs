using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Web.JavaScript;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatNewsCustomerServiceMessageItem {
        [JsonPropertyName("title")]
        public string Title {
            get;
            set;
        }

        [JsonPropertyName("description")]
        public string Description {
            get;
            set;
        }

        [JsonPropertyName("picurl")]
        public string PicUrl {
            get;
            set;
        }

        [JsonPropertyName("url")]
        public string Url {
            get;
            set;
        }
    }
}
