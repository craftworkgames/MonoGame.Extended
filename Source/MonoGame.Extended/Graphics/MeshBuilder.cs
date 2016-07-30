using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public delegate void OutputVertexDelegate<TVertexType>(ref TVertexType vertex) where TVertexType : struct, IVertexType;
    public delegate void OutputVertexIndexDelegate(int index);

    public class MeshBuilder<TVertexType> where TVertexType : struct, IVertexType
    {
        public OutputVertexDelegate<TVertexType> OutputVertex { get; }
        public OutputVertexIndexDelegate OutputIndex { get; }

        public MeshBuilder(OutputVertexDelegate<TVertexType> outputVertex, OutputVertexIndexDelegate outputIndex)
        {
            if (outputVertex == null)
            {
                throw new ArgumentNullException(nameof(outputVertex));
            }

            if (outputIndex == null)
            {
                throw new ArgumentNullException(nameof(outputIndex));
            }

            OutputVertex = outputVertex;
            OutputIndex = outputIndex;
        }
    }
}
