using System;
using System.Drawing;

namespace TheDotFactory.Config
{
    public sealed class OutputConfig
    {
        public PaddingRemoval HorizontalPaddingRemove { get; set; }
        public PaddingRemoval VerticalPaddingRemove { get; set; }
        public Boolean SpaceGeneration { get; set; }
        public Font OutputFont { get; set; }
        public ByteOrder ByteOrder { get; set; }
        public BitLayout BitLayout { get; set; }
        public Rotation Rotation { get; set; }
    }
}
