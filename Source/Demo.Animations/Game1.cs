using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogame.Extended.Animations.Transformations;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.Transformations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

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
            Components.Add(_animator);
        }

        private enum AnimationLayers
        {
            Movemement,
            Action,
            Face
        }

        protected override void Initialize() {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            var sprite = new Sprite(texture);

            var animation = new Animation("testanimation", 1000); //1 second
            animation.AnimationLayer = (int)AnimationLayers.Movemement;
            //tweenable transforms
            animation.AddTransfromations(sprite,
                //interface transforms
                new PositionTransform<Sprite>(0, new Vector2(100, 0)),
                new PositionTransform<Sprite>(100, new Vector2(10, 10), new CubicBezierEasing(0.1, 0.5, 1, 0.4)),
                new PositionTransform<Sprite>(500, new Vector2(20, 50)),
                new PositionTransform<Sprite>(1000, new Vector2(100, 0)),
                new ScaleTransform<Sprite>(0, Vector2.One),
                new ScaleTransform<Sprite>(250, Vector2.Zero),
                new ScaleTransform<Sprite>(500, Vector2.One),
                new RotationTransform<Sprite>(50, 3.14f),
                //type/reflection transforms
                new Vector2Transform<Sprite>(s => s.Origin, 0, new Vector2(0.5f, 0.5f)),
                new FloatTransform<Sprite>(s => s.Alpha, 0, 1),
                new ColorTransform<Sprite>(c => c.Color, 0, Color.Red)
                );
            //set transforms
            animation.AddTransfromations(sprite,
                new SetPropertyTransform<Sprite, TextureRegion2D>(s => s.TextureRegion, new TextureRegion2D("a", texture, 0, 0, 1, 1), 500)
                );

            _animator.AddAnimation(animation);

            _animator.RunAnimation("testanimation", true);

            _animator.RunAnimation(new Animation("non saved animation"), false);

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