using System;
using System.Collections;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public class Aspect
    {
        private Aspect()
        {
            AllSet = new BitArray(32);
            ExclusionSet = new BitArray(32);
            OneSet = new BitArray(32);
        }

        public BitArray AllSet { get; }
        public BitArray ExclusionSet { get; }
        public BitArray OneSet { get; }

        public static Builder All(params Type[] types)
        {
            return new Builder().All(types);
        }

        public static Builder One(params Type[] types)
        {
            return new Builder().One(types);
        }

        public static Builder Exclude(params Type[] types)
        {
            return new Builder().Exclude(types);
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

        public class Builder
        {
            public Builder()
            {
                AllTypes = new Bag<Type>();
                ExclusionTypes = new Bag<Type>();
                OneTypes = new Bag<Type>();
            }

            public Bag<Type> AllTypes { get; }
            public Bag<Type> ExclusionTypes { get; }
            public Bag<Type> OneTypes { get; }

            public Builder All(params Type[] types)
            {
                foreach (var type in types)
                    AllTypes.Add(type);

                return this;
            }

            public Builder One(params Type[] types)
            {
                foreach (var type in types)
                    OneTypes.Add(type);

                return this;
            }

            public Builder Exclude(params Type[] types)
            {
                foreach (var type in types)
                    ExclusionTypes.Add(type);

                return this;
            }

            public Aspect Build(ComponentManager componentManager)
            {
                var aspect = new Aspect();
                Associate(componentManager, AllTypes, aspect.AllSet);
                Associate(componentManager, OneTypes, aspect.OneSet);
                Associate(componentManager, ExclusionTypes, aspect.ExclusionSet);
                return aspect;
            }

            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            private static void Associate(ComponentManager componentManager, Bag<Type> types, BitArray bitArray)
            {
                foreach (var type in types)
                {
                    var id = componentManager.GetComponentTypeId(type);
                    bitArray.Set(id, true);
                }
            }
        }
    }


}