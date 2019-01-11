using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderInfo {
        ChinaAlibabaOrderStatus OrderStatus {
            get;
        }

        DateTime? ModifiedTime {
            get;
        }

        DateTime? CreateTime {
            get;
        }

        ChinaAlibabaOrderRefundStatus? RefundStatus {
            get;
        }

        ChinaAlibabaOrderRateStatus BuyerRateStatus {
            get;
        }

        ChinaAlibabaOrderRateStatus SellerRateStatus {
            get;
        }

        DateTime? PaymentTime {
            get;
        }

        DateTime? GoodsSendTime {
            get;
        }

        DateTime? ConfirmedTime {
            get;
        }

        long Discount {
            get;
        }

        long Carriage {
            get;
        }

        long RefundPayment {
            get;
        }

        long SumPayment {
            get;
        }

        string CloseReason {
            get;
        }

        string BuyerFeedback {
            get;
        }

        string ToPost {
            get;
        }

        string ToArea {
            get;
        }

        string AlipayTradeId {
            get;
        }

        string SellerCompanyName {
            get;
        }

        string SellerEmail {
            get;
        }

        string BuyerCompanyName {
            get;
        }

        string BuyerEmail {
            get;
        }

        long SumProductPayment {
            get;
        }

        IEnumerable<IChinaAlibabaOrderEntryInfo> EntryList {
            get;
        }

        IEnumerable<IChinaAlibabaOrderLogisticsInfo> LogisticsList {
            get;
        }

        IEnumerable<IChinaAlibabaOrderMemoInfo> MemoList {
            get;
        }

        IChinaAlibabaOrderInvoiceInfo InvoiceInfo {
            get;
        }

        bool IsStepPayAll {
            get;
        }

        ChinaAlibabaOrderPayStatus PayStatus {
            get;
        }

        ChinaAlibabaOrderLogisticsStatus LogisticsStatus {
            get;
        }

        string StepAgreementPath {
            get;
        }

        bool IsCodAudit {
            get;
        }

        long CodFee {
            get;
        }

        long CodBuyerFee {
            get;
        }

        long CodSellerFee {
            get;
        }

        long CodActualFee {
            get;
        }

        DateTime? SignTime {
            get;
        }

        string CodFeeDividend {
            get;
        }

        long CodInitFee {
            get;
        }

        long CodBuyerInitFee {
            get;
        }

        IChinaAlibabaOrderMemoInfo BuyerOrderMemo {
            get;
        }

        IChinaAlibabaOrderMemoInfo SellerOrderMemo {
            get;
        }

        long ID {
            get;
        }

        string BuyerMemberId {
            get;
        }

        string SellerMemberId {
            get;
        }

        long SellerUserId {
            get;
        }

        long BuyerUserId {
            get;
        }

        string BuyerAlipayId {
            get;
        }

        string SellerAlipayId {
            get;
        }

        string SellerLoginId {
            get;
        }

        string BuyerLoginId {
            get;
        }

        ChinaAlibabaOrderTradeType TradeType {
            get;
        }
    }

    public enum ChinaAlibabaOrderTradeType {
        [Description("统一交易流程")]
        UNIFY = 6,

        [Description("分阶段交易")]
        STEPPAY = 7,

        [Description("货到付款交易")]
        COD = 8,

        [Description("信用凭证支付交易")]
        CERTIFICATE = 9,
    }

    public enum ChinaAlibabaOrderStatus {
        [Description("交易关闭")]
        CANCEL,

        [Description("交易成功")]
        SUCCESS,

        [Description("等待买家付款")]
        WAIT_BUYER_PAY,

        [Description("等待卖家发货")]
        WAIT_SELLER_SEND,

        [Description("等待买家确认收货")]
        WAIT_BUYER_RECEIVE,

        [Description("分阶段等待卖家操作")]
        WAIT_SELLER_ACT,

        [Description("分阶段等待买家确认卖家操作")]
        WAIT_BUYER_CONFIRM_ACTION,

        [Description("分阶段等待卖家推进")]
        WAIT_SELLER_PUSH,

        [Description("等待物流公司揽件COD")]
        WAIT_LOGISTICS_TAKE_IN,

        [Description("等待买家签收COD")]
        WAIT_BUYER_SIGN,

        [Description("买家已签收COD")]
        SIGN_IN_SUCCESS,

        [Description("签收失败COD")]
        SIGN_IN_FAILED,
    }

    public enum ChinaAlibabaOrderRefundStatus {
        [Description("等待卖家同意退款协议")]
        WAIT_SELLER_AGREE,

        [Description("退款成功")]
        REFUND_SUCCESS,

        [Description("退款关闭")]
        REFUND_CLOSED,

        [Description("等待买家修改")]
        WAIT_BUYER_MODIFY,

        [Description("等待买家退货")]
        WAIT_BUYER_SEND,

        [Description("等待卖家确认收货")]
        WAIT_SELLER_RECEIVE,
    }

    public enum ChinaAlibabaOrderRateStatus {
        [Description("已评价")]
        YES = 4,

        [Description("未评价")]
        NO = 5,

        [Description("不需要评价")]
        NOT_REQUIRED = 6,
    }

    public enum ChinaAlibabaOrderPayStatus {
        [Description("等待买家付款")]
        WAIT_BUYER_PAY = 1,

        [Description("买家已付款")]
        BUYER_PAID = 2,

        [Description("交易关闭")]
        CANCEL = 4,

        [Description("交易成功")]
        SUCCESS = 6,

        [Description("交易被系统关闭")]
        SYSTEM_CANCEL = 8,
    }

    public enum ChinaAlibabaOrderLogisticsStatus {
        [Description("未创建物流订单")]
        UNCREATED = 8,

        [Description("未发货")]
        UNSENT = 1,

        [Description("已发货")]
        SENT = 2,

        [Description("已收货，交易成功")]
        SUCCESS = 3,

        [Description("已经退货，交易失败")]
        RETURNED = 4,

        [Description("部分发货，交易成功")]
        PARTIAL_SENT = 5,
    }
}
