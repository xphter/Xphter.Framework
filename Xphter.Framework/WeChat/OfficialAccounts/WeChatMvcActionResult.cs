using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatMvcActionResult : ActionResult {
        public WeChatMvcActionResult(IWeChatMessageResult messageResult) {
            this.m_messageResult = messageResult ?? WeChatEmptyMessageResult.Instance;
        }

        private IWeChatMessageResult m_messageResult;

        public override void ExecuteResult(ControllerContext context) {
            string contentType = this.m_messageResult.ContentType;
            string resultContent = this.m_messageResult.ResultContent;
            HttpResponseBase response = context.HttpContext.Response;

            if(contentType != null) {
                response.ContentType = contentType;
            }
            response.ContentEncoding = Encoding.UTF8;
            if(resultContent != null) {
                response.Write(resultContent);
            }
        }
    }
}
