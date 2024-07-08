using System;

namespace MonoGame.Extended.ECS
{
    //public class ComponentType : IEquatable<ComponentType>
    //{
    //    public ComponentType(Type type, int id)
    //    {
    //        Type = type;
    //        Id = id;
    //    }

    //    public Type Type { get; }
    //    public int Id { get; }

    //    public bool Equals(ComponentType other)
    //    {
    //        if (ReferenceEquals(null, other)) return false;
    //        if (ReferenceEquals(this, other)) return true;
    //        return Id == other.Id;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        if (obj.GetType() != GetType()) return false;
    //        return Equals((ComponentType) obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return Id;
    //    }

    //    public static bool operator ==(ComponentType left, ComponentType right)
    //    {
    //        return Equals(left, right);
    //    }

    //    public static bool operator !=(ComponentType left, ComponentType right)
    //    {
    //        return !Equals(left, right);
    //    }
    //}
}
