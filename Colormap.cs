// Author: Tomáš Karella

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;



namespace _051colormap
{
  class Colormap
  {
      struct Vector3
      {
          public float X;
          public float Y;
          public float Z;

          public static double DistanceSquared(Vector3 a, Vector3 b)
          {
              return  (Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y,2) + Math.Pow(a.Z - b.Z,2));
          }

          public static double Distance(Vector3 a, Vector3 b)
          {
              return Math.Sqrt(DistanceSquared(a,b));
          }
      }
        class KeyValueComparer : Comparer<KeyValuePair<Color,long>>
        {
            public override int Compare(KeyValuePair<Color, long> x, KeyValuePair<Color, long> y)
            {
                if (x.Value == y.Value) return 0; 
                else if (x.Value > y.Value) return -1;
                else return 1;
            }
        }
        class PairComparer:IComparer<Pair>
        {
            public int Compare(Pair x, Pair y)
            {
                if (x.value == y.value) return 0;
                else if (x.value > y.value) return -1;
                else return 1;
            }
        }

      class Pair
        {
            public Color color;
            public long value;

          public Pair(Color c, long v)
          {
              color = c;
              value = v;
          }
        }


        /// <summary>
        /// Generate a colormap based on input image.
        /// </summary>
        /// <param name="input">Input raster image.</param>
        /// <param name="numCol">Required colormap size (ignore it if you must)</param>
        /// <param name="colors">Output palette (array of colors).</param>
        /// <param name="colors">Output palette (array of colors).f</param>
        public static void Generate(Bitmap input, int numCol, out Color[] colors)
        {
         
        }
  }
}
