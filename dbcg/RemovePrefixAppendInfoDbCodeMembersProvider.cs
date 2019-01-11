using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Data;

namespace dbcg {
    public class RemovePrefixAppendInfoDbCodeMembersProvider : IDbCodeMembersProvider {
        #region IDbCodeMembersProvider Members

        public string Name {
            get {
                return "t_Name -> NameInfoMembers, v_Name -> NameViewInfoMembers";
            }
        }

        public string Description {
            get {
                return "Remove the prefix of table or view name and append \"InfoMembers\" to the end of name";
            }
        }

        public string GetMembersTypeName(IDbDataEntity data) {
            string name = data.Name;

            int index = name.IndexOf('_');
            if(index >= 0) {
                name = name.Substring(index + 1);
            }

            return name + (data is IDbViewEntity ? "ViewInfoMembers" : "InfoMembers");
        }

        #endregion
    }
}
