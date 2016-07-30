using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Shapes.Explicit
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ShapeVertex2D : IShapeVertexType<ShapeVertex2D>
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static ShapeVertex2D()
        {
            VertexElement[] elements = {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.Normal, 0)
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        public Vector2 Position;
        public Vector2 Normal;

        public ShapeVertex2D(Vector2 position, Vector2 normal)
        {
            Position = position;
            Normal = normal;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        [SuppressMessage(category: "ReSharper", checkId: "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return HashCodeHelper.GetHashCode(Position, Normal);
        }

        public static implicit operator Vector2(ShapeVertex2D vertex)
        {
            return vertex.Position;
        }

        public static implicit operator ShapeVertex2D(Vector2 position)
        {
            return new ShapeVertex2D(position, Vector2.Zero);
        }

        public void Transform(ref Matrix matrix, out ShapeVertex2D result)
        {
            result.Position.X = Position.X * matrix.M11 + Position.Y * matrix.M21 + matrix.M41;
            result.Position.Y = Position.X * matrix.M12 + Position.Y * matrix.M22 + matrix.M42;
            result.Normal.X = Normal.X * matrix.M11 + Normal.Y * matrix.M21;
            result.Normal.Y = Normal.X * matrix.M12 + Normal.Y * matrix.M22;
        }
    }
}
