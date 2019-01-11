using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xphter.Framework.Alibaba;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaGetMemberInfoArguments : IChinaAlibabaApiArguments {
        [ObfuscationAttribute]
        [AlibabaArgument("memberId")]
        public String MemberID {
            get;
            set;
        }
    }
}
