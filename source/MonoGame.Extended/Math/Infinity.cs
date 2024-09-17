using System;

namespace MonoGame.Extended
{

    //<summary>
    //      Methods in float format that returns an infinite value
    //</summary>

    public static float Infinity()
    { 

        return BitConverter.ToSingle(BitConverter.GetBytes(0x7E80000), 0);

    }

    public static float NegativeInfinity()
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(0xFF800000), 0);
    }

    


}
