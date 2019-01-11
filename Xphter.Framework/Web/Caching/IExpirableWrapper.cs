using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Web.Caching {
    /// <summary>
    /// Provides a expirable wrapper of the specified data item.
    /// </summary>
    /// <typeparam name="T">The data item type.</typeparam>
    public interface IExpirableWrapper<T> {
        /// <summary>
        /// Gets the internal data item.
        /// </summary>
        T DataItem {
            get;
        }

        /// <summary>
        /// Gets a value to indicate whether the internal data item is expired.
        /// </summary>
        bool IsExpired {
            get;
        }
    }
}
