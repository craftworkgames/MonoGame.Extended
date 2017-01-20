using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Systems
{
    public class SpriteBatchSystem : EntitySystem
    {
        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public SpriteBatchSystem(GraphicsDevice graphicsDevice, Camera2D camera, SpriteBatch spriteBatch = null)
        {
            _camera = camera;
            _spriteBatch = spriteBatch ?? new SpriteBatch(graphicsDevice);
        }

        public Effect Effect { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public SamplerState SamplerState { get; set; }
        public BlendState BlendState { get; set; }
        public SpriteSortMode SortMode { get; set; } = SpriteSortMode.Deferred;

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var sprites = GetComponent<SpriteCollectionComponent>()?.AnimatedSprites;

            if (sprites != null)
            {
                foreach (AnimatedSprite animatedSprite in sprites)
                    animatedSprite.Update(deltaTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var sprites = GetComponent<SpriteCollectionComponent>().Sprites;
            //var emitters = GetComponents<ParticleEmitter>();
            var transformMatrix = _camera.GetViewMatrix();

            _spriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect,
                transformMatrix);

            if (sprites != null)
            {
                foreach (var sprite in sprites)
                    DrawSprite(sprite);
            }

            //if (emitters != null)
            //{
            //    foreach (var particleEmitter in emitters)
            //        _spriteBatch.Draw(particleEmitter);
            //}

            _spriteBatch.End();
        }

        private void DrawSprite(Sprite sprite)
        {
            if (sprite.IsVisible)
            {
                var texture = sprite.TextureRegion.Texture;
                var sourceRectangle = sprite.TextureRegion.Bounds;

                var transform = Entity.Transform;

                _spriteBatch.Draw(texture, 
                    transform.WorldPosition, 
                    sourceRectangle, 
                    sprite.Color * sprite.Alpha, 
                    sprite.Rotation + transform.WorldRotation,
                    sprite.Origin,
                    sprite.Scale * transform.WorldScale, 
                    sprite.Effect, 
                    sprite.Depth);
            }
        }
    }
}