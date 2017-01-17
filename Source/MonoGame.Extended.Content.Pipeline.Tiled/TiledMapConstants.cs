namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public static class TiledMapConstants
    {
        // 4 vertices per tile
        public const int VerticesPerTile = 4;
        // 2 triangles per tile (mesh), with each triangle indexing 3 out of 4 vertices, so 6 vertices
        public const int IndicesPerTile = 6;
        // by using ushort type for indices we are limited to indexing vertices from 0 to 65535
        // this limits us on how many vertices can fit inside a single vertex buffer (65536 vertices) 
        public const int MaximumVerticesPerModel = ushort.MaxValue + 1;
        // and thus, we know how many tiles we can fit inside a vertex or index buffer (16384 tiles)
        public const int MaximumTilesPerGeometryContent = MaximumVerticesPerModel / VerticesPerTile;
        // and thus, we also know the maximum number of indices we can fit inside a single index buffer (98304 indices)
        public const int MaximumIndicesPerModel = MaximumTilesPerGeometryContent * IndicesPerTile;
        // these optimal maximum numbers of course are not considering texture bindings which would practically lower the actual number of tiles per vertex / index buffer
        // thus, the reason why it is a good to have ONE giant tileset (at least per layer)
    }
}
