using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Components
{
    public abstract class EntityComponent : IMovable, IRotatable, IScalable
    {
        protected EntityComponent()
        {
        }

        public Entity Entity { get; internal set; }
        
        private Vector2 _position;
        public Vector2 Position
        {
            get { return Entity?.Position ?? _position; }
            set
            {
                _position = value;

                if (Entity != null)
                    Entity.Position = _position;
            }
        }

        private float _rotation;
        public float Rotation
        {
            get { return Entity?.Rotation ?? _rotation; }
            set
            {
                _rotation = value;

                if (Entity != null)
                    Entity.Rotation = value;
            }
        }

        private Vector2 _scale;
        public Vector2 Scale
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
}