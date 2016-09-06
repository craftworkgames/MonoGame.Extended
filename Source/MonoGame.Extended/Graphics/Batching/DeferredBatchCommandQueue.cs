using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class DeferredBatchCommandQueue<TVertexType, TIndexType, TCommandData> :
            BatchCommandQueue<TVertexType, TIndexType, TCommandData>
        where TVertexType : struct, IVertexType
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
        where TIndexType : struct
    {
        private readonly BatchDrawCommand<TCommandData>[] _commands;
        private readonly GeometryBuffer<TVertexType, TIndexType> _geometryBuffer;
        private readonly ushort[] _sortedIndices;
        private readonly int _maximumCommandsCount;
        private int _commandsCount;
        private BatchDrawCommand<TCommandData> _currentCommand;
        internal BatchSortMode SortMode;

        internal DeferredBatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TIndexType, TCommandData> commandDrawer, int maximumCommandsCount)
            : base(graphicsDevice, commandDrawer)
        {
            _commands = new BatchDrawCommand<TCommandData>[maximumCommandsCount];
            _geometryBuffer = commandDrawer.GeometryBuffer;
            _sortedIndices = new ushort[_geometryBuffer.Indices.Length];
            _maximumCommandsCount = maximumCommandsCount;
        }

        protected internal override void Flush()
        {
            // quit early if there are no deferred commands to process
            if (_commandsCount < 0)
                return;

            var vertexBuffer = _geometryBuffer.VertexBuffer;
            // upload the vertices to the GPU's memory from CPU's memory
            vertexBuffer.SetData(_geometryBuffer.Vertices, 0, _geometryBuffer.VertexCount);
            // tell the graphics API we want to use that buffer of vertices for rendering
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            // conditional code depending on deferred mode
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (SortMode)
            {
                case BatchSortMode.Deferred:
                    FlushDeferred();
                    break;
                case BatchSortMode.DeferredSorted:
                    FlushDeferredSorted();
                    break;
            }

            // prevent a highly unlikely, but possible, memory leak where the current command would be holding onto some reference(s)
            _currentCommand.Data.SetReferencesToNull();

            // clear the geometry buffer if it is dynamic
            if (_geometryBuffer.BufferType == GeometryBufferType.Dynamic)
                _geometryBuffer.Clear();

            // reset the commands count
            // we don't clear the array of commands because we will just overwrite the values anyways
            _commandsCount = 0;
        }

        private void FlushDeferred()
        {
            var indexBuffer = _geometryBuffer.IndexBuffer;
            // upload the indices to the GPU's memory from CPU's memory
            indexBuffer.SetData(_geometryBuffer.Indices, 0, _geometryBuffer.IndexCount);
            // tell the graphics API we want to use that buffer of indices for rendering
            GraphicsDevice.Indices = indexBuffer;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _commandsCount; index++)
                CommandDrawer.Draw(ref _commands[index]);
        }

        private void FlushDeferredSorted()
        {
            // sort the commands
            Array.Sort(_commands, 0, _commandsCount);

            _currentCommand = _commands[0];
            ushort sortedIndiesCount = 0;
            var newCommandsCount = 1;

            // change the indices used by the command to use the sorted indices array so the indices are sequential for the graphics API
            _commands[0].StartIndex = 0;
            // get the number of vertices used for the current command, each index represents a vertex
            var commandIndicesCount = PrimitiveType.GetVerticesCount(_currentCommand.PrimitiveCount);
            // copy the indices of the current command into our sorted indices array so the indices are sequential for the graphics API
            Array.Copy(_geometryBuffer.Indices, _currentCommand.StartIndex, _sortedIndices, sortedIndiesCount,
                commandIndicesCount);
            sortedIndiesCount += (ushort)commandIndicesCount;

            // iterate through sorted commands checking if any can now be merged to reduce expensive draw calls to the graphics API
            // this might need to be changed for next-gen graphics API (Vulkan, Metal, DirectX 11) where the draw calls are not so expensive
            for (var index = 1; index < _commandsCount; index++)
            {
                var command = _commands[index];

                // get the number of vertices used for the command, each index represents a vertex
                commandIndicesCount = PrimitiveType.GetVerticesCount(command.PrimitiveCount);

                if (_currentCommand.CanMergeWith(command.SortKey, ref command.Data))
                {
                    // increase the number of primitives for the current command by the amount of the merged command
                    _commands[newCommandsCount - 1].PrimitiveCount =
                        _currentCommand.PrimitiveCount += command.PrimitiveCount;
                }
                else
                {
                    // could not merge command
                    newCommandsCount++;
                    // change the indices used by the command to use the sorted indices array so the indices are sequential for the graphics API
                    _commands[newCommandsCount - 1].StartIndex = sortedIndiesCount;
                    // set the current command to this command so we can check if the next command can be merged
                    _currentCommand = command;
                }

                // copy the indices of the command into our sorted indices array so the merged indices are sequential for the graphics API
                Array.Copy(_geometryBuffer.Indices, command.StartIndex, _sortedIndices, sortedIndiesCount,
                    commandIndicesCount);
                sortedIndiesCount += (ushort)commandIndicesCount;
            }

            var indexBuffer = _geometryBuffer.IndexBuffer;
            // upload the indices to the GPU's memory from CPU's memory
            indexBuffer.SetData(_sortedIndices, 0, sortedIndiesCount);
            // tell the graphics API we want to use that buffer of indices for rendering
            GraphicsDevice.Indices = indexBuffer;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < newCommandsCount; index++)
                CommandDrawer.Draw(ref _commands[index]);
        }

        internal override void EnqueueDrawCommand(int startIndex, int primitiveCount, float sortKey,
            ref TCommandData data)
        {
            // merge draw commands if possible to reduce expensive draw calls to the graphics API
            // this might need to be changed for next-gen graphics API (Vulkan, Metal, DirectX 11) where the draw calls are not so expensive
            if (_currentCommand.CanMergeWith(sortKey, ref data))
            {
                // since indices are added in sequential fasion we can just increase the primitive count of the current command
                _commands[_commandsCount - 1].PrimitiveCount = _currentCommand.PrimitiveCount += primitiveCount;
                return;
            }

            // overflow check
            if (_commandsCount >= _maximumCommandsCount)
                throw new BatchCommandQueueOverflowException(_maximumCommandsCount);

            // could not merge draw command, initialize a new one
            _currentCommand = new BatchDrawCommand<TCommandData>(startIndex, primitiveCount, sortKey, data);

            // append the command to the array
            _commands[_commandsCount++] = _currentCommand;
        }
    }
}