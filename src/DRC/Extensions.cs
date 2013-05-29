namespace DRC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public static class Extensions
    {
        private static readonly HashSet<Type> Set = new HashSet<Type>
        {
            typeof(Func<>),typeof(Func<,>),typeof(Func<,,>),typeof(Func<,,,>),typeof(Func<,,,,>),typeof(Func<,,,,,>),typeof(Func<,,,,,,>),typeof(Func<,,,,,,,>),typeof(Func<,,,,,,,,>),typeof(Func<,,,,,,,,,>),typeof(Func<,,,,,,,,,,>),typeof(Func<,,,,,,,,,,,>),typeof(Func<,,,,,,,,,,,,>),typeof(Func<,,,,,,,,,,,,,>),typeof(Func<,,,,,,,,,,,,,,>),typeof(Func<,,,,,,,,,,,,,,,>),typeof(Func<,,,,,,,,,,,,,,,,>)
        };
        
        public static bool IsFunc(this Type type, int minimalNumberOfTypeArguments = 1)
        {
            return (Set.Contains(type) || (type.IsGenericType && Set.Contains(type.GetGenericTypeDefinition()) && type.GetGenericArguments().Count() >= minimalNumberOfTypeArguments));
        }

        public static bool IsFunc(this Delegate fDelegate, int minimalNumberOfTypeArguments = 1)
        {
            return fDelegate.GetType().IsFunc(minimalNumberOfTypeArguments);
        }

        public static bool Implements(this Type type, Type implementedInterface)
        {
            if (implementedInterface.IsInterface)
            {
                return type.GetInterfaces().Contains(implementedInterface);
            }
            throw new ArgumentException("Must be an interface", "implementedInterface");
        }

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
            return (inputDelegate.IsFunc() &&
                    inputDelegate.GetType ().GetGenericArguments ().First () == typeof (Response));
        }

        public static bool IsOutput (this Delegate outputDelegate)
        {
            return (outputDelegate.IsFunc() &&
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