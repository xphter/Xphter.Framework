using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderInvoiceInfo {
        long ID {
            get;
        }

        long OrderID {
            get;
        }

        string InvoiceCompanyName {
            get;
        }

        string TaxpayerIdentify {
            get;
        }

        string BankAndAccount {
            get;
        }
    }
}
