using System;
using System.Collections;

namespace MonoGame.Extended.Entities
{
    public class Aspect
    {
        internal Aspect()
        {
            AllSet = new BitArray(32);
            ExclusionSet = new BitArray(32);
            OneSet = new BitArray(32);
        }

        public BitArray AllSet { get; }
        public BitArray ExclusionSet { get; }
        public BitArray OneSet { get; }

        public static AspectBuilder All(params Type[] types)
        {
            return new AspectBuilder().All(types);
        }

        public static AspectBuilder One(params Type[] types)
        {
            return new AspectBuilder().One(types);
        }

        public static AspectBuilder Exclude(params Type[] types)
        {
            return new AspectBuilder().Exclude(types);
        }

        public bool IsInterested(BitArray componentBits)
        {
            if (!AllSet.IsEmpty() && !componentBits.ContainsAll(AllSet))
                return false;

            if (!ExclusionSet.IsEmpty() && ExclusionSet.Intersects(componentBits))
                return false;

            if (!OneSet.IsEmpty() && !OneSet.Intersects(componentBits))
                return false;

            return true;
        }

    }
}