using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.Transformation;
using MonoGame.Extended.Sprites;

namespace Demo.Animations
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Animator _animator;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _animator = new Animator(this);
        }

        private enum AnimationLayers
        {
            Movemement,
            Action,
            Face
        }

        protected override void Initialize() {
            var animation = new Animation("testanimation", 1000); //1 second
            var sprite = new Sprite(new Texture2D(GraphicsDevice, 1, 1));

            animation.AddTransfromations(sprite,
                new PositionTransform<Sprite>(0, new Vector2(100, 0)),
                new PositionTransform<Sprite>(100, new Vector2(10, 10), new QuadraticBezierEasing(0.1, 0.5, 1, 0.4)),
                new PositionTransform<Sprite>(500, new Vector2(20, 50)),
                new PositionTransform<Sprite>(1000, new Vector2(100, 0)),
                new ScaleTransform<Sprite>(0, Vector2.One),
                new ScaleTransform<Sprite>(250, Vector2.Zero),
                new ScaleTransform<Sprite>(500, Vector2.One)
                );
            _animator.AddAnimation(animation, (int)AnimationLayers.Action);
            _animator.RunAnimation("testanimation", false);
            base.Initialize();
        }


        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}