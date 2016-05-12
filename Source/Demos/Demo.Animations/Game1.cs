using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.Tweens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Animations
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Components.Add(new AnimationComponent(this));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("logo-square-128");
            
            _sprite = new Sprite(logoTexture)
            {
                Position = viewportAdapter.Center.ToVector2(),
                Scale = Vector2.One * 0.5f,
                Color = new Color(Color.White, 0.0f)
            };

            CreateTweenThing();

        }

        private void CreateTweenThing()
        {
            //_sprite.CreateTweenChain()
            //    .Move(new Vector2(10, 10), 1.0f, EasingFunctions.SineEaseOut)
            //    .Rotate(MathHelper.Pi, 1.0f, EasingFunctions.CubicEaseInOut)
            //    .Scale(new Vector2(0.9f), 1.0f, EasingFunctions.QuadraticEaseInOut);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _sprite
                    .CreateTweenGroup()
                    .MoveTo(new Vector2(mouseState.X, mouseState.Y), 1.0f, EasingFunctions.BounceOut);
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}