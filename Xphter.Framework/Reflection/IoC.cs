using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Xphter.Framework.Collections;
using Xphter.Framework.IO;

namespace Xphter.Framework.Reflection {
    /// <summary>
    /// Represents a factory to create objects.
    /// </summary>
    public interface IObjectFactory {
        /// <summary>
        /// Creates a object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object CreateInstance(Type type);

        /// <summary>
        /// Creates some objects of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        object[] CreateInstances(Type type, int count);

        /// <summary>
        /// Creates a object of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CreateInstance<T>();

        /// <summary>
        /// Creates some objects of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        T[] CreateInstances<T>(int count);
    }

    /// <summary>
    /// The exception that is thrown when IObjectFactory fail to create object.
    /// </summary>
    [Serializable]
    public class ObjectFacgoryException : Exception {
        public ObjectFacgoryException()
            : base() {
        }

        public ObjectFacgoryException(string message)
            : base(message) {
        }

        public ObjectFacgoryException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public ObjectFacgoryException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }

    /// <summary>
    /// Represents a IObjectFactory which creates object by a XML configuration file.
    /// </summary>
    public class XmlConfigurationObjectFactory : IObjectFactory {
        /// <summary>
        /// Initialize a new instance of XmlConfigurationObjectFactory class.
        /// </summary>
        /// <param name="configFilePath">The XML configuration file path.</param>
        /// <param name="assemblies">The assemblies to load types.</param>
        /// <param name="proxyFactory">The factory to create proxy types.</param>
        /// <exception cref="System.ArgumentException"><paramref name="configFilePath"/> is invalid.</exception>
        /// <exception cref="Xphter.Framework.Reflection.ObjectFacgoryException">Failt to load or parse the XML configuration file.</exception>
        public XmlConfigurationObjectFactory(string configFilePath, IEnumerable<Assembly> assemblies, IProxyTypeFactory proxyFactory) {
            if(string.IsNullOrWhiteSpace(configFilePath)) {
                throw new ArgumentException("configFilePath is null or empty.", "configFilePath");
            }
            if(!PathUtility.IsValidLocalPath(configFilePath)) {
                throw new ArgumentException("configFilePath not represents a local filepath.", "configFilePath");
            }

            this.m_configFilePath = configFilePath;

            this.m_typeResolver = new AssembliesObjectTypeResolver(assemblies);

            this.m_argumentValueResolvers = TypeUtility.FilterTypesInAssemblies(new Assembly[] {
                Assembly.GetExecutingAssembly(),
            }, (item) => typeof(IObjectArgumentResolver).IsAssignableFrom(item) && item.IsClass && !item.IsAbstract && !item.IsGenericTypeDefinition && !item.ContainsGenericParameters).Select((item) => (IObjectArgumentResolver) Activator.CreateInstance(item)).ToArray();

            this.m_proxyFactory = proxyFactory;
            this.m_proxyTypes = new ConcurrentGrowOnlyDictionary<Type, Type>();
            this.m_externalInstanceGetters = new ConcurrentGrowOnlyDictionary<Type, Func<object>>();
            this.m_internalInstanceGetters = new ConcurrentGrowOnlyDictionary<string, Delegate>();

            this.Initialize();
        }

        private const int MAX_ARGUMENTS = 23;

        private readonly Type m_objectType = typeof(object);
        private readonly Type m_enumerableType = typeof(IEnumerable<>);

        protected string m_configFilePath;
        protected IDictionary<string, ObjectDefination> m_definations;

        protected IDictionary<string, object> m_objectsCache;
        protected IDictionary<string, object> m_objectLocks;

        private IObjectTypeResolver m_typeResolver;
        private IEnumerable<IObjectArgumentResolver> m_argumentValueResolvers;

        protected IProxyTypeFactory m_proxyFactory;
        protected ConcurrentGrowOnlyDictionary<Type, Type> m_proxyTypes;
        protected ConcurrentGrowOnlyDictionary<Type, Func<object>> m_externalInstanceGetters;
        protected ConcurrentGrowOnlyDictionary<string, Delegate> m_internalInstanceGetters;

        /// <summary>
        /// Loads and parses XML configuration file.
        /// </summary>
        protected virtual void Initialize() {
            ObjectsConfiguration config = this.LoadConfiguration(this.m_configFilePath);
            this.m_definations = this.ParseConfiguration(config);
            this.m_objectsCache = new Dictionary<string, object>(this.m_definations.Count);
            this.m_objectLocks = new Dictionary<string, object>(this.m_definations.Count);

            // initialize object locks of singleton mode
            foreach(KeyValuePair<string, ObjectDefination> pair in this.m_definations) {
                if(pair.Value.IsSingleton) {
                    this.m_objectLocks[pair.Key] = new object();
                }
            }
        }

