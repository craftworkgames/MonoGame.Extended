using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled.Graphics.Effects;

namespace MonoGame.Extended.Tiled.Graphics
{
    public class TiledMapLayerModelNew
    {
        public TiledMapLayerModelNew(GraphicsDevice graphicsDevice, Texture2D texture, VertexPositionTexture[] vertices, ushort[] indices)
        {
            Texture = texture;

            VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices, 0, vertices.Length);

            IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(indices, 0, indices.Length);

            TriangleCount = indices.Length / 3;
        }

        public Texture2D Texture { get; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public int TriangleCount { get; }
    }

    public class TiledMapModel
    {
        public TiledMapModel(TiledMapLayerModelNew[] layers)
        {
            LayerModels = layers;
        }

        public TiledMapLayerModelNew[] LayerModels { get; }
    }

    public class TiledMapModelBuilder
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly TiledMap _map;

        public TiledMapModelBuilder(GraphicsDevice graphicsDevice, TiledMap map)
        {
            _graphicsDevice = graphicsDevice;
            _map = map;
        }

        public TiledMapModel Build()
        {
            var layerModels = new List<TiledMapLayerModelNew>();

            foreach (var tileLayer in _map.Layers.OfType<TiledMapTileLayer>())
            {
                foreach (var tileset in _map.Tilesets)
                {
                    var texture = tileset.Texture;
                    var vertices = new List<VertexPositionTexture>();
                    var indices = new List<ushort>();

                    foreach (var tile in tileLayer.Tiles.Where(t => tileset.ContainsGlobalIdentifier(t.GlobalIdentifier)))
                    {
                        var tileGid = tile.GlobalIdentifier;
                        var localTileIdentifier = tileGid - tileset.FirstGlobalIdentifier;
                        var position = GetTilePosition(_map, tile);
                        var tilesetColumns = tileset.Columns == 0 ? 1 : tileset.Columns; // fixes a problem (what problem exactly?)
                        var sourceRectangle = TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, tileset.TileWidth, tileset.TileHeight, tilesetColumns, tileset.Margin, tileset.Spacing);
                        var flipFlags = tile.Flags;

                        indices.AddRange(CreateTileIndices(vertices.Count));
                        Debug.Assert(indices.Count <= TiledMapHelper.MaximumIndicesPerModel);

                        vertices.AddRange(CreateVertices(texture, position, sourceRectangle, flipFlags));
                        Debug.Assert(vertices.Count <= TiledMapHelper.MaximumVerticesPerModel);

                        // check if the current model is full
                        if (vertices.Count + TiledMapHelper.VerticesPerTile >= TiledMapHelper.MaximumVerticesPerModel)
                        {
                            layerModels.Add(new TiledMapLayerModelNew(_graphicsDevice, texture, vertices.ToArray(), indices.ToArray()));
                            vertices = new List<VertexPositionTexture>();
                            indices = new List<ushort>();
                        }
                    }

                    if (vertices.Any())
                        layerModels.Add(new TiledMapLayerModelNew(_graphicsDevice, texture, vertices.ToArray(), indices.ToArray()));
                }
            }

