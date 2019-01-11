using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Caching {
#pragma warning disable 420

    /// <summary>
    /// Provides a pool of some objects.
    /// </summary>
    public class ObjectPool<T> : IDisposable where T : class {
        /// <summary>
        /// Initializes a new instance of ObjectPool class.
        /// </summary>
        /// <param name="provider"></param>
        public ObjectPool(IPoolObjectProvider<T> provider)
            : this(0, provider) {
        }

        /// <summary>
        /// Initializes a new instance of ObjectPool class.
        /// </summary>
        /// <param name="initializeCount"></param>
        /// <param name="maxPoolSize"></param>
        /// <param name="provider"></param>
        public ObjectPool(int initializeCount, IPoolObjectProvider<T> provider) {
            if(initializeCount < 0) {
                throw new ArgumentException("initialize count is less than zero.", "initializeCount");
            }
            if(provider == null) {
                throw new ArgumentNullException("provider");
            }

            this.m_provider = provider;
            this.m_objectQueue = new LockFreeQueue<T>();

            if(initializeCount > 0) {
                for(int i = 0; i < initializeCount; i++) {
                    this.ReturnObject(this.CreateObject());
                }
            }
        }

        protected volatile int m_currentPoolSize;
        protected LockFreeQueue<T> m_objectQueue;
        protected IPoolObjectProvider<T> m_provider;

        /// <summary>
        /// Gets the number of objects created by this pool.
        /// </summary>
        public virtual int CurrentPoolSize {
            get {
                return this.m_currentPoolSize;
            }
        }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <returns></returns>
        protected virtual T CreateObject() {
            return this.m_provider.OnCreate();
        }

        /// <summary>
        /// Gets a available object from pool.
        /// </summary>
        /// <returns></returns>
        public virtual T TakeObject() {
            if(this.m_disposed) {
                throw new ObjectDisposedException("ObjectPool");
            }

            T obj = null;

            if(this.m_objectQueue.TryDequeue(out obj)) {
                Interlocked.Decrement(ref this.m_currentPoolSize);
            } else {
                obj = this.CreateObject();
            }
            this.m_provider.OnTake(obj);

            return obj;
        }

        /// <summary>
        /// Gets a object wrapper from pool. The caller should dispose this wrapper when it was changed to unused.
        /// </summary>
        /// <returns></returns>
        public virtual IObjectPoolItem<T> TakeItem() {
            if(this.m_disposed) {
                throw new ObjectDisposedException("ObjectPool");
            }

            T obj = this.TakeObject();

            return new ObjectPoolItem<T>(obj, this);
        }

        /// <summary>
        /// Returns the object to pool.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void ReturnObject(T obj) {
            if(this.m_disposed) {
                throw new ObjectDisposedException("ObjectPool");
            }
            if(obj == null) {
                throw new ArgumentNullException("obj");
            }

            Interlocked.Increment(ref this.m_currentPoolSize);
            try {
                this.m_provider.OnReturn(obj);
                this.m_objectQueue.Enqueue(obj);
            } catch {
                Interlocked.Decrement(ref this.m_currentPoolSize);
                throw;
            }
        }

        #region IDisposable Members

        private bool m_disposed;
        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            foreach(T item in this.m_objectQueue) {
                this.m_provider.OnRelease(item);
            }
            this.m_objectQueue.Clear();

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~ObjectPool() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion

        /// <summary>
        /// The internal pool item.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        protected class ObjectPoolItem<TItem> : IObjectPoolItem<TItem> where TItem : class {
            public ObjectPoolItem(TItem state, ObjectPool<TItem> pool) {
                this.m_state = state;
                this.m_pool = pool;
            }

            private TItem m_state;
            private ObjectPool<TItem> m_pool;

            #region IObjectPoolItem<TObj> Members

            /// <inheritdoc />
            public TItem State {
                get {
                    return this.m_state;
                }
            }

            #endregion

            #region IDisposable Members

            ~ObjectPoolItem() {
                throw new InvalidOperationException("You should dispose this object.");
            }

            public void Dispose() {
                this.m_pool.ReturnObject(this.m_state);
                GC.SuppressFinalize(this);
            }

            #endregion

            public override string ToString() {
                return this.State.ToString();
            }
        }
    }

    /// <summary>
    /// Provides objects for ObjectPool object.
    /// </summary>
    public interface IPoolObjectProvider<T> where T : class {
        /// <summary>
        /// Invokes when creating a new object.
        /// </summary>
        /// <returns></returns>
        T OnCreate();

        /// <summary>
        /// Invokes when taking a object from pool.
        /// </summary>
        /// <param name="obj"></param>
        void OnTake(T obj);

        /// <summary>
        /// Invokes when returning a object to pool.
        /// </summary>
        /// <param name="obj"></param>
        void OnReturn(T obj);

        /// <summary>
        /// Invokes when release resource of this object.
        /// </summary>
        /// <param name="obj"></param>
        void OnRelease(T obj);
    }

    /// <summary>
    /// Represents a object wrapper in ObjectPool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectPoolItem<T> : IDisposable where T : class {
        /// <summary>
        /// Gets the actual object.
        /// </summary>
        T State {
            get;
        }
    }

    /// <summary>
    /// Provides a empty implementation of IPoolObjectProvider interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmptyPoolObjectProvider<T> : IPoolObjectProvider<T> where T : class, new() {
        #region IPoolObjectProvider<T> Members

        /// <inheritdoc />
        public T OnCreate() {
            return new T();
        }

        /// <inheritdoc />
        public void OnTake(T obj) {
        }

        /// <inheritdoc />
        public void OnReturn(T obj) {
        }

        /// <inheritdoc />
        public void OnRelease(T obj) {
        }

        #endregion
    }

    /// <summary>
    /// Provides a implementation of IPoolObjectProvider interface based some delegates.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public class DelegatePoolObjectProvider<TState, TObject> : IPoolObjectProvider<TObject> where TObject : class {
        /// <summary>
        /// Initializes a new instance of DelegatePoolObjectProvider class.
        /// </summary>
        /// <param name="onCreate"></param>
        public DelegatePoolObjectProvider(Func<TState, TObject> onCreate)
            : this(default(TState), onCreate, null, null, null) {
        }

        /// <summary>
        /// Initializes a new instance of DelegatePoolObjectProvider class.
        /// </summary>
        /// <param name="onCreate"></param>
        public DelegatePoolObjectProvider(TState state, Func<TState, TObject> onCreate)
            : this(state, onCreate, null, null, null) {
        }

        /// <summary>
        /// Initializes a new instance of DelegatePoolObjectProvider class.
        /// </summary>
        /// <param name="onCreate"></param>
        /// <param name="onTake"></param>
        /// <param name="onReturn"></param>
        /// <param name="onRelease"></param>
        public DelegatePoolObjectProvider(TState state, Func<TState, TObject> onCreate, Action<TObject> onTake, Action<TObject> onReturn, Action<TObject> onRelease) {
            if(onCreate == null) {
                throw new ArgumentNullException("onCreate");
            }

            this.m_state = state;
            this.m_onCreate = onCreate;
            this.m_onTake = onTake;
            this.m_onReturn = onReturn;
            this.m_onRelease = onRelease;
        }

        private TState m_state;
        private Func<TState, TObject> m_onCreate;
        private Action<TObject> m_onTake;
        private Action<TObject> m_onReturn;
        private Action<TObject> m_onRelease;

        #region IPoolObjectProvider<T> Members

        /// <inheritdoc />
        public TObject OnCreate() {
            return this.m_onCreate(this.m_state);
        }

        /// <inheritdoc />
        public void OnTake(TObject obj) {
            if(this.m_onTake != null) {
                this.m_onTake(obj);
            }
        }

        /// <inheritdoc />
        public void OnReturn(TObject obj) {
            if(this.m_onReturn != null) {
                this.m_onReturn(obj);
            }
        }

        /// <inheritdoc />
        public void OnRelease(TObject obj) {
            if(this.m_onRelease != null) {
                this.m_onRelease(obj);
            }
        }

        #endregion
    }

#pragma warning restore 420
}
