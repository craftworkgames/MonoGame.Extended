using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Geometry;

namespace MonoGame.Extended.Graphics.Batching
{
    internal sealed class DeferredBatchCommandQueue<TVertexType, TIndexType, TCommandData> :
            BatchCommandQueue<TVertexType, TIndexType, TCommandData>
        where TVertexType : struct, IVertexType
        where TIndexType : struct
        where TCommandData : struct, IBatchDrawCommandData<TCommandData>
    {
        private readonly BatchDrawCommand<TCommandData>[] _commands;
        private readonly int _maximumCommandsCount;
        private readonly TIndexType[] _sortedIndices;
        private int _commandsCount;
        private BatchDrawCommand<TCommandData> _currentCommand;
        internal BatchSortMode SortMode;

        internal DeferredBatchCommandQueue(GraphicsDevice graphicsDevice,
            BatchCommandDrawer<TVertexType, TIndexType, TCommandData> commandDrawer,
            GraphicsGeometryData<TVertexType, TIndexType> geometryData, int maximumCommandsCount)
            : base(graphicsDevice, commandDrawer, geometryData)
        {
            _commands = new BatchDrawCommand<TCommandData>[maximumCommandsCount];
            _sortedIndices = new TIndexType[geometryData.Indices.Length];
            _maximumCommandsCount = maximumCommandsCount;
        }

        protected internal override void Flush()
        {
            // quit early if there are no deferred commands to process
            if (_commandsCount == 0)
                return;

            // upload the vertices to the GPU's memory from CPU's memory
            CommandDrawer.UploadVertices(GeometryData.Vertices, 0, GeometryData.VerticesCount);

            if (SortMode == BatchSortMode.Deferred)
            {
                CommandDrawer.UploadIndices(GeometryData.Indices, 0, GeometryData.IndicesCount);
            }
            else
            {
                // sort the commands
                Array.Sort(_commands, 0, _commandsCount);
                int sortedIndiesCount;
                SortIndices(out _commandsCount, out sortedIndiesCount);
                CommandDrawer.UploadIndices(_sortedIndices, 0, sortedIndiesCount);
            }

            CommandDrawer.SelectVertices();
            CommandDrawer.SelectIndices();

            // do the actual rendering using the graphics API
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _commandsCount; index++)
            {
                CommandDrawer.Draw(ref _commands[index]);
            }

            // prevent a possible memory leak where the current command would be holding onto some reference(s)
            _currentCommand.Data.SetReferencesToNull();

            // reset the commands count
            // we don't clear the array of commands because we will just overwrite the values anyways
            _commandsCount = 0;
        }

        private void SortIndices(out int commandsCount, out int sortedIndicesCount)
        {
            _currentCommand = _commands[0];
            var newCommandsCount = 1;
            sortedIndicesCount = 0;

            // change the indices used by the command to use the sorted indices array so the indices are sequential for the graphics API
            _commands[0].StartIndex = 0;
            // get the number of vertices used for the current command, each index represents a vertex
            var commandIndicesCount = _commands[0].PrimitiveType.GetVerticesCount(_currentCommand.PrimitiveCount);
            // copy the indices of the current command into our sorted indices array so the indices are sequential for the graphics API
            Array.Copy(GeometryData.Indices, _currentCommand.StartIndex, _sortedIndices, sortedIndicesCount,
                commandIndicesCount);
            sortedIndicesCount += (ushort)commandIndicesCount;

            // iterate through sorted commands checking if any can now be merged to reduce expensive draw calls to the graphics API
            // this might need to be changed for next-gen graphics API (Vulkan, Metal, DirectX 11) where the draw calls are not so expensive
            for (var index = 1; index < _commandsCount; index++)
            {
                var command = _commands[index];

                // get the number of vertices used for the command, each index represents a vertex
                commandIndicesCount = command.PrimitiveType.GetVerticesCount(command.PrimitiveCount);

                if (_currentCommand.CanMergeWith(command.PrimitiveType, command.SortKey, ref command.Data))
                {
                    if (command.PrimitiveType == PrimitiveType.TriangleStrip)
                    {
                        if (sortedIndicesCount + 2 > _sortedIndices.Length)
                            Flush();

                        // use degenerate triangles to merge triangle strips by repeating the first index of the first strip and second index of the last strip
                        _sortedIndices[sortedIndicesCount++] = _sortedIndices[sortedIndicesCount - 1];
                        _sortedIndices[sortedIndicesCount++ + 1] = GeometryData.Indices[command.StartIndex];
                    }

                    if (sortedIndicesCount + (ushort)commandIndicesCount > _sortedIndices.Length)
                        Flush();

                    // copy the indices of the command into our sorted indices array so the merged indices are sequential for the graphics API
                    Array.Copy(GeometryData.Indices, command.StartIndex, _sortedIndices, sortedIndicesCount,
                        commandIndicesCount);
                    sortedIndicesCount += (ushort)commandIndicesCount;

                    // increase the number of primitives for the current command by the amount of the merged command
                    _commands[newCommandsCount - 1].PrimitiveCount =
                        _currentCommand.PrimitiveCount += command.PrimitiveCount;
                }
                else
                {
                    // could not merge command

                    newCommandsCount++;
                    // change the indices used by the command to use the sorted indices array so the indices are sequential for the graphics API
                    _commands[newCommandsCount - 1].StartIndex = sortedIndicesCount;
                    // set the current command to this command so we can check if the next command can be merged
                    _currentCommand = command;

                    if (sortedIndicesCount + (ushort)commandIndicesCount > _sortedIndices.Length)
                        Flush();

                    // copy the indices of the command into our sorted indices array so the merged indices are sequential for the graphics API
                    Array.Copy(GeometryData.Indices, command.StartIndex, _sortedIndices, sortedIndicesCount,
                        commandIndicesCount);
                    sortedIndicesCount += (ushort)commandIndicesCount;
                }
            }

            commandsCount = newCommandsCount;
        }

        internal override void EnqueueDrawCommand(PrimitiveType primitiveType, int primitiveCount, int startIndex, float sortKey, ref TCommandData data)
        {
            // merge draw commands if possible to reduce expensive draw calls to the graphics API
            // this might need to be changed for next-gen graphics API (Vulkan, Metal, DirectX 11) where the draw calls are not so expensive
            // don't support merging triangle strip and line strip here
            if ((primitiveType != PrimitiveType.TriangleStrip) && (primitiveType != PrimitiveType.LineStrip) &&
                _currentCommand.CanMergeWith(primitiveType, sortKey, ref data))
            {
                // since indices are added in sequential fasion we can just increase the primitive count of the current command
                _commands[_commandsCount - 1].PrimitiveCount = _currentCommand.PrimitiveCount += primitiveCount;
                return;
            }

            // overflow check
            if (_commandsCount >= _maximumCommandsCount)
                Flush();

            // could not merge draw command, initialize a new one
            _currentCommand = new BatchDrawCommand<TCommandData>(primitiveType, startIndex, primitiveCount, sortKey,
                data);

            // append the command to the array
            _commands[_commandsCount++] = _currentCommand;
        }
    }
}