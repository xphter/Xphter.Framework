using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Represents a LIMIT clause in MySql.
    /// </summary>
    public class MySqlLimitClause : SqlExpression, ISqlClause {
        protected int m_offset;
        /// <summary>
        /// Gets or sets records offset.
        /// </summary>
        public int Offset {
            get {
                return this.m_offset;
            }
            set {
                this.m_offset = Math.Max(0, value);
            }
        }

        protected int m_count = -1;
        /// <summary>
        /// /Gets or sets records count.
        /// If it is undefined, return -1.
        /// </summary>
        public int Count {
            get {
                return this.m_count;
            }
            set {
                this.m_count = Math.Max(-1, value);
            }
        }

        #region ISqlClause Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return "LIMIT";
            }
        }

        /// <inheritdoc />
        public virtual bool IsAvailable {
            get {
                return this.m_count >= 0;
            }
        }

        /// <inheritdoc />
        public override string SqlString {
            get {
                return (this.m_sqlString = this.m_count < 0 ? string.Empty : string.Format("{0} {1}, {2}", this.Keyword, this.m_offset, this.m_count));
            }
        }

        #endregion
    }
}
