using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Our algorithms canvas. This is not the gui canvas but the one of which
    /// our evolutionary algorihm population consists.
    /// </summary>
    public interface ICanvas
    {
        // The elements on this canvas
        ICollection<IShape> Elements { get; }
    }
}
