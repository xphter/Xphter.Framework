using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public static class WeChatEventTypes {
        public const string SUBSCRIBE = "subscribe";
        public const string UNSUBSCRIBE = "unsubscribe";
        public const string SCAN = "SCAN";
        public const string LOCATION = "LOCATION";
        public const string CLICK = "CLICK";
        public const string VIEW = "VIEW";
        public const string SCANCODE_PUSH = "scancode_push";
        public const string SCANCODE_WAITMSG = "scancode_waitmsg";
        public const string PIC_SYSPHOTO = "pic_sysphoto";
        public const string PIC_PHOTO_OR_ALBUM = "pic_photo_or_album";
        public const string PIC_WEIXIN = "pic_weixin";
        public const string LOCATION_SELECT = "location_select";
    }
}
