using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps.Rendering;

/// <summary>
/// World-map tilemap renderer using SpriteBatch for simple integration with existing SpriteBatch-based code.
/// </summary>
/// <remarks>
/// <para>
/// Loads a collection of tilemaps and bakes per-tile draw data into world space at load time,
/// grouped by (WorldDepth, parallax factor). At draw time one SpriteBatch.Begin/End pair is
/// issued per unique parallax factor, with room-level and per-tile AABB culling applied within
/// each batch.
/// </para>
/// <para>
/// Animated tiles are supported. Call <see cref="Update"/> each frame to advance animation state.
/// </para>
/// </remarks>
public sealed class TilemapWorldSpriteBatchRenderer
{
    private readonly List<WorldGroup> _worldGroups = new List<WorldGroup>();
    private readonly List<TilemapTileData> _animatedTiles = new List<TilemapTileData>();
    private bool _isLoaded;

    /// <summary>
    /// Gets the number of room batches that passed culling during the most recent Draw call.
    /// </summary>
    /// <remarks>
    /// Updated after each call to <see cref="Draw(SpriteBatch, OrthographicCamera, int)"/>.
    /// Returns zero if Load has not been called or if Draw has not yet been called.
    /// </remarks>
    public int LastVisibleRoomCount { get; private set; }

    /// <summary>
    /// Gets or sets the blend state used when drawing tiles.
    /// </summary>
    /// <remarks>
    /// The default is <see cref="BlendState.NonPremultiplied"/>, which is correct for tileset
    /// textures loaded directly from file where the PNG pixel data has not been premultiplied.
    /// Set this to <see cref="BlendState.AlphaBlend"/> if your textures are loaded through
    /// the MonoGame content pipeline, which premultiplies alpha by default.
    /// </remarks>
    public BlendState BlendState { get; set; } = BlendState.NonPremultiplied;

    /// <summary>
    /// Gets or sets the sampler state used when drawing tiles.
    /// </summary>
    /// <remarks>
    /// The default is <see cref="SamplerState.PointClamp"/>, which is correct for pixel-art
    /// tilesets. It produces clean, hard-edged tiles at any zoom level with no edge bleeding.
    /// </remarks>
    public SamplerState SamplerState { get; set; } = SamplerState.PointClamp;

    /// <summary>
    /// Gets or sets the sprite sort mode used for each SpriteBatch.Begin call.
    /// </summary>
    /// <remarks>
    /// The default is <see cref="SpriteSortMode.Deferred"/>, which preserves draw order within
    /// a parallax group. <see cref="SpriteSortMode.Texture"/> can reduce GPU state changes for
    /// worlds that reference tiles from multiple tilesets, at the cost of rendering order guarantees.
    /// </remarks>
    public SpriteSortMode SpriteSortMode { get; set; } = SpriteSortMode.Deferred;

    /// <summary>
    /// Gets or sets the custom shader effect applied when drawing tiles, or null to use the default.
    /// </summary>
    public Effect Effect { get; set; }

