using System;

namespace MonoGame.Extended.Entities
{
    public enum AspectType
    {
        All,
        None,
        Any
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class AspectAttribute : Attribute
    {
        public AspectType Type { get; }
        public Type[] Components { get; }

        public AspectAttribute(AspectType type, params Type[] components)
        {
            Type = type;
            Components = components;
        }
    }
}
