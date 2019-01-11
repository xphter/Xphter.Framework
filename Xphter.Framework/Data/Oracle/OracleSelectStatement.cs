using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Represents a SELECT clause in SQL server.
    /// </summary>
    public class OracleSelectStatement : SqlExpression, ISqlSelectStatement {
        /// <summary>
        /// Initialize a new instance of OracleSelectStatement class.
        /// </summary>
        public OracleSelectStatement() {
            this.m_selectClause.AddExpressions(new SqlAllField(null));
            this.m_orderClause.AddExpression(this.m_rowNumber, SqlOrder.Asc);
        }

        /// <summary>
        /// Name of middle result when query records of specified count.
        /// </summary>
        public const string MIDDLE_RESULT_NAME = "mr";

        /// <summary>
        /// Records count.
        /// </summary>
        protected int m_count = -1;

        /// <summary>
        /// Select statement which can not set count of records.
        /// </summary>
        protected NoCountSelectStatement m_noCountStatement = new NoCountSelectStatement();

        /// <summary>
        /// ROWNUM variable.
        /// </summary>
        protected ISqlObject m_rowNumber = SqlObject.FromVariable("ROWNUM");

        /// <summary>
        /// Select clause.
        /// </summary>
        protected ISqlSelectClause m_selectClause = new SqlSelectClause();

        /// <summary>
        /// From clause.
        /// </summary>
        protected ISqlFromClause m_fromClause = new SqlFromClause();

        /// <summary>
        /// Where clause.
        /// </summary>
        protected ISqlWhereClause m_whereClause = new SqlWhereClause();

        /// <summary>
        /// Order clause.
        /// </summary>
        protected ISqlOrderClause m_orderClause = new SqlOrderClause();

        /// <summary>
        /// Gets SQL string of this statement.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSqlString() {
            if(this.m_count < 0) {
                return this.m_noCountStatement.SqlString;
            } else {
                //prepare clauses
                this.m_fromClause.Source = new OracleSource(this.m_noCountStatement, MIDDLE_RESULT_NAME);
                this.m_whereClause.Condition = this.m_rowNumber.GreaterEqual(new SqlStringExpression(this.m_count.ToString()));

                return string.Format("{0} {1} {2} {3}", this.m_selectClause.SqlString, this.m_fromClause.SqlString, this.m_whereClause.SqlString, this.m_orderClause.SqlString);
            }
        }

        #region ISqlSelectStatement Members

        /// <inheritdoc />
        public virtual string Keyword {
            get {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual bool IsValid {
            get {
                return this.m_noCountStatement.SelectClause.IsAvailable;
            }
        }

        /// <inheritdoc />
        public virtual ReadOnlyCollection<ISqlClause> Clauses {
            get {
                return this.m_noCountStatement.Clauses;
            }
        }

        /// <inheritdoc />
        public virtual int Count {
            get {
                return this.m_count;
            }
            set {
                this.m_count = Math.Max(-1, value);
            }
        }

        /// <inheritdoc />
        public virtual ISqlSelectStatement SetCount(int value) {
            this.Count = value;
            return this;
        }

        /// <inheritdoc />
        public virtual ISqlSelectClause SelectClause {
            get {
                return this.m_noCountStatement.SelectClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlFromClause FromClause {
            get {
                return this.m_noCountStatement.FromClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlWhereClause WhereClause {
            get {
                return this.m_noCountStatement.WhereClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlOrderClause OrderClause {
            get {
                return this.m_noCountStatement.OrderClause;
            }
        }

        /// <inheritdoc />
        public virtual ISqlGroupClause GroupClause {
            get {
                return this.m_noCountStatement.GroupClause;
            }
        }

        #endregion

        #region Internal Class

        /// <summary>
        /// Represents a SELECT statement which can not set count of records.
        /// </summary>
        protected class NoCountSelectStatement : SqlSelectStatement {
            #region SqlSelectStatement Members

            /// <inheritdoc />
            public override int Count {
                get {
                    throw new NotImplementedException();
                }
                set {
                    throw new NotImplementedException();
                }
            }

            #endregion
        }

        #endregion

        /// <inheritdoc />
        public override string SqlString {
            get {
                return this.m_sqlString = this.GetSqlString();
            }
        }

        /// <inheritdoc />
        public override ReadOnlyCollection<IDataParameter> Parameters {
            get {
                return this.m_noCountStatement.Parameters;
            }
        }
    }
}
