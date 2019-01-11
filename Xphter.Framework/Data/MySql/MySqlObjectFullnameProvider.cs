using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Data;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Provide fullname of a My SQL server object.
    /// </summary>
    internal class MySqlObjectFullnameProvider : ISqlObjectFullnameProvider {
        #region ISqlObjectFullnameProvider Members

        /// <inheritdoc />
        public string GetFullname(ISqlObject obj) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            return obj.Owner != null ? string.Format("{0}.`{1}`", obj.Owner.Fullname, obj.Name) : string.Format("`{0}`", obj.Name);
        }

        #endregion
    }
}
