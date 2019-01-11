using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    public static class SqlServerTableHints {
        public static readonly ISqlExpression NoExpand = new SqlStringExpression("NOEXPAND");

        public static ISqlFunction Index {
            get {
                return new SqlFunction("INDEX");
            }
        }

        public static readonly ISqlExpression KeepIdentity = new SqlStringExpression("KEEPIDENTITY");

        public static readonly ISqlExpression KeepDefaults = new SqlStringExpression("KEEPDEFAULTS");

        public static readonly ISqlExpression ForceSeek = new SqlStringExpression("FORCESEEK");

        public static ISqlFunction ForceSeekIndex {
            get {
                return new SqlFunction("FORCESEEK");
            }
        }

        public static readonly ISqlExpression ForceScan = new SqlStringExpression("FORCESCAN");

        public static readonly ISqlExpression HoldLock = new SqlStringExpression("HOLDLOCK");

        public static readonly ISqlExpression IgnoreConstraints = new SqlStringExpression("IGNORE_CONSTRAINTS");

        public static readonly ISqlExpression IgnoreTriggers = new SqlStringExpression("IGNORE_TRIGGERS");

        public static readonly ISqlExpression NoLock = new SqlStringExpression("NOLOCK");

        public static readonly ISqlExpression NoWait = new SqlStringExpression("NOWAIT");

        public static readonly ISqlExpression PageLock = new SqlStringExpression("PAGLOCK");

        public static readonly ISqlExpression ReadCommitted = new SqlStringExpression("READCOMMITTED");

        public static readonly ISqlExpression ReadCommittedLock = new SqlStringExpression("READCOMMITTEDLOCK");

        public static readonly ISqlExpression ReadPast = new SqlStringExpression("READPAST");

        public static readonly ISqlExpression ReadUncommitted = new SqlStringExpression("READUNCOMMITTED");

        public static readonly ISqlExpression RepeatableRead = new SqlStringExpression("REPEATABLEREAD");

        public static readonly ISqlExpression RowLock = new SqlStringExpression("ROWLOCK");

        public static readonly ISqlExpression Serializable = new SqlStringExpression("SERIALIZABLE");

        public static readonly ISqlExpression TableLock = new SqlStringExpression("TABLOCK");

        public static readonly ISqlExpression TableLockX = new SqlStringExpression("TABLOCKX");

        public static readonly ISqlExpression UpdateLock = new SqlStringExpression("UPDLOCK");

        public static readonly ISqlExpression XLock = new SqlStringExpression("XLOCK");
    }
}
