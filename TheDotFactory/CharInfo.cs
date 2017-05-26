using System;
using System.Drawing;
using TheDotFactory.Config;

namespace TheDotFactory
{
    /// <summary>
    /// Describes all info and performs all actions we need during the char generation.
    /// </summary>
    public sealed class CharInfo
    {
        private readonly OutputConfig cfg;

        /// <summary>
        /// Create ChatInfo instance.
        /// </summary>
        /// <param name="chr">Charcter.</param>
        /// <param name="charSize">Character bitmap size.</param>
        /// <param name="cfg">Output configuration.</param>
        public CharInfo(Char chr, Size charSize, OutputConfig cfg)
        {
            if (charSize.IsEmpty) throw new ArgumentOutOfRangeException(nameof(charSize));
            this.cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));

            Character = chr;
            Bitmap = new Bitmap(charSize.Width, charSize.Height);

            // Create grahpics entity for drawing and disable anti alias.
            var gfx = Graphics.FromImage(Bitmap);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            var sf = new StringFormat();

            // Draw black char in the middle of white background.
            var bitmapRect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            gfx.FillRectangle(Brushes.White, bitmapRect);
            using (var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                gfx.DrawString(Character.ToString(), cfg.OutputFont, Brushes.Black, bitmapRect,
                    stringFormat);
            }
            TrimRect = Bitmap.GetTrimBounds(Color.Black, CropStyle.All);
            Pages = Bitmap.ToPageArray(cfg);
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
        /// Bitmap pages.
        /// </summary>
        public Byte[] Pages { get; private set; }


        /// <summary>
        /// Crop image accordingly with crop style.
        /// </summary>
        /// <param name="rect">Bounding rectangle.</param>
        /// <param name="stl">Crop style.</param>
        /// <remarks>
        /// We firstly use crop style to determine clipping borders,
        /// after we check if there are fixed borders, if so we use them instead.
        /// Finally we update internal class state.
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
            Pages = Bitmap.ToPageArray(cfg);
        }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override String ToString() =>
            $"{Character}, {Bitmap.Width}x{Bitmap.Height}, {TrimRect.Width}x{TrimRect.Height}";


        // get font info from string
        private void PopulateFontInfoFromCharacters(ref MainForm.FontInfo fontInfo)
        {
            // get absolute height/width of characters
            (Int32, Int32) GetAbsoluteCharacterDimensions(Image charBitmap)
            {
                Int32 width, height;
                // check if bitmap exists, otherwise set as zero
                if (charBitmap == null)
                {
                    // zero height
                    width = 0;
                    height = 0;
                }
                else
                {
                    // get the absolute font character height. Depends on rotation
                    if (cfg.Rotation == Rotation.RotateZero || cfg.Rotation == Rotation.RotateOneEighty)
                    {
                        // if char is not rotated or rotated 180deg, its height is the actual height
                        height = charBitmap.Height;
                        width = charBitmap.Width;
                    }
                    else
                    {
                        // if char is rotated by 90 or 270, its height is the width of the rotated bitmap
                        height = charBitmap.Width;
                        width = charBitmap.Height;
                    }
                }
                return (width, height);
            }

            // do nothing if no chars defined
            if (fontInfo.characters.Length == 0) return;

            // total offset
            int charByteOffset = 0;
            int dummy = 0;

            // set start char
            fontInfo.startChar = (char)0xFFFF;
            fontInfo.endChar = ' ';

            // the fixed absolute character height
            // int fixedAbsoluteCharHeight;

            var kap = GetAbsoluteCharacterDimensions(Bitmap);

           // getAbsoluteCharacterDimensions(ref fontInfo.characters[0].bitmapToGenerate, ref dummy, ref fontInfo.charHeight);

            // iterate through letter string
            for (int charIdx = 0; charIdx < fontInfo.characters.Length; ++charIdx)
            {
                // skip empty bitmaps
                if (fontInfo.characters[charIdx].bitmapToGenerate == null) continue;

                // get char
                char currentChar = fontInfo.characters[charIdx].character;

                // is this character smaller than start char?
                if (currentChar < fontInfo.startChar) fontInfo.startChar = currentChar;

                // is this character bigger than end char?
                if (currentChar > fontInfo.endChar) fontInfo.endChar = currentChar;

                // populate number of rows
                //getAbsoluteCharacterDimensions(ref fontInfo.characters[charIdx].bitmapToGenerate,
                //    ref fontInfo.characters[charIdx].width,
                //    ref fontInfo.characters[charIdx].height);

                // populate offset of character
                fontInfo.characters[charIdx].offsetInBytes = charByteOffset;

                // increment byte offset
                charByteOffset += fontInfo.characters[charIdx].pages.Count;
            }
        }
    }
}
