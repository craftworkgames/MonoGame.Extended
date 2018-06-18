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
        private CollisionComponent _collisionComponent;
        private List<DemoActor> _actors;
        private SpriteBatch _spriteBatch;
        private Texture2D _spikeyBallTexture;
        private Texture2D _blankTexture;

        public CollisionDemo(GameMain game) : base(game)
        {
        }

        public override string Name { get; } = "Collisions";

        protected override void LoadContent()
        {
            _collisionComponent = new CollisionComponent(new RectangleF(-1000, -500, 2000, 1000));
            _actors = new List<DemoActor>();


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var spikeyBallTexture = Content.Load<Texture2D>("Textures/spike_ball");
            _spikeyBallTexture = spikeyBallTexture;

            _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.WhiteSmoke });

            var demoBall1 = new DemoBall(new Sprite(_spikeyBallTexture))
            {
                Position = new Vector2(200, 240),
                Bounds = new RectangleF(200, 240, 20, 20),
                Velocity = new Vector2(0, -120)
            };
            _actors.Add(demoBall1);

            var demoBall2 = new DemoBall(new Sprite(_spikeyBallTexture))
            {
                Position = new Vector2(600, 240),
                Bounds = new RectangleF(600, 240, 20, 20),
                Velocity = new Vector2(0, 120)
            };
            _actors.Add(demoBall2);

            var wall1 = new DemoWall(new Sprite(_blankTexture));
            wall1.Bounds = new RectangleF(0, 0, 800, 20);
            _actors.Add(wall1);

            var wall2 = new DemoWall(new Sprite(_blankTexture));
            wall2.Bounds = new RectangleF(0, 460, 800, 20);
            _actors.Add(wall2);

            foreach (var actor in _actors)
            {
                _collisionComponent.Insert(actor);
            }

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var actor in _actors)
            {
                actor.Update(gameTime);
            }
            _collisionComponent.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            foreach (var actor in _actors)
            {
                //_spriteBatch.Draw(_blankTexture, actor.BoundingBox.ToRectangle(), Color.WhiteSmoke);
                actor.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    #region Collision Demo Implementation

    class DemoActor : ICollisionActor, IUpdate
    {
        private readonly Sprite _sprite;

        public DemoActor(Sprite sprite)
        {
            _sprite = sprite;
            Bounds = sprite.GetBoundingRectangle(new Transform2());
        }


        public Vector2 Position { get; set; }
        public IShapeF Bounds { get; set; }

        public Vector2 Velocity { get; set; }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, Position, 0, Vector2.One);
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += gameTime.GetElapsedSeconds() * Velocity;
        }
    }

    class DemoWall : DemoActor
    {
        public DemoWall(Sprite sprite) : base(sprite)
        {
        }
    }

    /// <summary>
    /// Ball that bounces on wall
    /// </summary>
    class DemoBall : DemoActor
    {
        public DemoBall(Sprite sprite) : base(sprite)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Bounds.Position = Position;
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {

            Velocity *= -1;
            Position -= collisionInfo.PenetrationVector;
            Bounds.Position = Position;
            base.OnCollision(collisionInfo);
        }
    }

    #endregion
}