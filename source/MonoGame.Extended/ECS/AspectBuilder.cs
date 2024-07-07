using System;
using System.Collections.Specialized;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.ECS
{
    public class AspectBuilder
    {
        public AspectBuilder()
        {
            AllTypes = new Bag<Type>();
            ExclusionTypes = new Bag<Type>();
            OneTypes = new Bag<Type>();
        }

        public Bag<Type> AllTypes { get; }
        public Bag<Type> ExclusionTypes { get; }
        public Bag<Type> OneTypes { get; }

        public AspectBuilder All(params Type[] types)
        {
            foreach (var type in types)
                AllTypes.Add(type);

            return this;
        }

        public AspectBuilder One(params Type[] types)
        {
            foreach (var type in types)
                OneTypes.Add(type);

            return this;
        }

        public AspectBuilder Exclude(params Type[] types)
        {
            foreach (var type in types)
                ExclusionTypes.Add(type);

            return this;
        }

        public Aspect Build(ComponentManager componentManager)
        {
            var aspect = new Aspect();
            Associate(componentManager, AllTypes, ref aspect.AllSet);
            Associate(componentManager, OneTypes, ref aspect.OneSet);
            Associate(componentManager, ExclusionTypes, ref aspect.ExclusionSet);
            return aspect;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static void Associate(ComponentManager componentManager, Bag<Type> types, ref BitVector32 bits)
        {
            foreach (var type in types)
            {
                var id = componentManager.GetComponentTypeId(type);
                bits[1 << id] = true;
            }
        }
    }
}