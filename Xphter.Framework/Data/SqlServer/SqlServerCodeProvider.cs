using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xphter.Framework.Data;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide database info in database code for a SQL Server.
    /// </summary>
    public class SqlServerCodeProvider : IDbCodeDbInfoProvider {
        public static readonly string SqlServerUtility_EncodeLikeString = ((MethodCallExpression) (((Expression<Func<string>>) (() => SqlServerUtility.EncodeLikeString(null))).Body)).Method.Name;
        public static readonly string SqlServerUtility_CreateParameter = ((MethodCallExpression) (((Expression<Func<SqlParameter>>) (() => SqlServerUtility.CreateParameter(null, null))).Body)).Method.Name;
        public static readonly string SqlServerUtility_CreateParameterExpression = ((MethodCallExpression) (((Expression<Func<ISqlExpression>>) (() => SqlServerUtility.CreateParameterExpression(null, null))).Body)).Method.Name;

        public static readonly string ISqlObject_Name = ((MemberExpression) (((Expression<Func<ISqlObject, string>>) ((obj) => obj.Name)).Body)).Member.Name;

        public static readonly string SqlServerObjectValue_Object = ((MemberExpression) (((Expression<Func<SqlServerObjectValue, ISqlObject>>) ((obj) => obj.Object)).Body)).Member.Name;
        public static readonly string SqlServerObjectValue_Type = ((MemberExpression) (((Expression<Func<SqlServerObjectValue, SqlDbType>>) ((obj) => obj.Type)).Body)).Member.Name;
        public static readonly string SqlServerObjectValue_Value = ((MemberExpression) (((Expression<Func<SqlServerObjectValue, object>>) ((obj) => obj.Value)).Body)).Member.Name;

        public static readonly string String_IsNullOrWhiteSpace = ((MethodCallExpression) (((Expression<Func<bool>>) (() => string.IsNullOrWhiteSpace(null))).Body)).Method.Name;

        #region IDbCodeDbInfoProvider Members

        /// <inheritdoc />
        public string Name {
            get {
                return "SQL SERVER";
            }
        }

        /// <inheritdoc />
        public string Description {
            get {
                return "Provides info for generating code of SQL SERVER";
            }
        }

        /// <inheritdoc />
        public bool IsSupportGetLastIdentity {
            get {
                return true;
            }
        }

        /// <inheritdoc />
        public void PrepareCodeNamespace(CodeNamespace ns) {
            ns.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
            ns.Imports.Add(new CodeNamespaceImport("Xphter.Framework.Data"));
            ns.Imports.Add(new CodeNamespaceImport("Xphter.Framework.Data.SqlServer"));
        }

        /// <inheritdoc />
        public CodeExpression GetSqlObject(CodeExpression sqlExpression, CodeExpression name) {
            return new CodeObjectCreateExpression(typeof(SqlServerField), sqlExpression, name);
        }

        /// <inheritdoc />
        public CodeExpression GetSqlSource(CodeExpression schema, CodeExpression name) {
            if(schema != null) {
                return new CodeObjectCreateExpression(typeof(SqlServerSource), new CodeObjectCreateExpression(typeof(SqlServerSource), schema), name);
            } else {
                return new CodeObjectCreateExpression(typeof(SqlServerSource), name);
            }
        }

        /// <inheritdoc />
        public CodeExpression GetSubquerySource(CodeExpression selectStatement, CodeExpression name) {
            return new CodeObjectCreateExpression(typeof(SqlServerSource), selectStatement, name);
        }

        /// <inheritdoc />
        public CodeExpression GetSqlField(CodeExpression sqlSource, CodeExpression name) {
            return new CodeObjectCreateExpression(typeof(SqlServerField), sqlSource, name, new CodePrimitiveExpression(null));
        }

        /// <inheritdoc />
        public CodeExpression GetIdentityStatement() {
            return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(SqlServerUtility)), TypeUtility.GetMemberName<object, ISqlSelectStatement>((obj) => SqlServerUtility.GetIdentityStatement(false)), new CodePrimitiveExpression(false));
        }

        /// <inheritdoc />
        public CodeExpression GetIdentityStatement(CodeExpression name) {
            return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(SqlServerUtility)), TypeUtility.GetMemberName<object, ISqlSelectStatement>((obj) => SqlServerUtility.GetIdentityStatement(null)), name);
        }

        /// <inheritdoc />
        public CodeExpression GetLikeString(CodeExpression pattern) {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlServerUtility)),
                SqlServerUtility_EncodeLikeString,
                pattern);
        }

        /// <inheritdoc />
        public CodeExpression GetSqlStatementCommandTextProvider() {
            return new CodeCastExpression(typeof(ISqlStatementCommandTextProvider), new CodePrimitiveExpression(null));
        }

        /// <inheritdoc />
        public CodeExpression GetIdentityFieldValue(CodeExpression objReference, IDbFieldEntity field) {
            return null;
        }

        /// <inheritdoc />
        public CodeExpression GetEnableIdentityInsertStatement(IDbDataEntity data) {
            return new CodeObjectCreateExpression(
                typeof(SqlStringStatement),
                new CodePrimitiveExpression(string.Format("SET IDENTITY_INSERT {0} ON", data.SchemaQualifiedName)));
        }

        /// <inheritdoc />
        public Type GetSqlSourceType() {
            return typeof(SqlServerSource);
        }

        /// <inheritdoc />
        public Type GetSqlSelectStatementType() {
            return typeof(SqlServerSelectStatement);
        }

        /// <inheritdoc />
        public Type GetSqlInsertStatementType() {
            return typeof(SqlInsertStatement);
        }

        /// <inheritdoc />
        public Type GetSqlUpdateStatementType() {
            return typeof(SqlUpdateStatement);
        }

        /// <inheritdoc />
        public Type GetSqlDeleteStatementType() {
            return typeof(SqlDeleteStatement);
        }

        /// <inheritdoc />
        public Type GetDbConnectionType() {
            return typeof(SqlConnection);
        }

        /// <inheritdoc />
        public Type GetDbParameterType() {
            return typeof(SqlParameter);
        }

        /// <inheritdoc />
        public Type GetDbTypeType() {
            return typeof(SqlDbType);
        }

        /// <inheritdoc />
        public CodeExpression GetDbTypeValue(IDbFieldEntity field) {
            return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), Enum.GetName(typeof(SqlDbType), field.SpecificDbType));
        }

        /// <inheritdoc />
        public string GetDbParameterName(IDbDataEntity data) {
            if(!string.IsNullOrWhiteSpace(data.Schema)) {
                return string.Format("@{0}_{1}", data.Schema, data.Name);
            } else {
                return string.Format("@{0}", data.Name);
            }
        }

        /// <inheritdoc />
        public string GetDbParameterName(IDbFieldEntity field) {
            if(!string.IsNullOrWhiteSpace(field.Data.Schema)) {
                return string.Format("@{0}_{1}_{2}", field.Data.Schema, field.Data.Name, field.Name);
            } else {
                return string.Format("@{0}_{1}", field.Data.Name, field.Name);
            }
        }

        /// <inheritdoc />
        public CodeExpression GetDbParameter(CodeExpression name, IDbFieldEntity field, CodeExpression value) {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlServerUtility)),
                SqlServerUtility_CreateParameter, 
                name,
                this.GetDbTypeValue(field),
                value);
        }

        /// <inheritdoc />
        public CodeExpression GetDbParameter(CodeExpression name, CodeExpression dbType, CodeExpression value) {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlServerUtility)),
                SqlServerUtility_CreateParameter,
                name,
                dbType,
                value);
        }

        /// <inheritdoc />
        public CodeExpression GetDbParameterExpression(CodeExpression name, IDbFieldEntity field, CodeExpression value) {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlServerUtility)),
                SqlServerUtility_CreateParameterExpression,
                name,
                this.GetDbTypeValue(field),
                value);
        }

        /// <inheritdoc />
        public CodeExpression GetReadValue(CodeExpression reader, IDbFieldEntity field, CodeExpression index) {
            return null;
        }

        /// <inheritdoc />
        public Type GetSqlObjectValueType() {
            return typeof(SqlServerObjectValue);
        }

        /// <inheritdoc />
        public CodeExpression GetSqlObjectValueObject(CodeExpression value) {
            return new CodePropertyReferenceExpression(value, SqlServerObjectValue_Object);
        }

        /// <inheritdoc />
        public CodeExpression GetSqlObjectValueObjectName(CodeExpression value) {
            return new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(value, SqlServerObjectValue_Object), ISqlObject_Name);
        }

        /// <inheritdoc />
        public CodeExpression GetDbParameterExpressionFromSqlObjectValue(CodeExpression name, CodeExpression type, CodeExpression value) {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlServerUtility)),
                SqlServerUtility_CreateParameterExpression,
                name,
                type,
                value);
        }

        /// <inheritdoc />
        public CodeExpression GetSqlObjectValue(CodeExpression obj, IDbFieldEntity field, CodeExpression value) {
            return new CodeObjectCreateExpression(
                typeof(SqlServerObjectValue),
                obj,
                this.GetDbTypeValue(field),
                value);
        }

        #endregion
    }
}
