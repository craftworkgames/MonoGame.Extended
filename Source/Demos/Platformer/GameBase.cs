using System.Reflection;
using Autofac;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Platformer
{
    public abstract class GameBase : Game
    {
        // ReSharper disable once NotAccessedField.Local
        protected GraphicsDeviceManager GraphicsDeviceManager { get; }
        protected IContainer Container { get; private set; }
        protected EntityComponentSystem EntityComponentSystem { get; private set; }
        protected KeyboardInputService KeyboardInputService { get; }

        public int Width { get; }
        public int Height { get; }

        protected GameBase(int width = 800, int height = 480)
        {
            Width = width;
            Height = height;
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = width,
                PreferredBackBufferHeight = height
            };
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            KeyboardInputService = new KeyboardInputService();
        }

        protected override void Dispose(bool disposing)
        {
            EntityComponentSystem.Dispose();
            base.Dispose(disposing);
        }

        protected override void Initialize()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(KeyboardInputService);

            RegisterDependencies(containerBuilder);
            Container = containerBuilder.Build();
            EntityComponentSystem = new EntityComponentSystem(this, new AutofacDependencyResolver(Container));
            EntityComponentSystem.Scan(Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly());

            base.Initialize();
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder);

        protected override void Update(GameTime gameTime)
        {
            KeyboardInputService.Update();
            EntityComponentSystem.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);
            EntityComponentSystem.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}