        protected virtual string GetObjectKey(string objectID, Type objectType) {
            return !string.IsNullOrWhiteSpace(objectID) ? string.Format("{{{0}}}:{1}", objectID, objectType.FullName) : objectType.FullName;
        }

        protected virtual string GetInternalInstanceGetterKey(Type type, Type[] argumentTypes) {
            return string.Format("{0}({1})", type.AssemblyQualifiedName, argumentTypes.StringJoin(", ", (item) => item.AssemblyQualifiedName));
        }

        /// <summary>
        /// Loads XML configuration file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual ObjectsConfiguration LoadConfiguration(string path) {
            if(!File.Exists(path)) {
                throw new ObjectFacgoryException(string.Format("{0} is not existing.", path));
            }

            ObjectsConfiguration config = null;

            FileStream stream = FileUtility.CreateFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if(stream == null) {
                throw new ObjectFacgoryException(string.Format("Can not open {0} to read.", path));
            }

            using(TextReader reader = new StreamReader(stream, Encoding.UTF8)) {
                try {
                    config = (ObjectsConfiguration) new XmlSerializer(typeof(ObjectsConfiguration)).Deserialize(reader);
                } catch(InvalidOperationException ex) {
                    throw new ObjectFacgoryException(string.Format("Error to read configuration file {0}: {1}", path, ex.Message), ex);
                }
            }

            return config;
        }

        /// <summary>
        /// Parses and validates objects configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected virtual IDictionary<string, ObjectDefination> ParseConfiguration(ObjectsConfiguration config) {
            int index = 0;
            Type objectType = null;
            string objectKey = null;
            IDictionary<string, ObjectDefination> definations = new Dictionary<string, ObjectDefination>(config.Objects.Count);

            foreach(ObjectDefination defination in config.Objects) {
                if(string.IsNullOrWhiteSpace(defination.ObjectType)) {
                    throw new ObjectFacgoryException(string.Format("The type of {0}th object is empty.", index));
                }
                if((objectType = this.m_typeResolver.GetType(defination.ObjectType)) == null) {
                    throw new ObjectFacgoryException(string.Format("Can not resolve type name: {0}", defination.ObjectType));
                }
                if(definations.ContainsKey(objectKey = this.GetObjectKey(defination.ObjectID, objectType))) {
                    throw new ObjectFacgoryException(string.Format("The object type {0} is duplicated.", defination.ObjectType));
                }
                if(objectType.IsGenericType && objectType.ContainsGenericParameters) {
                    throw new ObjectFacgoryException(string.Format("{0} represents an open generic type.", defination.ObjectType));
                }
                if((objectType.IsInterface || objectType.IsAbstract) && string.IsNullOrWhiteSpace(defination.TargetType)) {
                    throw new ObjectFacgoryException(string.Format("{0} missing class attribute.", defination.ObjectType));
                }

                this.ValidateArgumments(defination, index);

                definations[objectKey] = defination;
                ++index;
            }

