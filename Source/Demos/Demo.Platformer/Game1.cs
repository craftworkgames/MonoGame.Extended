using Demo.Platformer.Entities;
using Demo.Platformer.Entities.Components;
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
        private EntityGameComponent _entityManager;

        //private Sprite _sprite0;
        //private Sprite _sprite1;
        //private Texture2D _logo;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            Components.Add(_entityManager = new EntityGameComponent(this));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //_logo = Content.Load<Texture2D>("logo-square-128");

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var texture = Content.Load<Texture2D>("tiny-characters");
            var atlas = TextureAtlas.Create(texture, 32, 32, 15);
            var animationFactory = new SpriteSheetAnimationFactory(atlas);

            var entity = _entityManager.CreateEntity();
            entity.AttachComponent(new SpriteComponent(atlas[0]));
            entity.Position = new Vector2(300, 300);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] {0, 1, 2, 3}, isReversed: true));
            _animator = new SpriteSheetAnimator(animationFactory);
            _animator.Play("idle");
            _sprite = _animator.CreateSprite(position: new Vector2(116, 273));
            //_sprite1 = _animator.CreateSprite(position: new Vector2(132, 273));
            _tiledMap = Content.Load<TiledMap>("level-1");

            var viewport = GraphicsDevice.Viewport;
            //_alphaTestEffect = new AlphaTestEffect(GraphicsDevice)
            //{
            //    Projection =
            //        Matrix.CreateTranslation(-0.5f, -0.5f, 0)*
            //        Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1),
            //    VertexColorEnabled = true,
            //    ReferenceAlpha = 128
            //};
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

        private AlphaTestEffect _alphaTestEffect;

        protected override void Draw(GameTime gameTime)
        {
            var viewMatrix = _camera.GetViewMatrix();

            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //_spriteBatch.Begin(
            //    sortMode: SpriteSortMode.FrontToBack, 
            //    samplerState: SamplerState.PointClamp, 
            //    depthStencilState: DepthStencilState.Default,
            //    blendState: BlendState.AlphaBlend,
            //    effect: _alphaTestEffect,
            //    transformMatrix: viewMatrix);

            //_spriteBatch.Draw(_logo, position: new Vector2(305, 160), color: Color.White, layerDepth: 0.5f);
            //_spriteBatch.Draw(_logo, position: new Vector2(375, 210), color: Color.Green, layerDepth: 0.25f);

            //_spriteBatch.End();
            
            _tiledMap.Draw(viewMatrix);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewMatrix);
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}