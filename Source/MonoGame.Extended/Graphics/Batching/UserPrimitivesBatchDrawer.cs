using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    public class UserPrimitivesBatcher<TVertexType> : IBatcher<TVertexType>
        where TVertexType : struct, IVertexType
    {
        private GraphicsDevice _graphicsDevice;
        private IDrawContext _defaultDrawContext;
        private TVertexType[] _vertices;
        private short[] _indices;

        public int MaximumBatchSize { get; } = 8192;

        public UserPrimitivesBatcher(GraphicsDevice graphicsDevice, IDrawContext defaultDrawContext = null)
        {
            _graphicsDevice = graphicsDevice;

            if (defaultDrawContext == null)
            {
                var basicEffect = new BasicEffect(graphicsDevice);
                _defaultDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);
            }
            else
            {
                _defaultDrawContext = defaultDrawContext;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            _graphicsDevice = null;
            _defaultDrawContext = null;
            _vertices = null;
            _indices = null;
        }

        public void Select(TVertexType[] vertices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            _vertices = vertices;
        }

        public void Select(TVertexType[] vertices, short[] indices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            if (indices == null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            _vertices = vertices;
            _indices = indices;
        }

        public void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, IDrawContext drawContext)
        {
            if (drawContext == null)
            {
                drawContext = _defaultDrawContext;
            }

            var primitiveCount = primitiveType.GetPrimitiveCount(vertexCount);

            var passesCount = drawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                drawContext.ApplyPass(passIndex);
                _graphicsDevice.DrawUserPrimitives(primitiveType, _vertices, startVertex, primitiveCount);
            }
        }

        public void Draw(PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount, IDrawContext drawContext)
        {
            if (drawContext == null)
            {
                drawContext = _defaultDrawContext;
            }

            var primitiveCount = primitiveType.GetPrimitiveCount(indexCount);

            var passesCount = drawContext.PassesCount;
            for (var passIndex = 0; passIndex < passesCount; ++passIndex)
            {
                drawContext.ApplyPass(passIndex);

                _graphicsDevice.DrawUserIndexedPrimitives(primitiveType, _vertices, startVertex, vertexCount, _indices, startIndex, primitiveCount);
            }
        }
    }
}
