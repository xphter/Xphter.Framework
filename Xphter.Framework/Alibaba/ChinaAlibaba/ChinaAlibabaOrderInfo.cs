using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderInfo : IChinaAlibabaOrderInfo {
        [ObfuscationAttribute]
        public string status;

        [ObfuscationAttribute]
        public string gmtModified;

        [ObfuscationAttribute]
        public string gmtCreate;

        [ObfuscationAttribute]
        public string refundStatus;

        [ObfuscationAttribute]
        public int buyerRateStatus;

        [ObfuscationAttribute]
        public int sellerRateStatus;

        [ObfuscationAttribute]
        public string gmtPayment;

        [ObfuscationAttribute]
        public string gmtGoodsSend;

        [ObfuscationAttribute]
        public string gmtConfirmed;

        [ObfuscationAttribute]
        public long discount;

        [ObfuscationAttribute]
        public long carriage;

        [ObfuscationAttribute]
        public long refundPayment;

        [ObfuscationAttribute]
        public long sumPayment;

        [ObfuscationAttribute]
        public string closeReason;

        [ObfuscationAttribute]
        public string buyerFeedback;

        [ObfuscationAttribute]
        public string toPost;

        [ObfuscationAttribute]
        public string toArea;

        [ObfuscationAttribute]
        public string alipayTradeId;

        [ObfuscationAttribute]
        public string sellerCompanyName;

        [ObfuscationAttribute]
        public string sellerEmail;

        [ObfuscationAttribute]
        public string buyerCompanyName;

        [ObfuscationAttribute]
        public string buyerEmail;

        [ObfuscationAttribute]
        public long sumProductPayment;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderEntryInfo[] orderEntries;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderLogisticsInfo[] logisticsOrderList;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderMemoInfo[] orderMemoList;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderInvoiceInfo orderInvoiceModel;

        [ObfuscationAttribute]
        public bool stepPayAll;

        [ObfuscationAttribute]
        public int payStatus;

        [ObfuscationAttribute]
        public int logisticsStatus;

        [ObfuscationAttribute]
        public string stepAgreementPath;

        [ObfuscationAttribute]
        public bool codAudit;

        [ObfuscationAttribute]
        public long codFee;

        [ObfuscationAttribute]
        public long codBuyerFee;

        [ObfuscationAttribute]
        public long codSellerFee;

        [ObfuscationAttribute]
        public long codActualFee;

        [ObfuscationAttribute]
        public string gmtSign;

        [ObfuscationAttribute]
        public string codFeeDividend;

        [ObfuscationAttribute]
        public long codInitFee;

        [ObfuscationAttribute]
        public long codBuyerInitFee;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderMemoInfo buyerOrderMemo;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderMemoInfo sellerOrderMemo;

        [ObfuscationAttribute]
        public long id;

        [ObfuscationAttribute]
        public string buyerMemberId;

        [ObfuscationAttribute]
        public string sellerMemberId;

        [ObfuscationAttribute]
        public long sellerUserId;

        [ObfuscationAttribute]
        public long buyerUserId;

        [ObfuscationAttribute]
        public string buyerAlipayId;

        [ObfuscationAttribute]
        public string sellerAlipayId;

        [ObfuscationAttribute]
        public string sellerLoginId;

        [ObfuscationAttribute]
        public string buyerLoginId;

        [ObfuscationAttribute]
        public string tradeTypeStr;

        public override int GetHashCode() {
            return this.id.GetHashCode();
        }

        public override bool Equals(object obj) {
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is ChinaAlibabaOrderInfo)) {
                return false;
            }

            return this.id == ((ChinaAlibabaOrderInfo) obj).id;
        }

        public override string ToString() {
            return this.buyerCompanyName ?? this.toArea;
        }

        #region IChinaAlibabaOrderInfo Members

        ChinaAlibabaOrderStatus IChinaAlibabaOrderInfo.OrderStatus {
            get {
                return (ChinaAlibabaOrderStatus) Enum.Parse(typeof(ChinaAlibabaOrderStatus), this.status);
            }
        }

        DateTime? IChinaAlibabaOrderInfo.ModifiedTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtModified) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtModified) : null;
            }
        }

        DateTime? IChinaAlibabaOrderInfo.CreateTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtCreate) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtCreate) : null;
            }
        }

        ChinaAlibabaOrderRefundStatus? IChinaAlibabaOrderInfo.RefundStatus {
            get {
                return !string.IsNullOrWhiteSpace(this.refundStatus) ? (ChinaAlibabaOrderRefundStatus?) Enum.Parse(typeof(ChinaAlibabaOrderRefundStatus), this.refundStatus) : null;
            }
        }

        ChinaAlibabaOrderRateStatus IChinaAlibabaOrderInfo.BuyerRateStatus {
            get {
                return (ChinaAlibabaOrderRateStatus) this.buyerRateStatus;
            }
        }

        ChinaAlibabaOrderRateStatus IChinaAlibabaOrderInfo.SellerRateStatus {
            get {
                return (ChinaAlibabaOrderRateStatus) this.sellerRateStatus;
            }
        }

        DateTime? IChinaAlibabaOrderInfo.PaymentTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtPayment) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtPayment) : null;
            }
        }

        DateTime? IChinaAlibabaOrderInfo.GoodsSendTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtGoodsSend) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtGoodsSend) : null;
            }
        }

        DateTime? IChinaAlibabaOrderInfo.ConfirmedTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtConfirmed) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtConfirmed) : null;
            }
        }

        long IChinaAlibabaOrderInfo.Discount {
            get {
                return this.discount;
            }
        }

        long IChinaAlibabaOrderInfo.Carriage {
            get {
                return this.carriage;
            }
        }

        long IChinaAlibabaOrderInfo.RefundPayment {
            get {
                return this.refundPayment;
            }
        }

        long IChinaAlibabaOrderInfo.SumPayment {
            get {
                return this.sumPayment;
            }
        }

        string IChinaAlibabaOrderInfo.CloseReason {
            get {
                return this.closeReason;
            }
        }

        string IChinaAlibabaOrderInfo.BuyerFeedback {
            get {
                return this.buyerFeedback;
            }
        }

        string IChinaAlibabaOrderInfo.ToPost {
            get {
                return this.toPost; 
            }
        }

        string IChinaAlibabaOrderInfo.ToArea {
            get {
                return this.toArea;
            }
        }

        string IChinaAlibabaOrderInfo.AlipayTradeId {
            get {
                return this.alipayTradeId;
            }
        }

        string IChinaAlibabaOrderInfo.SellerCompanyName {
            get {
                return this.sellerCompanyName;
            }
        }

        string IChinaAlibabaOrderInfo.SellerEmail {
            get {
                return this.sellerEmail;
            }
        }

        string IChinaAlibabaOrderInfo.BuyerCompanyName {
            get {
                return this.buyerCompanyName;
            }
        }

        string IChinaAlibabaOrderInfo.BuyerEmail {
            get {
                return this.buyerEmail;
            }
        }

        long IChinaAlibabaOrderInfo.SumProductPayment {
            get {
                return this.sumProductPayment;
            }
        }

        IEnumerable<IChinaAlibabaOrderEntryInfo> IChinaAlibabaOrderInfo.EntryList {
            get {
                return this.orderEntries;
            }
        }

        IEnumerable<IChinaAlibabaOrderLogisticsInfo> IChinaAlibabaOrderInfo.LogisticsList {
            get {
                return this.logisticsOrderList;
            }
        }

        IEnumerable<IChinaAlibabaOrderMemoInfo> IChinaAlibabaOrderInfo.MemoList {
            get {
                return this.orderMemoList;
            }
        }

        IChinaAlibabaOrderInvoiceInfo IChinaAlibabaOrderInfo.InvoiceInfo {
            get {
                return this.orderInvoiceModel;
            }
        }

        bool IChinaAlibabaOrderInfo.IsStepPayAll {
            get {
                return this.stepPayAll;
            }
        }

        ChinaAlibabaOrderPayStatus IChinaAlibabaOrderInfo.PayStatus {
            get {
                return (ChinaAlibabaOrderPayStatus) this.payStatus;
            }
        }

        ChinaAlibabaOrderLogisticsStatus IChinaAlibabaOrderInfo.LogisticsStatus {
            get {
                return (ChinaAlibabaOrderLogisticsStatus) this.logisticsStatus;
            }
        }

        string IChinaAlibabaOrderInfo.StepAgreementPath {
            get {
                return this.stepAgreementPath; 
            }
        }

        bool IChinaAlibabaOrderInfo.IsCodAudit {
            get {
                return this.codAudit; 
            }
        }

        long IChinaAlibabaOrderInfo.CodFee {
            get {
                return this.codFee;
            }
        }

        long IChinaAlibabaOrderInfo.CodBuyerFee {
            get {
                return this.codBuyerFee;
            }
        }

        long IChinaAlibabaOrderInfo.CodSellerFee {
            get {
                return this.codSellerFee;
            }
        }

        long IChinaAlibabaOrderInfo.CodActualFee {
            get {
                return this.codActualFee;
            }
        }

        DateTime? IChinaAlibabaOrderInfo.SignTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtSign) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtSign) : null;
            }
        }

        string IChinaAlibabaOrderInfo.CodFeeDividend {
            get {
                return this.codFeeDividend;
            }
        }

        long IChinaAlibabaOrderInfo.CodInitFee {
            get {
                return this.codInitFee;
            }
        }

        long IChinaAlibabaOrderInfo.CodBuyerInitFee {
            get {
                return this.codBuyerInitFee;
            }
        }

        IChinaAlibabaOrderMemoInfo IChinaAlibabaOrderInfo.BuyerOrderMemo {
            get {
                return this.buyerOrderMemo;
            }
        }

        IChinaAlibabaOrderMemoInfo IChinaAlibabaOrderInfo.SellerOrderMemo {
            get {
                return this.sellerOrderMemo;
            }
        }

        long IChinaAlibabaOrderInfo.ID {
            get {
                return this.id;
            }
        }

        string IChinaAlibabaOrderInfo.BuyerMemberId {
            get {
                return this.buyerMemberId;
            }
        }

        string IChinaAlibabaOrderInfo.SellerMemberId {
            get {
                return this.sellerMemberId;
            }
        }

        long IChinaAlibabaOrderInfo.SellerUserId {
            get {
                return this.sellerUserId;
            }
        }

        long IChinaAlibabaOrderInfo.BuyerUserId {
            get {
                return this.buyerUserId;
            }
        }

        string IChinaAlibabaOrderInfo.BuyerAlipayId {
            get {
                return this.buyerAlipayId;
            }
        }

        string IChinaAlibabaOrderInfo.SellerAlipayId {
            get {
                return this.sellerAlipayId;
            }
        }

        string IChinaAlibabaOrderInfo.SellerLoginId {
            get {
                return this.sellerLoginId;
            }
        }

        string IChinaAlibabaOrderInfo.BuyerLoginId {
            get {
                return this.buyerLoginId;
            }
        }

        ChinaAlibabaOrderTradeType IChinaAlibabaOrderInfo.TradeType {
            get {
                return (ChinaAlibabaOrderTradeType) Enum.Parse(typeof(ChinaAlibabaOrderTradeType), this.tradeTypeStr);
            }
        }

        #endregion
    }
}
