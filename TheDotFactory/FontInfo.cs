using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheDotFactory
{
    // We are here populating font info with the bitmaps, generating accordingly with a font, font size and a symbol.
    // We can do this once using the FontInfo class ctor
    // Input: source string, font, condition to generate space
    // Iutput: FontInfo

    public class FontInfo
    {
        public FontInfo(String srcStr, Font fnt, Boolean genSpace)
        {
            // generate char info for each character in the string using the largest bitmap size we're going to draw.
            var chrs = srcStr.ToCharArray()
                .Distinct()
                .Where(x => (genSpace || x != ' ') && x != '\n' && x != '\r')
                .OrderBy(x => x).ToArray();
            var charSizes = chrs.Select(x => TextRenderer.MeasureText(x.ToString(), fnt)).ToArray();
            var maxSize = new Size(charSizes.Max(x => x.Width), charSizes.Max(x => x.Height));
            CharInfos = chrs.Select(x => new CharInfo(x, maxSize, fnt)).ToArray();
        }

        public CharInfo[] CharInfos { get; }
    }
}
