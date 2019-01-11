using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IWeChatMenuItem {
        WeChatMenuItemType Type {
            get;
        }

        string Name {
            get;
        }

        string Value {
            get;
        }

        IEnumerable<IWeChatMenuItem> ChildItems {
            get;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class WeChatMenuItemTypeAttribute : Attribute {
        public WeChatMenuItemTypeAttribute(string menuItemType) {
            this.MenuItemType = menuItemType;
        }

        public string MenuItemType {
            get;
            private set;
        }
    }

    public enum WeChatMenuItemType {
        [WeChatMenuItemType("click")]
        [Description("点击菜单")]
        Click,

        [WeChatMenuItemType("view")]
        [Description("链接菜单")]
        View,
    }
}
