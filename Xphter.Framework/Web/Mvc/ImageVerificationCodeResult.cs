using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Xphter.Framework.Web.Mvc {
    /// <summary>
    /// Outputs the image verification code to the HTTP response.
    /// </summary>
    public class ImageVerificationCodeResult : ActionResult {
        /// <summary>
        /// Initialize a instance of ImageVerificationCodeResult class.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="format"></param>
        /// <exception cref="System.ArgumentException"><paramref name="code"/> is null.</exception>
        public ImageVerificationCodeResult(ImageVerificationCode code) {
            if(code == null) {
                throw new ArgumentNullException("code");
            }

            this.Code = code;
        }

        /// <summary>
        /// Gets the verification code.
        /// </summary>
        public ImageVerificationCode Code {
            get;
            private set;
        }

        /// <inheritdoc />
        public override void ExecuteResult(ControllerContext context) {
            this.Code.Flush(context.HttpContext.Response);
        }
    }
}
