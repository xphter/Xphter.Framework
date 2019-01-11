using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xphter.Framework {
    /// <summary>
    /// Provides a default implementation of IAsyncResult interface.
    /// </summary>
    public class AsyncResult : IAsyncResult {
        public AsyncResult(AsyncCallback callback, object asyncState) {
            this.m_callback = callback;
            this.m_asyncState = asyncState;
        }

        /// <summary>
        /// Whether operation completed.
        /// </summary>
        protected int m_isCompleted;

        /// <summary>
        /// Whether sync completed.
        /// </summary>
        protected bool m_completedSynchronously;

        /// <summary>
        /// The occurred exception.
        /// </summary>
        protected Exception m_exception;

        /// <summary>
        /// The callback delegate.
        /// </summary>
        protected readonly AsyncCallback m_callback;

        /// <summary>
        /// The user state of this async operation.
        /// </summary>
        protected readonly object m_asyncState;

        /// <summary>
        /// The core method to mark operation completed.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="completedSynchronously"></param>
        protected virtual void MarkCompletedCore(Exception exception, bool completedSynchronously) {
            this.m_exception = exception;
            this.m_completedSynchronously = completedSynchronously;

            if(this.m_waitHandle != null) {
                this.m_waitHandle.Set();
            }

            if(this.m_callback != null) {
                this.m_callback(this);
            }
        }

        /// <summary>
        /// Marks the operation to completed.
        /// </summary>
        /// <param name="exception"></param>
        public virtual void MarkCompleted(Exception exception) {
            this.MarkCompleted(exception, false);
        }

        /// <summary>
        /// Marks the operation to completed.
        /// </summary>
        /// <param name="exception"></param>
        public virtual void MarkCompleted(Exception exception, bool completedSynchronously) {
            if(Interlocked.CompareExchange(ref this.m_isCompleted, 1, 0) == 1) {
                return;
            }

            this.MarkCompletedCore(exception, completedSynchronously);
        }

        /// <summary>
        /// Waits operation to completed and throw the captured exception.
        /// </summary>
        public virtual void End() {
            if(!this.IsCompleted) {
                this.AsyncWaitHandle.WaitOne();
                this.AsyncWaitHandle.Close();
                this.m_waitHandle = null;
            }

            if(this.m_exception != null) {
                throw this.m_exception;
            }
        }

        #region IAsyncResult Members

        /// <inheritdoc />
        public object AsyncState {
            get {
                return this.m_asyncState;
            }
        }

        private ManualResetEvent m_waitHandle;
        /// <inheritdoc />
        public WaitHandle AsyncWaitHandle {
            get {
                if(this.m_waitHandle == null) {
                    bool isCompleted = this.IsCompleted;
                    ManualResetEvent e = new ManualResetEvent(isCompleted);
                    if(Interlocked.CompareExchange(ref this.m_waitHandle, e, null) != null) {
                        e.Close();
                        e = null;
                    } else {
                        if(!isCompleted && this.IsCompleted) {
                            this.m_waitHandle.Set();
                        }
                    }
                }

                return this.m_waitHandle;
            }
        }

        /// <inheritdoc />
        public bool CompletedSynchronously {
            get {
                return this.m_completedSynchronously;
            }
        }

        /// <inheritdoc />
        public bool IsCompleted {
            get {
                return Thread.VolatileRead(ref this.m_isCompleted) == 1;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a AsyncResult with a return data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncResult<T> : AsyncResult {
        public AsyncResult(AsyncCallback callback, object asyncState)
            : base(callback, asyncState) {
        }

        /// <summary>
        /// The return result.
        /// </summary>
        private T m_result = default(T);

        /// <summary>
        /// Marks the operation to completed.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="result"></param>
        public virtual void MarkCompleted(Exception exception, T result) {
            if(result is bool) {
                throw new InvalidOperationException("The type of result is boolean, this will result in a call of MarkCompleted(Exception, bool) method.");
            }

            this.MarkCompleted(exception, false, result);
        }

        /// <summary>
        /// Marks the operation to completed.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="result"></param>
        public virtual void MarkCompleted(Exception exception, bool completedSynchronously, T result) {
            if(Interlocked.CompareExchange(ref this.m_isCompleted, 1, 0) == 1) {
                return;
            }

            this.m_result = result;
            this.MarkCompletedCore(exception, completedSynchronously);
        }

        /// <inheritdoc />
        public new T End() {
            base.End();
            return this.m_result;
        }
    }
}
