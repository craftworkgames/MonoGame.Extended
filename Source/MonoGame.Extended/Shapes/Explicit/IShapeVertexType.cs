using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Shapes.Explicit
{
    public interface IShapeVertexType<TShapeVertexType> : IVertexType
        where TShapeVertexType : IShapeVertexType<TShapeVertexType>
    {
        void Transform(ref Matrix matrix, out TShapeVertexType result);
    }
}
