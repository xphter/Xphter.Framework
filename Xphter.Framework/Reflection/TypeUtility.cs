using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xphter.Framework.Collections;
using Xphter.Framework.IO;

namespace Xphter.Framework.Reflection {
    /// <summary>
    /// Provides functions to access a Type object.
    /// </summary>
    public static class TypeUtility {
        static TypeUtility() {
            g_integerTypes = new List<Type> {
                ByteType,
                UByteType,
                ShortType,
                UShortType,
                IntType,
                UintType,
                LongType,
                ULongType,
            };

            g_numberTypes = new List<Type> {
                ByteType,
                UByteType,
                ShortType,
                UShortType,
                IntType,
                UintType,
                LongType,
                ULongType,
                FloatType,
                DoubleType,
                DecimalType,
            };
        }

        /// <summary>
        /// The integer types list.
        /// </summary>
        private static readonly ICollection<Type> g_integerTypes;

        /// <summary>
        /// The number types list.
        /// </summary>
        private static readonly ICollection<Type> g_numberTypes;

        #region Data Types

        #region Number Types

        public static readonly Type ByteType = typeof(byte);
        public static readonly Type UByteType = typeof(sbyte);
        public static readonly Type ShortType = typeof(short);
        public static readonly Type UShortType = typeof(ushort);
        public static readonly Type IntType = typeof(int);
        public static readonly Type UintType = typeof(uint);
        public static readonly Type LongType = typeof(long);
        public static readonly Type ULongType = typeof(ulong);
        public static readonly Type FloatType = typeof(float);
        public static readonly Type DoubleType = typeof(double);
        public static readonly Type DecimalType = typeof(decimal);

        #endregion

        public static readonly Type BooleanType = typeof(bool);

        public static readonly Type CharType = typeof(char);

        public static readonly Type StringType = typeof(string);

        public static readonly Type DateTimeType = typeof(DateTime);

        public static readonly Type NullableType = typeof(Nullable<>);

        public static readonly Type IEnumerableType = typeof(IEnumerable);

        #endregion

        public static bool Is<T>(this Type type) {
            return Is(type, typeof(T));
        }

        public static bool Is(this Type leftType, Type rightType) {
            if(rightType == null) {
                throw new ArgumentNullException("rightType");
            }

            if(object.ReferenceEquals(leftType, rightType) || leftType.Equals(rightType)) {
                return true;
            }

            if(rightType.IsInterface) {
                return rightType.IsAssignableFrom(leftType) || leftType.GetInterface(rightType.FullName) != null;
            } else {
                if(leftType.IsInterface) {
                    return false;
                }

                bool result = false;
                Type subType = leftType;
                do {
                    if(result = rightType.Equals(subType) || rightType.IsAssignableFrom(subType)) {
                        break;
                    }
                } while((subType = subType.BaseType) != null);

                return result;
            }
        }

