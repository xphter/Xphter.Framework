using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Extends the HttpPostedFileBase class.
    /// </summary>
    public static class HttpPostedFileBaseExtension {
        /// <summary>
        /// Gets the file content.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetContent(this HttpPostedFileBase file) {
            using(MemoryStream writer = new MemoryStream(file.ContentLength)) {
                using(Stream reader = file.InputStream) {
                    int count;
                    byte[] buffer = new byte[file.ContentLength];
                    while((count = reader.Read(buffer, 0, buffer.Length)) > 0) {
                        writer.Write(buffer, 0, count);
                    }
                }

                return writer.ToArray();
            }
        }
    }
}
