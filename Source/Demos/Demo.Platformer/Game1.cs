using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Platformer
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private TiledMap _tiledMap;
        private Sprite _sprite;
        private SpriteSheetAnimator _animator;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var texture = Content.Load<Texture2D>("tiny-characters");
            var atlas = TextureAtlas.Create(texture, 32, 32, 15);
            var animationFactory = new SpriteSheetAnimationFactory(atlas);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] {0, 1, 2, 3}, isReversed: true));
            _animator = new SpriteSheetAnimator(animationFactory);
            _animator.Play("idle");
            _sprite = _animator.CreateSprite(position: new Vector2(116, 273));
            _tiledMap = Content.Load<TiledMap>("level-1");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                _sprite.Position += new Vector2(150, 0) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _sprite.Position -= new Vector2(150, 0) * deltaTime;

            _animator.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _tiledMap.Draw(_camera);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}