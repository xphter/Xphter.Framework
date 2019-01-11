using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a SELECT clause in SQL server.
    /// </summary>
    public class SqlServerSelectStatement : SqlSelectStatement {
        #region SqlSelectStatement Members

        /// <inheritdoc />
        protected override void Initialize() {
            this.m_selectClause = new SqlServerSelectClause();

            base.Initialize();
        }

        /// <inheritdoc />
        public override int Count {
            get {
                return ((SqlServerSelectClause) this.m_selectClause).Count;
            }
            set {
                ((SqlServerSelectClause) this.m_selectClause).Count = value;
            }
        }

        #endregion
    }
}
