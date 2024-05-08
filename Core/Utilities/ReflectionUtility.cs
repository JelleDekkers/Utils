using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils.Core
{
    public static class ReflectionUtility
    {
        public static IEnumerable<Type> GetAllTypes(Type type, bool checkAllAssemblies = false, bool includeAbstract = false)
        {
            if (!checkAllAssemblies)
            {
                return GetDerivedTypes(typeof(ReflectionUtility).Assembly, type, includeAbstract);
            }

            List<Type> types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                types.AddRange(GetDerivedTypes(assembly, type, includeAbstract));
            }

            return types;
        }

		public static bool HasDefaultConstructor(this Type t)
		{
			return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
		}

        public static IEnumerable<Type> GetDerivedTypes(Type type, bool includeAbstract = false)
        {
            return GetDerivedTypes(typeof(ReflectionUtility).Assembly, type, includeAbstract);
        }

        public static IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type type, bool includeAbstract = false)
        {
            return assembly.GetTypes().Where(
                t =>
                {
                    if (includeAbstract != t.IsAbstract)
                        return false;

                    return (t != type && type.IsAssignableFrom(t));
                }
            );
        }

        public static IEnumerable<FieldInfo> GetFieldInfos(Type type)
        {
            List<FieldInfo> fieldInfo = new List<FieldInfo>(type.GetFields());
            if (type.BaseType != null)
            {
                fieldInfo.AddRange(GetFieldInfos(type.BaseType));
            }

            return fieldInfo;
        }

        public static object Instantiate(Type classType, params object[] parameters)
        {
            return Activator.CreateInstance(classType, parameters);
        }

        public static object Instantiate(string classType, params object[] parameters)
        {
            Type type = Type.GetType(classType, true);
            if (type == null)
            {
                throw new ArgumentException(classType + " is not a valid class type.", "classType");
            }

            return Instantiate(type, parameters);
        }

        /// <summary>
        /// Returns a deep copy of an object, including all its fields with their values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T original) where T : class
        {
            if (original == null)
                return null;

            Type type = original.GetType();
            T copy = Activator.CreateInstance(type) as T;
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                if (!field.IsStatic && !field.IsLiteral)
                {
                    object value = field.GetValue(original);
                    field.SetValue(copy, value);
                }
            }

            return copy;
        }
    }
}