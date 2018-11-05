using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace Features.Demos
{
    public class CollisionDemo : DemoBase
    {
        private CollisionComponent _collisionComponent;
        private List<DemoActor> _actors;
        private SpriteBatch _spriteBatch;
        private Texture2D _spikeyBallTexture;
        private Texture2D _blankTexture;
        private DemoBall _controllableBall;

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
                Velocity = new Vector2(0, -120)
            };
            _actors.Add(demoBall1);

            var demoBall2 = new DemoBall(new Sprite(_spikeyBallTexture))
            {
                Position = new Vector2(600, 240),
                Velocity = new Vector2(0, 120)
            };
            _actors.Add(demoBall2);

            var controllableBall = new DemoBall(new Sprite(_spikeyBallTexture))
            {
                Position = new Vector2(400, 240),
                Velocity = new Vector2(0, 0)
            };
            _actors.Add(controllableBall);
            _controllableBall = controllableBall;

            var wall1 = new DemoWall(new Sprite(_blankTexture))
            {
                Bounds = new RectangleF(0, 0, 800, 20),
                Position = new Vector2(0, 0)
            };
            _actors.Add(wall1);

            var wall2 = new DemoWall(new Sprite(_blankTexture))
            {
                Position = new Vector2(0, 460),
                Bounds = new RectangleF(0, 0, 800, 20)
            };
            _actors.Add(wall2);

            var centerWall = new DemoWall(new Sprite(_blankTexture))
            {
                Bounds = new RectangleF(0, 0, 50, 50),
                Position = new Vector2(400, 200)
            };
            _actors.Add(centerWall);

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
                if (actor.Bounds is RectangleF rect)
                {
                    _spriteBatch.Draw(_blankTexture, rect.ToRectangle(), Color.WhiteSmoke);
                } else if (actor.Bounds is CircleF circle)
                {
                    _spriteBatch.Draw(_blankTexture, circle.Position, Color.WhiteSmoke);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    #region Collision Demo Implementation

    class DemoActor : ICollisionActor, IUpdate
    {
        private readonly Sprite _sprite;
        private Vector2 _position;

        public DemoActor(Sprite sprite)
        {
            _sprite = sprite;
            Bounds = sprite.GetBoundingRectangle(new Transform2());
        }


        public Vector2 Offset { get; set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Bounds.Position = value + Offset;
            }
        }

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
            Offset = new Vector2(75, 75);
            Bounds = new CircleF(Position + Offset, 60);
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
            base.OnCollision(collisionInfo);
        }
    }

    #endregion
}