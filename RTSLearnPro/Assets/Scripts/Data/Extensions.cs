using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Random = System.Random;

namespace G13Kit
{
    public static class Extensions
    {
        private static readonly Random rng = new Random();

        public static bool IsNullOrEmpty(this JToken token)
        {
            if (token == null)
            {
                return true;
            }

            return string.IsNullOrEmpty(token.ToString());
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T As<T>(this JToken token)
        {
            return (T)Activator.CreateInstance(typeof(T), token as JObject);
        }

        public static string[] SplitByNewLine(this string str)
        {
            return str.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}