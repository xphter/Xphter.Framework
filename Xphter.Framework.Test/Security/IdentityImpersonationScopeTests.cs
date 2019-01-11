using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Security;

namespace Xphter.Framework.Security.Tests {
    [TestClass()]
    public class IdentityImpersonationScopeTests {
        [TestMethod()]
        public void IdentityImpersonationScopeTest_Success() {
            using(IdentityImpersonationScope scope = new IdentityImpersonationScope("test", "test")) {
                WindowsIdentity identity = WindowsIdentity.GetCurrent(false);
                Assert.IsTrue(identity.Name.EndsWith("test"));

                File.ReadAllText(@"C:\Windows\System32\drivers\etc\hosts", Encoding.Default);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void IdentityImpersonationScopeTest_Error() {
            using(IdentityImpersonationScope scope = new IdentityImpersonationScope("test", "test")) {
                WindowsIdentity identity = WindowsIdentity.GetCurrent(false);
                Assert.IsTrue(identity.Name.EndsWith("test"));

                File.WriteAllText(@"C:\Windows\System32\drivers\etc\test.txt", "嘿嘿哈嘿", Encoding.Default);
            }
        }
    }
}
