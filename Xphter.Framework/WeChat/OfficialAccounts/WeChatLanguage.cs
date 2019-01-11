using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public enum WeChatLanguage {
        [Description("")]
        Uknown = 0x00,

        [Description("zh_CN")]
        SimplifiedChinese = 0x01,

        [Description("zh_TW")]
        TraditionalChinese = 0x08,

        [Description("en")]
        English = 0x80,
    }
}
