using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using Xphter.Framework.Collections;
using Xphter.Framework.IO;

namespace Xphter.Framework.Diagnostics {
    /// <summary>
    /// Represents the general type of a log data.
    /// </summary>
    public enum LogInfoType {
        [Description("无")]
        None = 0x00,

        [Description("信息")]
        Info = 0x10,

        [Description("调试")]
        Debug = 0x20,

        [Description("警告")]
        Warning = 0x30,

        [Description("错误")]
        Error = 0x40,

        [Description("故障")]
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
    /// Renders log data to a text writer.
    /// </summary>
    public interface ILogInfoRenderer {
        /// <summary>
        /// Renders the text representation of the specified log data to a text writer.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="writer"></param>
        void Render(ILogInfo info, TextWriter writer);
    }

    /// <summary>
    /// Renders log state object to a text writer.
    /// </summary>
    public interface ILogStateObjectRenderer {
        /// <summary>
        /// Renders the text representation of the specified state object to a text writer.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="stateObj"></param>
        /// <param name="writer"></param>
        void Render(ILogInfo info, object stateObj, TextWriter writer);
    }

    /// <summary>
    /// Renders log state object to a text writer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogStateObjectRenderer<T> {
        /// <summary>
        /// Renders the text representation of the specified state object to a text writer.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="stateObj"></param>
        /// <param name="writer"></param>
        void Render(ILogInfo info, T stateObj, TextWriter writer);
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
    /// Represents the method that will handle an event when an log error occurs.
    /// </summary>
    /// <param name="exception"></param>
    public delegate void LogErrorEventHandler(LogException exception);

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
        /// Raised when an error occurs.
        /// </summary>
        event LogErrorEventHandler Error;

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
        /// Closes this logger. A closed logger can not records any log data.
        /// </summary>
        void Close();
    }

    /// <summary>
    /// Provides a default implementation of ILogInfo.
    /// </summary>
    public class LogInfo : ILogInfo {
        static LogInfo() {
            IPAddress[] addresses = Dns.GetHostAddresses(string.Empty).Where((item) => item.AddressFamily == AddressFamily.InterNetwork || item.AddressFamily == AddressFamily.InterNetworkV6).ToArray();

            g_hostAddress = addresses.Length > 0 ? (addresses.FirstOrDefault((item) => item.AddressFamily == AddressFamily.InterNetwork) ?? addresses.FirstOrDefault((item) => item.AddressFamily == AddressFamily.InterNetworkV6)).ToString() : "unknown";
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

        public override string ToString() {
            return this.StateObject + string.Empty;
        }
    }

    #region Loggers

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

