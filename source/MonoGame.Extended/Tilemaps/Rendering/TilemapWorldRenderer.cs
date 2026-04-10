using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Tilemaps.Rendering;

/// <summary>
/// High-performance world-map renderer using GraphicsDevice directly.
/// </summary>
/// <remarks>
/// <para>
/// Pre-bakes all tile geometry from a collection of tilemaps into world-space vertex buffers
/// grouped by (WorldDepth, texture, parallax factor). Each group produces a single draw call
/// regardless of how many rooms contributed tiles to it.
/// </para>
/// <para>
/// Use <see cref="Draw(OrthographicCamera, int)"/> to render a specific depth layer, or
/// <see cref="Draw(OrthographicCamera)"/> to render depth 0.
/// </para>
/// <para>
/// Animated tiles are not currently supported in world mode. Tile geometry is baked at load
/// time and is not rebuilt per-frame. Use <see cref="TilemapWorldSpriteBatchRenderer"/> if
/// animated tile support is required.
/// </para>
/// </remarks>
public sealed class TilemapWorldRenderer : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly DefaultEffect _defaultEffect;
    private Effect _effect;
    private BlendState _blendState;
    private SamplerState _samplerState;
    private bool _isDisposed;

    private readonly List<WorldBatch> _worldBatches = new List<WorldBatch>();
    private bool _isLoaded;

    /// <summary>
    /// Gets or sets the blend state used when drawing tiles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <see cref="BlendState.NonPremultiplied"/>, which is correct for tileset
    /// textures loaded directly from file where the PNG pixel data has not been modified.
    /// </para>
    /// <para>
    /// Set this to <see cref="BlendState.AlphaBlend"/> if your tileset textures are loaded
    /// through the MonoGame content pipeline, which premultiplies alpha by default.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public BlendState BlendState
    {
        get
        {
            ThrowIfDisposed();
            return _blendState;
        }
        set
        {
            ThrowIfDisposed();
            _blendState = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// Gets or sets the sampler state used when drawing tiles.
    /// </summary>
    /// <remarks>
    /// The default is <see cref="SamplerState.PointClamp"/>, which is correct for pixel-art
    /// tilesets. It produces clean, hard-edged tiles at any zoom level with no edge bleeding.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public SamplerState SamplerState
    {
        get
        {
            ThrowIfDisposed();
            return _samplerState;
        }
        set
        {
            ThrowIfDisposed();
            _samplerState = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// Gets or sets the shader effect used when drawing tiles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is an internal <see cref="DefaultEffect"/> configured for textured,
    /// vertex-colored rendering.
    /// </para>
    /// <para>
    /// A custom effect must implement <see cref="IEffectMatrices"/> so the renderer can set
    /// the View, Projection, and World matrices. The effect must also declare a
    /// <c>Texture</c> parameter (set via <c>effect.Parameters["Texture"]</c>) and must be
    /// compatible with the <see cref="VertexPositionColorTexture"/> vertex declaration.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
    /// <exception cref="ArgumentException">Thrown if value does not implement IEffectMatrices.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public Effect Effect
    {
        get
        {
            ThrowIfDisposed();
            return _effect;
        }
        set
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(value);
            if (value is not IEffectMatrices)
            {
                throw new ArgumentException("Effect must implement IEffectMatrices.", nameof(value));
            }

            _effect = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapWorldRenderer"/> class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <exception cref="ArgumentNullException">Thrown if graphicsDevice is null.</exception>
    public TilemapWorldRenderer(GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        _graphicsDevice = graphicsDevice;
        _defaultEffect = new DefaultEffect(_graphicsDevice)
        {
            TextureEnabled = true,
            VertexColorEnabled = true
        };
        _effect = _defaultEffect;
        _blendState = BlendState.NonPremultiplied;
        _samplerState = SamplerState.PointClamp;
    }

    /// <summary>
    /// Loads a collection of tilemaps as a seamless world.
    /// </summary>
    /// <param name="tilemaps">The collection of tilemaps with world position data.</param>
    /// <remarks>
    /// Each tilemap's <see cref="Tilemap.WorldPosition"/> and <see cref="Tilemap.WorldDepth"/>
    /// properties determine where it is placed in the world and which depth layer it belongs to.
    /// All tile geometry is baked into world-space vertex buffers at load time.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if tilemaps is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Load(IEnumerable<Tilemap> tilemaps)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(tilemaps);

        Unload();

        Dictionary<TileBatchKey, TileAccumulator> groups = new Dictionary<TileBatchKey, TileAccumulator>();

        foreach (Tilemap tilemap in tilemaps)
        {
            int depth = tilemap.WorldDepth;
            Vector2 worldPos = tilemap.WorldPosition;

            foreach (TilemapLayer layer in tilemap.Layers)
            {
                if (layer is not TilemapTileLayer tileLayer)
                {
                    continue;
                }

                foreach (TilemapTileEntry entry in tileLayer.GetTiles())
                {
                    TilemapTileset tileset = tilemap.Tilesets.GetTilesetForGid(entry.Tile.GlobalId);
                    if (tileset == null)
                    {
                        continue;
                    }

                    int localId = entry.Tile.GlobalId - tileset.FirstGlobalId;
                    tileset.GetRenderSource(localId, out Texture2D tileTexture, out Rectangle sourceRect);

                    if (tileTexture == null)
                    {
                        continue;
                    }

                    TileBatchKey key = new TileBatchKey(depth, tileTexture, tileLayer.ParallaxFactor);
                    if (!groups.TryGetValue(key, out TileAccumulator group))
                    {
                        group = new TileAccumulator();
                        groups[key] = group;
                    }

                    Point tilePos = tilemap.TileToWorldPosition(entry.X, entry.Y);
                    Vector2 position = new Vector2(tilePos.X, tilePos.Y) + worldPos + tileLayer.Offset + tileset.TileOffset;
                    // Tiled bottom-aligns all tiles: shifts oversized tiles up and undersized tiles down.
                    position.Y += tileLayer.TileHeight - sourceRect.Height;

                    Color tileColor = new Color(1f, 1f, 1f, tileLayer.Opacity);
                    TilemapRendererShared.AddTileQuad(
                        group.Vertices, group.Indices,
                        position, sourceRect.Width, sourceRect.Height,
                        sourceRect, entry.Tile.FlipFlags, tileTexture, tileColor);
                }
            }
        }

        foreach (KeyValuePair<TileBatchKey, TileAccumulator> kvp in groups)
        {
            TileBatchKey batchKey = kvp.Key;
            TileAccumulator accumulator = kvp.Value;

            if (accumulator.Vertices.Count == 0)
            {
                continue;
            }

            LayerModel model = TilemapRendererShared.CreateLayerModel(_graphicsDevice, accumulator.Vertices.ToArray(), accumulator.Indices.ToArray(), batchKey.Texture);
            model.ParallaxFactor = batchKey.ParallaxFactor;
            _worldBatches.Add(new WorldBatch { Model = model, WorldDepth = batchKey.WorldDepth });
        }

        _isLoaded = true;
    }

    /// <summary>
    /// Loads a <see cref="TilemapWorld"/> as a seamless world.
    /// </summary>
    /// <param name="world">The tilemap world whose levels will be rendered.</param>
    /// <exception cref="ArgumentNullException">Thrown if world is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Load(TilemapWorld world)
    {
        ArgumentNullException.ThrowIfNull(world);
        Load(world.Levels);
    }

    /// <summary>
    /// Unloads all world data and disposes GPU resources.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Unload()
    {
        ThrowIfDisposed();

        foreach (WorldBatch batch in _worldBatches)
        {
            batch.Model.Dispose();
        }

        _worldBatches.Clear();
        _isLoaded = false;
    }

    /// <summary>
    /// Draws all tilemaps at depth 0.
    /// </summary>
    /// <param name="camera">The camera for view and projection matrices.</param>
    /// <exception cref="ArgumentNullException">Thrown if camera is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if Load has not been called.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Draw(OrthographicCamera camera)
    {
        Draw(camera, 0);
    }

    /// <summary>
    /// Draws all tilemaps at the specified world depth.
    /// </summary>
    /// <param name="camera">The camera for view and projection matrices.</param>
    /// <param name="worldDepth">The depth layer to render. Only tilemaps with this exact depth are drawn.</param>
    /// <exception cref="ArgumentNullException">Thrown if camera is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if Load has not been called.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Draw(OrthographicCamera camera, int worldDepth)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(camera);

        if (!_isLoaded)
        {
            throw new InvalidOperationException("No world loaded. Call Load first.");
        }

        _graphicsDevice.BlendState = _blendState;
        _graphicsDevice.SamplerStates[0] = _samplerState;

        IEffectMatrices matrices = (IEffectMatrices)_effect;

        Matrix worldView = camera.GetViewMatrix();
        // Snap view translation to integer pixels to prevent sub-pixel edge bleeding.
        worldView.M41 = MathF.Round(worldView.M41);
        worldView.M42 = MathF.Round(worldView.M42);
        matrices.View = worldView;
        matrices.Projection = Matrix.CreateOrthographicOffCenter(
            0, _graphicsDevice.Viewport.Width,
            _graphicsDevice.Viewport.Height, 0,
            0, 1);
        matrices.World = Matrix.Identity;

        foreach (WorldBatch batch in _worldBatches)
        {
            if (batch.WorldDepth != worldDepth)
            {
                continue;
            }

            if (batch.Model.ParallaxFactor == Vector2.One)
            {
                matrices.World = Matrix.Identity;
            }
            else
            {
                Vector2 offset = camera.Position * (Vector2.One - batch.Model.ParallaxFactor);
                matrices.World = Matrix.CreateTranslation(offset.X, offset.Y, 0f);
            }

            DrawModel(batch.Model);
        }

        matrices.World = Matrix.Identity;
    }

    private void DrawModel(LayerModel model)
    {
        _effect.Parameters["Texture"].SetValue(model.Texture);
        _graphicsDevice.SetVertexBuffer(model.VertexBuffer);
        _graphicsDevice.Indices = model.IndexBuffer;

        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                baseVertex: 0,
                startIndex: 0,
                primitiveCount: model.PrimitiveCount);
        }
    }

    /// <summary>
    /// Disposes all GPU resources used by this renderer.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Unload();
        _defaultEffect?.Dispose();
        _isDisposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(TilemapWorldRenderer));
        }
    }

    private readonly record struct TileBatchKey(int WorldDepth, Texture2D Texture, Vector2 ParallaxFactor);

    private sealed class TileAccumulator
    {
        public List<VertexPositionColorTexture> Vertices { get; } = new List<VertexPositionColorTexture>();
        public List<int> Indices { get; } = new List<int>();
    }

    private sealed class WorldBatch
    {
        public LayerModel Model { get; set; }
        public int WorldDepth { get; set; }
    }
}
