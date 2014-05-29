using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    public static class LinqHelper
    {
        public static ulong Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> selector)
        {
            var sum = 0UL;
            foreach (var element in source)
            {
                sum += selector(element);
            }
            return sum;
        }
    }
}
