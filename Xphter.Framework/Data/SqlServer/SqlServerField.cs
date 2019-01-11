using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a filed in SQL Server.
    /// </summary>
    public class SqlServerField : SqlAliasObject {
        /// <summary>
        /// Initialize a new instance of SqlServerField class from a common expression.
        /// </summary>
        /// <param name="function">A SQL expression.</param>
        /// <param name="name">Field name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerField(ISqlExpression expression, string name)
            : base(new SqlObject(expression, name, new SqlServerObjectFullnameProvider(), new SqlServerObjectNamedSqlStringProvider())) {
        }

        ///// <summary>
        ///// Initialize a new instance of SqlServerField class from a field name.
        ///// </summary>
        ///// <param name="source">A SQL Server source.</param>
        ///// <param name="name">Field name.</param>
        ///// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        //public SqlServerField(ISqlObject source, string name)
        //    : this(source, name, null) {
        //}

        /// <summary>
        /// Initialize a new instance of SqlServerField class from a field name and alias.
        /// </summary>
        /// <param name="source">A SQL Server source.</param>
        /// <param name="name">Field name.</param>
        /// <param name="alias">Field alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerField(ISqlObject source, string name, string alias)
            : base(new SqlObject(source, new SqlStringExpression(name), name, new SqlServerObjectFullnameProvider(), new SqlObjectFullnameSqlStringProvider()), alias, new SqlAliasObjectFullnameProvider(), new SqlServerAliasObjectSqlStringProvider()) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerField class from a server variable.
        /// </summary>
        /// <param name="variable">Server variable.</param>
        /// <param name="name">Field name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="variable"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerField(string variable, string name)
            : this(new SqlStringExpression(variable), name) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerField class from a function.
        /// </summary>
        /// <param name="function">A SQL function.</param>
        /// <param name="name">Field name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerField(ISqlFunction function, string name)
            : this((ISqlExpression) function, name) {
        }

        /// <summary>
        /// Creates a SqlServerField by the specified field with a alias.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static SqlServerField SetAlias(ISqlObject field, string alias) {
            if(field == null) {
                throw new ArgumentNullException("field");
            }

            if(string.IsNullOrWhiteSpace(alias)) {
                throw new ArgumentException("alias is null or empty", "alias");
            }

            return new SqlServerField(field.Owner, field.Name, alias);
        }
    }
}
