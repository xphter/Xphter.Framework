using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.WeChat.OfficialAccounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Xphter.Framework.WeChat.OfficialAccounts.Tests {
    [TestClass()]
    public class WeChatTextReplyTests {
        [TestMethod()]
        public void GetResultTest() {
            IWeChatMessageResult target = new WeChatTextReply {
                ToUserName = "to",
                FromUserName = "from",
                CreateTime = 131313,
                Content = "你好",
            };
            string result = target.ResultContent;
        }
    }
}
