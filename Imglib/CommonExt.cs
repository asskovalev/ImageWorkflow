using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imglib
{
    internal static class CommonExt
    {
        public static T Do<T>(this T src, Action<T> fn)
        {
            fn(src);
            return src;
        }

        public static IEnumerable<int> Upto(this int from, int count)
        {
            return Enumerable.Range(from, count);
        }

        public static void Foreach<T>(this IEnumerable<T> src, Action<T> fn)
        {
            foreach (var item in src)
                fn(item);
        }

        public static void Foreach<T>(this IEnumerable<T> src, Action<T, int> fn)
        {
            int counter = 0;
            foreach (var item in src)
            {
                fn(item, counter);
                counter++;
            }
        }

        public static R With<T, R>(this T src, Func<T, R> fn)
        {
            return fn(src);
        }
    }
}
