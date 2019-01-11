using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a constant SQL expression factory used in SQL server.
    /// </summary>
    public class SqlServerConstantExpressionFactory : IConstantSqlExpressionsFactory {
        #region IConstantSqlExpressionsFactory Members

        /// <inheritdoc />
        public ISqlExpression Create(bool value) {
            return new SqlStringExpression(value ? "1" : "0");
        }

        /// <inheritdoc />
        public ISqlExpression Create(byte value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(char value) {
            return new SqlStringExpression(string.Format("'{0}'", value == '\'' ? "''" : value.ToString()));
        }

        /// <inheritdoc />
        public ISqlExpression Create(short value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(ushort value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(int value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(uint value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(long value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(ulong value) {
            return new SqlStringExpression(value.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(float value) {
            return new SqlStringExpression(value.ToString("0.0000000"));
        }

        /// <inheritdoc />
        public ISqlExpression Create(double value) {
            return new SqlStringExpression(value.ToString("0.000000000000000"));
        }

        /// <inheritdoc />
        public ISqlExpression Create(decimal value) {
            return new SqlStringExpression(value.ToString("0.000000000000000"));
        }

        /// <inheritdoc />
        public ISqlExpression Create(DateTime value) {
            return new SqlStringExpression(value.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        /// <inheritdoc />
        public ISqlExpression Create(string value) {
            return new SqlStringExpression(value != null ? string.Format("'{0}'", SqlServerUtility.EncodeString(value)) : string.Empty);
        }

        /// <inheritdoc />
        public ISqlExpression CreateStartWith(string value) {
            return this.Create(SqlServerUtility.GetBeginLikeString(value));
        }

        /// <inheritdoc />
        public ISqlExpression CreateEndWith(string value) {
            return this.Create(SqlServerUtility.GetEndLikeString(value));
        }

        /// <inheritdoc />
        public ISqlExpression CreateContain(string value) {
            return this.Create(SqlServerUtility.GetContainLikeString(value));
        }

        /// <inheritdoc />
        public ISqlExpression Create(byte[] value) {
            if(value == null || value.Length == 0) {
                return new SqlStringExpression(string.Empty);
            }

            StringBuilder sqlString = new StringBuilder("0x");
            foreach(byte b in value) {
                sqlString.Append(b.ToString("X2"));
            }
            return new SqlStringExpression(sqlString.ToString());
        }

        /// <inheritdoc />
        public ISqlExpression Create(Guid value) {
            return this.Create(value.ToString());
        }

        #endregion
    }
}
