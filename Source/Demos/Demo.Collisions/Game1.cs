using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Graphics.Effects;

namespace Demo.Collisions
{
    public class Game1 : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        //private CollisionSimulation _collisionSimulation;

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
//            _collisionSimulation = new CollisionSimulation();
//
//            var body = _collisionSimulation.CreateBody(null);
//            var fixture1 = _collisionSimulation.CreateFixture(body, CollisionShape2D.Create(new[]
//            {
//                new Vector2(-1, 2),
//                new Vector2(-1, 0),
//                new Vector2(0, -3),
//                new Vector2(1, 0),
//                new Vector2(1, 1)
//            }));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;
            var viewport = graphicsDevice.Viewport;

            PrimitiveBatchHelper.SortAction = Array.Sort;

            //_collisionSimulation.Drawer = new CollisionDrawer(graphicsDevice, collisionDebugEffect);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //_collisionSimulation.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //_collisionSimulation.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
