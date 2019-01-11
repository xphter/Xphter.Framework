using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Web.Caching {
    /// <summary>
    /// Provides a default implementation of IExpirableWrapper class.
    /// </summary>
    /// <typeparam name="T">The data item type.</typeparam>
    /// <exception cref="System.ArgumentException"><paramref name="provider"/> is null.</exception>
    public class ExpirableWrapper<T> : IExpirableWrapper<T> {
        /// <summary>
        /// Initialize a instance of ExpirableWrapper class.
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="provider"></param>
        public ExpirableWrapper(T dataItem, IExpirilityProvider<T> provider) {
            if(provider == null) {
                throw new ArgumentException("The expirility provider is null.", "provider");
            }

            this.DataItem = dataItem;
            this.m_provider = provider;
        }

        /// <summary>
        /// The expirility provider.
        /// </summary>
        private IExpirilityProvider<T> m_provider;

        #region IExpirableWrapper Members

        /// <inheritdoc />
        public T DataItem {
            get;
            private set;
        }

        /// <inheritdoc />
        public bool IsExpired {
            get {
                return this.m_provider.GetIsExpired(this.DataItem);
            }
        }

        #endregion
    }
}
