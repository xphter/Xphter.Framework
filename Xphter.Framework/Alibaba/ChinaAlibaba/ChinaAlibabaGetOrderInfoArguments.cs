using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public class ChinaAlibabaGetOrderInfoArguments : IChinaAlibabaApiArguments {
        [AlibabaArgument("id")]
        public long ID {
            get;
            set;
        }

        [AlibabaArgument("needOrderEntries")]
        public bool? NeedOrderEntries {
            get;
            set;
        }

        [AlibabaArgument("needInvoiceInfo")]
        public bool? NeedInvoiceInfo {
            get;
            set;
        }

        [AlibabaArgument("needOrderMemoList")]
        public bool? NeedOrderMemoList {
            get;
            set;
        }

        [AlibabaArgument("needLogisticsOrderList")]
        public bool? NeedLogisticsOrderList {
            get;
            set;
        }
    }
}
