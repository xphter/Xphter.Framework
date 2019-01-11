using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderMemoInfo : IChinaAlibabaOrderMemoInfo {
        [ObfuscationAttribute]
        public long id;

        [ObfuscationAttribute]
        public string gmtModified;

        [ObfuscationAttribute]
        public string gmtCreate;

        [ObfuscationAttribute]
        public long orderId;

        [ObfuscationAttribute]
        public string memberId;

        [ObfuscationAttribute]
        public string remark;

        [ObfuscationAttribute]
        public string remarkIcon;

        public override string ToString() {
            return this.remark;
        }

        #region IChinaAlibabaOrderMemoInfo Members

        long IChinaAlibabaOrderMemoInfo.ID {
            get {
                return this.id;
            }
        }

        DateTime? IChinaAlibabaOrderMemoInfo.ModifiedTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtModified) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtModified) : null;
            }
        }

        DateTime? IChinaAlibabaOrderMemoInfo.CreateTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtCreate) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtCreate) : null;
            }
        }

        long IChinaAlibabaOrderMemoInfo.OrderID {
            get {
                return this.orderId;
            }
        }

        string IChinaAlibabaOrderMemoInfo.MemberID {
            get {
                return this.memberId;
            }
        }

        string IChinaAlibabaOrderMemoInfo.RemarkText {
            get {
                return this.remark;
            }
        }

        string IChinaAlibabaOrderMemoInfo.RemarkIcon {
            get {
                return this.remarkIcon;
            }
        }

        #endregion
    }
}
