using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;

namespace Xphter.Framework.Reflection {
    /// <summary>
    /// Provides a factory to create proxy type of the source type.
    /// </summary>
    public interface IProxyTypeFactory {
        /// <summary>
        /// Creates a proxy type of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Type CreateProxyType(Type type);

        /// <summary>
        /// Creates a proxy type of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Type CreateProxyType<T>();
    }

    /// <summary>
    /// Represents a method filter which can intercept the executing of a method.
    /// </summary>
    public interface IMethodFilter {
        /// <summary>
        /// Gets number of order, from small to large.
        /// </summary>
        int Order {
            get;
        }

        /// <summary>
        /// Invokes before executing a method.
        /// </summary>
        /// <param name="context"></param>
        void OnPreExecuting(IPreMethodExecutingContext context);

        /// <summary>
        /// Invoks after executing a method.
        /// </summary>
        /// <param name="context"></param>
        void OnPostExecuting(IPostMethodExecutingContext context);
    }

    /// <summary>
    /// Provides context data of executing a method for IMethodFilter interface.
    /// </summary>
    public interface IMethodExecutingContext {
        /// <summary>
        /// Gets the object invoking current method.
        /// </summary>
        object TargetObject {
            get;
        }

        /// <summary>
        /// Gets name of the type which declares executing method.
        /// </summary>
        string TypeName {
            get;
        }

        /// <summary>
        /// Gets name of executing method.
        /// </summary>
        string MethodName {
            get;
        }

        /// <summary>
        /// Gets method sign.
        /// </summary>
        string MethodSign {
            get;
        }

        /// <summary>
        /// Gets name-value pair of all arguments.
        /// </summary>
        IDictionary<string, object> Arguments {
            get;
        }

        /// <summary>
        /// Provides custom data for filter.
        /// </summary>
        IDictionary<string, object> FilterBag {
            get;
        }
    }

    /// <summary>
    /// Provides argument for IMethodFilter.OnPreExecuting method.
    /// </summary>
    public interface IPreMethodExecutingContext : IMethodExecutingContext {
        /// <summary>
        /// Gets or sets whether to cancel executing method.
        /// </summary>
        bool IsCancel {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a exception.
        /// </summary>
        Exception Exception {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides argument for IMethodFilter.OnPostExecuting method.
    /// </summary>
    public interface IPostMethodExecutingContext : IMethodExecutingContext {
        /// <summary>
        /// Gets a value to indicate whether should cancel executing method.
        /// </summary>
        bool IsCancel {
            get;
        }

        /// <summary>
        /// Gets the exception thrown when executing method.
        /// </summary>
        Exception Exception {
            get;
        }

        /// <summary>
        /// Gets or sets a value to indicate whether the execption has been handled.
        /// </summary>
        bool IsExceptionHandled {
            get;
            set;
        }
    }

    /// <summary>
    /// Provides a default implementation of IMethodExecutingContext interface.
    /// </summary>
    public class MethodExecutingContext : IMethodExecutingContext, IPreMethodExecutingContext, IPostMethodExecutingContext {
        public MethodExecutingContext(object targetObject, string typeName, string methodName, string methodSign) {
            this.TargetObject = targetObject;
            this.TypeName = typeName;
            this.MethodName = methodName;
            this.MethodSign = methodSign;
            this.Arguments = new Dictionary<string, object>();
            this.FilterBag = new Dictionary<string, object>();
        }

        public object TargetObject {
            get;
            private set;
        }

        /// <inherit />
        public string TypeName {
            get;
            private set;
        }

        /// <inherit />
        public string MethodName {
            get;
            private set;
        }

        /// <inherit />
        public string MethodSign {
            get;
            private set;
        }

        /// <inherit />
        public IDictionary<string, object> Arguments {
            get;
            private set;
        }

        /// <inherit />
        public IDictionary<string, object> FilterBag {
            get;
            private set;
        }

        /// <inherit />
        public Exception Exception {
            get;
            set;
        }

        /// <inherit />
        public bool IsCancel {
            get;
            set;
        }

        /// <inherit />
        public bool IsExceptionHandled {
            get;
            set;
        }

        /// <summary>
        /// Adds a named argument.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddArgument(string name, object value) {
            this.Arguments.Add(name, value);
        }
    }

    /// <summary>
    /// Provides a base class for IMethodFilter objects. All IMethodFilter attributes must inherit from this class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class MethodFilterAttribute : Attribute, IMethodFilter {
        public MethodFilterAttribute()
            : this(0, MethodFilterActOnPropertyMethods.GetMethod | MethodFilterActOnPropertyMethods.SetMethod) {
        }

        public MethodFilterAttribute(int order)
            : this(order, MethodFilterActOnPropertyMethods.GetMethod | MethodFilterActOnPropertyMethods.SetMethod) {
        }

        public MethodFilterAttribute(MethodFilterActOnPropertyMethods propertyMethods)
            : this(0, propertyMethods) {
        }

        public MethodFilterAttribute(int order, MethodFilterActOnPropertyMethods propertyMethods) {
            this.Order = order;
            this.PropertyMethods = propertyMethods;
        }

        /// <summary>
        /// Gets or sets a value to indicates this filter act on which methods of a property.
        /// </summary>
        public virtual MethodFilterActOnPropertyMethods PropertyMethods {
            get;
            set;
        }

        #region IMethodFilter 成员

        /// <inherit />
        public virtual int Order {
            get;
            private set;
        }

        /// <inherit />
        public abstract void OnPreExecuting(IPreMethodExecutingContext context);

        /// <inherit />
        public abstract void OnPostExecuting(IPostMethodExecutingContext context);

        #endregion
    }

