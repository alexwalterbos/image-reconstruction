//-----------------------------------------------------------------------------
// <copyright file="ICloneable.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    /// <summary>
    /// Defines an object that can be copied by using Object.Clone().
    /// The clone should not have any reference to the original
    /// </summary>
    /// <typeparam name="T">Type of class to clone</typeparam>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Create a copy of the object. This object should not have any 
        /// reference to the original and should be save to alter.
        /// </summary>
        /// <returns>The copy</returns>
        T Clone();
    }
}
