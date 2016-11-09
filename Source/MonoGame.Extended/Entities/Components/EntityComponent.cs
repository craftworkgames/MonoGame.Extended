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