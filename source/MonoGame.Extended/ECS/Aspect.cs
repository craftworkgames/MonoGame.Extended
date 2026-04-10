using System;

namespace MonoGame.Extended.ECS
{
    public class Aspect
    {
        internal Aspect()
        {
            AllSet = new ComponentBits();
            ExclusionSet = new ComponentBits();
            OneSet = new ComponentBits();
        }

        public ComponentBits AllSet;
        public ComponentBits ExclusionSet;
        public ComponentBits OneSet;

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

        public bool IsInterested(ComponentBits componentBits)
        {
            if (!AllSet.IsEmpty && !componentBits.HasAll(AllSet))
            {
                return false;
            }

            if (!ExclusionSet.IsEmpty && componentBits.HasAny(ExclusionSet))
            {
                return false;
            }

            if (!OneSet.IsEmpty && !componentBits.HasAny(OneSet))
            {
                return false;
            }

            return true;
        }
    }
}
