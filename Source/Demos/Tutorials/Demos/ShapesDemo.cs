using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Tutorials.Demos
{
    public class ShapesDemo : DemoBase
    {
        private SpriteBatch _spriteBatch;
        
        public ShapesDemo(GameMain game) 
            : base(game)
        {
        }

        public override string Name => "Shapes";

        private readonly Polygon _polygon = new Polygon(new[]
        {
            new Vector2(0, 0),
            new Vector2(40, 0),
            new Vector2(40, 40),
            new Vector2(60, 40),
            new Vector2(60, 60),
            new Vector2(0, 60),
        });

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            _spriteBatch.FillRectangle(10, 10, 50, 50, Color.DarkRed);
            _spriteBatch.DrawRectangle(10, 10, 50, 50, Color.Red, 2);

            _spriteBatch.DrawCircle(100, 100, 32, 32, Color.Green, 4);
            _spriteBatch.DrawEllipse(new Vector2(200, 200), new Vector2(50, 25), 32, Color.Orange, 8);

            _spriteBatch.DrawLine(300, 300, 400, 100, Color.White, 6);

            _spriteBatch.DrawPoint(500, 300, Color.Brown, 64);

            _spriteBatch.DrawPolygon(new Vector2(600, 200), _polygon, Color.Aqua);

            _spriteBatch.End();
        }
    }
}