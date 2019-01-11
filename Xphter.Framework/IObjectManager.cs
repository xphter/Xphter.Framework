using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides functions to register and get some objects.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    public interface IObjectManager<T> where T : class {
        /// <summary>
        /// Gets all registered objects.
        /// </summary>
        IEnumerable<T> Objects {
            get;
        }

        /// <summary>
        /// Registers objects from the specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        void Register(params Assembly[] assemblies);

        /// <summary>
        /// Registers objects from the specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        void Register(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Registers the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="obj"/> is null.</exception>
        void Register(T obj);
    }
}
