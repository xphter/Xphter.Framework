using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a child elements list.
    /// </summary>
    /// <typeparam name="PT">The type of parent element.</typeparam>
    /// <typeparam name="CT">The type of child element.</typeparam>
    [Serializable]
    public class ChildList<PT, CT> : ChildCollection<PT, CT>, IList<CT> {
        /// <summary>
        /// Initialize a new instance of ChildCollection class.
        /// </summary>
        /// <param name="parent">The parent element which can not is null.</param>
        /// <exception cref="System.ArgumentException"><paramref name="parent"/> is null.</exception>
        public ChildList(PT parent)
            : base(parent) {
        }

        /// <summary>
        /// Initialize a new instance of ChildCollection class.
        /// </summary>
        /// <param name="parent">The parent element which can not is null.</param>
        /// <param name="addAction">The executed action when add a child element.</param>
        /// <param name="removeAction">The executed action when remove a child element.</param>
        /// <exception cref="System.ArgumentException"><paramref name="parent"/> is null.</exception>
        public ChildList(PT parent, Action<PT, CT> addAction, Action<PT, CT> removeAction)
            : base(parent, addAction, removeAction) {
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        //  predicate, and returns the first occurrence within the entire Xphter.Framework.Collections.ChildList<PT, CT>.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public CT Find(Predicate<CT> match) {
            return this.m_children.Find(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        //  predicate, and returns the zero-based index of the first occurrence within
        //  the entire Xphter.Framework.Collections.ChildList<PT, CT>.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int FindIndex(Predicate<CT> match) {
            return this.m_children.FindIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        //  predicate, and returns the last occurrence within the entire Xphter.Framework.Collections.ChildList<PT, CT>.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public CT FindLast(Predicate<CT> match) {
            return this.m_children.FindLast(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        //  predicate, and returns the zero-based index of the last occurrence within
        //  the entire Xphter.Framework.Collections.ChildList<PT, CT>.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int FindLastIndex(Predicate<CT> match) {
            return this.FindLastIndex(match);
        }

        #region IList<CT> Members

        /// <inheritdoc />
        public int IndexOf(CT item) {
            return this.m_children.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, CT item) {
            this.m_children.Insert(index, item);
            if(item != null && this.m_addAction != null) {
                this.m_addAction(this.m_parent, item);
            }
        }

        /// <inheritdoc />
        public void RemoveAt(int index) {
            CT item = this.m_children[index];
            this.m_children.RemoveAt(index);
            if(item != null && this.m_removeAction != null) {
                this.m_removeAction(this.m_parent, item);
            }
        }

        /// <inheritdoc />
        public CT this[int index] {
            get {
                return this.m_children[index];
            }
            set {
                this.m_children[index] = value;
                if(value != null && this.m_addAction != null) {
                    this.m_addAction(this.m_parent, value);
                }
            }
        }

        #endregion
    }
}
