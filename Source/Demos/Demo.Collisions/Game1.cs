using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collision;
using MonoGame.Extended.Collision.Detection.Broadphase;
using MonoGame.Extended.Collision.Detection.Narrowphase;

namespace Demo.Collisions
{
    public class Game1 : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private CollisionSimulation2D _simulation;

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

            _colliderA = _simulation.CreateBoxCollider(null, new SizeF(50, 50));
            _colliderA.Transform.Position = new Vector2(50, 50);

            _colliderB = _simulation.CreateBoxCollider(null, new SizeF(100, 100));
            _colliderB.Transform.Position = new Vector2(150, 150);

            _simulation.BroadphaseCollision += OnBroadphaseCollision;
            _simulation.NarrowphaseCollision += OnNarrowphaseCollision;

            base.Initialize();
        }


        private void OnBroadphaseCollision(ref BroadphaseCollisionResult2D result, out bool cancel)
        {
            // broadphase collision detected between two colliders
            // this means the bounding volume of the two colliders are intersecting
            // you can cancel the collision here to prevent it from being considered for the narrow phase

            cancel = false;
        }

        private void OnNarrowphaseCollision(ref NarrowphaseCollisionResult2D result, out bool cancel)
        {
            // narrowphase collision detected between two colliders
            // this means the geometry of the two colliders are intersecting
            // you can cancel the collision here to prevent it from being processed by the responder

            // here we are just moving the first collider to a safe position
            var minimumTranslationVector = result.MinimumPenetration * result.MinimumPenetrationAxis;
            result.FirstCollider.Transform.Position += minimumTranslationVector;

            cancel = false;
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
            _colliderA.Transform.Rotation += MathHelper.ToRadians(1);

            ProcessColliderMovementInput(gameTime);

            // update simulation after moving things, otherwise simulating previous frame
            _simulation.Update(gameTime);

            base.Update(gameTime);
        }

        private void ProcessColliderMovementInput(GameTime gameTime)
        {
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
                // ensure the direction is a unit vector
                direction.Normalize();

                var speed = 50;
                _colliderA.Transform.Position += speed * direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _simulation.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
