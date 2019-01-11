using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Xphter.Framework.WeChat.OfficialAccounts;

namespace Xphter.Framework.WeChat.OfficialAccountsTest.Portal {
    [WeChatMessageHandler(WeChatMessageTypes.TEXT)]
    public class WeChatTextMessageHandler : IWeChatMessageHandler {
        #region IWeChatMessageHandler Members

        public WeChatMessageHandlerPriority Priority {
            get {
                return WeChatMessageHandlerPriority.Normal;
            }
        }

        public IAsyncResult BeginProcessMessage(IWeChatMessage message, IOfficialAccountService service, AsyncCallback callback, object userState) {
            AsyncResult<IWeChatMessageResult> result = new AsyncResult<IWeChatMessageResult>(callback, userState);
            IWeChatTextMessage textMessage = message as IWeChatTextMessage;

            if(string.Equals(textMessage.Content, "模板消息")) {
                string content = File.ReadAllText(HostingEnvironment.MapPath("~/order.json"), Encoding.UTF8).Replace("{openid}", textMessage.FromUserName);
                service.BeginSendTemplateMessage(content, (ar) => {
                    IWeChatTemplateMessageReturnValue value = service.EndSendTemplateMessage(ar);
                }, null);

                result.MarkCompleted(null, true, new WeChatTextReply {
                    ToUserName = message.FromUserName,
                    FromUserName = message.ToUserName,
                    CreateTime = message.CreateTime,
                    Content = "系统随后将向您发送模板消息",
                });
            } else if(string.Equals(textMessage.Content, "客服消息")) {
                string content = File.ReadAllText(HostingEnvironment.MapPath("~/kf.json"), Encoding.UTF8).Replace("{openid}", textMessage.FromUserName);
                service.BeginSendCustomerServiceMessage(content, (ar) => {
                    service.EndSendCustomerServiceMessage(ar);
                }, null);

                service.BeginSendCustomerServiceMessage(new WeChatTextCustomerServiceMessage("嘿嘿哈嘿") {
                    ToUserName = message.FromUserName,
                }, (ar) => {
                    service.EndSendCustomerServiceMessage(ar);
                }, null);

                WeChatNewsCustomerServiceMessage news = new WeChatNewsCustomerServiceMessage {
                    ToUserName = message.FromUserName,
                };
                news.News.Items.Add(new WeChatNewsCustomerServiceMessageItem {
                    Title = "丰满性感尤物洛可可浴室大摆诱人姿势",
                    Description = "丰满性感尤物洛可可浴室大摆诱人姿势",
                    PicUrl = "http://www.xunmeitu.com/Static/Images/2016/12/24/185706528004254.jpg",
                    Url = "http://www.xunmeitu.com/a/17798.html",
                });
                news.News.Items.Add(new WeChatNewsCustomerServiceMessageItem {
                    Title = "性感田妞肤白貌美惹眼上围乳此动人",
                    Description = "性感田妞肤白貌美惹眼上围乳此动人",
                    PicUrl = "http://www.xunmeitu.com/Static/Images/2016/10/07/153628983003544.jpg",
                    Url = "http://www.xunmeitu.com/a/9837.html",
                });
                service.BeginSendCustomerServiceMessage(news, (ar) => {
                    service.EndSendCustomerServiceMessage(ar);
                }, null);

                result.MarkCompleted(null, true, new WeChatTextReply {
                    ToUserName = message.FromUserName,
                    FromUserName = message.ToUserName,
                    CreateTime = message.CreateTime,
                    Content = "系统随后将向您发送客服消息",
                });
            } else {
                service.BeginGetUserInfo(message.FromUserName, WeChatLanguage.SimplifiedChinese, (ar) => {
                    IWeChatUserInfo userInfo = null;

                    try {
                        userInfo = service.EndGetUserInfo(ar);
                    } catch(Exception) {
                    }
                }, null);

                result.MarkCompleted(null, true, new WeChatTextReply {
                    ToUserName = message.FromUserName,
                    FromUserName = message.ToUserName,
                    CreateTime = message.CreateTime,
                    Content = textMessage.Content,
                });
            }

            return result;
        }

        public IWeChatMessageResult EndProcessMessage(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatMessageResult>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatMessageResult>) asyncResult).End();
        }

        #endregion
    }
}