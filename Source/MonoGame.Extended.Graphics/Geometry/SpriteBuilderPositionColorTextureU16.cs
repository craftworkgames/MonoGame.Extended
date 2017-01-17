using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public class SpriteBuilderPositionColorTextureU16 : GraphicsGeometryBuilder<VertexPositionColorTexture, ushort>
    {
        public SpriteBuilderPositionColorTextureU16()
            : base(PrimitiveType.TriangleList, 4, 6)
        {
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 1;
            Indices[4] = 3;
            Indices[5] = 2;
        }

        public unsafe void BuildSprite(ref Matrix2D transformMatrix, Texture2D texture, ref Rectangle sourceRectangle, Color? color = null, FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            var texelLeft = (sourceRectangle.X + 0.5f) / texture.Width;
            var texelTop = (sourceRectangle.Y + 0.5f) / texture.Height;
            var texelRight = (sourceRectangle.X + sourceRectangle.Width) / (float)texture.Width;
            var texelBottom = (sourceRectangle.Y + sourceRectangle.Height) / (float)texture.Height;

            var color1 = color ?? Color.White;

            fixed (VertexPositionColorTexture* fixedPointer = Vertices)
            {
                var pointer = fixedPointer;

                transformMatrix.Transform(0, 0, ref pointer[0].Position);
                pointer[0].Position.Z = depth;
                transformMatrix.Transform(sourceRectangle.Width, 0, ref pointer[1].Position);
                pointer[1].Position.Z = depth;
                transformMatrix.Transform(0, sourceRectangle.Height, ref pointer[2].Position);
                pointer[2].Position.Z = depth;
                transformMatrix.Transform(sourceRectangle.Width, sourceRectangle.Height, ref pointer[3].Position);
                pointer[3].Position.Z = depth;

                pointer[0].Color = color1;
                pointer[1].Color = color1;
                pointer[2].Color = color1;
                pointer[3].Color = color1;

                pointer[0].TextureCoordinate.X = texelLeft;
                pointer[0].TextureCoordinate.Y = texelTop;

                pointer[1].TextureCoordinate.X = texelRight;
                pointer[1].TextureCoordinate.Y = texelTop;

                pointer[2].TextureCoordinate.X = texelLeft;
                pointer[2].TextureCoordinate.Y = texelBottom;

                pointer[3].TextureCoordinate.X = texelRight;
                pointer[3].TextureCoordinate.Y = texelBottom;

                var flipDiagonally = (flags & FlipFlags.FlipDiagonally) != 0;
                var flipHorizontally = (flags & FlipFlags.FlipHorizontally) != 0;
                var flipVertically = (flags & FlipFlags.FlipVertically) != 0;

                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref pointer[1].TextureCoordinate.X, ref pointer[2].TextureCoordinate.X);
                    FloatHelper.Swap(ref pointer[1].TextureCoordinate.Y, ref pointer[2].TextureCoordinate.Y);
                }

                if (flipHorizontally)
                {
                    if (flipDiagonally)
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.Y, ref pointer[1].TextureCoordinate.Y);
                        FloatHelper.Swap(ref pointer[2].TextureCoordinate.Y, ref pointer[3].TextureCoordinate.Y);
                    }
                    else
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.X, ref pointer[1].TextureCoordinate.X);
                        FloatHelper.Swap(ref pointer[2].TextureCoordinate.X, ref pointer[3].TextureCoordinate.X);
                    }
                }

                if (flipVertically)
                {
                    if (flipDiagonally)
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.X, ref pointer[2].TextureCoordinate.X);
                        FloatHelper.Swap(ref pointer[1].TextureCoordinate.X, ref pointer[3].TextureCoordinate.X);
                    }
                    else
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.Y, ref pointer[2].TextureCoordinate.Y);
                        FloatHelper.Swap(ref pointer[1].TextureCoordinate.Y, ref pointer[3].TextureCoordinate.Y);
                    }
                }
            }
        }

        public unsafe void BuildSprite(ref Matrix2D transformMatrix, Texture2D texture, Color? color, FlipFlags flags = FlipFlags.None, float depth = 0)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            const int texelLeft = 0;
            const int texelTop = 0;
            const int texelRight = 1;
            const int texelBottom = 1;

            var color1 = color ?? Color.White;

            fixed (VertexPositionColorTexture* fixedPointer = Vertices)
            {
                var pointer = fixedPointer;

                transformMatrix.Transform(0, 0, ref pointer[0].Position);
                pointer[0].Position.Z = depth;
                transformMatrix.Transform(texture.Width, 0, ref pointer[1].Position);
                pointer[1].Position.Z = depth;
                transformMatrix.Transform(0, texture.Height, ref pointer[2].Position);
                pointer[2].Position.Z = depth;
                transformMatrix.Transform(texture.Width, texture.Height, ref pointer[3].Position);
                pointer[3].Position.Z = depth;

                pointer[0].Color = color1;
                pointer[1].Color = color1;
                pointer[2].Color = color1;
                pointer[3].Color = color1;

                pointer[0].TextureCoordinate.X = texelLeft;
                pointer[0].TextureCoordinate.Y = texelTop;

                pointer[1].TextureCoordinate.X = texelRight;
                pointer[1].TextureCoordinate.Y = texelTop;

                pointer[2].TextureCoordinate.X = texelLeft;
                pointer[2].TextureCoordinate.Y = texelBottom;

                pointer[3].TextureCoordinate.X = texelRight;
                pointer[3].TextureCoordinate.Y = texelBottom;

                var flipDiagonally = (flags & FlipFlags.FlipDiagonally) != 0;
                var flipHorizontally = (flags & FlipFlags.FlipHorizontally) != 0;
                var flipVertically = (flags & FlipFlags.FlipVertically) != 0;

                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref pointer[1].TextureCoordinate.X, ref pointer[2].TextureCoordinate.X);
                    FloatHelper.Swap(ref pointer[1].TextureCoordinate.Y, ref pointer[2].TextureCoordinate.Y);
                }

                if (flipHorizontally)
                {
                    if (flipDiagonally)
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.Y, ref pointer[1].TextureCoordinate.Y);
                        FloatHelper.Swap(ref pointer[2].TextureCoordinate.Y, ref pointer[3].TextureCoordinate.Y);
                    }
                    else
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.X, ref pointer[1].TextureCoordinate.X);
                        FloatHelper.Swap(ref pointer[2].TextureCoordinate.X, ref pointer[3].TextureCoordinate.X);
                    }
                }

                if (flipVertically)
                {
                    if (flipDiagonally)
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.X, ref pointer[2].TextureCoordinate.X);
                        FloatHelper.Swap(ref pointer[1].TextureCoordinate.X, ref pointer[3].TextureCoordinate.X);
                    }
                    else
                    {
                        FloatHelper.Swap(ref pointer[0].TextureCoordinate.Y, ref pointer[2].TextureCoordinate.Y);
                        FloatHelper.Swap(ref pointer[1].TextureCoordinate.Y, ref pointer[3].TextureCoordinate.Y);
                    }
                }
            }
        }
    }
}
