using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using SpriteBatch = MonoGame.Extended.Graphics.Batching.SpriteBatch;

namespace Demo.PrimitiveBatch
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private VertexMesh<VertexPositionColor> _vertexMesh; 

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            _spriteBatch = new SpriteBatch(graphicsDevice);
            _texture = Content.Load<Texture2D>("logo-square-128");

            var viewport = graphicsDevice.Viewport;
            var basicEffect = new BasicEffect(graphicsDevice)
            {
                Alpha = 1,
                VertexColorEnabled = true,
                LightingEnabled = false,
                Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1),
                World = Matrix.Identity,
                View  =
                // scale the x and y axis; flip the y-axis for cartesian coordinate system 
                Matrix.CreateScale(new Vector3(100, -100, 1))
                // move the origin from top left to center of the screen
                * Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0))
        };
            var drawContext = new EffectDrawContext<BasicEffect>(basicEffect);

            _primitiveBatch = new PrimitiveBatch<VertexPositionColor>(graphicsDevice, defaultDrawContext: drawContext);
            var vertices = new[]
{
                new VertexPositionColor(new Vector3(0, 0, 0), Color.Red),
                new VertexPositionColor(new Vector3(2, 0, 0), Color.Blue),
                new VertexPositionColor(new Vector3(1, 2, 0), Color.Green),
                new VertexPositionColor(new Vector3(3, 2, 0), Color.White)
            };
            var indices = new short[]
            {
                0,
                1,
                2,
                2,
                1,
                3
            };
            _vertexMesh = new VertexMesh<VertexPositionColor>(PrimitiveType.TriangleList, vertices, indices);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _primitiveBatch.Begin(BatchSortMode.Immediate);
            _primitiveBatch.DrawVertexMesh(_vertexMesh);
            _primitiveBatch.End();

            _spriteBatch.Begin(BatchSortMode.Immediate);
            _spriteBatch.Draw(_texture, Vector3.Zero);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
