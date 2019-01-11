using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Data {
    /// <summary>
    /// Provides a default implementation of ISqlStatementCommandTextProvider interface.
    /// </summary>
    public class DefaultSqlStatementCommandTextProvider : ISqlStatementCommandTextProvider {
        #region Singletone

        private class SingletoneContainer {
            static SingletoneContainer() {
                Instance = new DefaultSqlStatementCommandTextProvider();
            }

            public static DefaultSqlStatementCommandTextProvider Instance;
        }

        public static DefaultSqlStatementCommandTextProvider Instance {
            get {
                return SingletoneContainer.Instance;
            }
        }

        #endregion

        #region ISqlStatementCommandTextProvider Members

        /// <inheritdoc />
        public string GetCommandText(IEnumerable<ISqlStatement> statements) {
            if(statements == null || !statements.Any()) {
                throw new ArgumentException("statements is null or empty.", "statements");
            }

            return statements.Select((item) => item.SqlString).StringJoin(';');
        }

        #endregion
    }
}
