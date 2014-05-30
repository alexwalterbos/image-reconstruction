using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    public interface ICloneable<T>
    {
        /// <summary>
        /// Create a copy of the object
        /// </summary>
        /// <returns>The copy</returns>
        T Clone();
    }
}
