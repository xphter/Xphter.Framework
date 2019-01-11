using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatMenuItem : IWeChatMenuItem {
        public WeChatMenuItem() {
            this.Type = WeChatMenuItemType.Click;
            this.ChildItems = new List<WeChatMenuItem>();
        }

        public List<WeChatMenuItem> ChildItems {
            get;
            private set;
        }

        public override string ToString() {
            return this.Name;
        }

        #region IWeChatMenuItem Members

        public WeChatMenuItemType Type {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public string Value {
            get;
            set;
        }

        IEnumerable<IWeChatMenuItem> IWeChatMenuItem.ChildItems {
            get {
                return this.ChildItems;
            }
        }

        #endregion
    }
}
