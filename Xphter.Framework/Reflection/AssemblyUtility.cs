using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Reflection {
    /// <summary>
    /// Provides functions to access a Assembly object.
    /// </summary>
    public static class AssemblyUtility {
        /// <summary>
        /// Searche all types which has implemented the specifed interface type in this assembly.
        /// </summary>
        /// <typeparam name="I">Interface type.</typeparam>
        /// <param name="assembly">A assembly object.</param>
        /// <returns>Return a type collection.</returns>
        /// <exception cref="System.ArgumentException"><typeparamref name="I"/> is not a interface type.</exception>
        public static ICollection<Type> SearchImplementation<I>(this Assembly assembly) where I : class {
            if(!typeof(I).IsInterface) {
                throw new ArgumentException("I is not a interface type.", "I");
            }

            ICollection<Type> types = new List<Type>();
            foreach(Type type in assembly.GetTypes()) {
                if(!type.IsVisible) {
                    continue;
                }

                if(type.IsImplements<I>()) {
                    types.Add(type);
                }
            }

            return types;
        }

        /// <summary>
        /// Searche all types which has inherited from the specifed class type in this assembly.
        /// </summary>
        /// <typeparam name="I">Class type.</typeparam>
        /// <param name="assembly">A assembly object.</param>
        /// <returns>Return a type collection.</returns>
        /// <exception cref="System.ArgumentException"><typeparamref name="T"/> is a interface type.</exception>
        public static ICollection<Type> SearchOffspring<T>(this Assembly assembly) where T : class {
            if(typeof(T).IsInterface) {
                throw new ArgumentException("T is a interface type.", "T");
            }

            ICollection<Type> types = new List<Type>();
            foreach(Type type in assembly.GetTypes()) {
                if(!type.IsVisible) {
                    continue;
                }

                if(type.IsInherits<T>()) {
                    types.Add(type);
                }
            }

            return types;
        }

        /// <summary>
        /// Gets the types defined in this assembly.
        /// </summary>
        /// <param name="assembly">A assembly.</param>
        /// <param name="throwOnError">true to throw an exception if error occurred when find types.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(this Assembly assembly, bool throwOnError) {
            IEnumerable<Type> types = null;

            if(throwOnError) {
                types = assembly.GetTypes();
            } else {
                try {
                    types = assembly.GetTypes();
                } catch(ReflectionTypeLoadException ex) {
                    types = ex.Types.Where((item) => item != null);
                }
            }

            return types;
        }
    }
}
