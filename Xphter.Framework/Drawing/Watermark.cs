using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Xphter.Framework.IO;

namespace Xphter.Framework.Drawing {
    /// <summary>
    /// Represents a image watermark.
    /// </summary>
    public class Watermark : IDisposable {
        /// <summary>
        /// Initialize a new instance of Watermark class(Image watermark).
        /// </summary>
        /// <param name="imagePath">The image file path.</param>
        /// <param name="opacity">The opacity of this watermark.</param>
        /// <exception cref="System.ArgumentException"><paramref name="imageFile"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="imageFile"/> is not a valid file path.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="imageFile"/> is not existing.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="imageFile"/> not represents a valid image file.</exception>
        public Watermark(string imagePath, float opacity) {
            //bounds checking
            if(string.IsNullOrWhiteSpace(imagePath)) {
                throw new ArgumentException("imagePath is null or empty.", "imageFile");
            }
            if(!PathUtility.IsValidLocalPath(imagePath)) {
                throw new ArgumentException("imagePath is not a valid file path.", "imageFile");
            }
            if(!File.Exists(Path.GetFullPath(imagePath))) {
                throw new FileNotFoundException("imagePath is not existing.");
            }
            if(!ImageUtility.IsImage(imagePath)) {
                throw new ArgumentException("imagePath not represents a valid image file.", "imageFile");
            }

            using(Image image = Image.FromFile(imagePath)) {
                this.Initialize(image, opacity, null, null);
            }
        }

        /// <summary>
        /// Initialize a new instance of Watermark class(Image watermark).
        /// </summary>
        /// <param name="imagePath">The image file path.</param>
        /// <param name="opacity">The opacity of this watermark.</param>
        /// <param name="width">The width of this watermark.</param>
        /// <param name="height">The height of this watermark.</param>
        /// <exception cref="System.ArgumentException"><paramref name="imageFile"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="imageFile"/> is not a valid file path.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="imageFile"/> is not existing.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="imageFile"/> not represents a valid image file.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="width"/> is equal or less than zero.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="height"/> is equal or less than zero.</exception>
        public Watermark(string imagePath, float opacity, int width, int height) {
            //bounds checking
            if(string.IsNullOrWhiteSpace(imagePath)) {
                throw new ArgumentException("imagePath is null or empty.", "imageFile");
            }
            if(!PathUtility.IsValidLocalPath(imagePath)) {
                throw new ArgumentException("imagePath is not a valid file path.", "imageFile");
            }
            if(!File.Exists(Path.GetFullPath(imagePath))) {
                throw new FileNotFoundException("imagePath is not existing.");
            }
            if(!ImageUtility.IsImage(imagePath)) {
                throw new ArgumentException("imagePath not represents a valid image file.", "imageFile");
            }
            if(width <= 0) {
                throw new ArgumentException("width is equal or less than zero.", "width");
            }
            if(height <= 0) {
                throw new ArgumentException("height is equal or less than zero.", "height");
            }

            using(Image image = Image.FromFile(imagePath)) {
                this.Initialize(image, opacity, width, height);
            }
        }

        /// <summary>
        /// Initialize a new instance of Watermark class(Image watermark).
        /// </summary>
        /// <param name="image">The watermark image.</param>
        /// <param name="opacity">The opacity of this watermark.</param>
        /// <exception cref="System.ArgumentException"><paramref name="image"/> is null.</exception>
        public Watermark(Image image, float opacity) {
            //bounds checking
            if(image == null) {
                throw new ArgumentException("image is null.", "image");
            }

            this.Initialize(image, opacity, null, null);
        }

        /// <summary>
        /// Initialize a new instance of Watermark class(Image watermark).
        /// </summary>
        /// <param name="image">The watermark image.</param>
        /// <param name="opacity">The opacity of this watermark.</param>
        /// <param name="width">The width of this watermark.</param>
        /// <param name="height">The height of this watermark.</param>
        /// <exception cref="System.ArgumentException"><paramref name="image"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="width"/> is equal or less than zero.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="height"/> is equal or less than zero.</exception>
        public Watermark(Image image, float opacity, int width, int height) {
            //bounds checking
            if(image == null) {
                throw new ArgumentException("image is null.", "imageFile");
            }
            if(width <= 0) {
                throw new ArgumentException("width is equal or less than zero.", "width");
            }
            if(height <= 0) {
                throw new ArgumentException("height is equal or less than zero.", "height");
            }

            this.Initialize(image, opacity, width, height);
        }

        /// <summary>
        /// Initialize a new instance of Watermark(Text Watermark).
        /// </summary>
        /// <param name="text">The text will be marked.</param>
        /// <param name="font">The font used to mark text.</param>
        /// <param name="opacity">Opacity.</param>
        /// <exception cref="System.ArgumentException"><paramref name="text"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="font"/> is null.</exception>
        public Watermark(string text, Font font, Color color, float opacity) {
            if(string.IsNullOrWhiteSpace(text)) {
                throw new ArgumentException("Text is null or empty.", "text");
            }
            if(font == null) {
                throw new ArgumentException("Font is null.", "font");
            }

            this.Initialize(text, font, color, opacity);
        }

        /// <summary>
        /// The image of this watermark.
        /// </summary>
        private Image m_markImage;

        /// <summary>
        /// The event to lock <see cref="m_markImage"/>.
        /// </summary>
        private AutoResetEvent m_lock;

