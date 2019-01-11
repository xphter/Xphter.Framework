using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xphter.Framework.Reflection;

namespace Xphter.Framework {
    /// <summary>
    /// Provides a singleton implementation of IObjectManager interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectManager<T> : IObjectManager<T> where T : class {
        #region Singleton

        /// <summary>
        /// Initialize a new instance of ObjectManager class.
        /// </summary>
        protected ObjectManager() {
            this.m_objects = new List<T>();
        }

        private static class SingletonContainer {
            static SingletonContainer() {
                Instance = new ObjectManager<T>();
            }

            public static ObjectManager<T> Instance;
        }

        /// <summary>
        /// Gets the unique instance of ObjectManager class.
        /// </summary>
        public static ObjectManager<T> Instance {
            get {
                return SingletonContainer.Instance;
            }
        }

        #endregion

        protected ICollection<T> m_objects;

        #region IObjectManager<T> Members

        /// <inheritdoc />
        public virtual IEnumerable<T> Objects {
            get {
                return this.m_objects;
            }
        }

        /// <inheritdoc />
        public virtual void Register(params Assembly[] assemblies) {
            foreach(T item in TypeUtility.LoadInstances<T>(assemblies)) {
                this.Register(item);
            }
        }

        /// <inheritdoc />
        public virtual void Register(IEnumerable<Assembly> assemblies) {
            foreach(T item in TypeUtility.LoadInstances<T>(assemblies)) {
                this.Register(item);
            }
        }

        /// <inheritdoc />
        public virtual void Register(T obj) {
            if(obj == null) {
                throw new ArgumentNullException("obj");
            }

            if(!this.m_objects.Contains(obj)) {
                this.m_objects.Add(obj);
            }
        }

        #endregion
    }
}
