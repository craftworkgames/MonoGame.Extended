using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace Demo.Features.Demos
{
    public class CollisionDemo : DemoBase
    {
        private readonly CollisionComponent _collisionComponent =
            new CollisionComponent(new RectangleF(-1000, -500, 2000, 1000));
        private List<DemoActor> _actors = new List<DemoActor>();
        private SpriteBatch _spriteBatch;
        private Texture2D _spikeyBallTexture;

        public CollisionDemo(GameMain game) : base(game)
        {
        }

        public override string Name { get; } = "Collisions";

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var spikeyBallTexture = Content.Load<Texture2D>("Textures/spike_ball");
            _spikeyBallTexture = spikeyBallTexture;

            for (int i = 0; i < 3; i++)
            {
                var demoActor = new DemoActor(new Sprite(_spikeyBallTexture));
                demoActor.Position = new Vector2(100, 100);
                demoActor.BoundingBox = new RectangleF(100, 100, 20, 20);

                _actors.Add(demoActor);
                _collisionComponent.Insert(demoActor);
            }

            base.LoadContent();
        }

        protected override void Initialize()
        {
            

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _collisionComponent.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            foreach (var actor in _actors)
            {
                actor.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    #region Collision Demo Implementation

    class DemoActor : IActorTarget
    {
        private readonly Sprite _sprite;

        public DemoActor(Sprite sprite)
        {
            _sprite = sprite;
        }


        public Vector2 Position { get; set; }
        public RectangleF BoundingBox { get; set; }
        public Vector2 Velocity { get; set; }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            Velocity *= -1;
            Position += Vector2.One;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Position = Position;
            _sprite.Draw(spriteBatch);
        }
    }

    #endregion
}