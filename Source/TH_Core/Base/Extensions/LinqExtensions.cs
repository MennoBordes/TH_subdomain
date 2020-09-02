using System;
using System.Collections.Generic;
using System.Linq;

namespace TH.Core.Base.Extensions
{
    public static class LinqExtensions
    {
        /// <summary> Gets duplicate elements from this collection. </summary>
        public static IEnumerable<TSource> Duplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            IEnumerable<IGrouping<TKey, TSource>> grouped = source.GroupBy(selector);
            IEnumerable<IGrouping<TKey, TSource>> moreThen1 = grouped.Where(i => i.IsMultiple());
            return moreThen1.SelectMany(i => i);
        }

        /// <summary> Gets duplicate elements from this collection. </summary>
        public static IEnumerable<TSource> Duplicates<TSource, TKey>(this IEnumerable<TSource> source)
        {
            return source.Duplicates(i => i);
        }

        /// <summary> Checks if this collection contains multiple elements. </summary>
        /// <remarks> Using IsMultiple() in the Duplicates method is faster then Count() because this does not iterate the whole collection. </remarks>
        public static bool IsMultiple<T>(this IEnumerable<T> source)
        {
            IEnumerator<T> enumerator = source.GetEnumerator();
            return enumerator.MoveNext() && enumerator.MoveNext();
        }

        /// <summary> Looks for the first matching target. 
        /// <para> If match is not found, returns the first or default element. </para>
        /// <param name="checkSource"> Checks if the source is null and returns a default value, otherwise throws an exception. </param>
        /// </summary>
        public static TSource TargetFirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool checkSource = true)
        {
            if (source == null)
            {
                if (checkSource)
                {
                    return default(TSource);
                }
                else
                {
                    throw new NullReferenceException("Parameter 'source' can not be null.");
                }
            }

            if (source.Any(predicate))
            {
                return source.FirstOrDefault(predicate);
            }
            else
            {
                return source.FirstOrDefault();
            }
        }
    }
}
