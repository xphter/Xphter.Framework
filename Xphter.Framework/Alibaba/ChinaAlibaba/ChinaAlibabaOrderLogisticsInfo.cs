using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderLogisticsInfo : IChinaAlibabaOrderLogisticsInfo {
        [ObfuscationAttribute]
        public string type;

        [ObfuscationAttribute]
        public long id;

        [ObfuscationAttribute]
        public string status;

        [ObfuscationAttribute]
        public string gmtModified;

        [ObfuscationAttribute]
        public string gmtCreate;

        [ObfuscationAttribute]
        public long orderId;

        [ObfuscationAttribute]
        public long carriage;

        [ObfuscationAttribute]
        public string toPost;

        [ObfuscationAttribute]
        public string toArea;

        [ObfuscationAttribute]
        public long logisticsCompanyId;

        [ObfuscationAttribute]
        public string logisticsOrderNo;

        [ObfuscationAttribute]
        public string fromProvince;

        [ObfuscationAttribute]
        public string fromCity;

        [ObfuscationAttribute]
        public string fromArea;

        [ObfuscationAttribute]
        public string fromAddress;

        [ObfuscationAttribute]
        public string fromPhone;

        [ObfuscationAttribute]
        public string fromMobile;

        [ObfuscationAttribute]
        public string fromPost;

        [ObfuscationAttribute]
        public string toProvince;

        [ObfuscationAttribute]
        public string toContact;

        [ObfuscationAttribute]
        public string logisticsBillNo;

        [ObfuscationAttribute]
        public string gmtSend;

        public ChinaAlibabaOrderLogisticsCompanyInfo logisticsCompany;

        public override string ToString() {
            return this.toContact;
        }

        #region IChinaAlibabaOrderLogisticsInfo Members

        string IChinaAlibabaOrderLogisticsInfo.Type {
            get {
                return this.type;
            }
        }

        long IChinaAlibabaOrderLogisticsInfo.ID {
            get {
                return this.id;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.Status {
            get {
                return this.status;
            }
        }

        DateTime? IChinaAlibabaOrderLogisticsInfo.ModifiedTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtModified) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtModified) : null;
            }
        }

        DateTime? IChinaAlibabaOrderLogisticsInfo.CreateTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtCreate) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtCreate) : null;
            }
        }

        long IChinaAlibabaOrderLogisticsInfo.OrderId {
            get {
                return this.orderId;
            }
        }

        long IChinaAlibabaOrderLogisticsInfo.Carriage {
            get {
                return this.carriage;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.ToPost {
            get {
                return this.toPost;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.ToArea {
            get {
                return this.toArea; 
            }
        }

        long IChinaAlibabaOrderLogisticsInfo.LogisticsCompanyId {
            get {
                return this.logisticsCompanyId;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.LogisticsOrderNo {
            get {
                return this.logisticsOrderNo;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromProvince {
            get {
                return this.fromProvince;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromCity {
            get {
                return this.fromCity;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromArea {
            get {
                return this.fromArea;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromAddress {
            get {
                return this.fromAddress;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromPhone {
            get {
                return this.fromPhone;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromMobile {
            get {
                return this.fromMobile;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.FromPost {
            get {
                return this.fromPost;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.ToProvince {
            get {
                return this.toProvince;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.ToContact {
            get {
                return this.toContact;
            }
        }

        string IChinaAlibabaOrderLogisticsInfo.LogisticsBillNo {
            get {
                return this.logisticsBillNo;
            }
        }

        DateTime? IChinaAlibabaOrderLogisticsInfo.SendTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtSend) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtSend) : null;
            }
        }

        IChinaAlibabaOrderLogisticsCompanyInfo IChinaAlibabaOrderLogisticsInfo.LogisticsCompany {
            get {
                return this.logisticsCompany;
            }
        }

        #endregion
    }
}
