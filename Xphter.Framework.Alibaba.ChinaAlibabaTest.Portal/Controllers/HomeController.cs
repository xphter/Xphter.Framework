using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xphter.Framework.Alibaba.ChinaAlibaba;

namespace Xphter.Framework.Alibaba.ChinaAlibabaTest.Portal.Controllers {
    public class HomeController : AsyncController {
        private IChinaAlibabaService m_service;

        protected override void Initialize(RequestContext requestContext) {
            base.Initialize(requestContext);

            this.m_service = MvcApplication.ChinaAlibabaService;
        }

        private string GetAbsoluteUrl(string relativeUrl) {
            return string.Format("{0}://{1}:{2}{3}", this.Request.Url.Scheme, this.Request.Url.Host, this.Request.Url.Port, relativeUrl);
        }

        [NoAsyncTimeout]
        public void IndexAsync() {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetIsAuthorized((ar) => {
                Exception error = null;
                bool isAuthorized = false;

                try {
                    isAuthorized = this.m_service.EndGetIsAuthorized(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                ActionResult result = null;
                if(error != null) {
                    result = this.Content(string.Format("Error: {0}\r\n{1}", error.Message, error.StackTrace));
                } else if(isAuthorized) {
                    result = this.View();
                } else {
                    result = this.Redirect(AlibabaHelper.GetChinaOAuthUri(MvcApplication.APP_KEY, MvcApplication.APP_SECRET, this.GetAbsoluteUrl(this.Url.Action("Authorize")), null));
                }

                this.AsyncManager.Parameters["result"] = result;
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult IndexCompleted(ActionResult result) {
            return result;
        }

        [NoAsyncTimeout]
        public void AuthorizeAsync(string code) {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginAuthorize(code, this.GetAbsoluteUrl(this.Url.Action("Index")), (ar) => {
                Exception error = null;

                try {
                    this.m_service.EndAuthorize(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                ActionResult result = null;
                if(error != null) {
                    result = this.Content(string.Format("Error: {0}\r\n{1}", error.Message, error.StackTrace));
                } else {
                    result = this.Redirect(this.GetAbsoluteUrl(this.Url.Action("Index")));
                }

                this.AsyncManager.Parameters["result"] = result;
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult AuthorizeCompleted(ActionResult result) {
            return result;
        }

        [NoAsyncTimeout]
        public void MemberDetailAsync(string id) {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetMemberInfo(id ?? this.m_service.AuthorizedMemberID, (ar) => {
                Exception error = null;
                IChinaAlibabaMemberInfo member = null;

                try {
                    member = this.m_service.EndGetMemberInfo(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                ActionResult result = null;
                if(error != null) {
                    result = this.Content(string.Format("Error: {0}\r\n{1}", error.Message, error.StackTrace));
                } else {
                    result = this.View(member);
                }

                this.AsyncManager.Parameters["result"] = result;
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult MemberDetailCompleted(ActionResult result) {
            return result;
        }

        [NoAsyncTimeout]
        public void CurrentSalesOrdersAsync() {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetOrderList(new ChinaAlibabaGetOrderListArguments {
                SellerMemberId = this.m_service.AuthorizedMemberID,
                CreateEndTime = DateTime.Now,
                PageSize = 100,
            }, (ar) => {
                Exception error = null;
                IChinaAlibabaOrderList orderList = null;

                try {
                    orderList = this.m_service.EndGetOrderList(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                ActionResult result = null;
                if(error != null) {
                    result = this.Content(string.Format("Error: {0}\r\n{1}", error.Message, error.StackTrace));
                } else {
                    result = this.View(orderList);
                }

                this.AsyncManager.Parameters["result"] = result;
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult CurrentSalesOrdersCompleted(ActionResult result) {
            return result;
        }

        [NoAsyncTimeout]
        public void OrderDetailAsync(long id) {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetOrderInfo(new ChinaAlibabaGetOrderInfoArguments {
                ID = id,
                NeedOrderEntries = true,
                NeedOrderMemoList = true,
                NeedLogisticsOrderList = true,
                NeedInvoiceInfo = true,
            }, (ar) => {
                Exception error = null;
                IChinaAlibabaOrderInfo orderInfo = null;

                try {
                    orderInfo = this.m_service.EndGetOrderInfo(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                ActionResult result = null;
                if(error != null) {
                    result = this.Content(string.Format("Error: {0}\r\n{1}", error.Message, error.StackTrace));
                } else {
                    result = this.View(orderInfo);
                }

                this.AsyncManager.Parameters["result"] = result;
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult OrderDetailCompleted(ActionResult result) {
            return result;
        }
    }
}
