using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Features.Demos
{
    public class SceneGraphsDemo : DemoBase
    {
        public override string Name => "Scene Graphs";

        private BitmapFont _bitmapFont;
        private Camera2D _camera;
        private SceneNode _carNode;
        private SceneNode _hoveredNode;
        private SceneNode _leftWheelNode;
        private SceneNode _rightWheelNode;
        private SceneGraph _sceneGraph;

        private float _speed = 0.15f;
        private SpriteBatch _spriteBatch;

        public SceneGraphsDemo(GameMain game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _bitmapFont = Content.Load<BitmapFont>("Fonts/montserrat-32");

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter)
            {
                Zoom = 2.0f
            };
            _sceneGraph = new SceneGraph();

            var carHullTexture = Content.Load<Texture2D>("Textures/car-hull");
            var carHullSprite = new Sprite(carHullTexture);
            var carWheelTexture = Content.Load<Texture2D>("Textures/car-wheel");
            var carWheelSprite = new Sprite(carWheelTexture);

            _carNode = new SceneNode("car-hull", viewportAdapter.Center.ToVector2());
            _carNode.Entities.Add(carHullSprite);

            _leftWheelNode = new SceneNode("left-wheel", new Vector2(-29, 17));
            _leftWheelNode.Entities.Add(carWheelSprite);

            _rightWheelNode = new SceneNode("right-wheel", new Vector2(40, 17));
            _rightWheelNode.Entities.Add(carWheelSprite);

            _carNode.Children.Add(_rightWheelNode);
            _carNode.Children.Add(_leftWheelNode);
            _sceneGraph.RootNode.Children.Add(_carNode);
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.W))
                _speed += deltaTime * 0.5f;

            if (keyboardState.IsKeyDown(Keys.S))
                _speed -= deltaTime * 0.5f;

            _leftWheelNode.Rotation += _speed;
            _rightWheelNode.Rotation = _leftWheelNode.Rotation;
            _carNode.Position += new Vector2(_speed * 5, 0);

            // quick and dirty collision detection
            const int maxX = 535;
            if (_carNode.Position.X >= maxX)
            {
                _speed = -_speed * 0.2f;
                _carNode.Position = new Vector2(maxX, _carNode.Position.Y);
            }

            const int minX = 265;
            if (_carNode.Position.X <= minX)
            {
                _speed = -_speed * 0.2f;
                _carNode.Position = new Vector2(minX, _carNode.Position.Y);
            }

            var worldPosition = _camera.ScreenToWorld(mouseState.X, mouseState.Y);
            _hoveredNode = _sceneGraph.GetSceneNodeAt(worldPosition);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sceneGraph);
            _spriteBatch.FillRectangle(0, 266, 800, 240, Color.DarkOliveGreen);
            _spriteBatch.FillRectangle(200, 0, 5, 480, Color.DarkOliveGreen);
            _spriteBatch.FillRectangle(595, 0, 5, 480, Color.DarkOliveGreen);

            if (_hoveredNode != null)
            {
                var boundingRectangle = _hoveredNode.BoundingRectangle;
                _spriteBatch.DrawRectangle(boundingRectangle, Color.Black);
            }

            _spriteBatch.End();

            _spriteBatch.Begin();

            if (_hoveredNode != null)
                _spriteBatch.DrawString(_bitmapFont, _hoveredNode.Name, new Vector2(14, 2), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}