        protected virtual void OnError(Exception ex) {
            if(this.Error == null) {
                return;
            }

            try {
                this.Error(new LogException(ex.Message, ex));
            } catch {
                // ignore all exceptions
            }
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
                } catch(Exception ex) {
                    this.OnError(ex);
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
        public event LogErrorEventHandler Error;

        /// <inheritdoc />
        public virtual void Record(ILogInfo info) {
            this.ThrowIfNullOrDisposed(info);

            try {
                this.InternalRecord(info);
            } catch(Exception ex) {
                this.OnError(ex);
            }
        }

        /// <inheritdoc />
        public virtual void RecordAsync(ILogInfo info) {
            this.ThrowIfNullOrDisposed(info);

            try {
                this.m_logQueue.Enqueue(info);
                this.m_signal.Set();
            } catch(Exception ex) {
                this.OnError(ex);
            }
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
    /// Provides a default implementation of Logger class.
    /// </summary>
    public class DefaultLogger : Logger, ILogInfoFilter, ILogStorageSelector {
        public DefaultLogger(string name, DefaultLoggerStorageOption option)
            : this(name, option != null ? new DefaultLoggerStorageOption[] { option } : null) {
        }

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

        protected override ILogInfoFilter GetLogFilter(ILogInfo info) {
            return this;
        }

        protected override ILogStorageSelector GetStorageSelector(ILogInfo info) {
            return this;
        }

        protected override void DisposingCore() {
            if(this.m_storages != null) {
                foreach(ILogStorage storage in this.m_storages) {
                    storage.Dispose();
                }
            }
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
        public DefaultLoggerStorageOption(ILogStorage storage)
            : this(null, storage) {
        }

        public DefaultLoggerStorageOption(DefaultLoggerFilterRule rule, ILogStorage storage)
            : this(rule != null ? new DefaultLoggerFilterRule[] { rule } : null, storage != null ? new ILogStorage[] { storage } : null) {
        }

        public DefaultLoggerStorageOption(IEnumerable<ILogStorage> storages)
            : this(null, storages) {
        }

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

    #endregion

    #region Log Info Renderers

    /// <summary>
    /// Ignores the log state object and writes nothing to the text stream.
    /// </summary>
    public class IgnoredLogStateObjectRenderer : ILogStateObjectRenderer {
        #region ILogStateObjectRenderer Members

        /// <inheritdoc />
        public void Render(ILogInfo info, object stateObj, TextWriter writer) {
        }

        #endregion
    }

    /// <summary>
    /// Writes the text representation of an object to the text stream by calling ToString on that object.
    /// </summary>
    public class ToStringLogStateObjectRenderer : ILogStateObjectRenderer {
        #region ILogStateObjectRenderer Members

        /// <inheritdoc />
        public void Render(ILogInfo info, object stateObj, TextWriter writer) {
            if(stateObj != null) {
                writer.Write(stateObj);
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a configurable class that implements ILogStateObjectRenderer interface.
    /// 
    /// There are two renderer categories:
    /// 
    /// 1. renderer implements ILogStateObjectRenderer interface is the default renderer for all objects.
    /// 2. renderers implements ILogStateObjectRenderer`1 interface used to render objects of specified type.
    /// </summary>
    public class AggregateLogStateObjectRenderer : ILogStateObjectRenderer {
        public AggregateLogStateObjectRenderer(IEnumerable<ILogStateObjectRenderer> renderers) {
            if(renderers == null) {
                throw new ArgumentNullException("renderers");
            }

            Type baseType = typeof(ILogStateObjectRenderer<>), rendererType = null;
            this.m_objectRenderers = new Dictionary<Type, ILogStateObjectRenderer>();

            foreach(ILogStateObjectRenderer renderer in renderers) {
                if((rendererType = renderer.GetType().GetInterfaces().FirstOrDefault((item) => item.IsGenericType && item.GetGenericTypeDefinition().Equals(baseType))) != null) {
                    this.m_objectRenderers[rendererType.GetGenericArguments()[0]] = renderer;
                } else {
                    this.m_defaultObjectRenderer = renderer;
                }
            }

            this.m_defaultObjectRenderer = this.m_defaultObjectRenderer ?? new IgnoredLogStateObjectRenderer();
        }

        protected ILogStateObjectRenderer m_defaultObjectRenderer;
        protected IDictionary<Type, ILogStateObjectRenderer> m_objectRenderers;

        #region ILogStateObjectRenderer Members

        /// <inheritdoc />
        public virtual void Render(ILogInfo info, object stateObj, TextWriter writer) {
            if(stateObj == null) {
                return;
            }

            Type stateType = stateObj.GetType();

            if(this.m_objectRenderers.ContainsKey(stateType)) {
                this.m_objectRenderers[stateType].Render(info, stateObj, writer);
            } else {
                this.m_defaultObjectRenderer.Render(info, stateObj, writer);
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a base class that implements ILogInfoRenderer interface.
    /// </summary>
    public abstract class LogInfoRenderer : ILogInfoRenderer {
        public LogInfoRenderer()
            : this(null) {
        }

        public LogInfoRenderer(ILogStateObjectRenderer objectRenderer) {
            this.m_objectRenderer = objectRenderer ?? new IgnoredLogStateObjectRenderer();
        }

        protected ILogStateObjectRenderer m_objectRenderer;

        protected abstract void PreRenderStateObject(ILogInfo info, TextWriter writer);

        protected virtual void OnRenderStateObject(ILogInfo info, TextWriter writer) {
            this.m_objectRenderer.Render(info, info.StateObject, writer);
        }

        protected abstract void PostRenderStateObject(ILogInfo info, TextWriter writer);

        #region ILogInfoRenderer Members

        /// <inheritdoc />
        public virtual void Render(ILogInfo info, TextWriter writer) {
            this.PreRenderStateObject(info, writer);
            this.OnRenderStateObject(info, writer);
            this.PostRenderStateObject(info, writer);
        }

        #endregion
    }

    /// <summary>
    /// Separates all parts of log data by a specified character.
    /// </summary>
    public class SeparatedByCharLogInfoRenderer : LogInfoRenderer {
        public SeparatedByCharLogInfoRenderer(char startSeparator, char endSeparator)
            : this(startSeparator, endSeparator, null) {
        }

        public SeparatedByCharLogInfoRenderer(char startSeparator, char endSeparator, ILogStateObjectRenderer objectRenderer)
            : base(objectRenderer) {
            this.m_startSeparator = startSeparator;
            this.m_endSeparator = endSeparator;
            this.m_logTypeNames = new Dictionary<LogInfoType, string>();

            foreach(LogInfoType type in Enum.GetValues(typeof(LogInfoType))) {
                this.m_logTypeNames[type] = EnumUtility.GetDescription(type);
            }
        }

        protected char m_startSeparator;
        protected char m_endSeparator;
        protected IDictionary<LogInfoType, string> m_logTypeNames;

        protected override void PreRenderStateObject(ILogInfo info, TextWriter writer) {
            writer.Write("{0}{2:yyyy-MM-dd HH:mm:ss}{1}{0}{3}{1}{0}{4}{1}", this.m_startSeparator, this.m_endSeparator, info.CreateTime, info.HostAddress, info.HostName);

            if(!string.IsNullOrWhiteSpace(info.OperationSource)) {
                writer.Write("{0}{2}{1}", this.m_startSeparator, this.m_endSeparator, info.OperationSource);
            }
            if(!string.IsNullOrWhiteSpace(info.OperationName)) {
                writer.Write("{0}{2}{1}", this.m_startSeparator, this.m_endSeparator, info.OperationName);
            }

            writer.Write("{0}{2}{1}{0}{3}{1}", this.m_startSeparator, this.m_endSeparator, this.m_logTypeNames[info.LogType], info.LogLevel);

            if(info.StateObject != null) {
                writer.Write(this.m_startSeparator);
            }
        }

        protected override void PostRenderStateObject(ILogInfo info, TextWriter writer) {
            if(info.StateObject != null) {
                writer.Write(this.m_endSeparator);
            }

            if(info.ExceptionObject != null) {
                writer.Write("{0}{2}{1}{0}\r\n{3}{1}", this.m_startSeparator, this.m_endSeparator, info.ExceptionObject.Message, (info.ExceptionObject.InnerException ?? info.ExceptionObject).StackTrace);
            }
            writer.WriteLine();
        }
    }

    #endregion

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

    #region File Storages

    /// <summary>
    /// Represents a file log storage.
    /// </summary>
    public interface IFileLogStorage : ILogStorage {
        /// <summary>
        /// Gets the length in bytes of this file storage.
        /// </summary>
        long FileSize {
            get;
        }

        /// <summary>
        /// Gets the number of saved log data after creating this file storage.
        /// </summary>
        long LogCount {
            get;
        }
    }

    /// <summary>
    /// Gets a existing file storage or create a new.
    /// </summary>
    public interface IFileLogStorageFactory : IDisposable {
        /// <summary>
        /// Gets a existing file storage to record the specified log data. If not existing, return null.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        IFileLogStorage GetStorage(ILogInfo info);

        /// <summary>
        /// Creates a new file storage to record the specified log data.
        /// </summary>
        /// <param name="rootFolderPath"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        IFileLogStorage CreateStorage(string rootFolderPath, ILogInfo info);
    }

    /// <summary>
    /// Represents the capability of a file storage.
    /// </summary>
    public interface IFileLogStorageCapability {
        /// <summary>
        /// Checks whether the limits has exceed.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        bool IsExceed(ILogInfo info, IFileLogStorage storage);
    }

    /// <summary>
    /// Represents how to classify a log data.
    /// </summary>
    public interface IFileLogInfoClassifier {
        /// <summary>
        /// Gets path of the file to save the specified log info.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        string GetFilePath(ILogInfo info, DateTime time);
    }

    /// <summary>
    /// Defines how to create a file log storage factory.
    /// </summary>
    public interface IFileLogStorageFactoryResolver {
        /// <summary>
        /// Creates a new IFileLogStorageFactory object of the specified file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        IFileLogStorageFactory CreateFactory(string filePath, ILogInfo info);
    }

    /// <summary>
    /// Saves all log data to a single file.
    /// </summary>
    public class SingleFileLogStorage : IFileLogStorage {
        public SingleFileLogStorage(string filePath, bool append, ILogInfoRenderer renderer) {
            if(string.IsNullOrWhiteSpace(filePath)) {
                throw new ArgumentException("filePath is null or empty.", "filePath");
            }
            if(renderer == null) {
                throw new ArgumentNullException("renderer");
            }

            FileUtility.CreateFolder(this.m_filePath = filePath);

            this.m_renderer = renderer;
            this.m_writer = new StreamWriter(new FileStream(this.m_filePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8);
        }

        protected string m_filePath;
        protected StreamWriter m_writer;
        protected ILogInfoRenderer m_renderer;
        protected long m_count;

        protected void ThrowIfDisposed() {
            if(this.m_disposed) {
                throw new ObjectDisposedException(this.GetType().Name + ": " + this.m_filePath);
            }
        }

        #region IFileLogStorage Members

        /// <inheritdoc />
        public virtual long FileSize {
            get {
                this.ThrowIfDisposed();

                return this.m_writer.BaseStream.Length;
            }
        }

        /// <inheritdoc />
        public virtual long LogCount {
            get {
                return this.m_count;
            }
        }

        #endregion

        #region ILogStorage Members

        /// <inheritdoc />
        public virtual void Save(ILogInfo info) {
            this.ThrowIfDisposed();

            this.m_renderer.Render(info, this.m_writer);

            Interlocked.Increment(ref this.m_count);
        }

        #endregion

        #region IDisposable Members

        protected volatile bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_writer != null) {
                this.m_writer.Close();
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~SingleFileLogStorage() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }

    /// <summary>
    /// Saves log data to mutiple files. These files can be classified by machine or operation source or other conditions.
    /// </summary>
    public class MultipleFileLogStorage : IFileLogStorage {
        public MultipleFileLogStorage(string folderName, IFileLogStorageFactory storageFactory, IFileLogStorageCapability storageCapability) {
            if(string.IsNullOrWhiteSpace(folderName)) {
                throw new ArgumentException("folderName is null or empty.", "folderName");
            }
            if(storageFactory == null) {
                throw new ArgumentNullException("storageFactory");
            }
            if(storageCapability == null) {
                throw new ArgumentNullException("storageCapability");
            }

            this.m_storageFactory = storageFactory;
            this.m_storageCapability = storageCapability;
            this.m_rootFolderPath = Path.Combine(HostingEnvironment.IsHosted ? HostingEnvironment.ApplicationPhysicalPath : Environment.CurrentDirectory, folderName);
        }

        protected string m_rootFolderPath;
        protected IFileLogStorageFactory m_storageFactory;
        protected IFileLogStorageCapability m_storageCapability;

        protected void ThrowIfDisposed() {
            if(this.m_disposed) {
                throw new ObjectDisposedException(this.GetType().Name + ": " + this.m_rootFolderPath);
            }
        }

        #region IFileLogStorage Members

        /// <inheritdoc />
        public virtual long FileSize {
            get {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public virtual long LogCount {
            get {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ILogStorage Members

        /// <inheritdoc />
        public void Save(ILogInfo info) {
            IFileLogStorage storage = this.m_storageFactory.GetStorage(info);

            if(storage == null) {
                storage = this.m_storageFactory.CreateStorage(this.m_rootFolderPath, info);
            }

            while(this.m_storageCapability.IsExceed(info, storage)) {
                storage = this.m_storageFactory.CreateStorage(this.m_rootFolderPath, info);
            }

            storage.Save(info);
        }

        #endregion

        #region IDisposable Members

        protected volatile bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_storageFactory != null) {
                this.m_storageFactory.Dispose();
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~MultipleFileLogStorage() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }

    /// <summary>
    /// Provides the capability of a single file log storage.
    /// </summary>
    public class SingleFileLogStorageCapability : IFileLogStorageCapability {
        public SingleFileLogStorageCapability(int? maxSize, int? maxCount) {
            this.m_maxSize = maxSize;
            this.m_maxCount = maxCount;
        }

        protected int? m_maxSize;
        protected int? m_maxCount;

        #region IFileLogStorageCapability Members

        /// <inheritdoc />
        public virtual bool IsExceed(ILogInfo info, IFileLogStorage storage) {
            if(storage == null) {
                throw new ArgumentNullException("storage");
            }

            if(this.m_maxSize.HasValue && storage.FileSize >= this.m_maxSize.Value) {
                return true;
            }

            if(this.m_maxCount.HasValue && storage.LogCount >= this.m_maxCount.Value) {
                return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// Records log data from all machines and all sources to "one" file storage.
    /// "one" file storage not means a single file, it may be mutiple files with similar names.
    /// </summary>
    public class CentralizedFileLogStorageFactory : IFileLogStorageFactory {
        public CentralizedFileLogStorageFactory(string fileName, string fileNameFormat, string fileExtension, int? maxReservedFilesCount, ILogInfoRenderer logRenderer)
            : this(fileName, fileNameFormat, fileExtension, false, maxReservedFilesCount, logRenderer) {
        }

        public CentralizedFileLogStorageFactory(string fileName, string fileNameFormat, string fileExtension, bool isAppendFirstFile, int? maxReservedFilesCount, ILogInfoRenderer logRenderer) {
            if(string.IsNullOrWhiteSpace(fileName)) {
                throw new ArgumentException("fileName is null or empty.", "fileName");
            }
            if(string.IsNullOrWhiteSpace(fileExtension)) {
                throw new ArgumentException("fileExtension is null or empty.", "fileExtension");
            }
            if(maxReservedFilesCount.HasValue && maxReservedFilesCount < 0) {
                throw new ArgumentOutOfRangeException("maxReservedFilesCount", "maxReservedFilesCount is less than zero.");
            }
            if(logRenderer == null) {
                throw new ArgumentNullException("logRenderer");
            }

            if(!string.IsNullOrWhiteSpace(fileNameFormat)) {
                this.m_fileName = string.Format(fileNameFormat, fileName, DateTime.Now);
            } else {
                this.m_fileName = fileName;
            }
            this.m_fileExtension = fileExtension;
            this.m_isAppendFirstFile = isAppendFirstFile;
            this.m_maxReservedFilesCount = maxReservedFilesCount;
            this.m_renderer = logRenderer;
        }

        protected string m_fileName;
        protected string m_fileExtension;
        protected bool m_isAppendFirstFile;

        protected int m_reservedFilesCount;
        protected int? m_maxReservedFilesCount;

        protected IFileLogStorage m_storage;
        protected ILogInfoRenderer m_renderer;

        protected virtual string GetFilePath(string rootFolderPath, int index, int integerPlaces) {
            return Path.Combine(rootFolderPath, string.Format("{0}{1}{2}", this.m_fileName, index == 0 ? string.Empty : "_" + index.ToString("D" + integerPlaces), this.m_fileExtension));
        }

        protected virtual void LoopCover(string rootFolderPath) {
            string sourcePath = null, destinationPath = null;
            int integerPlaces = this.m_maxReservedFilesCount.HasValue ? this.m_maxReservedFilesCount.Value.GetIntegerPlaces() : 1;

            for(int i = this.m_maxReservedFilesCount.HasValue ? Math.Min(this.m_maxReservedFilesCount.Value - 1, this.m_reservedFilesCount) : this.m_reservedFilesCount; i >= 0; i--) {
                if(!File.Exists(sourcePath = this.GetFilePath(rootFolderPath, i, integerPlaces))) {
                    continue;
                }

                if(File.Exists(destinationPath = this.GetFilePath(rootFolderPath, i + 1, integerPlaces))) {
                    File.Delete(destinationPath);
                }

                File.Move(sourcePath, destinationPath);
            }
        }

        #region IFileLogStorageFactory Members

        public virtual IFileLogStorage GetStorage(ILogInfo info) {
            return this.m_storage;
        }

        public virtual IFileLogStorage CreateStorage(string rootFolderPath, ILogInfo info) {
            if(this.m_storage != null) {
                this.m_storage.Dispose();
                this.m_storage = null;

                this.LoopCover(rootFolderPath);

                ++this.m_reservedFilesCount;
            }

            return this.m_storage = new SingleFileLogStorage(this.GetFilePath(rootFolderPath, 0, 0), this.m_reservedFilesCount == 0 ? this.m_isAppendFirstFile : false, this.m_renderer);
        }

        #endregion

        #region IDisposable Members

        protected volatile bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_storage != null) {
                this.m_storage.Dispose();
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~CentralizedFileLogStorageFactory() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }

    /// <summary>
    /// Classifies log data by operation source.
    /// </summary>
    public class OperationSourceFileLogInfoClassifier : IFileLogInfoClassifier {
        public OperationSourceFileLogInfoClassifier(bool isClassifyByHost, string fileNameFormat, string fileExtension, string defaultSourceName) {
            if(string.IsNullOrWhiteSpace(fileExtension)) {
                throw new ArgumentException("fileExtension is null or empty.", "fileExtension");
            }
            if(string.IsNullOrWhiteSpace(defaultSourceName)) {
                throw new ArgumentException("defaultSourceName is null or empty.", "defaultSourceName");
            }

            this.m_isClassifyByHost = isClassifyByHost;
            this.m_fileNameFormat = fileNameFormat;
            this.m_fileExtension = fileExtension;
            this.m_defaultSourceName = defaultSourceName;
            this.m_cache = new ConcurrentDictionary<string, string>();
        }

        protected bool m_isClassifyByHost;
        protected string m_fileNameFormat;
        protected string m_fileExtension;
        protected string m_defaultSourceName;
        protected ConcurrentDictionary<string, string> m_cache;

        protected virtual string GetCacheKey(ILogInfo info) {
            return string.Format("{0}:{1}:{2}", info.HostAddress, info.OperationSource, info.LogType);
        }

        #region IFileLogInfoClassifier Members

        /// <inheritdoc />
        public string GetFilePath(ILogInfo info, DateTime time) {
            string filePath = null;
            string key = this.GetCacheKey(info);
            if(this.m_cache.TryGetValue(key, out filePath)) {
                return filePath;
            }

            string sourceName = info.OperationSource != null ? info.OperationSource : this.m_defaultSourceName;
            string fileName = !string.IsNullOrWhiteSpace(this.m_fileNameFormat) ? string.Format(this.m_fileNameFormat, sourceName, EnumUtility.GetDescription(info.LogType), time) : sourceName;

            this.m_cache.TryAdd(key, filePath = (this.m_isClassifyByHost ? Path.Combine(string.Format("{1}({0})", info.HostAddress, info.HostName), fileName) : fileName) + this.m_fileExtension);

            return filePath;
        }

        #endregion
    }

    /// <summary>
    /// Always creates a CentralizedFileLogStorageFactory object for each file path.
    /// </summary>
    public class CentralizedFileLogStorageFactoryResolver : IFileLogStorageFactoryResolver {
        public CentralizedFileLogStorageFactoryResolver(int? maxReservedFilesCount, ILogInfoRenderer logRenderer)
            : this(false, maxReservedFilesCount, logRenderer) {
        }

        public CentralizedFileLogStorageFactoryResolver(bool isAppendFirstFile, int? maxReservedFilesCount, ILogInfoRenderer logRenderer) {
            if(maxReservedFilesCount.HasValue && maxReservedFilesCount < 0) {
                throw new ArgumentOutOfRangeException("maxReservedFilesCount", "maxReservedFilesCount is less than zero.");
            }
            if(logRenderer == null) {
                throw new ArgumentNullException("logRenderer");
            }

            this.m_isAppendFirstFile = isAppendFirstFile;
            this.m_maxReservedFilesCount = maxReservedFilesCount;
            this.m_logRenderer = logRenderer;
        }

        protected bool m_isAppendFirstFile;
        protected int? m_maxReservedFilesCount;
        protected ILogInfoRenderer m_logRenderer;

        #region IFileLogStorageFactoryResolver Members

        /// <inheritdoc />
        public IFileLogStorageFactory CreateFactory(string filePath, ILogInfo info) {
            return new CentralizedFileLogStorageFactory(Path.GetFileNameWithoutExtension(filePath), null, Path.GetExtension(filePath), this.m_maxReservedFilesCount, this.m_logRenderer);
        }

        #endregion
    }

    /// <summary>
    /// Classified storage log data to mutiple files.
    /// </summary>
    public class DistributedFileLogStorageFactory : IFileLogStorageFactory {
        public DistributedFileLogStorageFactory(IFileLogInfoClassifier logClassifier, IFileLogStorageFactoryResolver factoryResolver) {
            if(logClassifier == null) {
                throw new ArgumentNullException("logClassifier");
            }
            if(factoryResolver == null) {
                throw new ArgumentNullException("factoryResolver");
            }

            this.m_createTime = DateTime.Now;
            this.m_logClassifier = logClassifier;
            this.m_factoryResolver = factoryResolver;
            this.m_factories = new Dictionary<string, IFileLogStorageFactory>();
        }

        protected DateTime m_createTime;
        protected IFileLogInfoClassifier m_logClassifier;
        protected IFileLogStorageFactoryResolver m_factoryResolver;
        protected IDictionary<string, IFileLogStorageFactory> m_factories;

        #region IFileLogStorageFactory Members

        public virtual IFileLogStorage GetStorage(ILogInfo info) {
            string filePath = this.m_logClassifier.GetFilePath(info, this.m_createTime);
            if(!this.m_factories.ContainsKey(filePath)) {
                return null;
            }

            return this.m_factories[filePath].GetStorage(info);
        }

        public virtual IFileLogStorage CreateStorage(string rootFolderPath, ILogInfo info) {
            IFileLogStorageFactory factory = null;
            string filePath = this.m_logClassifier.GetFilePath(info, this.m_createTime);
            
            if(!this.m_factories.ContainsKey(filePath)) {
                if((factory = this.m_factoryResolver.CreateFactory(filePath, info)) == null) {
                    throw new LogException(string.Format("Can not find a file storage factory of path: {0}.", filePath));
                }

                this.m_factories[filePath] = factory;
            } else {
                factory = this.m_factories[filePath];
            }

            if(Path.IsPathRooted(filePath)) {
                rootFolderPath = Path.GetDirectoryName(filePath);
            } else {
                rootFolderPath = Path.GetDirectoryName(Path.Combine(rootFolderPath, filePath));
            }

            return factory.CreateStorage(rootFolderPath, info);
        }

        #endregion

        #region IDisposable Members

        protected volatile bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_factories != null) {
                foreach(IFileLogStorageFactory factory in this.m_factories.Values) {
                    factory.Dispose();
                }
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~DistributedFileLogStorageFactory() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }

    #endregion

    #region Console Storage

    /// <summary>
    /// Prints log data to console.
    /// </summary>
    public class ConsoleLogStorage : ILogStorage {
        public ConsoleLogStorage(ILogInfoRenderer renderer)
            : this(renderer, (IEnumerable<ConsoleLogTypeColorOption>) null) {
        }

        public ConsoleLogStorage(ILogInfoRenderer renderer, ConsoleLogTypeColorOption option)
            : this(renderer, option != null ? new ConsoleLogTypeColorOption[] { option } : null) {
        }

        public ConsoleLogStorage(ILogInfoRenderer renderer, IEnumerable<ConsoleLogTypeColorOption> config) {
            if(renderer == null) {
                throw new ArgumentNullException("renderer");
            }

            this.m_renderer = renderer;
            this.m_config = new Dictionary<LogInfoType, IDictionary<int?, ConsoleColor>>();

            if(config != null) {
                NullableKeyDictionary<int?, ConsoleColor> map = null;

                foreach(IGrouping<LogInfoType, ConsoleLogTypeColorOption> group in config.GroupBy((item) => item.Type)) {
                    this.m_config[group.Key] = map = new NullableKeyDictionary<int?, ConsoleColor>();

                    foreach(ConsoleLogTypeColorOption option in group) {
                        map[option.Level] = option.Color;
                    }
                }
            }

            GC.SuppressFinalize(this);
        }

        protected ILogInfoRenderer m_renderer;
        protected IDictionary<LogInfoType, IDictionary<int?, ConsoleColor>> m_config;

        #region ILogStorage Members

        public virtual void Save(ILogInfo info) {
            ConsoleColor? color = null;

            if(this.m_config.ContainsKey(info.LogType)) {
                IDictionary<int?, ConsoleColor> colors = this.m_config[info.LogType];

                if(colors.ContainsKey(info.LogLevel)) {
                    color = colors[info.LogLevel];
                } else if(colors.ContainsKey(null)) {
                    color = colors[null];
                }

                if(color.HasValue) {
                    Console.ForegroundColor = color.Value;
                }
            }

            try {
                this.m_renderer.Render(info, Console.Out);
            } finally {
                if(color.HasValue) {
                    Console.ResetColor();
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

    /// <summary>
    /// Represents the color of specified log type and level.
    /// </summary>
    public class ConsoleLogTypeColorOption {
        public ConsoleLogTypeColorOption(LogInfoType type, int? level, ConsoleColor color) {
            this.m_type = type;
            this.m_level = level;
            this.m_color = color;
        }

        private LogInfoType m_type;
        private int? m_level;
        private ConsoleColor m_color;

        internal LogInfoType Type {
            get {
                return this.m_type;
            }
        }

        internal int? Level {
            get {
                return this.m_level;
            }
        }

        internal ConsoleColor Color {
            get {
                return this.m_color;
            }
        }

        public override string ToString() {
            return string.Format("{0} {1}: {2}", this.m_type, this.m_level.HasValue ? this.m_level.Value.ToString() : "all", this.m_color);
        }
    }

    #endregion

    #region Test Storages

#if DEBUG

    /// <summary>
    /// Always throw a exception for test.
    /// </summary>
    public class ErrorLogStorage : ILogStorage {
        public ErrorLogStorage() {
            GC.SuppressFinalize(this);
        }

        #region ILogStorage Members

        public void Save(ILogInfo info) {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }

#endif

    #endregion
}
