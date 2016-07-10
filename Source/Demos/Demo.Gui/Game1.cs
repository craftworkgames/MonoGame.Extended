using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private TextureAtlas _textureAtlas;
        private Sprite _sprite;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Components.Add(new GuiComponent(this, @"title-screen.gui"));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            using (var stream = TitleContainer.OpenStream(@"Content\kenney-gui-blue-atlas.xml"))
            {
                _textureAtlas = TextureAtlasReader.FromRawXml(Content, stream);
                _sprite = new Sprite(_textureAtlas["blue_boxCheckmark"]) {Position = new Vector2(400, 240)};
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();
        }
    }
}