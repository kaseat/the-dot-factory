using System;

namespace TheDotFactory.Config
{
    public enum Rotation : Byte
    {
        RotateZero,
        RotateNinety,
        RotateOneEighty,
        RotateTwoSeventy
    }

    public static class RotationHelpers
    {
        public static String GetString(this Rotation rtt)
        {
            switch (rtt)
            {
                case Rotation.RotateZero:
                    return "0°";
                case Rotation.RotateNinety:
                    return "90°";
                case Rotation.RotateOneEighty:
                    return "180°";
                case Rotation.RotateTwoSeventy:
                    return "270°";
                default:
                    throw new ArgumentOutOfRangeException(nameof(rtt), rtt, null);
            }
        }
    }
}