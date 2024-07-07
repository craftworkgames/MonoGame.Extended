using System;
using System.Collections.Specialized;

namespace MonoGame.Extended.ECS
{
    public class Aspect
    {
        internal Aspect()
        {
            AllSet = new BitVector32();
            ExclusionSet = new BitVector32();
            OneSet = new BitVector32();
        }

        public BitVector32 AllSet;
        public BitVector32 ExclusionSet;
        public BitVector32 OneSet;

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

        public bool IsInterested(BitVector32 componentBits)
        {
            if (AllSet.Data != 0 && (componentBits.Data & AllSet.Data) != AllSet.Data)
                return false;

            if (ExclusionSet.Data != 0 && (componentBits.Data & ExclusionSet.Data) != 0)
                return false;

            if (OneSet.Data != 0 && (componentBits.Data & OneSet.Data) == 0)
                return false;

            return true;
        }
    }
}