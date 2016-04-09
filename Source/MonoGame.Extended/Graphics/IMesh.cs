using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface IMesh<TVertexType>
        where TVertexType : struct, IVertexType
    {
    }
}
