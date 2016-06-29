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

        private ShapeBatch _batch;

        private float _rotationTheta;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;

            var viewport = graphicsDevice.Viewport;

            PrimitiveBatchHelper.SortAction = Array.Sort;

            _batch = new ShapeBatch(graphicsDevice);

            _batch.Effect.Projection = Matrix.CreateTranslation(xPosition: -0.5f, yPosition: -0.5f, zPosition: 0) * Matrix.CreateOrthographicOffCenter(left: 0, right: viewport.Width, bottom: viewport.Height, top: 0, zNearPlane: 0, zFarPlane: -1);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _rotationTheta += MathHelper.ToRadians(degrees: 1);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            var m = Matrix.CreateScale(1);
            _batch.Begin(BatchMode.Deferred, ref m);

//            var v1 = new VertexPositionColor(new Vector3(new Vector2(0, 0), 0), Color.AliceBlue);
//            var v2 = new VertexPositionColor(new Vector3(new Vector2(0, 100), 0), Color.AntiqueWhite);
//
//            _primitiveBatch.DrawLine(_simpleEffect, ref v1, ref v2);
//
//            var v3 = new VertexPositionColor(new Vector3(new Vector2(0, 0), 0), Color.Red);
//            var v4 = new VertexPositionColor(new Vector3(new Vector2(0, -100), 0), Color.DarkRed);
//
//            _primitiveBatch.DrawLine(_simpleEffect, ref v3, ref v4);

            //_batch.DrawCircle(_simpleEffect, new Vector2(0, 1), 1f, Color.Black * 0.5f);

            //var axis = new Vector2(x: (float)Math.Cos(_rotationTheta), y: (float)Math.Sin(_rotationTheta));

            _batch.DrawCircleOutline(new Vector2(250, 250), radius: 50f, color: Color.Black, circleSegments: 64);

            _batch.End();

            base.Draw(gameTime);
        }
    }
}
