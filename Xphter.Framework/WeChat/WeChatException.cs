using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.WeChat {
    public class WeChatException : Exception {
        public WeChatException()
            : base() {
        }

        public WeChatException(string message)
            : base(message) {
        }

        public WeChatException(int errorCode)
            : base() {
            this.ErrorCode = errorCode;
        }

        public WeChatException(string message, int errorCode)
            : base(message) {
            this.ErrorCode = errorCode;
        }

        public WeChatException(string message, int errorCode, Exception innerException)
            : base(message, innerException) {
            this.ErrorCode = errorCode;
        }

        public WeChatException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            this.ErrorCode = info.GetInt32(TypeUtility.GetMemberName<WeChatException, int>((obj) => obj.ErrorCode));
        }

        public int ErrorCode {
            get;
            private set;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue(TypeUtility.GetMemberName<WeChatException, int>((obj) => obj.ErrorCode), this.ErrorCode);
        }
    }
}
