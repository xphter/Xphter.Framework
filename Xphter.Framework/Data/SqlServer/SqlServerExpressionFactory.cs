using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a SQL Server database.
    /// </summary>
    public class SqlServerExpressionFactory : ISqlExpressionFactory {
        /// <summary>
        /// Constant expression factory.
        /// </summary>
        private IConstantSqlExpressionsFactory m_constantFactory = new SqlServerConstantExpressionFactory();

        #region ISqlExpressionFactory Members

        /// <inheritdoc />
        public IConstantSqlExpressionsFactory ConstantFactory {
            get {
                return this.m_constantFactory;
            }
        }

        /// <inheritdoc />
        public ISqlObject CreateSource(string name) {
            return new SqlServerSource(name);
        }

        /// <inheritdoc />
        public ISqlObject CreateSource(ISqlSelectStatement statement, string name) {
            return new SqlServerSource(statement, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlObject source, string name) {
            return new SqlServerField(source, name, null);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlObject source, string name, string alias) {
            return new SqlServerField(source, name, alias);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(string variable, string name) {
            return new SqlServerField(variable, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlFunction function, string name) {
            return new SqlServerField(function, name);
        }

        /// <inheritdoc />
        public ISqlSelectStatement CreateSelectStatement() {
            return new SqlServerSelectStatement();
        }

        /// <inheritdoc />
        public ISqlInsertStatement CreateInsertStatement() {
            return new SqlInsertStatement();
        }

        /// <inheritdoc />
        public ISqlUpdateStatement CreateUpdateStatement() {
            return new SqlUpdateStatement();
        }

        /// <inheritdoc />
        public ISqlDeleteStatement CreateDeleteStatement() {
            return new SqlDeleteStatement();
        }

        #endregion
    }
}
