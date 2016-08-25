using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Batching.Drawers;

namespace MonoGame.Extended.Graphics.Batching.Queuers
{
    internal class DeferredBatchCommandQueuer<TVertexType, TCommandContext> : BatchCommandQueuer<TVertexType, TCommandContext>
        where TVertexType : struct, IVertexType where TCommandContext : struct, IBatchCommandContext
    {
        private readonly BatchCommand<TCommandContext>[] _commands;
        private int _commandsCount;
        private BatchCommand<TCommandContext> _currentCommand;
        private readonly int _maximumCommandsCount;
        private readonly GeometryBuffer<TVertexType> _geometryBuffer;
        internal BatchSortMode SortMode;

        internal DeferredBatchCommandQueuer(BatchDrawer<TVertexType, TCommandContext> batchDrawer, int maximumCommandsCount)
            : base(batchDrawer)
        {
            _commands = new BatchCommand<TCommandContext>[maximumCommandsCount];
            _geometryBuffer = batchDrawer.GeometryBuffer;
            _maximumCommandsCount = maximumCommandsCount;
        }

        protected internal override void Flush()
        {
            BatchDrawer.SelectBuffers();

            // ReSharper disable once SwitchStatementMissingSomeCases
            // ReSharper disable ImpureMethodCallOnReadonlyValueField

            if (SortMode == BatchSortMode.DeferredSorted)
            {
                Array.Sort(_commands, 0, _commandsCount);
            }
 
            // ReSharper restore ImpureMethodCallOnReadonlyValueField

            for (var index = 0; index < _commandsCount; index++)
            {
                BatchDrawer.Draw(ref _commands[index]);
                _commands[index].Context = default(TCommandContext);
            }

            if (_geometryBuffer.BufferType == GeometryBufferType.Dynamic)
            {
                _geometryBuffer.Clear();
            }

            _commandsCount = 0;
        }

        internal override void EnqueueDraw(ushort startIndex, ushort indexCount, ref TCommandContext context, uint sortKey = 0)
        {
            var primitiveCount = (ushort)PrimitiveType.GetPrimitiveCount(indexCount);
            _currentCommand = new BatchCommand<TCommandContext>(sortKey, context, startIndex, primitiveCount);

            if (_commandsCount >= _maximumCommandsCount)
            {
                throw new BatchCommandQueueOverflowException(_maximumCommandsCount);
            }

            _commands[_commandsCount++] = _currentCommand;
        }
    }
}
