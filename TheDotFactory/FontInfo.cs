using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheDotFactory.Config;

namespace TheDotFactory
{
    /// <summary>
    /// Represents all information we need about the font.
    /// </summary>
    public sealed class FontInfo
    {
        /// <summary>
        /// Initialize FontInfo instance based on source string and user configurations.
        /// </summary>
        /// <param name="srcStr">Source string.</param>
        /// <param name="cfg">User output configurtion.</param>
        public FontInfo(String srcStr, OutputConfig cfg)
        {
            if (srcStr == null) throw new ArgumentNullException(nameof(srcStr));
            if (cfg == null) throw new ArgumentNullException(nameof(cfg));
            if (cfg.OutputFont == null) throw new ArgumentNullException(nameof(cfg));

            // Generate char info for each character in the string using the largest bitmap size we're going to draw.
            var chrs = srcStr.ToCharArray()
                .Distinct()
                .Where(x => (cfg.SpaceGeneration || x != ' ') && x != '\n' && x != '\r')
                .OrderBy(x => x)
                .ToArray();
            var charSizes = chrs.Select(x => TextRenderer.MeasureText(x.ToString(), cfg.OutputFont)).ToArray();
            var maxSize = new Size(charSizes.Max(x => x.Width), charSizes.Max(x => x.Height));
            CharInfos = chrs.Select(x => new CharInfo(x, maxSize, cfg)).ToArray();

            // Character clipping configuration.
            var left = 0;
            var right = maxSize.Width - 1;

            var top = 0;
            var bottom = maxSize.Height - 1;
            var cropSyle = CropStyle.None;

            switch (cfg.HorizontalPaddingRemove)
            {
                case PaddingRemoval.None:
                    break;
                case PaddingRemoval.Tighest:
                    cropSyle |= CropStyle.Left | CropStyle.Right;
                    break;
                case PaddingRemoval.Fixed:
                    left = CharInfos.Min(x => x.TrimRect.Left);
                    right = CharInfos.Max(x => x.TrimRect.Right);
                    cropSyle |= CropStyle.Hfix;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (cfg.VerticalPaddingRemove)
            {
                case PaddingRemoval.None:
                    break;
                case PaddingRemoval.Tighest:
                    cropSyle |= CropStyle.Top | CropStyle.Bot;
                    break;
                case PaddingRemoval.Fixed:
                    top = CharInfos.Min(x => x.TrimRect.Top);
                    bottom = CharInfos.Max(x => x.TrimRect.Bottom);
                    cropSyle |= CropStyle.Vfih;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var cropRect = new Rectangle(left, top, right - left, bottom - top);

            foreach (var charInfo in CharInfos)
                charInfo.CropBitmap(cropRect, cropSyle);
        }

        /// <summary>
        /// Character info.
        /// </summary>
        public CharInfo[] CharInfos { get; }
    }
}
