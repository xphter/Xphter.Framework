using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba {
    /// <summary>
    /// Provides form name and value of a field or property of a arguments object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AlibabaArgumentAttribute : Attribute {
        public AlibabaArgumentAttribute(string formName)
            : this(formName, null) {
        }

        public AlibabaArgumentAttribute(string formName, Type formValueProviderType) {
            this.FormName = formName;
            this.FormValueProviderType = formValueProviderType;
        }

        /// <summary>
        /// Gets or sets the form name.
        /// </summary>
        public string FormName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the form value provider type.
        /// </summary>
        public Type FormValueProviderType {
            get;
            set;
        }
    }
}
