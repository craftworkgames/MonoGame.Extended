using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace Demo.Shapes
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private ShapeBatch _shapeBatch;

        private CircleF _circle;
        private float _circleGrowTheta;
        private ArcF _arc;
        private RectangleF _rectangle;
        private float _rectangleGrowTheta;

        private Sprite _sprite;

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

            _sprite = new Sprite(Content.Load<Texture2D>("logo-square-128"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var viewport = GraphicsDevice.Viewport;

            _arc.Centre = new Vector2(200, 300);
            _arc.Radius = new SizeF(100, 100);
            _arc.StartAngle = 0;
            _arc.EndAngle += MathHelper.ToRadians(1.5f);

            _circleGrowTheta += MathHelper.ToRadians(2f);
            _circle.Radius = 70f + 15f * (float)Math.Cos(_circleGrowTheta);
            _circle.Centre = new Vector2(400, 100);

            _rectangleGrowTheta -= MathHelper.ToRadians(-1f);
            _rectangle.Size = 100f + 95f * (float)Math.Sin(_rectangleGrowTheta);
            _rectangle.Position = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);

            _sprite.Position = new Vector2(200, 200);
            _sprite.Rotation += MathHelper.ToRadians(1);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _shapeBatch.Begin(BatchMode.Deferred);

            _shapeBatch.Draw(ref _circle, Color.Black * 0.25f);
            _shapeBatch.DrawOutline(ref _circle, Color.Black);

            _shapeBatch.Draw(ref _arc, 0xFF57A64A.ToColor());
            _shapeBatch.DrawOutline(ref _arc, 0xFF265E4D.ToColor());

            _shapeBatch.Draw(ref _rectangle, 0xFFD85050.ToColor());
            _shapeBatch.DrawOutline(ref _rectangle, 0xFF3C0000.ToColor());

            _shapeBatch.Draw(_sprite);

            _shapeBatch.End();

            base.Draw(gameTime);
        }
    }
}
