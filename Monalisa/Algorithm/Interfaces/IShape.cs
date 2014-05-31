//-----------------------------------------------------------------------------
// <copyright file="IShape.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    /// <summary>
    /// Just a common interface for all shape types. Currently only polygons
    /// exists, but in the future we might expand by also including circles and
    /// ellipses.
    /// </summary>
    public interface IShape : ICloneable<IShape>
    {
        // Empty interface
    }
}
