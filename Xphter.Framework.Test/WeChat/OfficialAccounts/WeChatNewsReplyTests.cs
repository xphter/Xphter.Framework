using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.WeChat.OfficialAccounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Xphter.Framework.WeChat.OfficialAccounts.Tests {
    [TestClass()]
    public class WeChatNewsReplyTests {
        [TestMethod()]
        public void GetResultTest() {
            IWeChatMessageResult target = new WeChatNewsReply {
                ToUserName = "to",
                FromUserName = "from",
                CreateTime = 131313,
                Articles = new List<WeChatNewsReplyItem> {
                    new WeChatNewsReplyItem {
                        Title = "title1",
                        Description = "description1",
                        PicUrl = "picurl1",
                        Url = "url1",
                    },
                    new WeChatNewsReplyItem {
                        Title = "title2",
                        Description = "description2",
                        PicUrl = "picurl2",
                        Url = "url2",
                    },
                },
            };
            string result = target.ResultContent;
        }
    }
}
