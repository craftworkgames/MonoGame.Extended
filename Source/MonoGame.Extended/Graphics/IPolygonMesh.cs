using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public interface IPolygonMesh<TVertexType>
        where TVertexType : struct, IVertexType
    {
        IReadOnlyCollection<TVertexType> Vertices { get; }
        IReadOnlyCollection<short> Indices { get; }
    }
}
