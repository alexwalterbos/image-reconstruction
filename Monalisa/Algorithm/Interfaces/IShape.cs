namespace org.monalisa.algorithm
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
