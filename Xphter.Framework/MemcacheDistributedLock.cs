using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace Xphter.Framework {
    public class MemcacheDistributedLock : IDistributedLock {
        public MemcacheDistributedLock(IEnumerable<string> servers)
            : this(servers, 10, 20, 10000, DEFAULT_LOCK_TIMEOUT) {
        }

        public MemcacheDistributedLock(IEnumerable<string> servers, int minConnectionsCount, int maxConnectionsCount, int receiveTimeout, int defaultLockTimeout) {
            if(servers == null) {
                throw new ArgumentNullException("servers");
            }

            MemcachedClientConfiguration config = new MemcachedClientConfiguration {
#if DEBUG
                Protocol = MemcachedProtocol.Text,
#else
                Protocol = MemcachedProtocol.Binary,
#endif
            };
            config.SocketPool.MaxPoolSize = maxConnectionsCount;
            config.SocketPool.MinPoolSize = minConnectionsCount;
            
            config.SocketPool.ReceiveTimeout = TimeSpan.FromMilliseconds(receiveTimeout);

            int port = 0;
            IPAddress address = null;
            string[] points = null;
            foreach(string item in servers) {
                if((points = item.Split(':')).Length != 2) {
                    throw new ArgumentException(string.Format("{0} is invalid, the correct format is \"ip:port\".", item), "servers");
                }

                if(!IPAddress.TryParse(points[0], out address)) {
                    address = Dns.GetHostAddresses(points[0]).FirstOrDefault((obj) => obj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                if(address == null) {
                    throw new ArgumentException(string.Format("Can not determine the IP address of {0}.", points[0]), "servers");
                }

                if(!int.TryParse(points[1], out port)) {
                    throw new ArgumentException(string.Format("{0} is not a integer.", points[1]), "servers");
                }

                config.Servers.Add(new IPEndPoint(address, port));
            }

            this.m_client = new MemcachedClient(config);
            this.m_defaultLockTimeout = defaultLockTimeout;
        }

        private const int DEFAULT_LOCK_TIMEOUT = 10000;

        protected MemcachedClient m_client;
        protected int m_defaultLockTimeout;
        protected SpinWait m_spin;

        private bool InternalEnter(string key, int timeout) {
            if(key == null) {
                throw new ArgumentNullException("key");
            }

            bool result = true;
            DateTime start = DateTime.Now;

            while(!this.m_client.Store(StoreMode.Add, key, 1, timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : MemcachedClient.Infinite)) {
                if(!this.m_spin.NextSpinWillYield) {
                    this.m_spin.SpinOnce();
                } else {
                    Thread.Sleep(1);
                }

                if(timeout > 0 && (DateTime.Now - start).TotalMilliseconds >= timeout) {
                    result = false;
                    break;
                }
            }

            return result;
        }

        #region IDistributedLock Members

        public bool Enter(string key) {
            return this.InternalEnter(key, this.m_defaultLockTimeout);
        }

        public bool Enter(string key, int timeout) {
            return this.InternalEnter(key, timeout);
        }

        public void Exit(string key) {
            this.m_client.Remove(key);
        }

        public IDisposable GetLock(string key) {
            return this.GetLock(key, this.m_defaultLockTimeout);
        }

        public IDisposable GetLock(string key, int timeout) {
            this.InternalEnter(key, timeout);

            return new LockObject(key, this);
        }

        #endregion

        #region IDisposable Members

        private bool m_disposed;

        ~MemcacheDistributedLock() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            if(this.m_client != null) {
                this.m_client.Dispose();
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        private class LockObject : IDisposable {
            public LockObject(string key, MemcacheDistributedLock dlcok) {
                this.m_key = key;
                this.m_lock = dlcok;
            }

            private string m_key;
            private MemcacheDistributedLock m_lock;

            #region IDisposable Members

            private bool m_disposed;

            ~LockObject() {
                this.Disposing(false);
            }

            public void Dispose() {
                this.Disposing(true);
            }

            protected virtual void Disposing(bool disposing) {
                if(this.m_disposed) {
                    return;
                }
                this.m_disposed = true;

                if(!this.m_lock.m_disposed) {
                    try {
                        this.m_lock.Exit(this.m_key);
                    } catch {
                    }
                }

                if(disposing) {
                    GC.SuppressFinalize(this);
                }
            }

            #endregion
        }
    }
}
