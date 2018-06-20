using System;
using System.Collections;

namespace MonoGame.Extended.Entities
{
    public class Entity : IEquatable<Entity>
    {
        private readonly EntityManager _entityManager;
        private readonly ComponentManager _componentManager;

        internal Entity(int id, EntityManager entityManager, ComponentManager componentManager)
        {
            Id = id;

            _componentBits = new BitArray(16);
            _entityManager = entityManager;
            _componentManager = componentManager;
        }

        public int Id { get; }

        private BitArray _componentBits;
        private bool _componentsChanged;

        public BitArray ComponentBits
        {
            get
            {
                if (_componentsChanged)
                {
                    _componentManager.RefreshComponentBits(Id, ref _componentBits);
                    _componentsChanged = false;
                }

                return _componentBits;
            }
        } 

        public void Attach<T>(T component)
            where T : class 
        {
            var mapper = _componentManager.GetMapper<T>();
            mapper.Put(Id, component);
            _componentsChanged = true;
        }

        public void Detach<T>()
            where T : class
        {
            var mapper = _componentManager.GetMapper<T>();
            mapper.Delete(Id);
            _componentsChanged = true;
        }

        public T Get<T>()
            where T : class 
        {
            var mapper = _componentManager.GetMapper<T>();
            return mapper.Get(Id);
        }

        public void Destory()
        {
            _entityManager.DestroyEntity(Id);
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