    [Flags]
    public enum MethodFilterActOnPropertyMethods {
        None = 0x00,

        GetMethod = 0x01,

        SetMethod = 0x02,
    }

    /// <summary>
    /// Indicates to ignore the specified method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FilterIgnoreAttribute : Attribute {
    }

    /// <summary>
    /// Wraps the specified method in a transaction scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TransactionScopeAttribute : MethodFilterAttribute {
        public TransactionScopeAttribute()
            : this(false, IsolationLevel.ReadCommitted) {
        }

        public TransactionScopeAttribute(IsolationLevel isolationLevel)
            : this(false, isolationLevel) {
        }

        public TransactionScopeAttribute(bool isRequreNewTransaction, IsolationLevel isolationLevel) {
            this.IsRequreNewTransaction = isRequreNewTransaction;
            this.IsolationLevel = isolationLevel;
        }

        private const string SCOPE_KEY = "TransactionScope";

        public bool IsRequreNewTransaction {
            get;
            set;
        }

        public IsolationLevel IsolationLevel {
            get;
            set;
        }

        public override void OnPostExecuting(IPostMethodExecutingContext context) {
            if(context.Exception != null) {
                return;
            }

            TransactionScope scope = context.FilterBag.ContainsKey(SCOPE_KEY) ? (TransactionScope) context.FilterBag[SCOPE_KEY] : null;
            if(scope == null) {
                return;
            }

            using(scope) {
                scope.Complete();
            }
        }

