//-----------------------------------------------------------------------------
// <copyright file="MathHelper.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Helper for math related functions.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Calculates the long sum of a source using a selector function.
        /// </summary>
        /// <typeparam name="TSource">Type to convert</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="selector">Selector function</param>
        /// <returns>Sum of elements selected</returns>
        public static ulong Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> selector)
        {
            var sum = 0UL;
            foreach (var element in source)
            {
                sum += selector(element);
            }

            return sum;
        }

        /// <summary>
        /// Clips the lower and upper bound of a value.
        /// </summary>
        /// <param name="val">Value to clip</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>Clipped value</returns>
        public static int Clip(this int val, int lower, int upper)
        {
            return Math.Max(lower, Math.Min(upper, val));
        }

        /// <summary>
        /// Clips the lower and upper bound of a value.
        /// </summary>
        /// <param name="val">Value to clip</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>Clipped value</returns>
        public static byte Clip(this byte val, byte lower, byte upper)
        {
            return Math.Max(lower, Math.Min(upper, val));
        }

        /// <summary>
        /// Changes the value up or down by delta.
        /// </summary>
        /// <param name="val">Value to change</param>
        /// <param name="delta">Amount to change</param>
        /// <param name="r">Random number generator</param>
        /// <returns>Modified value</returns>
        public static int Change(this int val, int delta, Random r = null)
        {
            r = r ?? new Random();
            return (byte)((int)val + r.Next(-delta, delta + 1));
        }

        /// <summary>
        /// Changes the value up or down by delta.
        /// </summary>
        /// <param name="val">Value to change</param>
        /// <param name="delta">Amount to change</param>
        /// <param name="r">Random number generator</param>
        /// <returns>Modified value</returns>
        public static byte Change(this byte val, byte delta, Random r = null)
        {
            r = r ?? new Random();
            return (byte)(val + r.Next(-delta, delta + 1));
        }

        /// <summary>
        /// Generates a random unsigned long
        /// </summary>
        /// <param name="r">Random number generator</param>
        /// <returns>Randomly generated unsigned long</returns>
        public static ulong NextULong(this Random r)
        {
            var buffer = new byte[8];
            r.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
