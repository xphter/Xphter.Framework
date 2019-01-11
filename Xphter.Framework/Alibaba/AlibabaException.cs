using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Alibaba {
    /// <summary>
    /// The exception that is thrown when an error occurs while accessing alibaba service.
    /// </summary>
    public class AlibabaException : Exception {
        public AlibabaException()
            : base() {
        }

        public AlibabaException(string message)
            : base(message) {
        }

        public AlibabaException(int errorCode)
            : base() {
            this.ErrorCode = errorCode;
        }

        public AlibabaException(string message, int errorCode)
            : base(message) {
            this.ErrorCode = errorCode;
        }

        public AlibabaException(string message, int errorCode, Exception innerException)
            : base(message, innerException) {
            this.ErrorCode = errorCode;
        }

        public AlibabaException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            this.ErrorCode = info.GetInt32(TypeUtility.GetMemberName<AlibabaException, int>((obj) => obj.ErrorCode));
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode {
            get;
            private set;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue(TypeUtility.GetMemberName<AlibabaException, int>((obj) => obj.ErrorCode), this.ErrorCode);
        }
    }
}
