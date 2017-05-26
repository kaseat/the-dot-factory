using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheDotFactory.Config;

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
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            if (!Enum.IsDefined(typeof(CropStyle), stl))
                throw new InvalidEnumArgumentException(nameof(stl), (Int32) stl, typeof(CropStyle));

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

        /// <summary>
        /// Create page array from the bitmap.
        /// </summary>
        /// <param name="bmp">Source bitmap.</param>
        /// <param name="cfg">Output configuration.</param>
        /// <returns>Returns page array depending on output configuration.</returns>
        public static Byte[] ToPageArray(this Bitmap bmp, OutputConfig cfg)
        {
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            if (cfg == null) throw new ArgumentNullException(nameof(cfg));

            var pages = new List<Byte>();

            // for each row
            for (var row = 0; row < bmp.Height; row++)
            {
                // current byte value
                Byte currentValue = 0, bitsRead = 0;

                // for each column
                for (var column = 0; column < bmp.Width; ++column)
                {
                    // is pixel set?
                    if (bmp.GetPixel(column, row).ToArgb() == Color.Black.ToArgb())
                    {
                        // set the appropriate bit in the page
                        if (cfg.ByteOrder == ByteOrder.MsbFirst) currentValue |= (Byte) (1 << (7 - bitsRead));
                        else currentValue |= (Byte) (1 << bitsRead);
                    }

                    // increment number of bits read
                    ++bitsRead;

                    // have we filled a page?
                    if (bitsRead == 8)
                    {
                        // add byte to page array
                        pages.Add(currentValue);

                        // zero out current value
                        currentValue = 0;

                        // zero out bits read
                        bitsRead = 0;
                    }
                }

                // if we have bits left, add it as is
                if (bitsRead != 0) pages.Add(currentValue);
            }

            // generate an array of column major pages from row major pages
            Byte[] TransposePageArray(IReadOnlyList<Byte> rowMajorPages, Int32 w, Int32 h, ByteOrder bOrd)
            {
                // column major data has a byte for each column representing 8 rows
                var rowMajorPagesPerRow = (w + 7) / 8;
                var colMajorPagesPerRow = w;
                var colMajorRowCount = (h + 7) / 8;

                // create an array of pages filled with zeros for the column major data
                var colMajorPages = new Byte[colMajorPagesPerRow * colMajorRowCount];

                // generate the column major data
                for (var row = 0; row != h; ++row)
                {
                    for (var col = 0; col != w; ++col)
                    {
                        // get the byte containing the bit we want
                        var srcIdx = row * rowMajorPagesPerRow + col / 8;
                        var page = rowMajorPages[srcIdx];

                        // return a bitMask to pick out the 'bitIndex'th bit allowing for byteOrder
                        // MsbFirst: bitIndex = 0 = 0x01, bitIndex = 7 = 0x80
                        // LsbFirst: bitIndex = 0 = 0x80, bitIndex = 7 = 0x01
                        Int32 GetBitMask(Int32 bitIndex, ByteOrder byteOrder) => byteOrder == ByteOrder.MsbFirst
                            ? 0x01 << bitIndex
                            : 0x80 >> bitIndex;

                        // get the bit mask for the bit we want
                        var bitMask = GetBitMask(7 - col % 8, bOrd);

                        // set the bit in the column major data
                        if ((page & bitMask) == 0) continue;
                        var dstIdx = row / 8 * colMajorPagesPerRow + col;
                        var p = colMajorPages[dstIdx];
                        colMajorPages[dstIdx] = (Byte) (p | GetBitMask(row % 8, bOrd));
                    }
                }
                return colMajorPages;
            }

            // transpose the pages if column major data is requested
            return cfg.BitLayout == BitLayout.ColumnMajor
                ? TransposePageArray(pages, bmp.Width, bmp.Height, cfg.ByteOrder)
                : pages.ToArray();
        }
    }
}
