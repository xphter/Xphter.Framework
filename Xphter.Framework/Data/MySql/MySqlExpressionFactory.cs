using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Represents a MySql database.
    /// </summary>
    public class MySqlExpressionFactory : ISqlExpressionFactory {
        #region ISqlExpressionFactory Members

        /// <inheritdoc />
        public IConstantSqlExpressionsFactory ConstantFactory {
            get {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public ISqlObject CreateSource(string name) {
            return new MySqlSource(name);
        }

        /// <inheritdoc />
        public ISqlObject CreateSource(ISqlSelectStatement statement, string name) {
            return new MySqlSource(statement, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlObject source, string name) {
            return new MySqlField(source, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlObject source, string name, string alias) {
            return new MySqlField(source, name, alias);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(string variable, string name) {
            return new MySqlField(variable, name);
        }

        /// <inheritdoc />
        public ISqlObject CreateField(ISqlFunction function, string name) {
            return new MySqlField(function, name);
        }

        /// <inheritdoc />
        public ISqlSelectStatement CreateSelectStatement() {
            return new MySqlSelectStatement();
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
