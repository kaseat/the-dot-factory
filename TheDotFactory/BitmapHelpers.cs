using System.Drawing;

namespace TheDotFactory
{
    public static class BitmapHelpers
    {
        /// <summary>
        /// Trim bitmap based on <para>clr</para> color. 
        /// </summary>
        /// <param name="bmp">Source bitmap.</param>
        /// <param name="clr">Color to be left after trimming.</param>
        /// <param name="stl">Crop style.</param>
        /// <returns>Returns trimmed bitmap</returns>
        /// <remarks>
        /// Worst case: O(w*h–a*b)
        ///                                __
        /// ---------------------
        /// ---------------------   __
        /// ---xxxx----xxxx------
        /// ---xxxx----xxxx------          h
        /// ----xxx----xxx-------   b
        /// ----xxxxxxxxxx------- 
        /// ------xxxxxx---------   __
        /// ---------------------          __
        ///    |     a     |
        /// 
        /// |          w         |
        /// </remarks>
        public static Rectangle GetTrimBounds(this Bitmap bmp, Color clr, CropStyle stl)
        {
            var width = bmp.Width;
            var height = bmp.Height;
            var left = 0;
            var top = 0;
            var right = width - 1;
            var bottom = height - 1;
            var minRight = width - 1;
            var minBottom = height - 1;

            for (; top < bottom; top++)
            for (var x = 0; x < width; x++)
            {
                if (bmp.GetPixel(x, top).ToArgb() != clr.ToArgb()) continue;
                minRight = x;
                minBottom = top;
                goto lft;
            }

            lft:
            for (; left < minRight; left++)
            for (var y = bottom; y > top; y--)
            {
                if (bmp.GetPixel(left, y).ToArgb() != clr.ToArgb()) continue;
                minBottom = y;
                goto bot;
            }

            bot:
            for (; bottom > minBottom; bottom--)
            {
                for (var x = right; x >= left; x--)
                {
                    if (bmp.GetPixel(x, bottom).ToArgb() != clr.ToArgb()) continue;
                    minRight = x;
                    goto rgt;
                }
            }

            rgt:
            for (; right > minRight; right--)
            for (var y = bottom; y >= top; y--)
                if (bmp.GetPixel(right, y).ToArgb() == clr.ToArgb())
                    goto final;

            final:
            top = (stl & CropStyle.Top) == CropStyle.Top ? top : 0;
            bottom = (stl & CropStyle.Bot) == CropStyle.Bot ? bottom : bmp.Height - 1;
            left = (stl & CropStyle.Left) == CropStyle.Left ? left : 0;
            right = (stl & CropStyle.Right) == CropStyle.Right ? right : bmp.Width - 1;

            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }
    }
}
