using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace TH.Server.Base.Extensions
{
    /// <summary> Contains extension methods for different types of collections. </summary>
    public static class CollectionExtensions
    {
        /// <summary> Checks if this string collection has duplicate values. </summary>
        public static bool HasDuplicates(this string[] input)
        {
            List<string> values = new List<string>();

            bool hasDuplicates = false;

            foreach (string value in input)
            {
                if (values.Contains(value))
                {
                    hasDuplicates = true;
                    break;
                }
                values.Add(value);
            }

            return hasDuplicates;
        }

        /// <summary> Checks if this string collection has duplicate values. </summary>
        public static bool HasDuplicates(this List<string> input)
        {
            return input.ToArray().HasDuplicates();
        }

        /// <summary> Checks if this string collection has duplicate values. </summary>
        public static bool HasDuplicates(this IEnumerable<string> input)
        {
            return input.ToArray().HasDuplicates();
        }

        /// <summary> Safe ForEach method for IEnumerable collections. </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) return;

            IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action.Invoke(enumerator.Current);
            }
        }

        /// <summary> Safe ForEach method for ICollection collections. </summary>
        public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
        {
            if (collection == null)
                return;

            IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action.Invoke(enumerator.Current);
            }
        }

        /// <summary> Safe ForEach method for InternalDataCollectionBase collections. </summary>
        public static void ForEach(this InternalDataCollectionBase collection, Action<DataColumn> action)
        {
            if (collection == null)
                return;

            IEnumerator enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action.Invoke((DataColumn)enumerator.Current);
            }
        }

        /// <summary> Concats this array with specified array into a new array. </summary>
        public static T[] Concat<T>(this T[] x, T[] y)
        {
            if (x == null)
                throw new ArgumentNullException("x");
            if (y == null)
                throw new ArgumentNullException("y");

            int oldLen = x.Length;
            Array.Resize<T>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }

        /// <summary> Checks if this collection is null or empty. </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary> Indicator if specified key is present. </summary>
        public static bool HasKey(this NameValueCollection collection, string key)
        {
            if (collection == null)
                return false;

            return collection.AllKeys.Contains(key);
        }


        /// <summary> Clone list using serialization. </summary>
        public static List<T> CloneList<T>(this List<T> oldList)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<T>)formatter.Deserialize(stream);
        }

        /// <summary> Copy list using iCloneable. </summary>        
        public static List<T> CopyList<T>(this List<T> list) where T : ICloneable
        {
            var newList = new List<T>(list.Count);

            foreach (T item in list)
            {
                newList.Add((T)item.Clone());
            }

            return newList;
        }
    }
}
