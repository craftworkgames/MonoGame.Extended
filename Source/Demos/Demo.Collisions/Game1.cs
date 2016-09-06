using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collision;
using MonoGame.Extended.Collision.Broadphase.BoundingVolumes;
using MonoGame.Extended.Collision.Narrowphase.Shapes;
using MonoGame.Extended.Entities;

namespace Demo.Collisions
{
    public class Game1 : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private readonly EntityComponentSystem _entityComponentSystem = new EntityComponentSystem();

        private CollisionSimulation2D _simulation;

        private Entity _entityA;
        private Entity _entityB;

        private Collider2D _colliderA;
        private Collider2D _colliderB;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void Initialize()
        {
            _simulation = new CollisionSimulation2D();

            _entityA = _entityComponentSystem.CreateEntity(new Vector2(50, 50));
            _entityB = _entityComponentSystem.CreateEntity(new Vector2(150, 150));

            _colliderA = _simulation.CreateCollider(_entityA, new Box2D
                {
                    Size = new SizeF(50, 50)
                },
                BoundingVolumeType2D.BoundingBox);
            _colliderB = _simulation.CreateCollider(_entityB, new Box2D
            {
                Size = new SizeF(100, 100)
            },
                BoundingVolumeType2D.BoundingBox);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _simulation.Initialize(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _simulation.Update(gameTime);

            _entityA.Rotation += MathHelper.ToRadians(1);

            var keyboardState = Keyboard.GetState();

            var direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                direction += Vector2.UnitY;
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                direction -= Vector2.UnitY;
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                direction -= Vector2.UnitX;
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                direction += Vector2.UnitX;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                var speed = 50;
                _entityA.Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _simulation.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
