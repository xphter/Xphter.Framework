using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Backup and restore a SQL SERVER database;
    /// </summary>
    public class SqlServerMover {
        /// <summary>
        /// Initialize a instance of SqlServerMover class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="System.ArgumentException"><paramref name="connectionString"/> is null or empty.</exception>
        public SqlServerMover(string connectionString) {
            if(string.IsNullOrWhiteSpace(connectionString)) {
                throw new ArgumentException("The connection string is null or empty.", "connectionString");
            }

            this.m_connectionString = new SqlConnectionStringBuilder(connectionString) {
                AsynchronousProcessing = true,
                InitialCatalog = "master",
            }.ConnectionString;
        }

        private readonly string m_backupStatement = "BACKUP DATABASE @Name TO  DISK = @File WITH NOFORMAT, INIT, SKIP";
        private readonly string m_restoreStatement = "RESTORE DATABASE @Name FROM  DISK = @File WITH  FILE = 1,  REPLACE";

        private string m_connectionString;

        private SqlCommand CreateCommand(string name, string file, bool isBackup) {
            SqlConnection connection = null;
            SqlCommand command = null;
            try {
                connection = new SqlConnection(this.m_connectionString);
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;
                command.CommandText = isBackup ? this.m_backupStatement : this.m_restoreStatement;
                command.Parameters.Add(SqlHelper.CreateParameter<SqlParameter>("@Name", DbType.String, name));
                command.Parameters.Add(SqlHelper.CreateParameter<SqlParameter>("@File", DbType.String, file));
                return command;
            } catch(DbException ex) {
                this.Clean(connection, command);
                throw ex;
            } catch(DataException ex) {
                this.Clean(connection, command);
                throw ex;
            }
        }

        private void Clean(SqlConnection connection, SqlCommand command) {
            if(connection != null) {
                connection.Dispose();
            }
            if(command != null) {
                command.Dispose();
            }
        }

        private void CheckInput(string name, string file, bool isBackup) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("The database name is null or empty.", "name");
            }
            if(string.IsNullOrWhiteSpace(file)) {
                throw new ArgumentException("The file path is null or empty.", "file");
            }
            if(isBackup) {
                string directory = Path.GetDirectoryName(file);
                if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
            } else {
                if(!File.Exists(file)) {
                    throw new FileNotFoundException("The file used to restor is not existing.", file);
                }
            }
        }

        private void Move(string name, string file, bool isBackup) {
            this.CheckInput(name, file, isBackup);

            SqlCommand command = this.CreateCommand(name, file, isBackup);
            try {
                command.ExecuteNonQuery();
            } finally {
                this.Clean(command.Connection, command);
            }
        }

        private void MoveAsync(string name, string file, bool isBackup, Action<AsyncCompletedEventArgs> callback, object userState) {
            this.CheckInput(name, file, isBackup);

            AsyncOperation operation = AsyncOperationManager.CreateOperation(userState);
            SqlCommand command = this.CreateCommand(name, file, isBackup);
            command.BeginExecuteNonQuery((ar) => {
                Exception error = null;
                try {
                    command.EndExecuteNonQuery(ar);
                } catch(Exception ex) {
                    error = ex;
                } finally {
                    this.Clean(command.Connection, command);
                    operation.PostOperationCompleted((state) => callback(new AsyncCompletedEventArgs(error, false, state)), operation.UserSuppliedState);
                }
            }, null);
        }

        private IAsyncResult BeginMove(string name, string file, bool isBackup, AsyncCallback callback, object userState) {
            this.CheckInput(name, file ,isBackup);

            SqlCommand command = this.CreateCommand(name, file, isBackup);
            return new AsyncResultWrapper(command.BeginExecuteNonQuery(callback != null ? (ar) => {
                callback(new AsyncResultWrapper(ar, command));
            } : (AsyncCallback) null, userState), command);
        }

        private void EndMove(IAsyncResult ar) {
            if(!(ar is AsyncResultWrapper)) {
                throw new InvalidOperationException(string.Format("ar is not a {0} object.", typeof(AsyncResultWrapper).Name));
            }

            AsyncResultWrapper result = (AsyncResultWrapper) ar;
            SqlCommand command = result.Command;
            try {
                command.EndExecuteNonQuery(result.InternalResult);
            } finally {
                this.Clean(command.Connection, command);
            }
        }

        #region Backup Methods

        /// <summary>
        /// Backups database to the specified file.
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="file">File path.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> or <paramref name="file"/> is null or empty.</exception>
        public void Backup(string name, string file) {
            this.Move(name, file, true);
        }

        /// <summary>
        /// Asynchronous backup database to the specified file 
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="file">File path.</param>
        /// <param name="userState"></param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> or <paramref name="file"/> is null or empty.</exception>
        public void BackupAsync(string name, string file, object userState) {
            this.MoveAsync(name, file, true, this.OnBackupCompleted, userState);
        }

        /// <summary>
        /// Occurs when the backup operation has completed.
        /// </summary>
        public event AsyncCompletedEventHandler BackupCompleted;

        protected virtual void OnBackupCompleted(AsyncCompletedEventArgs e) {
            if(this.BackupCompleted != null) {
                this.BackupCompleted(this, e);
            }
        }

        /// <summary>
        /// Begin backup database to the specified file 
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="file">File path.</param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> or <paramref name="file"/> is null or empty.</exception>
        public IAsyncResult BeginBackup(string name, string file, AsyncCallback callback, object userState) {
            return this.BeginMove(name, file, true, callback, userState);
        }

        /// <summary>
        /// End backup.
        /// </summary>
        /// <param name="ar"></param>
        public void EndBackup(IAsyncResult ar) {
            this.EndMove(ar);
        }

        #endregion

        #region Restore Methods

        /// <summary>
        /// Restores database from the specified file.
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="file">File path.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> or <paramref name="file"/> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="file"/> is not existing.</exception>
        public void Restore(string name, string file) {
            this.Move(name, file, false);
        }

        /// <summary>
        /// Asynchronous restore database from the specified file 
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="file">File path.</param>
        /// <param name="userState"></param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> or <paramref name="file"/> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="file"/> is not existing.</exception>
        public void RestoreAsync(string name, string file, object userState) {
            this.MoveAsync(name, file, false, this.OnRestoreCompleted, userState);
        }

        /// <summary>
        /// Occurs when the restore operation has completed.
        /// </summary>
        public event AsyncCompletedEventHandler RestoreCompleted;

        protected virtual void OnRestoreCompleted(AsyncCompletedEventArgs e) {
            if(this.RestoreCompleted != null) {
                this.RestoreCompleted(this, e);
            }
        }

        /// <summary>
        /// Begin restore database from the specified file 
        /// </summary>
        /// <param name="name">Database name.</param>
        /// <param name="file">File path.</param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> or <paramref name="file"/> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="file"/> is not existing.</exception>
        public IAsyncResult BeginRestore(string name, string file, AsyncCallback callback, object userState) {
            return this.BeginMove(name, file, false, callback, userState);
        }

        /// <summary>
        /// End restore.
        /// </summary>
        /// <param name="ar"></param>
        public void EndRestore(IAsyncResult ar) {
            this.EndMove(ar);
        }

        #endregion

        #region AsyncResultWrapper Class

        /// <summary>
        /// Provides a wrapper of IAsyncResult.
        /// </summary>
        private class AsyncResultWrapper : IAsyncResult {
            /// <summary>
            /// Initialize a instance of AsyncResultWrapper class.
            /// </summary>
            /// <param name="result"></param>
            /// <param name="command"></param>
            public AsyncResultWrapper(IAsyncResult result, SqlCommand command) {
                this.InternalResult = result;
                this.Command = command;
            }

            /// <summary>
            /// Gets the internal IAsyncResult object.
            /// </summary>
            public IAsyncResult InternalResult {
                get;
                private set;
            }

            /// <summary>
            /// Gets the SqlCommand object.
            /// </summary>
            public SqlCommand Command {
                get;
                private set;
            }

            #region IAsyncResult Members

            /// <inheritdoc />
            public object AsyncState {
                get {
                    return this.InternalResult.AsyncState;
                }
            }

            /// <inheritdoc />
            public WaitHandle AsyncWaitHandle {
                get {
                    return this.InternalResult.AsyncWaitHandle;
                }
            }

            /// <inheritdoc />
            public bool CompletedSynchronously {
                get {
                    return this.InternalResult.CompletedSynchronously;
                }
            }

            /// <inheritdoc />
            public bool IsCompleted {
                get {
                    return this.InternalResult.IsCompleted;
                }
            }

            #endregion
        }

        #endregion
    }
}
