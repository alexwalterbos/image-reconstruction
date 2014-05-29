using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    public static class BitHelper
    {
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

        public static string ToByteString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        public static bool[] AsBitArray(this byte[] bytes)
        {
            return bytes.SelectMany(GetBits).ToArray();
        }

        public static byte[] AsByteArray(this bool[] bits)
        {
            return GetByte(bits).ToArray();
        }

        public static byte[] AsByteArray(this ICanvas canvas)
        {
            return canvas.Elements.SelectMany(shape => shape.AsByteArray()).ToArray();
        }

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

        public static byte[] AsByteArray(this Tuple<int, int> point)
        {
            var bytes = new List<byte>();
            // bitconverter is big endian (x86 is little endian)
            bytes.AddRange(BitConverter.GetBytes(point.Item1).Reverse());
            bytes.AddRange(BitConverter.GetBytes(point.Item2).Reverse());
            return bytes.ToArray();
        }

        public static bool[] AsBitArray(this Tuple<int, int> point)
        {
            return point.AsByteArray().AsBitArray();
        }

        public static bool[] AsBitArray(this IShape shape)
        {
            return shape.AsByteArray().AsBitArray();
        }

        public static bool[] AsBitArray(this ICanvas canvas)
        {
            return canvas.AsByteArray().AsBitArray();
        }

        public static Tuple<int, int> AsTuple(this byte[] bytes)
        {
            int int1 = BitConverter.ToInt32(bytes.Reverse().ToArray(), 4);
            int int2 = BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
            return new Tuple<int, int>(int1, int2);
        }

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

        public static Tuple<int, int> AsTuple(this bool[] bits)
        {
            return bits.AsByteArray().AsTuple();
        }

        public static IPolygon AsPolygon(this bool[] bits)
        {
            return bits.AsByteArray().AsPolygon();
        }

        public static ICanvas AsCanvas(this bool[] bits, EvolutionaryAlgorithm EA)
        {
            return bits.AsByteArray().AsCanvas(EA);
        }

        static IEnumerable<bool> GetBits(byte B)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (B & 0x80) != 0;
                B *= 2;
            }
        }

        static IEnumerable<byte> GetByte(bool[] b)
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
