using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    public interface IDistributedLock : IDisposable {
        /// <summary>
        /// Acquires an exclusive lock on the specified key.
        /// </summary>
        /// <param name="key">The lock key.</param>
        /// <returns>The return value is true if the lock is acquired; otherwise, the output is false.</returns>
        bool Enter(string key);

        /// <summary>
        /// Attempts, for the specified number of milliseconds, to acquire an exclusive lock on the specified key, and return a value that indicates whether the lock was taken.
        /// </summary>
        /// <param name="key">The lock key.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns>The return value is true if the lock is acquired; otherwise, the output is false.</returns>
        bool Enter(string key, int timeout);

        /// <summary>
        /// Releases an exclusive lock on ths specified key.
        /// </summary>
        /// <param name="key">The lock key.</param>
        void Exit(string key);

        /// <summary>
        /// Acquires an exclusive lock on the specified key, and return the object used to automatic releases this lock.
        /// </summary>
        /// <param name="key">The lock key.</param>
        /// <returns></returns>
        IDisposable GetLock(string key);

        /// <summary>
        /// Attempts, for the specified number of milliseconds, to acquires an exclusive lock on the specified key, and return the object used to automatic releases this lock.
        /// </summary>
        /// <param name="key">The lock key.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        IDisposable GetLock(string key, int timeout);
    }
}
