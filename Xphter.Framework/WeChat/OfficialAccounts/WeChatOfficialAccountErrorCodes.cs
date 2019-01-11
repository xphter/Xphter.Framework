using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public static class WeChatOfficialAccountErrorCodes {
        public const int SUCCESS = 0;

        public const int INVALID_ACCESS_TOKEN = 40001;

        public const int ILLEGAL_ACCESS_TOKEN = 40014;

        public const int MISSING_ACCESS_TOKEN = 41001;

        public const int ACCESS_TOKEN_TIMEOUT = 42001;
    }
}
