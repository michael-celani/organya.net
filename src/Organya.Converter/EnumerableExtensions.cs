using System;
using System.Collections.Generic;
using RangeTree;

namespace Organya.Converter
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Splits an enumerable at a predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A grouping of enumerables split at the predicate.</returns>
        public static IEnumerable<IEnumerable<T>> SplitBefore<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            IList<T> buffer = new List<T>();

            foreach (T item in enumerable)
            {
                if (predicate(item) && buffer.Count > 0)
                {
                    yield return buffer;

                    buffer = new List<T>();
                }

                buffer.Add(item);
            }

            if (buffer.Count > 0)
            {
                yield return buffer;
            }   
        }

        /// <summary>
        /// Constructs a <see cref="RangeTree"/> from an enumerable.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys of the tree.</typeparam>
        /// <typeparam name="TValue">The type of the values of the tree.</typeparam>
        /// <param name="values">The values to be used in the tree.</param>
        /// <param name="startSelector">A function that returns the start of the range of a value.</param>
        /// <param name="endSelector">A function that returns the end of the range of a value.</param>
        /// <returns>The <see cref="IRangeTree{TKey, TValue}"/> containing the values.</returns>
        public static IRangeTree<TKey, TValue> ToRangeTree<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> startSelector, Func<TValue, TKey> endSelector)
        {
            IRangeTree<TKey, TValue> rangeTree = new RangeTree<TKey, TValue>();

            foreach (TValue value in values)
            {
                TKey start = startSelector(value);
                TKey end = endSelector(value);
                rangeTree.Add(start, end, value);
            }

            return rangeTree;
        }
    }
}
