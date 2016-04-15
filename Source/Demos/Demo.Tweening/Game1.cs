using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.Fluent;
using MonoGame.Extended.Animations.Transformations;
using MonoGame.Extended.Interpolation.Easing;
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
        private TweeningAnimatorComponent _animatorComponent;
        public Game1() {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            _animatorComponent = new TweeningAnimatorComponent(this);
            Components.Add(_animatorComponent);
        }

        private Texture2D _logoTexture;
        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            _logoTexture = Content.Load<Texture2D>("logo-square-128");
            _sprite = new Sprite(_logoTexture) {
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
                .Tween(s => s.Alpha, 3000, 0, new LinearEasing { StepCount = 5 })
                .Tween(s => s.Alpha, 3500, 1)
                .Move(0, new Vector2(100, 100)) //IMoveable
                .Move(3300, new Vector2(600, 50), new CurveEasing(curve))
                .Move(6600, new Vector2(50, 400))
                .Move(8000, new Vector2(100, 100))
                .Set(s => s.Rotation, 5000, 3.14f) //dont interpolate
                .Set(s => s.Rotation, 5500, 0f)
                .Tween(s => s.Color, 0, Color.Red) //colors too
                .Tween(s => s.Color, 1000, Color.White);
            _animatorComponent.AddAnimation(animation);
            _animatorComponent.RunAnimation("testanimation", true);
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();
            if (keyboardState.IsKeyDown(Keys.D1)) _testFunction.EasingInOut = EasingInOut.In;
            if (keyboardState.IsKeyDown(Keys.D2)) _testFunction.EasingInOut = EasingInOut.Out;
            if (keyboardState.IsKeyDown(Keys.D3)) _testFunction.EasingInOut = EasingInOut.InOut;
            if (keyboardState.IsKeyDown(Keys.D4)) _testFunction.EasingInOut = EasingInOut.OutIn;

            if (keyboardState.IsKeyDown(Keys.Q)) _testFunction = new BackEasing();
            if (keyboardState.IsKeyDown(Keys.W)) _testFunction = new BounceEasing();
            if (keyboardState.IsKeyDown(Keys.E)) _testFunction = new CircularEasing();
            if (keyboardState.IsKeyDown(Keys.R)) _testFunction = new ElasticEasing();
            if (keyboardState.IsKeyDown(Keys.T)) _testFunction = new ExponentialEasing();
            if (keyboardState.IsKeyDown(Keys.Y)) _testFunction = new HermiteEasing();
            if (keyboardState.IsKeyDown(Keys.U)) _testFunction = new LogarithmicEasing();
            if (keyboardState.IsKeyDown(Keys.I)) _testFunction = PowerEasing.CubicEasing;
            if (keyboardState.IsKeyDown(Keys.O)) _testFunction = PowerEasing.QuinticEasing;
            if (keyboardState.IsKeyDown(Keys.P)) _testFunction = new SinusoidalEasing();

            if (keyboardState.IsKeyDown(Keys.NumPad0)) _testFunction.StepCount = null;
            if (keyboardState.IsKeyDown(Keys.NumPad1)) _testFunction.StepCount = 1;
            if (keyboardState.IsKeyDown(Keys.NumPad2)) _testFunction.StepCount = 2;
            if (keyboardState.IsKeyDown(Keys.NumPad3)) _testFunction.StepCount = 10;
            if (keyboardState.IsKeyDown(Keys.NumPad4)) _testFunction.StepCount = 40;
            if (keyboardState.IsKeyDown(Keys.NumPad5)) _testFunction.StepCount = 460;

            if (keyboardState.IsKeyDown(Keys.G)) _testFunction.RoundStepUp = false;
            if (keyboardState.IsKeyDown(Keys.H)) _testFunction.RoundStepUp = true;

            var title = _testFunction.GetType().Name + "-" + _testFunction.EasingInOut;
            if (_testFunction.StepCount.HasValue) title += "-" + _testFunction.StepCount.Value + "steps";
            Window.Title = title;

            base.Update(gameTime);
        }

        private EasingFunction _testFunction = EasingFunction.None;

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sprite);
            for (int i = 0; i < 460; i++) {
                var t = i / 460f;
                _spriteBatch.Draw(_logoTexture, new Vector2(330 + t * 460,10+ (1 - (float)_testFunction.Ease(t)) * 460), scale: new Vector2(0.05f, 0.05f));
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}