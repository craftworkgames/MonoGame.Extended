using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Features.Demos
{
    public class TweeningDemo : DemoBase
    {
        public override string Name => "Tweening";

        private readonly Game _game;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;

        public TweeningDemo(GameMain game) : base(game)
        {
            _game = game;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var animationComponent = new AnimationComponent(_game);
            var tweenComponent = new TweeningComponent(_game, animationComponent);
            Components.Add(animationComponent);
            Components.Add(tweenComponent);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("Textures/logo-square-128");

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
            _sprite.CreateTweenChain(CreateTweenThing)
                .Rotate(MathHelper.Pi, 1.0f, EasingFunctions.BounceOut)
                .Scale(new Vector2(1.1f), 1.0f, EasingFunctions.BounceOut);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
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