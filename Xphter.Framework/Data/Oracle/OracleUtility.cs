using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Provide a utility for ORACLE.
    /// </summary>
    public static class OracleUtility {
        /// <summary>
        /// Initialize OracleUtility class.
        /// </summary>
        static OracleUtility() {
            g_numericType = "number";

            g_timestampType = "timestamp";
            g_timestampRegex = new Regex(@"^timestamp\(\d\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            g_timestampTZRegex = new Regex(@"^timestamp\(\d\) with time zone$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            g_timestampLTZRegex = new Regex(@"^timestamp\(\d\) with local time zone$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            g_intervalYearType = "interval year";
            g_intervalDayType = "interval day";

            //intialize type mapping
            g_dataTypeMapping = new Dictionary<string, Type> {
                {"float", typeof(decimal)},
                {"binary_float", typeof(float)},
                {"binary_double", typeof(double)},
                {"date", typeof(DateTime)},
                {"char", typeof(string)},
                {"varchar", typeof(string)},
                {"varchar2", typeof(string)},
                {"nchar", typeof(string)},
                {"nvarchar2", typeof(string)},
                {"long", typeof(string)},
                {"clob", typeof(string)},
                {"nclob", typeof(string)},
                {"blob", typeof(byte[])},
                {"raw", typeof(byte[])},
                {"long raw", typeof(byte[])},
                {"rowid", typeof(string)},
                {"urowid", typeof(string)},
            };
            g_dbTypeMapping = new Dictionary<string, DbType> {
                {"float", DbType.Decimal},
                {"binary_float", DbType.Single},
                {"binary_double", DbType.Double},
                {"date", DbType.DateTime},
                {"char", DbType.AnsiStringFixedLength},
                {"varchar", DbType.AnsiString},
                {"varchar2", DbType.AnsiString},
                {"nchar", DbType.StringFixedLength},
                {"nvarchar2", DbType.String},
                {"long", DbType.AnsiString},
                {"clob", DbType.AnsiString},
                {"nclob", DbType.String},
                {"blob", DbType.Binary},
                {"raw", DbType.Binary},
                {"long raw", DbType.Binary},
                {"rowid", DbType.AnsiString},
                {"urowid", DbType.AnsiString},
            };
            g_oracleDbTypeMapping = new Dictionary<string, OracleDbType> {
                {"float", OracleDbType.Double},
                {"binary_float", OracleDbType.BinaryFloat},
                {"binary_double", OracleDbType.BinaryDouble},
                {"date", OracleDbType.Date},
                {"char", OracleDbType.Char},
                {"varchar", OracleDbType.Varchar2},
                {"varchar2", OracleDbType.Varchar2},
                {"nchar", OracleDbType.NChar},
                {"nvarchar2", OracleDbType.NVarchar2},
                {"long", OracleDbType.Long},
                {"clob", OracleDbType.Clob},
                {"nclob", OracleDbType.NClob},
                {"blob", OracleDbType.Blob},
                {"raw", OracleDbType.Raw},
                {"long raw", OracleDbType.LongRaw},
                {"rowid", OracleDbType.Char},
                {"urowid", OracleDbType.Char},
            };
        }

        private static readonly string g_numericType;
        private static readonly string g_timestampType;
        private static readonly Regex g_timestampRegex;
        private static readonly Regex g_timestampTZRegex;
        private static readonly Regex g_timestampLTZRegex;
        private static readonly string g_intervalYearType;
        private static readonly string g_intervalDayType;

        /// <summary>
        /// A mapping from database type to .Net runtime type.
        /// </summary>
        private static readonly IDictionary<string, Type> g_dataTypeMapping;

        /// <summary>
        /// A mapping from database type to System.Data.DbType.
        /// </summary>
        private static readonly IDictionary<string, DbType> g_dbTypeMapping;

        /// <summary>
        /// A mapping from database type name to Oracle.ManagedDataAccess.Client.OracleDbType.
        /// </summary>
        private static readonly IDictionary<string, OracleDbType> g_oracleDbTypeMapping;

        /// <summary>
        /// Determines number type by the specified precision and scale.
        /// </summary>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static Type GetNumberType(int? precision, int? scale) {
            if(!precision.HasValue || !scale.HasValue) {
                return typeof(decimal);
            }

            Type type = typeof(decimal);
            if(scale.Value <= 0) {
                if(precision.Value - scale.Value < 5) {
                    type = typeof(short);
                } else if(precision.Value - scale.Value < 10) {
                    type = typeof(int);
                } else if(precision.Value - scale.Value < 19) {
                    type = typeof(long);
                } else {
                    if(precision.Value < 8 && precision.Value - scale.Value <= 38) {
                        type = typeof(float);
                    } else if(precision.Value < 16) {
                        type = typeof(double);
                    }
                }
            } else {
                if(precision.Value < 8 && scale.Value <= 44) {
                    type = typeof(float);
                } else if(precision.Value < 16) {
                    type = typeof(double);
                }
            }

            return type;
        }

        /// <summary>
        /// Create a OracleParameter.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter OracleDbType.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        private static OracleParameter CreateParameterInternal(string name, OracleDbType? type, ParameterDirection? direction, object value) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Parameter name is null or empty.", "name");
            }

            OracleParameter parameter = new OracleParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            if(type.HasValue) {
                parameter.OracleDbType = type.Value;
            }
            if(direction.HasValue) {
                parameter.Direction = direction.Value;
            }
            return parameter;
        }

        /// <summary>
        /// Get corresponding .net type of the speicifed database type.
        /// </summary>
        /// <param name="typeName">Database type name.</param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns>Corresponding .net type of <paramref name="typeName"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is a invalid database type name.</exception>
        public static Type GetRuntimeType(string typeName, int? precision, int? scale) {
            if(string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Database type name is null or empty.", "dbType");
            }

            if(typeName.Equals(g_numericType, StringComparison.OrdinalIgnoreCase)) {
                return GetNumberType(precision, scale);
            } else if(typeName.StartsWith(g_timestampType, StringComparison.OrdinalIgnoreCase)) {
                return typeof(DateTime);
            } else if(typeName.StartsWith(g_intervalYearType, StringComparison.OrdinalIgnoreCase)) {
                return typeof(OracleIntervalYM);
            } else if(typeName.StartsWith(g_intervalDayType, StringComparison.OrdinalIgnoreCase)) {
                return typeof(TimeSpan);
            }

            if(!g_dataTypeMapping.ContainsKey(typeName = typeName.ToLower().Trim())) {
                throw new ArgumentException(string.Format("Database type name \"{0}\" is invalid.", typeName), "dbType");
            }

            return g_dataTypeMapping[typeName];
        }

        /// <summary>
        /// Get corresponding System.Data.DbType of the speicifed database type.
        /// </summary>
        /// <param name="typeName">Database type name.</param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns>Corresponding System.Data.DbType of <paramref name="typeName"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is a invalid database type name.</exception>
        public static DbType GetDbType(string typeName, int? precision, int? scale) {
            if(string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Database type name is null or empty.", "dbType");
            }

            if(typeName.Equals(g_numericType, StringComparison.OrdinalIgnoreCase)) {
                Type type = GetNumberType(precision, scale);
                if(type == typeof(short)) {
                    return DbType.Int16;
                } else if(type == typeof(int)) {
                    return DbType.Int32;
                } else if(type == typeof(long)) {
                    return DbType.Int64;
                } else if(type == typeof(float)) {
                    return DbType.Single;
                } else if(type == typeof(double)) {
                    return DbType.Double;
                } else {
                    return DbType.VarNumeric;
                }
            } else if(typeName.StartsWith(g_timestampType, StringComparison.OrdinalIgnoreCase)) {
                return DbType.DateTime;
            } else if(typeName.StartsWith(g_intervalYearType, StringComparison.OrdinalIgnoreCase)) {
                return DbType.Int64;
            } else if(typeName.StartsWith(g_intervalDayType, StringComparison.OrdinalIgnoreCase)) {
                return DbType.Object;
            }

            if(!g_dbTypeMapping.ContainsKey(typeName = typeName.ToLower().Trim())) {
                throw new ArgumentException(string.Format("Database type name \"{0}\" is invalid.", typeName), "dbType");
            }

            return g_dbTypeMapping[typeName];
        }

        /// <summary>
        /// Gets corresponding Oracle.ManagedDataAccess.Client.OracleDbType of the speicifed database type name.
        /// </summary>
        /// <param name="typeName">Database type name.</param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <returns>Corresponding Oracle.ManagedDataAccess.Client.OracleDbType of <paramref name="typeName"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is a invalid database type name.</exception>
        public static OracleDbType GetOracleDbType(string typeName, int? precision, int? scale) {
            if(string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Database type name is null or empty.", "dbType");
            }

            if(typeName.Equals(g_numericType, StringComparison.OrdinalIgnoreCase)) {
                Type type = GetNumberType(precision, scale);
                if(type == typeof(short)) {
                    return OracleDbType.Int16;
                } else if(type == typeof(int)) {
                    return OracleDbType.Int32;
                } else if(type == typeof(long)) {
                    return OracleDbType.Int64;
                } else if(type == typeof(float)) {
                    return OracleDbType.Single;
                } else if(type == typeof(double)) {
                    return OracleDbType.Double;
                } else {
                    return OracleDbType.Decimal;
                }
            } else if(g_timestampLTZRegex.IsMatch(typeName)) {
                return OracleDbType.TimeStampLTZ;
            } else if(g_timestampTZRegex.IsMatch(typeName)) {
                return OracleDbType.TimeStampTZ;
            } else if(g_timestampRegex.IsMatch(typeName)) {
                return OracleDbType.TimeStamp;
            } else if(typeName.StartsWith(g_intervalYearType, StringComparison.OrdinalIgnoreCase)) {
                return OracleDbType.IntervalYM;
            } else if(typeName.StartsWith(g_intervalDayType, StringComparison.OrdinalIgnoreCase)) {
                return OracleDbType.IntervalDS;
            }

            if(!g_oracleDbTypeMapping.ContainsKey(typeName = typeName.ToLower().Trim())) {
                throw new ArgumentException(string.Format("Database type name \"{0}\" is not supprted.", typeName), "dbType");
            }

            return g_oracleDbTypeMapping[typeName];
        }

        /// <summary>
        /// Creates a OracleParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleParameter CreateParameter(string name, object value) {
            return CreateParameterInternal(name, null, null, value);
        }

        /// <summary>
        /// Creates a OracleParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleParameter CreateParameter(string name, OracleDbType type, object value) {
            return CreateParameterInternal(name, type, null, value);
        }

        /// <summary>
        /// Creates a OracleParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleParameter CreateParameter(string name, OracleDbType type, ParameterDirection direction, object value) {
            return CreateParameterInternal(name, type, direction, value);
        }

        /// <summary>
        /// Creates the SQL expression of a the OracleParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISqlExpression CreateParameterExpression(string name, object value) {
            return SqlStringExpression.FromParameter(CreateParameterInternal(name, null, null, value));
        }

        /// <summary>
        /// Creates the SQL expression of a the OracleParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISqlExpression CreateParameterExpression(string name, OracleDbType type, object value) {
            return SqlStringExpression.FromParameter(CreateParameterInternal(name, type, null, value));
        }

        /// <summary>
        /// Creates the SQL expression of a the OracleParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISqlExpression CreateParameterExpression(string name, OracleDbType type, ParameterDirection direction, object value) {
            return SqlStringExpression.FromParameter(CreateParameterInternal(name, type, direction, value));
        }
    }
}
