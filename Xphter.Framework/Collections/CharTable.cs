using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a table of chars.
    /// </summary>
    public class CharTable : IEnumerable<char> {
        /// <summary>
        /// Initialize a new instance of CharTable class.
        /// </summary>
        /// <param name="reader"></param>
        public CharTable(TextReader reader)
            : this(reader.ReadToEnd()) {
        }

        /// <summary>
        /// Initialize a new instance of CharTable class.
        /// </summary>
        /// <param name="text"></param>
        public CharTable(string text) {
            this.Text = text;

            if(text != null) {
                IList<int> lines = new List<int>(g_lineRegex.Matches(text).Cast<Match>().Select((item) => item.Index));
                for(int i = 0; i < lines.Count; i++) {
                    this.m_lines[i] = new Range(lines[i], (i < lines.Count - 1 ? lines[i + 1] : text.Length) - lines[i]);
                }
            }
        }

        /// <summary>
        /// The regular expression used to parse all lines.
        /// </summary>
        private static Regex g_lineRegex = new Regex("^", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// The line data of a text, key is rowIndex, value is row range.
        /// </summary>
        protected IDictionary<int, Range> m_lines = new Dictionary<int, Range>();

        /// <summary>
        /// Gets the char at the specified row and column.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="rowIndex"/> is out of range.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="columnIndex"/> is out of range.</exception>
        public virtual char this[int rowIndex, int columnIndex] {
            get {
                if(rowIndex < 0 || rowIndex >= this.m_lines.Count) {
                    throw new ArgumentOutOfRangeException("rowIndex", "The row index is out of range.");
                }
                if(columnIndex < 0 || columnIndex >= this[rowIndex]) {
                    throw new ArgumentOutOfRangeException("columnIndex", "The column index is out of range.");
                }

                return this.Text[this.GetIndex(rowIndex, columnIndex)];
            }
        }

        /// <summary>
        /// Gets the number of chars in the specified row.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="rowIndex"/> is out of range.</exception>
        public virtual int this[int rowIndex] {
            get {
                if(rowIndex < 0 || rowIndex >= this.m_lines.Count) {
                    throw new ArgumentOutOfRangeException("rowIndex", "The row index is out of range.");
                }

                return Convert.ToInt32(this.m_lines[rowIndex].Length);
            }
        }

        /// <summary>
        /// Gets the under text.
        /// </summary>
        public virtual string Text {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public virtual int RowCount {
            get {
                return this.m_lines.Count;
            }
        }

        /// <summary>
        /// Gets the row index of the specifed position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public virtual int GetRowIndex(int index) {
            if(index < 0 || index >= this.Text.Length) {
                throw new ArgumentOutOfRangeException("index", "The index is out of range.");
            }

            return this.m_lines.Where((item) => item.Value.Contains(index)).First().Key;
        }

        /// <summary>
        /// Gets the column index of the specifed position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public virtual int GetColumnIndex(int index) {
            if(index < 0 || index >= this.Text.Length) {
                throw new ArgumentOutOfRangeException("index", "The index is out of range.");
            }

            Range range = this.m_lines.Where((item) => item.Value.Contains(index)).First().Value;
            return Convert.ToInt32(index - range.StartIndex);
        }

        /// <summary>
        /// Gets the position of in the specified row and column.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="rowIndex"/> is out of range.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="columnIndex"/> is out of range.</exception>
        public virtual int GetIndex(int rowIndex, int columnIndex) {
            if(rowIndex < 0 || rowIndex >= this.m_lines.Count) {
                throw new ArgumentOutOfRangeException("rowIndex", "The row index is out of range.");
            }
            if(columnIndex < 0 || columnIndex >= this[rowIndex]) {
                throw new ArgumentOutOfRangeException("columnIndex", "The column index is out of range.");
            }

            return Convert.ToInt32(this.m_lines[rowIndex].StartIndex + columnIndex);
        }

        #region IEnumerable<char> Members

        public IEnumerator<char> GetEnumerator() {
            return this.Text.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion
    }
}
