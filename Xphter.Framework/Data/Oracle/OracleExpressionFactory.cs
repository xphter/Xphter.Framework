using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Represents a Oracle database.
    /// </summary>
    public class OracleExpressionFactory : ISqlExpressionFactory {
        #region ISqlExpressionFactory Members

        /// <inheritdoc />
        public IConstantSqlExpressionsFactory ConstantFactory {
            get {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public ISqlObject CreateSource(string name) {
            return new OracleSource(name);
        }

        /// <inheritdoc />
        public ISqlObject CreateSource(ISqlSelectStatement statement, string name) {
            return new OracleSource(statement, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlObject source, string name) {
            return new OracleField(source, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlObject source, string name, string alias) {
            return new OracleField(source, name, alias);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(string variable, string name) {
            return new OracleField(variable, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlFunction function, string name) {
            return new OracleField(function, name);
        }

        /// <inheritdoc />
        public ISqlSelectStatement CreateSelectStatement() {
            return new OracleSelectStatement();
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
