using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class SpriteBatch
    {
        private readonly PrimitiveBatch<VertexPositionColorTexture> _primitiveBatch;
        private Vector2 _spriteItemTextureCoordinateTopLeft = new Vector2(0, 0);
        private Vector2 _spriteItemTextureCoordinateBottomRight = new Vector2(0, 0);
        private readonly VertexPositionColorTexture[] _spriteItemVertices = new VertexPositionColorTexture[3];

        private readonly short[] _spriteItemIndices = {
            0,
            1,
            2,
            1,
            3,
            2
        };

        public SpriteBatch(GraphicsDevice graphicsDevice, BatchDrawStrategy batchDrawStrategy = BatchDrawStrategy.UserPrimitives, IDrawContext drawContext = null, int maxmimumBatchSize = PrimitiveBatch<VertexPositionColor>.DefaultMaximumBatchSize)
        {
            _primitiveBatch = new PrimitiveBatch<VertexPositionColorTexture>(graphicsDevice, batchDrawStrategy, drawContext, maxmimumBatchSize);
        }

        public void Begin(BatchSortMode batchSortMode)
        {
            _primitiveBatch.Begin(batchSortMode);
        }

        public void Draw(Texture2D texture, Vector3 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects = SpriteEffects.None, IDrawContext drawContext = null)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            int width;
            int height;
            if (sourceRectangle.HasValue)
            {
                var rectangle = sourceRectangle.Value;
                width = rectangle.Width;
                height = rectangle.Height;
                _spriteItemTextureCoordinateTopLeft.X = rectangle.X / (float)texture.Width;
                _spriteItemTextureCoordinateTopLeft.Y = rectangle.Y / (float)texture.Height;
                _spriteItemTextureCoordinateBottomRight.X = (rectangle.X + rectangle.Width) / (float)texture.Width;
                _spriteItemTextureCoordinateBottomRight.Y = (rectangle.Y + rectangle.Height) / (float)texture.Height;
            }
            else
            {
                _spriteItemTextureCoordinateTopLeft.X = 0;
                _spriteItemTextureCoordinateTopLeft.Y = 0;
                _spriteItemTextureCoordinateBottomRight.X = 1;
                _spriteItemTextureCoordinateBottomRight.Y = 1;
                width = texture.Width;
                height = texture.Height;
            }

            if ((spriteEffects & SpriteEffects.FlipVertically) != 0)
            {
                var temp = _spriteItemTextureCoordinateBottomRight.Y;
                _spriteItemTextureCoordinateBottomRight.Y = _spriteItemTextureCoordinateTopLeft.Y;
                _spriteItemTextureCoordinateTopLeft.Y = temp;
            }
            if ((spriteEffects & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = _spriteItemTextureCoordinateBottomRight.X;
                _spriteItemTextureCoordinateBottomRight.X = _spriteItemTextureCoordinateTopLeft.X;
                _spriteItemTextureCoordinateTopLeft.X = temp;
            }

            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);
            // top left
            _spriteItemVertices[0] = new VertexPositionColorTexture(new Vector3(position.X + -origin.X * cos - -origin.Y * sin, position.Y + -origin.X * sin + -origin.Y * cos, position.Z), color, _spriteItemTextureCoordinateTopLeft);
            // top right
            _spriteItemVertices[1] = new VertexPositionColorTexture(new Vector3(position.X + (-origin.X + width) * cos - -origin.Y * sin, position.Y + (-origin.X + width) * sin + -origin.Y * cos, position.Z), color, new Vector2(_spriteItemTextureCoordinateBottomRight.X, _spriteItemTextureCoordinateTopLeft.Y));
            // bottom left
            _spriteItemVertices[2] = new VertexPositionColorTexture(new Vector3(position.X + -origin.X * cos - (-origin.Y + height) * sin, position.Y + -origin.X * sin + (-origin.Y + height) * cos, position.Z), color, new Vector2(_spriteItemTextureCoordinateTopLeft.X, _spriteItemTextureCoordinateBottomRight.Y));
            // bottom right
            _spriteItemVertices[3] = new VertexPositionColorTexture(new Vector3(position.X + (-origin.X + width) * cos - (-origin.Y + height) * sin, position.Y + (-origin.X + width) * sin + (-origin.Y + height) * cos, position.Z), color, _spriteItemTextureCoordinateBottomRight);

            _primitiveBatch.Draw(PrimitiveType.TriangleList, _spriteItemVertices, _spriteItemIndices, drawContext);
        }

        public void End()
        {
            _primitiveBatch.End();
        }
    }
}
