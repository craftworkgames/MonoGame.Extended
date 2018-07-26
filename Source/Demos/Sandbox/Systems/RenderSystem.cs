using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Sandbox.Components;

namespace Sandbox.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Raindrop> _raindropMapper;
        
        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(Transform2), typeof(Raindrop)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _raindropMapper = mapperService.GetMapper<Raindrop>();
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.DarkBlue * 0.2f);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var raindrop = _raindropMapper.Get(entity);

                _spriteBatch.FillRectangle(transform.Position, new Size2(raindrop.Size, raindrop.Size), Color.LightBlue);
            }

            _spriteBatch.End();
        }

    }
}