using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public interface IOfficialAccountService {
        bool IsSupportCallWeChatService {
            get;
        }

        bool Validate(string signature, string timestamp, string nonce);

        string GetBaseOAuthUri(string redirectUri, string state);

        string GetUserInfoOAuthUri(string redirectUri, string state);

        IAsyncResult BeginProcessMessage(byte[] data, AsyncCallback callback, object userState);

        IWeChatMessageResult EndProcessMessage(IAsyncResult asyncResult);

        IAsyncResult BeginClearQuota(AsyncCallback callback, object userState);

        void EndClearQuota(IAsyncResult asyncResult);

        IAsyncResult BeginGetServerInfo(AsyncCallback callback, object userState);

        IWeChatServerInfo EndGetServerInfo(IAsyncResult asyncResult);

        IAsyncResult BeginCreateMenuItems(IEnumerable<IWeChatMenuItem> menuItems, AsyncCallback callback, object userState);

        void EndGetCreateMenuItems(IAsyncResult asyncResult);

        IAsyncResult BeginGetOAuthAccessToken(string code, AsyncCallback callback, object userState);

        IWeChatOAuthAccessToken EndGetOAuthAccessToken(IAsyncResult asyncResult);

        IAsyncResult BeginGetOAuthUserInfo(string accessToken, string openID, WeChatLanguage language, AsyncCallback callback, object userState);

        IWeChatOAuthUserInfo EndGetOAuthUserInfo(IAsyncResult asyncResult);

        IAsyncResult BeginSendTemplateMessage(string message, AsyncCallback callback, object userState);

        IWeChatTemplateMessageReturnValue EndSendTemplateMessage(IAsyncResult asyncResult);

        IAsyncResult BeginSendCustomerServiceMessage(string message, AsyncCallback callback, object userState);

        IAsyncResult BeginSendCustomerServiceMessage(WeChatCustomerServiceMessage message, AsyncCallback callback, object userState);

        void EndSendCustomerServiceMessage(IAsyncResult asyncResult);

        IAsyncResult BeginGetUserInfo(string openID, WeChatLanguage language, AsyncCallback callback, object userState);

        IWeChatUserInfo EndGetUserInfo(IAsyncResult asyncResult);

        IAsyncResult BeginGetAllUsers(AsyncCallback callback, object userState);

        IWeChatUsersList EndGetAllUsers(IAsyncResult asyncResult);
    }
}
