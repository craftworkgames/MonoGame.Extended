using JamGame.Components;
using JamGame.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

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

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world = new WorldBuilder()
                .AddSystem(new MapRenderingSystem(GraphicsDevice, tileset))
                .AddSystem(new SpriteRenderingSystem(GraphicsDevice, texture))
                .AddSystem(new HudSystem(this, GraphicsDevice, font, tileset))
                .AddSystem(new PlayerMovementSystem(this))
                .Build();

            var player = _world.CreateEntity();
            player.Attach(new Sprite(tileset.GetTile(232)));
            player.Attach(new Transform2(100, 100));
            player.Attach(new Player());

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
