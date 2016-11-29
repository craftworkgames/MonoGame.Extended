using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class BatchCommandDrawer<TVertexType, TIndexType, TCommandData>
        where TVertexType : struct where TIndexType : struct where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;

        internal Effect Effect;

        internal BatchCommandDrawer(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            _graphicsDevice = graphicsDevice;
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
        }

        internal void UploadVertices(TVertexType[] vertices, int startVertex, int vertexCount)
        {
            _vertexBuffer.SetData(vertices, startVertex, vertexCount);
        }

        internal void UploadIndices(TIndexType[] indices, int startIndex, int indexCount)
        {
            _indexBuffer.SetData(indices, startIndex, indexCount);
        }

        internal void SelectVertices()
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        }

        internal void SelectIndices()
        {
            _graphicsDevice.Indices = _indexBuffer;
        }

        internal void Draw(ref BatchDrawCommand<TCommandData> command)
        {
            command.Data.ApplyTo(Effect);
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(command.PrimitiveType, 0, command.StartIndex,
                    command.PrimitiveCount);
            }
            command.Data.SetReferencesToNull();
        }
    }
}