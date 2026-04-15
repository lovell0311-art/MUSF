using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CustomFrameWork;

namespace ETModel
{
    public static class ReflectionTypeHelper
    {
        public static IEnumerable<Type> GetLoadableTypes(Assembly assembly, string context)
        {
            if (assembly == null)
            {
                yield break;
            }

            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = TryGetTypes(e);
                LogReflectionTypeLoadException(assembly, context, e);
            }

            if (types == null)
            {
                yield break;
            }

            foreach (Type type in types)
            {
                if (type != null)
                {
                    yield return type;
                }
            }
        }

        private static void LogReflectionTypeLoadException(Assembly assembly, string context, ReflectionTypeLoadException e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ReflectionTypeLoadException");
            builder.Append(" context=").Append(context ?? "unknown");
            builder.Append(" assembly=").Append(assembly.FullName);
            builder.Append(" message=").Append(e.Message);

            Exception[] loaderExceptions = TryGetLoaderExceptions(e);
            if (loaderExceptions != null)
            {
                for (int i = 0; i < loaderExceptions.Length; ++i)
                {
                    Exception loaderException = loaderExceptions[i];
                    if (loaderException == null)
                    {
                        continue;
                    }

                    builder.Append("\nLoaderException[")
                        .Append(i)
                        .Append("] ")
                        .Append(loaderException.GetType().Name)
                        .Append(": ")
                        .Append(loaderException.Message);

                    if (loaderException is FileNotFoundException fileNotFoundException &&
                        !string.IsNullOrWhiteSpace(fileNotFoundException.FileName))
                    {
                        builder.Append(" file=").Append(fileNotFoundException.FileName);
                    }
                }
            }

            Log.Error(builder.ToString());
        }

        private static Type[] TryGetTypes(ReflectionTypeLoadException e)
        {
            object value = TryGetMemberValue(e, "Types", "types", "_types", "_classes", "classes");
            if (value is Type[] types)
            {
                return types;
            }

            return TryFindMemberValue<Type[]>(e, name => name.IndexOf("type", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static Exception[] TryGetLoaderExceptions(ReflectionTypeLoadException e)
        {
            object value = TryGetMemberValue(e, "LoaderExceptions", "loaderExceptions", "_loaderExceptions");
            if (value is Exception[] loaderExceptions)
            {
                return loaderExceptions;
            }

            return TryFindMemberValue<Exception[]>(e, name =>
                name.IndexOf("loader", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("exception", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static object TryGetMemberValue(object instance, params string[] memberNames)
        {
            if (instance == null)
            {
                return null;
            }

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            for (Type type = instance.GetType(); type != null; type = type.BaseType)
            {
                foreach (string memberName in memberNames)
                {
                    PropertyInfo propertyInfo = type.GetProperty(memberName, bindingFlags);
                    if (propertyInfo != null && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        try
                        {
                            return propertyInfo.GetValue(instance, null);
                        }
                        catch
                        {
                        }
                    }

                    FieldInfo fieldInfo = type.GetField(memberName, bindingFlags);
                    if (fieldInfo != null)
                    {
                        try
                        {
                            return fieldInfo.GetValue(instance);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return null;
        }

        private static T TryFindMemberValue<T>(object instance, Func<string, bool> namePredicate) where T : class
        {
            if (instance == null)
            {
                return null;
            }

            T value = TryFindMemberValueCore<T>(instance, namePredicate);
            if (value != null || namePredicate == null)
            {
                return value;
            }

            return TryFindMemberValueCore<T>(instance, null);
        }

        private static T TryFindMemberValueCore<T>(object instance, Func<string, bool> namePredicate) where T : class
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            for (Type type = instance.GetType(); type != null; type = type.BaseType)
            {
                foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
                {
                    if (namePredicate != null && !namePredicate(fieldInfo.Name))
                    {
                        continue;
                    }

                    try
                    {
                        if (fieldInfo.GetValue(instance) is T fieldValue)
                        {
                            return fieldValue;
                        }
                    }
                    catch
                    {
                    }
                }

                foreach (PropertyInfo propertyInfo in type.GetProperties(bindingFlags))
                {
                    if (propertyInfo.GetIndexParameters().Length != 0)
                    {
                        continue;
                    }

                    if (namePredicate != null && !namePredicate(propertyInfo.Name))
                    {
                        continue;
                    }

                    try
                    {
                        if (propertyInfo.GetValue(instance, null) is T propertyValue)
                        {
                            return propertyValue;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return null;
        }
    }
}
