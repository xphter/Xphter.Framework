using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Alibaba;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public class ChinaAlibabaGetOrderListArguments : IChinaAlibabaApiArguments {
        [AlibabaArgument("buyerMemberId")]
        public String BuyerMemberId {
            get;
            set;
        }

        [AlibabaArgument("sellerMemberId")]
        public String SellerMemberId {
            get;
            set;
        }

        [AlibabaArgument("his")]
        public bool? IsHistory {
            get;
            set;
        }

        [AlibabaArgument("createStartTime")]
        public DateTime? CreateStartTime {
            get;
            set;
        }

        [AlibabaArgument("createEndTime")]
        public DateTime? CreateEndTime {
            get;
            set;
        }

        [AlibabaArgument("productName")]
        public String ProductName {
            get;
            set;
        }

        [AlibabaArgument("tradeTypeEnum")]
        public ChinaAlibabaOrderTradeType? TradeType {
            get;
            set;
        }

        [AlibabaArgument("orderStatusEnum")]
        public ChinaAlibabaOrderStatus? OrderStatus {
            get;
            set;
        }

        [AlibabaArgument("pageSize")]
        public int? PageSize {
            get;
            set;
        }

        [AlibabaArgument("page")]
        public int? PageNumber {
            get;
            set;
        }

        [AlibabaArgument("modifyStartTime")]
        public DateTime? ModifyStartTime {
            get;
            set;
        }

        [AlibabaArgument("modifyEndTime")]
        public DateTime? ModifyEndTime {
            get;
            set;
        }

        [AlibabaArgument("sellerRateStatus", typeof(EnumValueAlibabaArgumentFormValueProvider))]
        public int? SellerRateStatus {
            get;
            set;
        }

        [AlibabaArgument("buyerRateStatus", typeof(EnumValueAlibabaArgumentFormValueProvider))]
        public int? BuyerRateStatus {
            get;
            set;
        }

        [AlibabaArgument("refundStatus")]
        public ChinaAlibabaOrderRefundStatus? RefundStatus {
            get;
            set;
        }
    }
}
