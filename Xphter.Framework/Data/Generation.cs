using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Xphter.Framework.Collections;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Data {
    /// <summary>
    /// Based interface for all database entities.
    /// </summary>
    public interface IDbEntity {
        /// <summary>
        /// Gets entity name.
        /// </summary>
        string Name {
            get;
        }
    }

    /// <summary>
    /// Provide credential for a database server.
    /// </summary>
    public class DbCredential {
        /// <summary>
        /// Initialize a new instance of DbCredential class.
        /// Set IntegratedSecurity property to true.
        /// </summary>
        public DbCredential()
            : this(true, null, null) {
        }

        /// <summary>
        /// Initialize a new instance of DbCredential class.
        /// </summary>
        /// <param name="integratedSecurity">Whether use current operation user to login</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public DbCredential(bool integratedSecurity, string userName, string password) {
            this.IntegratedSecurity = integratedSecurity;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// Gets whether use current operation user to login this database server.
        /// </summary>
        public bool IntegratedSecurity {
            get;
            private set;
        }

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string UserName {
            get;
            private set;
        }

        /// <summary>
        /// Gets password.
        /// </summary>
        public string Password {
            get;
            private set;
        }
    }

    /// <summary>
    /// Represents a database server.
    /// </summary>
    public interface IDbSourceEntity : IDbEntity {
        /// <summary>
        /// Gets the credential for login this databaser server.
        /// </summary>
        DbCredential DbCredential {
            get;
        }

        /// <summary>
        /// Gets databases.
        /// </summary>
        IEnumerable<IDbDatabaseEntity> Databases {
            get;
        }
    }

    /// <summary>
    /// Represents a database entity.
    /// </summary>
    public interface IDbDatabaseEntity : IDbEntity {
        /// <summary>
        /// Gets the database server which contains this database.
        /// </summary>
        IDbSourceEntity Source {
            get;
        }

        /// <summary>
        /// Gets tables.
        /// </summary>
        IEnumerable<IDbTableEntity> Tables {
            get;
        }

        /// <summary>
        /// Gets views.
        /// </summary>
        IEnumerable<IDbViewEntity> Views {
            get;
        }
    }

    /// <summary>
    /// Based interface for table and view entity.
    /// </summary>
    public interface IDbDataEntity : IDbEntity {
        /// <summary>
        /// Gets the database which contains this table or view.
        /// </summary>
        IDbDatabaseEntity Database {
            get;
        }

        /// <summary>
        /// Gets schema name of this data.
        /// </summary>
        string Schema {
            get;
        }

        /// <summary>
        /// Gets the schema qualified name.
        /// </summary>
        string SchemaQualifiedName {
            get;
        }

        /// <summary>
        /// Gets the database qualified name.
        /// </summary>
        string DatabaseQualifiedName {
            get;
        }

        /// <summary>
        /// Gets field entities of this data.
        /// </summary>
        IEnumerable<IDbFieldEntity> Fields {
            get;
        }
    }

    /// <summary>
    /// Represents a view entity.
    /// </summary>
    public interface IDbViewEntity : IDbDataEntity {
        /// <summary>
        /// Gets field entities of this view.
        /// </summary>
        IEnumerable<IDbViewFieldEntity> ViewFields {
            get;
        }

        /// <summary>
        /// Gets tables used by this view.
        /// </summary>
        IEnumerable<IDbTableEntity> Tables {
            get;
        }
    }

    /// <summary>
    /// Represents a table entity.
    /// </summary>
    public interface IDbTableEntity : IDbDataEntity {
        /// <summary>
        /// Gets fields of this table.
        /// </summary>
        IEnumerable<IDbTableFieldEntity> TableFields {
            get;
        }

        /// <summary>
        /// Gets primary key constraint.
        /// </summary>
        IDbConstraintEntity PrimaryKey {
            get;
        }

        /// <summary>
        /// Gets unique key constraints.
        /// </summary>
        IEnumerable<IDbConstraintEntity> UniqueKeys {
            get;
        }

        /// <summary>
        /// Gets foreign key constraints.
        /// </summary>
        IEnumerable<IDbConstraintEntity> ForeignKeys {
            get;
        }
    }

    /// <summary>
    /// Based interface for table and view field entity.
    /// </summary>
    public interface IDbFieldEntity : IDbEntity {
        /// <summary>
        /// Gets data entity which contains this field.
        /// </summary>
        IDbDataEntity Data {
            get;
        }

        /// <summary>
        /// Gets index of this field.
        /// </summary>
        int Index {
            get;
        }

        /// <summary>
        /// Gets .net type of this field.
        /// </summary>
        Type Type {
            get;
        }

        /// <summary>
        /// Gets the common System.Data.DbType of this field.
        /// </summary>
        DbType DbType {
            get;
        }

        /// <summary>
        /// Gets the specific DbType of this field, such as SqlDbType, MySqlDbTyp, OracleDbType.
        /// </summary>
        int SpecificDbType {
            get;
        }

        /// <summary>
        /// Gets database type name of this field.
        /// </summary>
        string DatabaseType {
            get;
        }

        /// <summary>
        /// Gets max length of this field.
        /// </summary>
        int MaxLength {
            get;
        }

        /// <summary>
        /// Gets whether this field can be null.
        /// </summary>
        bool IsNullable {
            get;
        }
    }

    /// <summary>
    /// Represents a table field.
    /// </summary>
    public interface IDbTableFieldEntity : IDbFieldEntity {
        /// <summary>
        /// Gets table which contains this field.
        /// </summary>
        IDbTableEntity Table {
            get;
        }

        /// <summary>
        /// Gets description of this field.
        /// </summary>
        string Description {
            get;
        }

        /// <summary>
        /// Gets whether this field has a default value defination.
        /// </summary>
        bool HasDefaultValue {
            get;
        }

        /// <summary>
        /// Gets whether this field is identity column.
        /// </summary>
        bool IsIdentity {
            get;
        }

        /// <summary>
        /// Gets whether this field is read-only.
        /// </summary>
        bool IsReadOnly {
            get;
        }

        /// <summary>
        /// Gets the constraints on this field.
        /// </summary>
        IEnumerable<IDbConstraintEntity> Constraints {
            get;
        }

        /// <summary>
        /// Gets referenced field if this field is a foreign key.
        /// </summary>
        IDbTableFieldEntity ReferencedField {
            get;
        }

        /// <summary>
        /// Gets related fields whose reference this field.
        /// </summary>
        IEnumerable<IDbTableFieldEntity> RelatedFields {
            get;
        }
    }

    /// <summary>
    /// Represents a view field.
    /// </summary>
    public interface IDbViewFieldEntity : IDbFieldEntity {
        /// <summary>
        /// Gets view which contains this field.
        /// </summary>
        IDbViewEntity View {
            get;
        }

        /// <summary>
        /// Gets referenced table.
        /// </summary>
        IDbTableEntity Table {
            get;
        }

        /// <summary>
        /// Gets referenced field.
        /// </summary>
        IDbTableFieldEntity TableField {
            get;
        }
    }

    /// <summary>
    /// Represents a factory for creating database entities.
    /// </summary>
    public interface IDbEntityFactory {
        /// <summary>
        /// Create a database source entity.
        /// </summary>
        /// <param name="name">Database source name.</param>
        /// <param name="credential">Credential used to login this database source.</param>
        /// <returns>A IDbSourceEntity object.</returns>
        IDbSourceEntity CreateSource(string name, DbCredential credential);
    }

    /// <summary>
    /// Represents a constraint type in database.
    /// </summary>
    public enum DbConstraintType {
        /// <summary>
        /// This is not a constraint.
        /// </summary>
        None,

        /// <summary>
        /// This is a primary key constraint.
        /// </summary>
        PrimaryKey,

        /// <summary>
        /// This is unique key constraint.
        /// </summary>
        UniqueKey,

        /// <summary>
        /// This is a foreign key constraint.
        /// </summary>
        ForeignKey,
    }

    /// <summary>
    /// Represents a constraint entity.
    /// </summary>
    public interface IDbConstraintEntity : IDbEntity {
        /// <summary>
        /// Gets the table which contains this constraint entity.
        /// </summary>
        IDbTableEntity Table {
            get;
        }

        /// <summary>
        /// Gets constraint type.
        /// </summary>
        DbConstraintType Type {
            get;
        }

        /// <summary>
        /// Gets a value to indicate whether this is a unique constraint.
        /// </summary>
        bool IsUnique {
            get;
        }

        /// <summary>
        /// Gets the fields associated with this constraint.
        /// </summary>
        IEnumerable<IDbTableFieldEntity> Fields {
            get;
        }

        /// <summary>
        /// Gets the referenced table of this constraint.
        /// </summary>
        IDbTableEntity ReferenceTable {
            get;
        }

        /// <summary>
        /// Determines whether the two constraint entity have same fields.
        /// </summary>
        /// <param name="constraint">A constraint entity.</param>
        /// <returns>Returns true if <paramref name="constraint"/> has  the same fields as this constraint.</returns>
        bool HasSameFields(IDbConstraintEntity constraint);
    }

    /// <summary>
    /// Provide base class for all database entities.
    /// </summary>
    public class DbEntity : IDbEntity {
        /// <summary>
        /// Initalize a new instance of DbEntity class.
        /// </summary>
        /// <param name="name">Entity name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public DbEntity(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Entity name is null or empty.", "name");
            }

            this.m_name = name;
        }

        /// <summary>
        /// Entity name.
        /// </summary>
        protected string m_name;

        #region IDbEntity Members

        /// <inheritdoc />
        public string Name {
            get {
                return this.m_name;
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is DbEntity)) {
                return false;
            }
            return string.Equals(this.Name, ((DbEntity) obj).Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Name ?? string.Empty;
        }

        #endregion
    }

    /// <summary>
    /// Represents a database source.
    /// </summary>
    public class DbSourceEntity : DbEntity, IDbSourceEntity {
        /// <summary>
        /// Initialize a new instance of DbSourceEntity class.
        /// </summary>
        /// <param name="name">Database source name.</param>
        /// <param name="credential">Credential used to login this databse source.</param>
        /// <param name="databaseProvider">A IDbDatabaseEntityProvider object.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseProvider"/> is null.</exception>
        public DbSourceEntity(string name, DbCredential credential, IDbDatabaseEntityProvider databaseProvider)
            : base(name) {
            if(databaseProvider == null) {
                throw new ArgumentException("Database provider is null.", "databaseProvider");
            }

            this.m_credential = credential ?? new DbCredential();
            this.m_databaseProvider = databaseProvider;
        }

        /// <summary>
        /// Database source credential
        /// </summary>
        private DbCredential m_credential;

        /// <summary>
        /// Databases provider.
        /// </summary>
        private IDbDatabaseEntityProvider m_databaseProvider;

        /// <summary>
        /// Databases.
        /// </summary>
        private IEnumerable<IDbDatabaseEntity> m_databases;

        /// <summary>
        /// Gets databases provider.
        /// </summary>
        public IDbDatabaseEntityProvider DatabaseProvider {
            get {
                return this.m_databaseProvider;
            }
        }

        #region IDbSourceEntity Members

        /// <inheritdoc />
        public virtual DbCredential DbCredential {
            get {
                return this.m_credential;
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<IDbDatabaseEntity> Databases {
            get {
                return this.m_databases ?? (this.m_databases = this.m_databaseProvider.GetDatabases(this));
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a database.
    /// </summary>
    public class DbDatabaseEntity : DbEntity, IDbDatabaseEntity {
        /// <summary>
        /// Initialize a new instance of DbDatabaseEntity class.
        /// </summary>
        /// <param name="source">A database source.</param>
        /// <param name="name">Database name.</param>
        /// <param name="dataProvider">A IDbDataEntityProvider object.</param>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="dataProvider"/> is null.</exception>
        public DbDatabaseEntity(IDbSourceEntity source, string name, IDbDataEntityProvider dataProvider)
            : base(name) {
            if(source == null) {
                throw new ArgumentException("Database source of this database is null.", "source");
            }
            if(dataProvider == null) {
                throw new ArgumentException("Table and view provider is null.", "dataProvider");
            }
            this.m_source = source;
            this.m_dataProvider = dataProvider;
        }

        /// <summary>
        /// Database source.
        /// </summary>
        private IDbSourceEntity m_source;

        /// <summary>
        /// Tables.
        /// </summary>
        private IEnumerable<IDbTableEntity> m_tables;

        /// <summary>
        /// Views.
        /// </summary>
        private IEnumerable<IDbViewEntity> m_views;

        /// <summary>
        /// Table and view provider.
        /// </summary>
        private IDbDataEntityProvider m_dataProvider;

        /// <summary>
        /// Gets table and view provider.
        /// </summary>
        public IDbDataEntityProvider DataProvider {
            get {
                return this.m_dataProvider;
            }
        }

        #region IDbDatabaseEntity Members

        /// <inheritdoc />
        public IDbSourceEntity Source {
            get {
                return this.m_source;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbTableEntity> Tables {
            get {
                return this.m_tables ?? (this.m_tables = this.m_dataProvider.GetTables(this));
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbViewEntity> Views {
            get {
                return this.m_views ?? (this.m_views = this.m_dataProvider.GetViews(this));
            }
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is DbDatabaseEntity)) {
                return false;
            }
            DbDatabaseEntity entity = (DbDatabaseEntity) obj;
            return this.Source.Equals(entity.Source) && string.Equals(this.Name, entity.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0}.{1}", this.Source, this.Name);
        }

        #endregion
    }

    /// <summary>
    /// Represents table entity.
    /// </summary>
    internal abstract class DbTableEntity : DbEntity, IDbTableEntity {
        /// <summary>
        /// Initialize a new instance of DbTableEntity class.
        /// </summary>
        /// <param name="database">A database.</param>
        /// <param name="name">Table name.</param>
        /// <param name="schema">Table schema.</param>
        /// <param name="fieldProvider">Table field provider.</param>
        /// <exception cref="System.ArgumentException"><paramref name="database"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="fieldProvider"/> is null.</exception>
        public DbTableEntity(IDbDatabaseEntity database, string name, string schema, IDbTableFieldProvider fieldProvider)
            : base(name) {
            if(database == null) {
                throw new ArgumentException("Database of this table is null.", "database");
            }
            if(fieldProvider == null) {
                throw new ArgumentException("Table field provider is null.", "fieldProvider");
            }

            this.m_database = database;
            this.m_schema = schema;
            this.m_fieldProvider = fieldProvider;
        }

        /// <summary>
        /// Database entity.
        /// </summary>
        protected IDbDatabaseEntity m_database;

        /// <summary>
        /// Schema name.
        /// </summary>
        protected string m_schema;

        /// <summary>
        /// Fields provider.
        /// </summary>
        protected IDbTableFieldProvider m_fieldProvider;

        /// <summary>
        /// Fields.
        /// </summary>
        protected IEnumerable<IDbFieldEntity> m_fields;

        /// <summary>
        /// Table fields.
        /// </summary>
        protected IEnumerable<IDbTableFieldEntity> m_tableFields;

        /// <summary>
        /// Table primary key fields.
        /// </summary>
        protected IEnumerable<IDbConstraintEntity> m_primaryKeys;

        /// <summary>
        /// Table unique key fields.
        /// </summary>
        protected IEnumerable<IDbConstraintEntity> m_uniqueKeys;

        /// <summary>
        /// Table foreign key fields.
        /// </summary>
        protected IEnumerable<IDbConstraintEntity> m_foreignKeys;

        /// <summary>
        /// Gets field provider.
        /// </summary>
        public IDbTableFieldProvider FieldProvider {
            get {
                return this.m_fieldProvider;
            }
        }

        #region IDbTableEntity Members

        /// <inheritdoc />
        public IEnumerable<IDbFieldEntity> Fields {
            get {
                return this.m_fields ?? (this.m_fields = new List<IDbFieldEntity>(this.TableFields));
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbTableFieldEntity> TableFields {
            get {
                return this.m_tableFields ?? (this.m_tableFields = this.m_fieldProvider.GetFields(this));
            }
        }

        /// <inheritdoc />
        public IDbConstraintEntity PrimaryKey {
            get {
                if(this.m_primaryKeys == null) {
                    this.m_primaryKeys = new List<IDbConstraintEntity>((from field in this.TableFields
                                                                        from constraint in field.Constraints
                                                                        where constraint.Type == DbConstraintType.PrimaryKey
                                                                        select constraint).Distinct());
                }
                return this.m_primaryKeys.FirstOrDefault();
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbConstraintEntity> UniqueKeys {
            get {
                return this.m_uniqueKeys ?? (this.m_uniqueKeys = new List<IDbConstraintEntity>((from field in this.TableFields
                                                                                                from constraint in field.Constraints
                                                                                                where constraint.Type == DbConstraintType.UniqueKey
                                                                                                select constraint).Distinct()));
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbConstraintEntity> ForeignKeys {
            get {
                return this.m_foreignKeys ?? (this.m_foreignKeys = new List<IDbConstraintEntity>((from field in this.TableFields
                                                                                                  from constraint in field.Constraints
                                                                                                  where constraint.Type == DbConstraintType.ForeignKey
                                                                                                  select constraint).Distinct()));
            }
        }

        /// <inheritdoc />
        public IDbDatabaseEntity Database {
            get {
                return this.m_database;
            }
        }

        /// <inheritdoc />
        public string Schema {
            get {
                return this.m_schema;
            }
        }

        /// <inheritdoc />
        public abstract string SchemaQualifiedName {
            get;
        }

        /// <inheritdoc />
        public abstract string DatabaseQualifiedName {
            get;
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is DbTableEntity)) {
                return false;
            }
            DbTableEntity entity = (DbTableEntity) obj;
            return this.Database.Equals(entity.Database) && string.Equals(this.Name, entity.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.DatabaseQualifiedName;
        }

        #endregion
    }

    /// <summary>
    /// Represents view entity.
    /// </summary>
    internal abstract class DbViewEntity : DbEntity, IDbViewEntity {
        /// <summary>
        /// Initialize a new instance of DbViewEntity class.
        /// </summary>
        /// <param name="database">A database.</param>
        /// <param name="name">View name.</param>
        /// <param name="schema">View schema.</param>
        /// <param name="fieldProvider">View field provider.</param>
        /// <exception cref="System.ArgumentException"><paramref name="database"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="fieldProvider"/> is null.</exception>
        public DbViewEntity(IDbDatabaseEntity database, string name, string schema, IDbViewFieldProvider fieldProvider)
            : base(name) {
            if(database == null) {
                throw new ArgumentException("Database of this view is null.", "database");
            }
            if(fieldProvider == null) {
                throw new ArgumentException("View field provider is null.", "fieldProvider");
            }

            this.m_database = database;
            this.m_schema = schema;
            this.m_fieldProvider = fieldProvider;
        }

        /// <summary>
        /// Database entity.
        /// </summary>
        protected IDbDatabaseEntity m_database;

        /// <summary>
        /// Schema name.
        /// </summary>
        protected string m_schema;

        /// <summary>
        /// Fields provider.
        /// </summary>
        protected IDbViewFieldProvider m_fieldProvider;

        /// <summary>
        /// Fields.
        /// </summary>
        protected IEnumerable<IDbFieldEntity> m_fields;

        /// <summary>
        /// Table fields.
        /// </summary>
        protected IEnumerable<IDbViewFieldEntity> m_viewFields;

        /// <summary>
        /// Gets fields provider.
        /// </summary>
        public IDbViewFieldProvider FieldProvider {
            get {
                return this.m_fieldProvider;
            }
        }

        #region IDbViewEntity Members

        /// <inheritdoc />
        public IEnumerable<IDbFieldEntity> Fields {
            get {
                return this.m_fields ?? (this.m_fields = new List<IDbFieldEntity>(this.ViewFields));
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbViewFieldEntity> ViewFields {
            get {
                return this.m_viewFields ?? (this.m_viewFields = this.m_fieldProvider.GetFields(this));
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbTableEntity> Tables {
            get {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public IDbDatabaseEntity Database {
            get {
                return this.m_database;
            }
        }

        /// <inheritdoc />
        public string Schema {
            get {
                return this.m_schema;
            }
        }

        /// <inheritdoc />
        public abstract string SchemaQualifiedName {
            get;
        }

        /// <inheritdoc />
        public abstract string DatabaseQualifiedName {
            get;
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is DbViewEntity)) {
                return false;
            }
            DbViewEntity entity = (DbViewEntity) obj;
            return this.Database.Equals(entity.Database) && string.Equals(this.Name, entity.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.DatabaseQualifiedName;
        }

        #endregion
    }

    /// <summary>
    /// Provide databases for a database source entity.
    /// </summary>
    public interface IDbDatabaseEntityProvider {
        /// <summary>
        /// Gets databases in the specified database source.
        /// </summary>
        /// <param name="source">A database source.</param>
        /// <returns>Databases in <paramref name="source"/>.</returns>
        IEnumerable<IDbDatabaseEntity> GetDatabases(IDbSourceEntity source);
    }

    /// <summary>
    /// Provide table and view data for a database entity.
    /// </summary>
    public interface IDbDataEntityProvider {
        /// <summary>
        /// Gets tables in the specified database.
        /// </summary>
        /// <param name="database">A database.</param>
        /// <returns>Tables in <paramref name="database"/>.</returns>
        IEnumerable<IDbTableEntity> GetTables(IDbDatabaseEntity database);

        /// <summary>
        /// Gets views in the specified database.
        /// </summary>
        /// <param name="database">A database.</param>
        /// <returns>Views in <paramref name="database"/>.</returns>
        IEnumerable<IDbViewEntity> GetViews(IDbDatabaseEntity database);
    }

    /// <summary>
    /// Provide fields for a table entity.
    /// </summary>
    public interface IDbTableFieldProvider {
        /// <summary>
        /// Gets fields in the specified table.
        /// </summary>
        /// <param name="table">A table.</param>
        /// <returns>Fields in <paramref name="table"/>.</returns>
        IEnumerable<IDbTableFieldEntity> GetFields(IDbTableEntity table);
    }

    /// <summary>
    /// Provide fields for a view entity.
    /// </summary>
    public interface IDbViewFieldProvider {
        /// <summary>
        /// Gets fields in the specified view.
        /// </summary>
        /// <param name="view">A view.</param>
        /// <returns>Fields in <paramref name="view"/>.</returns>
        IEnumerable<IDbViewFieldEntity> GetFields(IDbViewEntity view);
    }

    /// <summary>
    /// Provide base class for table and view field entity.
    /// </summary>
    internal abstract class DbFieldEntity : DbEntity, IDbFieldEntity {
        /// <summary>
        /// Initialize a new instance of DbFieldEntity class.
        /// </summary>
        /// <param name="data">Data entity.</param>
        /// <param name="name">Field name.</param>
        /// <param name="index">Field index.</param>
        /// <param name="type">Field type.</param>
        /// <param name="dbType">Field DbType.</param>
        /// <param name="specificDbType">The specific DbType.</param>
        /// <param name="databaseType">Field database type.</param>
        /// <param name="maxLength">Maximum length of this field.</param>
        /// <param name="isNullable">Whether this field can be null.</param>
        public DbFieldEntity(IDbDataEntity data, string name, int index, Type type, DbType dbType, int specificDbType, string databaseType, int maxLength, bool isNullable)
            : base(name) {
            this.Data = data;
            this.Index = index;
            this.Type = type;
            this.DbType = dbType;
            this.SpecificDbType = specificDbType;
            this.DatabaseType = databaseType;
            this.MaxLength = maxLength;
            this.IsNullable = isNullable;
        }

        #region IDbFieldEntity Members

        /// <inheritdoc />
        public IDbDataEntity Data {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual int Index {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual Type Type {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual DbType DbType {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual int SpecificDbType {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual string DatabaseType {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual int MaxLength {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual bool IsNullable {
            get;
            protected set;
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is DbFieldEntity)) {
                return false;
            }
            DbFieldEntity entity = (DbFieldEntity) obj;
            return this.Data.Equals(entity.Data) && string.Equals(this.Name, entity.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0}.{1}", this.Data, this.Name);
        }

        #endregion
    }

    /// <summary>
    /// Represents a table field entity.
    /// </summary>
    internal class DbTableFieldEntity : DbFieldEntity, IDbTableFieldEntity {
        /// <summary>
        /// Initialize a new instance of DbTableFieldEntity class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="index">Field index.</param>
        /// <param name="type">Field type.</param>
        /// <param name="dbType">Field DbType.</param>
        /// <param name="specificDbType">Specific DbType.</param>
        /// <param name="databaseType">Field database type.</param>
        /// <param name="maxLength">Maximum length of this field.</param>
        /// <param name="isNullable">Whether this field can be null.</param>
        /// <param name="table">Table which contains this field.</param>
        /// <param name="description">Field description.</param>
        /// <param name="hasDefaultValue">Whether this field has a default value defination.</param>
        /// <param name="isIdentity">Whether this field is a identity column.</param>
        /// <param name="isReadOnly">Whether this field is a read-only.</param>
        /// <param name="constraints">Constraints on this field.</param>
        /// <param name="referencedField">Referenced field of foreign key.</param>
        public DbTableFieldEntity(string name, int index, Type type, DbType dbType, int specificDbType, string databaseType, int maxLength, bool isNullable, IDbTableEntity table, string description, bool hasDefaultValue, bool isIdentity, bool isReadOnly, IEnumerable<IDbConstraintEntity> constraints, IDbTableFieldEntity referencedField)
            : base(table, name, index, type, dbType, specificDbType, databaseType, maxLength, isNullable) {
            this.Table = table;
            this.Description = description;
            this.HasDefaultValue = hasDefaultValue;
            this.IsIdentity = isIdentity;
            this.IsReadOnly = isReadOnly;
            this.ReferencedField = referencedField;

            this.m_constraints = new List<IDbConstraintEntity>(constraints);
        }

        /// <summary>
        /// Initialize a new instance of DbTableFieldEntity class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="index">Field index.</param>
        /// <param name="type">Field type.</param>
        /// <param name="dbType">Field DbType.</param>
        /// <param name="specificDbType">Specific DbType.</param>
        /// <param name="databaseType">Field database type.</param>
        /// <param name="maxLength">Maximum length of this field.</param>
        /// <param name="isNullable">Whether this field can be null.</param>
        /// <param name="table">Table which contains this field.</param>
        /// <param name="description">Field description.</param>
        /// <param name="hasDefaultValue">Whether this field has a default value defination.</param>
        /// <param name="isIdentity">Whether this field is a identity column.</param>
        /// <param name="isReadOnly">Whether this field is a read-only.</param>
        /// <param name="constraints">Constraints on this field.</param>
        /// <param name="referencedField">Referenced field of foreign key.</param>
        internal DbTableFieldEntity(string name, int index, Type type, DbType dbType, int specificDbType, string databaseType, int maxLength, bool isNullable, IDbTableEntity table, string description, bool hasDefaultValue, bool isIdentity, bool isReadOnly, IDbTableFieldEntity referencedField)
            : base(table, name, index, type, dbType, specificDbType, databaseType, maxLength, isNullable) {
            this.Table = table;
            this.Description = description;
            this.HasDefaultValue = hasDefaultValue;
            this.IsIdentity = isIdentity;
            this.IsReadOnly = isReadOnly;
            this.ReferencedField = referencedField;

            this.m_constraints = new List<IDbConstraintEntity>();
        }

        /// <summary>
        /// Constraints.
        /// </summary>
        private List<IDbConstraintEntity> m_constraints;

        /// <summary>
        /// Related fields.
        /// </summary>
        private IEnumerable<IDbTableFieldEntity> m_relatedFields;

        #region IDbTableFieldEntity Members

        /// <inheritdoc />
        public virtual IDbTableEntity Table {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual string Description {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual bool HasDefaultValue {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual bool IsIdentity {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual bool IsReadOnly {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual IEnumerable<IDbConstraintEntity> Constraints {
            get {
                return this.m_constraints;
            }
        }

        /// <inheritdoc />
        public virtual IDbTableFieldEntity ReferencedField {
            get;
            internal set;
        }

        /// <inheritdoc />
        public IEnumerable<IDbTableFieldEntity> RelatedFields {
            get {
                if(this.m_relatedFields == null) {
                    this.m_relatedFields = new List<IDbTableFieldEntity>(from t in this.Table.Database.Tables
                                                                         from f in t.TableFields
                                                                         where f.ReferencedField != null && f.ReferencedField.Equals(this)
                                                                         select f);
                }
                return this.m_relatedFields;
            }
        }

        #endregion

        internal void AddConstraints(params IDbConstraintEntity[] constraints) {
            if(constraints == null || constraints.Length == 0) {
                return;
            }

            this.m_constraints.AddRange(constraints);
        }
    }

    /// <summary>
    /// Represents a view entity.
    /// </summary>
    internal class DbViewFieldEntity : DbFieldEntity, IDbViewFieldEntity {
        /// <summary>
        /// Initialize a new instance of DbViewFieldEntity class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="index">Field index.</param>
        /// <param name="type">Field type.</param>
        /// <param name="dbType">Field DbType.</param>
        /// <param name="specificDbType">Specific DbType.</param>
        /// <param name="databaseType">Field database type.</param>
        /// <param name="maxLength">Maximum length of this field.</param>
        /// <param name="isNullable">Whether this field can be null.</param>
        /// <param name="view">View which contains this field.</param>
        /// <param name="table">Table associated by this field.</param>
        /// <param name="tableField">Table field associated by this field.</param>
        public DbViewFieldEntity(string name, int index, Type type, DbType dbType, int specificDbType, string databaseType, int maxLength, bool isNullable, IDbViewEntity view, IDbTableEntity table, IDbTableFieldEntity tableField)
            : base(view, name, index, type, dbType, specificDbType, databaseType, maxLength, isNullable) {
            this.View = view;
            this.Table = table;
            this.TableField = tableField;
        }

        #region IDbViewFieldEntity Members

        /// <inheritdoc />
        public virtual IDbViewEntity View {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual IDbTableEntity Table {
            get;
            protected set;
        }

        /// <inheritdoc />
        public virtual IDbTableFieldEntity TableField {
            get;
            protected set;
        }

        #endregion
    }

    /// <summary>
    /// Represents a constraint entity.
    /// </summary>
    internal class DbConstraintEntity : DbEntity, IDbConstraintEntity {
        /// <summary>
        /// Initialize a new instance of DbConstraintEntity class.
        /// </summary>
        /// <param name="name">Constraint name.</param>
        /// <param name="table">Associated table.</param>
        /// <param name="type">Constraint type.</param>
        /// <param name="fields">Included fields.</param>
        /// <param name="referenceTable">Referenced table in a foreign key constraint.</param>
        public DbConstraintEntity(string name, IDbTableEntity table, DbConstraintType type, IEnumerable<IDbTableFieldEntity> fields, IDbTableEntity referenceTable)
            : base(name) {
            this.Table = table;
            this.Type = type;
            this.ReferenceTable = referenceTable;

            this.m_fields = new List<IDbTableFieldEntity>(fields);
        }

        /// <summary>
        /// Initialize a new instance of DbConstraintEntity class.
        /// </summary>
        /// <param name="name">Constraint name.</param>
        /// <param name="table">Associated table.</param>
        /// <param name="type">Constraint type.</param>
        /// <param name="referenceTable">Referenced table in a foreign key constraint.</param>
        internal DbConstraintEntity(string name, IDbTableEntity table, DbConstraintType type, IDbTableEntity referenceTable)
            : base(name) {
            this.Table = table;
            this.Type = type;
            this.ReferenceTable = referenceTable;

            this.m_fields = new List<IDbTableFieldEntity>();
        }

        /// <summary>
        /// Internal stored fields.
        /// </summary>
        private List<IDbTableFieldEntity> m_fields;

        #region IDbConstraintEntity Members

        /// <inheritdoc />
        public IDbTableEntity Table {
            get;
            protected set;
        }

        /// <inheritdoc />
        public DbConstraintType Type {
            get;
            protected set;
        }

        /// <inheritdoc />
        public bool IsUnique {
            get {
                switch(this.Type) {
                    case DbConstraintType.PrimaryKey:
                    case DbConstraintType.UniqueKey:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDbTableFieldEntity> Fields {
            get {
                return this.m_fields;
            }
        }

        /// <inheritdoc />
        public IDbTableEntity ReferenceTable {
            get;
            protected set;
        }

        /// <inheritdoc />
        public bool HasSameFields(IDbConstraintEntity constraint) {
            if(constraint == null) {
                throw new ArgumentException("Constraint is null.", "constraint");
            }

            bool isSame = true;

            if(!this.m_fields.Any()) {
                isSame = false;
            } else if(this.m_fields.Count() != constraint.Fields.Count()) {
                isSame = false;
            } else {
                IEnumerator<IDbTableFieldEntity> iterator1 = this.m_fields.GetEnumerator();
                IEnumerator<IDbTableFieldEntity> iterator2 = constraint.Fields.GetEnumerator();
                while(iterator1.MoveNext() && iterator2.MoveNext()) {
                    if(!iterator1.Current.Equals(iterator2.Current)) {
                        isSame = false;
                        break;
                    }
                }
            }

            return isSame;
        }

        #endregion

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is DbFieldEntity)) {
                return false;
            }
            DbConstraintEntity entity = (DbConstraintEntity) obj;
            return this.Table.Equals(entity.Table) && string.Equals(this.Name, entity.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format("{0}.{1}", this.Table, this.Name);
        }

        #endregion

        internal void AddFields(params IDbTableFieldEntity[] fields) {
            if(fields == null || fields.Length == 0) {
                return;
            }

            this.m_fields.AddRange(fields);
        }
    }

    public interface IConnectionStringSelector {
        string GetConnectionString(IList<string> writeConnectionStrings, IList<string> readConnectionStrings);
    }

    public class DefaultWriteConnectionStringSelector : IConnectionStringSelector {
        private volatile int m_count = 0;

        #region IConnectionStringSelector Members

        public virtual string GetConnectionString(IList<string> writeConnectionStrings, IList<string> readConnectionStrings) {
            ++m_count;

            if(writeConnectionStrings != null && writeConnectionStrings.Count > 0) {
                if(writeConnectionStrings.Count == 1) {
                    return writeConnectionStrings[0];
                } else {
                    return writeConnectionStrings[m_count % writeConnectionStrings.Count];
                }
            }

            if(readConnectionStrings != null && readConnectionStrings.Count > 0) {
                if(readConnectionStrings.Count == 1) {
                    return readConnectionStrings[0];
                } else {
                    return readConnectionStrings[m_count % readConnectionStrings.Count];
                }
            }

            throw new ArgumentException("writeConnectionStrings and readConnectionStrings is null or empty.", "writeConnectionStrings and readConnectionStrings");
        }

        #endregion
    }

    public class DefaultReadConnectionStringSelector : IConnectionStringSelector {
        private volatile int m_count = 0;

        #region IConnectionStringSelector Members

        public virtual string GetConnectionString(IList<string> writeConnectionStrings, IList<string> readConnectionStrings) {
            ++m_count;

            if(readConnectionStrings != null && readConnectionStrings.Count > 0) {
                if(readConnectionStrings.Count == 1) {
                    return readConnectionStrings[0];
                } else {
                    return readConnectionStrings[m_count % readConnectionStrings.Count];
                }
            }

            if(writeConnectionStrings != null && writeConnectionStrings.Count > 0) {
                if(writeConnectionStrings.Count == 1) {
                    return writeConnectionStrings[0];
                } else {
                    return writeConnectionStrings[m_count % writeConnectionStrings.Count];
                }
            }

            throw new ArgumentException("writeConnectionStrings and readConnectionStrings is null or empty.", "writeConnectionStrings and readConnectionStrings");
        }

        #endregion
    }

    /// <summary>
    /// Provides methods to read entity data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IDbEntityReader<out T> {
        /// <summary>
        /// Gets SQL SELECT statement used to load entities.
        /// </summary>
        /// <param name="count">The number of entities will be load, non-positive value indicate no limited.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>A ISqlSelectStatment.</returns>
        ISqlSelectStatement GetSelectStatement(int count, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Load entities by the specified condition.
        /// </summary>
        /// <param name="count">The number of entities will be load, non-positive value indicate no limited.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>The result collection.</returns>
        IEnumerable<T> Load(int count, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Load entities by the specified condition under the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="count">The number of entities will be load, non-positive value indicate no limited.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>The result collection.</returns>
        IEnumerable<T> Load(IDbConnection connection, int count, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Load entities by the specified condition under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="count">The number of entities will be load, non-positive value indicate no limited.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>The result collection.</returns>
        IEnumerable<T> Load(IDbTransaction transaction, int count, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Gets SQL SELECT statement used to load pagination entities.
        /// </summary>
        /// <param name="totalCount">The number of all data.</param>
        /// <param name="pageSize">The number of data in one page.</param>
        /// <param name="pageNumber">The page index start with one.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>A ISqlSelectStatment.</returns>
        ISqlSelectStatement GetSelectStatement(int totalCount, int pageSize, int pageNumber, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Load pagination entities by the specified condition.
        /// </summary>
        /// <param name="totalCount">The number of all data.</param>
        /// <param name="pageSize">The number of data in one page.</param>
        /// <param name="pageNumber">The page index start with one.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>The result collection.</returns>
        IEnumerable<T> Load(int totalCount, int pageSize, int pageNumber, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Load pagination entities by the specified condition under the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="totalCount">The number of all data.</param>
        /// <param name="pageSize">The number of data in one page.</param>
        /// <param name="pageNumber">The page index start with one.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>The result collection.</returns>
        IEnumerable<T> Load(IDbConnection connection, int totalCount, int pageSize, int pageNumber, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Load pagination entities by the specified condition under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="totalCount">The number of all data.</param>
        /// <param name="pageSize">The number of data in one page.</param>
        /// <param name="pageNumber">The page number start with one.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="orders">Query orders.</param>
        /// <param name="fields">Fields will be load.</param>
        /// <returns>The result collection.</returns>
        IEnumerable<T> Load(IDbTransaction transaction, int totalCount, int pageSize, int pageNumber, ISqlExpression condition, IEnumerable<SqlExpressionOrder> orders, params ISqlObject[] fields);

        /// <summary>
        /// Gets a SQL SELECT statement used to load number of entities which match the specified condition.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <returns>A ISqlSelectStatment.</returns>
        ISqlSelectStatement GetCountStatement(ISqlExpression condition);

        /// <summary>
        /// Get number of entities which match the specified condition.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <returns>The result collection.</returns>
        int LoadCount(ISqlExpression condition);

        /// <summary>
        /// Get number of entities which match the specified condition under the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <returns>The result collection.</returns>
        int LoadCount(IDbConnection connection, ISqlExpression condition);

        /// <summary>
        /// Get number of entities which match the specified condition under the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <returns>The result collection.</returns>
        int LoadCount(IDbTransaction transaction, ISqlExpression condition);

        /// <summary>
        /// Gets a SQL SELECT statement used to decide whether the data match the specified condition is existing.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <returns>A ISqlSelectStatment.</returns>
        ISqlSelectStatement GetExistsStatement(ISqlExpression condition);

        /// <summary>
        /// Gets a value to indicate whether the data match the specified condition is existing.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <returns>Whether existing.</returns>
        bool IsExists(ISqlExpression condition);

        /// <summary>
        /// Gets a value to indicate whether the data match the specified condition is existing.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <returns>Whether existing.</returns>
        bool IsExists(IDbConnection connection, ISqlExpression condition);

        /// <summary>
        /// Gets a value to indicate whether the data match the specified condition is existing.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <returns>Whether existing.</returns>
        bool IsExists(IDbTransaction transaction, ISqlExpression condition);

        /// <summary>
        /// Gets the last generated identity value.
        /// </summary>
        /// <returns>The last generated identity value.</returns>
        int GetLastIdentity();

        /// <summary>
        /// Gets the last generated identity value.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <returns>The last generated identity value.</returns>
        int GetLastIdentity(IDbConnection connection);

        /// <summary>
        /// Gets the last generated identity value.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <returns>The last generated identity value.</returns>
        int GetLastIdentity(IDbTransaction transaction);
    }

    /// <summary>
    /// Provides methods to update entity data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IDbEntityWriter<in T> {
        /// <summary>
        /// Get SQL parameters from the specified object.
        /// </summary>
        /// <param name="info">A <typeparamref name="T"/> object.</param>
        /// <returns>SQL parameters.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        IEnumerable<IDataParameter> GetParameters(T info);

        /// <summary>
        /// Get the SQL INSERT statement used to insert data.
        /// </summary>
        /// <param name="info">A entity info.</param>
        /// <returns>A SQL INSERT statement.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        ISqlInsertStatement GetInsertStatement(T info, bool enableIdentityInsert);

        /// <summary>
        /// Insert one data.
        /// </summary>
        /// <param name="info">A entity info.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        void Insert(T info);

        /// <summary>
        /// Insert one data with the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="info">A entity info.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        void Insert(IDbConnection connection, T info);

        /// <summary>
        /// Insert one data with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="info">A entity info.</param>
        /// <exception cref="System.ArgumentException"><paramref name="info"/> is null.</exception>
        void Insert(IDbTransaction transaction, T info);

        /// <summary>
        /// Insert some data.
        /// </summary>
        /// <param name="data">Some entries data.</param>
        /// <param name="enableIdentityInsert">True: insert identity field; False: not insert identity field.</param>
        void Insert(IEnumerable<T> data, bool enableIdentityInsert);

        /// <summary>
        /// Insert some data with the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="data">Some entries data.</param>
        /// <param name="enableIdentityInsert">True: insert identity field; False: not insert identity field.</param>
        void Insert(IDbConnection connection, IEnumerable<T> data, bool enableIdentityInsert);

        /// <summary>
        /// Insert some data with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="data">Some entries data.</param>
        /// <param name="enableIdentityInsert">True: insert identity field; False: not insert identity field.</param>
        void Insert(IDbTransaction transaction, IEnumerable<T> data, bool enableIdentityInsert);

        /// <summary>
        /// Get the SQL UPDATE statement used to update data.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <param name="values">Fields and it's value.</param>
        /// <returns>A SQL UPDATE statement.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="values"/> is null or empty.</exception>
        ISqlUpdateStatement GetUpdateStatement(ISqlExpression condition, params SqlObjectValue[] values);

        /// <summary>
        /// Update data.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <param name="values">Fields and it's value.</param>
        /// <returns>The number of effected records.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="values"/> is null or empty.</exception>
        int Update(ISqlExpression condition, params SqlObjectValue[] values);

        /// <summary>
        /// Update data with the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="values">Fields and it's value.</param>
        /// <returns>The number of effected records.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="values"/> is null or empty.</exception>
        int Update(IDbConnection connection, ISqlExpression condition, params SqlObjectValue[] values);

        /// <summary>
        /// Update data with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <param name="values">Fields and it's value.</param>
        /// <returns>The number of effected records.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="values"/> is null or empty.</exception>
        int Update(IDbTransaction transaction, ISqlExpression condition, params SqlObjectValue[] values);

        /// <summary>
        /// Get the SQL DELETE statement used to delete data.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <returns>The SQL DELETE statements.</returns>
        IEnumerable<ISqlDeleteStatement> GetDeteleStatement(ISqlExpression condition);

        /// <summary>
        /// Delete data.
        /// </summary>
        /// <param name="condition">Query condition.</param>
        /// <returns>The number of effected records.</returns>
        int Delete(ISqlExpression condition);

        /// <summary>
        /// Delete data with the specified connection.
        /// </summary>
        /// <param name="connection">The connection used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <returns>The number of effected records.</returns>
        int Delete(IDbConnection connection, ISqlExpression condition);

        /// <summary>
        /// Delete data with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute statements.</param>
        /// <param name="condition">Query condition.</param>
        /// <returns>The number of effected records.</returns>
        int Delete(IDbTransaction transaction, ISqlExpression condition);
    }

    /// <summary>
    /// Generate code for accessing database.
    /// </summary>
    public interface IDbCodeGenerator {
        /// <summary>
        /// Generate code for all tables and views in the specified database.
        /// </summary>
        /// <param name="database">A database entity.</param>
        /// <param name="isCascading">Whether generate code for cascading deletion</param>
        /// <param name="entityNamespaceName">Namespace of entity code.</param>
        /// <param name="accessorNamespaceName">Namespace of accessor code.</param>
        /// <param name="folder">The folder to save code files.</param>
        /// <exception cref="System.ArgumentException"><paramref name="database"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="namespaceName"/> is null or empty.</exception>
        void GenerateCode(IDbDatabaseEntity database, bool isCascading, string entityNamespaceName, string accessorNamespaceName, string folder);
    }

    /// <summary>
    /// Provides info of a specific database for IDbCodeGenerator.
    /// </summary>
    public interface IDbCodeDbInfoProvider {
        string Name {
            get;
        }

        string Description {
            get;
        }

        bool IsSupportGetLastIdentity {
            get;
        }

        void PrepareCodeNamespace(CodeNamespace ns);

        CodeExpression GetSqlObject(CodeExpression sqlExpression, CodeExpression name);

        CodeExpression GetSqlSource(CodeExpression schema, CodeExpression name);

        CodeExpression GetSubquerySource(CodeExpression selectStatement, CodeExpression name);

        CodeExpression GetSqlField(CodeExpression sqlSource, CodeExpression name);

        CodeExpression GetIdentityStatement();

        CodeExpression GetIdentityStatement(CodeExpression name);

        CodeExpression GetLikeString(CodeExpression pattern);

        CodeExpression GetSqlStatementCommandTextProvider();

        CodeExpression GetIdentityFieldValue(CodeExpression objReference, IDbFieldEntity field);

        CodeExpression GetEnableIdentityInsertStatement(IDbDataEntity data);

        Type GetSqlSourceType();

        Type GetSqlSelectStatementType();

        Type GetSqlInsertStatementType();

        Type GetSqlUpdateStatementType();

        Type GetSqlDeleteStatementType();

        Type GetDbConnectionType();

        Type GetDbParameterType();

        Type GetDbTypeType();

        CodeExpression GetDbTypeValue(IDbFieldEntity field);

        string GetDbParameterName(IDbDataEntity data);

        string GetDbParameterName(IDbFieldEntity field);

        CodeExpression GetDbParameter(CodeExpression name, IDbFieldEntity field, CodeExpression value);

        CodeExpression GetDbParameter(CodeExpression name, CodeExpression dbType, CodeExpression value);

        CodeExpression GetDbParameterExpression(CodeExpression name, IDbFieldEntity field, CodeExpression value);

        CodeExpression GetReadValue(CodeExpression reader, IDbFieldEntity field, CodeExpression index);

        Type GetSqlObjectValueType();

        CodeExpression GetSqlObjectValueObject(CodeExpression value);

        CodeExpression GetSqlObjectValueObjectName(CodeExpression value);

        CodeExpression GetDbParameterExpressionFromSqlObjectValue(CodeExpression name, CodeExpression type, CodeExpression value);

        CodeExpression GetSqlObjectValue(CodeExpression obj, IDbFieldEntity field, CodeExpression value);
    }

    /// <summary>
    /// Provides the members info for IDbCodeGenerator.
    /// </summary>
    public interface IDbCodeMembersProvider {
        string Name {
            get;
        }

        string Description {
            get;
        }

        string GetMembersTypeName(IDbDataEntity data);
    }

    /// <summary>
    /// Represents the default database code provider.
    /// </summary>
    public class DbCodeGenerator : IDbCodeGenerator {
        /// <summary>
        /// Initialize a new instance of DbCodeGenerator class.
        /// </summary>
        /// <param name="infoProvider"></param>
        /// <param name="codeProvider"></param>
        public DbCodeGenerator(IDbCodeDbInfoProvider infoProvider, IDbCodeMembersProvider membersProvider, CodeDomProvider codeProvider) {
            if(infoProvider == null) {
                throw new ArgumentException("SQL object provider used in this code provider is null.", "sqlObjectProvider");
            }
            if(codeProvider == null) {
                throw new ArgumentException("Code DOM provider is null.", "codeProvider");
            }

            this.m_dbInfoProvider = infoProvider;
            this.m_membersProvider = membersProvider;
            this.m_codeProvider = codeProvider;
        }

        #region Constants

        private const int MAX_MEMBERS_COUNT = 64;
        private const string MEMBERS_NAME_NONE = "None";
        private const string MEMBERS_NAME_All = "AllMembers";

        private static readonly Type NullableTypeDefinition = typeof(Nullable<>);

        #region Type Member Names

        public static readonly string Object_GetHashCode = ((MethodCallExpression) (((Expression<Func<object, int>>) ((obj) => obj.GetHashCode())).Body)).Method.Name;
        public static readonly string Object_Equals = ((MethodCallExpression) (((Expression<Func<object, bool>>) ((obj) => obj.Equals(null))).Body)).Method.Name;
        public static readonly string Object_GetType = ((MethodCallExpression) (((Expression<Func<object, Type>>) ((obj) => obj.GetType())).Body)).Method.Name;
        public static readonly string Object_ReferenceEquals = ((MethodCallExpression) (((Expression<Func<bool>>) (() => object.ReferenceEquals(null, null))).Body)).Method.Name;

        public static readonly string IDisposable_Dispose = ((MethodCallExpression) (((Expression<Action<IDisposable>>) ((obj) => obj.Dispose())).Body)).Method.Name;

        public static readonly string Enum_HasFlag = ((MethodCallExpression) (((Expression<Func<MemberAttributes, bool>>) ((obj) => obj.HasFlag(MemberAttributes.Public))).Body)).Method.Name;

        public static readonly string ICloneable_Clone = ((MethodCallExpression) (((Expression<Func<ICloneable, object>>) ((obj) => obj.Clone())).Body)).Method.Name;

        public static readonly string IDataReader_Read = ((MethodCallExpression) (((Expression<Func<IDataReader, bool>>) ((obj) => obj.Read())).Body)).Method.Name;
        public static readonly string IDataReader_IsDBNull = ((MethodCallExpression) (((Expression<Func<IDataReader, bool>>) ((obj) => obj.IsDBNull(0))).Body)).Method.Name;

        public static readonly string IDbConnection_ConnectionString = ((MemberExpression) (((Expression<Func<IDbConnection, string>>) ((obj) => obj.ConnectionString)).Body)).Member.Name;

        public static readonly string ICollection_Add = ((MethodCallExpression) (((Expression<Action<ICollection<object>>>) ((obj) => obj.Add(null))).Body)).Method.Name;
        public static readonly string ICollection_Count = ((MemberExpression) (((Expression<Func<ICollection<object>, int>>) ((obj) => obj.Count)).Body)).Member.Name;

        public static readonly string String_Length = ((MemberExpression) (((Expression<Func<string, int>>) ((obj) => obj.Length)).Body)).Member.Name;
        public static readonly string String_Substring = ((MethodCallExpression) (((Expression<Func<string, string>>) ((obj) => obj.Substring(0))).Body)).Method.Name;
        public static readonly string String_IsNullOrEmpty = ((MethodCallExpression) (((Expression<Func<bool>>) (() => string.IsNullOrEmpty(null))).Body)).Method.Name;
        public static readonly string String_IsNullOrWhiteSpace = ((MethodCallExpression) (((Expression<Func<bool>>) (() => string.IsNullOrWhiteSpace(null))).Body)).Method.Name;

        public static readonly string Array_Length = ((MemberExpression) (((Expression<Func<Array, int>>) ((obj) => obj.Length)).Body)).Member.Name;
        public static readonly string Array_Resize = ((MethodCallExpression) (((Expression<Action<int[]>>) ((obj) => Array.Resize<int>(ref obj, 0))).Body)).Method.Name;

        public static readonly string NotifyPropertyChanged_OnPropertyChanged = ((MethodCallExpression) (((Expression<Action<NotifyPropertyChanged>>) ((obj) => obj.OnPropertyChanged(null))).Body)).Method.Name;

        public static readonly string Type_IsAssignableFrom = ((MethodCallExpression) (((Expression<Func<Type, bool>>) ((obj) => obj.IsAssignableFrom(null))).Body)).Method.Name;

        public static readonly string Math_Min = ((MethodCallExpression) (((Expression<Func<float>>) (() => Math.Min(0F, 0F))).Body)).Method.Name;

        public static readonly string Convert_ToInt32 = ((MethodCallExpression) (((Expression<Func<int>>) (() => Convert.ToInt32((object) null))).Body)).Method.Name;
        public static readonly string Convert_IsDBNull = ((MethodCallExpression) (((Expression<Func<bool>>) (() => Convert.IsDBNull((object) null))).Body)).Method.Name;

        public static readonly string Nullable_HasValue = ((MemberExpression) (((Expression<Func<int?, bool>>) ((obj) => obj.HasValue)).Body)).Member.Name;
        public static readonly string Nullable_Value = ((MemberExpression) (((Expression<Func<int?, int>>) ((obj) => obj.Value)).Body)).Member.Name;

        public static readonly string ISqlSelectStatement_Count = ((MemberExpression) (((Expression<Func<ISqlSelectStatement, int>>) ((obj) => obj.Count)).Body)).Member.Name;
        public static readonly string ISqlSelectStatement_SelectClause = ((MemberExpression) (((Expression<Func<ISqlSelectStatement, ISqlSelectClause>>) ((obj) => obj.SelectClause)).Body)).Member.Name;
        public static readonly string ISqlSelectClause_AddExpressions = ((MethodCallExpression) (((Expression<Func<ISqlSelectClause, ISqlSelectClause>>) ((obj) => obj.AddExpressions())).Body)).Method.Name;
        public static readonly string ISqlSelectStatement_FromClause = ((MemberExpression) (((Expression<Func<ISqlSelectStatement, ISqlFromClause>>) ((obj) => obj.FromClause)).Body)).Member.Name;
        public static readonly string ISqlFromClause_Source = ((MemberExpression) (((Expression<Func<ISqlFromClause, ISqlExpression>>) ((obj) => obj.Source)).Body)).Member.Name;
        public static readonly string ISqlSelectStatement_WhereClause = ((MemberExpression) (((Expression<Func<ISqlSelectStatement, ISqlWhereClause>>) ((obj) => obj.WhereClause)).Body)).Member.Name;
        public static readonly string ISqlWhereClause_Condition = ((MemberExpression) (((Expression<Func<ISqlWhereClause, ISqlExpression>>) ((obj) => obj.Condition)).Body)).Member.Name;
        public static readonly string ISqlSelectStatement_OrderClause = ((MemberExpression) (((Expression<Func<ISqlSelectStatement, ISqlOrderClause>>) ((obj) => obj.OrderClause)).Body)).Member.Name;
        public static readonly string ISqlOrderClause_AddExpression = ((MethodCallExpression) (((Expression<Func<ISqlOrderClause, ISqlOrderClause>>) ((obj) => obj.AddExpression(null))).Body)).Method.Name;

        public static readonly string ISqlInsertStatement_FieldValueClause = ((MemberExpression) (((Expression<Func<ISqlInsertStatement, ISqlFieldValueClause>>) ((obj) => obj.FieldValueClause)).Body)).Member.Name;

        public static readonly string ISqlUpdateStatement_FieldValueClause = ((MemberExpression) (((Expression<Func<ISqlUpdateStatement, ISqlFieldValueClause>>) ((obj) => obj.FieldValueClause)).Body)).Member.Name;
        public static readonly string ISqlUpdateStatement_WhereClause = ((MemberExpression) (((Expression<Func<ISqlUpdateStatement, ISqlWhereClause>>) ((obj) => obj.WhereClause)).Body)).Member.Name;

        public static readonly string ISqlDeleteStatement_FromClause = ((MemberExpression) (((Expression<Func<ISqlDeleteStatement, ISqlFromClause>>) ((obj) => obj.FromClause)).Body)).Member.Name;
        public static readonly string ISqlDeleteStatement_WhereClause = ((MemberExpression) (((Expression<Func<ISqlDeleteStatement, ISqlWhereClause>>) ((obj) => obj.WhereClause)).Body)).Member.Name;

        public static readonly string ISqlFieldValueClause_Source = ((MemberExpression) (((Expression<Func<ISqlFieldValueClause, ISqlExpression>>) ((obj) => obj.Source)).Body)).Member.Name;
        public static readonly string ISqlFieldValueClause_AddField = ((MethodCallExpression) (((Expression<Func<ISqlFieldValueClause, ISqlFieldValueClause>>) ((obj) => obj.AddField(null, null))).Body)).Method.Name;

        public static readonly string ISqlFunction_AddArgument = ((MethodCallExpression) (((Expression<Func<ISqlFunction, ISqlFunction>>) ((obj) => obj.AddArgument())).Body)).Method.Name;

        public static readonly string IEnumerable_GetEnumerator = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, IEnumerator<object>>>) ((obj) => obj.GetEnumerator())).Body)).Method.Name;
        public static readonly string IEnumerable_Count = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, int>>) ((obj) => obj.Count())).Body)).Method.Name;
        public static readonly string IEnumerable_ToArray = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, object[]>>) ((obj) => obj.ToArray())).Body)).Method.Name;
        public static readonly string IEnumerable_FirstOrDefault = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, object>>) ((obj) => obj.FirstOrDefault())).Body)).Method.Name;
        public static readonly string IEnumerable_Concat = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, IEnumerable<object>>>) ((obj) => obj.Concat(null))).Body)).Method.Name;
        public static readonly string IEnumerable_Split = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, IEnumerable<IEnumerable<object>>>>) ((obj) => obj.Split(0))).Body)).Method.Name;
        public static readonly string IEnumerable_Contains = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>, bool>>) ((obj) => obj.Contains(null))).Body)).Method.Name;

        public static readonly string IEnumerator_MoveNext = ((MethodCallExpression) (((Expression<Func<IEnumerator<object>, bool>>) ((obj) => obj.MoveNext())).Body)).Method.Name;
        public static readonly string IEnumerator_Current = ((MemberExpression) (((Expression<Func<IEnumerator<object>, object>>) ((obj) => obj.Current)).Body)).Member.Name;

        public static readonly string Enumerable_Empty = ((MethodCallExpression) (((Expression<Func<IEnumerable<object>>>) (() => Enumerable.Empty<object>())).Body)).Method.Name;

        public static readonly string SqlExpressionOrder_Order = ((MemberExpression) (((Expression<Func<SqlExpressionOrder, SqlOrder>>) ((obj) => obj.Order)).Body)).Member.Name;
        public static readonly string SqlExpressionOrder_Expression = ((MemberExpression) (((Expression<Func<SqlExpressionOrder, ISqlExpression>>) ((obj) => obj.Expression)).Body)).Member.Name;

        public static readonly string SqlObjectValue_Object = ((MemberExpression) (((Expression<Func<SqlObjectValue, ISqlObject>>) ((obj) => obj.Object)).Body)).Member.Name;
        public static readonly string SqlObjectValue_Type = ((MemberExpression) (((Expression<Func<SqlObjectValue, DbType>>) ((obj) => obj.Type)).Body)).Member.Name;
        public static readonly string SqlObjectValue_Value = ((MemberExpression) (((Expression<Func<SqlObjectValue, object>>) ((obj) => obj.Value)).Body)).Member.Name;

        public static readonly string ISqlObject_Name = ((MemberExpression) (((Expression<Func<ISqlObject, string>>) ((obj) => obj.Name)).Body)).Member.Name;

        public static readonly string ISqlExpression_And = ((MethodCallExpression) (((Expression<Func<ISqlExpression, ISqlExpression>>) ((obj) => obj.And(null))).Body)).Method.Name;
        public static readonly string ISqlExpression_Not = ((MethodCallExpression) (((Expression<Func<ISqlExpression, ISqlExpression>>) ((obj) => obj.Not())).Body)).Method.Name;
        public static readonly string ISqlExpression_Like = ((MethodCallExpression) (((Expression<Func<ISqlExpression, ISqlExpression>>) ((obj) => obj.Like(null))).Body)).Method.Name;
        public static readonly string ISqlExpression_Equal = ((MethodCallExpression) (((Expression<Func<ISqlExpression, ISqlExpression>>) ((obj) => obj.Equal(null))).Body)).Method.Name;
        public static readonly string ISqlExpression_IsNull = ((MethodCallExpression) (((Expression<Action<ISqlExpression>>) ((obj) => obj.IsNull())).Body)).Method.Name;
        public static readonly string ISqlExpression_In = ((MethodCallExpression) (((Expression<Func<ISqlExpression, ISqlExpression>>) ((obj) => obj.In(null))).Body)).Method.Name;

        public static readonly string SqlExpression_True = ((MemberExpression) (((Expression<Func<ISqlExpression>>) (() => SqlExpression.True)).Body)).Member.Name;
        public static readonly string SqlExpression_False = ((MemberExpression) (((Expression<Func<ISqlExpression>>) (() => SqlExpression.False)).Body)).Member.Name;
        public static readonly string SqlStringExpression_FromParameter = ((MethodCallExpression) (((Expression<Func<IDataParameter, ISqlExpression>>) ((obj) => SqlStringExpression.FromParameter(obj))).Body)).Method.Name;

        public static readonly string SqlHelper_MaxNoQueryStatementsLimit = ((MemberExpression) (((Expression<Func<int>>) (() => SqlHelper.MaxNoQueryStatementsLimit))).Body).Member.Name;
        public static readonly string SqlHelper_CreateParameter = ((MethodCallExpression) (((Expression<Func<IDataParameter>>) (() => SqlHelper.CreateParameter<SqlParameter>(null, DbType.AnsiString, null)))).Body).Method.Name;
        public static readonly string SqlHelper_CreateParameterExpression = ((MethodCallExpression) (((Expression<Func<ISqlExpression>>) (() => SqlHelper.CreateParameterExpression<SqlParameter>(null, DbType.AnsiString, null)))).Body).Method.Name;
        public static readonly string SqlHelper_ExecuteSelect = ((MethodCallExpression) (((Expression<Func<IDataReader>>) (() => SqlHelper.ExecuteSelect((IDbConnection) null))).Body)).Method.Name;
        public static readonly string SqlHelper_ExecuteScalar = ((MethodCallExpression) (((Expression<Func<object>>) (() => SqlHelper.ExecuteScalar<object>((IDbConnection) null))).Body)).Method.Name;
        public static readonly string SqlHelper_ExecuteNoQuery = ((MethodCallExpression) (((Expression<Func<int>>) (() => SqlHelper.ExecuteNoQuery((IDbConnection) null))).Body)).Method.Name;
        public static readonly string SqlHelper_ExecuteInsert = ((MethodCallExpression) (((Expression<Func<int>>) (() => SqlHelper.ExecuteInsert((IDbConnection) null))).Body)).Method.Name;
        public static readonly string SqlHelper_ExecuteUpdate = ((MethodCallExpression) (((Expression<Func<int>>) (() => SqlHelper.ExecuteUpdate((IDbConnection) null))).Body)).Method.Name;
        public static readonly string SqlHelper_ExecuteDelete = ((MethodCallExpression) (((Expression<Func<int>>) (() => SqlHelper.ExecuteDelete((IDbConnection) null))).Body)).Method.Name;

        public static readonly string IConnectionStringSelector_GetConnectionString = ((MethodCallExpression) (((Expression<Func<IConnectionStringSelector, string>>) ((obj) => obj.GetConnectionString(null, null))).Body)).Method.Name;

        public static readonly string Interlocked_CompareExchange = "CompareExchange";
        public static readonly string Interlocked_Increment = "Increment";

        #endregion

        #endregion

        private string m_parameterIndexFieldName = "g_parameterIndex";

        private string m_useStableParameterNameArgumentName = "useStableParameterName";

        private string m_writeConnectionStringsFieldName = "m_writeConnectionStrings";

        private string m_readConnectionStringsFieldName = "m_readConnectionStrings";

        private string m_writeConnectionStringSelectorFieldName = "m_writeConnectionStringSelector";

        private string m_readConnectionStringSelectorFieldName = "m_readConnectionStringSelector";

        private string m_writeConnectionStringsPropertyName = "WriteConnectionStrings";

        private string m_readConnectionStringsPropertyName = "ReadConnectionStrings";

        private string m_writeConnectionStringSelectorPropertyName = "WriteConnectionStringSelector";

        private string m_readConnectionStringSelectorPropertyName = "ReadConnectionStringSelector";

        private string m_connectionStringPropertyName = "ConnectionString";

        private string m_initializeEntityMethodName = "Initialize";

        private string m_validateMethodName = "Validate";

        private string m_copyMethodName = "Copy";

        private string m_getWriteConnectionStringMethodName = "GetWriteConnectionString";

        private string m_getReadConnectionStringMethodName = "GetReadConnectionString";

        private string m_loadFromReaderMethodName = "LoadFromReader";

        private string m_getFiledsMethodName = "GetFields";

        private string m_getSelectStatementMethodName = "GetSelectStatement";

        private string m_getCountStatementMethodName = "GetCountStatement";

        private string m_getExistsStatementMethodName = "GetExistsStatement";

        private string m_loadMethodName = "Load";

        private string m_loadCountMethodName = "LoadCount";

        private string m_isExistsMethodName = "IsExists";

        private string m_getParametersMethodName = "GetParameters";

        private string m_getLastIdentityMethodName = "GetLastIdentity";

        private string m_getInsertStatementMethodName = "GetInsertStatement";

        private string m_insertMethodName = "Insert";

        private string m_getUpdateStatementMethodName = "GetUpdateStatement";

        private string m_updateMethodName = "Update";

        private string m_getDeteleStatementMethodName = "GetDeteleStatement";

        private string m_deleteMethodName = "Delete";

        private string m_getSoureMethodName = "GetSource";

        private string m_getQueryConditionMethodName = "GetQueryCondition";

        private ICollection<IDbConstraintEntity> m_processedConstraints = new List<IDbConstraintEntity>();

        private IDbCodeDbInfoProvider m_dbInfoProvider;

        private IDbCodeMembersProvider m_membersProvider;

        private CodeDomProvider m_codeProvider;

        #region Name Methods

        public string GetEntityCodeFilename(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}.{1}.Entity", data.Schema, data.Name) : string.Format("{0}.Entity", data.Name);
        }

        public string GetMembersCodeFilename(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}.{1}.Members", data.Schema, data.Name) : string.Format("{0}.Entity", data.Name);
        }

        public string GetQueryConditionCodeFilename(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}.{1}.QueryCondition", data.Schema, data.Name) : string.Format("{0}.QueryCondition", data.Name);
        }

        public string GetAccessorCodeFilename(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}.{1}.Accessor", data.Schema, data.Name) : string.Format("{0}.Accessor", data.Name);
        }

        private string GetEntityTypeName(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}_{1}_Entity", data.Schema, data.Name) : string.Format("{0}_Entity", data.Name);
        }

        private string GetMembersTypeName(IDbDataEntity data) {
            return this.m_membersProvider != null ? this.m_membersProvider.GetMembersTypeName(data) : !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}_{1}_Members", data.Schema, data.Name) : string.Format("{0}_Members", data.Name);
        }

        private string GetQueryConditionTypeName(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}_{1}_QueryCondition", data.Schema, data.Name) : string.Format("{0}_QueryCondition", data.Name);
        }

        private string GetAccssorTypeName(IDbDataEntity data) {
            return !string.IsNullOrWhiteSpace(data.Schema) ? string.Format("{0}_{1}_Accessor", data.Schema, data.Name) : string.Format("{0}_Accessor", data.Name);
        }

        private string GetTypeFullname(string namespaceName, string typeName) {
            return string.Format("{0}.{1}", namespaceName, typeName);
        }

        private string GetVariableName(string name) {
            return string.Format("{0}{1}", name.Substring(0, 1).ToLower(), name.Substring(1));
        }

        private string GetDataParameterName(IDbFieldEntity field) {
            return this.GetVariableName(field.Name) + "ParameterName";
        }

        private string GetDataSourceFieldName(IDbDataEntity data) {
            return string.Format("g_{0}", this.GetVariableName(data.Name));
        }

        private string GetDataSourcePropertyName(IDbDataEntity data) {
            return data.Name;
        }

        private string GetAllFieldsFieldName(IDbDataEntity data) {
            return string.Format("g_{0}Fields", this.GetVariableName(data.Name));
        }

        private string GetAllFieldsPropertyName(IDbDataEntity data) {
            return string.Format("{0}Fields", data.Name);
        }

        private string GetPrivateStaticFieldName(IDbFieldEntity field) {
            return string.Format("g_{0}", this.GetVariableName(field.Name));
        }

        private string GetPrivateInstanceFieldName(IDbFieldEntity field) {
            return string.Format("m_{0}", this.GetVariableName(field.Name));
        }

        private string GetDbTypePropertyName(IDbFieldEntity field) {
            return string.Format("{0}DbType", field.Name);
        }

        private string GetSpecificDbTypePropertyName(IDbFieldEntity field, Type type) {
            return string.Format("{0}{1}", field.Name, type.Name);
        }

        private string GetMaxLengthPropertyName(IDbFieldEntity field) {
            return string.Format("{0}MaxLength", field.Name);
        }

        private string GetCreateParameterMethodName(IDbFieldEntity field) {
            return string.Format("Create{0}Parameter", field.Name);
        }

        private string GetCreateParameterExpressionMethodName(IDbFieldEntity field) {
            return string.Format("Create{0}ParameterExpression", field.Name);
        }

        private string GetMethodNameByConstraint(string name, IDbConstraintEntity entity) {
            StringBuilder method = new StringBuilder(string.Format("{0}By", name));
            foreach(IDbTableFieldEntity f in entity.Fields) {
                method.Append(f.Name);
            }
            return method.ToString();
        }

        private string GetArgumentNameByConstraint(DbConstraintType type, IDbTableFieldEntity field) {
            if(type == DbConstraintType.None) {
                throw new ArgumentException("The constraint type is undefined.", "type");
            }

            string name = null;
            switch(type) {
                case DbConstraintType.PrimaryKey:
                    name = "pk" + field.Name;
                    break;
                case DbConstraintType.UniqueKey:
                    name = "uk" + field.Name;
                    break;
                case DbConstraintType.ForeignKey:
                    name = "fk" + field.Name;
                    break;
            }

            return name;
        }

        public CodeExpression GetOrderFieldAlias(CodeExpression indexExpression) {
            return new CodeBinaryOperatorExpression(new CodePrimitiveExpression("__xphter_dbcode__order__"), CodeBinaryOperatorType.Add, indexExpression);
        }

        #endregion

        #region Create Member Methods

        private CodeTypeDeclaration CreateType(CodeNamespace ns, string typeName, string typeComment, params Type[] baseTypes) {
            CodeTypeDeclaration type = new CodeTypeDeclaration(typeName);
            ns.Types.Add(type);

            type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
            type.Attributes = MemberAttributes.Public;
            type.IsClass = true;
            if(baseTypes != null) {
                foreach(Type t in baseTypes) {
                    type.BaseTypes.Add(new CodeTypeReference(t));
                }
            }

            type.Comments.Add(new CodeCommentStatement("<summary>", true));
            type.Comments.Add(new CodeCommentStatement(typeComment, true));
            type.Comments.Add(new CodeCommentStatement("</summary>", true));

            return type;
        }

        private CodeTypeDeclaration CreateEntityType(CodeNamespace ns, IDbDataEntity data) {
            return this.CreateType(ns, this.GetEntityTypeName(data), string.Format("Data entity of: {0}.", data.Name), typeof(NotifyPropertyChanged), typeof(ICloneable));
        }

        private CodeTypeDeclaration CreateMembersType(CodeNamespace ns, IDbDataEntity data) {
            CodeTypeDeclaration type = this.CreateType(ns, this.GetMembersTypeName(data), string.Format("Data members of: {0}.", data.Name), typeof(long));

            type.IsClass = false;
            type.IsEnum = true;
            type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(FlagsAttribute))));

            return type;
        }

        private CodeTypeDeclaration CreateQueryConditionType(CodeNamespace ns, IDbDataEntity data) {
            return this.CreateType(ns, this.GetQueryConditionTypeName(data), string.Format("Represents the common query conditon of {0}.", data.Name), typeof(NotifyPropertyChanged));
        }

        private CodeTypeDeclaration CreateAccessorType(CodeNamespace ns, string entityNamespaceName, IDbDataEntity data) {
            CodeTypeDeclaration type = this.CreateType(ns, this.GetAccssorTypeName(data), string.Format("Data accessor of: {0}.", data.Name));

            CodeTypeParameter parameter = new CodeTypeParameter("T");
            parameter.Constraints.Add(new CodeTypeReference(this.GetTypeFullname(entityNamespaceName, this.GetEntityTypeName(data))));
            parameter.HasConstructorConstraint = true;
            type.TypeParameters.Add(parameter);

            return type;
        }

        private CodeConstructor CreateDefaultConstructorOfEntityType(IDbDataEntity data) {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("Initialize a new instance of class {0}.", this.GetEntityTypeName(data)), true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));

            //invoke Initialize method
            constructor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_initializeEntityMethodName));

            return constructor;
        }

        private CodeConstructor CreateCopyConstructorOfEntityType(IDbDataEntity data) {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetEntityTypeName(data)), "info");
            constructor.Parameters.Add(infoArgument);

            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("Initialize a new instance of class {0} with the specified object.", this.GetEntityTypeName(data)), true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));

            // invoke Initialize and Copy method
            constructor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_initializeEntityMethodName));
            constructor.Statements.Add(new CodeSnippetStatement());
            constructor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_copyMethodName, new CodeArgumentReferenceExpression(infoArgument.Name)));

            return constructor;
        }

        private ICollection<CodeTypeMember> CreateInstanceConstructorsOfEntityType(IDbDataEntity data) {
            ICollection<CodeTypeMember> members = new List<CodeTypeMember>();

            CodeTypeMember constructor1 = members.AddItem(this.CreateDefaultConstructorOfEntityType(data));
            CodeTypeMember constructor2 = members.AddItem(this.CreateCopyConstructorOfEntityType(data));

            constructor1.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Constructor"));
            constructor2.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));

            return members;
        }

        private CodeMemberMethod CreateInitializeEntityMethod(IDbDataEntity data) {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Family;
            method.ReturnType = null;
            method.Name = this.m_initializeEntityMethodName;

            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Initialize entity members.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            return method;
        }

        private CodeMemberMethod CreateValidateMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.m_validateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetEntityTypeName(table)), "info");
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression emptyCallbackArgument = new CodeParameterDeclarationExpression(typeof(Action<string>), "emptyCallback");
            method.Parameters.Add(emptyCallbackArgument);
            CodeParameterDeclarationExpression overflowCallbackArgument = new CodeParameterDeclarationExpression(typeof(Action<string, int>), "overflowCallback");
            method.Parameters.Add(overflowCallbackArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Validate whether the specified object is valid.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The delegate will be invoke when a not nullable property is null or empty.</param>", emptyCallbackArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The delegate will be invoke when value of a property is too long.</param>", emptyCallbackArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>Return true if <paramref name=\"{0}\"/> is valid, otherwise return false.</returns>", infoArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"{0}\"><paramref name=\"{1}\"/> is null.</exception>", typeof(ArgumentException).Name, infoArgument.Name), true));

            //create method statements
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            CodeArgumentReferenceExpression emptyCallbackReference = new CodeArgumentReferenceExpression(emptyCallbackArgument.Name);
            CodeArgumentReferenceExpression overflowCallbackReference = new CodeArgumentReferenceExpression(overflowCallbackArgument.Name);

            //if info is null, then throw exception.
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //check each field
            foreach(IDbTableFieldEntity field in table.TableFields) {
                if(field.IsIdentity || field.IsReadOnly) {
                    continue;
                }

                //check nullable
                if(!field.IsNullable && !field.HasDefaultValue && !field.Type.IsValueType) {
                    if(field.Type.Equals(typeof(string))) {
                        method.Statements.Add(new CodeConditionStatement(
                            new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, new CodePropertyReferenceExpression(infoReference, field.Name)),
                            new CodeConditionStatement(
                                  new CodeBinaryOperatorExpression(emptyCallbackReference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                                  new CodeExpressionStatement(new CodeDelegateInvokeExpression(emptyCallbackReference, new CodePrimitiveExpression(field.Description ?? field.Name)))),
                            new CodeMethodReturnStatement(new CodePrimitiveExpression(false))));
                    } else if(field.Type.IsArray) {
                        method.Statements.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeBinaryOperatorExpression(
                                    new CodePropertyReferenceExpression(infoReference, field.Name),
                                    CodeBinaryOperatorType.IdentityEquality,
                                    new CodePrimitiveExpression(null)),
                                CodeBinaryOperatorType.BooleanOr,
                                new CodeBinaryOperatorExpression(
                                    new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(infoReference, field.Name), String_Length),
                                    CodeBinaryOperatorType.IdentityEquality,
                                    new CodePrimitiveExpression(0))),
                            new CodeConditionStatement(
                                  new CodeBinaryOperatorExpression(emptyCallbackReference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                                  new CodeExpressionStatement(new CodeDelegateInvokeExpression(emptyCallbackReference, new CodePrimitiveExpression(field.Description ?? field.Name)))),
                            new CodeMethodReturnStatement(new CodePrimitiveExpression(false))));
                    } else {
                        method.Statements.Add(new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodePropertyReferenceExpression(infoReference, field.Name),
                                CodeBinaryOperatorType.IdentityEquality,
                                new CodePrimitiveExpression(null)),
                            new CodeConditionStatement(
                                new CodeBinaryOperatorExpression(emptyCallbackReference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                                new CodeExpressionStatement(new CodeDelegateInvokeExpression(emptyCallbackReference, new CodePrimitiveExpression(field.Description ?? field.Name)))),
                            new CodeMethodReturnStatement(new CodePrimitiveExpression(false))));
                    }
                }

                //check maximum length
                if(field.MaxLength > 0 && (field.Type.Equals(typeof(string)) || field.Type.IsArray)) {
                    method.Statements.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(infoReference, field.Name), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                            CodeBinaryOperatorType.BooleanAnd,
                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(infoReference, field.Name), String_Length), CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(field.MaxLength))),
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(overflowCallbackReference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                            new CodeExpressionStatement(new CodeDelegateInvokeExpression(overflowCallbackReference, new CodePrimitiveExpression(field.Description ?? field.Name), new CodePrimitiveExpression(field.MaxLength)))),
                        new CodeMethodReturnStatement(new CodePrimitiveExpression(false))));
                }
            }
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));

            return method;
        }

        private CodeMemberMethod CreateGetHashCodeMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = (method.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Public | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = Object_GetHashCode;

            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));
            method.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Object Members"));

            //create method statements
            if(table.PrimaryKey != null) {
                CodeExpression result = new CodePrimitiveExpression(0);
                foreach(IDbTableFieldEntity field in table.PrimaryKey.Fields) {
                    result = new CodeBinaryOperatorExpression(
                        result,
                        CodeBinaryOperatorType.Add,
                        new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                            method.Name));
                }
                method.Statements.Add(new CodeMethodReturnStatement(result));
            } else {
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), method.Name)));
            }

            return method;
        }

        private CodeMemberMethod CreateEqualsMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = (method.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Public | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = Object_Equals;

            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));
            method.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));

            //create method arguments
            CodeParameterDeclarationExpression objArgument = new CodeParameterDeclarationExpression(typeof(object), "obj");
            method.Parameters.Add(objArgument);

            //create method statements
            string typeName = this.GetEntityTypeName(table);
            CodeArgumentReferenceExpression objReference = new CodeArgumentReferenceExpression(objArgument.Name);
            //if obj == null, then return false
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(objReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeMethodReturnStatement(new CodePrimitiveExpression(false))));

            //if object.ReferenceEquals(this, obj), then return true
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(object)),
                    Object_ReferenceEquals,
                    objReference,
                    new CodeThisReferenceExpression()),
                new CodeMethodReturnStatement(new CodePrimitiveExpression(true))));

            //if obj is not entity type, the return false
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(
                    new CodeTypeOfExpression(typeName),
                    Type_IsAssignableFrom,
                    new CodeMethodInvokeExpression(
                        objReference,
                        Object_GetType)),
                new CodeStatement[] { },
                new CodeStatement[] { new CodeMethodReturnStatement(new CodePrimitiveExpression(false)) }));

            //compare primary key
            CodeVariableDeclarationStatement entityVariable = null;
            method.Statements.Add(entityVariable = new CodeVariableDeclarationStatement(typeName, "entity", new CodeCastExpression(typeName, objReference)));
            CodeExpression condition = new CodePrimitiveExpression(true);
            if(table.PrimaryKey != null) {
                CodeVariableReferenceExpression entityReference = new CodeVariableReferenceExpression(entityVariable.Name);
                foreach(IDbTableFieldEntity field in table.PrimaryKey.Fields) {
                    condition = new CodeBinaryOperatorExpression(
                        condition,
                        CodeBinaryOperatorType.BooleanAnd,
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(typeof(object)),
                            Object_Equals,
                            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                            new CodePropertyReferenceExpression(entityReference, field.Name)));
                }
            }
            method.Statements.Add(new CodeMethodReturnStatement(condition));

            return method;
        }

        private CodeMemberMethod CreateCopyMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_copyMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetEntityTypeName(data)), "info");
            method.Parameters.Add(infoArgument);

            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Copy data from a instance of class {0} with the specified object.", this.GetEntityTypeName(data)), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            string fieldName = null;
            CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            foreach(IDbFieldEntity field in data.Fields) {
                fieldName = this.GetPrivateInstanceFieldName(field);
                method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisReference, fieldName), new CodeFieldReferenceExpression(infoReference, fieldName)));
            }

            return method;
        }

        private CodeMemberMethod CreateCloneMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(object));
            method.Name = ICloneable_Clone;

            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                "MemberwiseClone")));

            return method;
        }

        private ICollection<CodeTypeMember> CreateEntityProperty(IDbTableFieldEntity entity) {
            string description = entity.Description ?? entity.Name;
            ICollection<CodeTypeMember> members = new List<CodeTypeMember>(3);

            //create protected field member
            CodeMemberField field = new CodeMemberField(entity.Type.IsValueType && entity.IsNullable ? new CodeTypeReference(NullableTypeDefinition.MakeGenericType(entity.Type)) : new CodeTypeReference(entity.Type), this.GetPrivateInstanceFieldName(entity));
            members.Add(field);
            field.Attributes = MemberAttributes.Family;
            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement(string.Format("{0} field.", description), true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create public property memebr
            CodeMemberProperty property = new CodeMemberProperty();
            members.Add(property);
            property.Attributes = MemberAttributes.Public;
            property.Type = field.Type;
            property.Name = entity.Name;
            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(DescriptionAttribute)),
                new CodeAttributeArgument(new CodePrimitiveExpression(entity.Description ?? entity.Name))));
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
            if(entity.MaxLength > 0 && (entity.Type.Equals(typeof(string)) || entity.Type.IsArray)) {
                property.SetStatements.Add(new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                        CodeBinaryOperatorType.BooleanAnd,
                        new CodeBinaryOperatorExpression(
                            new CodePropertyReferenceExpression(new CodePropertySetValueReferenceExpression(), String_Length),
                            CodeBinaryOperatorType.GreaterThan,
                            new CodePrimitiveExpression(entity.MaxLength))),
                    entity.Type.IsArray ?
                    (CodeStatement) new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(Array)),
                        Array_Resize,
                        new CodeDirectionExpression(FieldDirection.Ref, new CodePropertySetValueReferenceExpression()),
                        new CodePrimitiveExpression(entity.MaxLength))) :
                    (CodeStatement) new CodeAssignStatement(
                        new CodePropertySetValueReferenceExpression(),
                        new CodeMethodInvokeExpression(
                            new CodePropertySetValueReferenceExpression(),
                            String_Substring,
                            new CodePrimitiveExpression(0),
                            new CodePrimitiveExpression(entity.MaxLength)))));
            }
            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePropertySetValueReferenceExpression()),
                new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), new CodePropertySetValueReferenceExpression()),
                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    NotifyPropertyChanged_OnPropertyChanged,
                    new CodePrimitiveExpression(property.Name)))));
            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement(string.Format("Gets or sets {0}.", description), true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create public max-length property member
            if(entity.MaxLength > 0 && (entity.Type.Equals(typeof(string)) || entity.Type.IsArray)) {
                property = new CodeMemberProperty();
                members.Add(property);
                property.Attributes = (property.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Public | MemberAttributes.Static;
                property.Type = new CodeTypeReference(typeof(int));
                property.Name = this.GetMaxLengthPropertyName(entity);
                property.HasGet = true;
                property.HasSet = false;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(entity.MaxLength)));
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement(string.Format("Gets max-length of {0}.", description), true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
            }

            return members;
        }

        private ICollection<CodeTypeMember> CreateEntityProperty(IDbViewFieldEntity entity) {
            ICollection<CodeTypeMember> members = new List<CodeTypeMember>(3);

            //create protected field member
            CodeMemberField field = new CodeMemberField(entity.Type.IsValueType && entity.IsNullable ? new CodeTypeReference(NullableTypeDefinition.MakeGenericType(entity.Type)) : new CodeTypeReference(entity.Type), this.GetPrivateInstanceFieldName(entity));
            members.Add(field);
            field.Attributes = MemberAttributes.Family;
            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement(string.Format("{0} field.", entity.Name), true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create public property memebr
            CodeMemberProperty property = new CodeMemberProperty();
            members.Add(property);
            property.Attributes = MemberAttributes.Public;
            property.Type = field.Type;
            property.Name = entity.Name;
            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(DescriptionAttribute)),
                new CodeAttributeArgument(new CodePrimitiveExpression(entity.Name))));
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePropertySetValueReferenceExpression()),
                new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), new CodePropertySetValueReferenceExpression()),
                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    NotifyPropertyChanged_OnPropertyChanged,
                    new CodePrimitiveExpression(property.Name)))));
            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement(string.Format("Gets or sets {0}.", entity.Name), true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create public max-length property member
            if(entity.MaxLength > 0 && (entity.Type.Equals(typeof(string)) || entity.Type.IsArray)) {
                property = new CodeMemberProperty();
                members.Add(property);
                property.Attributes = (property.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Public | MemberAttributes.Static;
                property.Type = new CodeTypeReference(typeof(int));
                property.Name = this.GetMaxLengthPropertyName(entity);
                property.HasGet = true;
                property.HasSet = false;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(entity.MaxLength)));
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement(string.Format("Gets max-length of {0}.", entity.Name), true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
            }

            return members;
        }

        private CodeMemberField CreateMembersField(string name, long value, string description) {
            CodeMemberField field = new CodeMemberField(typeof(Enum), name);
            field.InitExpression = new CodePrimitiveExpression(value);
            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement(string.Format("{0}.", description), true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));
            return field;
        }

        private void CreateQueryConditionProperty(IDbFieldEntity entity, string description, out CodeMemberField field, out CodeMemberProperty property) {
            //create protected field member
            field = new CodeMemberField(entity.Type.IsValueType ? new CodeTypeReference(NullableTypeDefinition.MakeGenericType(entity.Type)) : new CodeTypeReference(entity.Type), this.GetPrivateInstanceFieldName(entity));
            field.Attributes = MemberAttributes.Family;
            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement(string.Format("{0} field.", description), true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create public property memebr
            property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public;
            property.Type = field.Type;
            property.Name = entity.Name;
            property.CustomAttributes.Add(new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(DescriptionAttribute)),
                new CodeAttributeArgument(new CodePrimitiveExpression(description))));
            //property.GetStatements.Add(new CodeMethodReturnStatement(entity.Type.IsString() ?
            //    this.DbInfoProvider.GetLikeString(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)) :
            //    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
            /*
             * Property geter should return the original value,
             * Consider a case: condition2.Property = condition1.Property,
             * If geter return encoded value, then the return value of condition2.Property is duplicated encoding.
             */
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePropertySetValueReferenceExpression()),
                new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), new CodePropertySetValueReferenceExpression()),
                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    NotifyPropertyChanged_OnPropertyChanged,
                    new CodePrimitiveExpression(property.Name)))));
            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement(string.Format("Gets or sets {0}.", description), true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));
        }

        private CodeTypeConstructor CreateStaticConstructorOfAccessorType(IDbDataEntity data) {
            CodeTypeConstructor constructor = new CodeTypeConstructor();

            List<CodeExpression> allFields = new List<CodeExpression>(data.Fields.Count());
            //create data source field
            string dataSourceFiledName = this.GetDataSourceFieldName(data);
            constructor.Statements.Add(new CodeAssignStatement(
              new CodeFieldReferenceExpression(null, dataSourceFiledName),
              this.m_dbInfoProvider.GetSqlSource(!string.IsNullOrWhiteSpace(data.Schema) ? new CodePrimitiveExpression(data.Schema) : null, new CodePrimitiveExpression(data.Name))));

            //create data item field
            foreach(IDbFieldEntity field in data.Fields) {
                constructor.Statements.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)),
                    this.m_dbInfoProvider.GetSqlField(new CodeFieldReferenceExpression(null, dataSourceFiledName), new CodePrimitiveExpression(field.Name))));
                allFields.Add(new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)));
            }

            //create all item field
            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(null, this.GetAllFieldsFieldName(data)),
                new CodeArrayCreateExpression(typeof(ISqlObject), allFields.ToArray())));

            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("Initialize class {0}.", this.GetAccssorTypeName(data)), true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
            constructor.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Static Constructor"));
            constructor.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));

            return constructor;
        }

        private CodeConstructor[] CreateInstanceConstructorOfAccessorType(IDbDataEntity data) {
            ICollection<CodeConstructor> members = new List<CodeConstructor>(2);

            // constructor 1: with one connection string.
            CodeConstructor constructor = new CodeConstructor {
                Attributes = MemberAttributes.Public,
            };
            CodeParameterDeclarationExpression connectionStringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression connectionStringReference = new CodeArgumentReferenceExpression(connectionStringArgument.Name);
            constructor.Parameters.Add(connectionStringArgument);

            constructor.Statements.Add(new CodeAssignStatement(
                  new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), this.m_connectionStringPropertyName),
                  connectionStringReference));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringSelectorFieldName),
                new CodeObjectCreateExpression(typeof(DefaultWriteConnectionStringSelector))));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringSelectorFieldName),
                new CodeObjectCreateExpression(typeof(DefaultReadConnectionStringSelector))));

            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("Initialize a new instance of class {0}.", this.GetAccssorTypeName(data)), true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Connection string.</param>", connectionStringArgument.Name), true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"{0}\"><paramref name=\"{1}\"/> is null or empty.</exception>", typeof(ArgumentException).FullName, connectionStringArgument.Name), true));
            constructor.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Constructor"));
            members.Add(constructor);

            // constructor 2: with write and read connection strings.
            constructor = new CodeConstructor {
                Attributes = MemberAttributes.Public,
            };
            CodeParameterDeclarationExpression writeConnectionStringsArgument = new CodeParameterDeclarationExpression(typeof(IList<string>), "writeConnectionStrings");
            CodeParameterDeclarationExpression readConnectionStringsArgument = new CodeParameterDeclarationExpression(typeof(IList<string>), "readConnectionStrings");
            CodeParameterDeclarationExpression writeConnectionStringSelectorArgument = new CodeParameterDeclarationExpression(typeof(IConnectionStringSelector), "writeConnectionStringSelector");
            CodeParameterDeclarationExpression readConnectionStringSelectorArgument = new CodeParameterDeclarationExpression(typeof(IConnectionStringSelector), "readConnectionStringSelector");
            constructor.Parameters.Add(writeConnectionStringsArgument);
            constructor.Parameters.Add(readConnectionStringsArgument);
            constructor.Parameters.Add(writeConnectionStringSelectorArgument);
            constructor.Parameters.Add(readConnectionStringSelectorArgument);

            constructor.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsPropertyName),
                new CodeArgumentReferenceExpression(writeConnectionStringsArgument.Name)));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsPropertyName),
                new CodeArgumentReferenceExpression(readConnectionStringsArgument.Name)));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringSelectorFieldName),
                new CodeArgumentReferenceExpression(writeConnectionStringSelectorArgument.Name)));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringSelectorFieldName),
                new CodeArgumentReferenceExpression(readConnectionStringSelectorArgument.Name)));

            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("Initialize a new instance of class {0}.", this.GetAccssorTypeName(data)), true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
            members.Add(constructor);

            // constructor 3: with none parameters.
            constructor = new CodeConstructor {
                Attributes = MemberAttributes.Public,
            };

            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName),
                new CodeObjectCreateExpression(typeof(List<string>))));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName),
                new CodeObjectCreateExpression(typeof(List<string>))));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringSelectorFieldName),
                new CodeObjectCreateExpression(typeof(DefaultWriteConnectionStringSelector))));
            constructor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringSelectorFieldName),
                new CodeObjectCreateExpression(typeof(DefaultReadConnectionStringSelector))));

            constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
            constructor.Comments.Add(new CodeCommentStatement(string.Format("Initialize a new instance of class {0}.", this.GetAccssorTypeName(data)), true));
            constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
            constructor.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            members.Add(constructor);

            return members.ToArray();
        }

        private ICollection<CodeTypeMember> CreateSqlObjectFields(IDbDataEntity data) {
            CodeMemberField field = null;
            CodeMemberProperty property = new CodeMemberProperty();
            ICollection<CodeTypeMember> members = new List<CodeTypeMember>(data.Fields.Count() + 1);

            //create data source property
            field = new CodeMemberField(typeof(ISqlObject), this.GetDataSourceFieldName(data));
            field.Attributes = (field.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Assembly | MemberAttributes.Static;
            field.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Sql Object Fields"));
            members.Add(field);

            property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(ISqlObject));
            property.Name = this.GetDataSourcePropertyName(data);
            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Data source.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));
            property.HasGet = true;
            property.HasSet = false;
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(data))));
            members.Add(property);

            //create data field properties
            Type specificDbType = this.m_dbInfoProvider.GetDbTypeType();
            foreach(IDbFieldEntity entity in data.Fields) {
                //data field
                field = new CodeMemberField(typeof(ISqlObject), this.GetPrivateStaticFieldName(entity));
                field.Attributes = (field.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Assembly | MemberAttributes.Static;
                members.Add(field);

                //data property
                property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public;
                property.Type = new CodeTypeReference(typeof(ISqlObject));
                property.Name = entity.Name;
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement(string.Format("Data field: {0}.", entity.Name), true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
                property.HasGet = true;
                property.HasSet = false;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, field.Name)));
                members.Add(property);

                //data DbType property
                property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public;
                property.Type = new CodeTypeReference(typeof(DbType));
                property.Name = this.GetDbTypePropertyName(entity);
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement(string.Format("Gets DbType of {0}.", field.Name), true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
                property.HasGet = true;
                property.HasSet = false;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DbType)), Enum.GetName(typeof(DbType), entity.DbType))));
                members.Add(property);

                //data specific DbType property
                property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public;
                property.Type = new CodeTypeReference(specificDbType);
                property.Name = this.GetSpecificDbTypePropertyName(entity, specificDbType);
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement(string.Format("Gets {1} of {0}.", field.Name, specificDbType.Name), true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
                property.HasGet = true;
                property.HasSet = false;
                property.GetStatements.Add(new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbTypeValue(entity)));
                members.Add(property);
            }

            //create all fields property
            field = new CodeMemberField(typeof(ISqlObject[]), this.GetAllFieldsFieldName(data));
            field.Attributes = (field.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Assembly | MemberAttributes.Static;
            members.Add(field);

            property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(ISqlObject[]));
            property.Name = this.GetAllFieldsPropertyName(data);
            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Gets all data fields.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));
            property.HasGet = true;
            property.HasSet = false;
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, field.Name)));
            property.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            members.Add(property);

            return members;
        }

        private CodeMemberField CreateParameterIndexField() {
            CodeMemberField field = new CodeMemberField(typeof(long), this.m_parameterIndexFieldName);
            field.Attributes = (field.Attributes & ~MemberAttributes.ScopeMask & ~MemberAttributes.AccessMask) | MemberAttributes.Family | MemberAttributes.Static;

            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement("Paramter index, represents the next usable index.", true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            return field;
        }

        private CodeMemberField CreateWriteConnectionStringsField() {
            CodeMemberField field = new CodeMemberField(typeof(IList<string>), this.m_writeConnectionStringsFieldName);
            field.Attributes = (field.Attributes & ~MemberAttributes.ScopeMask & ~MemberAttributes.AccessMask) | MemberAttributes.Family;

            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement("Write connection string list.", true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            return field;
        }

        private CodeMemberField CreateReadConnectionStringsField() {
            CodeMemberField field = new CodeMemberField(typeof(IList<string>), this.m_readConnectionStringsFieldName);
            field.Attributes = (field.Attributes & ~MemberAttributes.ScopeMask & ~MemberAttributes.AccessMask) | MemberAttributes.Family;

            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement("Read connection string list.", true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            return field;
        }

        private CodeMemberField CreateWriteConnectionStringSelectorField() {
            CodeMemberField field = new CodeMemberField(typeof(IConnectionStringSelector), this.m_writeConnectionStringSelectorFieldName);
            field.Attributes = (field.Attributes & ~MemberAttributes.ScopeMask & ~MemberAttributes.AccessMask) | MemberAttributes.Family;

            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement("The selector to get write connection string.", true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            return field;
        }

        private CodeMemberField CreateReadConnectionStringSelectorField() {
            CodeMemberField field = new CodeMemberField(typeof(IConnectionStringSelector), this.m_readConnectionStringSelectorFieldName);
            field.Attributes = (field.Attributes & ~MemberAttributes.ScopeMask & ~MemberAttributes.AccessMask) | MemberAttributes.Family;

            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement("The selector to get read connection string.", true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            return field;
        }

        private CodeMemberProperty CreateWriteConnectionStringsProperty() {
            CodeMemberProperty property = new CodeMemberProperty();

            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(IList<string>));
            property.Name = this.m_writeConnectionStringsPropertyName;

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName)));

            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeAssignStatement(
                    new CodePropertySetValueReferenceExpression(), 
                    new CodeObjectCreateExpression(typeof(List<string>)))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName), new CodePropertySetValueReferenceExpression()));

            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Gets or sets the write connection string list.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            return property;
        }

        private CodeMemberProperty CreateReadConnectionStringsProperty() {
            CodeMemberProperty property = new CodeMemberProperty();

            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(IList<string>));
            property.Name = this.m_readConnectionStringsPropertyName;

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName)));

            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeAssignStatement(
                    new CodePropertySetValueReferenceExpression(),
                    new CodeObjectCreateExpression(typeof(List<string>)))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName), new CodePropertySetValueReferenceExpression()));

            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Gets or sets the read connection string list.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            return property;
        }

        private CodeMemberProperty CreateWriteConnectionStringSelectorProperty() {
            CodeMemberProperty property = new CodeMemberProperty();

            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(IConnectionStringSelector));
            property.Name = this.m_writeConnectionStringSelectorPropertyName;

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringSelectorFieldName)));

            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeAssignStatement(
                    new CodePropertySetValueReferenceExpression(),
                    new CodeObjectCreateExpression(typeof(DefaultWriteConnectionStringSelector)))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringSelectorFieldName), new CodePropertySetValueReferenceExpression()));

            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Gets or sets the write connection string selector.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            return property;
        }

        private CodeMemberProperty CreateReadConnectionStringSelectorProperty() {
            CodeMemberProperty property = new CodeMemberProperty();

            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(IConnectionStringSelector));
            property.Name = this.m_readConnectionStringSelectorPropertyName;

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringSelectorFieldName)));

            property.SetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeAssignStatement(
                    new CodePropertySetValueReferenceExpression(),
                    new CodeObjectCreateExpression(typeof(DefaultWriteConnectionStringSelector)))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringSelectorFieldName), new CodePropertySetValueReferenceExpression()));

            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Gets or sets the read connection string selector.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            return property;
        }

        private CodeMemberProperty CreateConnectionStringProperty() {
            CodeMemberProperty property = new CodeMemberProperty();

            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference(typeof(string));
            property.Name = this.m_connectionStringPropertyName;

            CodeExpression zero = new CodePrimitiveExpression(0);
            CodeFieldReferenceExpression writeField = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName);
            CodeFieldReferenceExpression readField = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName);

            property.GetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(writeField, ICollection_Count),  CodeBinaryOperatorType.GreaterThan, zero),
                new CodeMethodReturnStatement(new CodeIndexerExpression(writeField, zero))));
            property.GetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(readField, ICollection_Count), CodeBinaryOperatorType.GreaterThan, zero),
                new CodeMethodReturnStatement(new CodeIndexerExpression(readField, zero))));
            property.GetStatements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(InvalidOperationException), new CodePrimitiveExpression("The connection string is undefined."))));

            property.SetStatements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(string)),
                    String_IsNullOrWhiteSpace,
                    new CodePropertySetValueReferenceExpression()),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression("value")))));
            property.SetStatements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement listVariable = new CodeVariableDeclarationStatement(typeof(string[]), "list", new CodeArrayCreateExpression(typeof(string), new CodePrimitiveExpression(1)));
            CodeVariableReferenceExpression listReference = new CodeVariableReferenceExpression(listVariable.Name);

            property.SetStatements.Add(listVariable);
            property.SetStatements.Add(new CodeAssignStatement(
                new CodeArrayIndexerExpression(listReference, new CodePrimitiveExpression(0)),
                new CodePropertySetValueReferenceExpression()));
            property.SetStatements.Add(new CodeSnippetStatement());

            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName), new CodeObjectCreateExpression(typeof(List<string>), listReference)));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName), new CodeObjectCreateExpression(typeof(List<string>), listReference)));

            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement("Gets or sets connection string.", true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));
            property.Comments.Add(new CodeCommentStatement(string.Format("<exception cref=\"{0}\">Connection string is null or empty.</exception>", typeof(ArgumentException).FullName), true));

            return property;
        }

        private CodeMemberMethod CreateGetWriteConnectionString() {
            // create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(string));
            method.Name = this.m_getWriteConnectionStringMethodName;

            // create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Gets the write connection string.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringSelectorFieldName),
                IConnectionStringSelector_GetConnectionString,
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName),
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName))));

            return method;
        }

        private CodeMemberMethod CreateGetReadConnectionString() {
            // create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(string));
            method.Name = this.m_getReadConnectionStringMethodName;

            // create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Gets the read connection string.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringSelectorFieldName),
                IConnectionStringSelector_GetConnectionString,
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_writeConnectionStringsFieldName),
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.m_readConnectionStringsFieldName))));

            return method;
        }

        private CodeMemberMethod CreateLoadFromReaderMethod(IDbDataEntity data, CodeTypeParameter typeParameter) {
            // create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.FamilyAndAssembly;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadFromReaderMethodName;

            // create method arguments
            CodeParameterDeclarationExpression readerArgument = new CodeParameterDeclarationExpression(typeof(IDataReader), "reader");
            CodeArgumentReferenceExpression readerReference = new CodeArgumentReferenceExpression(readerArgument.Name);
            method.Parameters.Add(readerArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            // create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Load data from a IDataReader object.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A IDataReader object.</param>", readerArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Fields which will be load.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>A collection of {0}.</returns>", this.GetEntityTypeName(data)), true));

            // create field index variables
            method.Statements.Add(new CodeCommentStatement("field indices"));
            Type indexVariableType = typeof(int);
            CodeExpression indexVariableDefaultValue = new CodePrimitiveExpression(-1);
            IDictionary<IDbFieldEntity, CodeVariableDeclarationStatement> indexVariables = new Dictionary<IDbFieldEntity, CodeVariableDeclarationStatement>(data.Fields.Count());
            foreach(IDbFieldEntity entity in data.Fields) {
                method.Statements.Add(indexVariables[entity] = new CodeVariableDeclarationStatement(
                    indexVariableType,
                    string.Format("{0}{1}Index", entity.Name.Substring(0, 1).ToLower(), entity.Name.Substring(1)),
                    indexVariableDefaultValue));
            }
            method.Statements.Add(new CodeSnippetStatement());

            // create statements for seting value of field index variables
            method.Statements.Add(new CodeCommentStatement("check field indices"));
            CodeVariableDeclarationStatement fieldNameVariable = new CodeVariableDeclarationStatement(typeof(string), "fieldName");
            CodeVariableReferenceExpression fieldNameReference = new CodeVariableReferenceExpression(fieldNameVariable.Name);

            // create for direct assign statement
            int fieldIndex = 0;
            ICollection<CodeStatement> assignStatements = new List<CodeStatement>();
            foreach(IDbFieldEntity field in data.Fields) {
                assignStatements.Add(new CodeAssignStatement(
                    new CodeVariableReferenceExpression(indexVariables[field].Name),
                    new CodePrimitiveExpression(fieldIndex++)));
            }

            // create for iteration assign statement
            CodeVariableDeclarationStatement iterationVariable = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));
            CodeVariableReferenceExpression iternationReference = new CodeVariableReferenceExpression(iterationVariable.Name);
            CodeIterationStatement forStatement2 = new CodeIterationStatement(
                iterationVariable,
                new CodeBinaryOperatorExpression(
                    iternationReference,
                    CodeBinaryOperatorType.LessThan,
                    new CodePropertyReferenceExpression(fieldsReference, Array_Length)),
                new CodeAssignStatement(
                    iternationReference,
                    new CodeBinaryOperatorExpression(iternationReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

            // assign field name variable
            forStatement2.Statements.Add(new CodeAssignStatement(
              fieldNameReference,
              new CodePropertyReferenceExpression(
                  new CodeIndexerExpression(fieldsReference, iternationReference),
                  ISqlObject_Name)));

            // determine value of field index
            CodeConditionStatement switchStatement = null, branchStatement = null;
            foreach(IDbFieldEntity entity in data.Fields) {
                if(branchStatement == null) {
                    branchStatement = new CodeConditionStatement();
                    branchStatement.Condition = new CodeMethodInvokeExpression(fieldNameReference, Object_Equals, new CodePrimitiveExpression(entity.Name));
                    branchStatement.TrueStatements.Add(new CodeAssignStatement(
                        new CodeVariableReferenceExpression(indexVariables[entity].Name),
                        iternationReference));
                    switchStatement = branchStatement;
                    forStatement2.Statements.Add(switchStatement);
                } else {
                    branchStatement.FalseStatements.Add(new CodeConditionStatement(
                        new CodeMethodInvokeExpression(fieldNameReference, Object_Equals, new CodePrimitiveExpression(entity.Name)),
                        new CodeAssignStatement(new CodeVariableReferenceExpression(indexVariables[entity].Name), iternationReference)));
                    branchStatement = (CodeConditionStatement) branchStatement.FalseStatements[0];
                }
            }

            // add if statements
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    fieldsReference,
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeFieldReferenceExpression(null, this.GetAllFieldsFieldName(data))),
                assignStatements.ToArray(),
                new CodeStatement[] { fieldNameVariable, forStatement2 }));
            method.Statements.Add(new CodeSnippetStatement());

            // declare variables
            method.Statements.Add(new CodeCommentStatement("read query result"));
            CodeVariableDeclarationStatement entityVariable = new CodeVariableDeclarationStatement(new CodeTypeReference(typeParameter), "entity", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression entityReference = new CodeVariableReferenceExpression(entityVariable.Name);
            method.Statements.Add(entityVariable);
            CodeVariableDeclarationStatement resultVariable = new CodeVariableDeclarationStatement(
                new CodeTypeReference(typeof(ICollection<>).FullName, new CodeTypeReference(typeParameter)),
                "result",
                new CodeObjectCreateExpression(new CodeTypeReference(typeof(List<>).FullName, new CodeTypeReference(typeParameter))));
            CodeVariableReferenceExpression resultReference = new CodeVariableReferenceExpression(resultVariable.Name);
            method.Statements.Add(resultVariable);

            // create try-catch-finally statement used to dispose IDataReader
            CodeTryCatchFinallyStatement usingStatement = new CodeTryCatchFinallyStatement();
            method.Statements.Add(usingStatement);
            usingStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(
                readerReference,
                IDisposable_Dispose));

            // create for statement used to interate all data.
            CodeIterationStatement whileStatement = new CodeIterationStatement(
                new CodeSnippetStatement(),
                new CodeMethodInvokeExpression(
                    readerReference,
                    IDataReader_Read),
                new CodeSnippetStatement());
            usingStatement.TryStatements.Add(whileStatement);

            // create body of for statement
            whileStatement.Statements.Add(new CodeAssignStatement(entityReference, new CodeObjectCreateExpression(new CodeTypeReference(typeParameter))));
            foreach(IDbFieldEntity entity in data.Fields) {
                if(entity.IsNullable) {
                    whileStatement.Statements.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(indexVariables[entity].Name), CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                            CodeBinaryOperatorType.BooleanOr,
                            new CodeMethodInvokeExpression(
                                readerReference,
                                IDataReader_IsDBNull,
                                new CodeVariableReferenceExpression(indexVariables[entity].Name))),
                        new CodeStatement[0],
                        new CodeStatement[] { new CodeAssignStatement(
                            new CodePropertyReferenceExpression(entityReference, entity.Name),
                                this.m_dbInfoProvider.GetReadValue(readerReference, entity, new CodeVariableReferenceExpression(indexVariables[entity].Name)) ??
                                new CodeCastExpression(entity.Type, new CodeIndexerExpression(readerReference, new CodeVariableReferenceExpression(indexVariables[entity].Name)))) }));
                } else {
                    whileStatement.Statements.Add(new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeVariableReferenceExpression(indexVariables[entity].Name),
                            CodeBinaryOperatorType.GreaterThanOrEqual,
                            new CodePrimitiveExpression(0)),
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(entityReference, entity.Name),
                                this.m_dbInfoProvider.GetReadValue(readerReference, entity, new CodeVariableReferenceExpression(indexVariables[entity].Name)) ??
                                new CodeCastExpression(entity.Type, new CodeIndexerExpression(readerReference, new CodeVariableReferenceExpression(indexVariables[entity].Name))))));
                }
            }
            whileStatement.Statements.Add(new CodeMethodInvokeExpression(
                resultReference,
                ICollection_Add,
                entityReference));
            method.Statements.Add(new CodeSnippetStatement());

            // return
            method.Statements.Add(new CodeMethodReturnStatement(resultReference));

            return method;
        }

        private CodeMemberMethod CreateGetFiledsMethods(IDbDataEntity data, string entityNamespaceName) {
            // create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlObject[]).FullName);
            method.Name = this.m_getFiledsMethodName;

            // create method arguments
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(entityNamespaceName, this.GetMembersTypeName(data)), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            // create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Gets database fields from the specified members.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A members enum.</param>", membersArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>A array of {0}.</returns>", typeof(ISqlObject).Name), true));

            // check boundary condition
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    membersReference,
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(this.GetTypeFullname(entityNamespaceName, this.GetMembersTypeName(data))),
                        MEMBERS_NAME_All)),
                new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, this.GetAllFieldsFieldName(data)))));
            method.Statements.Add(new CodeSnippetStatement());

            // check boundary condition
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    membersReference,
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(this.GetTypeFullname(entityNamespaceName, this.GetMembersTypeName(data))),
                        MEMBERS_NAME_NONE)),
                new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(ISqlObject), 0))));
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement resultVariable = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(ICollection<ISqlObject>)), "result", new CodeObjectCreateExpression(typeof(List<ISqlObject>)));
            CodeVariableReferenceExpression resultReference = new CodeVariableReferenceExpression(resultVariable.Name);
            method.Statements.Add(resultVariable);

            // check each fields
            foreach(IDbFieldEntity field in data.Fields) {
                method.Statements.Add(new CodeConditionStatement(
                    new CodeMethodInvokeExpression(
                        membersReference, Enum_HasFlag,
                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.GetTypeFullname(entityNamespaceName, this.GetMembersTypeName(data))), field.Name)),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        resultReference, ICollection_Add,
                        new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field))))));
            }

            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(resultReference, IEnumerable_ToArray)));

            return method;
        }

        private ICollection<CodeMemberMethod> CreateCreateParameterMethods(IDbDataEntity data) {
            CodeMemberMethod method = null;
            CodeParameterDeclarationExpression nameParameter = null, valueParameter = null, typeParameter = null;
            IList<CodeMemberMethod> methods = new List<CodeMemberMethod>(data.Fields.Count() * 4);

            foreach(IDbFieldEntity field in data.Fields) {
                // create CreateParameter()
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(IDataParameter));
                method.Name = this.GetCreateParameterMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeConditionStatement(
                    new CodeArgumentReferenceExpression(nameParameter.Name),
                    new CodeStatement[] {
                        new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbParameter(
                            new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)), 
                            field,
                            new CodeArgumentReferenceExpression(valueParameter.Name))),
                    },
                    new CodeStatement[] {
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeFieldReferenceExpression(null, this.m_parameterIndexFieldName),
                                CodeBinaryOperatorType.IdentityEquality,
                                new CodePrimitiveExpression(long.MaxValue)),
                            new CodeAssignStatement(
                                new CodeFieldReferenceExpression(null, this.m_parameterIndexFieldName),
                                new CodePrimitiveExpression(0))),
                        new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbParameter(
                            new CodeBinaryOperatorExpression(
                                new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)),
                                CodeBinaryOperatorType.Add,
                                new CodeSnippetExpression(string.Format("{0}++", this.m_parameterIndexFieldName))), 
                            field,
                            new CodeArgumentReferenceExpression(valueParameter.Name))),
                    }));
                methods.Add(method);

                // create CreateParameter() with name
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(IDataParameter));
                method.Name = this.GetCreateParameterMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(string), this.GetDataParameterName(field)));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbParameter(
                    new CodeArgumentReferenceExpression(nameParameter.Name),
                    field,
                    new CodeArgumentReferenceExpression(valueParameter.Name))));
                methods.Add(method);

                // create CreateParameter() with common DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(IDataParameter));
                method.Name = this.GetCreateParameterMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(typeof(DbType), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeConditionStatement(
                    new CodeArgumentReferenceExpression(nameParameter.Name),
                    new CodeStatement[] {
                        new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                                SqlHelper_CreateParameter,
                                new CodeTypeReference(this.m_dbInfoProvider.GetDbParameterType())),
                            new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)),
                            new CodeArgumentReferenceExpression(typeParameter.Name),
                            new CodeArgumentReferenceExpression(valueParameter.Name))),
                    },
                    new CodeStatement[] {
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeFieldReferenceExpression(null, this.m_parameterIndexFieldName),
                                CodeBinaryOperatorType.IdentityEquality,
                                new CodePrimitiveExpression(long.MaxValue)),
                            new CodeAssignStatement(
                                new CodeFieldReferenceExpression(null, this.m_parameterIndexFieldName),
                                new CodePrimitiveExpression(0))),
                        new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                                SqlHelper_CreateParameter,
                                new CodeTypeReference(this.m_dbInfoProvider.GetDbParameterType())),
                            new CodeBinaryOperatorExpression(
                                new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)),
                                CodeBinaryOperatorType.Add,
                                new CodeSnippetExpression(string.Format("{0}++", this.m_parameterIndexFieldName))),
                            new CodeArgumentReferenceExpression(typeParameter.Name),
                            new CodeArgumentReferenceExpression(valueParameter.Name))),
                    }));
                methods.Add(method);

                // create CreateParameter() with name and common DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(IDataParameter));
                method.Name = this.GetCreateParameterMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(string), this.GetDataParameterName(field)));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(typeof(DbType), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_CreateParameter,
                        new CodeTypeReference(this.m_dbInfoProvider.GetDbParameterType())),
                   new CodeArgumentReferenceExpression(nameParameter.Name),
                   new CodeArgumentReferenceExpression(typeParameter.Name),
                   new CodeArgumentReferenceExpression(valueParameter.Name))));
                methods.Add(method);

                // create CreateParameter() with specific DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(IDataParameter));
                method.Name = this.GetCreateParameterMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetDbTypeType(), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeConditionStatement(
                    new CodeArgumentReferenceExpression(nameParameter.Name),
                    new CodeStatement[] {
                        new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbParameter(
                            new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)),
                            new CodeArgumentReferenceExpression(typeParameter.Name),
                            new CodeArgumentReferenceExpression(valueParameter.Name))),
                    },
                    new CodeStatement[] {
                        new CodeConditionStatement(
                            new CodeBinaryOperatorExpression(
                                new CodeFieldReferenceExpression(null, this.m_parameterIndexFieldName),
                                CodeBinaryOperatorType.IdentityEquality,
                                new CodePrimitiveExpression(long.MaxValue)),
                            new CodeAssignStatement(
                                new CodeFieldReferenceExpression(null, this.m_parameterIndexFieldName),
                                new CodePrimitiveExpression(0))),
                        new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbParameter(
                            new CodeBinaryOperatorExpression(
                                new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)),
                                CodeBinaryOperatorType.Add,
                                new CodeSnippetExpression(string.Format("{0}++", this.m_parameterIndexFieldName))),
                            new CodeArgumentReferenceExpression(typeParameter.Name),
                            new CodeArgumentReferenceExpression(valueParameter.Name))),
                    }));
                methods.Add(method);

                // create CreateParameter() with name and specific DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(IDataParameter));
                method.Name = this.GetCreateParameterMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(string), this.GetDataParameterName(field)));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetDbTypeType(), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(this.m_dbInfoProvider.GetDbParameter(
                    new CodeArgumentReferenceExpression(nameParameter.Name),
                    new CodeArgumentReferenceExpression(typeParameter.Name),
                    new CodeArgumentReferenceExpression(valueParameter.Name))));
                methods.Add(method);

                // create CreateParameterExpression()
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
                method.Name = this.GetCreateParameterExpressionMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlStringExpression)),
                    SqlStringExpression_FromParameter,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.GetCreateParameterMethodName(field),
                        new CodeArgumentReferenceExpression(nameParameter.Name),
                        new CodeArgumentReferenceExpression(valueParameter.Name)))));
                methods.Add(method);

                // create CreateParameterExpression() with name
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
                method.Name = this.GetCreateParameterExpressionMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(string), this.GetDataParameterName(field)));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlStringExpression)),
                    SqlStringExpression_FromParameter,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.GetCreateParameterMethodName(field),
                        new CodeArgumentReferenceExpression(nameParameter.Name),
                        new CodeArgumentReferenceExpression(valueParameter.Name)))));
                methods.Add(method);

                // create CreateParameterExpression() with common DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
                method.Name = this.GetCreateParameterExpressionMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(typeof(DbType), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlStringExpression)),
                    SqlStringExpression_FromParameter,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.GetCreateParameterMethodName(field),
                        new CodeArgumentReferenceExpression(nameParameter.Name),
                        new CodeArgumentReferenceExpression(typeParameter.Name),
                        new CodeArgumentReferenceExpression(valueParameter.Name)))));
                methods.Add(method);

                // create CreateParameterExpression() with name and common DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
                method.Name = this.GetCreateParameterExpressionMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(string), this.GetDataParameterName(field)));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(typeof(DbType), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlStringExpression)),
                    SqlStringExpression_FromParameter,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.GetCreateParameterMethodName(field),
                        new CodeArgumentReferenceExpression(nameParameter.Name),
                        new CodeArgumentReferenceExpression(typeParameter.Name),
                        new CodeArgumentReferenceExpression(valueParameter.Name)))));
                methods.Add(method);

                // create CreateParameterExpression() with specific DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
                method.Name = this.GetCreateParameterExpressionMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetDbTypeType(), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlStringExpression)),
                    SqlStringExpression_FromParameter,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.GetCreateParameterMethodName(field),
                        new CodeArgumentReferenceExpression(nameParameter.Name),
                        new CodeArgumentReferenceExpression(typeParameter.Name),
                        new CodeArgumentReferenceExpression(valueParameter.Name)))));
                methods.Add(method);

                // create CreateParameterExpression() with name and specific DbType
                method = new CodeMemberMethod();
                method.Attributes = MemberAttributes.Public;
                method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
                method.Name = this.GetCreateParameterExpressionMethodName(field);
                method.Parameters.Add(nameParameter = new CodeParameterDeclarationExpression(typeof(string), this.GetDataParameterName(field)));
                method.Parameters.Add(typeParameter = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetDbTypeType(), "dbType"));
                method.Parameters.Add(valueParameter = new CodeParameterDeclarationExpression(field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type, this.GetVariableName(field.Name)));
                method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlStringExpression)),
                    SqlStringExpression_FromParameter,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.GetCreateParameterMethodName(field),
                        new CodeArgumentReferenceExpression(nameParameter.Name),
                        new CodeArgumentReferenceExpression(typeParameter.Name),
                        new CodeArgumentReferenceExpression(valueParameter.Name)))));
                methods.Add(method);
            }

            if(methods.Count > 0) {
                methods[0].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("Create Parameter Methods (not sync read and write of {0} for performance)", this.m_parameterIndexFieldName)));
                methods[methods.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            }

            return methods;
        }

        private CodeMemberMethod CreateGetSelectStatementMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getSelectStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            CodeArgumentReferenceExpression countReference = new CodeArgumentReferenceExpression(countArgument.Name);
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(fieldsReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create sql statement
            method.Statements.Add(new CodeCommentStatement("construct SQL statement"));
            CodeVariableDeclarationStatement selectStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "selectStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression selectStatementReference = new CodeVariableReferenceExpression(selectStatementVariable.Name);
            method.Statements.Add(selectStatementVariable);

            //assign count
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    selectStatementReference,
                    ISqlSelectStatement_Count),
                countReference));

            //assign fields
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(
                    selectStatementReference,
                    ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                fieldsReference));

            //assign data source
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        selectStatementReference,
                        ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(data))));

            //assign query condition
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        selectStatementReference,
                        ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                conditionReference));

            //assign query orders
            CodeVariableDeclarationStatement enumeratorVariable = new CodeVariableDeclarationStatement(
                new CodeTypeReference(typeof(IEnumerator<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))),
                "enumerator",
                new CodeMethodInvokeExpression(
                    ordersReference,
                    IEnumerable_GetEnumerator));
            CodeVariableReferenceExpression enumeratorReference = new CodeVariableReferenceExpression(enumeratorVariable.Name);
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression(ordersArgument.Name), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                enumeratorVariable,
                new CodeIterationStatement(
                    new CodeSnippetStatement(),
                    new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                            selectStatementReference,
                            ISqlSelectStatement_OrderClause),
                        ISqlOrderClause_AddExpression,
                        new CodePropertyReferenceExpression(
                            enumeratorReference,
                            IEnumerator_Current))))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(selectStatementReference));

            return method;
        }

        private CodeMemberMethod CreateGetSelectStatementMethodMembers(IDbDataEntity data, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getSelectStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            CodeArgumentReferenceExpression countReference = new CodeArgumentReferenceExpression(countArgument.Name);
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_getSelectStatementMethodName,
                countReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodWithConnectionString(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, new CodeArgumentReferenceExpression(stringArgument.Name)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(
                            new CodeArgumentReferenceExpression(fieldsArgument.Name),
                            Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        new CodeArgumentReferenceExpression(stringArgument.Name)),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_loadMethodName,
                        connectionReference,
                        new CodeArgumentReferenceExpression(countArgument.Name),
                        new CodeArgumentReferenceExpression(conditionArgument.Name),
                        new CodeArgumentReferenceExpression(ordersArgument.Name),
                        fieldsReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateLoadMethod(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), 
                this.m_loadMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getReadConnectionStringMethodName),
                new CodeArgumentReferenceExpression(countArgument.Name),
                new CodeArgumentReferenceExpression(conditionArgument.Name),
                new CodeArgumentReferenceExpression(ordersArgument.Name),
                new CodeArgumentReferenceExpression(fieldsArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodMembersWithConnectionString(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            CodeArgumentReferenceExpression countReference = new CodeArgumentReferenceExpression(countArgument.Name);
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                stringReference, countReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodMembers(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            CodeArgumentReferenceExpression countReference = new CodeArgumentReferenceExpression(countArgument.Name);
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                countReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodConnection(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(
                            new CodeArgumentReferenceExpression(fieldsArgument.Name),
                            Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //execute sql statement
            method.Statements.Add(new CodeCommentStatement("read query result"));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadFromReaderMethodName,
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlHelper)),
                    SqlHelper_ExecuteSelect,
                    new CodeArgumentReferenceExpression(connectionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getSelectStatementMethodName,
                        new CodeArgumentReferenceExpression(countArgument.Name),
                        new CodeArgumentReferenceExpression(conditionArgument.Name),
                        new CodeArgumentReferenceExpression(ordersArgument.Name),
                        fieldsReference)),
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodConnectionMembers(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            CodeArgumentReferenceExpression connectionReference = new CodeArgumentReferenceExpression(connectionArgument.Name);
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            CodeArgumentReferenceExpression countReference = new CodeArgumentReferenceExpression(countArgument.Name);
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                connectionReference, countReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodTransaction(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(
                            new CodeArgumentReferenceExpression(fieldsArgument.Name),
                            Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //execute sql statement
            method.Statements.Add(new CodeCommentStatement("read query result"));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadFromReaderMethodName,
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlHelper)),
                    SqlHelper_ExecuteSelect,
                    new CodeArgumentReferenceExpression(transactionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getSelectStatementMethodName,
                        new CodeArgumentReferenceExpression(countArgument.Name),
                        new CodeArgumentReferenceExpression(conditionArgument.Name),
                        new CodeArgumentReferenceExpression(ordersArgument.Name),
                        fieldsReference)),
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodTransactionMembers(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            CodeArgumentReferenceExpression transactionReference = new CodeArgumentReferenceExpression(transactionArgument.Name);
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            CodeArgumentReferenceExpression countReference = new CodeArgumentReferenceExpression(countArgument.Name);
            method.Parameters.Add(countArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                transactionReference, countReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodWithConnectionString(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            CodeMethodInvokeExpression returnValue = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                stringReference,
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(countArgument.Name) : new CodePrimitiveExpression(-1),
                this.CreateCondition(constraint),
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(ordersArgument.Name) : new CodePrimitiveExpression(null),
                new CodeArgumentReferenceExpression(fieldsArgument.Name));
            method.Statements.Add(new CodeMethodReturnStatement(constraint.IsUnique ?
                new CodeMethodInvokeExpression(
                    returnValue,
                    IEnumerable_FirstOrDefault) :
                returnValue));

            return method;
        }

        private CodeMemberMethod CreateLoadMethod(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            CodeMethodInvokeExpression returnValue = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(countArgument.Name) : new CodePrimitiveExpression(-1),
                this.CreateCondition(constraint),
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(ordersArgument.Name) : new CodePrimitiveExpression(null),
                new CodeArgumentReferenceExpression(fieldsArgument.Name));
            method.Statements.Add(new CodeMethodReturnStatement(constraint.IsUnique ?
                new CodeMethodInvokeExpression(
                    returnValue,
                    IEnumerable_FirstOrDefault) :
                returnValue));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodMembersWithConnectionString(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodMembers(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodConnection(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            CodeMethodInvokeExpression returnValue = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(countArgument.Name) : new CodePrimitiveExpression(-1),
                this.CreateCondition(constraint),
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(ordersArgument.Name) : new CodePrimitiveExpression(null),
                new CodeArgumentReferenceExpression(fieldsArgument.Name));
            method.Statements.Add(new CodeMethodReturnStatement(constraint.IsUnique ?
                new CodeMethodInvokeExpression(
                    returnValue,
                    IEnumerable_FirstOrDefault) :
                returnValue));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodConnectionMembers(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodTransaction(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            CodeMethodInvokeExpression returnValue = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(countArgument.Name) : new CodePrimitiveExpression(-1),
                this.CreateCondition(constraint),
                !constraint.IsUnique ? (CodeExpression) new CodeArgumentReferenceExpression(ordersArgument.Name) : new CodePrimitiveExpression(null),
                new CodeArgumentReferenceExpression(fieldsArgument.Name));
            method.Statements.Add(new CodeMethodReturnStatement(constraint.IsUnique ?
                new CodeMethodInvokeExpression(
                    returnValue,
                    IEnumerable_FirstOrDefault) :
                returnValue));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodTransactionMembers(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = constraint.IsUnique ?
                new CodeTypeReference(typeParameter) :
                new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression countArgument = new CodeParameterDeclarationExpression(typeof(int), "count");
            if(!constraint.IsUnique) {
                method.Parameters.Add(countArgument);
            }
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            if(!constraint.IsUnique) {
                method.Parameters.Add(ordersArgument);
            }
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateGetSelectStatmentMethodPaginationGeneric(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getSelectStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page size is negative."),
                    new CodePrimitiveExpression(pageSizeArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(1)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page number is less than one."),
                    new CodePrimitiveExpression(pageNumberArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(ordersReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeMethodInvokeExpression(ordersReference, IEnumerable_Count),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Order fields is null or empty."),
                    new CodePrimitiveExpression(ordersArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(fieldsReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create sql statement
            method.Statements.Add(new CodeCommentStatement("construct SQL statement"));
            CodeVariableDeclarationStatement allStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "allStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression allStatementReference = new CodeVariableReferenceExpression(allStatementVariable.Name);
            method.Statements.Add(allStatementVariable);

            //assign Count
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_Count),
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.Multiply, pageNumberReference)));

            //assign query fields
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                fieldsReference));

            //assign data source
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(data))));

            //assign query condition
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                conditionReference));
            method.Statements.Add(new CodeSnippetStatement());

            //assign query orders
            CodeVariableDeclarationStatement orderExpressionIndexVariable = new CodeVariableDeclarationStatement(typeof(int), "orderExpressionIndex", new CodePrimitiveExpression(0));
            CodeVariableReferenceExpression orderExpressionIndexReference = new CodeVariableReferenceExpression(orderExpressionIndexVariable.Name);
            CodeVariableDeclarationStatement enumeratorVariable = new CodeVariableDeclarationStatement(
                new CodeTypeReference(typeof(IEnumerator<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))),
                "enumerator",
                new CodeMethodInvokeExpression(ordersReference, IEnumerable_GetEnumerator));
            CodeVariableReferenceExpression enumeratorReference = new CodeVariableReferenceExpression(enumeratorVariable.Name);
            method.Statements.Add(orderExpressionIndexVariable);
            method.Statements.Add(enumeratorVariable);

            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(1)),
                new CodeStatement[] {
                    new CodeIterationStatement(
                        new CodeSnippetStatement(),
                        new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                        new CodeSnippetStatement(),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_OrderClause),
                            ISqlOrderClause_AddExpression,
                            new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current)))),

                    new CodeMethodReturnStatement(allStatementReference),
                },
                new CodeStatement[] {
                    new CodeIterationStatement(
                        new CodeAssignStatement(orderExpressionIndexReference, new CodePrimitiveExpression(0)),
                        new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                        new CodeAssignStatement(orderExpressionIndexReference, new CodeBinaryOperatorExpression(orderExpressionIndexReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),

                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_OrderClause),
                            ISqlOrderClause_AddExpression,
                            new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current))),

                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_SelectClause),
                            ISqlSelectClause_AddExpressions,
                            this.m_dbInfoProvider.GetSqlObject(
                                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current), SqlExpressionOrder_Expression),
                                this.GetOrderFieldAlias(orderExpressionIndexReference))))),
                }));
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement allSourceVariable = new CodeVariableDeclarationStatement(typeof(ISqlObject), "allSource", this.m_dbInfoProvider.GetSubquerySource(allStatementReference, new CodePrimitiveExpression("AllData")));
            CodeVariableReferenceExpression allSourceReference = new CodeVariableReferenceExpression(allSourceVariable.Name);
            method.Statements.Add(new CodeCommentStatement("construct pagination SQL statement"));

            method.Statements.Add(allSourceVariable);
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement middleStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "middleStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression middleStatementReference = new CodeVariableReferenceExpression(middleStatementVariable.Name);
            method.Statements.Add(middleStatementVariable);
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_Count),
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(Math)),
                    Math_Min,
                    pageSizeReference,
                    new CodeBinaryOperatorExpression(
                        totalCountReference,
                        CodeBinaryOperatorType.Subtract,
                        new CodeBinaryOperatorExpression(
                            pageSizeReference,
                            CodeBinaryOperatorType.Multiply,
                            new CodeBinaryOperatorExpression(
                                pageNumberReference,
                                CodeBinaryOperatorType.Subtract,
                                new CodePrimitiveExpression(1)))))));
            method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                new CodeObjectCreateExpression(typeof(SqlAllField), allSourceReference))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                allSourceReference));
            method.Statements.Add(new CodeSnippetStatement());

            method.Statements.Add(new CodeAssignStatement(enumeratorReference, new CodeMethodInvokeExpression(ordersReference, IEnumerable_GetEnumerator)));
            method.Statements.Add(new CodeIterationStatement(
                    new CodeAssignStatement(orderExpressionIndexReference, new CodePrimitiveExpression(0)),
                    new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                    new CodeAssignStatement(orderExpressionIndexReference, new CodeBinaryOperatorExpression(orderExpressionIndexReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),

                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current),
                                SqlExpressionOrder_Order),
                            CodeBinaryOperatorType.IdentityEquality,
                            new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(SqlOrder)),
                                Enum.GetName(typeof(SqlOrder), SqlOrder.Asc))),

                        new CodeStatement[] {
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_OrderClause),
                                ISqlOrderClause_AddExpression,
                                this.m_dbInfoProvider.GetSqlField(allSourceReference, this.GetOrderFieldAlias(orderExpressionIndexReference)),
                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOrder)), Enum.GetName(typeof(SqlOrder), SqlOrder.Desc))))
                        },
                        new CodeStatement[] {
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_OrderClause),
                                ISqlOrderClause_AddExpression,
                                this.m_dbInfoProvider.GetSqlField(allSourceReference, this.GetOrderFieldAlias(orderExpressionIndexReference)),
                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOrder)), Enum.GetName(typeof(SqlOrder), SqlOrder.Asc)))) 
                        })));
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement middleSourceVariable = new CodeVariableDeclarationStatement(typeof(ISqlObject), "middleSource", this.m_dbInfoProvider.GetSubquerySource(middleStatementReference, new CodePrimitiveExpression("MiddleData")));
            CodeVariableReferenceExpression middleSourceReference = new CodeVariableReferenceExpression(middleSourceVariable.Name);
            method.Statements.Add(middleSourceVariable);
            method.Statements.Add(new CodeSnippetStatement());

            method.Statements.Add(new CodeCommentStatement("construct final SQL statement"));
            CodeVariableDeclarationStatement finalStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "finalStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression finalStatementReference = new CodeVariableReferenceExpression(finalStatementVariable.Name);
            method.Statements.Add(finalStatementVariable);

            CodeVariableDeclarationStatement iterationVariable = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));
            CodeVariableReferenceExpression iterationReference = new CodeVariableReferenceExpression(iterationVariable.Name);
            method.Statements.Add(new CodeIterationStatement(
                iterationVariable,
                new CodeBinaryOperatorExpression(iterationReference, CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(fieldsReference, Array_Length)),
                new CodeAssignStatement(iterationReference, new CodeBinaryOperatorExpression(iterationReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),

                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_SelectClause),
                    ISqlSelectClause_AddExpressions,
                    this.m_dbInfoProvider.GetSqlField(
                        middleSourceReference,
                        new CodePropertyReferenceExpression(
                            new CodeIndexerExpression(fieldsReference, iterationReference),
                            ISqlObject_Name))))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                middleSourceReference));
            method.Statements.Add(new CodeSnippetStatement());

            method.Statements.Add(new CodeAssignStatement(
                enumeratorReference,
                new CodeMethodInvokeExpression(ordersReference, IEnumerable_GetEnumerator)));
            method.Statements.Add(new CodeIterationStatement(
                new CodeAssignStatement(orderExpressionIndexReference, new CodePrimitiveExpression(0)),
                        new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                        new CodeAssignStatement(orderExpressionIndexReference, new CodeBinaryOperatorExpression(orderExpressionIndexReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),

                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_OrderClause),
                    ISqlOrderClause_AddExpression,
                    this.m_dbInfoProvider.GetSqlField(
                        middleSourceReference,
                        this.GetOrderFieldAlias(orderExpressionIndexReference)),
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current),
                        SqlExpressionOrder_Order)))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(finalStatementReference));

            return method;
        }

        private CodeMemberMethod CreateGetSelectStatmentMethodPaginationGenericMembers(IDbDataEntity data, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getSelectStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_getSelectStatementMethodName,
                totalCountReference, pageSizeReference, pageNumberReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateGetSelectStatmentMethodPaginationOnePrimaryKey(IDbFieldEntity primaryKey) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getSelectStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page size is negative."),
                    new CodePrimitiveExpression(pageSizeArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(1)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page number is less than one."),
                    new CodePrimitiveExpression(pageNumberArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(ordersReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeMethodInvokeExpression(ordersReference, IEnumerable_Count),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Order fields is null or empty."),
                    new CodePrimitiveExpression(ordersArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(fieldsReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            #region construct all source SQL statement

            method.Statements.Add(new CodeCommentStatement("construct all source SQL statement"));
            CodeVariableDeclarationStatement allStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "allStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression allStatementReference = new CodeVariableReferenceExpression(allStatementVariable.Name);
            method.Statements.Add(allStatementVariable);

            // assign count
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_Count),
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.Multiply, pageNumberReference)));

            // assign query fields
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(1)),
                new CodeStatement[] {
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_SelectClause),
                        ISqlSelectClause_AddExpressions,
                        fieldsReference)),
                },
                new CodeStatement[] {
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_SelectClause),
                        ISqlSelectClause_AddExpressions,
                        new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), primaryKey.Name))),
                }));

            //assign data source
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(primaryKey.Data))));

            // assign query condition
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                conditionReference));
            method.Statements.Add(new CodeSnippetStatement());

            // assign query orders
            CodeVariableDeclarationStatement orderExpressionIndexVariable = new CodeVariableDeclarationStatement(typeof(int), "orderExpressionIndex", new CodePrimitiveExpression(0));
            CodeVariableReferenceExpression orderExpressionIndexReference = new CodeVariableReferenceExpression(orderExpressionIndexVariable.Name);
            CodeVariableDeclarationStatement enumeratorVariable = new CodeVariableDeclarationStatement(
                new CodeTypeReference(typeof(IEnumerator<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))),
                "enumerator",
                new CodeMethodInvokeExpression(ordersReference, IEnumerable_GetEnumerator));
            CodeVariableReferenceExpression enumeratorReference = new CodeVariableReferenceExpression(enumeratorVariable.Name);
            method.Statements.Add(orderExpressionIndexVariable);
            method.Statements.Add(enumeratorVariable);

            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(1)),
                new CodeStatement[] {
                    new CodeIterationStatement(
                        new CodeSnippetStatement(),
                        new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                        new CodeSnippetStatement(),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_OrderClause),
                            ISqlOrderClause_AddExpression,
                            new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current)))),

                    new CodeMethodReturnStatement(allStatementReference),
                },
                new CodeStatement[] {
                    new CodeIterationStatement(
                        new CodeAssignStatement(orderExpressionIndexReference, new CodePrimitiveExpression(0)),
                        new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                        new CodeAssignStatement(orderExpressionIndexReference, new CodeBinaryOperatorExpression(orderExpressionIndexReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),

                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_OrderClause),
                            ISqlOrderClause_AddExpression,
                            new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current))),

                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodePropertyReferenceExpression(allStatementReference, ISqlSelectStatement_SelectClause),
                            ISqlSelectClause_AddExpressions,
                            this.m_dbInfoProvider.GetSqlObject(
                                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current), SqlExpressionOrder_Expression),
                                this.GetOrderFieldAlias(orderExpressionIndexReference))))),
                }));
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement allSourceVariable = new CodeVariableDeclarationStatement(typeof(ISqlObject), "allSource", this.m_dbInfoProvider.GetSubquerySource(allStatementReference, new CodePrimitiveExpression("AllData")));
            CodeVariableReferenceExpression allSourceReference = new CodeVariableReferenceExpression(allSourceVariable.Name);
            method.Statements.Add(allSourceVariable);
            method.Statements.Add(new CodeSnippetStatement());

            #endregion

            #region construct middle source SQL statement

            method.Statements.Add(new CodeCommentStatement("construct middle source SQL statement"));
            CodeVariableDeclarationStatement middleStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "middleStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression middleStatementReference = new CodeVariableReferenceExpression(middleStatementVariable.Name);
            method.Statements.Add(middleStatementVariable);

            // assign count
            method.Statements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_Count),
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(Math)),
                        Math_Min,
                        pageSizeReference,
                        new CodeBinaryOperatorExpression(
                            totalCountReference,
                            CodeBinaryOperatorType.Subtract,
                            new CodeBinaryOperatorExpression(
                                pageSizeReference,
                                CodeBinaryOperatorType.Multiply,
                                new CodeBinaryOperatorExpression(
                                    pageNumberReference,
                                    CodeBinaryOperatorType.Subtract,
                                    new CodePrimitiveExpression(1)))))));

            // assign query fields
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                this.m_dbInfoProvider.GetSqlField(
                    allSourceReference,
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), primaryKey.Name),
                        ISqlObject_Name))));

            // assign data source
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                allSourceReference));
            method.Statements.Add(new CodeSnippetStatement());

            // assign query orders
            method.Statements.Add(new CodeAssignStatement(enumeratorReference, new CodeMethodInvokeExpression(ordersReference, IEnumerable_GetEnumerator)));
            method.Statements.Add(new CodeIterationStatement(
                    new CodeAssignStatement(orderExpressionIndexReference, new CodePrimitiveExpression(0)),
                    new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                    new CodeAssignStatement(orderExpressionIndexReference, new CodeBinaryOperatorExpression(orderExpressionIndexReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),

                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current),
                                SqlExpressionOrder_Order),
                            CodeBinaryOperatorType.IdentityEquality,
                            new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(SqlOrder)),
                                Enum.GetName(typeof(SqlOrder), SqlOrder.Asc))),

                        new CodeStatement[] {
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_OrderClause),
                                ISqlOrderClause_AddExpression,
                                this.m_dbInfoProvider.GetSqlField(allSourceReference, this.GetOrderFieldAlias(orderExpressionIndexReference)),
                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOrder)), Enum.GetName(typeof(SqlOrder), SqlOrder.Desc))))
                        },
                        new CodeStatement[] {
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(
                                new CodePropertyReferenceExpression(middleStatementReference, ISqlSelectStatement_OrderClause),
                                ISqlOrderClause_AddExpression,
                                this.m_dbInfoProvider.GetSqlField(allSourceReference, this.GetOrderFieldAlias(orderExpressionIndexReference)),
                                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOrder)), Enum.GetName(typeof(SqlOrder), SqlOrder.Asc)))) 
                        })));
            method.Statements.Add(new CodeSnippetStatement());

            #endregion

            #region construct final SQL statement

            method.Statements.Add(new CodeCommentStatement("construct final SQL statement"));
            CodeVariableDeclarationStatement finalStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "finalStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression finalStatementReference = new CodeVariableReferenceExpression(finalStatementVariable.Name);
            method.Statements.Add(finalStatementVariable);


            // assign query fields
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                fieldsReference));

            //assign data source
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(primaryKey.Data))));

            // assign query condition
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), primaryKey.Name),
                    ISqlExpression_In,
                    middleStatementReference)));

            // assign query orders
            method.Statements.Add(new CodeAssignStatement(enumeratorReference, new CodeMethodInvokeExpression(ordersReference, IEnumerable_GetEnumerator)));
            method.Statements.Add(new CodeIterationStatement(
                new CodeSnippetStatement(),
                new CodeMethodInvokeExpression(enumeratorReference, IEnumerator_MoveNext),
                new CodeSnippetStatement(),
                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(finalStatementReference, ISqlSelectStatement_OrderClause),
                    ISqlOrderClause_AddExpression,
                    new CodePropertyReferenceExpression(enumeratorReference, IEnumerator_Current)))));
            method.Statements.Add(new CodeSnippetStatement());

            #endregion

            //return
            method.Statements.Add(new CodeMethodReturnStatement(finalStatementReference));

            return method;
        }

        private CodeMemberMethod CreateGetSelectStatmentMethodPaginationOnePrimaryKeyMembers(IDbFieldEntity primaryKey, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getSelectStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(primaryKey.Data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_getSelectStatementMethodName,
                totalCountReference, pageSizeReference, pageNumberReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationWithConnectionString(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page size is negative."),
                    new CodePrimitiveExpression(pageSizeArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(1)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page number is less than one."),
                    new CodePrimitiveExpression(pageNumberArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(ordersReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeMethodInvokeExpression(ordersReference, IEnumerable_Count),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Order fields is null or empty."),
                    new CodePrimitiveExpression(ordersArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(fieldsReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(
                            pageSizeReference,
                            CodeBinaryOperatorType.Multiply,
                            new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        totalCountReference)),
                new CodeMethodReturnStatement(new CodeObjectCreateExpression(
                    new CodeTypeReference(typeof(List<>).FullName, new CodeTypeReference(typeParameter)),
                    new CodePrimitiveExpression(0)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_loadMethodName,
                        connectionReference,
                        totalCountReference,
                        pageSizeReference,
                        pageNumberReference,
                        conditionReference,
                        ordersReference,
                        fieldsReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPagination(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getReadConnectionStringMethodName),
                totalCountReference,
                pageSizeReference,
                pageNumberReference,
                conditionReference,
                ordersReference,
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationMembersWithConnectionString(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                stringReference, totalCountReference, pageSizeReference, pageNumberReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationMembers(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                totalCountReference, pageSizeReference, pageNumberReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationConnection(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            CodeArgumentReferenceExpression connectionReference = new CodeArgumentReferenceExpression(connectionArgument.Name);
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page size is negative."),
                    new CodePrimitiveExpression(pageSizeArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(1)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page number is less than one."),
                    new CodePrimitiveExpression(pageNumberArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(ordersReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeMethodInvokeExpression(ordersReference, IEnumerable_Count),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Order fields is null or empty."),
                    new CodePrimitiveExpression(ordersArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(fieldsReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(
                            pageSizeReference,
                            CodeBinaryOperatorType.Multiply,
                            new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        totalCountReference)),
                new CodeMethodReturnStatement(new CodeObjectCreateExpression(
                    new CodeTypeReference(typeof(List<>).FullName, new CodeTypeReference(typeParameter)),
                    new CodePrimitiveExpression(0)))));
            method.Statements.Add(new CodeSnippetStatement());

            //execute sql statement
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadFromReaderMethodName,
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlHelper)),
                    SqlHelper_ExecuteSelect,
                    connectionReference,
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getSelectStatementMethodName,
                        totalCountReference,
                        pageSizeReference,
                        pageNumberReference,
                        conditionReference,
                        ordersReference,
                        fieldsReference)),
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationConnectionMembers(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            CodeArgumentReferenceExpression connectionReference = new CodeArgumentReferenceExpression(connectionArgument.Name);
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                connectionReference, totalCountReference, pageSizeReference, pageNumberReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationTransaction(IDbDataEntity data, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            CodeArgumentReferenceExpression transactionReference = new CodeArgumentReferenceExpression(transactionArgument.Name);
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page size is negative."),
                    new CodePrimitiveExpression(pageSizeArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(1)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("The page number is less than one."),
                    new CodePrimitiveExpression(pageNumberArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(ordersReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeMethodInvokeExpression(ordersReference, IEnumerable_Count),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Order fields is null or empty."),
                    new CodePrimitiveExpression(ordersArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(fieldsReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Fields is null or empty."),
                    new CodePrimitiveExpression(fieldsArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(pageSizeReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(
                            pageSizeReference,
                            CodeBinaryOperatorType.Multiply,
                            new CodeBinaryOperatorExpression(pageNumberReference, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        totalCountReference)),
                new CodeMethodReturnStatement(new CodeObjectCreateExpression(
                    new CodeTypeReference(typeof(List<>).FullName, new CodeTypeReference(typeParameter)),
                    new CodePrimitiveExpression(0)))));
            method.Statements.Add(new CodeSnippetStatement());

            //execute sql statement
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadFromReaderMethodName,
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlHelper)),
                    SqlHelper_ExecuteSelect,
                    transactionReference,
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getSelectStatementMethodName,
                        totalCountReference,
                        pageSizeReference,
                        pageNumberReference,
                        conditionReference,
                        ordersReference,
                        fieldsReference)),
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationTransactionMembers(IDbDataEntity data, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.m_loadMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            CodeArgumentReferenceExpression transactionReference = new CodeArgumentReferenceExpression(transactionArgument.Name);
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            CodeArgumentReferenceExpression totalCountReference = new CodeArgumentReferenceExpression(totalCountArgument.Name);
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            CodeArgumentReferenceExpression pageSizeReference = new CodeArgumentReferenceExpression(pageSizeArgument.Name);
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            CodeArgumentReferenceExpression pageNumberReference = new CodeArgumentReferenceExpression(pageNumberArgument.Name);
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            CodeArgumentReferenceExpression ordersReference = new CodeArgumentReferenceExpression(ordersArgument.Name);
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(data))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), this.m_loadMethodName,
                transactionReference, totalCountReference, pageSizeReference, pageNumberReference, conditionReference, ordersReference,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationWithConnectionString(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                stringReference,
                new CodeArgumentReferenceExpression(totalCountArgument.Name),
                new CodeArgumentReferenceExpression(pageSizeArgument.Name),
                new CodeArgumentReferenceExpression(pageNumberArgument.Name),
                this.CreateCondition(constraint),
                new CodeArgumentReferenceExpression(ordersArgument.Name),
                new CodeArgumentReferenceExpression(fieldsArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPagination(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                new CodeArgumentReferenceExpression(totalCountArgument.Name),
                new CodeArgumentReferenceExpression(pageSizeArgument.Name),
                new CodeArgumentReferenceExpression(pageNumberArgument.Name),
                this.CreateCondition(constraint),
                new CodeArgumentReferenceExpression(ordersArgument.Name),
                new CodeArgumentReferenceExpression(fieldsArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationMembersWithConnectionString(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationMembers(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationConnection(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                new CodeArgumentReferenceExpression(totalCountArgument.Name),
                new CodeArgumentReferenceExpression(pageSizeArgument.Name),
                new CodeArgumentReferenceExpression(pageNumberArgument.Name),
                this.CreateCondition(constraint),
                new CodeArgumentReferenceExpression(ordersArgument.Name),
                new CodeArgumentReferenceExpression(fieldsArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationConnectionMembers(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationTransaction(IDbConstraintEntity constraint, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "fields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            method.Parameters.Add(fieldsArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                new CodeArgumentReferenceExpression(totalCountArgument.Name),
                new CodeArgumentReferenceExpression(pageSizeArgument.Name),
                new CodeArgumentReferenceExpression(pageNumberArgument.Name),
                this.CreateCondition(constraint),
                new CodeArgumentReferenceExpression(ordersArgument.Name),
                new CodeArgumentReferenceExpression(fieldsArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateLoadMethodPaginationTransactionMembers(IDbConstraintEntity constraint, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeParameter));
            method.Name = this.GetMethodNameByConstraint(this.m_loadMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load pagination data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression totalCountArgument = new CodeParameterDeclarationExpression(typeof(int), "totalCount");
            method.Parameters.Add(totalCountArgument);
            CodeParameterDeclarationExpression pageSizeArgument = new CodeParameterDeclarationExpression(typeof(int), "pageSize");
            method.Parameters.Add(pageSizeArgument);
            CodeParameterDeclarationExpression pageNumberArgument = new CodeParameterDeclarationExpression(typeof(int), "pageNumber");
            method.Parameters.Add(pageNumberArgument);
            CodeParameterDeclarationExpression ordersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference(typeof(SqlExpressionOrder))), "orders");
            method.Parameters.Add(ordersArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(constraint.Table))), "members");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            ICollection<CodeExpression> parameters = new List<CodeExpression>(method.Parameters.Count - 1);
            foreach(CodeParameterDeclarationExpression item in method.Parameters) {
                if(item != membersArgument) {
                    parameters.Add(new CodeArgumentReferenceExpression(item.Name));
                }
            }
            parameters.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.GetMethodNameByConstraint(this.m_loadMethodName, constraint),
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateGetCountStatementMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getCountStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create sql statement
            method.Statements.Add(new CodeCommentStatement("construct SQL statement"));
            CodeVariableDeclarationStatement selectStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "selectStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression selectStatementReference = new CodeVariableReferenceExpression(selectStatementVariable.Name);
            method.Statements.Add(selectStatementVariable);
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(
                    selectStatementReference,
                    ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                this.m_dbInfoProvider.GetSqlObject(
                    new CodeMethodInvokeExpression(
                        new CodeObjectCreateExpression(typeof(SqlFunction), new CodePrimitiveExpression("COUNT")),
                        ISqlFunction_AddArgument,
                        new CodeObjectCreateExpression(typeof(SqlAllField), new CodePrimitiveExpression(null))),
                    new CodePrimitiveExpression("RecordCount"))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        selectStatementReference,
                        ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(data))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        selectStatementReference,
                        ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                new CodeArgumentReferenceExpression(conditionArgument.Name)));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(selectStatementReference));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethodWithConnectionString(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_loadCountMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_loadCountMethodName,
                        connectionReference,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_loadCountMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getReadConnectionStringMethodName),
                new CodeArgumentReferenceExpression(conditionArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethodConnection(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_loadCountMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //execute sql statement and return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(Convert)),
                Convert_ToInt32,
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteScalar,
                        new CodeTypeReference(typeof(object))),
                    new CodeArgumentReferenceExpression(connectionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getCountStatementMethodName,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethodTransaction(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_loadCountMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //execute sql statement and return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(Convert)),
                Convert_ToInt32,
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteScalar,
                        new CodeTypeReference(typeof(object))),
                    new CodeArgumentReferenceExpression(transactionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getCountStatementMethodName,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethodWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load number of data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                stringReference,
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethod(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load number of data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethodConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load number of data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountMethodTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load number of data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountExcludePrimaryKeyMethodWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>The number of {0}.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                stringReference,
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountExcludePrimaryKeyMethod(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>The number of {0}.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountExcludePrimaryKeyMethodConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>The number of {0}.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateLoadCountExcludePrimaryKeyMethodTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_loadCountMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Load data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>The number of {0}.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_loadCountMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateGetExistsStatementMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlSelectStatement));
            method.Name = this.m_getExistsStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create sql statement
            method.Statements.Add(new CodeCommentStatement("construct SQL statement"));
            CodeVariableDeclarationStatement oneVariable = new CodeVariableDeclarationStatement(typeof(ISqlExpression), "one", new CodeObjectCreateExpression(typeof(SqlStringExpression), new CodePrimitiveExpression("1")));
            CodeVariableReferenceExpression oneReference = new CodeVariableReferenceExpression(oneVariable.Name);
            method.Statements.Add(oneVariable);
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement subStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "subStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression subStatementReference = new CodeVariableReferenceExpression(subStatementVariable.Name);
            method.Statements.Add(subStatementVariable);
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(
                    subStatementReference,
                    ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                oneReference));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        subStatementReference,
                        ISqlSelectStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(data))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        subStatementReference,
                        ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                new CodeArgumentReferenceExpression(conditionArgument.Name)));
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement mainStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), "mainStatement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType()));
            CodeVariableReferenceExpression mainStatementReference = new CodeVariableReferenceExpression(mainStatementVariable.Name);
            method.Statements.Add(mainStatementVariable);
            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodePropertyReferenceExpression(
                    mainStatementReference,
                    ISqlSelectStatement_SelectClause),
                ISqlSelectClause_AddExpressions,
                oneReference));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        mainStatementReference,
                        ISqlSelectStatement_WhereClause),
                    ISqlWhereClause_Condition),
                new CodeMethodInvokeExpression(
                    new CodeObjectCreateExpression(typeof(SqlFunction), new CodePrimitiveExpression("EXISTS")),
                    ISqlFunction_AddArgument,
                    subStatementReference)));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(mainStatementReference));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethodWithConnectionString(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.m_isExistsMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_isExistsMethodName,
                        connectionReference,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.m_isExistsMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getReadConnectionStringMethodName),
                new CodeArgumentReferenceExpression(conditionArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethodConnection(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.m_isExistsMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //execute sql statement and return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteScalar,
                        new CodeTypeReference(typeof(object))),
                    new CodeArgumentReferenceExpression(connectionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getExistsStatementMethodName,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))),
                CodeBinaryOperatorType.IdentityInequality,
                new CodePrimitiveExpression(null))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethodTransaction(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.m_isExistsMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //execute sql statement and return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteScalar,
                        new CodeTypeReference(typeof(object))),
                    new CodeArgumentReferenceExpression(transactionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getExistsStatementMethodName,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))),
                CodeBinaryOperatorType.IdentityInequality,
                new CodePrimitiveExpression(null))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethodWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                stringReference,
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethod(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethodConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsMethodTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsExcludePrimaryKeyMethodWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>Whether existing.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                stringReference,
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsExcludePrimaryKeyMethod(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>Whether existing.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsExcludePrimaryKeyMethodConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>Whether existing.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateIsExistsExcludePrimaryKeyMethodTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Name = this.GetMethodNameByConstraint(this.m_isExistsMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Data existing detected by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<returns>Whether existing.</returns>", this.GetEntityTypeName(constraint.Table)), true));

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Parameters.AddRange(this.CreateArguments(constraint.Table.PrimaryKey).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_isExistsMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                new CodeMethodInvokeExpression(
                    this.CreateCondition(constraint),
                    ISqlExpression_And,
                    new CodeMethodInvokeExpression(
                        this.CreateCondition(constraint.Table.PrimaryKey),
                        ISqlExpression_Not)))));

            return method;
        }

        private CodeMemberMethod CreateGetParametersMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<IDataParameter>));
            method.Name = this.m_getParametersMethodName;

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method statements
            ICollection<CodeExpression> elements = new List<CodeExpression>(table.TableFields.Count());
            foreach(IDbFieldEntity field in table.Fields) {
                elements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.GetCreateParameterMethodName(field),
                    new CodePrimitiveExpression(true),
                    new CodePropertyReferenceExpression(infoReference, field.Name)));
                //elements.Add(this.m_dbInfoProvider.GetDbParameter(
                //    new CodePrimitiveExpression(this.m_dbInfoProvider.GetDbParameterName(field)), 
                //    field,
                //    (field.IsNullable && field.Type.IsValueType) ?
                //        (CodeExpression) new CodeSnippetExpression(string.Format("{0}.{1}.{2} ? (object){0}.{1}.{3} : (object)null",
                //            infoReference.ParameterName,
                //            field.Name,
                //            Nullable_HasValue,
                //            Nullable_Value)) :
                //        (CodeExpression) new CodePropertyReferenceExpression(infoReference, field.Name)));
            }
            method.Statements.Add(new CodeMethodReturnStatement(new CodeArrayCreateExpression(new CodeTypeReference(typeof(IDataParameter)), elements.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateGetInsertStatementWithNameMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(typeof(ISqlInsertStatement));
            method.Name = this.m_getInsertStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression enableArgument = new CodeParameterDeclarationExpression(typeof(bool), "enableIdentityInsert");
            CodeArgumentReferenceExpression enableReference = new CodeArgumentReferenceExpression(enableArgument.Name);
            method.Parameters.Add(enableArgument);
            CodeParameterDeclarationExpression useStableParameterNameArgument = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName);
            CodeArgumentReferenceExpression useStableParameterNameReference = new CodeArgumentReferenceExpression(useStableParameterNameArgument.Name);
            method.Parameters.Add(useStableParameterNameArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create INSERT statements
            CodeExpression identityFieldValue = null;
            CodeVariableDeclarationStatement insertStatementVariable = new CodeVariableDeclarationStatement(typeof(ISqlInsertStatement), "statement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlInsertStatementType()));
            method.Statements.Add(insertStatementVariable);
            CodePropertyReferenceExpression fieldValueClause = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(insertStatementVariable.Name), ISqlInsertStatement_FieldValueClause);
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(fieldValueClause, ISqlFieldValueClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(table))));
            foreach(IDbTableFieldEntity field in table.TableFields) {
                if(!field.IsIdentity && field.IsReadOnly) {
                    continue;
                }

                if(field.IsIdentity || field.IsNullable) {
                    identityFieldValue = field.IsIdentity ? this.m_dbInfoProvider.GetIdentityFieldValue(infoReference, field) : null;

                    method.Statements.Add(new CodeConditionStatement(
                        field.IsIdentity ?
                            enableReference :
                            field.Type.IsValueType ?
                                (CodeExpression) new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(infoReference, field.Name), Nullable_HasValue) :
                                field.Type.IsString() ?
                                    (CodeExpression) new CodeSnippetExpression(string.Format("!string.{0}({1}.{2})", String_IsNullOrWhiteSpace, infoArgument.Name, field.Name)) :
                                    (CodeExpression) new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(infoReference, field.Name), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                        new CodeStatement[] {
                            new CodeExpressionStatement(new CodeMethodInvokeExpression(
                                fieldValueClause,
                                ISqlFieldValueClause_AddField,
                                new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)),
                                new CodeMethodInvokeExpression(
                                    new CodeThisReferenceExpression(),
                                    this.GetCreateParameterExpressionMethodName(field),
                                    useStableParameterNameReference,
                                    field.IsNullable && field.Type.IsValueType ?
                                        new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(infoReference, field.Name), Nullable_Value) :
                                        new CodePropertyReferenceExpression(infoReference, field.Name)))),
                        },
                        identityFieldValue == null ? new CodeStatement[0] : new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            fieldValueClause,
                            ISqlFieldValueClause_AddField,
                            new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)),
                            identityFieldValue)) }));
                } else {
                    method.Statements.Add(new CodeMethodInvokeExpression(
                        fieldValueClause,
                        ISqlFieldValueClause_AddField,
                        new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)),
                        new CodeMethodInvokeExpression(
                            new CodeThisReferenceExpression(),
                            this.GetCreateParameterExpressionMethodName(field),
                            useStableParameterNameReference,
                            new CodePropertyReferenceExpression(infoReference, field.Name))));
                }
            }
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(insertStatementVariable.Name)));

            return method;
        }

        private CodeMemberMethod CreateGetInsertStatementMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlInsertStatement));
            method.Name = this.m_getInsertStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression enableArgument = new CodeParameterDeclarationExpression(typeof(bool), "enableIdentityInsert");
            CodeArgumentReferenceExpression enableReference = new CodeArgumentReferenceExpression(enableArgument.Name);
            method.Parameters.Add(enableArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getInsertStatementMethodName,
                infoReference, 
                enableReference, 
                new CodePrimitiveExpression(false))));

            return method;
        }

        private CodeMemberMethod CreateGetLastIdentityMethodWithConnectionString(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_getLastIdentityMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            if(!this.m_dbInfoProvider.IsSupportGetLastIdentity) {
                method.Statements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(NotSupportedException))));
                return method;
            }

            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getLastIdentityMethodName,
                        connectionReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateGetLastIdentityMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_getLastIdentityMethodName;

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            if(!this.m_dbInfoProvider.IsSupportGetLastIdentity) {
                method.Statements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(NotSupportedException))));
                return method;
            }

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getLastIdentityMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getReadConnectionStringMethodName))));

            return method;
        }

        private CodeMemberMethod CreateGetLastIdentityMethodConnection(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_getLastIdentityMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            if(!this.m_dbInfoProvider.IsSupportGetLastIdentity) {
                method.Statements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(NotSupportedException))));
                return method;
            }

            //execute sql statement and return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(Convert)),
                Convert_ToInt32,
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteScalar,
                        new CodeTypeReference(typeof(object))),
                    new CodeArgumentReferenceExpression(connectionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    this.m_dbInfoProvider.GetIdentityStatement(new CodePrimitiveExpression(data.Name))))));

            return method;
        }

        private CodeMemberMethod CreateGetLastIdentityMethodTransaction(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_getLastIdentityMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            if(!this.m_dbInfoProvider.IsSupportGetLastIdentity) {
                method.Statements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(NotSupportedException))));
                return method;
            }

            //execute sql statement and return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(Convert)),
                Convert_ToInt32,
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteScalar,
                        new CodeTypeReference(typeof(object))),
                    new CodeArgumentReferenceExpression(transactionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    this.m_dbInfoProvider.GetIdentityStatement(new CodePrimitiveExpression(data.Name))))));

            return method;
        }

        private CodeMemberMethod CreateInsertMethodWithConnectionString(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements 
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));     
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_insertMethodName,
                        connectionReference,
                        infoReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateInsertMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_insertMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                infoReference));

            return method;
        }

        private CodeMemberMethod CreateInsertMethodConnection(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //find identity field
            IDbTableFieldEntity identityField = (from field in table.TableFields
                                                 where field.IsIdentity
                                                 select field).FirstOrDefault();

            //execute SQL statement
            if(identityField != null) {
                method.Statements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(infoReference, identityField.Name),
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteInsert,
                        new CodeArgumentReferenceExpression(connectionArgument.Name),
                        this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                        new CodeMethodInvokeExpression(
                            new CodeThisReferenceExpression(),
                            this.m_getInsertStatementMethodName,
                            infoReference,
                            new CodePrimitiveExpression(false),
                            new CodePrimitiveExpression(true)),
                       this.m_dbInfoProvider.GetIdentityStatement())));
            } else {
                method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlHelper)),
                    SqlHelper_ExecuteInsert,
                    new CodeArgumentReferenceExpression(connectionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getInsertStatementMethodName,
                        infoReference,
                        new CodePrimitiveExpression(false),
                        new CodePrimitiveExpression(true)))));
            }

            return method;
        }

        private CodeMemberMethod CreateInsertMethodTransaction(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //find identity field
            IDbTableFieldEntity identityField = (from field in table.TableFields
                                                 where field.IsIdentity
                                                 select field).FirstOrDefault();

            //execute SQL statement
            if(identityField != null) {
                method.Statements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(infoReference, identityField.Name),
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteInsert,
                        new CodeArgumentReferenceExpression(transactionArgument.Name),
                        this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                        new CodeMethodInvokeExpression(
                            new CodeThisReferenceExpression(),
                            this.m_getInsertStatementMethodName,
                            infoReference,
                            new CodePrimitiveExpression(false),
                            new CodePrimitiveExpression(true)),
                       this.m_dbInfoProvider.GetIdentityStatement())));
            } else {
                method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SqlHelper)),
                    SqlHelper_ExecuteInsert,
                    new CodeArgumentReferenceExpression(transactionArgument.Name),
                    this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getInsertStatementMethodName,
                        infoReference,
                        new CodePrimitiveExpression(false),
                        new CodePrimitiveExpression(true)))));
            }

            return method;
        }

        private void CreateInsertMultipleMethodCore(CodeMemberMethod method, IDbTableEntity table, CodeTypeParameter typeParameter, CodeArgumentReferenceExpression dataReference, CodeArgumentReferenceExpression enableReference, CodeArgumentReferenceExpression dbReference) {
            CodeVariableDeclarationStatement dataEnumeratorVariable = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(IEnumerator<>).AssemblyQualifiedName, new CodeTypeReference(typeParameter)), "dataEnumerator", new CodeMethodInvokeExpression(dataReference, IEnumerable_GetEnumerator));
            CodeVariableReferenceExpression dataEnumeratorReference = new CodeVariableReferenceExpression(dataEnumeratorVariable.Name);
            CodeVariableDeclarationStatement fragmentsEnumeratorVariable = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(IEnumerator<IEnumerable<ISqlStatement>>)), "fragmentsEnumerator", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression fragmentsEnumeratorReference = new CodeVariableReferenceExpression(fragmentsEnumeratorVariable.Name);
            CodeVariableDeclarationStatement statementsVariable = new CodeVariableDeclarationStatement(typeof(ICollection<ISqlStatement>), "statements", new CodeObjectCreateExpression(typeof(List<ISqlStatement>)));
            CodeVariableReferenceExpression statementsReference = new CodeVariableReferenceExpression(statementsVariable.Name);
            method.Statements.Add(dataEnumeratorVariable);
            method.Statements.Add(fragmentsEnumeratorVariable);
            method.Statements.Add(statementsVariable);
            method.Statements.Add(new CodeSnippetStatement());

            method.Statements.Add(new CodeIterationStatement(
                new CodeSnippetStatement(),
                new CodeMethodInvokeExpression(dataEnumeratorReference, IEnumerator_MoveNext),
                new CodeSnippetStatement(),
                new CodeExpressionStatement(new CodeMethodInvokeExpression(
                    statementsReference,
                    ICollection_Add,
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getInsertStatementMethodName,
                        new CodePropertyReferenceExpression(dataEnumeratorReference, IEnumerator_Current),
                        enableReference,
                        new CodePrimitiveExpression(false))))));
            method.Statements.Add(new CodeSnippetStatement());

            CodeStatement insertStatement = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlHelper)), SqlHelper_MaxNoQueryStatementsLimit), CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0)),
                    CodeBinaryOperatorType.BooleanAnd,
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(statementsReference, ICollection_Count), CodeBinaryOperatorType.GreaterThan, new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlHelper)), SqlHelper_MaxNoQueryStatementsLimit))),
                new CodeStatement[] {
                    new CodeAssignStatement(
                        fragmentsEnumeratorReference,
                        new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(
                            statementsReference,
                            IEnumerable_Split,
                            new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlHelper)), SqlHelper_MaxNoQueryStatementsLimit)), IEnumerable_GetEnumerator)),
                    new CodeIterationStatement(
                        new CodeSnippetStatement(),
                        new CodeMethodInvokeExpression(fragmentsEnumeratorReference, IEnumerator_MoveNext),
                        new CodeSnippetStatement(),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(typeof(SqlHelper)),
                            SqlHelper_ExecuteNoQuery,
                            dbReference,
                            this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                            new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(fragmentsEnumeratorReference, IEnumerator_Current), IEnumerable_ToArray)))),
                },
                new CodeStatement[] {
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteNoQuery,
                        dbReference,
                        this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                        new CodeMethodInvokeExpression(statementsReference, IEnumerable_ToArray))),
                });

            CodeExpression enableIdentityInsertStatement = this.m_dbInfoProvider.GetEnableIdentityInsertStatement(table);
            if(enableIdentityInsertStatement == null) {
                method.Statements.Add(insertStatement);
                return;
            }

            CodeConditionStatement conditionStatement = new CodeConditionStatement(enableReference);
            method.Statements.Add(conditionStatement);

            CodeVariableDeclarationStatement prefixVariable = new CodeVariableDeclarationStatement(
                new CodeTypeReference(new CodeTypeReference(typeof(ISqlStatement)), 1),
                "prefix",
                new CodeArrayCreateExpression(typeof(ISqlStatement), new CodeExpression[] { enableIdentityInsertStatement }));
            CodeVariableReferenceExpression prefixReference = new CodeVariableReferenceExpression(prefixVariable.Name);
            conditionStatement.TrueStatements.Add(prefixVariable);
            conditionStatement.TrueStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlHelper)), SqlHelper_MaxNoQueryStatementsLimit), CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0)),
                    CodeBinaryOperatorType.BooleanAnd,
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(statementsReference, ICollection_Count), CodeBinaryOperatorType.GreaterThanOrEqual, new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlHelper)), SqlHelper_MaxNoQueryStatementsLimit))),
                new CodeStatement[] {
                    new CodeAssignStatement(
                        fragmentsEnumeratorReference,
                        new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(
                            statementsReference,
                            IEnumerable_Split,
                            new CodeBinaryOperatorExpression(
                                new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlHelper)), SqlHelper_MaxNoQueryStatementsLimit),
                                CodeBinaryOperatorType.Subtract,
                                new CodePrimitiveExpression(1))), IEnumerable_GetEnumerator)),
                    new CodeIterationStatement(
                        new CodeSnippetStatement(),
                        new CodeMethodInvokeExpression(fragmentsEnumeratorReference, IEnumerator_MoveNext),
                        new CodeSnippetStatement(),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(typeof(SqlHelper)),
                            SqlHelper_ExecuteNoQuery,
                            dbReference,
                            this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                            new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(prefixReference, IEnumerable_Concat, new CodePropertyReferenceExpression(fragmentsEnumeratorReference, IEnumerator_Current)), IEnumerable_ToArray)))),
                },
                new CodeStatement[] {
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(SqlHelper)),
                        SqlHelper_ExecuteNoQuery,
                        dbReference,
                        this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                        new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(prefixReference, IEnumerable_Concat, statementsReference), IEnumerable_ToArray))),
                }));

            conditionStatement.FalseStatements.Add(insertStatement);
        }

        private CodeMemberMethod CreateInsertMultipleMethodWithConnectionString(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression dataArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).AssemblyQualifiedName, new CodeTypeReference(typeParameter)), "data");
            CodeArgumentReferenceExpression dataReference = new CodeArgumentReferenceExpression(dataArgument.Name);
            CodeParameterDeclarationExpression enableArgument = new CodeParameterDeclarationExpression(typeof(bool), "enableIdentityInsert");
            CodeArgumentReferenceExpression enableReference = new CodeArgumentReferenceExpression(enableArgument.Name);
            method.Parameters.Add(dataArgument);
            method.Parameters.Add(enableArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(dataReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", dataArgument.Name)),
                    new CodePrimitiveExpression(dataArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_insertMethodName,
                        connectionReference,
                        dataReference,
                        enableReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateInsertMultipleMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression dataArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).AssemblyQualifiedName, new CodeTypeReference(typeParameter)), "data");
            CodeArgumentReferenceExpression dataReference = new CodeArgumentReferenceExpression(dataArgument.Name);
            CodeParameterDeclarationExpression enableArgument = new CodeParameterDeclarationExpression(typeof(bool), "enableIdentityInsert");
            CodeArgumentReferenceExpression enableReference = new CodeArgumentReferenceExpression(enableArgument.Name);
            method.Parameters.Add(dataArgument);
            method.Parameters.Add(enableArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_insertMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                dataReference,
                enableReference));

            return method;
        }

        private CodeMemberMethod CreateInsertMultipleMethodConnection(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            CodeArgumentReferenceExpression connectionReference = new CodeArgumentReferenceExpression(connectionArgument.Name);
            CodeParameterDeclarationExpression dataArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).AssemblyQualifiedName, new CodeTypeReference(typeParameter)), "data");
            CodeArgumentReferenceExpression dataReference = new CodeArgumentReferenceExpression(dataArgument.Name);
            CodeParameterDeclarationExpression enableArgument = new CodeParameterDeclarationExpression(typeof(bool), "enableIdentityInsert");
            CodeArgumentReferenceExpression enableReference = new CodeArgumentReferenceExpression(enableArgument.Name);
            method.Parameters.Add(connectionArgument);
            method.Parameters.Add(dataArgument);
            method.Parameters.Add(enableArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(dataReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", dataArgument.Name)),
                    new CodePrimitiveExpression(dataArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            this.CreateInsertMultipleMethodCore(method, table, typeParameter, dataReference, enableReference, connectionReference);

            return method;
        }

        private CodeMemberMethod CreateInsertMultipleMethodTransaction(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = null;
            method.Name = this.m_insertMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            CodeArgumentReferenceExpression transactionReference = new CodeArgumentReferenceExpression(transactionArgument.Name);
            CodeParameterDeclarationExpression dataArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IEnumerable<>).AssemblyQualifiedName, new CodeTypeReference(typeParameter)), "data");
            CodeArgumentReferenceExpression dataReference = new CodeArgumentReferenceExpression(dataArgument.Name);
            CodeParameterDeclarationExpression enableArgument = new CodeParameterDeclarationExpression(typeof(bool), "enableIdentityInsert");
            CodeArgumentReferenceExpression enableReference = new CodeArgumentReferenceExpression(enableArgument.Name);
            method.Parameters.Add(transactionArgument);
            method.Parameters.Add(dataArgument);
            method.Parameters.Add(enableArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(dataReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", dataArgument.Name)),
                    new CodePrimitiveExpression(dataArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            this.CreateInsertMultipleMethodCore(method, table, typeParameter, dataReference, enableReference, transactionReference);

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementWithNameMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression useStableParameterNameArgument = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName);
            method.Parameters.Add(useStableParameterNameArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            CodeVariableDeclarationStatement valueVariable = null;
            CodeVariableDeclarationStatement statementVariable = null;
            method.Statements.Add(valueVariable = new CodeVariableDeclarationStatement(typeof(SqlObjectValue), "value", new CodePrimitiveExpression(null)));
            method.Statements.Add(statementVariable = new CodeVariableDeclarationStatement(typeof(ISqlUpdateStatement), "statement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlUpdateStatementType())));

            CodeVariableReferenceExpression valueReference = new CodeVariableReferenceExpression(valueVariable.Name);
            CodeVariableReferenceExpression statementReference = new CodeVariableReferenceExpression(statementVariable.Name);
            CodePropertyReferenceExpression fieldValueClause = new CodePropertyReferenceExpression(statementReference, ISqlUpdateStatement_FieldValueClause);
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(fieldValueClause, ISqlFieldValueClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(table))));
            CodeVariableDeclarationStatement iterationVariable = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));
            CodeVariableReferenceExpression iterationReference = new CodeVariableReferenceExpression(iterationVariable.Name);
            method.Statements.Add(new CodeIterationStatement(
                iterationVariable,
                new CodeBinaryOperatorExpression(iterationReference, CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(valuesReference, Array_Length)),
                new CodeAssignStatement(iterationReference, new CodeBinaryOperatorExpression(iterationReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
                new CodeAssignStatement(valueReference, new CodeIndexerExpression(valuesReference, iterationReference)),
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        valueReference,
                        CodeBinaryOperatorType.IdentityInequality,
                        new CodePrimitiveExpression(null)),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        fieldValueClause,
                        ISqlFieldValueClause_AddField,
                        new CodePropertyReferenceExpression(valueReference, SqlObjectValue_Object),
                        new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                                SqlHelper_CreateParameterExpression,
                                new CodeTypeReference(this.m_dbInfoProvider.GetDbParameterType())),
                            new CodeSnippetExpression(string.Format(
                                "{0} ? \"{1}_\" + {2}.{3}.{4} : \"{1}_\" + {2}.{3}.{4} + {5}++",
                                useStableParameterNameArgument.Name,
                                this.m_dbInfoProvider.GetDbParameterName(table),
                                valueVariable.Name,
                                SqlObjectValue_Object,
                                ISqlObject_Name,
                                this.m_parameterIndexFieldName)),
                            new CodePropertyReferenceExpression(valueReference, SqlObjectValue_Type),
                            new CodeSnippetExpression(string.Format(
                                "{0}.{3} is {1} && {1}.{2}(({1}) {0}.{3}) ? null : {0}.{3}",
                                valueVariable.Name,
                                typeof(string).FullName,
                                String_IsNullOrWhiteSpace,
                                SqlObjectValue_Value))))))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(statementReference, ISqlUpdateStatement_WhereClause), ISqlWhereClause_Condition),
                new CodeArgumentReferenceExpression(conditionArgument.Name)));
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeMethodReturnStatement(statementReference));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), 
                this.m_getUpdateStatementMethodName,
                conditionReference,
                new CodePrimitiveExpression(false),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementWithNameMethodSpecificDbType(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression useStableParameterNameArgument = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName);
            method.Parameters.Add(useStableParameterNameArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            CodeVariableDeclarationStatement valueVariable = null;
            CodeVariableDeclarationStatement statementVariable = null;
            method.Statements.Add(valueVariable = new CodeVariableDeclarationStatement(this.m_dbInfoProvider.GetSqlObjectValueType(), "value", new CodePrimitiveExpression(null)));
            method.Statements.Add(statementVariable = new CodeVariableDeclarationStatement(typeof(ISqlUpdateStatement), "statement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlUpdateStatementType())));

            CodeVariableReferenceExpression valueReference = new CodeVariableReferenceExpression(valueVariable.Name);
            CodeVariableReferenceExpression statementReference = new CodeVariableReferenceExpression(statementVariable.Name);
            CodePropertyReferenceExpression fieldValueClause = new CodePropertyReferenceExpression(statementReference, ISqlUpdateStatement_FieldValueClause);
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(fieldValueClause, ISqlFieldValueClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(table))));
            CodeVariableDeclarationStatement iterationVariable = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));
            CodeVariableReferenceExpression iterationReference = new CodeVariableReferenceExpression(iterationVariable.Name);
            method.Statements.Add(new CodeIterationStatement(
                iterationVariable,
                new CodeBinaryOperatorExpression(iterationReference, CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(valuesReference, Array_Length)),
                new CodeAssignStatement(iterationReference, new CodeBinaryOperatorExpression(iterationReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
                new CodeAssignStatement(valueReference, new CodeIndexerExpression(valuesReference, iterationReference)),
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        valueReference,
                        CodeBinaryOperatorType.IdentityInequality,
                        new CodePrimitiveExpression(null)),
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        fieldValueClause,
                        ISqlFieldValueClause_AddField,
                        this.m_dbInfoProvider.GetSqlObjectValueObject(valueReference),
                        this.m_dbInfoProvider.GetDbParameterExpressionFromSqlObjectValue(
                            new CodeSnippetExpression(string.Format(
                                "{0} ? \"{1}_\" + {2}.{3}.{4} : \"{1}_\" + {2}.{3}.{4} + {5}++",
                                useStableParameterNameArgument.Name,
                                this.m_dbInfoProvider.GetDbParameterName(table),
                                valueVariable.Name,
                                SqlObjectValue_Object,
                                ISqlObject_Name,
                                this.m_parameterIndexFieldName)),
                            new CodePropertyReferenceExpression(valueReference, SqlObjectValue_Type),
                            new CodeSnippetExpression(string.Format(
                                "{0}.{3} is {1} && {1}.{2}(({1}) {0}.{3}) ? null : {0}.{3}",
                                valueVariable.Name,
                                typeof(string).FullName,
                                String_IsNullOrWhiteSpace,
                                SqlObjectValue_Value))))))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(statementReference, ISqlUpdateStatement_WhereClause), ISqlWhereClause_Condition),
                new CodeArgumentReferenceExpression(conditionArgument.Name)));
            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeMethodReturnStatement(statementReference));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementMethodSpecificDbType(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getUpdateStatementMethodName,
                conditionReference,
                new CodePrimitiveExpression(false),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodWithConnectionString(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_updateMethodName,
                        connectionReference,
                        new CodeArgumentReferenceExpression(conditionArgument.Name),
                        valuesReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                conditionReference,
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodConnection(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    new CodeArgumentReferenceExpression(conditionArgument.Name),
                    new CodePrimitiveExpression(true),
                    valuesReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodTransaction(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    new CodeArgumentReferenceExpression(conditionArgument.Name),
                    new CodePrimitiveExpression(true),
                    valuesReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbTypeWithConnectionString(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_updateMethodName,
                        connectionReference,
                        new CodeArgumentReferenceExpression(conditionArgument.Name),
                        valuesReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbType(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                conditionReference,
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbTypeConnection(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    new CodeArgumentReferenceExpression(conditionArgument.Name),
                    new CodePrimitiveExpression(true),
                    valuesReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbTypeTransaction(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    new CodeArgumentReferenceExpression(conditionArgument.Name),
                    new CodePrimitiveExpression(true),
                    valuesReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                stringReference,
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethod(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(typeof(SqlObjectValue[]), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbTypeWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                stringReference,
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbType(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbTypeConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodSpecificDbTypeTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_updateMethodName, constraint);

            //create method parameters
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression valuesArgument = new CodeParameterDeclarationExpression(this.m_dbInfoProvider.GetSqlObjectValueType().MakeArrayType(), "values");
            valuesArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression valuesReference = new CodeArgumentReferenceExpression(valuesArgument.Name);
            method.Parameters.Add(valuesArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Update data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected record..</returns>", true));

            //create bounds checking
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(valuesReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(valuesReference, Array_Length),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(0))),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Values is null or empty."),
                    new CodePrimitiveExpression(valuesArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.CreateCondition(constraint),
                valuesReference)));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Get SQL UPDATE statement by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The SQL UPDATE statement.</returns>", true));

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getUpdateStatementMethodName,
                infoReference,
                new CodePrimitiveExpression(false),
                new CodePrimitiveExpression(null))));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementWithNameMethodExceptedFields(IDbTableEntity table, CodeTypeParameter typeParameter) {
            // create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression useStableParameterNameArgument = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName);
            CodeArgumentReferenceExpression useStableParameterNameReference = new CodeArgumentReferenceExpression(useStableParameterNameArgument.Name);
            method.Parameters.Add(useStableParameterNameArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "exceptedFields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            // create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Gets SQL UPDATE statement by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted fields.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The SQL UPDATE statement.</returns>", true));

            // create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            // create condition of Primary Key
            CodeVariableDeclarationStatement conditionVariable = new CodeVariableDeclarationStatement(typeof(ISqlExpression), "condition", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression conditionReference = new CodeVariableReferenceExpression(conditionVariable.Name);
            method.Statements.Add(conditionVariable);
            method.Statements.Add(new CodeConditionStatement(
                useStableParameterNameReference,
                new CodeStatement[] {
                    new CodeAssignStatement(conditionReference, this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), true)),
                },
                new CodeStatement[] {
                    new CodeAssignStatement(conditionReference, this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), false)),
                }));
            method.Statements.Add(new CodeSnippetStatement());

            // parameters used to invoke Update method
            List<CodeExpression> parameters = new List<CodeExpression>();
            parameters.Add(conditionReference);
            parameters.Add(useStableParameterNameReference);

            // create field values
            foreach(IDbTableFieldEntity entity in table.TableFields) {
                if(entity.IsIdentity || entity.IsReadOnly) {
                    continue;
                }

                parameters.Add(this.m_dbInfoProvider.GetSqlObjectValue(
                    new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(entity)),
                    entity,
                    entity.IsNullable && entity.Type.IsValueType ?
                        (CodeExpression) new CodeSnippetExpression(string.Format("{0}.{1}.{2} ? (object) {0}.{1}.{3} : (object) null",
                            infoArgument.Name,
                            entity.Name,
                            Nullable_HasValue,
                            Nullable_Value)) :
                        (CodeExpression) new CodePropertyReferenceExpression(infoReference, entity.Name)));
            }

            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(fieldsReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                    CodeBinaryOperatorType.BooleanOr,
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(fieldsReference, Array_Length), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0))),
                new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    parameters.ToArray()))));
            method.Statements.Add(new CodeSnippetStatement());

            CodeVariableDeclarationStatement valuesVariable = new CodeVariableDeclarationStatement(typeof(ICollection<>).MakeGenericType(this.m_dbInfoProvider.GetSqlObjectValueType()), "values", new CodeObjectCreateExpression(typeof(List<>).MakeGenericType(this.m_dbInfoProvider.GetSqlObjectValueType())));
            CodeVariableReferenceExpression valuesReference = new CodeVariableReferenceExpression(valuesVariable.Name);
            method.Statements.Add(valuesVariable);

            //create field values
            foreach(IDbTableFieldEntity entity in table.TableFields) {
                if(entity.IsIdentity || entity.IsReadOnly) {
                    continue;
                }

                method.Statements.Add(new CodeConditionStatement(
                    new CodeMethodInvokeExpression(fieldsReference, IEnumerable_Contains, new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(entity))),
                    new CodeStatement[0],
                    new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(
                        valuesReference,
                        ICollection_Add,
                        this.m_dbInfoProvider.GetSqlObjectValue(
                            new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(entity)), 
                            entity, 
                            entity.IsNullable && entity.Type.IsValueType ?
                                (CodeExpression) new CodeSnippetExpression(string.Format("{0}.{1}.{2} ? (object) {0}.{1}.{3} : (object) null",
                                    infoArgument.Name,
                                    entity.Name,
                                    Nullable_HasValue,
                                    Nullable_Value)) :
                                (CodeExpression) new CodePropertyReferenceExpression(infoReference, entity.Name)))) }));
            }
            method.Statements.Add(new CodeSnippetStatement());

            //parameters used to invoke Update method
            parameters.Clear();
            parameters.Add(conditionReference);
            parameters.Add(useStableParameterNameReference);
            parameters.Add(new CodeMethodInvokeExpression(valuesReference, IEnumerable_ToArray));

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getUpdateStatementMethodName,
                parameters.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementMethodExceptedFields(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "exceptedFields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Gets SQL UPDATE statement by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted fields.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The SQL UPDATE statement.</returns>", true));

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getUpdateStatementMethodName,
                infoReference,
                new CodePrimitiveExpression(false),
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateGetUpdateStatementMethodExceptedMembers(IDbTableEntity table, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlUpdateStatement));
            method.Name = this.m_getUpdateStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(table)), "exceptedMembers");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Gets SQL UPDATE statement by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted members.</param>", membersArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The SQL UPDATE statement.</returns>", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getUpdateStatementMethodName,
                infoReference,
                new CodePrimitiveExpression(false),
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodWithConnectionString(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "exceptedFields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted fields.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_updateMethodName,
                        connectionReference,
                        infoReference,
                        fieldsReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "exceptedFields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted fields.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                infoReference,
                fieldsReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodMembersWithConnectionString(IDbTableEntity table, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(table)), "exceptedMembers");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted members.</param>", membersArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements   
            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name))))); 
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create database connection
            method.Statements.Add(new CodeCommentStatement("execute SQL statement"));
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_updateMethodName,
                        connectionReference,
                        infoReference,
                        membersReference)),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodMembers(IDbTableEntity table, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(table)), "exceptedMembers");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted members.</param>", membersArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_updateMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                infoReference,
                membersReference)));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodConnection(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "exceptedFields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted fields.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    infoReference,
                    new CodePrimitiveExpression(true),
                    fieldsReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodConnectionMembers(IDbTableEntity table, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(table)), "exceptedMembers");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted members.</param>", membersArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    infoReference,
                    new CodePrimitiveExpression(true),
                    new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference)))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodTransaction(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression fieldsArgument = new CodeParameterDeclarationExpression(typeof(ISqlObject[]), "exceptedFields");
            fieldsArgument.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
            CodeArgumentReferenceExpression fieldsReference = new CodeArgumentReferenceExpression(fieldsArgument.Name);
            method.Parameters.Add(fieldsArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted fields.</param>", fieldsArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    infoReference,
                    new CodePrimitiveExpression(true),
                    fieldsReference))));

            return method;
        }

        private CodeMemberMethod CreateUpdateMethodTransactionMembers(IDbTableEntity table, CodeTypeParameter typeParameter, string namespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_updateMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);
            CodeParameterDeclarationExpression membersArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(namespaceName, this.GetMembersTypeName(table)), "exceptedMembers");
            CodeArgumentReferenceExpression membersReference = new CodeArgumentReferenceExpression(membersArgument.Name);
            method.Parameters.Add(membersArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Update data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">Excepted members.</param>", membersArgument.Name), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create return statement
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteUpdate,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    this.m_getUpdateStatementMethodName,
                    infoReference,
                    new CodePrimitiveExpression(true),
                    new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getFiledsMethodName, membersReference)))));

            return method;
        }

        private CodeMemberMethod CreateGetDeteleStatementMethod(IDbTableEntity table, bool isCascading, string entityNamespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).MakeGenericType(typeof(ISqlDeleteStatement)));
            method.Name = this.m_getDeteleStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            CodeArgumentReferenceExpression conditionArgumentReference = new CodeArgumentReferenceExpression(conditionArgument.Name);
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            List<CodeExpression> deleteStatements = new List<CodeExpression>();

            //create statement to delete data
            CodeVariableDeclarationStatement statementVariable = new CodeVariableDeclarationStatement(typeof(ISqlDeleteStatement), "statement", new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlDeleteStatementType()));
            CodeVariableReferenceExpression statementReference = new CodeVariableReferenceExpression(statementVariable.Name);
            method.Statements.Add(new CodeCommentStatement("construct DELETE statement"));
            method.Statements.Add(statementVariable);
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        statementReference,
                        ISqlDeleteStatement_FromClause),
                    ISqlFromClause_Source),
                new CodeFieldReferenceExpression(null, this.GetDataSourceFieldName(table))));
            method.Statements.Add(new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodePropertyReferenceExpression(
                        statementReference,
                        ISqlDeleteStatement_WhereClause),
                    ISqlWhereClause_Condition),
                conditionArgumentReference));
            method.Statements.Add(new CodeSnippetStatement());
            deleteStatements.Add(statementReference);

            if(isCascading) {
                //create cascading delete statements
                int conditionIndex = 0;
                int regionIndex = 0;
                int statementIndex = 0;
                CodeVariableDeclarationStatement conditionVariable = null;
                CodeVariableReferenceExpression conditionReference = null;
                CodeVariableDeclarationStatement regionVariable = null;
                CodeVariableReferenceExpression regionReference = null;
                statementVariable = null;
                statementReference = null;
                Queue<IDbTableFieldEntity> fields = new Queue<IDbTableFieldEntity>();
                Queue<CodeVariableReferenceExpression> conditions = new Queue<CodeVariableReferenceExpression>();
                foreach(IDbTableFieldEntity entity in table.TableFields) {
                    if(!entity.RelatedFields.Any()) {
                        continue;
                    }

                    fields.Enqueue(entity);
                    conditions.Enqueue(new CodeVariableReferenceExpression(conditionArgument.Name));
                }

                //traverse all related fields
                IDbTableFieldEntity field = null;
                CodeVariableReferenceExpression condition = null;
                while(fields.Count > 0) {
                    field = fields.Dequeue();
                    condition = conditions.Dequeue();

                    //create region statement
                    method.Statements.Add(new CodeCommentStatement(string.Format("region statement of field {0} in table {1}", field.Name, field.Table.Name)));
                    method.Statements.Add(regionVariable = new CodeVariableDeclarationStatement(typeof(ISqlSelectStatement), string.Format("region{0}", regionIndex++), new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlSelectStatementType())));
                    regionReference = new CodeVariableReferenceExpression(regionVariable.Name);
                    method.Statements.Add(new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(regionReference, ISqlSelectStatement_SelectClause),
                        ISqlSelectClause_AddExpressions,
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(new CodeTypeReference(this.GetAccssorTypeName(field.Table), new CodeTypeReference(this.GetTypeFullname(entityNamespaceName, this.GetEntityTypeName(field.Table))))),
                            this.GetPrivateStaticFieldName(field))));
                    method.Statements.Add(new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(regionReference, ISqlSelectStatement_FromClause),
                            ISqlFromClause_Source),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(new CodeTypeReference(this.GetAccssorTypeName(field.Table), new CodeTypeReference(this.GetTypeFullname(entityNamespaceName, this.GetEntityTypeName(field.Table))))),
                            this.GetDataSourceFieldName(field.Table))));
                    method.Statements.Add(new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(regionReference, ISqlSelectStatement_WhereClause),
                            ISqlWhereClause_Condition),
                        condition));
                    method.Statements.Add(new CodeSnippetStatement());

                    //create delete statement for each related fields
                    foreach(IDbTableFieldEntity f in field.RelatedFields) {
                        //create condition statement
                        method.Statements.Add(new CodeCommentStatement(string.Format("condition used to delete data of table {0} by field {1}", f.Table.Name, f.Name)));
                        method.Statements.Add(conditionVariable = new CodeVariableDeclarationStatement(
                            typeof(ISqlExpression),
                            string.Format("condition{0}", conditionIndex++),
                            new CodeMethodInvokeExpression(
                                new CodeFieldReferenceExpression(
                                    new CodeTypeReferenceExpression(new CodeTypeReference(this.GetAccssorTypeName(f.Table), new CodeTypeReference(this.GetTypeFullname(entityNamespaceName, this.GetEntityTypeName(f.Table))))),
                                    this.GetPrivateStaticFieldName(f)),
                                ISqlExpression_In,
                                regionReference)));
                        conditionReference = new CodeVariableReferenceExpression(conditionVariable.Name);

                        //create delete statement
                        method.Statements.Add(new CodeCommentStatement(string.Format("delete data of {0}", f.Table.Name)));
                        method.Statements.Add(statementVariable = new CodeVariableDeclarationStatement(typeof(ISqlDeleteStatement), string.Format("statement{0}", statementIndex++), new CodeObjectCreateExpression(this.m_dbInfoProvider.GetSqlDeleteStatementType())));
                        statementReference = new CodeVariableReferenceExpression(statementVariable.Name);
                        method.Statements.Add(new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(statementReference, ISqlSelectStatement_FromClause),
                                ISqlFromClause_Source),
                            new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(new CodeTypeReference(this.GetAccssorTypeName(f.Table), new CodeTypeReference(this.GetTypeFullname(entityNamespaceName, this.GetEntityTypeName(f.Table))))),
                                this.GetDataSourceFieldName(f.Table))));
                        method.Statements.Add(new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(statementReference, ISqlSelectStatement_WhereClause),
                                ISqlWhereClause_Condition),
                            conditionReference));
                        method.Statements.Add(new CodeSnippetStatement());
                        deleteStatements.Add(statementReference);

                        //scan related table
                        foreach(IDbTableFieldEntity e in f.Table.TableFields) {
                            if(!e.RelatedFields.Any()) {
                                continue;
                            }

                            fields.Enqueue(e);
                            conditions.Enqueue(conditionReference);
                        }
                    }
                }
            }

            //return
            deleteStatements.Reverse();
            method.Statements.Add(new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(ISqlDeleteStatement), deleteStatements.ToArray())));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodWithConnectionString(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            method.Statements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), String_IsNullOrWhiteSpace, stringReference),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression("Connection string is null or empty."),
                    new CodePrimitiveExpression(stringArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //create method statements
            CodeVariableDeclarationStatement connectionVariable = new CodeVariableDeclarationStatement(typeof(IDbConnection), "connection", new CodePrimitiveExpression(null));
            CodeVariableReferenceExpression connectionReference = new CodeVariableReferenceExpression(connectionVariable.Name);
            method.Statements.Add(connectionVariable);
            method.Statements.Add(new CodeTryCatchFinallyStatement(
                new CodeStatement[] {
                    new CodeAssignStatement(
                        connectionReference,
                        new CodeObjectCreateExpression(this.m_dbInfoProvider.GetDbConnectionType())),
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            connectionReference,
                            IDbConnection_ConnectionString),
                        stringReference),
                    new CodeSnippetStatement(),
                    new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_deleteMethodName,
                        connectionReference,
                        new CodeArgumentReferenceExpression(conditionArgument.Name))),
                },
                new CodeCatchClause[0],
                new CodeStatement[] {
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            connectionReference,
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        new CodeExpressionStatement(new CodeMethodInvokeExpression(
                            connectionReference,
                            IDisposable_Dispose))),
                }));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethod(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.m_getWriteConnectionStringMethodName),
                new CodeArgumentReferenceExpression(conditionArgument.Name))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodConnection(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteDelete,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getDeteleStatementMethodName,
                        new CodeArgumentReferenceExpression(conditionArgument.Name)),
                    IEnumerable_ToArray))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodTransaction(IDbTableEntity table) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression conditionArgument = new CodeParameterDeclarationExpression(typeof(ISqlExpression), "condition");
            method.Parameters.Add(conditionArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<inheritdoc />", true));

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(SqlHelper)),
                SqlHelper_ExecuteDelete,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.m_dbInfoProvider.GetSqlStatementCommandTextProvider(),
                new CodeMethodInvokeExpression(
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        this.m_getDeteleStatementMethodName,
                        new CodeArgumentReferenceExpression(conditionArgument.Name)),
                    IEnumerable_ToArray))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodWithConnectionString(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_deleteMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Delete data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create method parameters and statements
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());

            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                stringReference,
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethod(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_deleteMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Delete data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create method parameters and statements
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodConnection(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_deleteMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Delete data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create method parameters and statements
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodTransaction(IDbConstraintEntity constraint) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.GetMethodNameByConstraint(this.m_deleteMethodName, constraint);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("Delete data by {0}.", constraint.Type), true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create method parameters and statements
            method.Parameters.AddRange(this.CreateArguments(constraint).ToArray());
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.CreateCondition(constraint))));

            return method;
        }

        private CodeMemberMethod CreateGetDeleteStatementMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(IEnumerable<>).MakeGenericType(typeof(ISqlDeleteStatement)));
            method.Name = this.m_getDeteleStatementMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Get SQL DELETE statement by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The SQL DELETE statemen.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getDeteleStatementMethodName,
                this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), false))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodWithConnectionString(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression stringArgument = new CodeParameterDeclarationExpression(typeof(string), "connectionString");
            CodeArgumentReferenceExpression stringReference = new CodeArgumentReferenceExpression(stringArgument.Name);
            method.Parameters.Add(stringArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Delete data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                stringReference,
                this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), true))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethod(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Delete data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), true))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodConnection(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression connectionArgument = new CodeParameterDeclarationExpression(typeof(IDbConnection), "connection");
            method.Parameters.Add(connectionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Delete data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                new CodeArgumentReferenceExpression(connectionArgument.Name),
                this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), true))));

            return method;
        }

        private CodeMemberMethod CreateDeleteMethodTransaction(IDbTableEntity table, CodeTypeParameter typeParameter) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Name = this.m_deleteMethodName;

            //create method parameters
            CodeParameterDeclarationExpression transactionArgument = new CodeParameterDeclarationExpression(typeof(IDbTransaction), "transaction");
            method.Parameters.Add(transactionArgument);
            CodeParameterDeclarationExpression infoArgument = new CodeParameterDeclarationExpression(new CodeTypeReference(typeParameter), "info");
            CodeArgumentReferenceExpression infoReference = new CodeArgumentReferenceExpression(infoArgument.Name);
            method.Parameters.Add(infoArgument);

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Delete data by the specified info.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));
            method.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">A {1} object.</param>", infoArgument.Name, this.GetEntityTypeName(table)), true));
            method.Comments.Add(new CodeCommentStatement("<returns>The number of effected records.</returns>", true));

            //create bounds checking statements      
            method.Statements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(infoReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(
                    typeof(ArgumentException),
                    new CodePrimitiveExpression(string.Format("{0} is null.", infoArgument.Name)),
                    new CodePrimitiveExpression(infoArgument.Name)))));
            method.Statements.Add(new CodeSnippetStatement());

            //return
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_deleteMethodName,
                new CodeArgumentReferenceExpression(transactionArgument.Name),
                this.CreateCondition(table.PrimaryKey, new CodeVariableReferenceExpression(infoArgument.Name), true))));

            return method;
        }

        private CodeExpression CreateCondition(IDbConstraintEntity constraint) {
            CodeExpression condition = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlExpression)), SqlExpression_True);

            foreach(IDbTableFieldEntity field in constraint.Fields) {
                if(field.Type.IsValueType && field.IsNullable) {
                    condition = new CodeMethodInvokeExpression(
                        condition,
                        ISqlExpression_And,
                        new CodeSnippetExpression(string.Format(
                            "{0}.{1} ? {2}.{3}(this.{4}(true, {0}.{5})) : {2}.{6}()",
                            this.GetArgumentNameByConstraint(constraint.Type, field),
                            Nullable_HasValue,
                            this.GetPrivateStaticFieldName(field),
                            ISqlExpression_Equal,
                            this.GetCreateParameterExpressionMethodName(field),
                            Nullable_Value,
                            ISqlExpression_IsNull)));
                } else {
                    condition = new CodeMethodInvokeExpression(
                        condition, 
                        ISqlExpression_And,
                        new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)), 
                            ISqlExpression_Equal,
                            new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(), 
                                this.GetCreateParameterExpressionMethodName(field), 
                                new CodePrimitiveExpression(true), 
                                new CodeArgumentReferenceExpression(this.GetArgumentNameByConstraint(constraint.Type, field)))));
                }
            }

            return condition;
        }

        private CodeExpression CreateCondition(IDbConstraintEntity constraint, CodeVariableReferenceExpression info, bool useStableParameterName) {
            CodeExpression condition = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlExpression)), SqlExpression_True);

            foreach(IDbTableFieldEntity field in constraint.Fields) {
                if(field.Type.IsValueType && field.IsNullable) {
                    condition = new CodeMethodInvokeExpression(
                        condition,
                        ISqlExpression_And,
                        new CodeSnippetExpression(string.Format(
                            "{0}.{1}.{2} ? {3}.{4}(this.{5}({6}, {0}.{1}.{7})) : {3}.{8}()",
                            info.VariableName,
                            field.Name,
                            Nullable_HasValue,
                            this.GetPrivateStaticFieldName(field),
                            ISqlExpression_Equal,
                            this.GetCreateParameterExpressionMethodName(field),
                            useStableParameterName.ToString().ToLower(),
                            Nullable_Value,
                            ISqlExpression_IsNull)));
                } else {
                    condition = new CodeMethodInvokeExpression(
                        condition,
                        ISqlExpression_And,
                        new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)),
                            ISqlExpression_Equal,
                            new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(),
                                this.GetCreateParameterExpressionMethodName(field),
                                new CodePrimitiveExpression(useStableParameterName),
                                new CodePropertyReferenceExpression(info, field.Name))));
                }
            }

            return condition;
        }

        private ICollection<CodeParameterDeclarationExpression> CreateArguments(IDbConstraintEntity constraint) {
            ICollection<CodeParameterDeclarationExpression> arguments = new List<CodeParameterDeclarationExpression>(constraint.Fields.Count());
            foreach(IDbTableFieldEntity field in constraint.Fields) {
                arguments.Add(new CodeParameterDeclarationExpression(
                    field.Type.IsValueType && field.IsNullable ? NullableTypeDefinition.MakeGenericType(field.Type) : field.Type,
                    this.GetArgumentNameByConstraint(constraint.Type, field)));
            }
            return arguments;
        }

        private ICollection<CodeTypeMember> CreateReaderMethods(IDbDataEntity data, CodeTypeParameter typeParameter, string entityNamespaceName) {
            IList<CodeTypeMember> members = new List<CodeTypeMember>();

            members.Add(new CodeSnippetTypeMember());
            members.Add(this.CreateGetSelectStatementMethod(data));
            members.Add(this.CreateLoadMethodWithConnectionString(data, typeParameter));
            members.Add(this.CreateLoadMethod(data, typeParameter));
            members.Add(this.CreateLoadMethodConnection(data, typeParameter));
            members.Add(this.CreateLoadMethodTransaction(data, typeParameter));
            if(data.Fields.Count() <= MAX_MEMBERS_COUNT) {
                members.Add(this.CreateGetSelectStatementMethodMembers(data, entityNamespaceName));
                members.Add(this.CreateLoadMethodMembersWithConnectionString(data, typeParameter, entityNamespaceName));
                members.Add(this.CreateLoadMethodMembers(data, typeParameter, entityNamespaceName));
                members.Add(this.CreateLoadMethodConnectionMembers(data, typeParameter, entityNamespaceName));
                members.Add(this.CreateLoadMethodTransactionMembers(data, typeParameter, entityNamespaceName));
            }
            if(data is IDbTableEntity) {
                IDbTableEntity table = (IDbTableEntity) data;
                IDbFieldEntity primaryKey = table.PrimaryKey != null && table.PrimaryKey.Fields.Count() == 1 ? table.PrimaryKey.Fields.First() : null;
                if(primaryKey != null) {
                    members.Add(this.CreateGetSelectStatmentMethodPaginationOnePrimaryKey(primaryKey));
                    if(data.Fields.Count() <= MAX_MEMBERS_COUNT) {
                        members.Add(this.CreateGetSelectStatmentMethodPaginationOnePrimaryKeyMembers(primaryKey, entityNamespaceName));
                    }
                } else {
                    members.Add(this.CreateGetSelectStatmentMethodPaginationGeneric(data));
                    if(data.Fields.Count() <= MAX_MEMBERS_COUNT) {
                        members.Add(this.CreateGetSelectStatmentMethodPaginationGenericMembers(data, entityNamespaceName));
                    }
                }
            } else {
                members.Add(this.CreateGetSelectStatmentMethodPaginationGeneric(data));
                if(data.Fields.Count() <= MAX_MEMBERS_COUNT) {
                    members.Add(this.CreateGetSelectStatmentMethodPaginationGenericMembers(data, entityNamespaceName));
                }
            }
            members.Add(this.CreateLoadMethodPaginationWithConnectionString(data, typeParameter));
            members.Add(this.CreateLoadMethodPagination(data, typeParameter));
            members.Add(this.CreateLoadMethodPaginationConnection(data, typeParameter));
            members.Add(this.CreateLoadMethodPaginationTransaction(data, typeParameter));
            if(data.Fields.Count() <= MAX_MEMBERS_COUNT) {
                members.Add(this.CreateLoadMethodPaginationMembersWithConnectionString(data, typeParameter, entityNamespaceName));
                members.Add(this.CreateLoadMethodPaginationMembers(data, typeParameter, entityNamespaceName));
                members.Add(this.CreateLoadMethodPaginationConnectionMembers(data, typeParameter, entityNamespaceName));
                members.Add(this.CreateLoadMethodPaginationTransactionMembers(data, typeParameter, entityNamespaceName));
            }
            members.Add(this.CreateGetCountStatementMethod(data));
            members.Add(this.CreateLoadCountMethodWithConnectionString(data));
            members.Add(this.CreateLoadCountMethod(data));
            members.Add(this.CreateLoadCountMethodConnection(data));
            members.Add(this.CreateLoadCountMethodTransaction(data));
            members.Add(this.CreateGetExistsStatementMethod(data));
            members.Add(this.CreateIsExistsMethodWithConnectionString(data));
            members.Add(this.CreateIsExistsMethod(data));
            members.Add(this.CreateIsExistsMethodConnection(data));
            members.Add(this.CreateIsExistsMethodTransaction(data));
            members.Add(this.CreateGetLastIdentityMethodWithConnectionString(data));
            members.Add(this.CreateGetLastIdentityMethod(data));
            members.Add(this.CreateGetLastIdentityMethodConnection(data));
            members.Add(this.CreateGetLastIdentityMethodTransaction(data));
            members.Add(new CodeSnippetTypeMember());

            if(members.Count > 0) {
                members[0].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Methods", typeof(IDbEntityReader<>).Name)));
                members[members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            }

            return members;
        }

        private ICollection<CodeTypeMember> CreateWriterMethods(IDbTableEntity table, bool isCascading, CodeTypeParameter typeParameter, string entityNamespaceName) {
            IList<CodeTypeMember> members = new List<CodeTypeMember>();

            members.Add(new CodeSnippetTypeMember());
            members.Add(this.CreateGetParametersMethod(table, typeParameter));
            members.Add(this.CreateGetInsertStatementMethod(table, typeParameter));
            members.Add(this.CreateInsertMethodWithConnectionString(table, typeParameter));
            members.Add(this.CreateInsertMethod(table, typeParameter));
            members.Add(this.CreateInsertMethodConnection(table, typeParameter));
            members.Add(this.CreateInsertMethodTransaction(table, typeParameter));
            members.Add(this.CreateInsertMultipleMethodWithConnectionString(table, typeParameter));
            members.Add(this.CreateInsertMultipleMethod(table, typeParameter));
            members.Add(this.CreateInsertMultipleMethodConnection(table, typeParameter));
            members.Add(this.CreateInsertMultipleMethodTransaction(table, typeParameter));
            members.Add(this.CreateGetUpdateStatementMethod(table));
            members.Add(this.CreateGetUpdateStatementMethodSpecificDbType(table));
            members.Add(this.CreateUpdateMethodWithConnectionString(table));
            members.Add(this.CreateUpdateMethod(table));
            members.Add(this.CreateUpdateMethodConnection(table));
            members.Add(this.CreateUpdateMethodTransaction(table));
            members.Add(this.CreateUpdateMethodSpecificDbTypeWithConnectionString(table));
            members.Add(this.CreateUpdateMethodSpecificDbType(table));
            members.Add(this.CreateUpdateMethodSpecificDbTypeConnection(table));
            members.Add(this.CreateUpdateMethodSpecificDbTypeTransaction(table));
            members.Add(this.CreateGetDeteleStatementMethod(table, isCascading, entityNamespaceName));
            members.Add(this.CreateDeleteMethodWithConnectionString(table));
            members.Add(this.CreateDeleteMethod(table));
            members.Add(this.CreateDeleteMethodConnection(table));
            members.Add(this.CreateDeleteMethodTransaction(table));
            members.Add(new CodeSnippetTypeMember());

            if(members.Count > 0) {
                members[0].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Methods", typeof(IDbEntityWriter<>).Name)));
                members[members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            }

            return members;
        }

        private ICollection<CodeTypeMember> CreatePrimaryKeyMethods(IDbTableEntity table, CodeTypeParameter typeParameter, string entityNamespaceName) {
            IDbConstraintEntity constraint = table.PrimaryKey;
            IList<CodeTypeMember> members = new List<CodeTypeMember>();

            if(constraint != null) {
                members.Add(new CodeSnippetTypeMember());

                bool hasSame = (from c in this.m_processedConstraints
                                where c.HasSameFields(constraint)
                                select c).Count() > 0;

                if(!hasSame) {
                    members.Add(this.CreateLoadMethodWithConnectionString(constraint, typeParameter));
                    members.Add(this.CreateLoadMethod(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodConnection(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodTransaction(constraint, typeParameter));
                    if(table.Fields.Count() <= MAX_MEMBERS_COUNT) {
                        members.Add(this.CreateLoadMethodMembersWithConnectionString(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodConnectionMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodTransactionMembers(constraint, typeParameter, entityNamespaceName));
                    }
                    members.Add(this.CreateLoadCountMethodWithConnectionString(constraint));
                    members.Add(this.CreateLoadCountMethod(constraint));
                    members.Add(this.CreateLoadCountMethodConnection(constraint));
                    members.Add(this.CreateLoadCountMethodTransaction(constraint));
                    members.Add(this.CreateIsExistsMethodWithConnectionString(constraint));
                    members.Add(this.CreateIsExistsMethod(constraint));
                    members.Add(this.CreateIsExistsMethodConnection(constraint));
                    members.Add(this.CreateIsExistsMethodTransaction(constraint));
                    members.Add(this.CreateUpdateMethodWithConnectionString(constraint));
                    members.Add(this.CreateUpdateMethod(constraint));
                    members.Add(this.CreateUpdateMethodConnection(constraint));
                    members.Add(this.CreateUpdateMethodTransaction(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeWithConnectionString(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbType(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeConnection(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeTransaction(constraint));
                    members.Add(this.CreateDeleteMethodWithConnectionString(constraint));
                    members.Add(this.CreateDeleteMethod(constraint));
                    members.Add(this.CreateDeleteMethodConnection(constraint));
                    members.Add(this.CreateDeleteMethodTransaction(constraint));
                }

                members.Add(this.CreateGetUpdateStatementMethod(table, typeParameter));
                members.Add(this.CreateGetUpdateStatementMethodExceptedFields(table, typeParameter));
                members.Add(this.CreateGetUpdateStatementMethodExceptedMembers(table, typeParameter, entityNamespaceName));
                members.Add(this.CreateUpdateMethodWithConnectionString(table, typeParameter));
                members.Add(this.CreateUpdateMethod(table, typeParameter));
                members.Add(this.CreateUpdateMethodConnection(table, typeParameter));
                members.Add(this.CreateUpdateMethodTransaction(table, typeParameter));
                if(table.Fields.Count() <= MAX_MEMBERS_COUNT) {
                    members.Add(this.CreateUpdateMethodMembersWithConnectionString(table, typeParameter, entityNamespaceName));
                    members.Add(this.CreateUpdateMethodMembers(table, typeParameter, entityNamespaceName));
                    members.Add(this.CreateUpdateMethodConnectionMembers(table, typeParameter, entityNamespaceName));
                    members.Add(this.CreateUpdateMethodTransactionMembers(table, typeParameter, entityNamespaceName));
                }
                members.Add(this.CreateGetDeleteStatementMethod(table, typeParameter));
                members.Add(this.CreateDeleteMethodWithConnectionString(table, typeParameter));
                members.Add(this.CreateDeleteMethod(table, typeParameter));
                members.Add(this.CreateDeleteMethodConnection(table, typeParameter));
                members.Add(this.CreateDeleteMethodTransaction(table, typeParameter));
                members.Add(new CodeSnippetTypeMember());

                if(members.Count > 0) {
                    members[0].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Primary Key Methods"));
                    members[members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
                }

                this.m_processedConstraints.Add(constraint);
            }

            return members;
        }

        private ICollection<CodeTypeMember> CreateUniqueKeyMethods(IDbTableEntity table, CodeTypeParameter typeParameter, string entityNamespaceName) {
            bool hasSame = false;
            IList<CodeTypeMember> members = new List<CodeTypeMember>();
            members.Add(new CodeSnippetTypeMember());

            foreach(IDbConstraintEntity constraint in table.UniqueKeys) {
                hasSame = (from c in this.m_processedConstraints
                           where c.HasSameFields(constraint)
                           select c).Count() > 0;

                if(!hasSame) {
                    members.Add(this.CreateLoadMethodWithConnectionString(constraint, typeParameter));
                    members.Add(this.CreateLoadMethod(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodConnection(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodTransaction(constraint, typeParameter));
                    if(table.Fields.Count() <= MAX_MEMBERS_COUNT) {
                        members.Add(this.CreateLoadMethodMembersWithConnectionString(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodConnectionMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodTransactionMembers(constraint, typeParameter, entityNamespaceName));
                    }
                    members.Add(this.CreateLoadCountMethodWithConnectionString(constraint));
                    members.Add(this.CreateLoadCountMethod(constraint));
                    members.Add(this.CreateLoadCountMethodConnection(constraint));
                    members.Add(this.CreateLoadCountMethodTransaction(constraint));
                    members.Add(this.CreateIsExistsMethodWithConnectionString(constraint));
                    members.Add(this.CreateIsExistsMethod(constraint));
                    members.Add(this.CreateIsExistsMethodConnection(constraint));
                    members.Add(this.CreateIsExistsMethodTransaction(constraint));
                    if(constraint.Table.PrimaryKey != null) {
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethodWithConnectionString(constraint));
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethod(constraint));
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethodConnection(constraint));
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethodTransaction(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethodWithConnectionString(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethod(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethodConnection(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethodTransaction(constraint));
                    }
                    members.Add(this.CreateUpdateMethodWithConnectionString(constraint));
                    members.Add(this.CreateUpdateMethod(constraint));
                    members.Add(this.CreateUpdateMethodConnection(constraint));
                    members.Add(this.CreateUpdateMethodTransaction(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeWithConnectionString(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbType(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeConnection(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeTransaction(constraint));
                    members.Add(this.CreateDeleteMethodWithConnectionString(constraint));
                    members.Add(this.CreateDeleteMethod(constraint));
                    members.Add(this.CreateDeleteMethodConnection(constraint));
                    members.Add(this.CreateDeleteMethodTransaction(constraint));
                }

                this.m_processedConstraints.Add(constraint);
            }
            members.Add(new CodeSnippetTypeMember());

            if(members.Count > 0) {
                members[0].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Unique Key Methods"));
                members[members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            }

            return members;
        }

        private ICollection<CodeTypeMember> CreateForeignKeyMethods(IDbTableEntity table, CodeTypeParameter typeParameter, string entityNamespaceName) {
            bool hasSame = false;
            IList<CodeTypeMember> members = new List<CodeTypeMember>();
            members.Add(new CodeSnippetTypeMember());

            foreach(IDbConstraintEntity constraint in table.ForeignKeys) {
                hasSame = (from c in this.m_processedConstraints
                           where c.HasSameFields(constraint)
                           select c).Count() > 0;

                if(!hasSame) {
                    members.Add(this.CreateLoadMethodWithConnectionString(constraint, typeParameter));
                    members.Add(this.CreateLoadMethod(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodConnection(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodTransaction(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodPaginationWithConnectionString(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodPagination(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodPaginationConnection(constraint, typeParameter));
                    members.Add(this.CreateLoadMethodPaginationTransaction(constraint, typeParameter));
                    if(table.Fields.Count() <= MAX_MEMBERS_COUNT) {
                        members.Add(this.CreateLoadMethodMembersWithConnectionString(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodConnectionMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodTransactionMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodPaginationMembersWithConnectionString(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodPaginationMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodPaginationConnectionMembers(constraint, typeParameter, entityNamespaceName));
                        members.Add(this.CreateLoadMethodPaginationTransactionMembers(constraint, typeParameter, entityNamespaceName));
                    }
                    members.Add(this.CreateLoadCountMethodWithConnectionString(constraint));
                    members.Add(this.CreateLoadCountMethod(constraint));
                    members.Add(this.CreateLoadCountMethodConnection(constraint));
                    members.Add(this.CreateLoadCountMethodTransaction(constraint));
                    members.Add(this.CreateIsExistsMethodWithConnectionString(constraint));
                    members.Add(this.CreateIsExistsMethod(constraint));
                    members.Add(this.CreateIsExistsMethodConnection(constraint));
                    members.Add(this.CreateIsExistsMethodTransaction(constraint));
                    if(constraint.Table.PrimaryKey != null) {
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethodWithConnectionString(constraint));
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethod(constraint));
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethodConnection(constraint));
                        members.Add(this.CreateLoadCountExcludePrimaryKeyMethodTransaction(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethodWithConnectionString(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethod(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethodConnection(constraint));
                        members.Add(this.CreateIsExistsExcludePrimaryKeyMethodTransaction(constraint));
                    }
                    members.Add(this.CreateUpdateMethodWithConnectionString(constraint));
                    members.Add(this.CreateUpdateMethod(constraint));
                    members.Add(this.CreateUpdateMethodConnection(constraint));
                    members.Add(this.CreateUpdateMethodTransaction(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeWithConnectionString(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbType(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeConnection(constraint));
                    members.Add(this.CreateUpdateMethodSpecificDbTypeTransaction(constraint));
                    members.Add(this.CreateDeleteMethodWithConnectionString(constraint));
                    members.Add(this.CreateDeleteMethod(constraint));
                    members.Add(this.CreateDeleteMethodConnection(constraint));
                    members.Add(this.CreateDeleteMethodTransaction(constraint));
                }

                this.m_processedConstraints.Add(constraint);
            }
            members.Add(new CodeSnippetTypeMember());

            if(members.Count > 0) {
                members[0].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Foreign Key Methods"));
                members[members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            }

            return members;
        }

        private CodeMemberMethod CreateGetSourceMethod(IDbDataEntity data) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(this.m_dbInfoProvider.GetSqlSourceType());
            method.Name = this.m_getSoureMethodName;

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Get query source.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create method body
            method.Statements.Add(new CodeMethodReturnStatement(this.m_dbInfoProvider.GetSqlSource(!string.IsNullOrWhiteSpace(data.Schema) ? new CodePrimitiveExpression(data.Schema) : null, new CodePrimitiveExpression(data.Name))));

            return method;
        }

        private CodeMemberMethod CreateGetQueryConditionMethod(IDbDataEntity data, string entityNamespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
            method.Name = this.m_getQueryConditionMethodName;

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Get query condition.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create parameter
            CodeParameterDeclarationExpression queryConditionArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(entityNamespaceName, this.GetQueryConditionTypeName(data)), "queryCondition");
            CodeArgumentReferenceExpression queryConditionReference = new CodeArgumentReferenceExpression(queryConditionArgument.Name);
            method.Parameters.Add(queryConditionArgument);

            //create method statements
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                this.m_getQueryConditionMethodName,
                queryConditionReference,
                new CodePrimitiveExpression(true))));

            return method;
        }

        private CodeMemberMethod CreateGetQueryConditionWithNameMethod(IDbDataEntity data, string entityNamespaceName) {
            //create method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public;
            method.ReturnType = new CodeTypeReference(typeof(ISqlExpression));
            method.Name = this.m_getQueryConditionMethodName;

            //create method comments
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("Get query condition.", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //create parameter
            CodeParameterDeclarationExpression queryConditionArgument = new CodeParameterDeclarationExpression(this.GetTypeFullname(entityNamespaceName, this.GetQueryConditionTypeName(data)), "queryCondition");
            CodeArgumentReferenceExpression queryConditionReference = new CodeArgumentReferenceExpression(queryConditionArgument.Name);
            method.Parameters.Add(queryConditionArgument);
            CodeParameterDeclarationExpression useStableParameterNameArgument = new CodeParameterDeclarationExpression(typeof(bool), this.m_useStableParameterNameArgumentName);
            CodeArgumentReferenceExpression useStableParameterNameReference = new CodeArgumentReferenceExpression(useStableParameterNameArgument.Name);
            method.Parameters.Add(useStableParameterNameArgument);

            //create method statements
            CodeVariableDeclarationStatement condition = new CodeVariableDeclarationStatement(
                typeof(ISqlExpression),
                "condition",
                new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlExpression)), SqlExpression_True));
            CodeVariableReferenceExpression conditionReference = new CodeVariableReferenceExpression(condition.Name);

            method.Statements.Add(condition);
            method.Statements.Add(new CodeSnippetStatement());

            CodePropertyReferenceExpression property = null;
            foreach(IDbFieldEntity field in data.Fields) {
                property = new CodePropertyReferenceExpression(queryConditionReference, field.Name);

                method.Statements.Add(new CodeConditionStatement(
                    field.Type.IsValueType ?
                        (CodeExpression) new CodePropertyReferenceExpression(property, Nullable_HasValue) :
                        field.Type.IsString() ?
                            (CodeExpression) new CodeSnippetExpression(string.Format("!string.{0}({1}.{2})", String_IsNullOrWhiteSpace, queryConditionArgument.Name, field.Name)) :
                            (CodeExpression) new CodeBinaryOperatorExpression(property, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                    new CodeAssignStatement(
                        conditionReference,
                        new CodeMethodInvokeExpression(
                            conditionReference,
                            ISqlExpression_And,
                            new CodeMethodInvokeExpression(
                                new CodeFieldReferenceExpression(null, this.GetPrivateStaticFieldName(field)),
                                ISqlExpression_Equal,
                                new CodeMethodInvokeExpression(
                                    new CodeThisReferenceExpression(),
                                    this.GetCreateParameterExpressionMethodName(field),
                                    useStableParameterNameReference,
                                    field.Type.IsValueType ?
                                        (CodeExpression) new CodePropertyReferenceExpression(property, Nullable_Value) :
                                        (CodeExpression) property))))));
            }

            method.Statements.Add(new CodeSnippetStatement());
            method.Statements.Add(new CodeMethodReturnStatement(conditionReference));

            return method;
        }

        #endregion

        #region Generate Code Methods

        internal CodeCompileUnit GenerateCompileUnit() {
            return new CodeCompileUnit();
        }

        internal CodeNamespace GenerateNamespace(CodeCompileUnit unit, string namespaceName) {
            CodeNamespace ns = new CodeNamespace(namespaceName);
            unit.Namespaces.Add(ns);

            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data"));
            ns.Imports.Add(new CodeNamespaceImport("System.Linq"));
            ns.Imports.Add(new CodeNamespaceImport("System.Threading"));
            ns.Imports.Add(new CodeNamespaceImport("Xphter.Framework"));
            ns.Imports.Add(new CodeNamespaceImport("Xphter.Framework.Collections"));
            this.m_dbInfoProvider.PrepareCodeNamespace(ns);

            return ns;
        }

        internal CodeTypeDeclaration GenerateEntityCode(CodeNamespace ns, IDbTableEntity table) {
            CodeTypeDeclaration type = this.CreateEntityType(ns, table);

            type.Members.AddRange(this.CreateInstanceConstructorsOfEntityType(table).ToArray());
            type.Members.Add(this.CreateGetHashCodeMethod(table));
            type.Members.Add(this.CreateEqualsMethod(table));
            type.Members.Add(this.CreateInitializeEntityMethod(table));
            type.Members.Add(this.CreateValidateMethod(table));
            type.Members.Add(this.CreateCopyMethod(table));
            type.Members.Add(this.CreateCloneMethod(table));
            foreach(IDbTableFieldEntity field in table.TableFields) {
                type.Members.AddRange(this.CreateEntityProperty(field).ToArray());
            }

            return type;
        }

        internal CodeTypeDeclaration GenerateEntityCode(CodeNamespace ns, IDbViewEntity view) {
            CodeTypeDeclaration type = this.CreateEntityType(ns, view);

            type.Members.AddRange(this.CreateInstanceConstructorsOfEntityType(view).ToArray());
            type.Members.Add(this.CreateInitializeEntityMethod(view));
            type.Members.Add(this.CreateCopyMethod(view));
            type.Members.Add(this.CreateCloneMethod(view));
            foreach(IDbViewFieldEntity field in view.ViewFields) {
                type.Members.AddRange(this.CreateEntityProperty(field).ToArray());
            }

            return type;
        }

        internal CodeTypeDeclaration GenerateMembersCode(CodeNamespace ns, IDbTableEntity table) {
            CodeTypeDeclaration type = this.CreateMembersType(ns, table);

            // check none member
            if(table.TableFields.Any((item) => string.Equals(item.Name, MEMBERS_NAME_NONE))) {
                throw new ArgumentException(string.Format("Can not create members type of table {0}, it has a field named {1}.", table.Name, MEMBERS_NAME_NONE), "table");
            }

            // create none member
            type.Members.Add(this.CreateMembersField(MEMBERS_NAME_NONE, 0, "None members"));

            // create each members
            int index = 0;
            long value = 0, all = 0;
            foreach(IDbTableFieldEntity field in table.TableFields) {
                value = Convert.ToInt64(Math.Pow(2, index++));
                all |= value;

                type.Members.Add(this.CreateMembersField(field.Name, value, field.Description ?? field.Name));
            }
            type.Members.Add(this.CreateMembersField(MEMBERS_NAME_All, all, "All members"));

            return type;
        }

        internal CodeTypeDeclaration GenerateMembersCode(CodeNamespace ns, IDbViewEntity view) {
            CodeTypeDeclaration type = this.CreateMembersType(ns, view);

            // check none member
            if(view.ViewFields.Any((item) => string.Equals(item.Name, MEMBERS_NAME_NONE))) {
                throw new ArgumentException(string.Format("Can not create members type of view {0}, it has a field named {1}.", view.Name, MEMBERS_NAME_NONE), "view");
            }

            // create none member
            type.Members.Add(this.CreateMembersField(MEMBERS_NAME_NONE, 0, "None members"));

            // create each members
            int index = 0;
            long value = 0, all = 0;
            foreach(IDbViewFieldEntity field in view.ViewFields) {
                value = Convert.ToInt64(Math.Pow(2, index++));
                all |= value;

                type.Members.Add(this.CreateMembersField(field.Name, value, field.Name));
            }
            type.Members.Add(this.CreateMembersField(MEMBERS_NAME_All, all, "All members"));

            return type;
        }

        internal CodeTypeDeclaration GenerateQueryConditionCode(CodeNamespace ns, IDbTableEntity table) {
            CodeTypeDeclaration type = this.CreateQueryConditionType(ns, table);

            //create fields and properties
            CodeMemberField field = null;
            CodeMemberProperty property = null;
            foreach(IDbTableFieldEntity entity in table.Fields) {
                this.CreateQueryConditionProperty(entity, entity.Description ?? entity.Name, out field, out property);
                type.Members.Add(field);
                type.Members.Add(property);
            }

            return type;
        }

        internal CodeTypeDeclaration GenerateQueryConditionCode(CodeNamespace ns, IDbViewEntity view) {
            //create type
            CodeTypeDeclaration type = this.CreateQueryConditionType(ns, view);

            //create fields and properties
            CodeMemberField field = null;
            CodeMemberProperty property = null;
            foreach(IDbViewFieldEntity entity in view.Fields) {
                this.CreateQueryConditionProperty(entity, entity.Name, out field, out property);
                type.Members.Add(field);
                type.Members.Add(property);
            }

            return type;
        }

        internal void CreateFieldsOfAccessorType(CodeTypeDeclaration type) {
            type.Members.Add(this.CreateParameterIndexField());
            type.Members.Add(this.CreateWriteConnectionStringsField());
            type.Members.Add(this.CreateReadConnectionStringsField());
            type.Members.Add(this.CreateWriteConnectionStringSelectorField());
            type.Members.Add(this.CreateReadConnectionStringSelectorField());
        }

        internal void CreatePropertiesOfAccessorType(CodeTypeDeclaration type) {
            type.Members.Add(this.CreateWriteConnectionStringsProperty());
            type.Members.Add(this.CreateReadConnectionStringsProperty());
            type.Members.Add(this.CreateWriteConnectionStringSelectorProperty());
            type.Members.Add(this.CreateReadConnectionStringSelectorProperty());
            type.Members.Add(this.CreateConnectionStringProperty());
        }

        internal void CreateInternalMethodsOfAccessorType(CodeTypeDeclaration type, IDbDataEntity data, CodeTypeParameter typeParameter) {
            type.Members.Add(this.CreateGetWriteConnectionString());
            type.Members.Add(this.CreateGetReadConnectionString());
            type.Members.Add(this.CreateLoadFromReaderMethod(data, typeParameter));
        }

        internal CodeTypeDeclaration GenerateAccessorCode(CodeNamespace ns, IDbTableEntity table, bool isCascading, string entityNamespaceName) {
            CodeTypeDeclaration type = this.CreateAccessorType(ns, entityNamespaceName, table);
            CodeTypeParameter typeParameter = type.TypeParameters[0];
            type.BaseTypes.Add(new CodeTypeReference(typeof(IDbEntityReader<>).FullName, new CodeTypeReference(typeParameter)));
            type.BaseTypes.Add(new CodeTypeReference(typeof(IDbEntityWriter<>).FullName, new CodeTypeReference(typeParameter)));

            type.Members.Add(this.CreateStaticConstructorOfAccessorType(table));
            type.Members.AddRange(this.CreateSqlObjectFields(table).ToArray());
            type.Members.AddRange(this.CreateInstanceConstructorOfAccessorType(table));

            this.CreateFieldsOfAccessorType(type);
            this.CreatePropertiesOfAccessorType(type);
            this.CreateInternalMethodsOfAccessorType(type, table, typeParameter);
            
            if(table.Fields.Count() <= MAX_MEMBERS_COUNT) {
                type.Members.Add(this.CreateGetFiledsMethods(table, entityNamespaceName));   
            }
            type.Members.AddRange(this.CreateCreateParameterMethods(table).ToArray());
            type.Members.Add(this.CreateGetInsertStatementWithNameMethod(table, typeParameter));
            type.Members.Add(this.CreateGetUpdateStatementWithNameMethod(table));
            type.Members.Add(this.CreateGetUpdateStatementWithNameMethodSpecificDbType(table));
            if(table.PrimaryKey != null) {
                type.Members.Add(this.CreateGetUpdateStatementWithNameMethodExceptedFields(table, typeParameter));
            }
            type.Members.AddRange(this.CreateReaderMethods(table, typeParameter, entityNamespaceName).ToArray());
            type.Members.AddRange(this.CreateWriterMethods(table, isCascading, typeParameter, entityNamespaceName).ToArray());

            this.m_processedConstraints.Clear();
            if(table.PrimaryKey != null) {
                type.Members.AddRange(this.CreatePrimaryKeyMethods(table, typeParameter, entityNamespaceName).ToArray());
            }
            if(table.UniqueKeys.Any()) {
                type.Members.AddRange(this.CreateUniqueKeyMethods(table, typeParameter, entityNamespaceName).ToArray());
            }
            if(table.ForeignKeys.Any()) {
                type.Members.AddRange(this.CreateForeignKeyMethods(table, typeParameter, entityNamespaceName).ToArray());
            }
            type.Members.Add(this.CreateGetSourceMethod(table));
            type.Members.Add(this.CreateGetQueryConditionMethod(table, entityNamespaceName));
            type.Members.Add(this.CreateGetQueryConditionWithNameMethod(table, entityNamespaceName));

            return type;
        }

        internal CodeTypeDeclaration GenerateAccessorCode(CodeNamespace ns, IDbViewEntity view, string entityNamespaceName) {
            CodeTypeDeclaration type = this.CreateAccessorType(ns, entityNamespaceName, view);
            CodeTypeParameter typeParameter = type.TypeParameters[0];
            type.BaseTypes.Add(new CodeTypeReference(typeof(IDbEntityReader<>).FullName, new CodeTypeReference(typeParameter)));

            type.Members.Add(this.CreateStaticConstructorOfAccessorType(view));
            type.Members.AddRange(this.CreateSqlObjectFields(view).ToArray());
            type.Members.AddRange(this.CreateInstanceConstructorOfAccessorType(view));

            this.CreateFieldsOfAccessorType(type);
            this.CreatePropertiesOfAccessorType(type);
            this.CreateInternalMethodsOfAccessorType(type, view, typeParameter);

            if(view.Fields.Count() <= MAX_MEMBERS_COUNT) {
                type.Members.Add(this.CreateGetFiledsMethods(view, entityNamespaceName));
            }
            type.Members.AddRange(this.CreateCreateParameterMethods(view).ToArray());
            type.Members.AddRange(this.CreateReaderMethods(view, typeParameter, entityNamespaceName).ToArray());
            type.Members.Add(this.CreateGetSourceMethod(view));
            type.Members.Add(this.CreateGetQueryConditionMethod(view, entityNamespaceName));
            type.Members.Add(this.CreateGetQueryConditionWithNameMethod(view, entityNamespaceName));

            return type;
        }

        #endregion

        #region IDbCodeGenerator Members

        /// <inheritdoc />
        public void GenerateCode(IDbDatabaseEntity database, bool isCascading, string entityNamespaceName, string accessorNamespaceName, string folder) {
            if(database == null) {
                throw new ArgumentException("Database entity is null.", "database");
            }
            if(string.IsNullOrWhiteSpace(entityNamespaceName)) {
                throw new ArgumentException("Namespace is null or empty.", "namespaceName");
            }

            //Code generation options
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "Block";
            options.ElseOnClosing = true;
            options.IndentString = "    ";
            options.VerbatimOrder = true;

            if(!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            //generate codes
            CodeCompileUnit unit = null;
            CodeNamespace ns = null;

            foreach(IDbTableEntity table in database.Tables) {
                unit = this.GenerateCompileUnit();
                ns = this.GenerateNamespace(unit, entityNamespaceName);
                this.GenerateEntityCode(ns, table);
                if(table.Fields.Count() <= MAX_MEMBERS_COUNT) {
                    this.GenerateMembersCode(ns, table);
                }
                this.GenerateQueryConditionCode(ns, table);

                using(TextWriter writer = new StreamWriter(Path.Combine(folder ?? string.Empty, string.Format("{0}.{1}", this.GetEntityCodeFilename(table), this.m_codeProvider.FileExtension)), false, Encoding.UTF8)) {
                    this.m_codeProvider.GenerateCodeFromCompileUnit(unit, writer, options);
                }

                unit = this.GenerateCompileUnit();
                ns = this.GenerateNamespace(unit, accessorNamespaceName);
                this.GenerateAccessorCode(ns, table, isCascading, entityNamespaceName);
                
                using(TextWriter writer = new StreamWriter(Path.Combine(folder ?? string.Empty, string.Format("{0}.{1}", this.GetAccessorCodeFilename(table), this.m_codeProvider.FileExtension)), false, Encoding.UTF8)) {
                    this.m_codeProvider.GenerateCodeFromCompileUnit(unit, writer, options);
                }
            }

            foreach(IDbViewEntity view in database.Views) {
                unit = this.GenerateCompileUnit();
                ns = this.GenerateNamespace(unit, entityNamespaceName);
                this.GenerateEntityCode(ns, view);
                if(view.Fields.Count() <= MAX_MEMBERS_COUNT) {
                    this.GenerateMembersCode(ns, view);
                }
                this.GenerateQueryConditionCode(ns, view);

                using(TextWriter writer = new StreamWriter(Path.Combine(folder ?? string.Empty, string.Format("{0}.{1}", this.GetEntityCodeFilename(view), this.m_codeProvider.FileExtension)), false, Encoding.UTF8)) {
                    this.m_codeProvider.GenerateCodeFromCompileUnit(unit, writer, options);
                }

                unit = this.GenerateCompileUnit();
                ns = this.GenerateNamespace(unit, accessorNamespaceName);
                this.GenerateAccessorCode(ns, view, entityNamespaceName);

                using(TextWriter writer = new StreamWriter(Path.Combine(folder ?? string.Empty, string.Format("{0}.{1}", this.GetAccessorCodeFilename(view), this.m_codeProvider.FileExtension)), false, Encoding.UTF8)) {
                    this.m_codeProvider.GenerateCodeFromCompileUnit(unit, writer, options);
                }
            }
        }

        #endregion
    }
}
