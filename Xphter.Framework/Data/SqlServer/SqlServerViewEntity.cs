using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    internal class SqlServerViewEntity : DbViewEntity {
        public SqlServerViewEntity(IDbDatabaseEntity database, string name, string schema, IDbViewFieldProvider fieldProvider)
            : base(database, name, schema, fieldProvider) {
        }

        public override string SchemaQualifiedName {
            get {
                return string.Format("[{0}].[{1}]", this.m_schema, this.m_name);
            }
        }

        public override string DatabaseQualifiedName {
            get {
                return string.Format("[{0}].[{1}].[{2}]", this.m_database.Name, this.m_schema, this.m_name);
            }
        }
    }
}
