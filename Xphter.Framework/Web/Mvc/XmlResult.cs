using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Xphter.Framework.IO;

namespace Xphter.Framework.Web.Mvc {
    /// <summary>
    /// Outputs the XML content to the HTTP response, the default content encoding is UTF-8.
    /// </summary>
    public class XmlResult : ActionResult {
        /// <summary>
        /// Initializes a new instance of XmlResult class.
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="data"/> is null.</exception>
        public XmlResult(object data)
            : this(data, Encoding.UTF8) {
        }

        /// <summary>
        /// Initializes a new instance of XmlResult class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="data"/> is null.</exception>
        public XmlResult(object data, Encoding encoding) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }

            using(StringWriter writer = new StringWriterWithEncoding(encoding = encoding ?? Encoding.UTF8)) {
                new XmlSerializer(data.GetType()).Serialize(writer, data);
                this.m_content = writer.GetStringBuilder().ToString();
            }
            this.m_encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of XmlResult class.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="content"/> is null.</exception>
        public XmlResult(string content)
            : this(content, Encoding.UTF8) {
        }

        /// <summary>
        /// Initializes a new instance of XmlResult class.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="content"/> is null.</exception>
        public XmlResult(string content, Encoding encoding) {
            if(content == null) {
                throw new ArgumentNullException("content");
            }

            this.m_content = content;
            this.m_encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// Initializes a new instance of XmlResult class.
        /// </summary>
        /// <param name="document"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="document"/> is null.</exception>
        public XmlResult(XmlDocument document)
            : this(document, Encoding.UTF8) {
        }

        /// <summary>
        /// Initializes a new instance of XmlResult class.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="document"/> is null.</exception>
        public XmlResult(XmlDocument document, Encoding encoding) {
            if(document == null) {
                throw new ArgumentNullException("document");
            }

            this.m_content = document.OuterXml;
            this.m_encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// The XML content text.
        /// </summary>
        private string m_content;

        /// <summary>
        /// The content-encoding of HTTP response.
        /// </summary>
        private Encoding m_encoding;

        /// <inheritdoc />
        public override void ExecuteResult(ControllerContext context) {
            if(context == null) {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentEncoding = this.m_encoding;
            response.ContentType = "application/xml";
            response.Write(this.m_content);
        }
    }
}
