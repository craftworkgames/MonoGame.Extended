using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics
{
    public class GeometryBuilder<TVertexType> where TVertexType : struct, IVertexType
    {
        public delegate void OutputVertexDelegate(ref TVertexType vertex);
        public delegate void OutputVertexIndexDelegate(int index);

        public OutputVertexDelegate OutputVertex { get; }
        public OutputVertexIndexDelegate OutputIndex { get; }

        public GeometryBuilder(OutputVertexDelegate outputVertex, OutputVertexIndexDelegate outputIndex)
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
