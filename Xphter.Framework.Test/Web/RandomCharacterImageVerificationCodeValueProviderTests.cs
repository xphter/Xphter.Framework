using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;
using Xphter.Framework.Web;

namespace Xphter.Framework.Web.Tests {
    [TestClass()]
    public class RandomCharacterImageVerificationCodeValueProviderTests {
        [TestMethod()]
        public void CreateTest() {
            RandomCharacterImageVerificationCodeValueProvider provider = new RandomCharacterImageVerificationCodeValueProvider(new RandomCharacterVerificationCodeValueOption {
                CharactersCount = new Range(4, 4),
                Characters = "abcdefghijklmnopqrstuvwxyz",
            });

            ICollection<string> codes = new List<string>();
            for(int i = 0; i < 10; i++) {
                codes.Add(provider.GetValue());
            }

            Assert.AreEqual(codes.Count, codes.Distinct().Count());
        }
    }
}
