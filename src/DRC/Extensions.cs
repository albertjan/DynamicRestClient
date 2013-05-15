using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DRC
{
    public static class Extensions
    {
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
                    inputDelegate.GetType ().GetGenericArguments ().First () == typeof (WebResponse));
        }

        public static bool IsOutput (this Delegate outputDelegate)
        {
            return (outputDelegate.GetType ().Name.Contains ("Func") &&
                    outputDelegate.GetType ().GetGenericArguments ().Last () == typeof (ClientRequest));
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