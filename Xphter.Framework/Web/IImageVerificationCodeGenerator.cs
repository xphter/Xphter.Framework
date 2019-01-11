using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Xphter.Framework.IO;
using Xphter.Framework.Net;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Generates a image verification code.
    /// </summary>
    public interface IImageVerificationCodeGenerator {
        /// <summary>
        /// Generates a new image verification code.
        /// </summary>
        /// <returns></returns>
        ImageVerificationCode Generate();
    }

    /// <summary>
    /// Provides the value of a image verification code.
    /// </summary>
    public interface IImageVerificationCodeValueProvider {
        /// <summary>
        /// Gets a new value of verification code.
        /// </summary>
        /// <returns></returns>
        string GetValue();
    }

    /// <summary>
    /// Represents a image verification code.
    /// </summary>
    public class ImageVerificationCode {
        /// <summary>
        /// Initialize a instance of VerificationCode class.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="headers"></param>
        /// <exception cref="System.ArgumentException"><paramref name="image"/> is null.</exception>
        public ImageVerificationCode(Image image, ImageFormat format, string value, IDictionary<string, string> headers) {
            if(image == null) {
                throw new ArgumentException("The image is null.", "image");
            }

            headers = headers ?? new Dictionary<string, string>();
            headers["Content-Type"] = HttpHelper.GetContentType(format);
            this.Initialize(this.GetDataFromImage(image, format), value, headers);
        }

        /// <summary>
        /// Initialize a instance of VerificationCode class.
        /// </summary>
        /// <param name="response"></param>
        public ImageVerificationCode(HttpWebResponse response) {
            IDictionary<string, string> headers = new Dictionary<string, string>(response.Headers.Count);
            foreach(string name in response.Headers) {
                headers[name] = response.Headers[name];
            }

            this.Initialize(response.GetRawContent(1024), null, headers);
        }

        /// <summary>
        /// Initialize a instance of VerificationCode class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="headers"></param>
        /// <exception cref="System.ArgumentException"><paramref name="data"/> is null or empty.</exception>
        public ImageVerificationCode(byte[] data, ImageFormat format, string value, IDictionary<string, string> headers) {
            if(data == null || data.Length == 0) {
                throw new ArgumentException("The image byte data is null or empty.", "image");
            }

            headers = headers ?? new Dictionary<string, string>();
            headers["Content-Type"] = HttpHelper.GetContentType(format);
            this.Initialize(data, value, headers);
        }

        private byte[] m_data;
        private IDictionary<string, string> m_headers;

        /// <summary>
        /// Gets the value of verification code.
        /// </summary>
        public string Value {
            get;
            private set;
        }

        protected void Initialize(byte[] data, string value, IDictionary<string, string> headers) {
            this.m_data = data;
            this.Value = value;
            this.m_headers = headers ?? new Dictionary<string, string>();
            this.m_headers["Content-Length"] = this.m_data.Length.ToString();
        }

        /// <summary>
        /// Gets byte data from a image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private byte[] GetDataFromImage(Image image, ImageFormat format) {
            using(MemoryStream writer = new MemoryStream()) {
                using(image) {
                    image.Save(writer, format);
                }
                return writer.ToArray();
            }
        }

        /// <summary>
        /// Saves this verification code to a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="System.ArgumentException"><paramref name="filename"/> is null or empty or not represents a valid file path.</exception>
        public virtual void Save(string filename) {
            if(string.IsNullOrWhiteSpace(filename)) {
                throw new ArgumentException("The file path is null or empty.", "filename");
            }
            if(!PathUtility.IsValidLocalPath(filename)) {
                throw new ArgumentException("filename not represents a valid file path.", "filename");
            }

            string folder = Path.GetDirectoryName(filename);
            if(!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllBytes(filename, this.m_data);
        }

        /// <summary>
        /// Flushs all content to a HTTP response.
        /// </summary>
        /// <param name="response"></param>
        public virtual void Flush(HttpResponseBase response) {
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            foreach(KeyValuePair<string, string> item in this.m_headers) {
                response.AppendHeader(item.Key, item.Value);
            }

            response.OutputStream.Write(this.m_data, 0, this.m_data.Length);
        }
    }
}
