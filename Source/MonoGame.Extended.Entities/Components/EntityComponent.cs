using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Components
{
    public abstract class EntityComponent : IMovable, IRotatable, IScalable 
    {
        private Vector2 _position;
        private float _rotation;
        private Vector2 _scale;

        protected EntityComponent()
        {
        }

        public Entity Entity { get; internal set; }

        public virtual Vector2 Position
        {
            get { return Entity?.Position ?? _position; }
            set
            {
                _position = value;

                if (Entity != null)
                    Entity.Position = _position;
            }
        }

        public virtual float Rotation
        {
            get { return Entity?.Rotation ?? _rotation; }
            set
            {
                _rotation = value;

                if (Entity != null)
                    Entity.Rotation = value;
            }
        }

        public virtual Vector2 Scale
        {
            get { return Entity?.Scale ?? _scale; }
            set
            {
                _scale = value;

                if (Entity != null)
                    _scale = value;
            }
        }
    }


    public class TransformableComponent<T> : EntityComponent, IMovable, IRotatable, IScalable 
        where T : Transform2D<T>
    {
        public TransformableComponent(T transformable)
        {
            _target = transformable;
        }

        private readonly T _target;
        public T Target
        {
            get
            {
                if (Entity != null)
                {
                    _target.Position = Entity.Position;
                    _target.Rotation = Entity.Rotation;
                    _target.Scale = Entity.Scale;
                }

                return _target;
            }
        }
    }
}