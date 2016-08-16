using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Platformer.Entities.Components
{
    public abstract class EntityComponent
    {
        protected EntityComponent()
        {
        }

        public Entity Entity { get; internal set; }

        public abstract void Update(float deltaTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public class SpriteComponent : EntityComponent, ISpriteBatchDrawable
    {
        public SpriteComponent(TextureRegion2D textureRegion)
        {
            IsVisible = true;
            TextureRegion = textureRegion;
            Color = Color.White;
            Origin = new Vector2(textureRegion.Size.Width, textureRegion.Size.Height)*0.5f;
            Effect = SpriteEffects.None;
        }

        public bool IsVisible { get; set; }
        public TextureRegion2D TextureRegion { get; set; }
        public Color Color { get; }
        public Vector2 Origin { get; }
        public SpriteEffects Effect { get; }
        public Vector2 Position => Entity.Position;
        public float Rotation => Entity.Rotation;
        public Vector2 Scale => Entity.Scale;

        public override void Update(float deltaTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRegion, Position, Color, Rotation, Origin, Scale, Effect, 0);
        }
    }
}
