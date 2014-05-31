using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Helper class for conversion from and to bits and bytes
    /// </summary>
    public static class BitHelper
    {
        /// <summary>
        /// Converts a bool (bit) array to string ("00000001 00001111").
        /// Note that this method puts a space between each 8 bits.
        /// </summary>
        /// <example>
        /// var bits = new bool[] { true, true, true, true, true, true, false, false };
        /// bits.ToBitString();
        /// // "11111100"
        /// </example>
        /// <param name="bits">The bits to convert</param>
        /// <returns>A string in the form "0011"</returns>
        public static string ToBitString(this bool[] bits)
        {
            var str = new StringBuilder();
            for (int i = 0; i < bits.Length; i++)
            {
                if (i != 0 && i % 8 == 0) str.Append(" ");
                str.Append(bits[i] ? 1 : 0);
            }
            return str.ToString();
        }

        /// <summary>
        /// Converts a byte array to a hexdecimal string ("DA FE D0").
        /// Note that this puts a space between each byte.
        /// </summary>
        /// <example>
        /// var bytes = new byte[] { 0xDA, 0xFE, 0xD0 };
        /// bytes.ToByteString();
        /// // "DA FE D0"
        /// </example>
        /// <param name="bytes">The bytes to convert</param>
        /// <returns>A string in the form "FF 0A" </returns>
        public static string ToByteString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        /// <summary>
        /// Converts a byte array to a bit array.
        /// </summary>
        /// <example>
        /// var bytes = new byte[] { 0xff };
        /// var bits = bytes.AsBitArray();
        /// // bits = [ true, true, true, true, true, true, true, true ]
        /// </example>
        /// <param name="bytes">Bytes to convert</param>
        /// <returns>Boolean array representing the bits</returns>
        public static bool[] AsBitArray(this byte[] bytes)
        {
            return bytes.SelectMany(GetBits).ToArray();
        }

        /// <summary>
        /// Converts a bit array to a byte array.
        /// Assumes a multitude of 8 bits to convert.
        /// </summary>
        /// <example>
        /// var bits = new bit[] { true, true, true, true, true, true, false, false };
        /// var bytes = bits.AsByteArray();
        /// // bytes = { 0xfc }
        /// </example>
        /// <param name="bits">Bits to convert</param>
        /// <returns>Byte array</returns>
        public static byte[] AsByteArray(this bool[] bits)
        {
            return GetByte(bits).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="ICanvas"/> to a byte array. 
        /// The array is build up of [ (4 + 8 x PolygonEdgeCount ) x PolygonCount ] bytes
        /// </summary>
        /// <example>
        /// given canvas of two triangles (total 56 bytes):
        /// byte    |   Description
        ///  0- 3       RGBA triangle 1 (4x byte)
        ///  4-11       Coordinate 1 triangle 1 (tuple of 2x int)
        /// 12-19       Coordinate 2 triangle 1 (tuple of 2x int)
        /// 19-27       Coordinate 3 triangle 1 (tuple of 2x int)
        /// 28-31       RGBA triangle 2 (4x bytes)
        /// 32-39       Coordinate 1 triangle 2 (tuple of 2x int)
        /// 40-47       Coordinate 2 triangle 2 (tuple of 2x int)
        /// 48-55       Coordinate 3 triangle 2 (tuple of 2x int)
        /// </example>
        /// <param name="canvas">The canvas to convert</param>
        /// <returns>Byte array representing the canvas</returns>
        public static byte[] AsByteArray(this ICanvas canvas)
        {
            return canvas.Elements.SelectMany(shape => shape.AsByteArray()).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="IShape"/> to byte array.
        /// Can only convert Polygon shapes (throws NotSupportedException otherwise).
        /// The array is build up of ( 4 + 8 x Coordinates ) bytes.
        /// </summary>
        /// <example>
        /// Given a triangle (total 28 bytes)
        /// byte    |   Description
        ///  0- 3       RGBA triangle 1 (4x byte)
        ///  4-11       Coordinate 1 triangle 1 (tuple of 2x int)
        /// 12-19       Coordinate 2 triangle 1 (tuple of 2x int)
        /// 19-27       Coordinate 3 triangle 1 (tuple of 2x int)
        /// </example>
        /// <param name="shape">The shape to convert to byte array</param>
        /// <returns>Byte array representing the shape</returns>
        public static byte[] AsByteArray(this IShape shape)
        {
            var polygon = (IPolygon)shape;
            if (polygon == null) throw new NotSupportedException("Only polygons are supported at this time");

            var bytes = new List<byte>();
            bytes.Add(polygon.Red);
            bytes.Add(polygon.Green);
            bytes.Add(polygon.Blue);
            bytes.Add(polygon.Alpha);
            bytes.AddRange(polygon.Coordinates.SelectMany(AsByteArray));
            return bytes.ToArray();
        }

        /// <summary>
        /// Converts a <see cref="Tuple"/>{int, int} to a byte array.
        /// This array is build up of 8 bytes (two integers).
        /// note that int is signed, the MSB is the sign.
        /// </summary>
        /// <example>
        /// var tuple = new Tuple{int, int} { int.max, int.min };
        /// tuple.AsByteArray();
        /// // [ 0xFF 0x7F ] = [ 1111111 0111111 ]
        /// </example>
        /// <param name="point">The point to convert to a byte array</param>
        /// <returns>Byte array representing the point.</returns>
        public static byte[] AsByteArray(this Tuple<int, int> point)
        {
            var bytes = new List<byte>();
            // bitconverter is big endian (x86 is little endian) => reverse
            bytes.AddRange(BitConverter.GetBytes(point.Item1).Reverse());
            bytes.AddRange(BitConverter.GetBytes(point.Item2).Reverse());
            return bytes.ToArray();
        }

        /// <summary>
        /// Converts a <see cref="Tuple"/>{int, int} to a bit array.
        /// This array is build up of 64 bits (two integers).
        /// note that int is signed, the MSB is the sign.
        /// </summary>
        /// <example>
        /// var tuple = new Tuple{int, int} { int.max, int.min };
        /// tuple.AsBitArray();
        /// // [ 1111111 0111111 ]
        /// </example>
        /// <param name="point">The point to convert to a bit array</param>
        /// <returns>Bit array representing the point.</returns>
        public static bool[] AsBitArray(this Tuple<int, int> point)
        {
            return point.AsByteArray().AsBitArray();
        }


        /// <summary>
        /// Converts a <see cref="IShape"/> to bit array.
        /// Can only convert Polygon shapes (throws NotSupportedException otherwise)
        /// The array is build up of ( 32 + 64 x Coordinates ) bits.
        /// </summary>
        /// <example>
        /// Given a triangle (total 224 bits)
        /// bit     |   Description
        ///   0- 31     RGBA triangle 1 (4x byte)
        ///  32- 95     Coordinate 1 triangle 1 (tuple of 2x int)
        ///  96-159     Coordinate 2 triangle 1 (tuple of 2x int)
        /// 160-223     Coordinate 3 triangle 1 (tuple of 2x int)
        /// </example>
        /// <param name="shape">The shape to convert to bit array</param>
        /// <returns>Bit array representing the shape</returns>
        public static bool[] AsBitArray(this IShape shape)
        {
            return shape.AsByteArray().AsBitArray();
        }

        /// <summary>
        /// Converts a <see cref="ICanvas"/> to a byte array. 
        /// The array is build up of [ (32 + 64 x PolygonEdgeCount ) x PolygonCount ] bits.
        /// </summary>
        /// <example>
        /// given canvas of two triangles (total 56 bytes):
        /// bit     |   Description
        ///   0- 31     RGBA triangle 1 (4x byte)
        ///  32- 95     Coordinate 1 triangle 1 (tuple of 2x int)
        ///  96-159     Coordinate 2 triangle 1 (tuple of 2x int)
        /// 160-223     Coordinate 3 triangle 1 (tuple of 2x int)
        /// 224-255     RGBA triangle 2 (4x bytes)
        /// 256-319     Coordinate 1 triangle 2 (tuple of 2x int)
        /// 320-383     Coordinate 2 triangle 2 (tuple of 2x int)
        /// 384-447     Coordinate 3 triangle 2 (tuple of 2x int)
        /// </example>
        /// <param name="canvas">The canvas to convert</param>
        /// <returns>Byte array representing the canvas</returns>
        public static bool[] AsBitArray(this ICanvas canvas)
        {
            return canvas.AsByteArray().AsBitArray();
        }

        /// <summary>
        /// Converts a little endian byte array of 8 bytes to <see cref="Tuple"/>{int, int}.
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <returns>A tuple of two integers</returns>
        public static Tuple<int, int> AsTuple(this byte[] bytes)
        {
            int int1 = BitConverter.ToInt32(bytes.Reverse().ToArray(), 4);
            int int2 = BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
            return new Tuple<int, int>(int1, int2);
        }

        /// <summary>
        /// Converts a little endian byte array of ( 4 + 8 x N ) bytes to a
        /// <see cref="IPolygon"/> with N coordinates
        /// </summary>
        /// <param name="bytes">Bytes to convert</param>
        /// <returns>A polygon with N coordinates</returns>
        public static IPolygon AsPolygon(this byte[] bytes)
        {
            if (bytes.Length % 8 != 4) throw new ArgumentException("invalid number of bytes for a polygon");
            var polygon = new Polygon();
            polygon.Red = bytes[0];
            polygon.Green = bytes[1];
            polygon.Blue = bytes[2];
            polygon.Alpha = bytes[3];
            for (int i = 4; i < bytes.Length; i += 8)
                polygon.Coordinates.Add(bytes.Skip(i).Take(8).ToArray().AsTuple());
            return polygon;
        }

        /// <summary>
        /// Converts a little endian byte array of [ ( 4 + 8 x PolygonEdgeCount ) * PolygonCount ] bytes
        /// to a <see cref="ICanvas"/> object using settings in the given <seealso cref="EvolutionaryAlgorithm"/>.
        /// </summary>
        /// <param name="bytes">Bytes to convert</param>
        /// <param name="EA">Algorithm to get PolygonCount and PolygonEdgeCount from</param>
        /// <returns>A canvas for the given EvolutionaryAlgorithm</returns>
        public static ICanvas AsCanvas(this byte[] bytes, EvolutionaryAlgorithm EA)
        {
            // per polygon: 4B+8B*coords
            // per canvas: (4B+8B*C)*N where N=polygoncount            
            var expectedPerPolygon = 4 + 8 * EA.PolygonEdgeCount;
            var expectedPerCanvas = expectedPerPolygon * EA.PolygonCount;
            if (bytes.Length != expectedPerCanvas) throw new ArgumentException(string.Format("expected {0} bytes, got {1}.", expectedPerCanvas, bytes.Length));

            var canvas = new Canvas(EA);
            canvas.Elements = new List<IShape>();
            for (int i = 0; i < bytes.Length; i += expectedPerPolygon)
                canvas.Elements.Add(bytes.Skip(i).Take(expectedPerPolygon).ToArray().AsPolygon());

            return canvas;
        }


        /// <summary>
        /// Converts a little endian bit array of 64 bits to <see cref="Tuple"/>{int, int}.
        /// </summary>
        /// <param name="bits">bit array to convert</param>
        /// <returns>A tuple of two integers</returns>
        public static Tuple<int, int> AsTuple(this bool[] bits)
        {
            return bits.AsByteArray().AsTuple();
        }


        /// <summary>
        /// Converts a little endian byte array of ( 32 + 64 x N ) bits to a
        /// <see cref="IPolygon"/> with N coordinates
        /// </summary>
        /// <param name="bits">Bits to convert</param>
        /// <returns>A polygon with N coordinates</returns>
        public static IPolygon AsPolygon(this bool[] bits)
        {
            return bits.AsByteArray().AsPolygon();
        }

        /// <summary>
        /// Converts a little endian byte array of [ ( 32 + 64 x PolygonEdgeCount ) * PolygonCount ] bits
        /// to a <see cref="ICanvas"/> object using settings in the given <seealso cref="EvolutionaryAlgorithm"/>.
        /// </summary>
        /// <param name="bits">Bits to convert</param>
        /// <param name="EA">Algorithm to get PolygonCount and PolygonEdgeCount from</param>
        /// <returns>A canvas for the given EvolutionaryAlgorithm</returns>
        public static ICanvas AsCanvas(this bool[] bits, EvolutionaryAlgorithm EA)
        {
            return bits.AsByteArray().AsCanvas(EA);
        }


        // enumerate bits from a byte
        private static IEnumerable<bool> GetBits(byte B)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (B & 0x80) != 0;
                B *= 2;
            }
        }

        // enumerate bytes from bit array (assumes b.length%8==0)
        private static IEnumerable<byte> GetByte(bool[] b)
        {
            if (b.Length % 8 != 0) throw new ArgumentException("only multitudes of 8 bits can be converted atm.");
            byte B = 0x00;
            for (int i = 0; i < b.Length; i++)
            {
                if (i % 8 == 0) B = 0x00;
                B += (byte)(b[i] ? 0x01 << (7 - i % 8) : 0x00);
                if (i % 8 == 7) yield return B;
            }
        }
    }
}
