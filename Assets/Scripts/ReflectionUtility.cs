using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public static IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type type, bool includeAbstract = false)
    {
        return assembly.GetTypes().Where(
            t =>
            {
                if(includeAbstract != t.IsAbstract)
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
}