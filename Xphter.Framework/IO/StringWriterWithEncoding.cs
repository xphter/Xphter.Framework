using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Provides a StringWriter with setting text encoding.
    /// </summary>
    public sealed class StringWriterWithEncoding : StringWriter {
        /// <summary>
        /// Initialize a instance of StringWriterWithEncoding class.
        /// </summary>
        /// <param name="encoding"></param>
        public StringWriterWithEncoding(Encoding encoding) {
            this.m_encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// The text encoding.
        /// </summary>
        private Encoding m_encoding;

        /// <inheritdoc />
        public override Encoding Encoding {
            get {
                return this.m_encoding;
            }
        }
    }
}
