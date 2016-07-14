using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Wip;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private GuiComponent _guiComponent;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Components.Add(_guiComponent = new GuiComponent(this));
        }

        protected override void LoadContent()
        {
            _guiComponent.LoadGui("title-screen.gui");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
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

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

            //_spriteBatch.DrawRectangle(rectangleF, Color.Red);
            _spriteBatch.End();
        }
    }
}