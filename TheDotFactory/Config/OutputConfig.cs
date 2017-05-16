using System;
using System.Drawing;

namespace TheDotFactory.Config
{
    public class OutputConfig
    {
        public PaddingRemoval HorizontalPaddingRemove { get; set; }
        public PaddingRemoval VerticalPaddingRemove { get; set; }
        public Boolean SpaceGeneration { get; set; }
        public Font OutputFont { get; set; }
    }
}
