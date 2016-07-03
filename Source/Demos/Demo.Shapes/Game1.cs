using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace Demo.Shapes
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private ShapeBatch _shapeBatch;

        private Vector2 _circlePosition;
        private float _circleRadius;
        private float _circleTheta;

        private Vector2 _rectanglePosition;
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
            _circlePosition = mouseState.Position.ToVector2();

            _rectangleTheta -= MathHelper.ToRadians(-1f);
            _rectangleSize = 100f + 95f * (float)Math.Sin(_rectangleTheta);
            _rectanglePosition = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //TODO: Reduce overhead by using a similiar technique to ShapeBuilder but for PrimitiveBatch; fill a buffer of vertices and then enqueue them in one call. RenderGeometryBuilder

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _shapeBatch.Begin(BatchMode.Deferred);

            _shapeBatch.DrawLine(Vector2.Zero, new Vector2(100, 100), Color.Red, 7);

            //_shapeBatch.DrawCircle(_circlePosition, _circleRadius, Color.Black * 0.25f);
            //            _shapeBatch.DrawCircleOutline(_circlePosition, _circleRadius, Color.Black);
            _shapeBatch.DrawArc(_circlePosition, _circleRadius, 0, _circleTheta, Color.FromNonPremultiplied(39, 139, 39, 255));
            //            _shapeBatch.DrawArcOutline(_circlePosition, _circleRadius, 0, _circleTheta, Color.Red);
            //
            _shapeBatch.DrawRectangleOffCenter(_rectanglePosition, _rectangleSize, Color.Red, _rectangleTheta);
            //_shapeBatch.DrawRectangleOutlineOffCenter(_rectanglePosition, _rectangleSize, Color.Black, _rectangleTheta);
            //

            _shapeBatch.DrawSprite(_spriteTexture, new Vector2(200, 200), rotation: _rectangleTheta);

            _shapeBatch.End();

            base.Draw(gameTime);
        }
    }
}
