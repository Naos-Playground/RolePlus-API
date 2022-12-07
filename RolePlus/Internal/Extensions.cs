// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class Extensions
    {
        internal static T Random<T>(this IEnumerable<T> collections)
        {
            T[] enumerable = collections as T[] ?? collections.ToArray();
            return !enumerable.Any() ? default : enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Length));
        }

        internal static T Random<T>(this IEnumerable<T> collections, IEnumerable<T> targetCollection)
        {
            T[] enumerable = targetCollection as T[] ?? targetCollection.ToArray();
            return !enumerable.Any() ? default : enumerable.ElementAt(UnityEngine.Random.Range(0, targetCollection.Count()));
        }

        internal static bool EvaluateProbability(this int chance) => UnityEngine.Random.Range(0, 100) <= chance;

        internal static bool EvaluateProbability(this float chance) => UnityEngine.Random.Range(0, 101f) <= chance;

        internal static float Randomize(this float data, float min, float max) => UnityEngine.Random.Range(data - min, data + max);

        internal static int Randomize(this int data, int min, int max) => UnityEngine.Random.Range(data - min, data + max);
    }
}