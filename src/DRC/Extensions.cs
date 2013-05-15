using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DRC
{
    public static class Extensions
    {
        public static Dictionary<string, string[]> ToDictionary(this WebHeaderCollection headers)
        {
            var dict = new Dictionary<string, string[]>();

            for (var i = 0; i < headers.Count; ++i)
            {
                var header = headers.GetKey(i);
                dict.Add(header, headers.GetValues(i));
            }

            return dict;
        }

        public static T TakeNthOccurence<T> (this IEnumerable<T> source, Func<T, bool> predicate, int n)
        {
            var it = source.GetEnumerator();
            var count = 0;
            while (it.MoveNext())
            {
                if (predicate (it.Current)) 
                    count++;
                if (count == n) return it.Current;
            }
            throw new InvalidOperationException ("No element satisfies the condition in predicate. ");

        }

        public static bool IsInput (this Delegate inputDelegate)
        {
            return (inputDelegate.GetType ().Name.Contains ("Func") &&
                    inputDelegate.GetType ().GetGenericArguments ().First () == typeof (Response));
        }

        public static bool IsOutput (this Delegate outputDelegate)
        {
            return (outputDelegate.GetType ().Name.Contains ("Func") &&
                    outputDelegate.GetType ().GetGenericArguments ().Last () == typeof (Request));
        }

        public static IEnumerable<T> SkipLastN<T> (this IEnumerable<T> source, int n)
        {
            var it = source.GetEnumerator ();
            bool hasRemainingItems;
            var cache = new Queue<T> (n + 1);
            do
            {
                if (hasRemainingItems = it.MoveNext ())
                {
                    cache.Enqueue (it.Current);
                    if (cache.Count > n)
                        yield return cache.Dequeue ();
                }
            } while (hasRemainingItems);
        }

    }
}