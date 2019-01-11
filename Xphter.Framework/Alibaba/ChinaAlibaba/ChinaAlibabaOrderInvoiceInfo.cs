using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderInvoiceInfo : IChinaAlibabaOrderInvoiceInfo {
        [ObfuscationAttribute]
        public long id;

        [ObfuscationAttribute]
        public long orderId;

        [ObfuscationAttribute]
        public string invoiceCompanyName;

        [ObfuscationAttribute]
        public string taxpayerIdentify;

        [ObfuscationAttribute]
        public string bankAndAccount;

        public override string ToString() {
            return this.invoiceCompanyName;
        }

        #region IChinaAlibabaOrderInvoiceInfo Members

        long IChinaAlibabaOrderInvoiceInfo.ID {
            get {
                return this.id;
            }
        }

        long IChinaAlibabaOrderInvoiceInfo.OrderID {
            get {
                return this.orderId;
            }
        }

        string IChinaAlibabaOrderInvoiceInfo.InvoiceCompanyName {
            get {
                return this.invoiceCompanyName;
            }
        }

        string IChinaAlibabaOrderInvoiceInfo.TaxpayerIdentify {
            get {
                return this.taxpayerIdentify;
            }
        }

        string IChinaAlibabaOrderInvoiceInfo.BankAndAccount {
            get {
                return this.bankAndAccount;
            }
        }

        #endregion
    }
}
