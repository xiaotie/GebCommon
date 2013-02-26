using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Geb.Utils.WinForm
{
	public static class BitmapHelper
	{
		/// <summary>
		/// 画点
		/// </summary>
		/// <param name="map"></param>
		/// <param name="color"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void DrawPoint(this Bitmap map, Color color, int x, int y)
		{
			if (x < 0 || y < 0 || x >= map.Width || y >= map.Height) return;
			map.SetPixel(x, y, color);
		}

		/// <summary>
		/// 画十字
		/// </summary>
		/// <param name="map"></param>
		/// <param name="color"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void DrawCross(this Bitmap map, Color color, int x, int y)
		{
			map.DrawPoint(color, x, y);
			map.DrawPoint(color, x - 1, y);
			map.DrawPoint(color, x + 1, y);
			map.DrawPoint(color, x, y - 1);
			map.DrawPoint(color, x, y + 1);
		}

        public static Bitmap Clone(this Bitmap map)
        {
            return map.Clone() as Bitmap;
        }

        public static Bitmap CloneToFormat8bppIndexed(this Bitmap map)
        {
            return map.Clone(new Rectangle(0,0,map.Width, map.Height), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        }

        public static Bitmap CloneTo(this Bitmap map, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            return map.Clone(new Rectangle(0, 0, map.Width, map.Height), pixelFormat);
        }

        public static int ToGray(this Color c)
        {
            return (int) (0.299* c.R + 0.587*c.G + 0.114*c.B) ;
        }

        public static Rectangle GetRectangle(this Bitmap map)
        {
            return new Rectangle(0,0,map.Width, map.Height);
        }
	}
}