    /// <summary>
    /// Loads a collection of tilemaps for world-map rendering. All tile positions are baked into
    /// world space at load time, grouped by (WorldDepth, parallax factor).
    /// </summary>
    /// <param name="tilemaps">The tilemaps that make up the world.</param>
    /// <exception cref="ArgumentNullException">Thrown if tilemaps is null.</exception>
    public void Load(IEnumerable<Tilemap> tilemaps)
    {
        ArgumentNullException.ThrowIfNull(tilemaps);

        Unload();

        Dictionary<WorldGroupKey, WorldGroup> groupIndex = new Dictionary<WorldGroupKey, WorldGroup>();
        Dictionary<WorldRoomKey, WorldRoomBatch> roomIndex = new Dictionary<WorldRoomKey, WorldRoomBatch>();
        HashSet<TilemapTileData> animatedSet = new HashSet<TilemapTileData>();

        foreach (Tilemap tilemap in tilemaps)
        {
            int depth = tilemap.WorldDepth;
            Vector2 worldPos = tilemap.WorldPosition;
            RectangleF roomBounds = new RectangleF(
                worldPos.X, worldPos.Y,
                tilemap.Width * tilemap.TileWidth,
                tilemap.Height * tilemap.TileHeight);

            foreach (TilemapLayer layer in tilemap.Layers)
            {
                if (layer is not TilemapTileLayer tileLayer || !layer.IsVisible)
                {
                    continue;
                }

                Vector2 parallax = tileLayer.ParallaxFactor;
                WorldGroupKey groupKey = new WorldGroupKey(depth, parallax);
                WorldRoomKey roomKey = new WorldRoomKey(depth, parallax, tilemap);

                if (!groupIndex.TryGetValue(groupKey, out WorldGroup group))
                {
                    group = new WorldGroup { WorldDepth = depth, ParallaxFactor = parallax };
                    groupIndex[groupKey] = group;
                    _worldGroups.Add(group);
                }

                if (!roomIndex.TryGetValue(roomKey, out WorldRoomBatch roomBatch))
                {
                    roomBatch = new WorldRoomBatch { WorldBounds = roomBounds };
                    roomIndex[roomKey] = roomBatch;
                    group.Rooms.Add(roomBatch);
                }

                Color layerColor = layer.TintColor.HasValue
                    ? layer.TintColor.Value * layer.Opacity
                    : Color.White * layer.Opacity;

                foreach (TilemapTileEntry entry in tileLayer.GetTiles())
                {
                    int localId = entry.Tile.GetLocalId(tilemap.Tilesets, out TilemapTileset tileset);
                    if (tileset == null)
                    {
                        continue;
                    }

                    TilemapTileData tileData = tileset.GetTileData(localId);
                    TilemapTileData animatedData = null;

                    if (tileData?.Animation != null)
                    {
                        animatedData = tileData;
                        if (animatedSet.Add(tileData))
                        {
                            _animatedTiles.Add(tileData);
                        }
                    }

                    tileset.GetRenderSource(localId, out Texture2D tileTexture, out Rectangle sourceRect);

                    if (tileTexture == null)
                    {
                        continue;
                    }

                    Point tilePos = tilemap.TileToWorldPosition(entry.X, entry.Y);
                    Vector2 drawPosition = new Vector2(tilePos.X, tilePos.Y) + worldPos + tileLayer.Offset + tileset.TileOffset;
                    // Tiled bottom-aligns all tiles: shifts oversized tiles up and undersized tiles down.
                    drawPosition.Y += tileLayer.TileHeight - sourceRect.Height;

                    roomBatch.Tiles.Add(new WorldTileSprite(
                        tileTexture, sourceRect, animatedData, tileset,
                        drawPosition, entry.Tile.FlipFlags,
                        sourceRect.Width, sourceRect.Height, layerColor));
                }
            }
        }

        _isLoaded = true;
    }

    /// <summary>
    /// Loads a <see cref="TilemapWorld"/> for world-map rendering.
    /// </summary>
    /// <param name="world">The tilemap world whose levels will be rendered.</param>
    /// <exception cref="ArgumentNullException">Thrown if world is null.</exception>
    public void Load(TilemapWorld world)
    {
        ArgumentNullException.ThrowIfNull(world);
        Load(world.Levels);
    }

    /// <summary>
    /// Unloads all world data.
    /// </summary>
    public void Unload()
    {
        _worldGroups.Clear();
        _animatedTiles.Clear();
        _isLoaded = false;
    }

    /// <summary>
    /// Updates animated tiles.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    /// <exception cref="ArgumentNullException">Thrown if gameTime is null.</exception>
    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        if (_animatedTiles.Count == 0)
        {
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        for (int i = 0; i < _animatedTiles.Count; i++)
        {
            _animatedTiles[i].Animation.Update(dt);
        }
    }

    /// <summary>
    /// Draws all tilemaps at depth 0.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw with.</param>
    /// <param name="camera">The camera providing the view transform and visible bounds.</param>
    /// <exception cref="ArgumentNullException">Thrown if spriteBatch or camera is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if Load has not been called.</exception>
    public void Draw(SpriteBatch spriteBatch, OrthographicCamera camera)
    {
        Draw(spriteBatch, camera, 0);
    }

    /// <summary>
    /// Draws all tilemaps at the specified world depth. One SpriteBatch.Begin/End pair is issued
    /// per unique parallax factor; all rooms sharing a parallax factor are drawn in that single
    /// batch. Per-room and per-tile culling against the camera bounds is applied within each batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw with.</param>
    /// <param name="camera">The camera providing the view transform and visible bounds.</param>
    /// <param name="worldDepth">The world depth layer to draw.</param>
    /// <exception cref="ArgumentNullException">Thrown if spriteBatch or camera is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if Load has not been called.</exception>
    public void Draw(SpriteBatch spriteBatch, OrthographicCamera camera, int worldDepth)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        ArgumentNullException.ThrowIfNull(camera);

        if (!_isLoaded)
        {
            throw new InvalidOperationException("No world loaded. Call Load first.");
        }

        RectangleF camBounds = camera.BoundingRectangle;

        Matrix view = camera.GetViewMatrix();
        view.M41 = MathF.Round(view.M41);
        view.M42 = MathF.Round(view.M42);

        int visibleRooms = 0;

        foreach (WorldGroup group in _worldGroups)
        {
            if (group.WorldDepth != worldDepth)
            {
                continue;
            }

            Vector2 parallax = group.ParallaxFactor;

            // Compute the effective visible world region for this parallax factor.
            // With parallax origin at (0,0): worldLeft = camBounds.X * parallax.X
            float cullLeft = camBounds.X * parallax.X;
            float cullTop = camBounds.Y * parallax.Y;
            float cullRight = cullLeft + camBounds.Width;
            float cullBottom = cullTop + camBounds.Height;

            Matrix batchView = view;
            if (parallax != Vector2.One)
            {
                Vector2 tl = new Vector2(camBounds.X, camBounds.Y);
                Vector2 offset = (Vector2.One - parallax) * tl;
                batchView = Matrix.CreateTranslation(offset.X, offset.Y, 0f) * view;
            }

            spriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, null, null, Effect, batchView);

