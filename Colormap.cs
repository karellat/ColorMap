// Author: Tomáš Karella

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;



namespace _051colormap
{
  class Colormap
  {
      class HSVcomperer : IComparer<HSV>
      {
          public int Compare(HSV x, HSV y)
          {
              if (x.amount > y.amount) return -1;
              else if (x.amount == y.amount) return 0;
              else return 1; 
          }
      }

      class HSV
      {
          public double hue;
          public double saturation;
          public double value;
          public long amount;

          public HSV(double hue, double saturation, double value, long amount)
          {
              this.hue = hue;
              this.saturation = saturation;
              this.amount = amount;
              this.value = value;
          }

          public HSV(Color c, long amount)
          {
              double h = 0;
              double s = 0;
              double v = 0;
              MathSupport.Arith.ColorToHSV(c, out h, out s, out v);
              this.value = v;
              this.saturation = s;
              this.hue = h;
              this.amount = amount;
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
            int height = input.Height;
            int width = input.Width;
            Dictionary<Color, long> dic= new Dictionary<Color, long>();
            colors = new Color[numCol];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color c = input.GetPixel(j, i);
                    if (dic.ContainsKey(c))
                        dic[c]++;
                    else 
                        dic.Add(c,1);
                }
            }

          List<HSV> list = new List<HSV>(dic.Count);
          foreach (var item in dic)
          {
              list.Add(new HSV(item.Key,item.Value));
          }
          list.Sort(new HSVcomperer());
          int index = 0;
          while (list.Count != index)
          {
              List<HSV> workingCopy = new List<HSV>();
              var actual = list[index];
              for (int i = 0; i <= index;i++)
              {
                  workingCopy.Add(list[i]);
              }
              for (int i = index; i < list.Count; i++)
              {
                  if (Math.Abs(list[i].amount - list[index].amount) < 10000)
                    workingCopy[index].amount += list[i].amount;
                  else
                    workingCopy.Add(list[i]);
              }
              list = workingCopy;
              index++;
          }

          list.Sort(new HSVcomperer());
          List<Color> colorList = new List<Color>(list.Count);

          foreach (var hsv in list)
          {
              colorList.Add(MathSupport.Arith.HSVToColor(hsv.hue,hsv.saturation,hsv.value));
          }
          int count = colorList.Count;
          for (int i = 0; i < numCol; i++)
          {
              colors[i] = colorList[i];
          }
        }
  }
}
