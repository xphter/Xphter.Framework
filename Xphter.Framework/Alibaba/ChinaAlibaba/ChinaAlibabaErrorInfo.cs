using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaErrorInfo {
        [ObfuscationAttribute]
        public string error {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string error_message {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string error_description {
            get;
            set;
        }

        public override string ToString() {
            return this.error_message ?? this.error_description;
        }
    }
}
