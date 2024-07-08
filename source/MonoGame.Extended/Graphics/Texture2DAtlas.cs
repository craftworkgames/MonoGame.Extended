// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a 2D texture atlas that contains a collection of texture regions.
/// </summary>
/// <remarks>
///     <para>
///         A texture atlas, also known as a tile map, tile engine, or sprite sheet, is a large image that contains a 
///         collection of sub-images, or "textures", each representing a texture map for a specific part of a 2D or 3D model.
///     </para>
///     <para>
///         These sub-textures can be rendered by adjusting the texture coordinates (UV map) to reference the appropriate
///         part of the atlas. This technique allows efficient rendering in applications where many small textures are 
///         frequently used.
///     </para>
///     <para>
///         By storing textures in a single atlas, the graphics hardware treats them as a single unit, which can save memory 
///         and improve performance by reducing the number of rendering state changes. Binding one large texture once is 
///         typically faster than binding multiple smaller textures individually.
///     </para>
///     <para>
///         However, careful alignment is necessary to avoid texture bleeding when using mipmapping, and to prevent artifacts 
///         between tiles when using texture compression.
///     </para>
/// </remarks>
public class Texture2DAtlas : IEnumerable<Texture2DRegion>
{
    private readonly List<Texture2DRegion> _regionsByIndex = new List<Texture2DRegion>();
    private readonly Dictionary<string, Texture2DRegion> _regionsByName = new Dictionary<string, Texture2DRegion>();

    /// <summary>
    /// Gets the name of the texture atlas.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the underlying 2D texture.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// Gets the number of regions in the atlas.
    /// </summary>
    public int RegionCount => _regionsByIndex.Count;

    /// <summary>
    /// Gets the <see cref="Texture2DRegion"/> at the specified index.
    /// </summary>
    /// <param name="index">The index of the texture region.</param>
    /// <returns>The texture region at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the value of the <paramref name="index"/> parameter is less than zero or greater than or equal to
    /// the total number of regions in this atlas.
    /// </exception>
    public Texture2DRegion this[int index] => GetRegion(index);

    /// <summary>
    /// Gets the <see cref="Texture2DRegion"/> with the specified name.
    /// </summary>
    /// <param name="name">The name of the texture region.</param>
    /// <returns>The texture region with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if this atlas does not contain a region with a name that matches the <paramref name="name"/> parameter.
    /// </exception>
    public Texture2DRegion this[string name] => GetRegion(name);

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DAtlas"/> class with the specified texture.
    /// </summary>
    /// <param name="texture">The texture to create the atlas from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="texture"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if <paramref name="texture"/> is disposed.</exception>
    public Texture2DAtlas(Texture2D texture) : this(null, texture) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DAtlas"/> class with the specified name and texture.
    /// </summary>
    /// <param name="name">The name of the texture atlas.</param>
    /// <param name="texture">The texture to create the atlas from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="texture"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if <paramref name="texture"/> is disposed.</exception>
    public Texture2DAtlas(string name, Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        if (texture.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(texture), $"{nameof(texture)} was disposed prior");
        }

        if (string.IsNullOrEmpty(name))
        {
            name = $"{texture.Name}Atlas";
        }

