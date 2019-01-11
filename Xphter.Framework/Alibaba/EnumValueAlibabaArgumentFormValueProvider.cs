using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba {
    /// <summary>
    /// Provides the integer value of a Enum as the form value.
    /// </summary>
    public class EnumValueAlibabaArgumentFormValueProvider : IAlibabaArgumentFormValueProvider {
        #region IAlibabaArgumentFormValueProvider Members

        /// <inheritdoc />
        public string GetFormValue(object value) {
            return ((int) value).ToString();
        }

        #endregion
    }
}
