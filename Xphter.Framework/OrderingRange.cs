using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// 表示一个正序的范围
    /// </summary>
    /// <typeparam name="T">范围内对象的类型</typeparam>
    /// <exception cref="System.ArgumentOutOfRangeException">start大于end</exception>
    /// <exception cref="System.ArgumentNullException">converter为NULL</exception>
    public class OrderingRange<T> : IEnumerable<T> where T : IComparable<T> {
        public OrderingRange(T start, T end, Func<T, T> converter) {
            if(typeof(T).IsValueType) {
                if(start.CompareTo(end) > 0) {
                    throw new ArgumentOutOfRangeException("start", "start大于end");
                }
            } else {
                if(start != null && start.CompareTo(end) > 0) {
                    throw new ArgumentOutOfRangeException("start", "start大于end");
                }
            }
            if(converter == null) {
                throw new ArgumentNullException("converter", "converter为NULL");
            }

            this.Start = start;
            this.End = end;
            this.m_converter = converter;
        }

        /// <summary>
        /// 获取开始元素。
        /// </summary>
        public T Start {
            get;
            private set;
        }

        /// <summary>
        /// 获取结束元素。
        /// </summary>
        public T End {
            get;
            private set;
        }

        /// <summary>
        /// 根据当前元素获取下一个元素的转换器。
        /// </summary>
        private Func<T, T> m_converter;

        #region IEnumerable<T> Members

        /// <inherit />
        public IEnumerator<T> GetEnumerator() {
            T item = this.Start;

            if(typeof(T).IsValueType) {
                while(item.CompareTo(this.End) <= 0) {
                    yield return item;

                    item = this.m_converter(item);
                }
            } else {
                while(item == null || item.CompareTo(this.End) <= 0) {
                    yield return item;

                    item = this.m_converter(item);
                }
            }
            yield break;
        }

        #endregion

        #region IEnumerable Members

        /// <inherit />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion
    }
}