            return definations;
        }

        /// <summary>
        /// Validates arguments of a object defination.
        /// </summary>
        /// <param name="defination"></param>
        /// <param name="index"></param>
        private void ValidateArgumments(ObjectDefination defination, int index) {
            if(defination.Arguments.Count == 0) {
                return;
            }

            Queue<ObjectArgumentDefination> queue = new Queue<ObjectArgumentDefination>();
            foreach(ObjectArgumentDefination item in defination.Arguments) {
                queue.Enqueue(item);
            }

            ObjectArgumentDefination argument = null;
            while(queue.Count > 0) {
                argument = queue.Dequeue();
                if(string.IsNullOrWhiteSpace(argument.ArgumentType)) {
                    throw new ObjectFacgoryException(string.Format("The argument type name of {0}th object is empty.", index));
                }

                foreach(ObjectArgumentDefination item in argument.Arguments) {
                    queue.Enqueue(item);
                }
            }
        }

        /// <summary>
        /// Gets type of the specified argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private Type GetArgumentType(ObjectArgumentDefination argument) {
            Type argumentType = this.m_typeResolver.GetType(argument.ArgumentType);
            if(argumentType == null) {
                throw new ObjectFacgoryException(string.Format("Can not resolve type name: {0}", argument.ArgumentType));
            }

            if(argumentType.IsGenericTypeDefinition || argumentType.IsGenericType && argumentType.ContainsGenericParameters) {
                if(!argumentType.Equals(this.m_enumerableType) && argumentType.GetInterface(this.m_enumerableType.FullName) == null) {
                    throw new ObjectFacgoryException(string.Format("Not supported type name: {0}", argument.ArgumentType));
                }
                if(string.IsNullOrWhiteSpace(argument.ItemType)) {
                    throw new ObjectFacgoryException("The type name of elements in a IEnumberable is empty.");
                }

                Type itemType = this.m_typeResolver.GetType(argument.ItemType);
                if(argumentType == null) {
                    throw new ObjectFacgoryException(string.Format("Can not resolve type name: {0}", argument.ItemType));
                }

                argumentType = argumentType.MakeGenericType(itemType);
            }

            return argumentType;
        }

        /// <summary>
        /// Gets value of the specified argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="argumentType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private object GetArgumentValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            IObjectArgumentResolver argumentResolver = this.m_argumentValueResolvers.Where((item) => item.Check(argumentType)).FirstOrDefault();
            if(argumentResolver == null) {
                throw new ObjectFacgoryException(string.Format("Can not find the resolver to get argument value of {0}", argumentType.FullName));
            }

            return argumentResolver.GetValue(argument, argumentType, context);
        }

        private Func<object> CreateExternalInstanceGetter(Type type) {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if(constructor == null) {
                throw new ObjectFacgoryException(string.Format("{0} don't has a parameterless constructor.", type.FullName));
            }

            DynamicMethod method = new DynamicMethod(string.Empty, typeof(object), null, type);
            ILGenerator generator = method.GetILGenerator();

            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Ret);

            return (Func<object>) method.CreateDelegate(typeof(Func<object>));
        }

        private Delegate CreateInternalInstanceGetter(Type type, Type[] argumentTypes) {
            if(argumentTypes.Length > MAX_ARGUMENTS) {
                throw new NotSupportedException(string.Format("The maximum supported arguments number is {0}", MAX_ARGUMENTS));
            }

            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
            if(constructor == null) {
                throw new ObjectFacgoryException(string.Format("Can not find constructor of type {0} with arguments: {1}", type.FullName, argumentTypes.StringJoin(", ", (item) => item.FullName)));
            }

            Type argumentType = null;
            DynamicMethod method = new DynamicMethod(string.Empty, typeof(object), argumentTypes.Length > 0 ? ArrayUtility.Repeat<Type>(typeof(object), argumentTypes.Length) : null, type, true);
            ILGenerator generator = method.GetILGenerator();

            for(int i = 0; i < argumentTypes.Length; i++) {
                method.DefineParameter(i + 1, ParameterAttributes.In, "arg" + i);
            }
            for(int i = 0; i < argumentTypes.Length; i++) {
                argumentType = argumentTypes[i];

                generator.Emit(OpCodes.Ldarg, i);
                if(!argumentType.Equals(m_objectType)) {
                    if(argumentType.IsValueType) {
                        generator.Emit(OpCodes.Unbox_Any, argumentType);
                    } else {
                        generator.Emit(OpCodes.Castclass, argumentType);
                    }
                }
            }
            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Ret);

            switch(argumentTypes.Length) {
                case 0:
                    return method.CreateDelegate(typeof(Func<object>));
                case 1:
                    return method.CreateDelegate(typeof(Func<object, object>));
                case 2:
                    return method.CreateDelegate(typeof(Func<object, object, object>));
                case 3:
                    return method.CreateDelegate(typeof(Func<object, object, object, object>));
                case 4:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object>));
                case 5:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object>));
                case 6:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object>));
                case 7:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object>));
                case 8:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object>));
                case 9:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object>));
                case 10:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object>));
                case 11:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object>));
                case 12:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 13:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 14:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 15:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 16:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 17:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 18:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 19:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 20:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 21:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 22:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                case 23:
                    return method.CreateDelegate(typeof(Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>));
                default:
                    throw new NotSupportedException(string.Format("The maximum supported arguments number is {0}", MAX_ARGUMENTS));
            }
        }

        /// <summary>
        /// Creates objects of the type which is not defines in the XML configuration file.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private object[] CreateExternalInstances(Type objectType, int count) {
            if(objectType.IsGenericType && objectType.ContainsGenericParameters) {
                throw new ObjectFacgoryException(string.Format("{0} represents an open generic type.", objectType.FullName));
            }
            if(objectType.IsInterface || objectType.IsAbstract) {
                throw new ObjectFacgoryException(string.Format("{0} is not a concrete type.", objectType.FullName));
            }

            // find proxy type
            Type proxyType = this.m_proxyFactory != null ? this.m_proxyTypes.GetOrAdd(objectType, (item) => this.m_proxyFactory.CreateProxyType(item)) : objectType;
            Func<object> instanceGetter = this.m_externalInstanceGetters.GetOrAdd(proxyType, (item) => this.CreateExternalInstanceGetter(item));

            // creates instances
            object[] instance = new object[count];
            try {
                for(int i = 0; i < count; i++) {
                    instance[i] = instanceGetter();
                }
            } catch(Exception ex) {
                throw new ObjectFacgoryException(string.Format("Error to create instance of type {0}: {1}", objectType.FullName, ex.Message), ex);
            }

            return instance;
        }

        /// <summary>
        /// Creates objects by the arguments defined in the specified object defination.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="arguments"></param>
        /// <param name="context"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private object[] CreateInternalInstances(Type targetType, IList<ObjectArgumentDefination> arguments, IObjectFactoryContext context, int count) {
            // find proxy type
            Type proxyType = this.m_proxyFactory != null ? this.m_proxyTypes.GetOrAdd(targetType, (item) => this.m_proxyFactory.CreateProxyType(item)) : targetType;

            ObjectArgumentDefination argument = null;
            Type[] argumentTypes = new Type[arguments.Count];
            object[] argumentValues = new object[arguments.Count];

            for(int i = 0; i < arguments.Count; i++) {
                argumentTypes[i] = this.GetArgumentType(argument = arguments[i]);
                argumentValues[i] = this.GetArgumentValue(argument, argumentTypes[i], context);
            }

            // create instance getter
            string getterKey = this.GetInternalInstanceGetterKey(proxyType, argumentTypes);
            Delegate instanceGetter = this.m_internalInstanceGetters.GetOrAdd(getterKey, (item) => this.CreateInternalInstanceGetter(proxyType, argumentTypes));

            // create instances
            object[] instances = new object[count];

            try {
                switch(argumentTypes.Length) {
                    case 0:
                        Func<object> getter0 = (Func<object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter0();
                        }
                        break;
                    case 1:
                        Func<object, object> getter1 = (Func<object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter1(argumentValues[0]);
                        }
                        break;
                    case 2:
                        Func<object, object, object> getter2 = (Func<object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter2(argumentValues[0], argumentValues[1]);
                        }
                        break;
                    case 3:
                        Func<object, object, object, object> getter3 = (Func<object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter3(argumentValues[0], argumentValues[1], argumentValues[2]);
                        }
                        break;
                    case 4:
                        Func<object, object, object, object, object> getter4 = (Func<object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter4(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3]);
                        }
                        break;
                    case 5:
                        Func<object, object, object, object, object, object> getter5 = (Func<object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter5(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4]);
                        }
                        break;
                    case 6:
                        Func<object, object, object, object, object, object, object> getter6 = (Func<object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter6(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5]);
                        }
                        break;
                    case 7:
                        Func<object, object, object, object, object, object, object, object> getter7 = (Func<object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter7(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6]);
                        }
                        break;
                    case 8:
                        Func<object, object, object, object, object, object, object, object, object> getter8 = (Func<object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter8(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7]);
                        }
                        break;
                    case 9:
                        Func<object, object, object, object, object, object, object, object, object, object> getter9 = (Func<object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter9(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8]);
                        }
                        break;
                    case 10:
                        Func<object, object, object, object, object, object, object, object, object, object, object> getter10 = (Func<object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter10(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9]);
                        }
                        break;
                    case 11:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object> getter11 = (Func<object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter11(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10]);
                        }
                        break;
                    case 12:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object> getter12 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter12(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11]);
                        }
                        break;
                    case 13:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter13 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter13(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12]);
                        }
                        break;
                    case 14:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter14 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter14(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13]);
                        }
                        break;
                    case 15:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter15 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter15(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14]);
                        }
                        break;
                    case 16:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter16 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter16(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15]);
                        }
                        break;
                    case 17:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter17 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter17(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16]);
                        }
                        break;
                    case 18:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter18 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter18(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16], argumentValues[17]);
                        }
                        break;
                    case 19:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter19 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter19(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16], argumentValues[17], argumentValues[18]);
                        }
                        break;
                    case 20:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter20 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter20(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16], argumentValues[17], argumentValues[18], argumentValues[19]);
                        }
                        break;
                    case 21:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter21 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter21(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16], argumentValues[17], argumentValues[18], argumentValues[19], argumentValues[20]);
                        }
                        break;
                    case 22:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter22 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter22(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16], argumentValues[17], argumentValues[18], argumentValues[19], argumentValues[20], argumentValues[21]);
                        }
                        break;
                    case 23:
                        Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> getter23 = (Func<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) instanceGetter;
                        for(int i = 0; i < count; i++) {
                            instances[i] = getter23(argumentValues[0], argumentValues[1], argumentValues[2], argumentValues[3], argumentValues[4], argumentValues[5], argumentValues[6], argumentValues[7], argumentValues[8], argumentValues[9], argumentValues[10], argumentValues[11], argumentValues[12], argumentValues[13], argumentValues[14], argumentValues[15], argumentValues[16], argumentValues[17], argumentValues[18], argumentValues[19], argumentValues[20], argumentValues[21], argumentValues[22]);
                        }
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The maximum supported arguments number is {0}", MAX_ARGUMENTS));
                }
            } catch(Exception ex) {
                throw new ObjectFacgoryException(string.Format("Error to create instance of type {0}: {1}", proxyType.FullName, ex.Message), ex);
            }

            return instances;
        }

        /// <summary>
        /// Creates instances of the specified type and object ID.
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="objectType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private object[] CreateInstances(string objectID, Type objectType, IObjectFactoryContext context, int count) {
            if(objectType == null) {
                throw new ArgumentNullException("objectType");
            }
            if(objectType.ContainsGenericParameters) {
                throw new ObjectFacgoryException("objectType is an open generic type.");
            }

            string objectKey = this.GetObjectKey(objectID, objectType);

            if(!this.m_definations.ContainsKey(objectKey)) {
                if(!string.IsNullOrWhiteSpace(objectID)) {
                    throw new ObjectFacgoryException(string.Format("Can not find defination of type {0} with ID: {1}", objectType.FullName, objectID));
                }

                return this.CreateExternalInstances(objectType, count);
            }

            ObjectDefination defination = this.m_definations[objectKey];
            Type targetType = !string.IsNullOrWhiteSpace(defination.TargetType) ? this.m_typeResolver.GetType(defination.TargetType) : objectType;
            if(targetType == null) {
                throw new ObjectFacgoryException(string.Format("Can not resolve type name: {0}", defination.TargetType));
            }
            if(!objectType.IsAssignableFrom(targetType)) {
                throw new ObjectFacgoryException(string.Format("{0} is not a {1}", targetType.FullName, objectType.FullName));
            }
            if(targetType.IsGenericTypeDefinition || targetType.IsGenericType && targetType.ContainsGenericParameters) {
                throw new ObjectFacgoryException(string.Format("{0} represents an open generic type.", targetType.FullName));
            }

            if(targetType.Equals(objectType)) {
                if(targetType.IsInterface || targetType.IsAbstract) {
                    throw new ObjectFacgoryException(string.Format("{0} is not a concrete type.", targetType.FullName));
                }

                if(defination.IsSingleton) {
                    object result = null;

                    if(!this.m_objectsCache.ContainsKey(objectKey)) {
                        lock(this.m_objectLocks[objectKey]) {
                            if(!this.m_objectsCache.ContainsKey(objectKey)) {
                                this.m_objectsCache[objectKey] = result = context.CreateInstances(targetType, defination.Arguments, 1)[0];
                            }
                        }
                    }

                    return ArrayUtility.Repeat<object>(result ?? this.m_objectsCache[objectKey], count);
                } else {
                    return context.CreateInstances(targetType, defination.Arguments, count);
                }
            } else {
                if(defination.IsSingleton) {
                    object result = null;

                    if(!this.m_objectsCache.ContainsKey(objectKey)) {
                        lock(this.m_objectLocks[objectKey]) {
                            if(!this.m_objectsCache.ContainsKey(objectKey)) {
                                this.m_objectsCache[objectKey] = result = context.CreateInstances(defination.TargetID, targetType, 1)[0];
                            }
                        }
                    }

                    return ArrayUtility.Repeat<object>(result ?? this.m_objectsCache[objectKey], count);
                } else {
                    return context.CreateInstances(defination.TargetID, targetType, count);
                }
            }
        }

        #region IObjectFactory Members

        /// <inheritdoc />
        public virtual object CreateInstance(Type type) {
            return this.CreateInstances(type, 1)[0];
        }

        public virtual object[] CreateInstances(Type type, int count) {
            return this.CreateInstances(null, type, new ObjectFactoryContext(type, this), count);
        }

        /// <inheritdoc />
        public virtual T CreateInstance<T>() {
            return this.CreateInstances<T>(1)[0];
        }

        /// <inheritdoc />
        public virtual T[] CreateInstances<T>(int count) {
            return this.CreateInstances(typeof(T), count).Cast<T>().ToArray();
        }

        #endregion

        private class ObjectFactoryContext : IObjectFactoryContext {
            public ObjectFactoryContext(Type type, XmlConfigurationObjectFactory factory) {
                this.m_factory = factory;
                this.m_stack = new Stack<string>();

                this.m_stack.Push(factory.GetObjectKey(null, type));
            }

            private Stack<string> m_stack;
            private XmlConfigurationObjectFactory m_factory;

            #region IObjectFactoryContext Members

            public IObjectTypeResolver TypeResolver {
                get {
                    return this.m_factory.m_typeResolver;
                }
            }

            public object[] CreateInstances(string id, Type type, int count) {
                string key = this.m_factory.GetObjectKey(id, type);
                if(this.m_stack.Contains(key)) {
                    throw new ObjectFacgoryException(string.Format("{0} is already request to create, stack trace: {1}", type.FullName, this.m_stack.Reverse().StringJoin(" -> ")));
                }

                this.m_stack.Push(key);
                object[] instances = this.m_factory.CreateInstances(id, type, this, count);
                this.m_stack.Pop();

                return instances;
            }

            public object[] CreateInstances(Type type, IList<ObjectArgumentDefination> arguments, int count) {
                return this.m_factory.CreateInternalInstances(type, arguments, this, count);
            }

            public object GetArgumentValue(ObjectArgumentDefination argument) {
                Type argumentType = this.m_factory.GetArgumentType(argument);
                return this.m_factory.GetArgumentValue(argument, argumentType, this);
            }

            #endregion
        }
    }

    /// <summary>
    /// Provides configuration to create objects.
    /// </summary>
    [XmlRoot("configuration")]
    public class ObjectsConfiguration {
        public ObjectsConfiguration() {
            this.Objects = new List<ObjectDefination>();
        }

        [XmlElement("object")]
        public List<ObjectDefination> Objects {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the defination of creating a object.
    /// </summary>
    public class ObjectDefination {
        public ObjectDefination() {
            this.Arguments = new List<ObjectArgumentDefination>();
        }

        [XmlAttribute("id")]
        public string ObjectID {
            get;
            set;
        }

        [XmlAttribute("type")]
        public string ObjectType {
            get;
            set;
        }

        [XmlAttribute("class")]
        public string TargetType {
            get;
            set;
        }

        [XmlAttribute("ref")]
        public string TargetID {
            get;
            set;
        }

        [XmlAttribute("singleton")]
        public bool IsSingleton {
            get;
            set;
        }

        [XmlElement("argument")]
        public List<ObjectArgumentDefination> Arguments {
            get;
            set;
        }

        public override string ToString() {
            return !string.IsNullOrWhiteSpace(this.TargetType) ? string.Format("{0} -> {1}", !string.IsNullOrWhiteSpace(this.ObjectID) ? string.Format("{{{0}}}:{1}", this.ObjectID, this.ObjectType) : this.ObjectType, !string.IsNullOrWhiteSpace(this.TargetID) ? string.Format("{{{0}}}:{1}", this.TargetID, this.TargetType) : this.TargetType) : !string.IsNullOrWhiteSpace(this.ObjectID) ? string.Format("{{{0}}}:{1}", this.ObjectID, this.ObjectType) : this.ObjectType;
        }
    }

    /// <summary>
    /// Represents a argument of constructor.
    /// </summary>
    public class ObjectArgumentDefination {
        public ObjectArgumentDefination() {
            this.Arguments = new List<ObjectArgumentDefination>();
        }

        [XmlAttribute("type")]
        public string ArgumentType {
            get;
            set;
        }

        [XmlAttribute("value")]
        public string ArgumentValue {
            get;
            set;
        }

        [XmlAttribute("ref")]
        public string ObjectID {
            get;
            set;
        }

        [XmlAttribute("item")]
        public string ItemType {
            get;
            set;
        }

        [XmlElement("argument")]
        public List<ObjectArgumentDefination> Arguments {
            get;
            set;
        }

        public override string ToString() {
            return !string.IsNullOrWhiteSpace(this.ArgumentValue) ? string.Format("{0} -> {1}", this.ArgumentType, !string.IsNullOrWhiteSpace(this.ObjectID) ? string.Format("{{{0}}}:{1}", this.ObjectID, this.ArgumentValue) : this.ArgumentValue) : !string.IsNullOrWhiteSpace(this.ObjectID) ? string.Format("{{{0}}}:{1}", this.ObjectID, this.ArgumentType) : this.ArgumentType;
        }
    }

    /// <summary>
    /// Provides Type of type name.
    /// </summary>
    internal interface IObjectTypeResolver {
        /// <summary>
        /// Gets the Type by the specified type name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        Type GetType(string fullName);
    }

    /// <summary>
    /// Provides types from some assemblies.
    /// </summary>
    internal class AssembliesObjectTypeResolver : IObjectTypeResolver {
        public AssembliesObjectTypeResolver(IEnumerable<Assembly> assemblies) {
            if(assemblies == null || !assemblies.Any()) {
                assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            }

            this.m_assemblies = assemblies;
            this.m_cache = new ConcurrentGrowOnlyDictionary<string, Type>();
        }

        protected IEnumerable<Assembly> m_assemblies;
        protected ConcurrentGrowOnlyDictionary<string, Type> m_cache;

        /// <summary>
        /// Finds type from assemblies and mscorlib.dll.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        protected Type InternalGetType(string fullName) {
            Type type = null;

            foreach(Assembly item in this.m_assemblies) {
                if((type = item.GetType(fullName, false, false)) != null) {
                    break;
                }
            }
            if(type == null) {
                type = Type.GetType(fullName, false, false);
            }

            return type;
        }

        #region IObjectTypeResolver Members

        /// <inheritdoc />
        public virtual Type GetType(string fullName) {
            if(string.IsNullOrWhiteSpace(fullName)) {
                throw new ArgumentException("fullName is null or empty.", "fullName");
            }

            return this.m_cache.GetOrAdd(fullName, this.InternalGetType);
        }

        #endregion
    }

    /// <summary>
    /// Provides the shared functions to create objects.
    /// </summary>
    internal interface IObjectFactoryContext {
        /// <summary>
        /// Gets the IObjectTypeResolver object.
        /// </summary>
        IObjectTypeResolver TypeResolver {
            get;
        }

        /// <summary>
        /// Creates instances of the specified type.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        object[] CreateInstances(string id, Type type, int count);

        /// <summary>
        /// Creates instances of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        object[] CreateInstances(Type type, IList<ObjectArgumentDefination> arguments, int count);

        /// <summary>
        /// Gets value of the specified argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        object GetArgumentValue(ObjectArgumentDefination argument);
    }

    /// <summary>
    /// Creates object by a XML configuration file.
    /// </summary>
    [Obsolete]
    internal interface IXmlConfigurationObjectFactory {
        /// <summary>
        /// Creates instance of the specified type.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object CreateInstance(string id, Type type);

        /// <summary>
        /// Creates instance of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        object CreateInstance(Type type, IList<ObjectArgumentDefination> arguments);
    }

    /// <summary>
    /// Provides value of a argument.
    /// </summary>
    [Obsolete]
    internal interface IObjectArgumentValueFactory {
        /// <summary>
        /// Gets value of the specified argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        object GetArgumentValue(ObjectArgumentDefination argument);
    }

    /// <summary>
    /// Parses value of a argument.
    /// </summary>
    internal interface IObjectArgumentResolver {
        /// <summary>
        /// Checks whether can parse the specified argument type.
        /// </summary>
        /// <param name="argumentType"></param>
        /// <returns></returns>
        bool Check(Type argumentType);

        /// <summary>
        /// Gets value of the specified argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="argumentType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context);
    }

    /// <summary>
    /// Parses bool argument.
    /// </summary>
    internal class BooleanObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(bool);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return false;
            }

            bool value = false;

            if(!bool.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses byte argument.
    /// </summary>
    internal class ByteObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(byte);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return (byte) 0;
            }

            byte value = 0;

            if(!byte.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses sbyte argument.
    /// </summary>
    internal class SByteObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(sbyte);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return (sbyte) 0;
            }

            sbyte value = 0;

            if(!sbyte.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses short argument.
    /// </summary>
    internal class Int16ObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(short);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return (short) 0;
            }

            short value = 0;

            if(!short.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses ushort argument.
    /// </summary>
    internal class UInt16ObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(ushort);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return (ushort) 0;
            }

            ushort value = 0;

            if(!ushort.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses int argument.
    /// </summary>
    internal class Int32ObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(int);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return 0;
            }

            int value = 0;

            if(!int.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses uint argument.
    /// </summary>
    internal class UInt32ObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(uint);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return (uint) 0;
            }

            uint value = 0;

            if(!uint.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses long argument.
    /// </summary>
    internal class Int64ObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(long);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return 0L;
            }

            long value = 0L;

            if(!long.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses ulong argument.
    /// </summary>
    internal class UInt64ObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(ulong);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return 0UL;
            }

            ulong value = 0UL;

            if(!ulong.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses Enum argument.
    /// </summary>
    internal class EnumObjectArgumentResolver : IObjectArgumentResolver {
        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.IsEnum;
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return Enum.GetValues(argumentType).GetValue(0);
            }

            object value = null;

            try {
                value = Enum.Parse(argumentType, argument.ArgumentValue, true);
            } catch(Exception) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, argumentType.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses float argument.
    /// </summary>
    internal class SingleObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(float);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return 0F;
            }

            float value = 0F;

            if(!float.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses double argument.
    /// </summary>
    internal class DoubleObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(double);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return 0D;
            }

            double value = 0D;

            if(!double.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses decimal argument.
    /// </summary>
    internal class DecimalObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(decimal);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return 0M;
            }

            decimal value = 0M;

            if(!decimal.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses char argument.
    /// </summary>
    internal class CharObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(char);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            return !string.IsNullOrEmpty(argument.ArgumentValue) ? argument.ArgumentValue[0] : '\0';
        }

        #endregion
    }

    /// <summary>
    /// Parses string argument.
    /// </summary>
    internal class StringObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(string);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            return argument.ArgumentValue;
        }

        #endregion
    }

    /// <summary>
    /// Parses DateTime argument.
    /// </summary>
    internal class DateTimeObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_type = typeof(DateTime);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.Equals(this.m_type);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return DateTime.Now;
            }

            DateTime value;

            if(!DateTime.TryParse(argument.ArgumentValue, out value)) {
                throw new ObjectFacgoryException(string.Format("Can not convert {0} to a {1} object.", argument.ArgumentValue, this.m_type.FullName));
            }

            return value;
        }

        #endregion
    }

    /// <summary>
    /// Parses general object argument.
    /// </summary>
    internal class ObjectObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_enumerableType = typeof(IEnumerable<>);

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return (argumentType.IsInterface || argumentType.IsClass) && !argumentType.IsString() && (!argumentType.IsGenericType || !argumentType.GetGenericTypeDefinition().Equals(this.m_enumerableType) && argumentType.GetGenericTypeDefinition().GetInterface(this.m_enumerableType.FullName) == null);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            Type targetType = argumentType;

            if(argument.ArgumentValue != null && string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return null;
            }

            if(!string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                if((targetType = context.TypeResolver.GetType(argument.ArgumentValue)) == null) {
                    throw new ObjectFacgoryException(string.Format("Can not resolve type name: {0}", argument.ArgumentValue));
                }
                if(!argumentType.IsAssignableFrom(targetType)) {
                    throw new ObjectFacgoryException(string.Format("{0} is not a {1}", targetType.FullName, argumentType.FullName));
                }
            }

            if(string.IsNullOrWhiteSpace(argument.ObjectID) && argument.Arguments.Count > 0) {
                return context.CreateInstances(targetType, argument.Arguments, 1)[0];
            } else {
                return context.CreateInstances(argument.ObjectID, targetType, 1)[0];
            }
        }

        #endregion
    }

    /// <summary>
    /// Parses list argument.
    /// </summary>
    internal class EnumerableObjectArgumentResolver : IObjectArgumentResolver {
        private Type m_enumerableType = typeof(IEnumerable<>);
        private Type m_collectionType = typeof(ICollection<>);
        private string m_addMethodName = ((MethodCallExpression) (((Expression<Action<List<object>>>) ((obj) => obj.Add(null))).Body)).Method.Name;

        #region IObjectArgumentResolver Members

        public bool Check(Type argumentType) {
            return argumentType.IsGenericType && (argumentType.GetGenericTypeDefinition().Equals(this.m_enumerableType) || argumentType.GetGenericTypeDefinition().GetInterface(this.m_enumerableType.FullName) != null);
        }

        public object GetValue(ObjectArgumentDefination argument, Type argumentType, IObjectFactoryContext context) {
            if(string.IsNullOrWhiteSpace(argument.ArgumentValue)) {
                return null;
            }

            Type targetType = null;

            if((targetType = context.TypeResolver.GetType(argument.ArgumentValue)) == null) {
                throw new ObjectFacgoryException(string.Format("Can not resolve type name: {0}", argument.ArgumentValue));
            }
            if(!targetType.IsGenericType || targetType.GetGenericTypeDefinition().GetInterface(this.m_collectionType.FullName) == null) {
                throw new ObjectFacgoryException(string.Format("Not supported type name: {0}", targetType.FullName));
            }

            object value = null;
            try {
                value = Activator.CreateInstance(targetType.ContainsGenericParameters ? targetType.MakeGenericType(argumentType.GetGenericArguments()[0]) : targetType);
            } catch(Exception ex) {
                throw new ObjectFacgoryException(string.Format("Error to create instance of type {0}: {1}", argument.ArgumentValue, ex.Message), ex);
            }

            object[] parameters = new object[1];
            MethodInfo addMethod = value.GetType().GetMethod(this.m_addMethodName);
            foreach(ObjectArgumentDefination item in argument.Arguments) {
                parameters[0] = context.GetArgumentValue(item);
                addMethod.Invoke(value, parameters);
            }

            return value;
        }

        #endregion
    }
}
