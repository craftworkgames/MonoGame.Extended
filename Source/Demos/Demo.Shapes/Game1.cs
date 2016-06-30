using System;
using System.Collections.Generic;
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
        private MouseState _mouseState;
        private MouseState _previousMouseState;
        private Vector2 _circlePosition;
        private float _circleRadius;
        private float _circleTheta;

        private readonly List<Vector2> _points = new List<Vector2>();

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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _previousMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            _circlePosition = _mouseState.Position.ToVector2();
            _circleTheta += MathHelper.ToRadians(1.5f);
            _circleRadius = 70f + 12.5f * (float)Math.Cos(_circleTheta);

            if (_mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _points.Add(_mouseState.Position.ToVector2());
            }

            if (_mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                _points.Clear();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _shapeBatch.Begin();

            //TODO: DrawRectangle, DrawSprite, DrawString

            _shapeBatch.DrawPolygonLine(_points, color: Color.White);

            _shapeBatch.DrawCircle(_circlePosition, _circleRadius, Color.Black * 0.5f);
            _shapeBatch.DrawCircleOutline(_circlePosition, _circleRadius, color: Color.Black);
            _shapeBatch.DrawArc(_circlePosition, _circleRadius, 0, _circleTheta, Color.Red * 0.5f);
            _shapeBatch.DrawArcOutline(_circlePosition, _circleRadius, 0, _circleTheta, color: Color.Red);

            _shapeBatch.End();

            base.Draw(gameTime);
        }
    }
}
