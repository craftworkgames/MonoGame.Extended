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
        //private Tween<float> _tween;

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
            
            _sprite = new Sprite(logoTexture) { Position = viewportAdapter.Center.ToVector2(), Scale = Vector2.One * 0.5f };
            _sprite
                .TweenSequence()
                .Move(new Vector2(100, 100), 1.0f, EasingFunctions.SineEaseOut)
                .Delay(1.0f)
                .RotateTo(MathHelper.Pi, 1.0f, EasingFunctions.CubicEaseInOut)
                .Delay(1.0f)
                .ScaleTo(Vector2.One*1.5f, 3.0f, EasingFunctions.QuadraticEaseInOut)
                .Delay(1.0f)
                .FadeTo(0.5f, 10.0f, EasingFunctions.SineEaseOut);
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

            //_sprite.Rotation += deltaTime;

            //_tween.Update(deltaTime);

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