            foreach (WorldRoomBatch room in group.Rooms)
            {
                RectangleF b = room.WorldBounds;
                if (b.Right <= cullLeft || b.Left >= cullRight ||
                    b.Bottom <= cullTop || b.Top >= cullBottom)
                {
                    continue;
                }

                visibleRooms++;

                foreach (WorldTileSprite tile in room.Tiles)
                {
                    Vector2 pos = tile.WorldPosition;
                    if (pos.X + tile.TileWidth <= cullLeft || pos.X >= cullRight ||
                        pos.Y + tile.TileHeight <= cullTop || pos.Y >= cullBottom)
                    {
                        continue;
                    }

                    Texture2D drawTexture;
                    Rectangle src;

                    if (tile.AnimatedTileData?.Animation != null)
                    {
                        tile.Tileset.GetRenderSource(tile.AnimatedTileData.LocalId, out drawTexture, out src);

                        if (drawTexture == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        drawTexture = tile.Texture;
                        src = tile.SourceRect;
                    }

                    DrawTile(spriteBatch, drawTexture, pos, src,
                             tile.FlipFlags, src.Width, src.Height, tile.Color);
                }
            }

            spriteBatch.End();
        }

        LastVisibleRoomCount = visibleRooms;
    }

    private static void DrawTile(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle sourceRect, TilemapTileFlipFlags flipFlags, int tileWidth, int tileHeight, Color color)
    {
        bool flipH = (flipFlags & TilemapTileFlipFlags.FlipHorizontally) != 0;
        bool flipV = (flipFlags & TilemapTileFlipFlags.FlipVertically) != 0;
        bool flipD = (flipFlags & TilemapTileFlipFlags.FlipDiagonally) != 0;

        Vector2 scale = new Vector2(
            (float)tileWidth / sourceRect.Width,
            (float)tileHeight / sourceRect.Height);

        if (!flipD)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (flipH)
            {
                effects |= SpriteEffects.FlipHorizontally;
            }

            if (flipV)
            {
                effects |= SpriteEffects.FlipVertically;
            }

            spriteBatch.Draw(texture, position, sourceRect, color, 0f, Vector2.Zero, scale, effects, 0f);
        }
        else
        {
            // Diagonal flip encodes 90-degree rotations per Tiled's convention.
            float rotation;
            SpriteEffects effects;

            if (!flipH && flipV)
            {
                rotation = MathHelper.PiOver2;
                effects = SpriteEffects.None;
            }
            else if (flipH && !flipV)
            {
                rotation = -MathHelper.PiOver2;
                effects = SpriteEffects.None;
            }
            else if (flipH && flipV)
            {
                rotation = MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                rotation = -MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 origin = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height * 0.5f);
            Vector2 center = position + new Vector2(tileWidth * 0.5f, tileHeight * 0.5f);
            spriteBatch.Draw(texture, center, sourceRect, color, rotation, origin, scale, effects, 0f);
        }
    }

    private readonly record struct WorldGroupKey(int WorldDepth, Vector2 ParallaxFactor);
    private readonly record struct WorldRoomKey(int WorldDepth, Vector2 ParallaxFactor, Tilemap Tilemap);

    private readonly struct WorldTileSprite
    {
        public readonly Texture2D Texture;
        public readonly Rectangle SourceRect;
        public readonly TilemapTileData AnimatedTileData;
        public readonly TilemapTileset Tileset;
        public readonly Vector2 WorldPosition;
        public readonly TilemapTileFlipFlags FlipFlags;
        public readonly int TileWidth;
        public readonly int TileHeight;
        public readonly Color Color;

        public WorldTileSprite(
            Texture2D texture, Rectangle sourceRect, TilemapTileData animatedTileData,
            TilemapTileset tileset, Vector2 worldPosition, TilemapTileFlipFlags flipFlags,
            int tileWidth, int tileHeight, Color color)
        {
            Texture = texture;
            SourceRect = sourceRect;
            AnimatedTileData = animatedTileData;
            Tileset = tileset;
            WorldPosition = worldPosition;
            FlipFlags = flipFlags;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Color = color;
        }
    }

    private sealed class WorldRoomBatch
    {
        public RectangleF WorldBounds;
        public readonly List<WorldTileSprite> Tiles = new List<WorldTileSprite>();
    }

    private sealed class WorldGroup
    {
        public int WorldDepth;
        public Vector2 ParallaxFactor;
        public readonly List<WorldRoomBatch> Rooms = new List<WorldRoomBatch>();
    }
}
