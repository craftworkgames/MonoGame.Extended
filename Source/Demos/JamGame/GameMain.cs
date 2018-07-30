using JamGame.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;

namespace JamGame
{
    public class MainGame : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private World _world;

        public MainGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1400,
                PreferredBackBufferHeight = 900
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        public MouseState MouseState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        protected override void LoadContent()
        {
            var font = Content.Load<BitmapFont>("Sensation");
            var texture = Content.Load<Texture2D>("0x72_16x16DungeonTileset.v4");
            var tileset = new Tileset(texture, 16, 16);
            var entityFactory = new EntityFactory(tileset);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world = new WorldBuilder()
                .AddSystem(new MapRenderingSystem(GraphicsDevice, tileset))
                .AddSystem(new SpriteRenderingSystem(GraphicsDevice, texture))
                .AddSystem(new HudSystem(this, GraphicsDevice, font, tileset))
                .AddSystem(new PlayerControlSystem(this, entityFactory))
                .AddSystem(new BodyMovementSystem())
                .AddSystem(new CollisionSystem())
                .AddSystem(new CollisionResponseSystem())
                .Build();

            entityFactory.World = _world;
            entityFactory.SpawnPlayer(100, 100);

            entityFactory.SpawnZombie(200, 80);
            entityFactory.SpawnSkeleton(200, 100);
            entityFactory.SpawnPurpleThing(200, 120);
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _world.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _world.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
