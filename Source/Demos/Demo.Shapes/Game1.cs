using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Shapes;

namespace Demo.Shapes
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private ShapeBatch _shapeBatch;

        private Point2F _circlePosition;
        private float _circleRadius;
        private float _circleTheta;

        private Point2F _rectanglePosition;
        private float _rectangleTheta;
        private float _rectangleSize;

        private Texture2D _spriteTexture;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;

            PrimitiveBatchHelper.SortAction = Array.Sort;

            _shapeBatch = new ShapeBatch(graphicsDevice);

            _spriteTexture = Content.Load<Texture2D>("logo-square-128");

            GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var viewport = GraphicsDevice.Viewport;
            var mouseState = Mouse.GetState();

            _circleTheta += MathHelper.ToRadians(1.5f);
            _circleRadius = 70f + 15f * (float)Math.Cos(_circleTheta);
            _circlePosition = new Point2F(mouseState.X, mouseState.Y);

            _rectangleTheta -= MathHelper.ToRadians(-1f);
            _rectangleSize = 100f + 95f * (float)Math.Sin(_rectangleTheta);
            _rectanglePosition = new Point2F(viewport.Width * 0.5f, viewport.Height * 0.5f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _shapeBatch.Begin(BatchMode.Deferred);

            var line = new Line2F(Point2F.Zero, new Point2F(100f, 100f));
            _shapeBatch.DrawLine(ref line, 7, Color.Red);

            //_shapeBatch.DrawCircle(_circlePosition, _circleRadius, Color.Black * 0.25f);
            //            _shapeBatch.DrawCircleOutline(_circlePosition, _circleRadius, Color.Black);

            var arc = new ArcF(_circlePosition, new SizeF(100, 150), 0, _circleTheta);
            _shapeBatch.DrawArc(ref arc, Color.FromNonPremultiplied(39, 139, 39, 255));
            //            _shapeBatch.DrawArcOutline(_circlePosition, _circleRadius, 0, _circleTheta, Color.Red);
            //

            var rectangle = new RectangleF(_rectanglePosition, _rectangleSize);
            _shapeBatch.DrawRectangle(ref rectangle, Color.Red);
            //_shapeBatch.DrawRectangleOutlineOffCenter(_rectanglePosition, _rectangleSize, Color.Black, _rectangleTheta);
            //

            var sprite = new Sprite(new Point2F(200, 200));
            _shapeBatch.DrawSprite(ref sprite, _spriteTexture);

            _shapeBatch.End();

            base.Draw(gameTime);
        }
    }
}
