using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class NinePatchRegion2D : TextureRegion2D
    {
        public Rectangle[] SourcePatches { get; } = new Rectangle[9];
        public Thickness Padding { get; }
        public int LeftPadding => Padding.Left;
        public int TopPadding => Padding.Top;
        public int RightPadding => Padding.Right;
        public int BottomPadding => Padding.Bottom;

        public NinePatchRegion2D(TextureRegion2D textureRegion, Thickness padding)
            : base(textureRegion.Name, textureRegion.Texture, textureRegion.X, textureRegion.Y, textureRegion.Width, textureRegion.Height)
        {
            Padding = padding;
            CachePatches(textureRegion.Bounds, SourcePatches);
        }

        public NinePatchRegion2D(TextureRegion2D textureRegion, int padding)
            : this(textureRegion, padding, padding, padding, padding)
        {
        }

        public NinePatchRegion2D(TextureRegion2D textureRegion, int leftRightPadding, int topBottomPadding)
            : this(textureRegion, leftRightPadding, topBottomPadding, leftRightPadding, topBottomPadding)
        {
        }

        public NinePatchRegion2D(TextureRegion2D textureRegion, int leftPadding, int topPadding, int rightPadding, int bottomPadding)
            : this(textureRegion, new Thickness(leftPadding, topPadding, rightPadding, bottomPadding))
        {
        }

        public NinePatchRegion2D(Texture2D texture, Thickness thickness)
            : this(new TextureRegion2D(texture), thickness)
        {
        }

        public const int TopLeft = 0;
        public const int TopMiddle = 1;
        public const int TopRight = 2;
        public const int MiddleLeft = 3;
        public const int Middle = 4;
        public const int MiddleRight = 5;
        public const int BottomLeft = 6;
        public const int BottomMiddle = 7;
        public const int BottomRight = 8;

        private readonly Rectangle[] _destinationPatches = new Rectangle[9];

        public Rectangle[] CreatePatches(Rectangle rectangle)
        {
            CachePatches(rectangle, _destinationPatches);
            return _destinationPatches;
        }

        private void CachePatches(Rectangle sourceRectangle, Rectangle[] patchCache)
        {
            var x = sourceRectangle.X;
            var y = sourceRectangle.Y;
            var w = sourceRectangle.Width;
            var h = sourceRectangle.Height;
            var middleWidth = w - LeftPadding - RightPadding;
            var middleHeight = h - TopPadding - BottomPadding;
            var bottomY = y + h - BottomPadding;
            var rightX = x + w - RightPadding;
            var leftX = x + LeftPadding;
            var topY = y + TopPadding;

            patchCache[TopLeft] = new Rectangle(x, y, LeftPadding, TopPadding);
            patchCache[TopMiddle] = new Rectangle(leftX, y, middleWidth, TopPadding);
            patchCache[TopRight] = new Rectangle(rightX, y, RightPadding, TopPadding);
            patchCache[MiddleLeft] = new Rectangle(x, topY, LeftPadding, middleHeight);
            patchCache[Middle] = new Rectangle(leftX, topY, middleWidth, middleHeight);
            patchCache[MiddleRight] = new Rectangle(rightX, topY, RightPadding, middleHeight);
            patchCache[BottomLeft] = new Rectangle(x, bottomY, LeftPadding, BottomPadding);
            patchCache[BottomMiddle] = new Rectangle(leftX, bottomY, middleWidth, BottomPadding);
            patchCache[BottomRight] = new Rectangle(rightX, bottomY, RightPadding, BottomPadding);
        }
    }
}