        public override void OnPreExecuting(IPreMethodExecutingContext context) {
            if(context.IsCancel) {
                return;
            }

            context.FilterBag[SCOPE_KEY] = new TransactionScope(this.IsRequreNewTransaction ? TransactionScopeOption.RequiresNew : TransactionScopeOption.Required, new TransactionOptions {
                IsolationLevel = this.IsolationLevel,
            });
        }
    }

    /// <summary>
    /// Creates proxy type for types which has flag with MethodFilterAttribute attributes.
    /// </summary>
    public class MethodFilterProxyTypeFactory : IProxyTypeFactory {
        static MethodFilterProxyTypeFactory() {
            g_classFilters = new Dictionary<string, IMethodFilter[]>();
            g_getClassFilterMethod = typeof(MethodFilterProxyTypeFactory).GetMethod(((MethodCallExpression) ((Expression<Func<IMethodFilter>>) (() => MethodFilterProxyTypeFactory.GetClassFilter(null, 0))).Body).Method.Name, BindingFlags.Static | BindingFlags.Public);

            g_contextConstructor = typeof(MethodExecutingContext).GetConstructor(new Type[] {
                typeof(object), typeof(string), typeof(string), typeof(string),
            });
            g_contextExceptionGetter = typeof(MethodExecutingContext).GetProperty(((MemberExpression) ((Expression<Func<MethodExecutingContext, Exception>>) ((obj) => obj.Exception)).Body).Member.Name).GetGetMethod();
            g_contextExceptionSetter = typeof(MethodExecutingContext).GetProperty(((MemberExpression) ((Expression<Func<MethodExecutingContext, Exception>>) ((obj) => obj.Exception)).Body).Member.Name).GetSetMethod();
            g_contextIsCancelGetter = typeof(MethodExecutingContext).GetProperty(((MemberExpression) ((Expression<Func<MethodExecutingContext, bool>>) ((obj) => obj.IsCancel)).Body).Member.Name).GetGetMethod();
            g_contextAddArgumentMethod = typeof(MethodExecutingContext).GetMethod(((MethodCallExpression) ((Expression<Action<MethodExecutingContext>>) ((obj) => obj.AddArgument(null, null))).Body).Method.Name);

            g_onPreExecutingMethod = typeof(IMethodFilter).GetMethod(((MethodCallExpression) ((Expression<Action<IMethodFilter>>) ((obj) => obj.OnPreExecuting(null))).Body).Method.Name);
            g_onPostExecutingMethod = typeof(IMethodFilter).GetMethod(((MethodCallExpression) ((Expression<Action<IMethodFilter>>) ((obj) => obj.OnPostExecuting(null))).Body).Method.Name);

            g_canceledExceptionConstructor = typeof(OperationCanceledException).GetConstructor(Type.EmptyTypes);
        }

        public MethodFilterProxyTypeFactory(string assemblyName) {
            if(string.IsNullOrWhiteSpace(assemblyName)) {
                throw new ArgumentException("assemblyName is null or empty.", "assemblyName");
            }

            this.m_assemblyName = assemblyName;

            this.Initialize();
        }

        /// <summary>
        /// Intializes the proxy type factory.
        /// </summary>
        protected virtual void Initialize() {
#if DEBUG
            AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
#else
            AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
#endif

            this.m_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(this.m_assemblyName), access);
            this.m_moduleBuilder = this.m_assemblyBuilder.DefineDynamicModule(this.m_assemblyName + ".dll");
        }

        private string m_assemblyName;
        private AssemblyBuilder m_assemblyBuilder;
        private ModuleBuilder m_moduleBuilder;

        // used to cache all method filters
        private static IDictionary<string, IMethodFilter[]> g_classFilters;
        private static readonly MethodInfo g_getClassFilterMethod;

        private static readonly ConstructorInfo g_contextConstructor;
        private static readonly MethodInfo g_contextExceptionGetter;
        private static readonly MethodInfo g_contextExceptionSetter;
        private static readonly MethodInfo g_contextIsCancelGetter;
        private static readonly MethodInfo g_contextAddArgumentMethod;

        private static readonly MethodInfo g_onPreExecutingMethod;
        private static readonly MethodInfo g_onPostExecutingMethod;

        private static readonly ConstructorInfo g_canceledExceptionConstructor;

        public static IMethodFilter GetClassFilter(string classKey, int filterIndex) {
            return g_classFilters[classKey][filterIndex];
        }

        private string GetProxyTypeName(Type type) {
            // for generic type: use GUID instand of Type.FullName
            return string.Format("__Proxy_{0}__{1}", type.Name, Guid.NewGuid().ToString("N"));
        }

        private string GetClassKey(Type sourceType, Type proxyType) {
            return proxyType.Name;
        }

        private string GetStaticFieldName(int filterIndex) {
            return string.Format("g__Filter__{0}", filterIndex);
        }

        private bool IsMethodOverridable(MethodInfo method) {
            return method != null && (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly) && !method.IsFamilyAndAssembly && method.IsVirtual && !method.IsFinal;
        }

        private CustomAttributeBuilder CreateCustomAttribute(CustomAttributeData data) {
            ICollection<PropertyInfo> properties = new List<PropertyInfo>();
            ICollection<object> propertyValues = new List<object>();
            ICollection<FieldInfo> fields = new List<FieldInfo>();
            ICollection<object> fieldValues = new List<object>();

            foreach(CustomAttributeNamedArgument item in data.NamedArguments) {
                switch(item.MemberInfo.MemberType) {
                    case MemberTypes.Property:
                        properties.Add((PropertyInfo) item.MemberInfo);
                        propertyValues.Add(item.TypedValue.Value);
                        break;
                    case MemberTypes.Field:
                        fields.Add((FieldInfo) item.MemberInfo);
                        fieldValues.Add(item.TypedValue.Value);
                        break;
                }
            }

            return new CustomAttributeBuilder(data.Constructor, data.ConstructorArguments.Select((item) => item.Value).ToArray(), properties.ToArray(), propertyValues.ToArray(), fields.ToArray(), fieldValues.ToArray());
        }

        private void InjectCustomAttributes(Type sourceType, TypeBuilder targetType) {
            foreach(CustomAttributeData item in sourceType.GetCustomAttributesData()) {
                if(!typeof(IMethodFilter).IsAssignableFrom(item.Constructor.DeclaringType)) {
                    targetType.SetCustomAttribute(this.CreateCustomAttribute(item));
                }
            }
        }

        private void InjectCustomAttributes(MethodInfo sourceMethod, MethodBuilder targetMethod) {
            foreach(CustomAttributeData item in sourceMethod.GetCustomAttributesData()) {
                if(!typeof(CompilerGeneratedAttribute).IsAssignableFrom(item.Constructor.DeclaringType) &&
                    !typeof(IMethodFilter).IsAssignableFrom(item.Constructor.DeclaringType)) {
                    targetMethod.SetCustomAttribute(this.CreateCustomAttribute(item));
                }
            }
        }

        private void InjectCustomAttributes(ParameterInfo sourceParameter, ParameterBuilder targetParameter) {
            foreach(CustomAttributeData item in sourceParameter.GetCustomAttributesData()) {
                targetParameter.SetCustomAttribute(this.CreateCustomAttribute(item));
            }
        }

        protected virtual MethodBuilder CreateProxyMethod(TypeBuilder typeBuilder, MethodInfo method, MethodFilterAttribute[] classFilters, MethodFilterAttribute[] methodFilters, FieldBuilder[] staticFields, int startFilterIndex, Func<MethodFilterAttribute, bool> predicate) {
            ILGenerator generator = null;

            Type typeParameter = null;
            Type[] typeParameters = null;
            GenericTypeParameterBuilder typeParameterBuilder = null;
            GenericTypeParameterBuilder[] typeParameterBuilders = null;

            MethodBuilder methodBuilder = null;
            ParameterInfo methodParameter = null;
            ParameterInfo[] methodParameters = method.GetParameters();
            LocalBuilder contextVariableBuilder = null, exceptionVariableBuilder = null, returnVariableBuilder = null;
            Label checkCancelLabel, callBaseMethodLabel;

            // create method
            MethodAttributes methodAttributes = (method.IsPublic ? MethodAttributes.Public : MethodAttributes.Family) | MethodAttributes.Virtual;
            if(method.IsSpecialName) {
                methodAttributes |= MethodAttributes.SpecialName;
            }
            methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, methodParameters.Select((item) => item.ParameterType).ToArray());
            this.InjectCustomAttributes(method, methodBuilder);

            // create method type arguments
            if(method.IsGenericMethodDefinition || method.IsGenericMethod && method.ContainsGenericParameters) {
                typeParameters = method.GetGenericArguments().Where((item) => item.IsGenericParameter).ToArray();
                if(typeParameters.Length > 0) {
                    typeParameterBuilders = methodBuilder.DefineGenericParameters(typeParameters.Select((item) => item.Name).ToArray());

                    for(int j = 0; j < typeParameters.Length; j++) {
                        typeParameter = typeParameters[j];
                        typeParameterBuilder = typeParameterBuilders[j];

                        typeParameterBuilder.SetGenericParameterAttributes(typeParameter.GenericParameterAttributes);
                        foreach(Type constraint in typeParameter.GetGenericParameterConstraints()) {
                            if(constraint.IsClass) {
                                typeParameterBuilder.SetBaseTypeConstraint(constraint);
                            } else {
                                typeParameterBuilder.SetInterfaceConstraints(constraint);
                            }
                        }
                    }
                }
            }

            // create method parameters
            for(int j = 0; j < methodParameters.Length; j++) {
                this.InjectCustomAttributes(
                    methodParameters[j],
                    methodBuilder.DefineParameter(j + 1, methodParameters[j].Attributes, methodParameters[j].Name));
            }

            // delcare local variables
            generator = methodBuilder.GetILGenerator();
            contextVariableBuilder = generator.DeclareLocal(typeof(MethodExecutingContext), false);
            exceptionVariableBuilder = generator.DeclareLocal(typeof(Exception), false);
            if(!method.ReturnType.Equals(typeof(void))) {
                returnVariableBuilder = generator.DeclareLocal(method.ReturnType, false);
            }

            // initialize local variables
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldstr, method.ReflectedType.FullName);
            generator.Emit(OpCodes.Ldstr, method.Name);
            generator.Emit(OpCodes.Ldstr, method.ToString());
            generator.Emit(OpCodes.Newobj, g_contextConstructor);
            generator.Emit(OpCodes.Stloc_0);

            // initialize arguments of context variable
            for(int j = 0; j < methodParameters.Length; j++) {
                if((methodParameter = methodParameters[j]).IsOut) {
                    continue;
                }

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldstr, methodParameter.Name);
                generator.Emit(OpCodes.Ldarg, j + 1);
                if(methodParameter.ParameterType.IsValueType) {
                    generator.Emit(OpCodes.Box, methodParameter.ParameterType);
                }
                generator.Emit(OpCodes.Callvirt, g_contextAddArgumentMethod);
            }

            // invoke OnPreExecuting
            for(int j = 0; j < classFilters.Length; j++) {
                if(!predicate(classFilters[j])) {
                    continue;
                }

                generator.Emit(OpCodes.Ldsfld, staticFields[j]);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Callvirt, g_onPreExecutingMethod);
            }
            for(int j = 0; j < methodFilters.Length; j++) {
                if(!predicate(methodFilters[j])) {
                    continue;
                }

                generator.Emit(OpCodes.Ldsfld, staticFields[startFilterIndex + j]);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Callvirt, g_onPreExecutingMethod);
            }

            // check Exception property
            generator.BeginExceptionBlock();
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Callvirt, g_contextExceptionGetter);
            generator.Emit(OpCodes.Brfalse_S, checkCancelLabel = generator.DefineLabel());
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Callvirt, g_contextExceptionGetter);
            generator.Emit(OpCodes.Throw);

            // check IsCancel property
            generator.MarkLabel(checkCancelLabel);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Callvirt, g_contextIsCancelGetter);
            generator.Emit(OpCodes.Brfalse_S, callBaseMethodLabel = generator.DefineLabel());
            generator.Emit(OpCodes.Newobj, g_canceledExceptionConstructor);
            generator.Emit(OpCodes.Throw);

            // call current method
            generator.BeginExceptionBlock();
            generator.MarkLabel(callBaseMethodLabel);
            generator.Emit(OpCodes.Ldarg_0);
            for(int j = 1; j <= methodParameters.Length; j++) {
                generator.Emit(OpCodes.Ldarg, j);
            }
            generator.Emit(OpCodes.Call, method);
            if(!method.ReturnType.Equals(typeof(void))) {
                generator.Emit(OpCodes.Stloc_2);
            }

            // catch Exception
            generator.BeginCatchBlock(typeof(Exception));
            generator.Emit(OpCodes.Stloc_1);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldloc_1);
            generator.Emit(OpCodes.Callvirt, g_contextExceptionSetter);
            generator.Emit(OpCodes.Ldloc_1);
            generator.Emit(OpCodes.Throw);
            generator.EndExceptionBlock();

            // invoke OnPostExecuting
            generator.BeginFinallyBlock();
            for(int j = 0; j < methodFilters.Length; j++) {
                if(!predicate(methodFilters[j])) {
                    continue;
                }

                generator.Emit(OpCodes.Ldsfld, staticFields[startFilterIndex + j]);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Callvirt, g_onPostExecutingMethod);
            }
            for(int j = 0; j < classFilters.Length; j++) {
                if(!predicate(classFilters[j])) {
                    continue;
                }

                generator.Emit(OpCodes.Ldsfld, staticFields[j]);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Callvirt, g_onPostExecutingMethod);
            }
            generator.EndExceptionBlock();

            if(!method.ReturnType.Equals(typeof(void))) {
                generator.Emit(OpCodes.Ldloc_2);
            }
            generator.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        /// <summary>
        /// Creates a proxy type of the specified source type.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        protected virtual Type InternalCreateProxyType(Type sourceType) {
            // load virtual methods
            MethodInfo[] methods = sourceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where((item) =>
                // not constrcutor methods
                !item.IsConstructor &&

                // only overridable methods
                this.IsMethodOverridable(item) &&

                // ignore property methods and operator override method
                !item.IsSpecialName &&

                // ignore marked methods
                item.GetCustomAttributes(typeof(FilterIgnoreAttribute), true).Length == 0 &&

                // ignore common methods defined in System.Object class
                !item.DeclaringType.Equals(typeof(object))).ToArray();

            // load virtual properties
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where((item) =>
                // ignore index properties
                item.GetIndexParameters().Length == 0 &&

                // can read or write
                (item.CanRead || item.CanWrite) &&

                // ignore special properties
                !item.IsSpecialName &&

                // ignore marked methods
                Attribute.GetCustomAttributes(item, typeof(FilterIgnoreAttribute), true).Length == 0 &&

                // ignore common properties defined in System.Object class
                !item.DeclaringType.Equals(typeof(object))).ToArray();

            // find class filters
            MethodFilterAttribute[] classFilters = sourceType.GetCustomAttributes(typeof(MethodFilterAttribute), true).Cast<MethodFilterAttribute>().OrderBy((item) => item.Order).ToArray();

            // find methods filters
            MethodFilterAttribute[][] methodFilters = new MethodFilterAttribute[methods.Length][];
            for(int i = 0; i < methods.Length; i++) {
                methodFilters[i] = methods[i].GetCustomAttributes(typeof(MethodFilterAttribute), true).Cast<MethodFilterAttribute>().OrderBy((item) => item.Order).ToArray();
            }

            // find properties filters
            MethodFilterAttribute[][] propertyFilters = new MethodFilterAttribute[properties.Length][];
            for(int i = 0; i < properties.Length; i++) {
                propertyFilters[i] = Attribute.GetCustomAttributes(properties[i], typeof(MethodFilterAttribute), true).Cast<MethodFilterAttribute>().OrderBy((item) => item.Order).ToArray();
            }

            if(methods.Length == 0 && properties.Length == 0 || classFilters.Length == 0 && methodFilters.All((item) => item.Length == 0) && propertyFilters.All((item) => item.Length == 0)) {
                return sourceType;
            }

            ILGenerator generator = null;

            #region Create Proxy Type

            TypeAttributes typeAttributes = sourceType.Attributes;
            typeAttributes &= ~(TypeAttributes.NestedPrivate | TypeAttributes.NestedPublic | TypeAttributes.NestedFamily | TypeAttributes.NestedAssembly | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem);
            typeAttributes |= TypeAttributes.Public;

            Type typeParameter = null;
            Type[] typeParameters = null;
            GenericTypeParameterBuilder typeParameterBuilder = null;
            GenericTypeParameterBuilder[] typeParameterBuilders = null;
            TypeBuilder typeBuilder = this.m_moduleBuilder.DefineType(this.GetProxyTypeName(sourceType), typeAttributes, sourceType);
            this.InjectCustomAttributes(sourceType, typeBuilder);

            // create type arguments
            if(sourceType.IsGenericTypeDefinition || sourceType.IsGenericType && sourceType.ContainsGenericParameters) {
                typeParameters = sourceType.GetGenericArguments().Where((item) => item.IsGenericParameter).ToArray();
                if(typeParameters.Length > 0) {
                    typeParameterBuilders = typeBuilder.DefineGenericParameters(typeParameters.Select((item) => item.Name).ToArray());

                    for(int j = 0; j < typeParameters.Length; j++) {
                        typeParameter = typeParameters[j];
                        typeParameterBuilder = typeParameterBuilders[j];

                        typeParameterBuilder.SetGenericParameterAttributes(typeParameter.GenericParameterAttributes);
                        foreach(Type constraint in typeParameter.GetGenericParameterConstraints()) {
                            if(constraint.IsClass) {
                                typeParameterBuilder.SetBaseTypeConstraint(constraint);
                            } else {
                                typeParameterBuilder.SetInterfaceConstraints(constraint);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Create Static Fields

            int filterIndex = 0;
            FieldBuilder[] staticFields = new FieldBuilder[classFilters.Length + methodFilters.Sum((item) => item.Length) + propertyFilters.Sum((item) => item.Length)];

            for(int i = 0; i < classFilters.Length; i++, filterIndex++) {
                staticFields[filterIndex] = typeBuilder.DefineField(this.GetStaticFieldName(filterIndex), typeof(IMethodFilter), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
            }
            for(int i = 0; i < methods.Length; i++) {
                if(methodFilters[i].Length == 0) {
                    continue;
                }

                for(int j = 0; j < methodFilters[i].Length; j++, filterIndex++) {
                    staticFields[filterIndex] = typeBuilder.DefineField(this.GetStaticFieldName(filterIndex), typeof(IMethodFilter), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
                }
            }
            for(int i = 0; i < properties.Length; i++) {
                if(propertyFilters[i].Length == 0) {
                    continue;
                }

                for(int j = 0; j < propertyFilters[i].Length; j++, filterIndex++) {
                    staticFields[filterIndex] = typeBuilder.DefineField(this.GetStaticFieldName(filterIndex), typeof(IMethodFilter), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
                }
            }

            #endregion

            #region Create Static Constructor

            string classKey = this.GetClassKey(sourceType, typeBuilder);
            IMethodFilter[] filters = g_classFilters[classKey] = new IMethodFilter[staticFields.Length];

            ConstructorBuilder constructorBuilder = typeBuilder.DefineTypeInitializer();
            generator = constructorBuilder.GetILGenerator();

            for(int i = 0; i < classFilters.Length; i++) {
                filters[i] = classFilters[i];

                generator.Emit(OpCodes.Ldstr, classKey);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Call, g_getClassFilterMethod);
                generator.Emit(OpCodes.Stsfld, staticFields[i]);
            }

            filterIndex = classFilters.Length;

            for(int i = 0; i < methods.Length; i++) {
                if(methodFilters[i].Length == 0) {
                    continue;
                }

                for(int j = 0; j < methodFilters[i].Length; j++, filterIndex++) {
                    filters[filterIndex] = methodFilters[i][j];

                    generator.Emit(OpCodes.Ldstr, classKey);
                    generator.Emit(OpCodes.Ldc_I4, filterIndex);
                    generator.Emit(OpCodes.Call, g_getClassFilterMethod);
                    generator.Emit(OpCodes.Stsfld, staticFields[filterIndex]);
                }
            }
            for(int i = 0; i < properties.Length; i++) {
                if(propertyFilters[i].Length == 0) {
                    continue;
                }

                for(int j = 0; j < propertyFilters[i].Length; j++, filterIndex++) {
                    filters[filterIndex] = propertyFilters[i][j];

                    generator.Emit(OpCodes.Ldstr, classKey);
                    generator.Emit(OpCodes.Ldc_I4, filterIndex);
                    generator.Emit(OpCodes.Call, g_getClassFilterMethod);
                    generator.Emit(OpCodes.Stsfld, staticFields[filterIndex]);
                }
            }

            generator.Emit(OpCodes.Ret);

            #endregion

            #region Create Instance Constructors

            ParameterInfo[] constructorParameters = null;
            foreach(ConstructorInfo constructor in sourceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                constructorParameters = constructor.GetParameters();
                constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorParameters.Select((item) => item.ParameterType).ToArray());
                for(int i = 0; i < constructorParameters.Length; i++) {
                    this.InjectCustomAttributes(
                        constructorParameters[i],
                        constructorBuilder.DefineParameter(i + 1, constructorParameters[i].Attributes, constructorParameters[i].Name));
                }

                generator = constructorBuilder.GetILGenerator();

                // call base constructor
                generator.Emit(OpCodes.Ldarg_0);
                for(int i = 1; i <= constructorParameters.Length; i++) {
                    generator.Emit(OpCodes.Ldarg, i);
                }
                generator.Emit(OpCodes.Call, constructor);

                generator.Emit(OpCodes.Ret);
            }

            #endregion

            #region Create Methods

            filterIndex = classFilters.Length;

            for(int i = 0; i < methods.Length; i++) {
                if(methodFilters[i].Length + classFilters.Length == 0) {
                    continue;
                }

                this.CreateProxyMethod(typeBuilder, methods[i], classFilters, methodFilters[i], staticFields, filterIndex, (item) => true);

                filterIndex += methodFilters[i].Length;
            }

            PropertyInfo property = null;
            PropertyBuilder propertyBuilder = null;
            MethodInfo getMethod = null, setMethod = null;
            bool isGetMethodOverridable = false, isSetMethodOverridable = false;
            bool isPropertyFilterGetMethod = false, isPropertyFilterSetMethod = false;
            bool isClassFilterGetMethod = classFilters.Any((item) => item.PropertyMethods.HasFlag(MethodFilterActOnPropertyMethods.GetMethod));
            bool isClassFilterSetMethod = classFilters.Any((item) => item.PropertyMethods.HasFlag(MethodFilterActOnPropertyMethods.SetMethod));

            for(int i = 0; i < properties.Length; i++) {
                if(propertyFilters[i].Length + classFilters.Length == 0) {
                    continue;
                }

                property = properties[i];
                isPropertyFilterGetMethod = propertyFilters[i].Any((item) => item.PropertyMethods.HasFlag(MethodFilterActOnPropertyMethods.GetMethod));
                isPropertyFilterSetMethod = propertyFilters[i].Any((item) => item.PropertyMethods.HasFlag(MethodFilterActOnPropertyMethods.SetMethod));

                isGetMethodOverridable = this.IsMethodOverridable(getMethod = property.GetGetMethod(true)) && (isClassFilterGetMethod || isPropertyFilterGetMethod);
                isSetMethodOverridable = this.IsMethodOverridable(setMethod = property.GetSetMethod(true)) && (isClassFilterSetMethod || isPropertyFilterSetMethod);
                if(!isGetMethodOverridable && !isSetMethodOverridable) {
                    continue;
                }

                propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);
                if(isGetMethodOverridable) {
                    propertyBuilder.SetGetMethod(this.CreateProxyMethod(typeBuilder, getMethod, classFilters, propertyFilters[i], staticFields, filterIndex, (item) => item.PropertyMethods.HasFlag(MethodFilterActOnPropertyMethods.GetMethod)));
                }
                if(isSetMethodOverridable) {
                    propertyBuilder.SetSetMethod(this.CreateProxyMethod(typeBuilder, setMethod, classFilters, propertyFilters[i], staticFields, filterIndex, (item) => item.PropertyMethods.HasFlag(MethodFilterActOnPropertyMethods.SetMethod)));
                }

                filterIndex += propertyFilters[i].Length;
            }

            #endregion

            return typeBuilder.CreateType();
        }

#if DEBUG

        /// <summary>
        /// Saves all generated types to a assembly and save to the specified file.
        /// </summary>
        /// <param name="filepath"></param>
        internal void SaveAssembly(string filepath) {
            this.m_assemblyBuilder.Save(!string.IsNullOrWhiteSpace(filepath) ? filepath : this.m_moduleBuilder.ScopeName);
        }

#endif

        #region IProxyTypeFactory 成员

        /// <inherit />
        public Type CreateProxyType(Type type) {
            return this.InternalCreateProxyType(type);
        }

        /// <inherit />
        public Type CreateProxyType<T>() {
            return this.InternalCreateProxyType(typeof(T));
        }

        #endregion
    }
}
