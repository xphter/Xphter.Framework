using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a SQL SERVER object and it's value.
    /// </summary>
    public class SqlServerObjectValue {
        /// <summary>
        /// Initialize a new instance of SqlServerObjectValue.
        /// </summary>
        /// <param name="obj">A SQL object.</param>
        /// <param name="type">The SqlDbType of <paramref name="obj"/>.</param>
        /// <param name="value">The value of <paramref name="obj"/>.</param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is null.</exception>
        public SqlServerObjectValue(ISqlObject obj, SqlDbType type, object value) {
            if(obj == null) {
                throw new ArgumentException("SQL object is null.", "obj");
            }

            this.Object = obj;
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Gets the SQL object.
        /// </summary>
        public ISqlObject Object {
            get;
            private set;
        }

        /// <summary>
        /// Gets the SqlDbType of this SQL object.
        /// </summary>
        public SqlDbType Type {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value of this SQL object.
        /// </summary>
        public object Value {
            get;
            private set;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Object.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is SqlServerObjectValue)) {
                return false;
            }
            return this.Object.Equals(((SqlServerObjectValue) obj).Object);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Object.ToString();
        }

        #endregion
    }
}
