using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba {
    /// <summary>
    /// Provides form value of a field or property  of a arguments object
    /// </summary>
    public interface IAlibabaArgumentFormValueProvider {
        /// <summary>
        /// Gets form value of the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetFormValue(object value);
    }
}
