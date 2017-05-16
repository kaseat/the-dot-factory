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
    }
}