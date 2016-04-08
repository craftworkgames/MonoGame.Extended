using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Shapes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2D : IVertexType, IMovable
    {
        public Vertex2D(Vector2 pos, Vector2 texCoord) {
            Position = pos;
            TextureCoordinate = texCoord;
        }
        public Vector2 Position { get; set; }
        public Vector2 TextureCoordinate { get; set; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2DColor : IVertexType
    {
        public Vector2 Position;
        public Vector2 TextureCoordinate;
        public Color Color;
        public Vertex2DColor(Vector2 pos, Vector2 texCoord, Color color) {
            Position = pos;
            TextureCoordinate = texCoord;
            Color = color;
        }
        public Vertex2DColor(Vertex2D vertex, Color color) {
            Position = vertex.Position;
            TextureCoordinate = vertex.TextureCoordinate;
            Color = color;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                );
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}