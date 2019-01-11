using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaMap<TReturn> {
        [ObfuscationAttribute]
        public int total;

        [ObfuscationAttribute]
        public bool success;

        [ObfuscationAttribute]
        public TReturn[] toReturn;
    }
}
