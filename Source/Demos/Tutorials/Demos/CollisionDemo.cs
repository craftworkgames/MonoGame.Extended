using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace Tutorials.Demos
{
    public class CollisionDemo : DemoBase
    {
        private CollisionComponent _collisionComponent;
        private List<DemoActor> _actors;
        private SpriteBatch _spriteBatch;
        private Texture2D _spikyBallTexture;
        private Texture2D _blankTexture;
        private DemoBall _controllableBall;

        public CollisionDemo(GameMain game) : base(game)
        {
        }

        public override string Name { get; } = "Collisions";

        protected override void LoadContent()
        {
            _collisionComponent = new CollisionComponent(new RectangleF(-10000, -5000, 20000, 10000));
            _actors = new List<DemoActor>();


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var spikeyBallTexture = Content.Load<Texture2D>("Textures/spike_ball");
            _spikyBallTexture = spikeyBallTexture;

            _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.WhiteSmoke });

            var spikeyBallRight = new DemoBall(new Sprite(_spikyBallTexture))
            {
                Position = new Vector2(600, 240),
                Velocity = new Vector2(0, 120)
            };
            _actors.Add(spikeyBallRight);

            var controllableBall = new ControllableBall(new Sprite(_spikyBallTexture))
            {
                Position = new Vector2(400, 240),
                Velocity = new Vector2(0, 0)
            };
            _actors.Add(controllableBall);
            _controllableBall = controllableBall;

            var topWall = new DemoWall(new Sprite(_blankTexture))
            {
                Bounds = new RectangleF(0, 0, 800, 20),
                Position = new Vector2(0, 0)
            };
            _actors.Add(topWall);

            var bottomWall = new DemoWall(new Sprite(_blankTexture))
            {
                Position = new Vector2(0, 460),
                Bounds = new RectangleF(0, 0, 800, 20)
            };
            _actors.Add(bottomWall);

            var spikeyBallCenter = new StationaryBall(new Sprite(_spikyBallTexture))
            {
                Position = new Vector2(400, 240),
                Velocity = Vector2.Zero,
            };
            _actors.Add(spikeyBallCenter);

            foreach (var actor in _actors)
            {
                _collisionComponent.Insert(actor);
            }

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateControlledBall(gameTime, _controllableBall);

            foreach (var actor in _actors)
            {
                actor.Update(gameTime);
            }
            _collisionComponent.Update(gameTime);
            base.Update(gameTime);
        }

        private void UpdateControlledBall(GameTime gameTime, DemoActor actor)
        {
            var kb = Keyboard.GetState();
            var speed = 150.0f;

            var position = actor.Position;
            var distance = speed * gameTime.GetElapsedSeconds();

            if (kb.IsKeyDown(Keys.W)) position.Y -= distance;
            if (kb.IsKeyDown(Keys.S)) position.Y += distance;
            if (kb.IsKeyDown(Keys.A)) position.X -= distance;
            if (kb.IsKeyDown(Keys.D)) position.X += distance;

            actor.Position = position;
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

    class DemoActor : ICollisionActor, IUpdate
    {
        private readonly Sprite _sprite;
        private Vector2 _position;

        public DemoActor(Sprite sprite)
        {
            _sprite = sprite;
            Bounds = sprite.GetBoundingRectangle(new Transform2());
        }

        /// <summary>
        /// Gets or sets the actor's position and updates teh actor's bounds
        /// position.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                Bounds.Position = value + Offset;
            }
        }

        /// <summary>
        /// Gets or sets the actor's collision bounds.
        /// </summary>
        public IShapeF Bounds { get; set; }

        /// <summary>
        /// Gets or sets how far the actor's collision bounds are offset from 
        /// the actor's position.
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Gets or sets the actor's velocity.
        /// </summary>
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
            Bounds = new CircleF(Position + Offset, 60);
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            Velocity *= -1;
            Position -= collisionInfo.PenetrationVector;
            base.OnCollision(collisionInfo);
        }
    }

    /// <summary>
    /// Ball that doesn't move when collided with.
    /// </summary>
    class StationaryBall : DemoBall
    {
        public StationaryBall(Sprite sprite) : base(sprite)
        {
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
    }

    /// <summary>
    /// Player controlled ball.
    /// </summary>
    class ControllableBall : DemoBall
    {
        public ControllableBall(Sprite sprite) : base(sprite)
        {
            Bounds = new CircleF(Position + Offset, 60);
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            Position -= collisionInfo.PenetrationVector;
        }
    }
}