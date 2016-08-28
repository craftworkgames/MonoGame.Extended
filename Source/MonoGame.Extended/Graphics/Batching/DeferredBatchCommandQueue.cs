using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class DeferredBatchCommandQueuer<TVertexType, TCommandData> : BatchCommandQueue<TVertexType, TCommandData>
        where TVertexType : struct, IVertexType where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        private readonly BatchDrawCommand<TCommandData>[] _commands;
        private int _commandsCount;
        private readonly int _maximumCommandsCount;
        private readonly GeometryBuffer<TVertexType> _geometryBuffer;
        internal BatchSortMode SortMode;
        private BatchDrawCommand<TCommandData> _currentCommand;

        internal DeferredBatchCommandQueuer(BatchDrawer<TVertexType, TCommandData> batchDrawer, int maximumCommandsCount)
            : base(batchDrawer)
        {
            _commands = new BatchDrawCommand<TCommandData>[maximumCommandsCount];
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

            _currentCommand = new BatchDrawCommand<TCommandData>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _commandsCount; index++)
            {
                BatchDrawer.Draw(ref _commands[index]);
                _commands[index].Data.SetReferencesToNull();
            }

            if (_geometryBuffer.BufferType == GeometryBufferType.Dynamic)
            {
                _geometryBuffer.Clear();
            }

            _commandsCount = 0;
        }

        internal override void EnqueueDrawCommand(ushort startIndex, ushort primitiveCount, uint sortKey, ref TCommandData data)
        {
            if (_currentCommand.CanMergeWith(sortKey, ref data))
            {
                _commands[_commandsCount - 1].PrimitiveCount = _currentCommand.PrimitiveCount += primitiveCount;
                return;
            }

            _currentCommand = new BatchDrawCommand<TCommandData>(startIndex, primitiveCount, sortKey, data);

            if (_commandsCount >= _maximumCommandsCount)
            {
                throw new BatchCommandQueueOverflowException(_maximumCommandsCount);
            }

            _commands[_commandsCount++] = _currentCommand;
        }
    }
}