        /// <summary>
        /// Gets the opacity of this watermark.
        /// </summary>
        public float Opacity {
            get;
            private set;
        }

        /// <summary>
        /// Gets the width of this watermark.
        /// </summary>
        public int Width {
            get;
            private set;
        }

        /// <summary>
        /// Gets the height of this watermark.
        /// </summary>
        public int Height {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the position of this watermark.
        /// </summary>
        public WatermarkPosition Position {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the margin of this watermark.
        /// </summary>
        public Point Margin {
            get;
            set;
        }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="text">Watermark text.</param>
        /// <param name="font">Font.</param>
        /// <param name="color">Text color.</param>
        /// <param name="opacity">Opacity.</param>
        private void Initialize(string text, Font font, Color color, float opacity) {
            //create watermark image
            Size size = TextRenderer.MeasureText(text, font);
            using(Image image = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb)) {
                using(Graphics g = Graphics.FromImage(image)) {
                    g.Clear(Color.Transparent);
                    using(Brush brush = new SolidBrush(color)) {
                        g.DrawString(text, font, brush, 0, 0);
                    }
                }

                this.Initialize(image, opacity, size.Width, size.Height);
            }
        }

        /// <summary>
        /// Initialize this object.
        /// </summary>
        /// <param name="image">Watermark image.</param>
        /// <param name="opacity">Opacity.</param>
        /// <param name="width">Watermark width.</param>
        /// <param name="height">Watermark height.</param>
        private void Initialize(Image image, float opacity, int? width, int? height) {
            //initialize options
            this.Opacity = Math.Max(0, Math.Min(1, opacity));
            this.Width = width ?? image.Width;
            this.Height = height ?? image.Height;

            //create watermark image
            if(this.Opacity < 1) {
                using(Image scaledImage = image.Scale(this.Width, this.Height)) {
                    this.m_markImage = scaledImage.Transparency(this.Opacity);
                }
            } else {
                this.m_markImage = image.Scale(this.Width, this.Height);
            }
            this.m_lock = new AutoResetEvent(true);
        }

        /// <summary>
        /// Mark this watermark to the specified image.
        /// </summary>
        /// <param name="image">The image want to be marked.</param>
        /// <returns>A new image which has been marked by this watermark.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="image"/> is null.</exception>
        public Image Mark(Image image) {
            return this.Mark(image, this.Position);
        }

        /// <summary>
        /// Mark this watermark to the specified image.
        /// </summary>
        /// <param name="image">The image want to be marked.</param>
        /// <param name="position">The position of watermark.</param>
        /// <returns>A new image which has been marked by this watermark.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="image"/> is null.</exception>
        public Image Mark(Image image, WatermarkPosition position) {
            if(image == null) {
                throw new ArgumentException("image is null.", "image");
            }

            //determines position of watermark
            int x = 0, y = 0;
            switch(this.Position = position) {
                case WatermarkPosition.LeftTop:
                    x = this.Margin.X;
                    y = this.Margin.Y;
                    break;
                case WatermarkPosition.LeftCenter:
                    x = this.Margin.X;
                    y = (image.Height - this.Height) / 2;
                    break;
                case WatermarkPosition.LeftBottom:
                    x = this.Margin.X;
                    y = image.Height - this.Height - this.Margin.Y;
                    break;
                case WatermarkPosition.CenterTop:
                    x = (image.Width - this.Width) / 2;
                    y = this.Margin.Y;
                    break;
                case WatermarkPosition.Center:
                    x = (image.Width - this.Width) / 2;
                    y = (image.Height - this.Height) / 2;
                    break;
                case WatermarkPosition.CenterBottom:
                    x = (image.Width - this.Width) / 2;
                    y = image.Height - this.Height - this.Margin.Y;
                    break;
                case WatermarkPosition.RightTop:
                    x = image.Width - this.Width - this.Margin.X;
                    y = this.Margin.Y;
                    break;
                case WatermarkPosition.RightCenter:
                    x = image.Width - this.Width - this.Margin.X;
                    y = (image.Height - this.Height) / 2;
                    break;
                case WatermarkPosition.RightBottom:
                    x = image.Width - this.Width - this.Margin.X;
                    y = image.Height - this.Height - this.Margin.Y;
                    break;
            }

            //draw watermark
            Bitmap canvas = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using(Graphics g = Graphics.FromImage(canvas)) {
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, image.Width, image.Height);

                this.m_lock.WaitOne();
                g.DrawImage(this.m_markImage, x, y, this.Width, this.Height);
                this.m_lock.Set();
            }

            return canvas;
        }

        #region IDisposable Members

        ~Watermark() {
            this.Dispose();
        }

        protected bool m_isDisposed;

        /// <inheritdoc />
        public void Dispose() {
            if(this.m_isDisposed) {
                return;
            }

            this.m_markImage.Dispose();
            this.m_lock.Dispose();
            this.m_isDisposed = true;
        }

        #endregion
    }

    /// <summary>
    /// Represents the position of watermark.
    /// </summary>
    public enum WatermarkPosition {
        [Description("左上")]
        LeftTop,

        [Description("左中")]
        LeftCenter,

        [Description("左下")]
        LeftBottom,

        [Description("中上")]
        CenterTop,

        [Description("居中")]
        Center,

        [Description("中下")]
        CenterBottom,

        [Description("右上")]
        RightTop,

        [Description("右中")]
        RightCenter,

        [Description("右下")]
        RightBottom,
    }
}
