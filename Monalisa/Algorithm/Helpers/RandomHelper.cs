using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    public static class RandomHelper
    {
        public static ulong NextULong(this Random r, ulong max = ulong.MaxValue)
        {
            var buffer = new byte[8];
            r.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
