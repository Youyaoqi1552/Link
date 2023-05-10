using System;
using System.Collections.Generic;

namespace Common.Extensions
{
    public static class CollectionExtensions
    {
        public static int Choice<T>(
            this T[] source)
        {
            if (source.Length <= 1)
            {
                return 0;
            }
            return UnityEngine.Random.Range(0, source.Length);
        }
        
        public static int Choice<T>(
            this List<T> source)
        {
            if (source.Count <= 1)
            {
                return 0;
            }
            return UnityEngine.Random.Range(0, source.Count);
        }
        
        public static void Shuffle<T>(
            this T[] source)
        {
            if (source.Length <= 1)
            {
                return;
            }

            var random = new Random();
            for (var i = source.Length - 1; i > 0; i--)
            {
                var idx = random.Next(i - 1);
                (source[i], source[idx]) = (source[idx], source[i]);
            }
        }
        
        public static void Shuffle<T>(
            this List<T> source)
        {
            if (source.Count <= 1)
            {
                return;
            }

            var random = new Random();
            for (var i = source.Count - 1; i > 0; i--)
            {
                var idx = random.Next(i - 1);
                (source[i], source[idx]) = (source[idx], source[i]);
            }
        }
        
        public static void Shuffle<TKey, TValue>(
            this Dictionary<TKey, TValue> source)
        {
            if (source.Count <= 1)
            {
                return;
            }
            
            var random = new Random();
            var keys = new List<TKey>(source.Keys);
            for (var i = keys.Count - 1; i > 0; i--)
            {
                var idx = random.Next(i - 1);
                (source[keys[i]], source[keys[idx]]) = (source[keys[idx]], source[keys[i]]);
            }
        }
    }
}
