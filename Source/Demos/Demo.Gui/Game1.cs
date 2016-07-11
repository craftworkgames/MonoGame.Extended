using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace Demo.Gui
{
    public interface IGuiDrawable
    {
        void Draw(SpriteBatch spriteBatch, RectangleF rectangle);
    }

    public class GuiSprite : IGuiDrawable
    {
        public TextureRegion2D TextureRegion { get; set; }

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            spriteBatch.Draw(TextureRegion, rectangle.Location, Color.White);
        }
    }

    public class GuiText : IGuiDrawable
    {
        public BitmapFont Font { get; set; }
        public string Text { get; set; }
        
        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            spriteBatch.DrawString(Font, Text, rectangle.Location, Color.White);
        }
    }

    public class GuiTemplate
    {
        public GuiTemplate()
        {
            Drawables = new List<IGuiDrawable>();
        }

        public List<IGuiDrawable> Drawables { get; }

        public void Draw(SpriteBatch spriteBatch, RectangleF rectangle)
        {
            foreach (var drawable in Drawables)
                drawable.Draw(spriteBatch, rectangle);
        }
    }

    public class GuiDefinition
    {
        public string TextureAtlas { get; set; }
        public string[] Fonts { get; set; }
    }

    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private TextureAtlas _textureAtlas;
        private SpriteBatch _spriteBatch;
        private GuiTemplate _template;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            //Components.Add(new GuiComponent(this, @"title-screen.gui"));
        }

        protected override void LoadContent()
        {
            const string json =
@"{ 
    'TextureAtlas': 'Content/kenney-gui-blue-atlas.xml',
    'Fonts': [ 'montserrat-32' ],
    
  }";

            var guiDefinition = JsonConvert.DeserializeObject<GuiDefinition>(json);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var bitmapFont = Content.Load<BitmapFont>(guiDefinition.Fonts[0]);

            using (var stream = TitleContainer.OpenStream(guiDefinition.TextureAtlas))
            {
                _textureAtlas = TextureAtlasReader.FromRawXml(Content, stream);

                var textureRegion = _textureAtlas["blue_button00"];
                _template = new GuiTemplate
                {
                    Drawables =
                    {
                        new GuiSprite { TextureRegion = textureRegion },
                        new GuiText { Font = bitmapFont, Text = "Monkey" }
                    }
                };
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
            var rectangleF = new RectangleF(100, 100, 200, 100);
            _template.Draw(_spriteBatch, rectangleF);
            _spriteBatch.DrawRectangle(rectangleF, Color.Red);
            _spriteBatch.End();
        }
    }
}