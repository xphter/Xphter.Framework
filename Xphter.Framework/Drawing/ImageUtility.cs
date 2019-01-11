using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xphter.Framework.IO;
using Xphter.Framework.Net;

namespace Xphter.Framework.Drawing {
    /// <summary>
    /// Provides functions used to access or modify images.
    /// </summary>
    public static class ImageUtility {
        /// <summary>
        /// Checks and normalizes the specified file extensions.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string PrepareExtension(string extension) {
            if(string.IsNullOrWhiteSpace(extension)) {
                throw new ArgumentException("The file extension is null or empty.", "extension");
            }

            if(!extension.StartsWith(".")) {
                extension = "." + extension;
            }

            return extension;
        }

        /// <summary>
        /// Scales this image.
        /// </summary>
        /// <param name="image">The image want to be scale.</param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <returns>A new image with the specified width and height.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="width"/> or <paramref name="height"/> is less than zero.</exception>
        public static Image Scale(this Image image, int width, int height) {
            if(width < 0) {
                throw new ArgumentException("with is less than zero.", "width");
            }
            if(height < 0) {
                throw new ArgumentException("height is less than zero.", "height");
            }

            PixelFormat format = image.PixelFormat;
            switch(format) {
                //using a custom pixel format when the original pixel format is indexed.
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    format = PixelFormat.Format32bppArgb;
                    break;
                //using a custom pixel format when the original pixel format is undefined.
                case PixelFormat.Undefined:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                    format = PixelFormat.Format32bppArgb;
                    break;
            }
            Bitmap canvas = new Bitmap(width, height, format);
            using(Graphics g = Graphics.FromImage(canvas)) {
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, width, height);
            }
            return canvas;
        }

        /// <summary>
        /// Set transparency of this image.
        /// </summary>
        /// <param name="image">The image want to be set transparency.</param>
        /// <param name="alpha">The value of alpha component, 0 represents completely transparent and 1 represents opaque.</param>
        /// <returns>A new image with the specifed transparency.</returns>
        public static Image Transparency(this Image image, float alpha) {
            alpha = Math.Max(0, Math.Min(alpha, 1));

            Bitmap canvas = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using(Graphics g = Graphics.FromImage(canvas)) {
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            BitmapData data = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), ImageLockMode.ReadWrite, canvas.PixelFormat);
            byte[] argb = new byte[Math.Abs(data.Stride) * data.Height];
            int pixelSize = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            int offset = Math.Abs(data.Stride) - data.Width * pixelSize;
            Marshal.Copy(data.Scan0, argb, 0, argb.Length);
            for(int i = 0, j = 0; i < argb.Length; i += pixelSize, j++) {
                if(j > 0 && j % data.Width == 0) {
                    i += offset;
                }

                argb[i + 3] = (byte) Math.Floor(argb[i + 3] * alpha);
            }
            Marshal.Copy(argb, 0, data.Scan0, argb.Length);

            canvas.UnlockBits(data);

            return canvas;
        }

        /// <summary>
        /// Make the blank and white version of this image.
        /// </summary>
        /// <param name="image">The image want to be set transparency.</param>
        /// <returns>A new image which is blank and white.</returns>
        public static Image MakeBlankAndWhite(this Image image) {
            Bitmap canvas = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using(Graphics g = Graphics.FromImage(canvas)) {
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            BitmapData data = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), ImageLockMode.ReadWrite, canvas.PixelFormat);
            byte[] argb = new byte[Math.Abs(data.Stride) * data.Height];
            int pixelSize = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            int offset = Math.Abs(data.Stride) - data.Width * pixelSize;
            Marshal.Copy(data.Scan0, argb, 0, argb.Length);
            for(int i = 0, j = 0; i < argb.Length; i += pixelSize, j++) {
                if(j > 0 && j % data.Width == 0) {
                    i += offset;
                }

                argb[i] = argb[i + 1] = argb[i + 2] = (byte) ((argb[i] + argb[i + 1] + argb[i + 2]) / 3);
            }
            Marshal.Copy(argb, 0, data.Scan0, argb.Length);

            canvas.UnlockBits(data);

            return canvas;
        }

        /// <summary>
        /// Make the specified color to transparent of this image.
        /// </summary>
        /// <param name="image">The image want to be make transparent.</param>
        /// <param name="color">A color. The pixel will to be transparent if it is near this color.</param>
        /// <param name="delta">The allow error used to calculate the color adjacency.</param>
        /// <returns>A new image.</returns>
        /// <exception cref="System.ArgumentException">The absolute value of <paramref name="delta"/> is greater than 255.</exception>
        public static Image MakeTransparent(this Image image, Color color, int delta) {
            if(Math.Abs(delta = Math.Abs(delta)) > 255) {
                throw new ArgumentException("The absolute value of allowed error must be not greater than 255.", "delta");
            }

            Bitmap canvas = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using(Graphics graphics = Graphics.FromImage(canvas)) {
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            int r = color.R, g = color.G, b = color.B;
            BitmapData data = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), ImageLockMode.ReadWrite, canvas.PixelFormat);
            byte[] argb = new byte[Math.Abs(data.Stride) * data.Height];
            int pixelSize = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            int offset = Math.Abs(data.Stride) - data.Width * pixelSize;
            Marshal.Copy(data.Scan0, argb, 0, argb.Length);
            for(int i = 0, j = 0; i < argb.Length; i += pixelSize, j++) {
                if(j > 0 && j % data.Width == 0) {
                    i += offset;
                }

                if(Math.Abs(argb[i] - r) <= delta &&
                  Math.Abs(argb[i + 1] - g) <= delta &&
                  Math.Abs(argb[i + 2] - b) <= delta) {
                    argb[i + 3] = 0x00;
                }
            }
            Marshal.Copy(argb, 0, data.Scan0, argb.Length);

            canvas.UnlockBits(data);

            return canvas;
        }

        /// <summary>
        /// Set a watermark for this image.
        /// </summary>
        /// <param name="image">A image object.</param>
        /// <param name="watermark">The watermark will be seted.</param>
        /// <returns>A new image with the specified watermark.</returns>
        /// <exception cref="System.ArgumentNullException">watermark is null.</exception>
        public static Image Watermark(this Image image, Watermark watermark) {
            if(watermark == null) {
                throw new ArgumentNullException("watermark", "watermark is null.");
            }

            return watermark.Mark(image);
        }

        /// <summary>
        /// Set a watermark for this image.
        /// </summary>
        /// <param name="image">A image object.</param>
        /// <param name="watermark">The watermark will be seted.</param>
        /// <param name="position">The position of watermark.</param>
        /// <returns>A new image with the specified watermark.</returns>
        /// <exception cref="System.ArgumentNullException">watermark is null.</exception>
        public static Image Watermark(this Image image, Watermark watermark, WatermarkPosition position) {
            if(watermark == null) {
                throw new ArgumentNullException("watermark", "watermark is null.");
            }

            return watermark.Mark(image, position);
        }

        /// <summary>
        /// Saves this image to a file of the specified path.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="path"></param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        public static void SaveTo(this Image image, string path) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("path is null or empty.", "path");
            }

            string folder = Path.GetDirectoryName(path);
            if(!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            image.Save(path, GetImageFormat(Path.GetExtension(path)) ?? image.RawFormat);
        }

        /// <summary>
        /// Saves this image to a file of the specified path.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="path"></param>
        /// <param name="encoder"></param>
        /// <param name="parameters"></param>
        public static void SaveTo(this Image image, string path, ImageCodecInfo encoder, EncoderParameters parameters) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("path is null or empty.", "path");
            }

            string folder = Path.GetDirectoryName(path);
            if(!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            image.Save(path, encoder, parameters);
        }

        /// <summary>
        /// Determines whether the specified path represents a image file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Return true if <paramref name="path"/> represents a image file, otherwise return false.</returns>
        public static bool IsImage(string path) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("path is null or empty.", "path");
            }
            if(!PathUtility.IsValidLocalPath(path)) {
                throw new ArgumentException("path is not a valid file path.", "path");
            }
            if(!File.Exists(path = Path.GetFullPath(path))) {
                throw new FileNotFoundException("path is not existing.");
            }

            bool result = false;

            try {
                using(Image image = Image.FromFile(path)) {
                    result = true;
                }
            } catch {
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified data represents a image.
        /// </summary>
        /// <param name="data">The data will be check</param>
        /// <returns>Return true if <paramref name="data"/> represents a image, otherwise return false.</returns>
        public static bool IsImage(byte[] data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }

            if(data.Length == 0) {
                return false;
            }

            bool result = false;

            using(MemoryStream reader = new MemoryStream(data, false)) {
                try {
                    using(Image image = Image.FromStream(reader)) {
                        result = true;
                    }
                } catch {
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the ImageFormat from the specified file extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="extension"/> is null or empty.</exception>
        public static ImageFormat GetImageFormat(string extension) {
            extension = PrepareExtension(extension);
            switch(extension.ToLower()) {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                case ".ico":
                case ".icon":
                    return ImageFormat.Icon;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the MIME type from the specified file extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetMimeType(string extension) {
            extension = PrepareExtension(extension);
            return HttpHelper.GetContentType(extension);
        }

        /// <summary>
        /// Gets the image encoder from the specified file extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageEncoder(string extension) {
            extension = PrepareExtension(extension);
            string mimeType = HttpHelper.GetContentType(extension);
            return ImageCodecInfo.GetImageEncoders().Where((item) => string.Equals(item.MimeType, mimeType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the image encoder from the specified image format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageEncoder(ImageFormat format) {
            return ImageCodecInfo.GetImageEncoders().Where((item) => item.FormatID.Equals(format.Guid)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the image decoder from the specified file extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageDecoder(string extension) {
            extension = PrepareExtension(extension);
            string mimeType = HttpHelper.GetContentType(extension);
            return ImageCodecInfo.GetImageDecoders().Where((item) => string.Equals(item.MimeType, mimeType, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the image decoder from the specified image format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageDecoder(ImageFormat format) {
            return ImageCodecInfo.GetImageDecoders().Where((item) => item.FormatID.Equals(format.Guid)).FirstOrDefault();
        }
    }
}
