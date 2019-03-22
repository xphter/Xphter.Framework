using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Diagnostics {
    /// <summary>
    /// Represents the general type of a log data.
    /// </summary>
    public enum LogInfoType {
        [Description("无")]
        None = 0x00,

        [Description("普通信息")]
        Info = 0x10,

        [Description("调试信息")]
        Debug = 0x20,

        [Description("警告信息")]
        Warning = 0x30,

        [Description("错误信息")]
        Error = 0x40,

        [Description("故障信息")]
        Fault = 0x80,
    }

    /// <summary>
    /// Represents information of a log data.
    /// </summary>
    public interface ILogInfo {
        /// <summary>
        /// Gets the create time.
        /// </summary>
        DateTime CreateTime {
            get;
        }

        /// <summary>
        /// Gets the host address.
        /// </summary>
        string HostAddress {
            get;
        }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        string HostName {
            get;
        }

        /// <summary>
        /// Gets the log source.
        /// </summary>
        string OperationSource {
            get;
        }

        /// <summary>
        /// Gets current operation name.
        /// </summary>
        string OperationName {
            get;
        }

        /// <summary>
        /// Gets the log type.
        /// </summary>
        LogInfoType LogType {
            get;
        }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        int LogLevel {
            get;
        }

        /// <summary>
        /// Gets a object associated with this log.
        /// </summary>
        object StateObject {
            get;
        }

        /// <summary>
        /// Gets the ocurred exception.
        /// </summary>
        Exception ExceptionObject {
            get;
        }
    }

    /// <summary>
    /// Defines whether to save a log.
    /// </summary>
    public interface ILogInfoFilter {
        /// <summary>
        /// Checks whether to log this info.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Return true, if should record <paramref name="info"/>; otherwise false.</returns>
        bool Filter(ILogInfo info);
    }

    /// <summary>
    /// Defines how to select a storage.
    /// </summary>
    public interface ILogStorageSelector {
        /// <summary>
        /// Gets a ILogStorage to record <paramref name="info"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        ILogStorage SelectStorage(ILogInfo info);
    }

    /// <summary>
    /// Represents a storage of log data.
    /// </summary>
    public interface ILogStorage : IDisposable {
        /// <summary>
        /// Saves the specified log info.
        /// </summary>
        /// <param name="info"></param>
        void Save(ILogInfo info);
    }

    /// <summary>
    /// Converts log data to text.
    /// </summary>
    public interface ILogInfoFormatter {
        /// <summary>
        /// Gets text representation of the specified log info.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        string Format(ILogInfo info);
    }

    /// <summary>
    /// Converts log state obejct to text.
    /// </summary>
    public interface ILogStateObjetFormatter {
        /// <summary>
        /// Gets text representation of the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Format(object obj);
    }

    /// <summary>
    /// Represents the exception of log components.
    /// </summary>
    public class LogException : Exception {
        public LogException()
            : base() {
        }

        public LogException(string message)
            : base(message) {
        }

        protected LogException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public LogException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }

    /// <summary>
    /// Represents a logger.
    /// </summary>
    public interface ILogger : IDisposable {
        /// <summary>
        /// Gets log name.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Records the specified log.
        /// </summary>
        /// <param name="info"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="info"/> is null.</exception>
        void Record(ILogInfo info);

        /// <summary>
        /// Starts a asynchronously operatin to record the specified log and return immediately.
        /// </summary>
        /// <param name="info"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="info"/> is null.</exception>
        void RecordAsync(ILogInfo info);

        /// <summary>
        /// Closes this logger. A closed logger can not records any logs.
        /// </summary>
        void Close();
    }

    /// <summary>
    /// Provides a default implementation of ILogInfo.
    /// </summary>
    public class LogInfo : ILogInfo {
        static LogInfo() {
            IPAddress[] addresses = Dns.GetHostAddresses(string.Empty).Where((item) => item.AddressFamily == AddressFamily.InterNetwork || item.AddressFamily == AddressFamily.InterNetworkV6).ToArray();

            g_hostAddress = addresses.Length > 0 ? (addresses.FirstOrDefault((item) => item.AddressFamily == AddressFamily.InterNetwork) ?? addresses.FirstOrDefault((item) => item.AddressFamily == AddressFamily.InterNetworkV6)).ToString() : null;
            g_hostName = Environment.MachineName;
        }

        public LogInfo() {
            this.m_createTime = DateTime.Now;
        }

        private static string g_hostAddress;
        private static string g_hostName;

        private DateTime m_createTime;

        #region ILogInfo Members

        /// <inheritdoc />
        public DateTime CreateTime {
            get {
                return this.m_createTime;
            }
        }

        /// <inheritdoc />
        public string HostAddress {
            get {
                return g_hostAddress;
            }
        }

        /// <inheritdoc />
        public string HostName {
            get {
                return g_hostName;
            }
        }

        /// <inheritdoc />
        public string OperationSource {
            get;
            set;
        }

        /// <inheritdoc />
        public string OperationName {
            get;
            set;
        }

        /// <inheritdoc />
        public LogInfoType LogType {
            get;
            set;
        }

        /// <inheritdoc />
        public int LogLevel {
            get;
            set;
        }

        /// <inheritdoc />
        public object StateObject {
            get;
            set;
        }

        /// <inheritdoc />
        public Exception ExceptionObject {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Provides a base class that implements ILogger interface.
    /// </summary>
    public abstract class Logger : ILogger {
        public Logger(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("name is null or empty.", "name");
            }

            this.m_name = name;
            this.m_signal = new AutoResetEvent(false);
            this.m_logQueue = new LockFreeQueue<ILogInfo>();

            new Thread(this.ScanQueue).Start();
        }

        protected string m_name;
        protected AutoResetEvent m_signal;
        protected LockFreeQueue<ILogInfo> m_logQueue;

        protected abstract ILogInfoFilter GetLogFilter(ILogInfo info);

        protected abstract ILogStorageSelector GetStorageSelector(ILogInfo info);

        protected void ThrowIfNullOrDisposed(ILogInfo info) {
            if(info == null) {
                throw new ArgumentNullException("info");
            }

            if(this.m_disposed) {
                throw new ObjectDisposedException(this.m_name);
            }
        }

        protected virtual void InternalRecord(ILogInfo info) {
            ILogInfoFilter filter = this.GetLogFilter(info);
            if(filter != null && !filter.Filter(info)) {
                return;
            }

            ILogStorageSelector selector = this.GetStorageSelector(info);
            if(selector == null) {
                throw new LogException("Can not find a ILogStorageSelector.");
            }

            ILogStorage storage = selector.SelectStorage(info);
            if(storage == null) {
                return;
            }

            storage.Save(info);
        }

        private void ScanQueue() {
            while(true) {
                try {
                    this.m_signal.WaitOne();
                } catch(ObjectDisposedException) {
                    this.ProcessQueue();

                    break;
                }

                this.ProcessQueue();
            }
        }

        private void ProcessQueue() {
            ILogInfo info = null;

            while(this.m_logQueue.TryDequeue(out info)) {
                try {
                    this.InternalRecord(info);
                } catch {
                }
            }
        }

        #region ILogger Members

        /// <inheritdoc />
        public virtual string Name {
            get {
                return this.m_name;
            }
        }

        /// <inheritdoc />
        public virtual void Record(ILogInfo info) {
            this.ThrowIfNullOrDisposed(info);

            this.InternalRecord(info);
        }

        /// <inheritdoc />
        public virtual void RecordAsync(ILogInfo info) {
            this.ThrowIfNullOrDisposed(info);

            this.m_logQueue.Enqueue(info);
            this.m_signal.Set();
        }

        /// <inheritdoc />
        public virtual void Close() {
            this.Disposing(true);
        }

        #endregion

        #region IDisposable Members

        protected volatile bool m_disposed;

        protected abstract void DisposingCore();

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_signal != null) {
                using(this.m_signal) {
                    this.m_signal.Set();
                }
            }

            this.DisposingCore();

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~Logger() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }

    /// <summary>
    /// Provides a chain of log storages.
    /// </summary>
    public class AggregateLogStorage : ILogStorage {
        public AggregateLogStorage(IEnumerable<ILogStorage> storages) {
            if(storages == null) {
                throw new ArgumentNullException("storages");
            }
            if(!storages.Any((item) => item != null)) {
                throw new ArgumentNullException("storages is empty.", "storages");
            }

            this.m_storages = storages.Where((item) => item != null).ToArray();
        }

        protected IEnumerable<ILogStorage> m_storages;

        protected void ThrowIfDisposed() {
            if(this.m_disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        #region ILogStorage Members

        public void Save(ILogInfo info) {
            this.ThrowIfDisposed();

            foreach(ILogStorage storage in this.m_storages) {
                storage.Save(info);
            }
        }

        #endregion

        #region IDisposable Members

        protected volatile bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_storages != null) {
                foreach(ILogStorage storage in this.m_storages) {
                    storage.Dispose();
                }
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~AggregateLogStorage() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }

    public class DefaultLogger : Logger, ILogInfoFilter, ILogStorageSelector {
        public DefaultLogger(string name, IEnumerable<DefaultLoggerStorageOption> config)
            : base(name) {
            this.m_storages = new List<ILogStorage>();
            this.m_cache = new ConcurrentDictionary<string, ILogStorage>();

            foreach(DefaultLoggerStorageOption option in this.m_config = config ?? Enumerable.Empty<DefaultLoggerStorageOption>()) {
                this.m_storages.AddRange(option.Storages);
            }
        }

        private IEnumerable<DefaultLoggerStorageOption> m_config;
        private ICollection<ILogStorage> m_storages;

        private ConcurrentDictionary<string, ILogStorage> m_cache;

        private string GetCacheKey(ILogInfo info) {
            return string.Format("{0}_{1}", info.LogType, info.LogLevel);
        }

        #region Logger Members

        protected override void DisposingCore() {
            if(this.m_storages != null) {
                foreach(ILogStorage storage in this.m_storages) {
                    storage.Dispose();
                }
            }
        }

        protected override ILogInfoFilter GetLogFilter(ILogInfo info) {
            return this;
        }

        protected override ILogStorageSelector GetStorageSelector(ILogInfo info) {
            return this;
        }

        #endregion

        #region ILogFilter Members

        public bool Filter(ILogInfo info) {
            return this.m_config.Any((item) => item.IsMatch(info));
        }

        #endregion

        #region ILogStorageSelector Members

        public ILogStorage SelectStorage(ILogInfo info) {
            ILogStorage result = null;
            string cacheKey = this.GetCacheKey(info);

            if(this.m_cache.TryGetValue(cacheKey, out result)) {
                return result;
            }

            ILogStorage[] storages = (from i in this.m_config
                                      where i.IsMatch(info)
                                      from j in i.Storages
                                      select j).ToArray();
            this.m_cache.TryAdd(cacheKey, result = storages.Length > 0 ? new ChainedLogStorage(storages) : null);

            return result;
        }

        #endregion

        /// <summary>
        /// Provides a chain of storages.
        /// </summary>
        private class ChainedLogStorage : ILogStorage {
            public ChainedLogStorage(IEnumerable<ILogStorage> storages) {
                this.m_storages = storages;

                GC.SuppressFinalize(this);
            }

            protected IEnumerable<ILogStorage> m_storages;

            #region ILogStorage Members

            public void Save(ILogInfo info) {
                foreach(ILogStorage storage in this.m_storages) {
                    storage.Save(info);
                }
            }

            #endregion

            #region IDisposable Members

            ~ChainedLogStorage() {
            }

            public void Dispose() {
            }

            #endregion
        }
    }

    public class DefaultLoggerFilterRule {
        public DefaultLoggerFilterRule(LogInfoType? logType, int? minLevel, int? maxLevel) {
            this.m_logType = logType;
            this.m_minLevel = minLevel;
            this.m_maxLevel = maxLevel;
        }

        private LogInfoType? m_logType;
        private int? m_minLevel;
        private int? m_maxLevel;

        internal bool IsMatch(ILogInfo info) {
            if(this.m_logType.HasValue && info.LogType != this.m_logType.Value) {
                return false;
            }

            if(this.m_minLevel.HasValue && info.LogLevel < this.m_minLevel.Value) {
                return false;
            }

            if(this.m_maxLevel.HasValue && info.LogLevel > this.m_maxLevel.Value) {
                return false;
            }

            return true;
        }
    }

    public class DefaultLoggerStorageOption {
        public DefaultLoggerStorageOption(IEnumerable<DefaultLoggerFilterRule> rules, IEnumerable<ILogStorage> storages) {
            this.m_rules = rules ?? Enumerable.Empty<DefaultLoggerFilterRule>();
            this.m_storages = storages ?? Enumerable.Empty<ILogStorage>();
        }

        private IEnumerable<DefaultLoggerFilterRule> m_rules;
        private IEnumerable<ILogStorage> m_storages;

        internal IEnumerable<ILogStorage> Storages {
            get {
                return this.m_storages;
            }
        }

        internal bool IsMatch(ILogInfo info) {
            return this.m_storages.Any() && (!this.m_rules.Any() || this.m_rules.Any((item) => item.IsMatch(info)));
        }
    }
}
