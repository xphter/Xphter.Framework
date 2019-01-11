using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xphter.Framework.WeChat.OfficialAccounts;

namespace Xphter.Framework.WeChat.OfficialAccountsTest.Portal {
    [WeChatEventHandler(WeChatEventTypes.SUBSCRIBE)]
    public class WeChatSubscribeEventHandler : IWeChatMessageHandler {
        #region IWeChatMessageHandler Members

        public WeChatMessageHandlerPriority Priority {
            get {
                return WeChatMessageHandlerPriority.Normal;
            }
        }

        public IAsyncResult BeginProcessMessage(IWeChatMessage message, IOfficialAccountService service, AsyncCallback callback, object userState) {
            IWeChatMessageResult messageResult = new WeChatTextReply {
                ToUserName = message.FromUserName,
                FromUserName = message.ToUserName,
                CreateTime = message.CreateTime,
                Content = "你好，欢迎关注南阳仙草药业公众号，<a href=\"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx4c8496aa5431914e&redirect_uri=http%3A%2F%2Fweixin.xphter.com%2Fhome%2Fbinding&response_type=code&scope=snsapi_base&state=123#wechat_redirect\">点击链接</a>绑定微信号，随时了解您的订单动态。发送\"订单提醒\"消息，系统将向您发送订单提醒。",
                //Content = "你好，欢迎关注南阳仙草药业公众号，<a href=\"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx4c8496aa5431914e&redirect_uri=http%3A%2F%2Fxphter.gicp.net%2FXphter.Framework.WeChat.OfficialAccountsTest.Portal%2Fhome%2Fbinding&response_type=code&scope=snsapi_base&state=123#wechat_redirect\">点击链接</a>绑定微信号，随时了解您的订单动态。发送\"订单提醒\"消息，系统将向您发送订单提醒。",
            };

            AsyncResult<IWeChatMessageResult> result = new AsyncResult<IWeChatMessageResult>(callback, userState);
            result.MarkCompleted(null, true, messageResult);
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