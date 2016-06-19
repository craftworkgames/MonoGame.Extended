using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal abstract class BatchDrawer<TVertexType> : IDisposable
        where TVertexType : struct, IVertexType
    {
        internal GraphicsDevice GraphicsDevice;
        internal readonly ushort MaximumVerticesCount;
        internal readonly ushort MaximumIndicesCount;
        internal List<Action> CommandDelegates;
        private IDrawContext _currentDrawContext;
        protected Effect Effect;

        protected BatchDrawer(GraphicsDevice graphicsDevice, ushort maximumVerticesCount = PrimitiveBatch<TVertexType>.DefaultMaximumVerticesCount, ushort maximumIndicesCount = PrimitiveBatch<TVertexType>.DefaultMaximumIndicesCount)
        {
            GraphicsDevice = graphicsDevice;
            MaximumVerticesCount = maximumVerticesCount;
            MaximumIndicesCount = maximumIndicesCount;

            CommandDelegates = new List<Action>();
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

            GraphicsDevice = null;
        }

        internal abstract void Select(TVertexType[] vertices);
        internal abstract void Select(TVertexType[] vertices, short[] indices);
        internal abstract void Draw(IDrawContext drawContext, PrimitiveType primitiveType, int startVertex, int vertexCount);
        internal abstract void Draw(IDrawContext drawContext, PrimitiveType primitiveType, int startVertex, int vertexCount, int startIndex, int indexCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ChangeDrawContextIfNecessary(IDrawContext drawContext)
        {
            if (_currentDrawContext == drawContext && !drawContext.NeedsToApplyChanges)
            {
                return;
            }

            drawContext.Apply(out Effect);
            _currentDrawContext = drawContext;
        }
    }
}
