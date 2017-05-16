using System;

namespace TheDotFactory
{
    [Flags]
    public enum CropStyle : Byte
    {
        None = 0x00,
        Top = 0x01,
        Bot = 0x02,
        Left = 0x04,
        Right = 0x08,
        All = 0x0F,
        Vfih = 0x10,
        Hfix = 0x20
    }
}
