using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TheDotFactory
{
    // Here is the CharInfo class providing all info we need during the char generation.
    public class CharInfo
    {
        /// <summary>
        /// Create ChatInfo instance.
        /// </summary>
        /// <param name="chr">Charcter.</param>
        /// <param name="charSize">Character bitmap size.</param>
        /// <param name="fnt">Character font.</param>
        public CharInfo(Char chr, Size charSize, Font fnt)
        {
            Character = chr;
            Bitmap = new Bitmap(charSize.Width, charSize.Height);

            // Create grahpics entity for drawing and disable anti alias.
            var gfx = Graphics.FromImage(Bitmap);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            var sf =new StringFormat();

            // Draw black char in the middle of white background.
            var bitmapRect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            gfx.FillRectangle(Brushes.White, bitmapRect);
            gfx.DrawString(Character.ToString(), fnt, Brushes.Black, bitmapRect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center});
            TrimRect = Bitmap.GetTrimBounds(Color.Black,CropStyle.All);
        }

        /// <summary>
        /// Character value.
        /// </summary>
        public Char Character { get; }

        /// <summary>
        /// Character bitmap.
        /// </summary>
        public Bitmap Bitmap { get; private set; }

        /// <summary>
        /// Char bounding rect.
        /// </summary>
        public Rectangle TrimRect { get; private set; }

        /// <summary>
        /// Crop image accordinatly with crop style.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="stl"></param>
        /// <remarks>
        /// We firstly use crop style to determine clipping borders,
        /// after we check if there are fixed borders, if so we use them istead.
        /// </remarks>
        public void CropBitmap(Rectangle rect, CropStyle stl)
        {
            if (rect.X + rect.Width > Bitmap.Width) throw new ArgumentOutOfRangeException(nameof(rect));
            if (rect.Y + rect.Height > Bitmap.Height) throw new ArgumentOutOfRangeException(nameof(rect));


            var top = (stl & CropStyle.Top) == CropStyle.Top ? TrimRect.Top : 0;
            var bottom = (stl & CropStyle.Bot) == CropStyle.Bot ? TrimRect.Bottom : Bitmap.Height - 1;
            var left = (stl & CropStyle.Left) == CropStyle.Left ? TrimRect.Left : 0;
            var right = (stl & CropStyle.Right) == CropStyle.Right ? TrimRect.Right : Bitmap.Width - 1;

            if ((stl & CropStyle.Vfih) == CropStyle.Vfih)
            {
                top = rect.Top;
                bottom = rect.Bottom;
            }

            if ((stl & CropStyle.Hfix) == CropStyle.Hfix)
            {
                left = rect.Left;
                right = rect.Right;
            }
            
            var croppedBitmap = Bitmap.Clone(new Rectangle(left, top, right - left, bottom - top), Bitmap.PixelFormat);

            TrimRect = croppedBitmap.GetTrimBounds(Color.Black, CropStyle.All);
            Bitmap.Dispose();
            Bitmap = croppedBitmap;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override String ToString() => $"{Character}, {Bitmap.Width}x{Bitmap.Height}";
    }
}
