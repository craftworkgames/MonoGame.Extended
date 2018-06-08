
using System;

namespace MonoGame.Extended.Entities
{
    public class ComponentType : IEquatable<ComponentType>
    {
        public ComponentType(Type type, int index)
        {
            Type = type;
            Index = index;
        }

        public Type Type { get; }
        public int Index { get; }

        public bool Equals(ComponentType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ComponentType) obj);
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public static bool operator ==(ComponentType left, ComponentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComponentType left, ComponentType right)
        {
            return !Equals(left, right);
        }
    }
}
