using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace MonoGame.Extended.Tilemaps.Rendering;

/// <summary>
/// High-performance tilemap renderer using GraphicsDevice directly.
/// </summary>
/// <remarks>
/// <para>
/// This renderer provides optimal performance by rendering tiles using vertex and index buffers.
/// Supports layer grouping to merge multiple layers into a single draw call, significantly
/// reducing draw call overhead for maps with many layers.
/// </para>
/// <para>
/// Layer groups can be defined dynamically and updated at runtime. Groups are rebuilt
/// automatically when modified. For best performance with static maps, define groups
/// once during initialization.
/// </para>
/// </remarks>
public sealed class TilemapRenderer : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly DefaultEffect _defaultEffect;
    private Effect _effect;
    private Tilemap _tilemap;
    private RenderMode _defaultRenderMode;
    private BlendState _blendState;
    private SamplerState _samplerState;
    private bool _isDisposed;

    private BlendState _savedBlendState;
    private SamplerState _savedSamplerState;
    private RasterizerState _savedRasterizerState;
    private DepthStencilState _savedDepthStencilState;

    private readonly Dictionary<string, LayerGroup> _layerGroups;
    private readonly Dictionary<TilemapLayer, string> _layerToGroup;
    private readonly Dictionary<TilemapTileLayer, List<LayerModel>> _layerModels;
    private readonly Dictionary<TilemapImageLayer, LayerModel> _imageLayerModels;
    private readonly Dictionary<TilemapImageLayer, RepeatImageLayerModel> _repeatImageLayerModels;
    private readonly Dictionary<TilemapObjectLayer, List<LayerModel>> _objectLayerModels;
    private readonly List<LayerModel> _mergedUngroupedModels;
    private bool _mergedUngroupedDirty;
    private readonly HashSet<string> _drawnGroupsBuffer;

    private readonly List<TilemapTileData> _animatedTiles = new List<TilemapTileData>();
    private readonly HashSet<string> _groupsWithAnimations = new HashSet<string>();
    private bool _mergedUngroupedHasAnimations;

    private bool _isDrawing;
    private OrthographicCamera _camera;

    /// <summary>
    /// Gets the current default rendering mode.
    /// </summary>
    public RenderMode DefaultRenderMode
    {
        get
        {
            ThrowIfDisposed();
            return _defaultRenderMode;
        }
    }

    /// <summary>
    /// Gets or sets the blend state used when drawing tiles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <see cref="BlendState.NonPremultiplied"/>, which is correct for tileset
    /// textures loaded directly from file (e.g., via a stream or custom loader) where the PNG
    /// pixel data has not been modified.
    /// </para>
    /// <para>
    /// Set this to <see cref="BlendState.AlphaBlend"/> if your tileset textures are loaded
    /// through the MonoGame content pipeline, which premultiplies alpha by default. Note that
    /// layer opacity will not apply correctly to the RGB channel in this mode because the
    /// renderer bakes opacity only into the vertex color alpha at load time.
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
    /// <para>
    /// The default is <see cref="SamplerState.PointClamp"/>, which is correct for pixel-art
    /// tilesets. It produces clean, hard-edged tiles at any zoom level or rotation angle
    /// with no edge bleeding between adjacent tiles in the texture atlas.
    /// </para>
    /// <para>
    /// <see cref="SamplerState.LinearClamp"/> enables smooth sub-pixel interpolation for
    /// non-pixel-art tilesets. When using LinearClamp, keep the camera position at integer
    /// world-pixel coordinates to prevent linear filtering from blending tile edge texels with
    /// their atlas neighbors. LinearClamp also produces fringe artifacts at tile edges when
    /// the camera is rotated, so it is not recommended for maps that use camera rotation.
    /// </para>
    /// <para>
    /// For repeating image layers, the renderer automatically uses the wrap equivalent of
    /// the configured state: <see cref="SamplerState.PointClamp"/> becomes
    /// <see cref="SamplerState.PointWrap"/>, and <see cref="SamplerState.LinearClamp"/>
    /// becomes <see cref="SamplerState.LinearWrap"/>. Any other state is used as-is,
    /// so you can pass a wrap state directly if needed.
    /// </para>
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
    /// Gets the names of all defined layer groups.
    /// </summary>
    public IReadOnlyList<string> LayerGroups => new List<string>(_layerGroups.Keys);

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapRenderer"/> class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <exception cref="ArgumentNullException">Thrown if graphicsDevice is null.</exception>
    public TilemapRenderer(GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        _graphicsDevice = graphicsDevice;
        _defaultEffect = new DefaultEffect(_graphicsDevice)
        {
            TextureEnabled = true,
            VertexColorEnabled = true
        };
        _effect = _defaultEffect;

        _layerGroups = new Dictionary<string, LayerGroup>();
        _layerToGroup = new Dictionary<TilemapLayer, string>();
        _layerModels = new Dictionary<TilemapTileLayer, List<LayerModel>>();
        _imageLayerModels = new Dictionary<TilemapImageLayer, LayerModel>();
        _repeatImageLayerModels = new Dictionary<TilemapImageLayer, RepeatImageLayerModel>();
        _objectLayerModels = new Dictionary<TilemapObjectLayer, List<LayerModel>>();
        _mergedUngroupedModels = new List<LayerModel>();
        _drawnGroupsBuffer = new HashSet<string>();

        _defaultRenderMode = RenderMode.Merged;
        _blendState = BlendState.NonPremultiplied;
        _samplerState = SamplerState.PointClamp;
    }

    /// <summary>
    /// Loads a tilemap for rendering.
    /// </summary>
    /// <param name="tilemap">The tilemap to load.</param>
    /// <exception cref="ArgumentNullException">Thrown if tilemap is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void LoadTilemap(Tilemap tilemap)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(tilemap);

        UnloadTilemap();

        _tilemap = tilemap;

        BuildLayerModels();
    }

    /// <summary>
    /// Unloads the current tilemap and disposes all GPU resources.
    /// </summary>
    public void UnloadTilemap()
    {
        ThrowIfDisposed();

        foreach (List<LayerModel> models in _layerModels.Values)
        {
            foreach (LayerModel model in models)
            {
                model.Dispose();
            }
        }
        _layerModels.Clear();

        foreach (LayerModel model in _imageLayerModels.Values)
        {
            model.Dispose();
        }
        _imageLayerModels.Clear();

        foreach (RepeatImageLayerModel model in _repeatImageLayerModels.Values)
        {
            model.Dispose();
        }
        _repeatImageLayerModels.Clear();

        foreach (List<LayerModel> models in _objectLayerModels.Values)
        {
            foreach (LayerModel model in models)
            {
                model.Dispose();
            }
        }
        _objectLayerModels.Clear();

        foreach (LayerGroup group in _layerGroups.Values)
        {
            foreach (LayerModel model in group.MergedModels)
            {
                model.Dispose();
            }
        }
        _layerGroups.Clear();
        _layerToGroup.Clear();

        foreach (LayerModel model in _mergedUngroupedModels)
        {
            model.Dispose();
        }
        _mergedUngroupedModels.Clear();
        _mergedUngroupedDirty = false;

        _tilemap = null;
        _animatedTiles.Clear();
        _groupsWithAnimations.Clear();
        _mergedUngroupedHasAnimations = false;
    }

    /// <summary>
    /// Sets the default rendering mode for ungrouped layers.
    /// </summary>
    /// <param name="mode">The render mode.</param>
    /// <remarks>
    /// This affects the behavior of <see cref="Draw(OrthographicCamera)"/> for layers that are not
    /// in any group.
    /// </remarks>
    public void SetDefaultRenderMode(RenderMode mode)
    {
        ThrowIfDisposed();
        _defaultRenderMode = mode;
    }

    /// <summary>
    /// Defines a layer group for merged rendering.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <param name="layerNames">The names of the layers to include in the group.</param>
    /// <remarks>
    /// <para>
    /// Layers in a group are merged into a single draw call for optimal performance.
    /// Layers are rendered in the order specified, back to front.
    /// </para>
    /// <para>
    /// Groups can be redefined at any time. The merged vertex/index buffers are rebuilt
    /// when the group is next drawn.
    /// </para>
    /// <para>
    /// Each layer can only belong to one group. If a layer is already in another group,
    /// it will be removed from the previous group and added to the new one.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if groupName or layerNames is null.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if any layer name is not found in the tilemap.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no tilemap is loaded.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void DefineLayerGroup(string groupName, params string[] layerNames)
    {
        ThrowIfDisposed();
        ThrowIfNoTilemap();

        ArgumentNullException.ThrowIfNull(groupName);
        ArgumentNullException.ThrowIfNull(layerNames);

        List<TilemapLayer> layers = new List<TilemapLayer>(layerNames.Length);
        foreach (string layerName in layerNames)
        {
            layers.Add(_tilemap.Layers[layerName]);
        }

        if (_layerGroups.ContainsKey(groupName))
        {
            RemoveLayerGroup(groupName);
        }

        foreach (TilemapLayer layer in layers)
        {
            if (_layerToGroup.TryGetValue(layer, out string existingGroupName))
            {
                LayerGroup existingGroup = _layerGroups[existingGroupName];
                existingGroup.Layers.Remove(layer);
                existingGroup.IsDirty = true;
                _layerToGroup.Remove(layer);
            }
        }

        LayerGroup group = new LayerGroup { Layers = layers };
        _layerGroups[groupName] = group;

        foreach (TilemapLayer layer in layers)
        {
            _layerToGroup[layer] = groupName;
        }

        _mergedUngroupedDirty = true;
        UpdateAnimationSets();
    }

    /// <summary>
    /// Defines a layer group using layer indices.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <param name="startIndex">The index of the first layer.</param>
    /// <param name="count">The number of layers to include.</param>
    /// <remarks>
    /// Each layer can only belong to one group. If a layer is already in another group,
    /// it will be removed from the previous group and added to the new one.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if groupName is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if layer indices are out of range.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no tilemap is loaded.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void DefineLayerGroup(string groupName, int startIndex, int count)
    {
        ThrowIfDisposed();
        ThrowIfNoTilemap();

        ArgumentNullException.ThrowIfNull(groupName);

        if (startIndex < 0 || startIndex >= _tilemap.Layers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (count <= 0 || startIndex + count > _tilemap.Layers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        string[] layerNames = new string[count];
        for (int i = 0; i < count; i++)
        {
            layerNames[i] = _tilemap.Layers[startIndex + i].Name;
        }

        DefineLayerGroup(groupName, layerNames);
    }

    /// <summary>
    /// Removes a layer group.
    /// </summary>
    /// <param name="groupName">The name of the group to remove.</param>
    /// <remarks>
    /// Layers in the removed group become ungrouped and will be drawn individually
    /// if <see cref="Draw(OrthographicCamera)"/> is called.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void RemoveLayerGroup(string groupName)
    {
        ThrowIfDisposed();

        if (_layerGroups.ContainsKey(groupName))
        {
            LayerGroup group = _layerGroups[groupName];

            foreach (TilemapLayer layer in group.Layers)
            {
                _layerToGroup.Remove(layer);
            }

            foreach (LayerModel model in group.MergedModels)
            {
                model.Dispose();
            }

            _layerGroups.Remove(groupName);
            _mergedUngroupedDirty = true;
            UpdateAnimationSets();
        }
    }

    /// <summary>
    /// Gets whether a layer group is defined.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <returns>True if the group exists; otherwise, false.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public bool HasLayerGroup(string groupName)
    {
        ThrowIfDisposed();
        return _layerGroups.ContainsKey(groupName);
    }

    /// <summary>
    /// Marks a layer group as needing rebuild.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <remarks>
    /// Call this after modifying tiles in layers that belong to a group.
    /// The group will be rebuilt on the next draw call.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if the group does not exist.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void MarkGroupDirty(string groupName)
    {
        ThrowIfDisposed();

        if (!_layerGroups.TryGetValue(groupName, out LayerGroup group))
        {
            throw new ArgumentException($"Layer group '{groupName}' does not exist.", nameof(groupName));
        }

        group.IsDirty = true;
    }

    /// <summary>
    /// Rebuilds the merged vertex/index buffers for a layer group.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <remarks>
    /// This is called automatically when drawing a dirty group, but can be called
    /// manually to control when the rebuild happens (e.g., during loading).
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if the group does not exist.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void RebuildLayerGroup(string groupName)
    {
        ThrowIfDisposed();

        if (!_layerGroups.TryGetValue(groupName, out LayerGroup group))
        {
            throw new ArgumentException($"Layer group '{groupName}' does not exist.", nameof(groupName));
        }

        RebuildLayerGroupInternal(group);
    }

    /// <summary>
    /// Begins a manual rendering sequence.
    /// </summary>
    /// <param name="camera">The camera for view and projection matrices.</param>
    /// <remarks>
    /// Call this before <see cref="DrawLayerGroup"/> or <see cref="DrawLayer(string)"/>.
    /// Must be paired with <see cref="EndDraw"/>.
    /// Automatically saves GraphicsDevice state for restoration after SpriteBatch usage.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if camera is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no tilemap is loaded or already drawing.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void BeginDraw(OrthographicCamera camera)
    {
        ThrowIfDisposed();
        ThrowIfNoTilemap();

        ArgumentNullException.ThrowIfNull(camera);

        if (_isDrawing)
        {
            throw new InvalidOperationException("BeginDraw already called. Call EndDraw first.");
        }

        _isDrawing = true;
        _camera = camera;

        SaveGraphicsDeviceState();

        _graphicsDevice.SamplerStates[0] = _samplerState;
        _graphicsDevice.BlendState = _blendState;

        IEffectMatrices matrices = (IEffectMatrices)_effect;
        Matrix view = camera.GetViewMatrix();

        // Snap view translation to integer pixels to prevent sub-pixel edge bleeding when
        // LinearClamp is configured. Matches the behavior of TilemapSpriteBatchRenderer.
        view.M41 = MathF.Round(view.M41);
        view.M42 = MathF.Round(view.M42);

        matrices.View = view;
        matrices.Projection = Matrix.CreateOrthographicOffCenter(
            0, _graphicsDevice.Viewport.Width,
            _graphicsDevice.Viewport.Height, 0,
            0, 1);
        matrices.World = Matrix.Identity;
    }

    /// <summary>
    /// Draws a layer group.
    /// </summary>
    /// <param name="groupName">The name of the layer group.</param>
    /// <remarks>
    /// All layers in the group are rendered in a single draw call.
    /// Must be called between <see cref="BeginDraw"/> and <see cref="EndDraw"/>.
    /// If the group is marked dirty, it will be rebuilt before drawing.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if called outside of BeginDraw/EndDraw.</exception>
    /// <exception cref="ArgumentException">Thrown if the group does not exist.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void DrawLayerGroup(string groupName)
    {
        ThrowIfDisposed();
        ThrowIfNotDrawing();

        if (!_layerGroups.TryGetValue(groupName, out LayerGroup group))
        {
            throw new ArgumentException($"Layer group '{groupName}' does not exist.", nameof(groupName));
        }

        if (group.IsDirty)
        {
            RebuildLayerGroupInternal(group);
        }

        foreach (LayerModel model in group.MergedModels)
        {
            ApplyParallaxWorld(model.ParallaxFactor);
            DrawLayerModel(model);
        }
    }

    /// <summary>
    /// Draws a single layer by name.
    /// </summary>
    /// <param name="layerName">The name of the layer.</param>
    /// <remarks>
    /// If the layer is part of a group, only this layer is drawn (not the entire group).
    /// Must be called between <see cref="BeginDraw"/> and <see cref="EndDraw"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if called outside of BeginDraw/EndDraw.</exception>
    /// <exception cref="ArgumentException">Thrown if the layer does not exist.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void DrawLayer(string layerName)
    {
        ThrowIfDisposed();
        ThrowIfNotDrawing();

        DrawLayerInternal(_tilemap.Layers[layerName]);
    }

    /// <summary>
    /// Draws a single layer by index.
    /// </summary>
    /// <param name="layerIndex">The index of the layer.</param>
    /// <remarks>
    /// If the layer is part of a group, only this layer is drawn (not the entire group).
    /// Must be called between <see cref="BeginDraw"/> and <see cref="EndDraw"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if called outside of BeginDraw/EndDraw.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the layer index is out of range.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void DrawLayer(int layerIndex)
    {
        ThrowIfDisposed();
        ThrowIfNotDrawing();

        if (layerIndex < 0 || layerIndex >= _tilemap.Layers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(layerIndex));
        }

        DrawLayerInternal(_tilemap.Layers[layerIndex]);
    }

    /// <summary>
    /// Draws a subset of layers by name, in the order specified.
    /// </summary>
    /// <param name="camera">The camera for view and projection matrices.</param>
    /// <param name="layerNames">The names of the layers to draw.</param>
    /// <remarks>
    /// <para>
    /// This is a convenience wrapper around <see cref="BeginDraw"/>, <see cref="DrawLayer(string)"/>,
    /// and <see cref="EndDraw"/>. Each layer is drawn individually; group merging is bypassed.
    /// </para>
    /// <para>
    /// For interleaving tilemap layers with sprite or entity drawing, use the manual
    /// <see cref="BeginDraw"/> / <see cref="DrawLayer(string)"/> / <see cref="EndDraw"/> path instead.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if camera or layerNames is null.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if any layer name is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no tilemap is loaded.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void DrawLayers(OrthographicCamera camera, params string[] layerNames)
    {
        ThrowIfDisposed();
        ThrowIfNoTilemap();

        ArgumentNullException.ThrowIfNull(camera);
        ArgumentNullException.ThrowIfNull(layerNames);

        BeginDraw(camera);

        foreach (string name in layerNames)
        {
            DrawLayerInternal(_tilemap.Layers[name]);
        }

        EndDraw();
    }

    /// <summary>
    /// Ends a manual rendering sequence.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if BeginDraw was not called.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void EndDraw()
    {
        ThrowIfDisposed();
        ThrowIfNotDrawing();

        _isDrawing = false;
        ((IEffectMatrices)_effect).World = Matrix.Identity;
        _camera = null;

        RestoreGraphicsDeviceState();
    }

    /// <summary>
    /// Draws all layers automatically.
    /// </summary>
    /// <param name="camera">The camera for view and projection matrices.</param>
    /// <remarks>
    /// <para>
    /// Draws all visible layers. Grouped layers are drawn as a group (single draw call).
    /// Ungrouped layers are drawn according to <see cref="DefaultRenderMode"/>.
    /// </para>
    /// <para>
    /// For fine-grained control over layer rendering order (e.g., to inject sprites
    /// between layers), use manual rendering with <see cref="BeginDraw(OrthographicCamera)"/>,
    /// <see cref="DrawLayerGroup"/>, and <see cref="EndDraw"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if camera is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no tilemap is loaded.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Draw(OrthographicCamera camera)
    {
        ThrowIfDisposed();
        ThrowIfNoTilemap();

        ArgumentNullException.ThrowIfNull(camera);

        BeginDraw(camera);

        _drawnGroupsBuffer.Clear();

        if (_defaultRenderMode == RenderMode.Merged)
        {
            if (_mergedUngroupedDirty)
            {
                BuildMergedUngroupedModels();
            }

            // Find the last ungrouped tile layer so image/object layers that follow it in
            // z-order are drawn after the merged buffer, not before.
            TilemapLayerCollection layers = _tilemap.Layers;
            int lastTileLayerIndex = -1;
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] is TilemapTileLayer && !_layerToGroup.ContainsKey(layers[i]))
                {
                    lastTileLayerIndex = i;
                }
            }

            for (int i = 0; i <= lastTileLayerIndex; i++)
            {
                TilemapLayer layer = layers[i];
                if (_layerToGroup.TryGetValue(layer, out string groupName))
                {
                    if (_drawnGroupsBuffer.Add(groupName))
                    {
                        DrawLayerGroup(groupName);
                    }
                }
                else if (layer is TilemapImageLayer || layer is TilemapObjectLayer)
                {
                    DrawLayerInternal(layer);
                }
            }

            foreach (LayerModel model in _mergedUngroupedModels)
            {
                ApplyParallaxWorld(model.ParallaxFactor);
                DrawLayerModel(model);
            }

            for (int i = lastTileLayerIndex + 1; i < layers.Count; i++)
            {
                TilemapLayer layer = layers[i];
                if (_layerToGroup.TryGetValue(layer, out string groupName))
                {
                    if (_drawnGroupsBuffer.Add(groupName))
                    {
                        DrawLayerGroup(groupName);
                    }
                }
                else if (layer is TilemapImageLayer || layer is TilemapObjectLayer)
                {
                    DrawLayerInternal(layer);
                }
            }
        }
        else
        {
            foreach (TilemapLayer layer in _tilemap.Layers)
            {
                if (_layerToGroup.TryGetValue(layer, out string groupName))
                {
                    if (_drawnGroupsBuffer.Add(groupName))
                    {
                        DrawLayerGroup(groupName);
                    }
                }
                else
                {
                    DrawLayerInternal(layer);
                }
            }
        }

        EndDraw();
    }

    /// <summary>
    /// Updates animated tiles.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    /// <remarks>
    /// Must be called each frame to update tile animations.
    /// Automatically marks groups containing animated tiles as dirty when frames change.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if gameTime is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void Update(GameTime gameTime)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(gameTime);

        if (_animatedTiles.Count == 0)
        {
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        bool anyFrameChanged = false;

        for (int i = 0; i < _animatedTiles.Count; i++)
        {
            TilemapTileData tileData = _animatedTiles[i];
            int prevFrame = tileData.Animation.CurrentFrameIndex;
            tileData.Animation.Update(dt);
            if (tileData.Animation.CurrentFrameIndex != prevFrame)
            {
                anyFrameChanged = true;
            }
        }

        if (anyFrameChanged)
        {
            foreach (string groupName in _groupsWithAnimations)
            {
                MarkGroupDirty(groupName);
            }

            if (_mergedUngroupedHasAnimations)
            {
                _mergedUngroupedDirty = true;
            }
        }
    }

    /// <summary>
    /// Saves the current GraphicsDevice state.
    /// </summary>
    /// <remarks>
    /// Call this before using SpriteBatch between <see cref="BeginDraw"/> and <see cref="EndDraw"/>.
    /// Paired with <see cref="RestoreGraphicsDeviceState"/>.
    /// Note: <see cref="BeginDraw"/> automatically saves state, so this is only needed for
    /// additional state changes.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void SaveGraphicsDeviceState()
    {
        ThrowIfDisposed();

        _savedBlendState = _graphicsDevice.BlendState;
        _savedSamplerState = _graphicsDevice.SamplerStates[0];
        _savedRasterizerState = _graphicsDevice.RasterizerState;
        _savedDepthStencilState = _graphicsDevice.DepthStencilState;
    }

    /// <summary>
    /// Restores previously saved GraphicsDevice state.
    /// </summary>
    /// <remarks>
    /// Call this after using SpriteBatch between <see cref="BeginDraw"/> and <see cref="EndDraw"/>.
    /// Paired with <see cref="SaveGraphicsDeviceState"/>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if this renderer has been disposed.</exception>
    public void RestoreGraphicsDeviceState()
    {
        ThrowIfDisposed();

        _graphicsDevice.BlendState = _savedBlendState;
        _graphicsDevice.SamplerStates[0] = _savedSamplerState;
        _graphicsDevice.RasterizerState = _savedRasterizerState;
        _graphicsDevice.DepthStencilState = _savedDepthStencilState;
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

        UnloadTilemap();
        _defaultEffect?.Dispose();
        _isDisposed = true;
    }

    private void BuildLayerModels()
    {
        foreach (TilemapLayer layer in _tilemap.Layers)
        {
            if (layer is TilemapTileLayer tileLayer)
            {
                List<LayerModel> models = BuildLayerModels(tileLayer);
                if (models.Count > 0)
                {
                    _layerModels[tileLayer] = models;
                }
            }
            else if (layer is TilemapImageLayer imageLayer)
            {
                if (imageLayer.RepeatX || imageLayer.RepeatY)
                {
                    RepeatImageLayerModel model = BuildRepeatImageLayerModel(imageLayer);
                    if (model != null)
                    {
                        _repeatImageLayerModels[imageLayer] = model;
                    }
                }
                else
                {
                    LayerModel model = BuildImageLayerModel(imageLayer);
                    if (model != null)
                    {
                        _imageLayerModels[imageLayer] = model;
                    }
                }
            }
            else if (layer is TilemapObjectLayer objectLayer)
            {
                List<LayerModel> models = BuildObjectLayerModels(objectLayer);
                if (models.Count > 0)
                {
                    _objectLayerModels[objectLayer] = models;
                }
            }
        }

        _mergedUngroupedDirty = true;
        BuildAnimatedTilesList();
        UpdateAnimationSets();
    }

    private LayerModel BuildImageLayerModel(TilemapImageLayer imageLayer)
    {
        if (imageLayer.Texture == null)
        {
            return null;
        }

        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        List<int> indices = new List<int>();

        Vector2 position = imageLayer.Position + imageLayer.Offset;
        Rectangle sourceRect = new Rectangle(0, 0, imageLayer.Texture.Width, imageLayer.Texture.Height);
        Color color = new Color(1f, 1f, 1f, imageLayer.Opacity);

        TilemapRendererShared.AddTileQuad(vertices, indices, position, imageLayer.Texture.Width, imageLayer.Texture.Height,
            sourceRect, TilemapTileFlipFlags.None, imageLayer.Texture, color);

        LayerModel model = TilemapRendererShared.CreateLayerModel(_graphicsDevice, vertices.ToArray(), indices.ToArray(), imageLayer.Texture);
        model.ParallaxFactor = imageLayer.ParallaxFactor;
        return model;
    }

    private RepeatImageLayerModel BuildRepeatImageLayerModel(TilemapImageLayer imageLayer)
    {
        if (imageLayer.Texture == null)
        {
            return null;
        }

        DynamicVertexBuffer dynVB = new DynamicVertexBuffer(
            _graphicsDevice,
            typeof(VertexPositionColorTexture),
            4,
            BufferUsage.WriteOnly);
        dynVB.SetData(new VertexPositionColorTexture[4]);

        IndexBuffer ib = new IndexBuffer(
            _graphicsDevice,
            IndexElementSize.SixteenBits,
            6,
            BufferUsage.WriteOnly);
        ib.SetData(new ushort[] { 0, 1, 2, 1, 3, 2 });

        return new RepeatImageLayerModel
        {
            VertexBuffer = dynVB,
            IndexBuffer = ib,
            Texture = imageLayer.Texture,
            RepeatX = imageLayer.RepeatX,
            RepeatY = imageLayer.RepeatY,
            LayerPosition = imageLayer.Position + imageLayer.Offset,
            Color = new Color(1f, 1f, 1f, imageLayer.Opacity)
        };
    }

    /// <remarks>
    /// Only <see cref="TilemapTileObject"/> instances are rendered; other object types are skipped.
    /// Objects are batched by texture to minimize draw calls. A new batch is opened whenever the
    /// texture changes, preserving back-to-front draw order within the layer.
    /// Tile object positions follow the Tiled convention: the position is at the bottom-left corner,
    /// and rotation is clockwise in screen (Y-down) space. Vertex positions are pre-computed at
    /// load time so rotation is fully resolved in the GPU buffer.
    /// </remarks>
    private List<LayerModel> BuildObjectLayerModels(TilemapObjectLayer objectLayer)
    {
        List<LayerModel> models = new List<LayerModel>();
        List<VertexPositionColorTexture> currentVertices = new List<VertexPositionColorTexture>();
        List<int> currentIndices = new List<int>();
        Texture2D currentTexture = null;

        Color layerColor = new Color(1f, 1f, 1f, objectLayer.Opacity);

        foreach (TilemapObject obj in objectLayer.Objects)
        {
            if (!obj.IsVisible)
            {
                continue;
            }

            if (obj is not TilemapTileObject tileObj)
            {
                continue;
            }

            int localId = tileObj.Tile.GetLocalId(_tilemap.Tilesets, out TilemapTileset tileset);

            if (tileset == null)
            {
                continue;
            }

            tileset.GetRenderSource(localId, out Texture2D texture, out Rectangle sourceRect);

            if (texture == null)
            {
                continue;
            }

            if (currentTexture != null && currentTexture != texture)
            {
                LayerModel flushed = TilemapRendererShared.CreateLayerModel(_graphicsDevice, currentVertices.ToArray(), currentIndices.ToArray(), currentTexture);
                flushed.ParallaxFactor = objectLayer.ParallaxFactor;
                models.Add(flushed);
                currentVertices.Clear();
                currentIndices.Clear();
            }

            currentTexture = texture;

            AddObjectTileQuad(
                currentVertices, currentIndices,
                tileObj.Position, tileObj.Size.X, tileObj.Size.Y,
                tileObj.Rotation, sourceRect, tileObj.Tile.FlipFlags,
                texture, layerColor);
        }

        if (currentVertices.Count > 0)
        {
            LayerModel last = TilemapRendererShared.CreateLayerModel(_graphicsDevice, currentVertices.ToArray(), currentIndices.ToArray(), currentTexture);
            last.ParallaxFactor = objectLayer.ParallaxFactor;
            models.Add(last);
        }

        return models;
    }

    private void AddObjectTileQuad(List<VertexPositionColorTexture> vertices, List<int> indices, Vector2 pivot, float width, float height, float rotation, Rectangle sourceRect, TilemapTileFlipFlags flipFlags, Texture2D texture, Color color)
    {
        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);

        // Corners relative to the bottom-left pivot before rotation.
        // In Y-down space the top edge is at y = -height.
        // Clockwise rotation: rotate(vx, vy) = (vx*cos - vy*sin, vx*sin + vy*cos)
        Vector3 topLeft = new Vector3(pivot.X + height * sin, pivot.Y - height * cos, 0);
        Vector3 topRight = new Vector3(pivot.X + width * cos + height * sin, pivot.Y + width * sin - height * cos, 0);
        Vector3 bottomLeft = new Vector3(pivot.X, pivot.Y, 0);
        Vector3 bottomRight = new Vector3(pivot.X + width * cos, pivot.Y + width * sin, 0);

        Vector2[] uvs = TilemapRendererShared.CalculateTextureCoordinates(sourceRect, flipFlags, texture);

        int vertexOffset = vertices.Count;
        vertices.Add(new VertexPositionColorTexture(topLeft, color, uvs[0]));
        vertices.Add(new VertexPositionColorTexture(topRight, color, uvs[1]));
        vertices.Add(new VertexPositionColorTexture(bottomLeft, color, uvs[2]));
        vertices.Add(new VertexPositionColorTexture(bottomRight, color, uvs[3]));

        indices.Add(vertexOffset);
        indices.Add(vertexOffset + 1);
        indices.Add(vertexOffset + 2);
        indices.Add(vertexOffset + 1);
        indices.Add(vertexOffset + 3);
        indices.Add(vertexOffset + 2);
    }

    private List<LayerModel> BuildLayerModels(TilemapTileLayer tileLayer)
    {
        List<TileBatch> batches = new List<TileBatch>();
        Color layerColor = new Color(1f, 1f, 1f, tileLayer.Opacity);
        Vector2 layerParallax = tileLayer.ParallaxFactor;

        foreach (TilemapTileEntry entry in tileLayer.GetTiles())
        {
            int localId = entry.Tile.GetLocalId(_tilemap.Tilesets, out TilemapTileset tileset);

            if (tileset == null)
            {
                continue;
            }

            tileset.GetRenderSource(localId, out Texture2D texture, out Rectangle sourceRect);

            if (texture == null)
            {
                continue;
            }

            Point worldPos = _tilemap.TileToWorldPosition(entry.X, entry.Y);
            Vector2 position = new Vector2(worldPos.X, worldPos.Y) + tileLayer.Offset + tileset.TileOffset;
            // Tiled bottom-aligns all tiles: shifts oversized tiles up and undersized tiles down.
            position.Y += tileLayer.TileHeight - sourceRect.Height;

            TileBatch last = batches.Count > 0 ? batches[batches.Count - 1] : null;
            if (last == null || last.Texture != texture || last.ParallaxFactor != layerParallax)
            {
                last = new TileBatch(texture, layerParallax, new List<VertexPositionColorTexture>(), new List<int>());
                batches.Add(last);
            }

            TilemapRendererShared.AddTileQuad(last.Vertices, last.Indices, position,
                sourceRect.Width, sourceRect.Height,
                sourceRect, entry.Tile.FlipFlags, texture, layerColor);
        }

        List<LayerModel> result = new List<LayerModel>();

        foreach (TileBatch batch in batches)
        {
            if (batch.Vertices.Count == 0)
            {
                continue;
            }

            LayerModel model = TilemapRendererShared.CreateLayerModel(_graphicsDevice, batch.Vertices.ToArray(), batch.Indices.ToArray(), batch.Texture);
            model.ParallaxFactor = layerParallax;
            result.Add(model);
        }

        return result;
    }

    private void ApplyParallaxWorld(Vector2 parallaxFactor)
    {
        IEffectMatrices matrices = (IEffectMatrices)_effect;

        if (parallaxFactor == Vector2.One)
        {
            matrices.World = Matrix.Identity;
            return;
        }

        // The viewport top-left in world space is the correct parallax scroll reference,
        // not camera.Position (which is the top-left only at zoom=1).
        RectangleF camBounds = _camera.BoundingRectangle;
        Vector2 tl = new Vector2(camBounds.X, camBounds.Y);
        Vector2 parallaxOrigin = _tilemap.ParallaxOrigin;
        Vector2 offset = (Vector2.One - parallaxFactor) * (tl - parallaxOrigin);
        matrices.World = Matrix.CreateTranslation(offset.X, offset.Y, 0f);
    }

    private void DrawLayerModel(LayerModel model)
    {
        if (model == null)
        {
            return;
        }

        _effect.Parameters["Texture"].SetValue(model.Texture);

        _graphicsDevice.SetVertexBuffer(model.VertexBuffer);
        _graphicsDevice.Indices = model.IndexBuffer;

        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                baseVertex: 0,
                minVertexIndex: 0,
                numVertices: model.VertexBuffer.VertexCount,
                startIndex: 0,
                primitiveCount: model.PrimitiveCount);
        }
    }

    private void DrawRepeatImageLayerModel(RepeatImageLayerModel model)
    {
        SamplerState previous = _graphicsDevice.SamplerStates[0];

        // Repeating image layers use UV coordinates that extend beyond [0, 1], so a wrap
        // sampler is required. Use the wrap equivalent of the user's configured sampler state
        // to preserve their point vs. linear preference.
        _graphicsDevice.SamplerStates[0] = TilemapRendererShared.GetWrapSamplerState(_samplerState);

        _effect.Parameters["Texture"].SetValue(model.Texture);
        _graphicsDevice.SetVertexBuffer(model.VertexBuffer);
        _graphicsDevice.Indices = model.IndexBuffer;

        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                baseVertex: 0,
                minVertexIndex: 0,
                numVertices: model.VertexBuffer.VertexCount,
                startIndex: 0,
                primitiveCount: 2);
        }

        _graphicsDevice.SamplerStates[0] = previous;
    }

    private void DrawLayerInternal(TilemapLayer layer)
    {
        if (!layer.IsVisible)
        {
            return;
        }

        if (layer is TilemapTileLayer tileLayer)
        {
            // Find the corresponding layer models by layer identity, not by index.
            // Index-based lookup breaks when non-tile layers are interleaved with tile layers
            // because _layerModels only contains entries for layers that produced at least one tile.
            if (_layerModels.TryGetValue(tileLayer, out List<LayerModel> tileModels))
            {
                ApplyParallaxWorld(tileLayer.ParallaxFactor);

                foreach (LayerModel tileModel in tileModels)
                {
                    DrawLayerModel(tileModel);
                }
            }

            return;
        }

        if (layer is TilemapImageLayer imageLayer)
        {
            if (_repeatImageLayerModels.TryGetValue(imageLayer, out RepeatImageLayerModel repeatModel))
            {
                IEffectMatrices matrices = (IEffectMatrices)_effect;
                Matrix invViewProj = Matrix.Invert(matrices.View * matrices.Projection);
                Vector2 parallaxOffset = _camera.Position * (Vector2.One - imageLayer.ParallaxFactor);
                repeatModel.UpdateVertices(invViewProj, parallaxOffset);
                ApplyParallaxWorld(imageLayer.ParallaxFactor);
                DrawRepeatImageLayerModel(repeatModel);
            }
            else if (_imageLayerModels.TryGetValue(imageLayer, out LayerModel imageModel))
            {
                ApplyParallaxWorld(imageLayer.ParallaxFactor);
                DrawLayerModel(imageModel);
            }

            return;
        }

        if (layer is TilemapObjectLayer objectLayer)
        {
            if (_objectLayerModels.TryGetValue(objectLayer, out List<LayerModel> objModels))
            {
                foreach (LayerModel model in objModels)
                {
                    ApplyParallaxWorld(model.ParallaxFactor);
                    DrawLayerModel(model);
                }
            }
        }
    }

    private void RebuildLayerGroupInternal(LayerGroup group)
    {
        foreach (LayerModel oldModel in group.MergedModels)
        {
            oldModel.Dispose();
        }
        group.MergedModels.Clear();

        // Accumulate one batch per contiguous run of tiles with the same texture and parallax
        // factor. A new batch is opened on either change to preserve back-to-front draw order
        // and to ensure each batch can be drawn with a single World matrix.
        List<TileBatch> orderedBatches = new List<TileBatch>();

        foreach (TilemapLayer layer in group.Layers)
        {
            if (!layer.IsVisible)
            {
                continue;
            }

            if (layer is not TilemapTileLayer tileLayer)
            {
                continue;
            }

            // Bake per-layer opacity into the vertex color so that layers with
            // different opacity values render correctly within a single merged draw call.
            Color layerColor = new Color(1f, 1f, 1f, tileLayer.Opacity);
            Vector2 layerParallax = layer.ParallaxFactor;

            foreach (TilemapTileEntry entry in tileLayer.GetTiles())
            {
                int localId = entry.Tile.GetLocalId(_tilemap.Tilesets, out TilemapTileset tileset);

                if (tileset == null)
                {
                    continue;
                }

                tileset.GetRenderSource(localId, out Texture2D texture, out Rectangle sourceRect);

                if (texture == null)
                {
                    continue;
                }

                Point worldPos = _tilemap.TileToWorldPosition(entry.X, entry.Y);
                Vector2 position = new Vector2(worldPos.X, worldPos.Y) + tileLayer.Offset + tileset.TileOffset;
                // Tiled bottom-aligns all tiles: shifts oversized tiles up and undersized tiles down.
                position.Y += tileLayer.TileHeight - sourceRect.Height;

                TileBatch last = orderedBatches.Count > 0 ? orderedBatches[orderedBatches.Count - 1] : null;
                if (last == null || last.Texture != texture || last.ParallaxFactor != layerParallax)
                {
                    last = new TileBatch(texture, layerParallax, new List<VertexPositionColorTexture>(), new List<int>());
                    orderedBatches.Add(last);
                }

                TilemapRendererShared.AddTileQuad(last.Vertices, last.Indices, position,
                    sourceRect.Width, sourceRect.Height,
                    sourceRect, entry.Tile.FlipFlags, texture, layerColor);
            }
        }

        foreach (TileBatch batch in orderedBatches)
        {
            if (batch.Vertices.Count == 0)
            {
                continue;
            }

            LayerModel model = TilemapRendererShared.CreateLayerModel(_graphicsDevice, batch.Vertices.ToArray(), batch.Indices.ToArray(), batch.Texture);
            model.ParallaxFactor = batch.ParallaxFactor;
            group.MergedModels.Add(model);
        }

        group.IsDirty = false;
    }

    private void BuildMergedUngroupedModels()
    {
        foreach (LayerModel oldModel in _mergedUngroupedModels)
        {
            oldModel.Dispose();
        }
        _mergedUngroupedModels.Clear();

        List<TileBatch> batches = new List<TileBatch>();

        foreach (TilemapLayer layer in _tilemap.Layers)
        {
            if (_layerToGroup.ContainsKey(layer) || !layer.IsVisible)
            {
                continue;
            }

            if (layer is not TilemapTileLayer tileLayer)
            {
                continue;
            }

            Color layerColor = new Color(1f, 1f, 1f, tileLayer.Opacity);
            Vector2 layerParallax = layer.ParallaxFactor;

            foreach (TilemapTileEntry entry in tileLayer.GetTiles())
            {
                int localId = entry.Tile.GetLocalId(_tilemap.Tilesets, out TilemapTileset tileset);

                if (tileset == null)
                {
                    continue;
                }

                tileset.GetRenderSource(localId, out Texture2D texture, out Rectangle sourceRect);

                if (texture == null)
                {
                    continue;
                }

                Point worldPos = _tilemap.TileToWorldPosition(entry.X, entry.Y);
                Vector2 position = new Vector2(worldPos.X, worldPos.Y) + tileLayer.Offset + tileset.TileOffset;

                // Tiled bottom-aligns all tiles: shifts oversized tiles up and undersized tiles down.
                position.Y += tileLayer.TileHeight - sourceRect.Height;

                TileBatch last = batches.Count > 0 ? batches[batches.Count - 1] : null;
                if (last == null || last.Texture != texture || last.ParallaxFactor != layerParallax)
                {
                    last = new TileBatch(texture, layerParallax, new List<VertexPositionColorTexture>(), new List<int>());
                    batches.Add(last);
                }

                TilemapRendererShared.AddTileQuad(last.Vertices, last.Indices, position,
                    sourceRect.Width, sourceRect.Height,
                    sourceRect, entry.Tile.FlipFlags, texture, layerColor);
            }
        }

        foreach (TileBatch batch in batches)
        {
            if (batch.Vertices.Count == 0)
            {
                continue;
            }

            LayerModel model = TilemapRendererShared.CreateLayerModel(_graphicsDevice, batch.Vertices.ToArray(), batch.Indices.ToArray(), batch.Texture);
            model.ParallaxFactor = batch.ParallaxFactor;
            _mergedUngroupedModels.Add(model);
        }

        _mergedUngroupedDirty = false;
    }

    private void BuildAnimatedTilesList()
    {
        _animatedTiles.Clear();
        if (_tilemap == null)
        {
            return;
        }

        foreach (TilemapTileset tileset in _tilemap.Tilesets)
        {
            IReadOnlyList<TilemapTileData> animated = tileset.GetAnimatedTiles();
            for (int i = 0; i < animated.Count; i++)
            {
                _animatedTiles.Add(animated[i]);
            }
        }
    }

    private void UpdateAnimationSets()
    {
        _groupsWithAnimations.Clear();
        _mergedUngroupedHasAnimations = false;

        if (_tilemap == null || _animatedTiles.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, LayerGroup> kvp in _layerGroups)
        {
            foreach (TilemapLayer layer in kvp.Value.Layers)
            {
                if (layer is TilemapTileLayer)
                {
                    _groupsWithAnimations.Add(kvp.Key);
                    break;
                }
            }
        }

        foreach (TilemapLayer layer in _tilemap.Layers)
        {
            if (!_layerToGroup.ContainsKey(layer) && layer is TilemapTileLayer)
            {
                _mergedUngroupedHasAnimations = true;
                break;
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(TilemapRenderer));
        }
    }

    private void ThrowIfNoTilemap()
    {
        if (_tilemap == null)
        {
            throw new InvalidOperationException("No tilemap loaded. Call LoadTilemap first.");
        }
    }

    private void ThrowIfNotDrawing()
    {
        if (!_isDrawing)
        {
            throw new InvalidOperationException("Not in drawing state. Call BeginDraw first.");
        }
    }

    private sealed class LayerGroup
    {
        public List<TilemapLayer> Layers { get; set; } = new List<TilemapLayer>();
        public List<LayerModel> MergedModels { get; set; } = new List<LayerModel>();
        public bool IsDirty { get; set; } = true;
    }

    private sealed record TileBatch(
        Texture2D Texture,
        Vector2 ParallaxFactor,
        List<VertexPositionColorTexture> Vertices,
        List<int> Indices
    );

    private sealed class RepeatImageLayerModel : IDisposable
    {
        public DynamicVertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public Texture2D Texture { get; set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public Vector2 LayerPosition { get; set; }
        public Color Color { get; set; }

        private readonly VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4];

        public void UpdateVertices(Matrix inverseViewProjection, Vector2 parallaxOffset)
        {
            int texW = Texture.Width;
            int texH = Texture.Height;

            // Unproject the screen corners to world space. The GPU represents the visible screen
            // as a -1 to +1 range on both axes, with Y inverted (+1 = screen top, -1 = screen bottom).
            // Transforming these fixed corners back to world space gives correct bounds regardless
            // of viewport adapter scale (virtual vs physical resolution).
            Vector4 tlW = Vector4.Transform(new Vector4(-1f, 1f, 0f, 1f), inverseViewProjection);
            Vector4 brW = Vector4.Transform(new Vector4(1f, -1f, 0f, 1f), inverseViewProjection);

            float screenLeft = tlW.X / tlW.W;
            float screenTop = tlW.Y / tlW.W;
            float screenRight = brW.X / brW.W;
            float screenBottom = brW.Y / brW.W;

            float left, right, top, bottom;
            float uLeft, uRight, vTop, vBottom;

            if (RepeatX)
            {
                // Vertex position = unprojected world X minus parallax offset (which is added
                // back by the World matrix in ApplyParallaxWorld before drawing).
                // The vertex position is also the parallax-corrected world X, so UVs use it
                // directly: this makes the texture scroll at the parallax factor's speed.
                left = screenLeft - parallaxOffset.X;
                right = screenRight - parallaxOffset.X;
                uLeft = (left - LayerPosition.X) / texW;
                uRight = (right - LayerPosition.X) / texW;
            }
            else
            {
                left = LayerPosition.X;
                right = LayerPosition.X + texW;
                uLeft = 0f;
                uRight = 1f;
            }

            if (RepeatY)
            {
                top = screenTop - parallaxOffset.Y;
                bottom = screenBottom - parallaxOffset.Y;
                vTop = (top - LayerPosition.Y) / texH;
                vBottom = (bottom - LayerPosition.Y) / texH;
            }
            else
            {
                top = LayerPosition.Y;
                bottom = LayerPosition.Y + texH;
                vTop = 0f;
                vBottom = 1f;
            }

            _vertices[0] = new VertexPositionColorTexture(new Vector3(left, top, 0), Color, new Vector2(uLeft, vTop));
            _vertices[1] = new VertexPositionColorTexture(new Vector3(right, top, 0), Color, new Vector2(uRight, vTop));
            _vertices[2] = new VertexPositionColorTexture(new Vector3(left, bottom, 0), Color, new Vector2(uLeft, vBottom));
            _vertices[3] = new VertexPositionColorTexture(new Vector3(right, bottom, 0), Color, new Vector2(uRight, vBottom));

            VertexBuffer.SetData(_vertices);
        }

        public void Dispose()
        {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
        }
    }
}
