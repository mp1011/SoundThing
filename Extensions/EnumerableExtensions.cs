using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> AllCombinations<T>(this IEnumerable<T> list, int minLength, int maxLength) =>
            Enumerable.Range(minLength, maxLength - minLength)
                      .SelectMany(p => list.AllCombinations(p));

        public static IEnumerable<T[]> AllCombinations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 0 || list.Count() == 0)
                return Array.Empty<T[]>();

            if(length == 1)
                return list.Select(p => new T[] { p });
 
            return list.SelectMany((p, i) =>
            {
                var start = new T[] { p };
                var tails = list.Skip(i + 1).AllCombinations(length - 1);

                return tails.Select(t => start.Concat(t).ToArray());
            });
        }

    }
}
