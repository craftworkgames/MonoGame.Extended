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

        private Random _random;
        private Vector2 _circlePosition;
        private float _circleRadius;
        private float _circleTheta;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;

            _random = new Random();
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;

            PrimitiveBatchHelper.SortAction = Array.Sort;

            _shapeBatch = new ShapeBatch(graphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var mouseState = Mouse.GetState();

            _circlePosition = mouseState.Position.ToVector2();
            _circleTheta += MathHelper.ToRadians(1.5f);
            _circleRadius = 80f + 40f * (float)Math.Cos(_circleTheta);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _shapeBatch.Begin();

//            var v1 = new VertexPositionColor(new Vector3(new Vector2(0, 0), 0), Color.AliceBlue);
//            var v2 = new VertexPositionColor(new Vector3(new Vector2(0, 100), 0), Color.AntiqueWhite);
//
//            _primitiveBatch.DrawLine(_simpleEffect, ref v1, ref v2);
//
//            var v3 = new VertexPositionColor(new Vector3(new Vector2(0, 0), 0), Color.Red);
//            var v4 = new VertexPositionColor(new Vector3(new Vector2(0, -100), 0), Color.DarkRed);
//
//            _primitiveBatch.DrawLine(_simpleEffect, ref v3, ref v4);

            _shapeBatch.DrawCircle(_circlePosition, _circleRadius, Color.Black * 0.5f);

            //var axis = new Vector2(x: (float)Math.Cos(_rotationTheta), y: (float)Math.Sin(_rotationTheta));

            _shapeBatch.DrawCircleOutline(_circlePosition, _circleRadius, color: Color.Black);

            _shapeBatch.DrawArcOutline(_circlePosition, _circleRadius, 0, _circleTheta, color: Color.Red);

            _shapeBatch.DrawArc(_circlePosition, _circleRadius, 0, _circleTheta, Color.Red * 0.5f);

            _shapeBatch.End();

            base.Draw(gameTime);
        }
    }
}
