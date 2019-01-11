using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Web.Caching {
    /// <summary>
    /// Provides the logic of checking the expirility of the data item.
    /// </summary>
    /// <typeparam name="T">The data item type.</typeparam>
    public interface IExpirilityProvider<T> {
        /// <summary>
        /// Checks whether the specified data item is expired.
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        bool GetIsExpired(T dataItem);
    }
}
