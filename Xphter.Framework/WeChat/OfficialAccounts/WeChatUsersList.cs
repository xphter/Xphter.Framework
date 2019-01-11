using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatUsersList : IWeChatUsersList {
        public WeChatUsersList() {
            this.OpenIDList = new List<string>();
        }

        public List<string> OpenIDList {
            get;
            private set;
        }

        #region IWeChatUsersList Members

        int IWeChatUsersList.TotalCount {
            get {
                return this.OpenIDList.Count;
            }
        }

        IEnumerable<string> IWeChatUsersList.OpenIDList {
            get {
                return this.OpenIDList;
            }
        }

        #endregion
    }
}
