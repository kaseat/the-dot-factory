using System;
using TheDotFactory.Config;

namespace TheDotFactory
{
    public static class TmpHelpers
    {
        public static PaddingRemoval ToPddRem(this OutputConfiguration.PaddingRemoval padd)
        {
            
            switch (padd)
            {
                case OutputConfiguration.PaddingRemoval.None:
                    return PaddingRemoval.None;
                case OutputConfiguration.PaddingRemoval.Tighest:
                    return PaddingRemoval.Tighest;
                case OutputConfiguration.PaddingRemoval.Fixed:
                    return PaddingRemoval.Fixed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(padd), padd, null);
            }
        }

        public static ByteOrder ToBord(this OutputConfiguration.ByteOrder bord)
        {
            switch (bord)
            {
                case OutputConfiguration.ByteOrder.LsbFirst:
                    return ByteOrder.LsbFirst;
                case OutputConfiguration.ByteOrder.MsbFirst:
                    return ByteOrder.MsbFirst;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bord), bord, null);
            }
        }

        public static BitLayout ToBitLayout(this OutputConfiguration.BitLayout btl)
        {
            switch (btl)
            {
                case OutputConfiguration.BitLayout.RowMajor:
                    return BitLayout.RowMajor;
                case OutputConfiguration.BitLayout.ColumnMajor:
                    return BitLayout.ColumnMajor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(btl), btl, null);
            }
        }
    }
}