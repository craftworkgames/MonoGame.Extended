using System;
using System.Collections.Specialized;

namespace MonoGame.Extended.ECS
{
    public class Entity : IEquatable<Entity>
    {
        private readonly EntityManager _entityManager;
        private readonly ComponentManager _componentManager;

        internal Entity(int id, EntityManager entityManager, ComponentManager componentManager)
        {
            Id = id;

            _entityManager = entityManager;
            _componentManager = componentManager;
        }

        public int Id { get; }
        
        public BitVector32 ComponentBits => _entityManager.GetComponentBits(Id);

        public void Attach<T>(T component)
            where T : class 
        {
            var mapper = _componentManager.GetMapper<T>();
            mapper.Put(Id, component);
        }

        public void Detach<T>()
            where T : class
        {
            var mapper = _componentManager.GetMapper<T>();
            mapper.Delete(Id);
        }

        public T Get<T>()
            where T : class 
        {
            var mapper = _componentManager.GetMapper<T>();
            return mapper.Get(Id);
        }


        public bool Has<T>() 
            where T : class
        {
            return _componentManager.GetMapper<T>().Has(Id);
        }

        public void Destroy()
        {
            _entityManager.Destroy(Id);
        }

        public bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id;
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }
    }
}
