using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.Easing;
using MonoGame.Extended.Animations.Fluent;
using MonoGame.Extended.Animations.Transformations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Tweening
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;
        private Animator _animator;
        public Game1() {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            _animator = new Animator(this);
            Components.Add(_animator);
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("logo-square-128");
            _sprite = new Sprite(logoTexture) {
                Position = viewportAdapter.Center.ToVector2()
            };
            var curve = new Curve(); //monogame curve
            curve.Keys.Add(new CurveKey(0, 0));
            curve.Keys.Add(new CurveKey(0.3f, 0.1f));
            curve.Keys.Add(new CurveKey(0.7f, 0.9f));
            curve.Keys.Add(new CurveKey(1, 1));

            var animation = new Animation("testanimation");
            animation.Transform(_sprite)
                .Tween(s => s.Alpha, 1000, 1) //interpolate
                .Tween(s => s.Alpha, 3000, 0, new StepEasing(5))
                .Tween(s => s.Alpha, 3500, 1)
                .Move(0, new Vector2(100, 100)) //IMoveable
                .Move(3300, new Vector2(600, 50), new CurveEasing(curve))
                .Move(6600, new Vector2(50, 400))
                .Move(8000, new Vector2(100, 100))
                .Set(s => s.Rotation, 5000, 3.14f) //dont interpolate
                .Set(s => s.Rotation, 5500, 0f)
                .Tween(s => s.Color, 0, Color.Red) //colors too
                .Tween(s => s.Color, 1000, Color.White);
            _animator.AddAnimation(animation);
            _animator.RunAnimation("testanimation", true);
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}