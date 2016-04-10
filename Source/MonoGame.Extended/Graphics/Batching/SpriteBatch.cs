using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class SpriteBatch
    {
        private readonly PrimitiveBatch<VertexPositionColorTexture> _primitiveBatch;
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
            Vector2 textureCoordinateTopLeft;
            Vector2 textureCoordinateBottomRight;
            if (sourceRectangle.HasValue)
            {
                var rectangle = sourceRectangle.Value;
                width = rectangle.Width;
                height = rectangle.Height;
                textureCoordinateTopLeft.X = rectangle.X / (float)texture.Width;
                textureCoordinateTopLeft.Y = rectangle.Y / (float)texture.Height;
                textureCoordinateBottomRight.X = (rectangle.X + rectangle.Width) / (float)texture.Width;
                textureCoordinateBottomRight.Y = (rectangle.Y + rectangle.Height) / (float)texture.Height;
            }
            else
            {
                textureCoordinateTopLeft.X = 0;
                textureCoordinateTopLeft.Y = 0;
                textureCoordinateBottomRight.X = 1;
                textureCoordinateBottomRight.Y = 1;
                width = texture.Width;
                height = texture.Height;
            }

            if ((spriteEffects & SpriteEffects.FlipVertically) != 0)
            {
                var temp = textureCoordinateBottomRight.Y;
                textureCoordinateBottomRight.Y = textureCoordinateTopLeft.Y;
                textureCoordinateTopLeft.Y = temp;
            }
            if ((spriteEffects & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = textureCoordinateBottomRight.X;
                textureCoordinateBottomRight.X = textureCoordinateTopLeft.X;
                textureCoordinateTopLeft.X = temp;
            }

            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);
            // top left
            _spriteItemVertices[0] = new VertexPositionColorTexture(new Vector3(position.X + -origin.X * cos - -origin.Y * sin, position.Y + -origin.X * sin + -origin.Y * cos, position.Z), color, textureCoordinateTopLeft);
            // top right
            _spriteItemVertices[1] = new VertexPositionColorTexture(new Vector3(position.X + (-origin.X + width) * cos - -origin.Y * sin, position.Y + (-origin.X + width) * sin + -origin.Y * cos, position.Z), color, new Vector2(textureCoordinateBottomRight.X, textureCoordinateTopLeft.Y));
            // bottom left
            _spriteItemVertices[2] = new VertexPositionColorTexture(new Vector3(position.X + -origin.X * cos - (-origin.Y + height) * sin, position.Y + -origin.X * sin + (-origin.Y + height) * cos, position.Z), color, new Vector2(textureCoordinateTopLeft.X, textureCoordinateBottomRight.Y));
            // bottom right
            _spriteItemVertices[3] = new VertexPositionColorTexture(new Vector3(position.X + (-origin.X + width) * cos - (-origin.Y + height) * sin, position.Y + (-origin.X + width) * sin + (-origin.Y + height) * cos, position.Z), color, textureCoordinateBottomRight);

            _primitiveBatch.Draw(PrimitiveType.TriangleList, _spriteItemVertices, _spriteItemIndices, drawContext);
        }

        public void End()
        {
            _primitiveBatch.End();
        }
    }
}