        /// <summary>
        /// Determines whether this type has implemented the specifed interface type.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if this type has implemented <typeparamref name="I"/>, otherwise return false.</returns>
        /// <exception cref="System.ArgumentException"><typeparamref name="I"/> is not a interface type.</exception>
        public static bool IsImplements<I>(this Type type) where I : class {
            Type interfaceType = typeof(I);
            if(!interfaceType.IsInterface) {
                throw new ArgumentException("I is not a interface type.", "I");
            }

            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether this type has inherited the specifed class type.
        /// </summary>
        /// <typeparam name="T">The class type.</typeparam>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if this type is the same as <typeparamref name="T"/> or has inherited <typeparamref name="T"/>, otherwise return false.</returns>
        /// <exception cref="System.ArgumentException"><typeparamref name="T"/> is a interface type.</exception>
        public static bool IsInherits<T>(this Type type) where T : class {
            Type classType = typeof(T);
            if(classType.IsInterface) {
                throw new ArgumentException("T is a interface type.", "T");
            }

            return type.Equals(classType) || type.IsSubclassOf(classType);
        }

        /// <summary>
        /// Determines whether this type is a integer type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a integer type, otherwise return false.</returns>
        public static bool IsInteger(this Type type) {
            return g_integerTypes.Contains(type);
        }

        /// <summary>
        /// Determines whether this type is a number type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a number type, otherwise return false.</returns>
        public static bool IsNumber(this Type type) {
            return g_numberTypes.Contains(type);
        }

        /// <summary>
        /// Determines whether this type is a byte type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a byte type, otherwise return false.</returns>
        public static bool IsByte(this Type type) {
            return type.Equals(ByteType) || type.Equals(UByteType);
        }

        /// <summary>
        /// Determines whether this type is a 32 bits integer type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a 32 bits integer type, otherwise return false.</returns>
        public static bool IsInt32(this Type type) {
            return type.Equals(IntType);
        }

        /// <summary>
        /// Determines whether this type is a 64 bits integer type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a 64 bits integer type, otherwise return false.</returns>
        public static bool IsInt64(this Type type) {
            return type.Equals(LongType);
        }

        /// <summary>
        /// Determines whether this type is a single float type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a single float type, otherwise return false.</returns>
        public static bool IsSingle(this Type type) {
            return type.Equals(FloatType);
        }

        /// <summary>
        /// Determines whether this type is a double float type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a double float type, otherwise return false.</returns>
        public static bool IsDouble(this Type type) {
            return type.Equals(DoubleType);
        }

        /// <summary>
        /// Determines whether this type is a Decimal type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a Decimal type, otherwise return false.</returns>
        public static bool IsDecimal(this Type type) {
            return type.Equals(DecimalType);
        }

        /// <summary>
        /// Determines whether this type is a boolean type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a boolean type, otherwise return false.</returns>
        public static bool IsBoolean(this Type type) {
            return type.Equals(BooleanType);
        }

        /// <summary>
        /// Determines whether this type is a string type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a string type, otherwise return false.</returns>
        public static bool IsString(this Type type) {
            return type.Equals(StringType);
        }

        /// <summary>
        /// Determines whether this type is a char type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a char type, otherwise return false.</returns>
        public static bool IsChar(this Type type) {
            return type.Equals(CharType);
        }

        /// <summary>
        /// Determines whether this type is a DateTime type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a DateTime type, otherwise return false.</returns>
        public static bool IsDateTime(this Type type) {
            return type.Equals(DateTimeType);
        }

        /// <summary>
        /// Determines whether this type is a Nullable type.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a Nullable type, otherwise return false.</returns>
        public static bool IsNullable(this Type type) {
            return type.IsGenericType && !type.ContainsGenericParameters && type.GetGenericTypeDefinition().Equals(NullableType);
        }

        /// <summary>
        /// Determines whether this type is a IEnumerable type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this Type type) {
            return IEnumerableType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether this type is a simple type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimple(this Type type) {
            if(IsBoolean(type) || IsNumber(type) || IsChar(type) || IsString(type) || IsDateTime(type) || type.IsEnum) {
                return true;
            }

            if(IsNullable(type)) {
                return IsSimple(type.GetGenericArguments()[0]);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this type is a public concrete class.
        /// </summary>
        /// <param name="type">A Type object.</param>
        /// <returns>Return true if <typeparamref name="type"/> is a public concrete class, otherwise return false.</returns>
        public static bool IsPublicConcreteClass(this Type type) {
            return (type.IsNested ? type.IsNestedPublic : type.IsPublic) && type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition && !type.ContainsGenericParameters;
        }

        public static MethodInfo GetGenericMethod<TObj>(Expression<Action<TObj>> expression, BindingFlags flags, Type[] argumentTypes) {
            return GetGenericMethod(typeof(TObj), GetMemberName<TObj>(expression), flags, argumentTypes);
        }

        public static MethodInfo GetGenericMethod<TObj, TReturn>(Expression<Func<TObj, TReturn>> expression, BindingFlags flags, Type[] argumentTypes) {
            return GetGenericMethod(typeof(TObj), GetMemberName<TObj, TReturn>(expression), flags, argumentTypes);
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Type[] argumentTypes) {
            MethodInfo method = null;

            foreach(MethodInfo item in type.GetMethods(flags)) {
                if(item.IsGenericMethod &&
                    item.ContainsGenericParameters &&
                    item.Name.Equals(name) &&
                    item.GetParameters().Select((obj) => obj.ParameterType).Where((obj) => !obj.IsGenericParameter).Equals<Type>(argumentTypes)) {
                    method = item;
                    break;
                }
            }

            return method;
        }

        /// <summary>
        /// Searchs types in ths specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <typeparam name="T">The base type.</typeparam>
        public static IEnumerable<Type> FilterTypesInAssemblies<T>(IEnumerable<Assembly> assemblies) {
            Type t = typeof(T);
            return FilterTypesInAssemblies(assemblies, (item) => item.IsPublicConcreteClass() && t.IsAssignableFrom(item) && item.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 0);
        }

        /// <summary>
        /// Searchs types in ths specified assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FilterTypesInAssemblies(IEnumerable<Assembly> assemblies, Predicate<Type> predicate) {
            Type[] types = null;
            IEnumerable<Type> allTypes = Type.EmptyTypes;

            foreach(Assembly assembly in assemblies) {
                if(assembly == null) {
                    continue;
                }

                try {
                    types = assembly.GetTypes();
                } catch(ReflectionTypeLoadException ex) {
                    types = ex.Types;
                }
                allTypes = allTypes.Concat(types);
            }

            return allTypes.Where((item) => item != null && predicate(item));
        }

        /// <summary>
        /// Loads types from the specified assemblies and creates instance of each type use the default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<T> LoadInstances<T>(IEnumerable<Assembly> assemblies) {
            if(assemblies == null) {
                throw new ArgumentException("The assemblies is null.", "assemblies");
            }

            return TypeUtility.FilterTypesInAssemblies<T>(assemblies).Where((item) => item.GetConstructor(Type.EmptyTypes) != null).Select((item) => (T) Activator.CreateInstance(item));
        }

        /// <summary>
        /// Loads types from assemblies in the specified folder and creates instance of each type use the default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="includeSubfolder"></param>
        /// <returns></returns>
        public static IEnumerable<T> LoadInstancesFromFolder<T>(string folderPath, bool includeSubfolder) {
            if(!PathUtility.IsValidLocalPath(folderPath)) {
                throw new ArgumentException("The folder path is not a valid local file path.", "folderPath");
            }

            return TypeUtility.LoadInstances<T>(Directory.GetFiles(folderPath, "*.dll", includeSubfolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select((item) => {
                try {
                    return Assembly.LoadFile(item);
                } catch(BadImageFormatException) {
                    return null;
                }
            }));
        }

        /// <summary>
        /// Gets associated member name of the specified expression which must be a method call expression or member access expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null or is a neither method call expression nor member access expression.</exception>
        public static string GetMemberName<T>(Expression<Action<T>> expression) {
            if(expression == null) {
                throw new ArgumentException("The expression is null.", "expression");
            }

            switch(expression.Body.NodeType) {
                case ExpressionType.Call:
                    return ((MethodCallExpression) expression.Body).Method.Name;
                case ExpressionType.MemberAccess:
                    return ((MemberExpression) expression.Body).Member.Name;
                default:
                    throw new ArgumentException("The expression is not a method call expression or member access expression.", "expression");
            }
        }

        /// <summary>
        /// Gets associated member name of the specified expression which must be a method call expression or member access expression.
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null or is a neither method call expression nor member access expression.</exception>
        public static string GetMemberName<TObj, TReturn>(Expression<Func<TObj, TReturn>> expression) {
            if(expression == null) {
                throw new ArgumentException("The expression is null.", "expression");
            }

            switch(expression.Body.NodeType) {
                case ExpressionType.Call:
                    return ((MethodCallExpression) expression.Body).Method.Name;
                case ExpressionType.MemberAccess:
                    return ((MemberExpression) expression.Body).Member.Name;
                default:
                    throw new ArgumentException("The expression is not a method call expression or member access expression.", "expression");
            }
        }

        /// <summary>
        /// Gets the description of associated member of the specified expression which must be a method call expression or member access expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null or is a neither method call expression nor member access expression.</exception>
        public static string GetMemberDescription<T>(Expression<Action<T>> expression) {
            if(expression == null) {
                throw new ArgumentException("The expression is null.", "expression");
            }

            DescriptionAttribute description = (DescriptionAttribute) typeof(T).GetMember(GetMemberName<T>(expression), BindingFlags.Instance | BindingFlags.Public).First().GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return description != null ? description.Description : null;
        }

        /// <summary>
        /// Gets the description of associated member of the specified expression which must be a method call expression or member access expression.
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="expression"/> is null or is a neither method call expression nor member access expression.</exception>
        public static string GetMemberDescription<TObj, TReturn>(Expression<Func<TObj, TReturn>> expression) {
            if(expression == null) {
                throw new ArgumentException("The expression is null.", "expression");
            }

            DescriptionAttribute description = (DescriptionAttribute) typeof(TObj).GetMember(GetMemberName<TObj, TReturn>(expression), BindingFlags.Instance | BindingFlags.Public).First().GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return description != null ? description.Description : null;
        }

        /// <summary>
        /// Gets the element type of a IEnumerable object.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Type GetEnumerableElementType(IEnumerable<object> list) {
            if(list == null) {
                throw new ArgumentNullException("list");
            }

            return GetEnumerableElementType(list.GetType());
        }

        /// <summary>
        /// Gets the element type of a IEnumerable object.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Type GetEnumerableElementType(Type type) {
            if(type == null) {
                throw new ArgumentNullException("type");
            }

            Type definition = typeof(IEnumerable<>);

            if(type.IsArray) {
                return type.GetElementType();
            }
            if(type.IsGenericType && type.GetGenericTypeDefinition() == definition) {
                return type.GetGenericArguments()[0];
            }

            type = type.GetInterfaces().Where((item) => item.IsGenericType && item.GetGenericTypeDefinition() == definition).FirstOrDefault();
            if(type != null) {
                return type.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
