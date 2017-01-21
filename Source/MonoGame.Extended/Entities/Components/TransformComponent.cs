using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class TransformComponent
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;

        public float Depth { get; set; } = 0f;
    }
}
