using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a child elements collection.
    /// </summary>
    /// <typeparam name="PT">The type of parent element.</typeparam>
    /// <typeparam name="CT">The type of child element.</typeparam>
    [Serializable]
    public class ChildCollection<PT, CT> : ICollection<CT> {
        /// <summary>
        /// Initialize a new instance of ChildCollection class.
        /// </summary>
        /// <param name="parent">The parent element which can not is null.</param>
        /// <exception cref="System.ArgumentException"><paramref name="parent"/> is null.</exception>
        public ChildCollection(PT parent)
            : this(parent, null, null) {
        }

        /// <summary>
        /// Initialize a new instance of ChildCollection class.
        /// </summary>
        /// <param name="parent">The parent element which can not is null.</param>
        /// <param name="addAction">The executed action when add a child element.</param>
        /// <param name="removeAction">The executed action when remove a child element.</param>
        /// <exception cref="System.ArgumentException"><paramref name="parent"/> is null.</exception>
        public ChildCollection(PT parent, Action<PT, CT> addAction, Action<PT, CT> removeAction) {
            if(parent == null) {
                throw new ArgumentException("Parent element is null.", "parent");
            }

            this.m_parent = parent;
            this.m_addAction = addAction;
            this.m_removeAction = removeAction;
        }

        /// <summary>
        /// The parent element.
        /// </summary>
        protected PT m_parent;

        /// <summary>
        /// The child element collection.
        /// </summary>
        protected List<CT> m_children = new List<CT>();

        /// <summary>
        /// The executed action when add/remove child element.
        /// </summary>
        protected Action<PT, CT> m_addAction;
        protected Action<PT, CT> m_removeAction;

        #region ICollection<CT> Members

        public virtual void Add(CT item) {
            this.m_children.Add(item);
            if(item != null && this.m_addAction != null) {
                this.m_addAction(this.m_parent, item);
            }
        }

        public virtual void Clear() {
            ICollection<CT> items = null;
            if(this.m_removeAction != null) {
                items = new List<CT>(this.m_children);
            }

            this.m_children.Clear();
            if(items != null) {
                foreach(CT item in items) {
                    if(item != null) {
                        this.m_removeAction(this.m_parent, item);
                    }
                }
                items.Clear();
                items = null;
            }
        }

        public virtual bool Contains(CT item) {
            return this.m_children.Contains(item);
        }

        public virtual void CopyTo(CT[] array, int arrayIndex) {
            this.m_children.CopyTo(array, arrayIndex);
        }

        public virtual int Count {
            get {
                return this.m_children.Count;
            }
        }

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        public virtual bool Remove(CT item) {
            bool result = this.m_children.Remove(item);
            if(result && item != null && this.m_removeAction != null) {
                this.m_removeAction(this.m_parent, item);
            }
            return result;
        }

        #endregion

        #region IEnumerable<CT> Members

        public virtual IEnumerator<CT> GetEnumerator() {
            return this.m_children.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.m_children.GetEnumerator();
        }

        #endregion
    }
}
