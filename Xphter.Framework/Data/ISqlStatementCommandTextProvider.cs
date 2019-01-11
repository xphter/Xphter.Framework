using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data {
    /// <summary>
    /// Provides command text of a IDbCommand by the specified ISqlStatement objects.
    /// </summary>
    public interface ISqlStatementCommandTextProvider {
        /// <summary>
        /// Gets command text by the specified ISqlStatement objects.
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="statements"/> is null or empty.</exception>
        string GetCommandText(IEnumerable<ISqlStatement> statements);
    }
}
