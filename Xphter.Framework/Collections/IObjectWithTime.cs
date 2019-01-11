using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a object which has a Time property.
    /// </summary>
    public interface IObjectWithTime {
        /// <summary>
        /// Gets the time.
        /// </summary>
        DateTime Time {
            get;
        }
    }

    /// <summary>
    /// Provides a default implementation of IObjectWithTime interface.
    /// </summary>
    public class ObjectWithTime : IObjectWithTime {
        /// <summary>
        /// Initialize a new instance of ObjectWithTime class.
        /// </summary>
        /// <param name="time"></param>
        public ObjectWithTime(DateTime time) {
            this.Time = time;
        }

        #region IObjectWithTime Members

        /// <inheritdoc />
        public DateTime Time {
            get;
            private set;
        }

        #endregion
    }
}
