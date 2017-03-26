using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Components
{
    [Component]
    [ComponentPool(Capacity = 20)]
    public class SpriteComponent : Component
    {
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public float Depth { get; }
        public SpriteEffects Effects { get; }
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; }

        public override void Reset()
        {
            Color = Color.White;
            Texture = null;
        }
    }
}
