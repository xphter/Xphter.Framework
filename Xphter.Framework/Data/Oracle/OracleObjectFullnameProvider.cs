using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Data;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Provide fullname of a Oracle server object.
    /// </summary>
    internal class OracleObjectFullnameProvider : ISqlObjectFullnameProvider {
        #region ISqlObjectFullnameProvider Members

        /// <inheritdoc />
        public string GetFullname(ISqlObject obj) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            return obj.Owner != null ? string.Format("{0}.\"{1}\"", obj.Owner.Fullname, obj.Name.ToUpper()) : string.Format("\"{0}\"", obj.Name.ToUpper());
        }

        #endregion
    }
}
