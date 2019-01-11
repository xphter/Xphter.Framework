using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Collections.Tests {
    [TestClass()]
    public class NullableKeyDictionaryTests {
        [TestMethod()]
        public void NullableKeyDictionaryTest() {
            int value = 0;
            IDictionary<string, int> target = new NullableKeyDictionary<string, int>();

            target.Add(null, 13);
            target.Add(new KeyValuePair<string, int>("a", 14));
            Assert.IsTrue(target.ContainsKey(null));
            Assert.IsTrue(target.ContainsKey("a"));
            Assert.IsTrue(target.Contains(new KeyValuePair<string, int>(null, 13)));
            Assert.IsTrue(target.Contains(new KeyValuePair<string, int>("a", 14)));
            Assert.IsTrue(target.Keys.Contains(null));
            Assert.IsTrue(target.Keys.Contains("a"));
            Assert.IsTrue(target.Values.Contains(13));
            Assert.IsTrue(target.Values.Contains(14));
            Assert.AreEqual(target.Count, 2);

            Assert.IsTrue(target.TryGetValue(null, out value));
            Assert.AreEqual(value, 13);
            Assert.IsTrue(target.TryGetValue("a", out value));
            Assert.AreEqual(value, 14);

            foreach(KeyValuePair<string, int> item in target) {
                Console.WriteLine(item);
            }

            Assert.IsTrue(target.Remove(null));
            Assert.IsFalse(target.Remove(new KeyValuePair<string, int>(null, 13)));
            Assert.IsTrue(target.Remove("a"));
            Assert.IsFalse(target.Remove(new KeyValuePair<string, int>("a", 14)));

            foreach(KeyValuePair<string, int> item in target) {
                Console.WriteLine(item);
            }
        }
    }
}
