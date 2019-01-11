using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderEntryInfo {
        long ID {
            get;
        }

        string ProductName {
            get;
        }

        long SourceId {
            get;
        }

        decimal Quantity {
            get;
        }

        string Unit {
            get;
        }

        string ProductPicUrl {
            get;
        }

        ChinaAlibabaOrderSnapshotImageInfo[] SnapshotImages {
            get;
        }

        string OrderSourceType {
            get;
        }

        long Price {
            get;
        }

        string CurrencyCode {
            get;
        }

        long DiscountPrice {
            get;
        }

        long Amount {
            get;
        }

        decimal Discount {
            get;
        }

        string Coupon {
            get;
        }

        long EntryDiscount {
            get;
        }

        long EntryCouponAmount {
            get;
        }

        long UnitPrice {
            get;
        }

        long PromotionsFee {
            get;
        }

        long ActualPayFee {
            get;
        }

        string SnapshotId {
            get;
        }

        ChinaAlibabaOrderStatus EntryStatus {
            get;
        }

        ChinaAlibabaOrderRefundStatus? EntryRefundStatus {
            get;
        }

        ChinaAlibabaOrderPayStatus EntryPayStatus {
            get;
        }

        string RefundId {
            get;
        }

        string RefundIdForAs {
            get;
        }

        long CategoryId {
            get;
        }

        long OrderId {
            get;
        }

        string SpecId {
            get;
        }

        bool HasGuaranteeSupport {
            get;
        }

        string IndustrySecurityCodes {
            get;
        }

        bool HasBuyerSecuritySupport {
            get;
        }

        ChinaAlibabaOrderRateStatus BuyerRateStatus {
            get;
        }

        ChinaAlibabaOrderRateStatus SellerRateStatus {
            get;
        }

        string CloseReason {
            get;
        }

        DateTime? CreateTime {
            get;
        }

        DateTime? ModifiedTime {
            get;
        }

        DateTime? CompletedTime {
            get;
        }

        string Remark {
            get;
        }

        ChinaAlibabaOrderLogisticsStatus LogisticsStatus {
            get;
        }

        string ExternalId {
            get;
        }
    }
}
