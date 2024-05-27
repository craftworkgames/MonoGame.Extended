using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public class GeometryBuilder2D : GeometryBuilder<VertexPositionColorTexture, ushort>
    {
        public GeometryBuilder2D(int maximumVerticesCount, int maximumIndicesCount)
            : base(maximumVerticesCount, maximumIndicesCount)
        {
        }

        public void BuildSprite(int indexOffset, ref Matrix3x2 transformMatrix, Texture2D texture,
            ref Rectangle sourceRectangle,
            Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            var texelLeft = 0f;
            var texelTop = 0f;
            var texelRight = 1f;
            var texelBottom = 1f;

            if (sourceRectangle.Width > 0)
            {
                texelLeft = (float)sourceRectangle.X / texture.Width;
                texelTop = (float)sourceRectangle.Y / texture.Height;
                texelRight = (sourceRectangle.X + sourceRectangle.Width) / (float)texture.Width;
                texelBottom = (sourceRectangle.Y + sourceRectangle.Height) / (float)texture.Height;
            }
            else
            {
                sourceRectangle.Width = texture.Width;
                sourceRectangle.Height = texture.Height;
            }

            var color1 = color ?? Color.White;

            var vertices = Vertices;

            transformMatrix.Transform(0, 0, ref vertices[0].Position);
            vertices[0].Position.Z = depth;
            vertices[0].Color = color1;
            vertices[0].TextureCoordinate.X = texelLeft;
            vertices[0].TextureCoordinate.Y = texelTop;

            transformMatrix.Transform(sourceRectangle.Width, 0, ref vertices[1].Position);
            vertices[1].Position.Z = depth;
            vertices[1].Color = color1;
            vertices[1].TextureCoordinate.X = texelRight;
            vertices[1].TextureCoordinate.Y = texelTop;

            transformMatrix.Transform(0, sourceRectangle.Height, ref vertices[2].Position);
            vertices[2].Position.Z = depth;
            vertices[2].Color = color1;
            vertices[2].TextureCoordinate.X = texelLeft;
            vertices[2].TextureCoordinate.Y = texelBottom;

            transformMatrix.Transform(sourceRectangle.Width, sourceRectangle.Height, ref vertices[3].Position);
            vertices[3].Position.Z = depth;
            vertices[3].Color = color1;
            vertices[3].TextureCoordinate.X = texelRight;
            vertices[3].TextureCoordinate.Y = texelBottom;

            var flipDiagonally = (flags & FlipFlags.FlipDiagonally) != 0;
            var flipHorizontally = (flags & FlipFlags.FlipHorizontally) != 0;
            var flipVertically = (flags & FlipFlags.FlipVertically) != 0;

            if (flipDiagonally)
            {
                FloatHelper.Swap(ref vertices[1].TextureCoordinate.X, ref vertices[2].TextureCoordinate.X);
                FloatHelper.Swap(ref vertices[1].TextureCoordinate.Y, ref vertices[2].TextureCoordinate.Y);
            }

            if (flipHorizontally)
                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref vertices[0].TextureCoordinate.Y, ref vertices[1].TextureCoordinate.Y);
                    FloatHelper.Swap(ref vertices[2].TextureCoordinate.Y, ref vertices[3].TextureCoordinate.Y);
                }
                else
                {
                    FloatHelper.Swap(ref vertices[0].TextureCoordinate.X, ref vertices[1].TextureCoordinate.X);
                    FloatHelper.Swap(ref vertices[2].TextureCoordinate.X, ref vertices[3].TextureCoordinate.X);
                }

            if (flipVertically)
                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref vertices[0].TextureCoordinate.X, ref vertices[2].TextureCoordinate.X);
                    FloatHelper.Swap(ref vertices[1].TextureCoordinate.X, ref vertices[3].TextureCoordinate.X);
                }
                else
                {
                    FloatHelper.Swap(ref vertices[0].TextureCoordinate.Y, ref vertices[2].TextureCoordinate.Y);
                    FloatHelper.Swap(ref vertices[1].TextureCoordinate.Y, ref vertices[3].TextureCoordinate.Y);
                }

            VertexCount = 4;
            AddQuadrilateralIndices(indexOffset);
            IndexCount = 6;
            PrimitivesCount = 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddQuadrilateralIndices(int indexOffset)
        {
            Indices[0] = (ushort)(0 + indexOffset);
            Indices[1] = (ushort)(1 + indexOffset);
            Indices[2] = (ushort)(2 + indexOffset);
            Indices[3] = (ushort)(1 + indexOffset);
            Indices[4] = (ushort)(3 + indexOffset);
            Indices[5] = (ushort)(2 + indexOffset);
        }
    }
}
