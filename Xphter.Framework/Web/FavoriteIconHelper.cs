using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provides functions to access favicon.ico file.
    /// </summary>
    public static class FavoriteIconHelper {
        public const string URL = "~/favicon.ico";

        /// <summary>
        /// Gets a value to indicate whether the favicon.icon file is existing.
        /// </summary>
        /// <returns></returns>
        public static bool Exists() {
            return File.Exists(HostingEnvironment.MapPath(URL));
        }

        /// <summary>
        /// Updates the favicon.ico with the specified stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Update(Stream reader, out string message) {
            message = null;

            Icon icon = null;
            try {
                icon = new Icon(reader);
            } catch {
                message = "文件不包含有效的图标数据";
                return false;
            }

            using(icon) {
                using(Stream writer = File.Open(HostingEnvironment.MapPath(URL), FileMode.Create, FileAccess.Write, FileShare.None)) {
                    icon.Save(writer);
                }
            }
            return true;
        }
    }
}