        Name = name;
        Texture = texture;
    }

    /// <summary>
    /// Creates a new texture region and adds it to this atlas.
    /// </summary>
    /// <param name="x">The x-coordinate of the region.</param>
    /// <param name="y">The y-coordinate of the region.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    /// <returns>The created texture region.</returns>
    public Texture2DRegion CreateRegion(int x, int y, int width, int height) => CreateRegion(new Rectangle(x, y, width, height), null);

    /// <summary>
    /// Creates a new texture region with the specified name and adds it to this atlas.
    /// </summary>
    /// <param name="x">The x-coordinate of the region.</param>
    /// <param name="y">The y-coordinate of the region.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    /// <param name="name">The name of the texture region.</param>
    /// <returns>The created texture region.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a region with the same name as the <paramref name="name"/> parameter already exists in this atlas.
    /// </exception>
    public Texture2DRegion CreateRegion(int x, int y, int width, int height, string name) => CreateRegion(new Rectangle(x, y, width, height), name);

    /// <summary>
    /// Creates a new texture region and adds it to this atlas.
    /// </summary>
    /// <param name="location">The location of the region.</param>
    /// <param name="size">The size, in pixels, of the region.</param>
    /// <returns>The created texture region.</returns>
    public Texture2DRegion CreateRegion(Point location, Size size) => CreateRegion(new Rectangle(location.X, location.Y, size.Width, size.Height), null);

    /// <summary>
    /// Creates a new texture region with the specified name and adds it to this atlas.
    /// </summary>
    /// <param name="location">The location of the region.</param>
    /// <param name="size">The size, in pixels, of the region.</param>
    /// <param name="name">The name of the texture region.</param>
    /// <returns>The created texture region.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a region with the same name as the <paramref name="name"/> parameter already exists in this atlas.
    /// </exception>
    public Texture2DRegion CreateRegion(string name, Point location, Size size) => CreateRegion(new Rectangle(location.X, location.Y, size.Width, size.Height), name);

    /// <summary>
    /// Creates a new texture region and adds it to this atlas.
    /// </summary>
    /// <param name="bounds">The bounds of the region.</param>
    /// <returns>The created texture region.</returns>
    public Texture2DRegion CreateRegion(Rectangle bounds) => CreateRegion(bounds, null);

    /// <summary>
    /// Creates a new texture region with the specified name and adds it to this atlas.
    /// </summary>
    /// <param name="bounds">The bounds of the region.</param>
    /// <param name="name">The name of the texture region.</param>
    /// <returns>The created texture region.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a region with the same name as the <paramref name="name"/> parameter already exists in this atlas.
    /// </exception>
    public Texture2DRegion CreateRegion(Rectangle bounds, string name)
    {
        Texture2DRegion region = new Texture2DRegion(Texture, bounds, name);
        AddRegion(region);
        return region;
    }

    /// <summary>
    /// Determines whether the atlas contains a region with the specified name.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>
    /// <see langword="true"/> if the atlas contains a region with the specified name; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool ContainsRegion(string name) => _regionsByName.ContainsKey(name);

    /// <summary>
    /// Gets the index of the region with the specified name.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>The index of the region if found; otherwise, <c>-1</c>.</returns>
    public int GetIndexOfRegion(string name)
    {
        for (int i = 0; i < _regionsByIndex.Count; i++)
        {
            if (_regionsByIndex[i].Name == name)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Gets the region at the specified index.
    /// </summary>
    /// <param name="index">The index of the region.</param>
    /// <returns>The region at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throw if the value of the <paramref name="index"/> is less than zero or is greater than or equal to the total
    /// number of regions in this atlas.
    /// </exception>
    public Texture2DRegion GetRegion(int index) => _regionsByIndex[index];

    /// <summary>
    /// Gets the region with the specified name.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>The region with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if this atlas does not contain a region with a name that matches the <paramref name="name"/> parameter.
    /// </exception>
    public Texture2DRegion GetRegion(string name) => _regionsByName[name];

    /// <summary>
    /// Tries to get the region at the specified index.
    /// </summary>
    /// <param name="index">The index of the region.</param>
    /// <param name="region">
    /// When this method returns, contains the region at the specified index, if the index is found; otherwise,
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the region is found at the specified index; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetRegion(int index, out Texture2DRegion region)
    {
        region = default;

        if (index < 0 || index >= _regionsByIndex.Count)
        {
            return false;
        }

        region = _regionsByIndex[index];
        return true;
    }

    /// <summary>
    /// Tries to get the region with the specified name.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="region">
    /// When this method returns, contains the region with the specified name, if the name is found; otherwise,
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the region is found with the specified name; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetRegion(string name, out Texture2DRegion region) => _regionsByName.TryGetValue(name, out region);

    /// <summary>
    /// Gets the regions at the specified indexes.
    /// </summary>
    /// <param name="indexes">The indexes of the regions to get.</param>
    /// <returns>An array of the regions at the specified indexes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the value of any index in the <paramref name="indexes"/> parameter is less than zero or is greater
    /// than or equal to the total number of regions in this atlas.
    /// </exception>
    public Texture2DRegion[] GetRegions(params int[] indexes)
    {
        Texture2DRegion[] regions = new Texture2DRegion[indexes.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            regions[i] = GetRegion(indexes[i]);
        }

        return regions;
    }
    
    internal Texture2DRegion[] GetRegions(ReadOnlySpan<IAnimationFrame> frames)
    {
        Texture2DRegion[] regions = new Texture2DRegion[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            regions[i] = GetRegion(frames[i].FrameIndex);
        }

        return regions;
    }

    /// <summary>
    /// Gets the regions with the specified names.
    /// </summary>
    /// <param name="names">The names of the regions to get.</param>
    /// <returns>An array of the regions with the specified names.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if a region is not found in this atlas with a name that matches any of the names in the
    /// <paramref name="names"/> parameter.
    /// </exception>
    public Texture2DRegion[] GetRegions(params string[] names)
    {
        Texture2DRegion[] regions = new Texture2DRegion[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            regions[i] = GetRegion(names[i]);
        }

        return regions;
    }

    /// <summary>
    /// Removes the region at the specified index.
    /// </summary>
    /// <param name="index">The index of the region to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the region is successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throw if the value of the <paramref name="index"/> is less than zero or is greater than or equal to the total
    /// number of regions in this atlas.
    /// </exception>
    public bool RemoveRegion(int index)
    {
        if (TryGetRegion(index, out Texture2DRegion region))
        {
            return RemoveRegion(region);
        }

        return false;
    }

    /// <summary>
    /// Removes the region with the specified name.
    /// </summary>
    /// <param name="name">The name of the region to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the region is successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool RemoveRegion(string name)
    {
        if (TryGetRegion(name, out Texture2DRegion region))
        {
            return RemoveRegion(region);
        }

        return false;
    }

    /// <summary>
    /// Removes all regions from the atlas.
    /// </summary>
    public void ClearRegions()
    {
        _regionsByIndex.Clear();
        _regionsByName.Clear();
    }

    private void AddRegion(Texture2DRegion region)
    {
        if (_regionsByName.ContainsKey(region.Name))
        {
            throw new InvalidOperationException($"This {nameof(Texture2DAtlas)} already contains a {nameof(Texture2DRegion)} with the name '{region.Name}'");
        }

        _regionsByIndex.Add(region);
        _regionsByName.Add(region.Name, region);
    }

    /// <summary>
    /// Creates a new <see cref="Sprite"/> using the region from this atlas at the specified index.
    /// </summary>
    /// <param name="regionIndex">The index of the region to use.</param>
    /// <returns>The <see cref="Sprite"/> created using the region at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throw if the value of the <paramref name="regionIndex"/> is less than zero or is greater than or equal to the total
    /// number of regions in this atlas.
    /// </exception>
    public Sprite CreateSprite(int regionIndex)
    {
        Texture2DRegion region = GetRegion(regionIndex);
        return new Sprite(region);
    }

    /// <summary>
    /// Creates a new <see cref="Sprite"/> using the region from this atlas with the specified name.
    /// </summary>
    /// <param name="regionName">The name of the region to use.</param>
    /// <returns>The <see cref="Sprite"/> created using the region with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if this atlas does not contain a region with a name that matches the <paramref name="regionName"/> parameter.
    /// </exception>
    public Sprite CreateSprite(string regionName)
    {
        Texture2DRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    private bool RemoveRegion(Texture2DRegion region) => _regionsByIndex.Remove(region) && _regionsByName.Remove(region.Name);

    /// <summary>
    /// Returns an enumerator that iterates through the collection of texture regions.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<Texture2DRegion> GetEnumerator() => _regionsByIndex.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection of texture regions.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Creates a new <see cref="Texture2DAtlas"/> from the specified texture by dividing it into regions.
    /// </summary>
    /// <param name="name">The name of the texture atlas.</param>
    /// <param name="texture">The source texture to create the atlas from.</param>
    /// <param name="regionWidth">The width, in pixels, of each region.</param>
    /// <param name="regionHeight">The height, in pixels, of each region.</param>
    /// <param name="maxRegionCount">
    /// The maximum number of regions to create. Defaults to <see cref="int.MaxValue"/>.
    /// </param>
    /// <param name="margin">
    /// The margin, in pixels,  to leave around the edges of the texture. Defaults to <c>0</c>.
    /// </param>
    /// <param name="spacing">The spacing, in pixels, between regions. Defaults to <c>0</c>.</param>
    /// <returns>A <see cref="Texture2DAtlas"/> containing the created regions.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="texture"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if <paramref name="texture"/> is disposed.</exception>
    public static Texture2DAtlas Create(string name, Texture2D texture, int regionWidth, int regionHeight,
    int maxRegionCount = int.MaxValue, int margin = 0, int spacing = 0)
    {
        var textureAtlas = new Texture2DAtlas(name, texture);
        var count = 0;
        var width = texture.Width - margin;
        var height = texture.Height - margin;
        var xIncrement = regionWidth + spacing;
        var yIncrement = regionHeight + spacing;

        var columns = (width - margin + spacing) / xIncrement;
        var rows = (height - margin + spacing) / yIncrement;
        var totalRegions = columns * rows;

        for (var i = 0; i < totalRegions; i++)
        {
            var x = margin + (i % columns) * xIncrement;
            var y = margin + (i / columns) * yIncrement;

            if (x >= width || y >= height)
                break;

            textureAtlas.CreateRegion(x, y, regionWidth, regionHeight);
            count++;

            if (count >= maxRegionCount)
                return textureAtlas;
        }

        return textureAtlas;
    }

}
