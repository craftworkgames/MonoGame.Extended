using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Geometry
{
    public class GraphicsGeometryBufferPositionColorTextureU16 : GraphicsGeometryBuffer<VertexPositionColorTexture, ushort>
    {
        public GraphicsGeometryBufferPositionColorTextureU16(GraphicsDevice graphicsDevice, int maximumVertices, int maximumIndices, bool isDynamic) 
            : base(graphicsDevice, maximumVertices, maximumIndices, isDynamic)
        {
        }

        protected override unsafe void CopyIndices(ushort[] source, int sourceStartIndex, ushort[] destination, int desintationStartIndex, int length, int offset)
        {
            fixed (ushort* fixedDestinationPointer = destination)
            fixed (ushort* fixedSourcePointer = source)
            {
                var pointerDestination = fixedDestinationPointer + desintationStartIndex;
                var pointerSource = fixedSourcePointer + sourceStartIndex;
                var pointerDestinationEnd = pointerDestination + length;

                while (pointerDestination < pointerDestinationEnd)
                {
                    *pointerDestination++ = (ushort)(*pointerSource++ + offset);
                }
            }
        }
    }
}
