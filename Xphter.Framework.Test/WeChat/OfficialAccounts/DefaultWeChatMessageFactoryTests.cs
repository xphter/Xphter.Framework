using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.WeChat.OfficialAccounts;

namespace Xphter.Framework.WeChat.OfficialAccounts.Tests {
    [TestClass()]
    public class DefaultWeChatMessageFactoryTests {
        [TestMethod()]
        public void GetMessageTest_TextMessage() {
            DefaultWeChatMessageFactory target = new DefaultWeChatMessageFactory();
            IWeChatMessage message = target.GetMessage(Xphter.Framework.Test.Properties.Resources.WeChatTextMessage, null);
        }
    }
}
