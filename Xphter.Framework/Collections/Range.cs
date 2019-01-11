using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a range of integer numbers.
    /// </summary>
    public class Range : IEnumerable<long>, IComparable<Range> {
        /// <summary>
        /// Initialize a new instance of Range class.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="length">Length.</param>
        public Range(long start, long length) {
            this.m_startIndex = start;
            this.m_length = Math.Max(0, length);
            this.m_endIndex = this.m_startIndex + this.m_length - 1;
            long hashCode = this.m_startIndex * 13 + this.m_length;
            this.m_hashCode = (hashCode >= int.MinValue && hashCode <= int.MaxValue ? Convert.ToInt32(hashCode) : 0);
        }

        /// <summary>
        /// Start index.
        /// </summary>
        private long m_startIndex;

        /// <summary>
        /// Range length.
        /// </summary>
        private long m_length;

        /// <summary>
        /// End index.
        /// </summary>
        private long m_endIndex;

        /// <summary>
        /// Hash code.
        /// </summary>
        private int m_hashCode;

        /// <summary>
        /// Gets start index of this range.
        /// </summary>
        public long StartIndex {
            get {
                return this.m_startIndex;
            }
        }

        /// <summary>
        /// Gets end index of this range.
        /// </summary>
        public long EndIndex {
            get {
                return this.m_endIndex;
            }
        }

        /// <summary>
        /// Gets length of this range.
        /// </summary>
        public long Length {
            get {
                return this.m_length;
            }
        }

        #region Range.Empty Property

        private class EmptyInstanceContainer {
            static EmptyInstanceContainer() {
                Instance = new Range(0, 0);
            }

            public static readonly Range Instance;
        }

        /// <summary>
        /// Gets the empty range instance.
        /// </summary>
        public static Range Empty {
            get {
                return EmptyInstanceContainer.Instance;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc />
        public IEnumerator<long> GetEnumerator() {
            for(long i = 0; i < this.m_length; i++) {
                yield return this.m_startIndex + i;
            }
            yield break;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Computes the intersection of this range and the specified range.
        /// </summary>
        /// <param name="range">A range.</param>
        /// <returns>Intersection of this range and <paramref name="range"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="range"/> is null.</exception>
        public Range Intersect(Range range) {
            if(range == null) {
                throw new ArgumentException("Target range is null.", "range");
            }
            if(this.m_length == 0 ||
              range.m_length == 0 ||
              this.m_endIndex < range.m_startIndex ||
              this.m_startIndex > range.m_endIndex) {
                return Range.Empty;
            }

            long start = Math.Max(this.m_startIndex, range.m_startIndex);
            long end = Math.Min(this.m_endIndex, range.m_endIndex);
            return new Range(start, end - start + 1);
        }

        /// <summary>
        /// Determines whether this range contains the specified range.
        /// </summary>
        /// <param name="range">A range.</param>
        /// <returns>Return true if this range contains <paramref name="range"/> otherwise return false.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="range"/> is null.</exception>
        public bool Contains(Range range) {
            if(range == null) {
                throw new ArgumentException("Target range is null.", "range");
            }
            if(this.m_length == 0 ||
              range.m_length == 0) {
                return false;
            }

            return this.m_startIndex <= range.m_startIndex && this.m_endIndex >= range.m_endIndex;
        }

        /// <summary>
        /// Determines whether this range contains the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Contains(long index) {
            return this.StartIndex <= index && index <= this.EndIndex;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.m_hashCode;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(Object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is Range)) {
                return false;
            }
            Range range = (Range) obj;
            return this.m_startIndex == range.m_startIndex && this.m_length == range.m_length;
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.m_length > 0 ? string.Format("{0} ~ {1}", this.m_startIndex, this.m_endIndex) : "Empty";
        }

        #endregion

        #region IComparable Members

        /// <inheritdoc />
        public int CompareTo(Range other) {
            if(other == null) {
                return 1;
            }

            int result = 0;

            if(this.m_startIndex < other.m_startIndex) {
                result = -1;
            } else if(this.m_startIndex == other.m_startIndex) {
                if(this.m_length < other.m_length) {
                    result = -1;
                } else if(this.m_length == other.m_length) {
                    result = 0;
                } else {
                    result = 1;
                }
            } else {
                result = 1;
            }

            return result;
        }

        #endregion
    }
}
