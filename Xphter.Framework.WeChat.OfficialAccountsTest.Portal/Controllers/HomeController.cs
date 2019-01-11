using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Xphter.Framework.Collections;
using Xphter.Framework.WeChat.OfficialAccounts;

namespace Xphter.Framework.WeChat.OfficialAccountsTest.Portal.Controllers {
    [SessionState(SessionStateBehavior.Disabled)]
    public class HomeController : AsyncController {
        private IOfficialAccountService m_service;

        protected override void Initialize(RequestContext requestContext) {
            base.Initialize(requestContext);

            this.m_service = MvcApplication.OfficialAccountService;
        }

        public ActionResult Index() {
            return this.View(this.m_service);
        }

        [NoAsyncTimeout]
        public void CallbackAsync(string signature, string timestamp, string nonce, string echostr) {
            if(!this.m_service.Validate(signature, timestamp, nonce)) {
                this.AsyncManager.Parameters["result"] = new EmptyResult();
                return;
            }

            switch(this.Request.HttpMethod.ToLower()) {
                case "get":
                    this.AsyncManager.Parameters["result"] = this.Content(echostr);
                    break;
                case "post":
                    this.AsyncManager.OutstandingOperations.Increment();
                    this.m_service.BeginProcessMessage(this.Request.BinaryRead(this.Request.ContentLength), (ar) => {
                        IWeChatMessageResult result = this.m_service.EndProcessMessage(ar);
                        this.AsyncManager.Parameters["result"] = new WeChatMvcActionResult(result);
                        this.AsyncManager.OutstandingOperations.Decrement();
                    }, null);
                    break;
                default:
                    this.AsyncManager.Parameters["result"] = new EmptyResult();
                    break;
            }
        }

        public ActionResult CallbackCompleted(ActionResult result) {
            return result;
        }

        [HttpGet]
        [NoAsyncTimeout]
        public void ClearQuotaAsync() {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginClearQuota((ar) => {
                try {
                    this.m_service.EndClearQuota(ar);
                    this.AsyncManager.Parameters["result"] = this.Content("CLEAR SUCCESS");
                } catch(Exception ex) {
                    this.AsyncManager.Parameters["result"] = this.Content("CLEAR FAIL: " + ex.Message);
                }
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult ClearQuotaCompleted(ActionResult result) {
            return result;
        }

        [HttpGet]
        [NoAsyncTimeout]
        public void GetServerInfoAsync() {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetServerInfo((ar) => {
                this.AsyncManager.Parameters["result"] = this.Content(this.m_service.EndGetServerInfo(ar).IpList.StringJoin("<br />"));
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult GetServerInfoCompleted(ActionResult result) {
            return result;
        }

        [HttpGet]
        [NoAsyncTimeout]
        public void GetAllUsersAsync() {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetAllUsers((ar) => {
                this.AsyncManager.Parameters["result"] = this.Content(this.m_service.EndGetAllUsers(ar).OpenIDList.StringJoin("<br />"));
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult GetAllUsersCompleted(ActionResult result) {
            return result;
        }

        [HttpGet]
        [NoAsyncTimeout]
        public void GetMultipleUsersInfoAsync() {
            this.AsyncManager.OutstandingOperations.Increment();
            //string[] openIDList = new string[] {
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //    "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA", "oN7zZsyfPKXIb12Ml2khxmQSJqoA",
            //};
            string[] openIDList = new string[] {
                "oN7zZsyfPKXIb12Ml2khxmQSJqoA"
            };
            ICollection<string> nicknames = new List<string>();
            using(CountdownEvent cde = new CountdownEvent(openIDList.Length)) {
                foreach(string openID in openIDList) {
                    new Thread(() => {
                        this.m_service.BeginGetUserInfo(openID, WeChatLanguage.SimplifiedChinese, (ar) => {
                            IWeChatUserInfo user = null;
                            try {
                                user = this.m_service.EndGetUserInfo(ar);
                                nicknames.Add(user.Nickname);
                            } catch {
                            } finally {
                                cde.Signal();
                            }
                        }, null);
                    }).Start();
                }

                cde.Wait();
            }

            this.AsyncManager.Parameters["result"] = this.Content(nicknames.StringJoin("<br />"));
            this.AsyncManager.OutstandingOperations.Decrement();
        }

        public ActionResult GetMultipleUsersInfoCompleted(ActionResult result) {
            return result;
        }

        [HttpGet]
        public void BindingAsync(string code) {
            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginGetOAuthAccessToken(code, (ar) => {
                try {
                    this.AsyncManager.Parameters["openID"] = this.m_service.EndGetOAuthAccessToken(ar).OpenID;
                } catch(Exception ex) {
                    this.AsyncManager.Parameters["openID"] = string.Format("Error: {0}", ex.Message);
                }
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult BindingCompleted(string openID) {
            return this.View((object) openID);
        }

        public void CreateMenuAsync() {
            WeChatMenuItem item1 = new WeChatMenuItem {
                Type = WeChatMenuItemType.Click,
                Name = "购物网站",
                Value = "click1",
            };
            item1.ChildItems.Add(new WeChatMenuItem {
                Type = WeChatMenuItemType.View,
                Name = "淘宝",
                Value = "http://m.taobao.com",
            });
            item1.ChildItems.Add(new WeChatMenuItem {
                Type = WeChatMenuItemType.View,
                Name = "京东",
                Value = "http://m.jd.com",
            });

            WeChatMenuItem item2 = new WeChatMenuItem {
                Type = WeChatMenuItemType.Click,
                Name = "搜索查询",
                Value = "click2",
            };
            item2.ChildItems.Add(new WeChatMenuItem {
                Type = WeChatMenuItemType.View,
                Name = "百度",
                Value = "http://m.baidu.com",
            });
            item2.ChildItems.Add(new WeChatMenuItem {
                Type = WeChatMenuItemType.View,
                Name = "备案查询",
                Value = "http://www.beianm.com",
            });

            WeChatMenuItem item3 = new WeChatMenuItem {
                Type = WeChatMenuItemType.View,
                Name = "美女图片",
                Value = "http://www.qianqianwu.com",
            };

            this.AsyncManager.OutstandingOperations.Increment();
            this.m_service.BeginCreateMenuItems(new IWeChatMenuItem[] {
                item1, item2, item3,
            }, (ar) => {
                Exception error = null;
                try {
                    this.m_service.EndGetCreateMenuItems(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                this.AsyncManager.Parameters["error"] = error;
                this.AsyncManager.OutstandingOperations.Decrement();
            }, null);
        }

        public ActionResult CreateMenuCompleted(Exception error) {
            if(error != null) {
                return this.Content(string.Format("创建菜单失败：{0}\r\n\r\n{1}", error.Message, error.StackTrace));
            } else {
                return this.Content("创建菜单成功");
            }
        }
    }
}
