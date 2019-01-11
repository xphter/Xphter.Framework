using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Represents a SELECT clause in MySql.
    /// </summary>
    public class MySqlSelectStatement : SqlSelectStatement {
        protected MySqlLimitClause m_limitClause = new MySqlLimitClause();
        /// <summary>
        /// Gets Limit clause.
        /// </summary>
        public MySqlLimitClause LimitClause {
            get {
                return this.m_limitClause;
            }
        }

        #region SqlSelectStatement Members

        /// <inheritdoc />
        protected override void Initialize() {
            base.Initialize();

            this.m_clauses.Add(this.m_limitClause);
        }

        /// <inheritdoc />
        public override int Count {
            get {
                return this.m_limitClause.Count;
            }
            set {
                this.m_limitClause.Count = value;
            }
        }

        #endregion
    }
}
