using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderEntryInfo : IChinaAlibabaOrderEntryInfo {
        [ObfuscationAttribute]
        public long id;

        [ObfuscationAttribute]
        public string productName;

        [ObfuscationAttribute]
        public long sourceId;

        [ObfuscationAttribute]
        public decimal quantity;

        [ObfuscationAttribute]
        public string unit;

        [ObfuscationAttribute]
        public string productPic;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderSnapshotImageInfo[] snapshotImages;

        [ObfuscationAttribute]
        public string orderSourceType;

        [ObfuscationAttribute]
        public long price;

        [ObfuscationAttribute]
        public string currencyCode;

        [ObfuscationAttribute]
        public long discountPrice;

        [ObfuscationAttribute]
        public long amount;

        [ObfuscationAttribute]
        public decimal discount;

        [ObfuscationAttribute]
        public string coupon;

        [ObfuscationAttribute]
        public long entryDiscount;

        [ObfuscationAttribute]
        public long entryCouponAmount;

        [ObfuscationAttribute]
        public long unitPrice;

        [ObfuscationAttribute]
        public long promotionsFee;

        [ObfuscationAttribute]
        public long actualPayFee;

        [ObfuscationAttribute]
        public string snapshotId;

        [ObfuscationAttribute]
        public string entryStatusStr;

        [ObfuscationAttribute]
        public string entryRefundStatus;

        [ObfuscationAttribute]
        public string entryRefundStatusForAs;

        [ObfuscationAttribute]
        public int entryPayStatus;

        [ObfuscationAttribute]
        public string refundId;

        [ObfuscationAttribute]
        public string refundIdForAs;

        [ObfuscationAttribute]
        public long categoryId;

        [ObfuscationAttribute]
        public long orderId;

        [ObfuscationAttribute]
        public string specId;

        [ObfuscationAttribute]
        public bool guaranteeSupport;

        [ObfuscationAttribute]
        public string industrySecurityCodes;

        [ObfuscationAttribute]
        public bool buyerSecuritySupport;

        [ObfuscationAttribute]
        public int buyerRateStatus;

        [ObfuscationAttribute]
        public int sellerRateStatus;

        [ObfuscationAttribute]
        public string closeReason;

        [ObfuscationAttribute]
        public string gmtCreate;

        [ObfuscationAttribute]
        public string gmtModified;

        [ObfuscationAttribute]
        public string gmtCompleted;

        [ObfuscationAttribute]
        public string remark;

        [ObfuscationAttribute]
        public int logisticsStatus;

        [ObfuscationAttribute]
        public string externalId;

        public override string ToString() {
            return this.productName;
        }

        #region IChinaAlibabaOrderEntryInfo Members

        long IChinaAlibabaOrderEntryInfo.ID {
            get {
                return this.id;
            }
        }

        string IChinaAlibabaOrderEntryInfo.ProductName {
            get {
                return this.productName;
            }
        }

        long IChinaAlibabaOrderEntryInfo.SourceId {
            get {
                return this.sourceId;
            }
        }

        decimal IChinaAlibabaOrderEntryInfo.Quantity {
            get {
                return this.quantity;
            }
        }

        string IChinaAlibabaOrderEntryInfo.Unit {
            get {
                return this.unit;
            }
        }

        string IChinaAlibabaOrderEntryInfo.ProductPicUrl {
            get {
                return this.productPic;
            }
        }

        ChinaAlibabaOrderSnapshotImageInfo[] IChinaAlibabaOrderEntryInfo.SnapshotImages {
            get {
                return this.snapshotImages;
            }
        }

        string IChinaAlibabaOrderEntryInfo.OrderSourceType {
            get {
                return this.orderSourceType;
            }
        }

        long IChinaAlibabaOrderEntryInfo.Price {
            get {
                return this.price;
            }
        }

        string IChinaAlibabaOrderEntryInfo.CurrencyCode {
            get {
                return this.currencyCode;
            }
        }

        long IChinaAlibabaOrderEntryInfo.DiscountPrice {
            get {
                return this.discountPrice;
            }
        }

        long IChinaAlibabaOrderEntryInfo.Amount {
            get {
                return this.amount;
            }
        }

        decimal IChinaAlibabaOrderEntryInfo.Discount {
            get {
                return this.discount;
            }
        }

        string IChinaAlibabaOrderEntryInfo.Coupon {
            get {
                return this.coupon;
            }
        }

        long IChinaAlibabaOrderEntryInfo.EntryDiscount {
            get {
                return this.entryDiscount;
            }
        }

        long IChinaAlibabaOrderEntryInfo.EntryCouponAmount {
            get {
                return this.entryCouponAmount;
            }
        }

        long IChinaAlibabaOrderEntryInfo.UnitPrice {
            get {
                return this.unitPrice;
            }
        }

        long IChinaAlibabaOrderEntryInfo.PromotionsFee {
            get {
                return this.promotionsFee;
            }
        }

        long IChinaAlibabaOrderEntryInfo.ActualPayFee {
            get {
                return this.actualPayFee;
            }
        }

        string IChinaAlibabaOrderEntryInfo.SnapshotId {
            get {
                return this.snapshotId;
            }
        }

        ChinaAlibabaOrderStatus IChinaAlibabaOrderEntryInfo.EntryStatus {
            get {
                ChinaAlibabaOrderStatus value = 0;
                if(Enum.TryParse(this.entryStatusStr, out value)) {
                    return value;
                }

                switch(this.entryStatusStr) {
                    case "cancel":
                        return ChinaAlibabaOrderStatus.CANCEL;
                    case "success":
                        return ChinaAlibabaOrderStatus.SUCCESS;
                    case "waitbuyerpay":
                        return ChinaAlibabaOrderStatus.WAIT_BUYER_PAY;
                    case "waitsellersend":
                        return ChinaAlibabaOrderStatus.WAIT_SELLER_SEND;
                    case "waitbuyerreceive":
                        return ChinaAlibabaOrderStatus.WAIT_BUYER_RECEIVE;
                    case "waitselleract":
                        return ChinaAlibabaOrderStatus.WAIT_SELLER_ACT;
                    case "waitbuyerconfirmaction":
                        return ChinaAlibabaOrderStatus.WAIT_BUYER_CONFIRM_ACTION;
                    case "waitsellerpush":
                        return ChinaAlibabaOrderStatus.WAIT_SELLER_PUSH;
                    case "waitlogisticstakein":
                        return ChinaAlibabaOrderStatus.WAIT_LOGISTICS_TAKE_IN;
                    case "waitbuyersign":
                        return ChinaAlibabaOrderStatus.WAIT_BUYER_SIGN;
                    case "signinsuccess":
                        return ChinaAlibabaOrderStatus.SIGN_IN_SUCCESS;
                    case "signinfailed":
                        return ChinaAlibabaOrderStatus.SIGN_IN_FAILED;
                    default:
                        throw new InvalidCastException(string.Format("Invalid entryStatus: {0}", this.entryStatusStr));
                }
            }
        }

        ChinaAlibabaOrderRefundStatus? IChinaAlibabaOrderEntryInfo.EntryRefundStatus {
            get {
                if(string.IsNullOrWhiteSpace(this.entryRefundStatus)) {
                    return null;
                }

                ChinaAlibabaOrderRefundStatus value = 0;
                if(Enum.TryParse(this.entryRefundStatus, out value)) {
                    return value;
                }

                switch(this.entryRefundStatus) {
                    case "waitselleragree":
                        return ChinaAlibabaOrderRefundStatus.WAIT_SELLER_AGREE;
                    case "refundsuccess":
                        return ChinaAlibabaOrderRefundStatus.REFUND_SUCCESS;
                    case "refundclose":
                        return ChinaAlibabaOrderRefundStatus.REFUND_CLOSED;
                    case "waitbuyermodify":
                        return ChinaAlibabaOrderRefundStatus.WAIT_BUYER_MODIFY;
                    case "waitbuyersend":
                        return ChinaAlibabaOrderRefundStatus.WAIT_BUYER_SEND;
                    case "waitsellerreceive":
                        return ChinaAlibabaOrderRefundStatus.WAIT_SELLER_RECEIVE;
                    default:
                        throw new InvalidCastException(string.Format("Invalid entryRefundStatus: {0}", this.entryRefundStatus));
                }
            }
        }

        ChinaAlibabaOrderPayStatus IChinaAlibabaOrderEntryInfo.EntryPayStatus {
            get {
                return (ChinaAlibabaOrderPayStatus) this.entryPayStatus;
            }
        }

        string IChinaAlibabaOrderEntryInfo.RefundId {
            get {
                return this.refundId;
            }
        }

        string IChinaAlibabaOrderEntryInfo.RefundIdForAs {
            get {
                return this.refundIdForAs;
            }
        }

        long IChinaAlibabaOrderEntryInfo.CategoryId {
            get {
                return this.categoryId;
            }
        }

        long IChinaAlibabaOrderEntryInfo.OrderId {
            get {
                return this.orderId;
            }
        }

        string IChinaAlibabaOrderEntryInfo.SpecId {
            get {
                return this.specId;
            }
        }

        bool IChinaAlibabaOrderEntryInfo.HasGuaranteeSupport {
            get {
                return this.guaranteeSupport;
            }
        }

        string IChinaAlibabaOrderEntryInfo.IndustrySecurityCodes {
            get {
                return this.industrySecurityCodes;
            }
        }

        bool IChinaAlibabaOrderEntryInfo.HasBuyerSecuritySupport {
            get {
                return this.buyerSecuritySupport;
            }
        }

        ChinaAlibabaOrderRateStatus IChinaAlibabaOrderEntryInfo.BuyerRateStatus {
            get {
                return (ChinaAlibabaOrderRateStatus) this.buyerRateStatus;
            }
        }

        ChinaAlibabaOrderRateStatus IChinaAlibabaOrderEntryInfo.SellerRateStatus {
            get {
                return (ChinaAlibabaOrderRateStatus) this.sellerRateStatus;
            }
        }

        string IChinaAlibabaOrderEntryInfo.CloseReason {
            get {
                return this.closeReason;
            }
        }

        DateTime? IChinaAlibabaOrderEntryInfo.CreateTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtCreate) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtCreate) : null;
            }
        }

        DateTime? IChinaAlibabaOrderEntryInfo.ModifiedTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtModified) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtModified) : null;
            }
        }

        DateTime? IChinaAlibabaOrderEntryInfo.CompletedTime {
            get {
                return !string.IsNullOrWhiteSpace(this.gmtCompleted) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.gmtCompleted) : null;
            }
        }

        string IChinaAlibabaOrderEntryInfo.Remark {
            get {
                return this.remark;
            }
        }

        ChinaAlibabaOrderLogisticsStatus IChinaAlibabaOrderEntryInfo.LogisticsStatus {
            get {
                return (ChinaAlibabaOrderLogisticsStatus) this.logisticsStatus;
            }
        }

        string IChinaAlibabaOrderEntryInfo.ExternalId {
            get {
                return this.externalId;
            }
        }

        #endregion
    }
}
