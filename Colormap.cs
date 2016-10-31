// Author: Tomáš Karella

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


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

      private static List<Pair>[] outputList = new List<Pair>[10];

        /// <summary>
        /// Generate a colormap based on input image.
        /// </summary>
        /// <param name="input">Input raster image.</param>
        /// <param name="numCol">Required colormap size (ignore it if you must)</param>
        /// <param name="colors">Output palette (array of colors).</param>
        /// <param name="colors">Output palette (array of colors).f</param>
        public static void Generate(Bitmap input, int numCol, out Color[] colors)
        {
            int width = input.Width;
            int height = input.Height;
            colors = new Color[numCol];
            Dictionary<Color, long> dic = new Dictionary<Color, long>();
            //hard loop every color,
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    
                    var c = input.GetPixel(j, i);
                    
                    var cl = Color.FromArgb((c.R/16)*16, (c.G/16)*16, (c.B/16)*16);
                    
                    if (dic.ContainsKey(cl))
                        checked {dic[cl]++;}
                    else dic.Add(cl, 1);
                }
            }
            var l = dic.ToList();
            l.Sort(new KeyValueComparer());
            var myList = new List<Pair>(l.Count);
            foreach (var item in l)
            {
                myList.Add(new Pair(item.Key,item.Value));
            }
            Task[] tasks = new Task[10];
            int constant = 10000;
            for (int i = 0; i < 10; i++)
            {
                var x = i;

                var c = i == 9 ? 0 : constant;
                var copy = myList.ToList();
                tasks[i] = new Task(()=>MakeCompose(c,copy,x));
                tasks[i].Start();
                constant = constant/2; 
            }
            Task.WaitAll(tasks);
            List<Pair> best = null;
            for (int i = 9; i >= 0; i--)
            {
                
                if (outputList[i].Count < numCol)
                {
                    
                    if (i == 9) best = outputList[9];
                    else best = outputList[i + 1];
                    break; 
                }
                if (i == 0)
                    best = outputList[0];
            }

            int correctAmount = (numCol > best.Count) ? best.Count : numCol;
            colors = new Color[correctAmount];
            for (int i = 0; i < correctAmount; i++)
            {
                colors[i] = best[i].color; 
            }

        }

      private static void MakeCompose(int constant, List<Pair> myList,int output)
      {
            int index = 0;
            while (index < myList.Count)
            {
                var selectItem = myList[index];
                byte cleanR = selectItem.color.R;
                byte cleanG = selectItem.color.G;
                byte cleanB = selectItem.color.B;
                byte white = Math.Min(cleanR, Math.Min(cleanG, cleanB));
                cleanR -= white;
                cleanB -= white;
                cleanG -= white;

                Vector3 v = new Vector3() { X = cleanR, Y = cleanG, Z = cleanB };
                var nList = new List<Pair>();

                for (int i = 0; i <= index; i++)
                {
                    nList.Add(myList[i]);
                }

                for (int i = index + 1; i < myList.Count; i++)
                {
                    var cR = myList[i].color.R;
                    var cG = myList[i].color.G;
                    var cB = myList[i].color.B;
                    var cW = Math.Min(cR, Math.Min(cG, cB));
                    cR -= cW;
                    cG -= cW;
                    cB -= cW;
                    Vector3 c = new Vector3() { X = cR, Y = cG, Z = cB };
                    if (Vector3.DistanceSquared(v, c) < constant)
                    {
                        checked
                        {
                            nList[index].value += myList[i].value;
                        }
                    }
                    else nList.Add(new Pair(myList[i].color, myList[i].value));
                }

                index++;
                myList = nList;

            }
            myList.Sort(new PairComparer());
          outputList[output] = myList;
      }
  }
}
