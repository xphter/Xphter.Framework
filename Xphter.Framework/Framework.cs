using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    #region Async Completed Delegate

    /// <summary>
    /// Represents the method that will be invoke when an async operation has complated.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void AsyncCompletedEventHandler<TResult>(object sender, AsyncCompletedEventArgs<TResult> args);

    /// <summary>
    /// Provides data for XXXCompleted event.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncCompletedEventArgs<TResult> : AsyncCompletedEventArgs {
        /// <summary>
        /// Initialize a new instance of AsyncCompletedEventArgs class.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cancelled"></param>
        /// <param name="userState"></param>
        /// <param name="result"></param>
        public AsyncCompletedEventArgs(Exception error, bool cancelled, object userState, TResult result)
            : base(error, cancelled, userState) {
            this.Result = result;
        }

        private TResult m_result;

        /// <summary>
        /// Gets the result of async operation.
        /// </summary>
        public TResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return this.m_result;
            }
            private set {
                this.m_result = value;
            }
        }
    }

    /// <summary>
    /// Represents the method to process XXXProgressChanged event.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ProgressChangedEventHandler<TData>(object sender, ProgressChangedEventArgs<TData> e);

    /// <summary>
    /// Provides data for XXXProgressChanged event.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ProgressChangedEventArgs<TData> : ProgressChangedEventArgs {
        /// <summary>
        /// Initialize a new instance of ProgressChangedEventArgs class.
        /// </summary>
        /// <param name="progressPercentage"></param>
        /// <param name="userState"></param>
        /// <param name="data"></param>
        public ProgressChangedEventArgs(int progressPercentage, object userState, TData data)
            : base(progressPercentage, userState) {
            this.Data = data;
        }

        /// <summary>
        /// Gets the carried data of this notification.
        /// </summary>
        public TData Data {
            get;
            private set;
        }
    }

    #endregion
}
