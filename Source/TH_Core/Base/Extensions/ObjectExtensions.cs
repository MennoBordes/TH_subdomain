﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TH.Core.Base.Extensions.ArrayExtensions;

namespace TH.Core.Base.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary> Cast object A to object B (preferred method for downcasting). </summary>
        public static T Cast<T>(this object obj) where T : class
        {
            // NULL Check
            if (obj == null)
                return null;

            // Create Instance
            T instance = Activator.CreateInstance<T>();

            Type foundType = obj.GetType();

            // Get source properties
            PropertyInfo[] properties = foundType.GetProperties();

            // Try copy each property value
            foreach (PropertyInfo pSource in properties)
            {
                try
                {
                    PropertyInfo pTarget = typeof(T).GetProperty(pSource.Name);
                    pTarget.SetValue(instance, pSource.GetValue(obj, null), null);
                }
                catch { }
            }

            return instance;
        }

        /// <summary> Merge properties of object B into object A. </summary>
        public static void Merge(this object objA, object objB)
        {
            // NULL Check
            if (objA == null || objB == null)
                return;

            Type typeA = objA.GetType();
            Type typeB = objB.GetType();

            // Get source properties
            PropertyInfo[] propertiesA = typeA.GetProperties();

            // Try copy each property value
            foreach (PropertyInfo pSourceA in propertiesA)
            {
                try
                {
                    PropertyInfo pObjA = typeA.GetProperty(pSourceA.Name);
                    PropertyInfo pObjB = typeB.GetProperty(pSourceA.Name);

                    object pB = pObjB.GetValue(objB, null);

                    pObjA.SetValue(objA, pB, null);
                }
                catch { }
            }
        }

        /// <summary> Checks if the object is of type enumerable. </summary>        
        public static bool IsEnumerable(this object obj)
        {
            if (obj != null)
            {
                if (obj.GetType().GetInterfaces().Any(
                        i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary> Get Property Info. </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo Property<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            //if (type != propInfo.ReflectedType &&
            //    !type.IsSubclassOf(propInfo.ReflectedType))
            //    throw new ArgumentException(string.Format(
            //        "Expresion '{0}' refers to a property that is not from type {1}.",
            //        propertyLambda.ToString(),
            //        type));

            return propInfo;
        }


        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String))
                return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null)
                return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
                return originalObject;
            if (visited.ContainsKey(originalObject))
                return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
                return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false)
                    continue;
                if (IsPrimitive(fieldInfo.FieldType))
                    continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }

        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null)
                return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0)
                    return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do
                    action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
