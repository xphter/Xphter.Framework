using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Diagnostics;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Diagnostics.Tests {
    [TestClass()]
    public class SingleFileLogStorageTests {
        public const string CONFIG_FILE_NAME = "logConfig.xml";

        [ClassInitialize]
        public static void Initialize(TestContext context) {
            File.WriteAllText(CONFIG_FILE_NAME, Xphter.Framework.Test.Properties.Resources.LogConfig.Replace("ref=\"\"", "ref=\"single\""), Encoding.UTF8);
        }

        [TestMethod()]
        public void AllTest() {
            IObjectFactory objectFactory = new XmlConfigurationObjectFactory(CONFIG_FILE_NAME, new Assembly[] {
                Assembly.GetExecutingAssembly(),
            }, null);

            using(ILogger logger = objectFactory.CreateInstance<ILogger>()) {
                logger.Record(new LogInfo {
                    LogType = LogInfoType.Info,
                    StateObject = "信息",
                });

                logger.Record(new LogInfo {
                    LogType = LogInfoType.Warning,
                    StateObject = "警告",
                });

                logger.Record(new LogInfo {
                    LogType = LogInfoType.Error,
                    StateObject = "错误",
                    ExceptionObject = new NotImplementedException("未实现"),
                });

                logger.Record(new LogInfo {
                    LogType = LogInfoType.Error,
                    LogLevel = 2,
                    StateObject = "错误",
                    ExceptionObject = new NotImplementedException("未实现"),
                });

                int count = 1000000;

                for(int i = 0; i < count; i++) {
                    logger.RecordAsync(new LogInfo {
                        LogType = LogInfoType.Warning,
                        StateObject = "警告" + (i + 1),
                    });
                }

                Thread.Sleep(20000);
            }
        }
    }
}
