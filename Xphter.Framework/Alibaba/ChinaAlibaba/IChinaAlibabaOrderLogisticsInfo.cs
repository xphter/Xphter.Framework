using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderLogisticsInfo {
        string Type {
            get;
        }

        long ID {
            get;
        }

        string Status {
            get;
        }

        DateTime? ModifiedTime {
            get;
        }

        DateTime? CreateTime {
            get;
        }

        long OrderId {
            get;
        }

        long Carriage {
            get;
        }

        string ToPost {
            get;
        }

        string ToArea {
            get;
        }

        long LogisticsCompanyId {
            get;
        }

        string LogisticsOrderNo {
            get;
        }

        string FromProvince {
            get;
        }

        string FromCity {
            get;
        }

        string FromArea {
            get;
        }

        string FromAddress {
            get;
        }

        string FromPhone {
            get;
        }

        string FromMobile {
            get;
        }

        string FromPost {
            get;
        }

        string ToProvince {
            get;
        }

        string ToContact {
            get;
        }

        string LogisticsBillNo {
            get;
        }

        DateTime? SendTime {
            get;
        }

        IChinaAlibabaOrderLogisticsCompanyInfo LogisticsCompany {
            get;
        }
    }
}
