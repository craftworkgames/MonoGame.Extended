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
        private Sprite _sprite0;
        private Sprite _sprite1;
        private SpriteSheetAnimator _animator;
        private Texture2D _logo;

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

            _logo = Content.Load<Texture2D>("logo-square-128");

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var texture = Content.Load<Texture2D>("tiny-characters");
            var atlas = TextureAtlas.Create(texture, 32, 32, 15);
            var animationFactory = new SpriteSheetAnimationFactory(atlas);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] {0, 1, 2, 3}, isReversed: true));
            _animator = new SpriteSheetAnimator(animationFactory);
            _animator.Play("idle");
            _sprite0 = _animator.CreateSprite(position: new Vector2(116, 273));
            _sprite1 = _animator.CreateSprite(position: new Vector2(132, 273));
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
                _sprite0.Position += new Vector2(150, 0) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _sprite0.Position -= new Vector2(150, 0) * deltaTime;

            //_animator.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _tiledMap.Draw(_camera.GetViewMatrix());
            
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

            _spriteBatch.Draw(_logo, position: new Vector2(250, 250), color: Color.White, layerDepth: 0.5f);
            _spriteBatch.Draw(_logo, position: new Vector2(300, 300), color: Color.Green, layerDepth: 0.25f);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}