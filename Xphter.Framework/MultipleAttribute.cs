using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Specifies the target can provides multiple values.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class MultipleAttribute : Attribute {
    }
}
