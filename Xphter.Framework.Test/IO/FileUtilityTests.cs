using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.IO;

namespace Xphter.Framework.IO.Tests {
    [TestClass()]
    public class FileUtilityTests {
        [TestMethod()]
        public void BeginReadAllTextTest() {
            byte[] buffer = new byte[1024 * 1024];
            string path = @"Resources\AsyncFileStream.txt";

            string expected = File.ReadAllText(path, Encoding.Default);

            IAsyncResult asyncResult = FileUtility.BeginReadAllText(path, buffer, Encoding.Default, null, null);
            string actual = FileUtility.EndReadAllText(asyncResult);

            Assert.AreEqual<string>(expected, actual);
        }

        [TestMethod()]
        public void CreateFileStreamTest_Sync() {
            string path = "CreateFile.txt", content = "dupeng";
            byte[] data = Encoding.UTF8.GetBytes(content);
            using(FileStream writer = FileUtility.CreateFileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024, false)) {
                writer.Write(data, 0, data.Length);
            }

            using(FileStream reader = FileUtility.CreateFileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024, false)) {
                data = new byte[reader.Length];
                reader.Read(data, 0, data.Length);
            }

            Assert.AreEqual(Encoding.UTF8.GetString(data), content);
        }

        [TestMethod()]
        public void CreateFileStreamTest_Async() {
            string path = "CreateFile.txt", content = "dupeng";
            byte[] data = Encoding.UTF8.GetBytes(content);

            bool result = false;
            ManualResetEvent mre = new ManualResetEvent(false);
            FileStream writer = FileUtility.CreateFileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024, false);
            writer.BeginWrite(data, 0, data.Length, (ar1) => {
                try {
                    using(writer) {
                        writer.EndWrite(ar1);
                    }

                    FileStream reader = FileUtility.CreateFileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024, false);
                    data = new byte[reader.Length];

                    reader.BeginRead(data, 0, data.Length, (ar2) => {
                        try {
                            using(reader) {
                                reader.EndRead(ar2);
                            }

                            result = true;
                        } finally {
                            mre.Set();
                        }
                    }, null);
                } catch {
                    mre.Set();
                }
            }, null);

            mre.WaitOne();

            Assert.IsTrue(result);
            Assert.AreEqual(Encoding.UTF8.GetString(data), content);
        }

        [TestMethod()]
        public void CreateFileStreamTest_ReadNull() {
            string path = "CreateFile.txt";
            using(FileStream writer = FileUtility.CreateFileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 1024, false)) {
                FileStream reader = FileUtility.CreateFileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024, false);
                Assert.IsNull(reader);
            }
        }

        [TestMethod()]
        public void CreateFileStreamTest_WriteNull() {
            string path = "CreateFile.txt";
            using(FileStream reader = FileUtility.CreateFileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Delete, 1024, true)) {
                FileStream writer = FileUtility.CreateFileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024, true);
                Assert.IsNull(writer);
            }
        }
    }
}
