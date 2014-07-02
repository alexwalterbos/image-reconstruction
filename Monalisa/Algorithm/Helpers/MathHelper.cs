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

        // return a number : number < 0 concave,  number > 0 convex, number = 0 if num_vertices < 3 
        public static bool IsConcave(this Polygon shape)
        {
            var polygon = shape.Clone() as Polygon;
            int num_vertices = polygon.Coordinates.Count;
            double cur_det_value;

            for (int i = 0; i < num_vertices; i++)
            {
                var coord = polygon.Coordinates[i];
                var int1 = coord.Item1;
                var int2 = coord.Item2;
                polygon.Coordinates[i] = new Tuple<int, int>(int1, int2);
                
            }

            Tuple<int, int> v1 = CalcVector(polygon.Coordinates[0], polygon.Coordinates[num_vertices - 1]);
            Tuple<int, int> v2 = CalcVector(polygon.Coordinates[1], polygon.Coordinates[0]);
            double det_value = CalcDet(v1, v2);

            for (int i = 1; i<num_vertices-1; i++){
                v1 = v2;
                v2 = CalcVector(polygon.Coordinates[i+1],polygon.Coordinates[i]);
                cur_det_value = CalcDet(v1,v2);

                if( (cur_det_value * det_value) < 0.0)
                {
                    return true;
                }
            }

            v1 = v2;
            v2 = CalcVector(polygon.Coordinates[0], polygon.Coordinates[num_vertices - 1]);
            cur_det_value = CalcDet(v1, v2);

            if ( (cur_det_value * det_value) < 0.0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Calculate the difference vector
        public static Tuple<int,int> CalcVector(Tuple<int,int> v1, Tuple<int,int> v2)
        {
            var vector = new Tuple<int,int>(v1.Item1 - v2.Item1, v1.Item2 - v2.Item2);
            return vector;
        }

        public static double CalcDet(Tuple<int,int> v1, Tuple<int,int> v2)
        {
            return (v1.Item1 * v2.Item2 - v1.Item2 * v2.Item1);

        }
    
    }
}
