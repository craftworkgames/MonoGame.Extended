using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities.Systems
{
    public class SpriteBatchSystem : EntitySystem
    {
        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public SpriteBatchSystem(SpriteBatch spriteBatch, Camera2D camera)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
        }

        public Effect Effect { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public SamplerState SamplerState { get; set; }
        public BlendState BlendState { get; set; }
        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.BackToFront;

        protected override void Update(Entity entity, GameTime gameTime)
        {
            var sprites = entity.GetComponents<SpriteComponent>();
            foreach (SpriteComponent sprite in sprites)
            {
                if (sprite.CurrentAnimation != null && !sprite.CurrentAnimation.IsComplete)
                {
                    sprite.CurrentAnimation.Update(gameTime);
                    sprite.TextureRegion = sprite.CurrentAnimation.CurrentFrame;
                }
            }
        }

        protected override void Draw(Entity entity, GameTime gameTime)
        {
            var entityTransform = entity.GetComponent<Components.Transform>();
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect,
                transformMatrix);

            var sprites = entity.GetComponents<SpriteComponent>();
            foreach (SpriteComponent sprite in sprites)
                DrawSprite(sprite, entityTransform);

            //foreach (ParticleEmitter particleEmitter in entity.GetComponents<ParticleEmitter>())
            //    _spriteBatch.Draw(particleEmitter);

            _spriteBatch.End();
        }

        private void DrawSprite(SpriteComponent sprite, Transform transform)
        {
            if (!sprite.IsVisible || sprite.TextureRegion == null)
                return;

            _spriteBatch.Draw(
                texture         : sprite.TextureRegion.Texture,
                position        : sprite.Position + (transform?.Position ?? Vector2.Zero),
                sourceRectangle : sprite.TextureRegion.Bounds,
                color           : sprite.Color * sprite.Alpha,
                rotation        : sprite.Rotation + (transform?.Rotation ?? 0f),
                origin          : sprite.Origin,
                scale           : sprite.Scale * (transform?.Scale ?? Vector2.One),
                effects         : sprite.Effect,
                layerDepth      : sprite.Depth);
        }
    }
}