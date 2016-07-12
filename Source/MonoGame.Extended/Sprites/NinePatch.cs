using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class NinePatch
    {
        public NinePatch(TextureRegion2D textureRegion, int leftPadding, int topPadding, int rightPadding, int bottomPadding)
        {
            TextureRegion = textureRegion;
            LeftPadding = leftPadding;
            TopPadding = topPadding;
            RightPadding = rightPadding;
            BottomPadding = bottomPadding;
            Color = Color.White;

            _sourcePatches = CreatePatches(textureRegion.Bounds);
        }

        private readonly Rectangle[] _sourcePatches;

        public TextureRegion2D TextureRegion { get; }
        public int LeftPadding { get; }
        public int TopPadding { get; }
        public int RightPadding { get; }
        public int BottomPadding { get; }
        public Color Color { get; set; }

        private Rectangle[] CreatePatches(Rectangle rectangle)
        {
            var x = rectangle.X;
            var y = rectangle.Y;
            var w = rectangle.Width;
            var h = rectangle.Height;
            var middleWidth = w - LeftPadding - RightPadding;
            var middleHeight = h - TopPadding - BottomPadding;
            var bottomY = y + h - BottomPadding;
            var rightX = x + w - RightPadding;
            var leftX = x + LeftPadding;
            var topY = y + TopPadding;
            var patches = new[]
            {
                new Rectangle(x,      y,        LeftPadding,  TopPadding),      // top left
                new Rectangle(leftX,  y,        middleWidth,  TopPadding),      // top middle
                new Rectangle(rightX, y,        RightPadding, TopPadding),      // top right
                new Rectangle(x,      topY,     LeftPadding,  middleHeight),    // left middle
                new Rectangle(leftX,  topY,     middleWidth,  middleHeight),    // middle
                new Rectangle(rightX, topY,     RightPadding, middleHeight),    // right middle
                new Rectangle(x,      bottomY,  LeftPadding,  BottomPadding),   // bottom left
                new Rectangle(leftX,  bottomY,  middleWidth,  BottomPadding),   // bottom middle
                new Rectangle(rightX, bottomY,  RightPadding, BottomPadding)    // bottom right
            };
            return patches;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            var destinationPatches = CreatePatches(rectangle);

            for (var i = 0; i < _sourcePatches.Length; i++)
            {
                spriteBatch.Draw(TextureRegion.Texture, sourceRectangle: _sourcePatches[i],
                    destinationRectangle: destinationPatches[i], color: Color);
            }
        }
    }
}