            return new TiledMapModel(layerModels.ToArray());
        }

        private static Point2 GetTilePosition(TiledMap map, TiledMapTile mapTile)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    return TiledMapHelper.GetOrthogonalPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientation.Isometric:
                    return TiledMapHelper.GetIsometricPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientation.Staggered:
                    throw new NotImplementedException("Staggered maps are not yet implemented.");
                default:
                    throw new NotSupportedException($"Tiled Map {map.Orientation} is not supported.");
            }
        }

        private static IEnumerable<VertexPositionTexture> CreateVertices(Texture2D texture, Point2 position, Rectangle sourceRectangle, TiledMapTileFlipFlags flags = TiledMapTileFlipFlags.None)
        {
            var reciprocalWidth = 1f / texture.Width;
            var reciprocalHeight = 1f / texture.Height;
            var texelLeft = (sourceRectangle.X + 0.5f) * reciprocalWidth;
            var texelTop = (sourceRectangle.Y + 0.5f) * reciprocalHeight;
            var texelRight = (sourceRectangle.X + sourceRectangle.Width) * reciprocalWidth;
            var texelBottom = (sourceRectangle.Y + sourceRectangle.Height) * reciprocalHeight;

            VertexPositionTexture vertexTopLeft, vertexTopRight, vertexBottomLeft, vertexBottomRight;

            vertexTopLeft.Position = new Vector3(position, 0);
            vertexTopRight.Position = new Vector3(position + new Vector2(sourceRectangle.Width, 0), 0);
            vertexBottomLeft.Position = new Vector3(position + new Vector2(0, sourceRectangle.Height), 0);
            vertexBottomRight.Position = new Vector3(position + new Vector2(sourceRectangle.Width, sourceRectangle.Height), 0);

            vertexTopLeft.TextureCoordinate.Y = texelTop;
            vertexTopLeft.TextureCoordinate.X = texelLeft;

            vertexTopRight.TextureCoordinate.Y = texelTop;
            vertexTopRight.TextureCoordinate.X = texelRight;

            vertexBottomLeft.TextureCoordinate.Y = texelBottom;
            vertexBottomLeft.TextureCoordinate.X = texelLeft;

            vertexBottomRight.TextureCoordinate.Y = texelBottom;
            vertexBottomRight.TextureCoordinate.X = texelRight;

            var flipDiagonally = (flags & TiledMapTileFlipFlags.FlipDiagonally) != 0;
            var flipHorizontally = (flags & TiledMapTileFlipFlags.FlipHorizontally) != 0;
            var flipVertically = (flags & TiledMapTileFlipFlags.FlipVertically) != 0;

            if (flipDiagonally)
            {
                FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.X, ref vertexBottomLeft.TextureCoordinate.X);
                FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.Y, ref vertexBottomLeft.TextureCoordinate.Y);
            }

            if (flipHorizontally)
            {
                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.Y, ref vertexTopRight.TextureCoordinate.Y);
                    FloatHelper.Swap(ref vertexBottomLeft.TextureCoordinate.Y, ref vertexBottomRight.TextureCoordinate.Y);
                }
                else
                {
                    FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.X, ref vertexTopRight.TextureCoordinate.X);
                    FloatHelper.Swap(ref vertexBottomLeft.TextureCoordinate.X, ref vertexBottomRight.TextureCoordinate.X);
                }
            }

            if (flipVertically)
            {
                if (flipDiagonally)
                {
                    FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.X, ref vertexBottomLeft.TextureCoordinate.X);
                    FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.X, ref vertexBottomRight.TextureCoordinate.X);
                }
                else
                {
                    FloatHelper.Swap(ref vertexTopLeft.TextureCoordinate.Y, ref vertexBottomLeft.TextureCoordinate.Y);
                    FloatHelper.Swap(ref vertexTopRight.TextureCoordinate.Y, ref vertexBottomRight.TextureCoordinate.Y);
                }
            }

            yield return vertexTopLeft;
            yield return vertexTopRight;
            yield return vertexBottomLeft;
            yield return vertexBottomRight;
        }

        private static IEnumerable<ushort> CreateTileIndices(int indexOffset)
        {
            yield return (ushort)(0 + indexOffset);
            yield return (ushort)(1 + indexOffset);
            yield return (ushort)(2 + indexOffset);
            yield return (ushort)(1 + indexOffset);
            yield return (ushort)(3 + indexOffset);
            yield return (ushort)(2 + indexOffset);
        }


        //private static IEnumerable<TiledMapLayerModelContent> CreateTileLayerModels(TiledMapContent map, string layerName, IEnumerable<TiledMapTile> tiles)
        //{
        //    // the code below builds the geometry (triangles) for every tile
        //    // for every unique tileset used by a tile in a layer, we are going to end up with a different model (list of vertices and list of indices pair)
        //    // we also could end up with more models if the map is very large
        //    // regardless, each model is going to require one draw call to render at runtime

        //    var modelsByTileset = new Dictionary<TiledMapTilesetContent, List<TiledMapLayerModelContent>>();

        //    // loop through all the tiles in the proper render order, building the geometry for each tile
        //    // by processing the tiles in the correct rendering order we ensure the geometry for the tiles will be rendered correctly later using the painter's algorithm
        //    foreach (var tile in tiles)
        //    {
        //        // get the tileset for this tile
        //        var tileGlobalIdentifier = tile.GlobalIdentifier;
        //        var tileset = map.Tilesets.FirstOrDefault(x => x.ContainsGlobalIdentifier(tileGlobalIdentifier));
        //        if (tileset == null)
        //            throw new NullReferenceException(
        //                $"Could not find tileset for global tile identifier '{tileGlobalIdentifier}'");

        //        var localTileIdentifier = tileGlobalIdentifier - tileset.FirstGlobalIdentifier;
        //        Debug.Assert(tileset != null);

        //        // check if this tile is animated
        //        var tilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalIdentifier == localTileIdentifier);
        //        var isAnimatedTile = tilesetTile?.Frames != null && tilesetTile.Frames.Count > 0;

        //        // check if we already have built a list of models for this tileset
        //        TiledMapLayerModelContent model;
        //        List<TiledMapLayerModelContent> models;

        //        if (modelsByTileset.TryGetValue(tileset, out models))
        //        {
        //            // if we found the list of models for this tileset, try to use the last model added
        //            // (assuming the the ones before the last are all full)
        //            model = models.FindLast(x => x is TiledMapLayerAnimatedModelContent == isAnimatedTile);
        //            // since it is possible that the correct type of model was not added yet we might have to create the correct model now
        //            if (model == null)
        //            {
        //                model = isAnimatedTile
        //                    ? new TiledMapLayerAnimatedModelContent(layerName, tileset)
        //                    : new TiledMapLayerModelContent(layerName, tileset);
        //                models.Add(model);
        //            }
        //        }
        //        else
        //        {
        //            // if we have not found the list of models for this tileset, we need to create the list and start a new model of the correct type
        //            models = new List<TiledMapLayerModelContent>();
        //            model = isAnimatedTile
        //                ? new TiledMapLayerAnimatedModelContent(layerName, tileset)
        //                : new TiledMapLayerModelContent(layerName, tileset);
        //            models.Add(model);
        //            modelsByTileset.Add(tileset, models);
        //        }

        //        // check if the current model is full
        //        if (model.Vertices.Count + TiledMapHelper.VerticesPerTile > TiledMapHelper.MaximumVerticesPerModel)
        //        {
        //            // if the current model is full, we need to start a new one
        //            model = isAnimatedTile
        //                ? new TiledMapLayerAnimatedModelContent(layerName, tileset)
        //                : new TiledMapLayerModelContent(layerName, tileset);
        //            models.Add(model);
        //        }

        //        // if the tile is animated, record the index of animated tile for the tilset so we can get the correct texture coordinates at runtime
        //        if (isAnimatedTile)
        //        {
        //            var animatedModel = (TiledMapLayerAnimatedModelContent)model;
        //            animatedModel.AddAnimatedTile(tilesetTile);
        //        }

        //        // fixes a problem
        //        if (tileset.Columns == 0)
        //            tileset.Columns = 1;

        //        // build the geometry for the tile
        //        var position = GetTilePosition(map, tile);
        //        var sourceRectangle = TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, tileset.TileWidth,
        //            tileset.TileHeight, tileset.Columns, tileset.Margin, tileset.Spacing);
        //        var flipFlags = tile.Flags;
        //        model.AddTileIndices();
        //        model.AddTileVertices(position, sourceRectangle, flipFlags);
        //    }

        //    // for each tileset used in this layer
        //    foreach (var keyValuePair in modelsByTileset)
        //    {
        //        var models = keyValuePair.Value;

        //        // and for each model apart of a tileset
        //        foreach (var model in models)
        //            yield return model;
        //    }
        //}
    }

    public class TiledMapRenderer : IDisposable
    {
        private readonly TiledMapModel _mapModel;

        private readonly TiledMap _map;
        private readonly TiledMapEffect _defaultEffect;
        private Matrix _worldMatrix = Matrix.Identity;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly Dictionary<TiledMapTileset, List<TiledMapTilesetAnimatedTile>> _animatedTilesByTileset;

        public TiledMapRenderer(GraphicsDevice graphicsDevice, TiledMap map)
        {
            if (graphicsDevice == null) throw new ArgumentNullException(nameof(graphicsDevice));
            if (map == null) throw new ArgumentNullException(nameof(map));

            _map = map;
            _graphicsDevice = graphicsDevice;
            _defaultEffect = new TiledMapEffect(graphicsDevice);

            _animatedTilesByTileset = _map.Tilesets.ToDictionary(i => i, i => i.Tiles.OfType<TiledMapTilesetAnimatedTile>().ToList());

            var modelBuilder = new TiledMapModelBuilder(graphicsDevice, map);
            _mapModel = modelBuilder.Build();
        }

        public void Dispose()
        {
            _defaultEffect.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            for (var tilesetIndex = 0; tilesetIndex < _map.Tilesets.Count; tilesetIndex++)
            {
                var tileset = _map.Tilesets[tilesetIndex];
                var animatedTiles = _animatedTilesByTileset[tileset];

                for (var animatedTileIndex = 0; animatedTileIndex < animatedTiles.Count; animatedTileIndex++)
                {
                    var animatedTilesetTile = animatedTiles[animatedTileIndex];
                    animatedTilesetTile.Update(gameTime);
                }
            }

            //for (var index = 0; index < _map.TileLayers.Count; index++)
            //{
            //    var layer = _map.TileLayers[index];
            //    UpdateAnimatedModels(layer.AnimatedModels);
            //}
        }

        public void Draw(Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, -1);

            Draw(ref viewMatrix1, ref projectionMatrix1, effect, depth);
        }

        public void Draw(ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
        {
            for (var index = 0; index < _map.Layers.Count; index++)
                Draw(index, ref viewMatrix, ref projectionMatrix, effect, depth);
        }

        public void Draw(int layerIndex, Matrix? viewMatrix = null, Matrix? projectionMatrix = null, Effect effect = null, float depth = 0.0f)
        {
            var viewMatrix1 = viewMatrix ?? Matrix.Identity;
            var projectionMatrix1 = projectionMatrix ?? Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, -1);

            Draw(layerIndex, ref viewMatrix1, ref projectionMatrix1, effect, depth);
        }

        public void Draw(int layerIndex, ref Matrix viewMatrix, ref Matrix projectionMatrix, Effect effect = null, float depth = 0.0f)
        {
            var layer = _map.Layers[layerIndex];

            if (!layer.IsVisible)
                return;

            if (layer is TiledMapObjectLayer)
                return;

            _worldMatrix.Translation = new Vector3(layer.Offset.X, layer.Offset.Y, depth);

            var effect1 = effect ?? _defaultEffect;
            var tiledMapEffect = effect1 as ITiledMapEffect;
            if (tiledMapEffect == null)
                return;

            // model-to-world transform
            tiledMapEffect.World = _worldMatrix;
            tiledMapEffect.View = viewMatrix;
            tiledMapEffect.Projection = projectionMatrix;

            foreach (var layerModel in _mapModel.LayerModels)
            {
                // desired alpha
                tiledMapEffect.Alpha = layer.Opacity;

                // desired texture
                tiledMapEffect.Texture = layerModel.Texture;

                // bind the vertex and index buffer
                _graphicsDevice.SetVertexBuffer(layerModel.VertexBuffer);
                _graphicsDevice.Indices = layerModel.IndexBuffer;

                // for each pass in our effect
                foreach (var pass in effect1.CurrentTechnique.Passes)
                {
                    // apply the pass, effectively choosing which vertex shader and fragment (pixel) shader to use
                    pass.Apply();

                    // draw the geometry from the vertex buffer / index buffer
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, layerModel.TriangleCount);
                }
            }
        }

        //// ReSharper disable once ParameterTypeCanBeEnumerable.Local
        //private static unsafe void UpdateAnimatedModels(TiledMapLayerAnimatedModel[] animatedModels)
        //{
        //    foreach (var animatedModel in animatedModels)
        //    {
        //        // update the texture coordinates for each animated tile
        //        fixed (VertexPositionTexture* fixedVerticesPointer = animatedModel.Vertices)
        //        {
        //            var verticesPointer = fixedVerticesPointer;

        //            foreach (var animatedTile in animatedModel.AnimatedTilesetTiles)
        //            {
        //                var currentFrameTextureCoordinates = animatedTile.CurrentAnimationFrame.TextureCoordinates;

        //                (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[0];
        //                (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[1];
        //                (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[2];
        //                (*verticesPointer++).TextureCoordinate = currentFrameTextureCoordinates[3];
        //            }
        //        }

        //        // copy (upload) the updated vertices to the GPU's memory
        //        animatedModel.VertexBuffer.SetData(animatedModel.Vertices, 0, animatedModel.Vertices.Length);
        //    }
        //}
    }
}