using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a SELECT clause in SQL server.
    /// </summary>
    internal class SqlServerSelectClause : SqlSelectClause {
        private int m_count = -1;
        /// <inheritdoc />
        public int Count {
            get {
                return this.m_count;
            }
            set {
                this.m_count = Math.Max(-1, value);
            }
        }

        /// <inheritdoc />
        protected override string GetSqlString() {
            return this.m_count < 0 ? base.GetSqlString() : base.GetSqlString().Insert(this.Keyword.Length, string.Format(" TOP {0}", this.m_count));
        }
    }
}
