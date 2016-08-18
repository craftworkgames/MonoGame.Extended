using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Platformer.Entities.Components
{
    public class SpriteComponent : EntityComponent
    {
        public SpriteComponent(TextureRegion2D textureRegion)
        {
            IsVisible = true;
            TextureRegion = textureRegion;
            Color = Color.White;
            Origin = new Vector2(textureRegion.Size.Width, textureRegion.Size.Height)*0.5f;
            Effect = SpriteEffects.None;
            Alpha = 1.0f;
            Depth = 0.0f;
        }

        public bool IsVisible { get; set; }
        public TextureRegion2D TextureRegion { get; set; }
        public Color Color { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effect { get; set; }
        public Vector2 Position => Entity.Position;
        public float Rotation => Entity.Rotation;
        public Vector2 Scale => Entity.Scale;
        public float Alpha { get; set; }
        public float Depth { get; set; }
    }
}
