using System;
using System.Drawing;

namespace TheDotFactory
{
    // Here is the CharInfo class providing all info we need during the char generation.
    public class CharInfo
    {
        public CharInfo(Char chr, Size charSize, Font fnt)
        {
            Character = chr;
            BitmapOriginal = new Bitmap(charSize.Width, charSize.Height);

            // Create grahpics entity for drawing and disable anti alias.
            var gfx = Graphics.FromImage(BitmapOriginal);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            // Draw black char in the middle of white background.
            var bitmapRect = new Rectangle(0, 0, BitmapOriginal.Width, BitmapOriginal.Height);
            gfx.FillRectangle(Brushes.White, bitmapRect);
            gfx.DrawString(Character.ToString(), fnt, Brushes.Black, bitmapRect,
                new StringFormat { Alignment = StringAlignment.Center });
        }

        public Char Character { get; }

        public Bitmap BitmapOriginal { get; }

        public override String ToString() => $"{Character}, {BitmapOriginal.Width}x{BitmapOriginal.Height}";
    }
}
