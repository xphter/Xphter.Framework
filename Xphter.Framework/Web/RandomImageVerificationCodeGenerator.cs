using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Uses some random options to generate verification code.
    /// </summary>
    public class RandomImageVerificationCodeGenerator : IImageVerificationCodeGenerator {
        /// <summary>
        /// Initialize a instance of RandomImageVerificationCodeGenerator class.
        /// </summary>
        /// <param name="option"></param>
        public RandomImageVerificationCodeGenerator(IImageVerificationCodeValueProvider valueProvider, RandomImageVerificationCodeOption option) {
            if(valueProvider == null) {
                throw new ArgumentNullException("valueProvider");
            }
            if(option == null) {
                throw new ArgumentNullException("option");
            }

            this.m_valueProvider = valueProvider;
            this.m_random = option.RandomNumber ?? new Random(Environment.TickCount);
            this.m_backgroundColors = option.BackgroundColors != null ? new List<Color>(option.BackgroundColors) : new List<Color>(0);
            this.m_foregroundColors = option.ForegroundColors != null ? new List<Color>(option.ForegroundColors) : new List<Color>(0);
            this.m_canvasWidth = option.CanvasWidth != null ? new List<int>(option.CanvasWidth.Select((item) => (int) item)) : new List<int>(0);
            this.m_canvasHeight = option.CanvasHeight != null ? new List<int>(option.CanvasHeight.Select((item) => (int) item)) : new List<int>(0);
            this.m_fontSize = option.FontSize != null ? new List<int>(option.FontSize.Select((item) => (int) item)) : new List<int>(0);
            this.m_fontFamilies = option.FontFamilies != null ? new List<FontFamily>(option.FontFamilies) : new List<FontFamily>(0);
            this.m_fontStyles = option.FontStyles != null ? new List<FontStyle>(option.FontStyles) : new List<FontStyle>(0);
            this.m_maxRotationDegree = Math.Max(DEFAULT_ROTATION_DEGREE, option.MaxRotationDegree);
            this.m_noisesTimes = Math.Max(DEFAULT_NOISES_TIMES, option.NoisesTimes);
            this.m_throughLinesCount = Math.Max(DEFAULT_THROUGH_LINES_COUNT, option.ThroughLinesCount);
        }

        private const int DEFAULT_ROTATION_DEGREE = 0;
        private const int DEFAULT_NOISES_TIMES = 10;
        private const int DEFAULT_THROUGH_LINES_COUNT = 2;

        #region Options

        protected IImageVerificationCodeValueProvider m_valueProvider;
        protected Random m_random;
        protected IList<Color> m_backgroundColors;
        protected IList<Color> m_foregroundColors;
        protected IList<int> m_canvasWidth;
        protected IList<int> m_canvasHeight;
        protected IList<int> m_fontSize;
        protected IList<FontFamily> m_fontFamilies;
        protected IList<FontStyle> m_fontStyles;
        protected float m_maxRotationDegree;
        protected int m_noisesTimes;
        protected int m_throughLinesCount;

        #endregion

        protected virtual T GetRandomValue<T>(IList<T> range) {
            if(range.Count == 0) {
                return default(T);
            }
            if(range.Count == 1) {
                return range[0];
            }

            return range[this.m_random.Next(0, range.Count)];
        }

        protected virtual Color GetBackgroundColor() {
            return this.GetRandomValue<Color>(this.m_backgroundColors);
        }

        protected virtual Color GetForegroundColor() {
            return this.GetRandomValue<Color>(this.m_foregroundColors);
        }

        protected virtual int GetCanvasWidth() {
            return this.GetRandomValue<int>(this.m_canvasWidth);
        }

        protected virtual int GetCanvasHeight() {
            return this.GetRandomValue<int>(this.m_canvasHeight);
        }

        protected virtual int GetFontSize() {
            return this.GetRandomValue<int>(this.m_fontSize);
        }

        protected virtual FontFamily GetFontFamily() {
            return this.GetRandomValue<FontFamily>(this.m_fontFamilies) ?? FontFamily.GenericSansSerif;
        }

        protected virtual FontStyle GetFontStyle(FontFamily family) {
            FontStyle style = this.GetRandomValue<FontStyle>(this.m_fontStyles);

            if(!family.IsStyleAvailable(style)) {
                foreach(FontStyle fs in Enum.GetValues(typeof(FontStyle))) {
                    if(family.IsStyleAvailable(fs)) {
                        style = fs;
                        break;
                    }
                }
            }

            return style;
        }

        protected virtual Size GetCharacterSize(char c, Graphics g, Font font) {
            SizeF size = g.MeasureString(c.ToString(), font, PointF.Empty, StringFormat.GenericTypographic);
            return new Size((int) size.Width, font.Height);
        }

        protected virtual Matrix GetTransformMatrix(Rectangle bounds, Size charSize) {
            Matrix matrix = new Matrix();

            if(this.m_maxRotationDegree != 0) {
                matrix.RotateAt((float) (this.m_random.NextDouble() * 2 - 1) * this.m_maxRotationDegree, new PointF(bounds.X + charSize.Width / 2, bounds.Y + charSize.Height / 2), MatrixOrder.Append);
            }
            if(bounds.Width > charSize.Width) {
                if(bounds.X > 0) {
                    matrix.Translate(this.GetRandomValue<int>(new List<int>(new Range(-2 * (bounds.Width - charSize.Width), (bounds.Width - charSize.Width) * 4).Select((item) => (int) item))), 0, MatrixOrder.Append);
                } else {
                    matrix.Translate(this.GetRandomValue<int>(new List<int>(new Range(0, (bounds.Width - charSize.Width) * 2).Select((item) => (int) item))), 0, MatrixOrder.Append);
                }
            }
            if(bounds.Height > charSize.Height) {
                matrix.Translate(0, this.GetRandomValue<int>(new List<int>(new Range(0, bounds.Height - charSize.Height).Select((item) => (int) item))), MatrixOrder.Append);
            }

            return matrix;
        }

        protected virtual Brush GetBrush(Color foregroundColor) {
            return new SolidBrush(foregroundColor);
        }

        protected virtual ImageVerificationCode GenerateCode() {
            int canvasWidth = this.GetCanvasWidth();
            int canvasHeight = this.GetCanvasHeight();
            if(canvasWidth <= 0 || canvasHeight <= 0) {
                return null;
            }

            Color foregroundColor = Color.Empty;
            Color backgroundColor = this.GetBackgroundColor();
            IEnumerable<char> characters = this.m_valueProvider.GetValue();

            Font font = null;
            int fontSize = 0;
            FontFamily fontFamily = null;
            FontStyle fontStyle = FontStyle.Regular;

            int x = 0, y = 0;
            Matrix matrix = null;
            int width = canvasWidth / characters.Count();

            int noiseWidth = 2;
            Point noise = Point.Empty;
            List<Point> noises = new List<Point>(characters.Count() * this.m_noisesTimes);

            Point lineStart = Point.Empty;
            Point lineEnd = Point.Empty;

            Image canvas = new Bitmap(canvasWidth, canvasHeight, PixelFormat.Format32bppArgb);
            using(Graphics graphics = Graphics.FromImage(canvas)) {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(backgroundColor);

                // draw noises
                for(int i = 0; i < noises.Capacity; i++) {
                    do {
                        noise = new Point(this.m_random.Next(0, canvasWidth), this.m_random.Next(0, canvasHeight));
                    } while(noises.Contains(noise));

                    foregroundColor = this.GetForegroundColor();
                    using(Pen pen = new Pen(foregroundColor, noiseWidth)) {
                        graphics.DrawLine(pen, noise.X, noise.Y, noise.X + noiseWidth, noise.Y + noiseWidth);
                    }
                }

                // draw each charactor
                foreach(char c in characters) {
                    fontSize = this.GetFontSize();
                    fontFamily = this.GetFontFamily();
                    fontStyle = this.GetFontStyle(fontFamily);
                    foregroundColor = this.GetForegroundColor();

                    using(font = new Font(fontFamily, fontSize, fontStyle)) {
                        matrix = this.GetTransformMatrix(new Rectangle(x, y, width, canvasHeight), this.GetCharacterSize(c, graphics, font));

                        using(Brush brush = this.GetBrush(foregroundColor)) {
                            graphics.MultiplyTransform(matrix, MatrixOrder.Append);
                            graphics.DrawString(c.ToString(), font, brush, x, y);
                            graphics.ResetTransform();
                        }
                    }

                    x += width;
                }

                // draw through lines
                for(int i = 0; i < this.m_throughLinesCount; i++) {
                    foregroundColor = this.GetForegroundColor();
                    lineStart = new Point(0, this.m_random.Next(0, canvasHeight));
                    lineEnd = new Point(canvasWidth, this.m_random.Next(0, canvasHeight));

                    using(Pen pen = new Pen(foregroundColor, noiseWidth)) {
                        graphics.DrawLine(pen, lineStart, lineEnd);
                    }
                }
            }

            return new ImageVerificationCode(canvas, ImageFormat.Png, new string(characters.ToArray()), null);
        }

        #region IImageVerificationCodeGenerator Members

        public ImageVerificationCode Generate() {
            return this.GenerateCode();
        }

        #endregion
    }

    /// <summary>
    /// Provides options of RandomImageVerificationCodeGenerator class.
    /// </summary>
    public class RandomImageVerificationCodeOption {
        public Random RandomNumber {
            get;
            set;
        }

        public IEnumerable<Color> BackgroundColors {
            get;
            set;
        }

        public IEnumerable<Color> ForegroundColors {
            get;
            set;
        }

        public Range CanvasWidth {
            get;
            set;
        }

        public Range CanvasHeight {
            get;
            set;
        }

        public Range FontSize {
            get;
            set;
        }

        public IEnumerable<FontFamily> FontFamilies {
            get;
            set;
        }

        public IEnumerable<FontStyle> FontStyles {
            get;
            set;
        }

        public float MaxRotationDegree {
            get;
            set;
        }

        public int NoisesTimes {
            get;
            set;
        }

        public int ThroughLinesCount {
            get;
            set;
        }
    }
}
