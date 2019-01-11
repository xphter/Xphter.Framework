using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Reflection {
    /// <summary>
    /// Provides functions to access methods of a object.
    /// </summary>
    public static class MethodUtility {
        /// <summary>
        /// Returns the method that invoked the currently executing method.
        /// </summary>
        /// <returns></returns>
        public static MethodBase GetCallingMethod() {
            StackTrace stack = new StackTrace();
            if(stack.FrameCount < 3) {
                return null;
            }

            return stack.GetFrame(2).GetMethod();
        }
    }
}
