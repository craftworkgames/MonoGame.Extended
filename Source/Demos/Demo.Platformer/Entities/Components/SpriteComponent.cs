using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    [EntityComponent]
    [EntityComponentPool(InitialSize = 100)]
    public class SpriteComponent : EntityComponent
    {
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public float Depth { get; set; }
        public SpriteEffects Effects { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; }

        public override void Reset()
        {
            Color = Color.White;
            Texture = null;
        }
    }
}
