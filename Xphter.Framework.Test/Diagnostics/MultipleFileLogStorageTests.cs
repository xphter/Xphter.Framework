using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Diagnostics;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Diagnostics.Tests {
    [TestClass()]
    public class MultipleFileLogStorageTests {
        public const string CONFIG_FILE_NAME = "logConfig.xml";

        [ClassInitialize]
        public static void Initialize(TestContext context) {
        }

        [TestMethod()]
        public void CentralizedFileLogStorageFactoryTest() {
            File.WriteAllText(CONFIG_FILE_NAME, Xphter.Framework.Test.Properties.Resources.LogConfig.Replace("ref=\"\"", "ref=\"centralized\""), Encoding.UTF8);

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
                        LogType = LogInfoType.Error,
                        StateObject = "错误" + (i + 1),
                    });
                }

                Thread.Sleep(20000);
            }
        }

        [TestMethod()]
        public void DistributedFileLogStorageFactoryTest_NoSource() {
            File.WriteAllText(CONFIG_FILE_NAME, Xphter.Framework.Test.Properties.Resources.LogConfig.Replace("ref=\"\"", "ref=\"distributed\""), Encoding.UTF8);

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
                    StateObject = "错误",
                    ExceptionObject = new NotImplementedException("未实现"),
                });
            }
        }

        [TestMethod()]
        public void DistributedFileLogStorageFactoryTest_WithSource() {
            File.WriteAllText(CONFIG_FILE_NAME, Xphter.Framework.Test.Properties.Resources.LogConfig.Replace("ref=\"\"", "ref=\"distributed\""), Encoding.UTF8);

            IObjectFactory objectFactory = new XmlConfigurationObjectFactory(CONFIG_FILE_NAME, new Assembly[] {
                Assembly.GetExecutingAssembly(),
            }, null);

            using(ILogger logger = objectFactory.CreateInstance<ILogger>()) {
                logger.Record(new LogInfo {
                    OperationSource = "来源0",
                    LogType = LogInfoType.Info,
                    StateObject = "信息",
                });
                logger.RecordAsync(new LogInfo {
                    OperationSource = "来源1",
                    LogType = LogInfoType.Warning,
                    StateObject = "警告0",
                });
                logger.RecordAsync(new LogInfo {
                    OperationSource = "来源2",
                    LogType = LogInfoType.Error,
                    StateObject = "错误0",
                });

                int count = 1000000;

                Task t1 = Task.Factory.StartNew(() => {
                    for(int i = 0; i < count; i++) {
                        logger.RecordAsync(new LogInfo {
                            OperationSource = "来源1",
                            LogType = LogInfoType.Warning,
                            StateObject = "警告" + (i + 1),
                        });
                    }
                });

                Task t2 = Task.Factory.StartNew(() => {
                    for(int i = 0; i < count; i++) {
                        logger.RecordAsync(new LogInfo {
                            OperationSource = "来源2",
                            LogType = LogInfoType.Error,
                            StateObject = "错误" + (i + 1),
                        });
                    }
                });

                Task.WaitAll(t1, t2);

                Thread.Sleep(20000);
            }
        }
    }
}
