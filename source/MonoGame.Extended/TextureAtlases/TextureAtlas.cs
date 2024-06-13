using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    /// <summary>
    ///     Defines a texture atlas which stores a source image and contains regions specifying its sub-images.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Texture atlas (also called a tile map, tile engine, or sprite sheet) is a large image containing a collection,
    ///         or "atlas", of sub-images, each of which is a texture map for some part of a 2D or 3D model.
    ///         The sub-textures can be rendered by modifying the texture coordinates of the object's uvmap on the atlas,
    ///         essentially telling it which part of the image its texture is in.
    ///         In an application where many small textures are used frequently, it is often more efficient to store the
    ///         textures in a texture atlas which is treated as a single unit by the graphics hardware.
    ///         This saves memory and because there are less rendering state changes by binding once, it can be faster to bind
    ///         one large texture once than to bind many smaller textures as they are drawn.
    ///         Careful alignment may be needed to avoid bleeding between sub textures when used with mipmapping, and artefacts
    ///         between tiles for texture compression.
    ///     </para>
    /// </remarks>
    public class TextureAtlas : IEnumerable<TextureRegion>
    {
        /// <summary>
        ///     Initializes a new texture atlas with an empty list of regions.
        /// </summary>
        /// <param name="name">The asset name of this texture atlas</param>
        /// <param name="texture">Source <see cref="Texture2D " /> image used to draw on screen.</param>
        public TextureAtlas(string name, Texture2D texture)
        {
            Name = name;
            Texture = texture;

            _regionsByName = new Dictionary<string, TextureRegion>();
            _regionsByIndex = new List<TextureRegion>();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new texture atlas and populates it with regions.
        /// </summary>
        /// <param name="name">The asset name of this texture atlas</param>
        /// <param name="texture">Source <see cref="!:Texture2D " /> image used to draw on screen.</param>
        /// <param name="regions">A collection of regions to populate the atlas with.</param>
        public TextureAtlas(string name, Texture2D texture, Dictionary<string, Rectangle> regions)
            : this(name, texture)
        {
            foreach (var region in regions)
                CreateRegion(region.Key, region.Value.X, region.Value.Y, region.Value.Width, region.Value.Height);
        }

        private readonly Dictionary<string, TextureRegion> _regionsByName;
        private readonly List<TextureRegion> _regionsByIndex;

        public string Name { get; }

        /// <summary>
        ///     Gets a source <see cref="Texture2D" /> image.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        ///     Gets a list of regions in the <see cref="TextureAtlas" />.
        /// </summary>
        public IEnumerable<TextureRegion> Regions => _regionsByIndex;

        /// <summary>
        ///     Gets the number of regions in the <see cref="TextureAtlas" />.
        /// </summary>
        public int RegionCount => _regionsByIndex.Count;

        public TextureRegion this[string name] => GetRegion(name);
        public TextureRegion this[int index] => GetRegion(index);

        /// <summary>
        ///     Gets the enumerator of the <see cref="TextureAtlas" />' list of regions.
        /// </summary>
        /// <returns>The <see cref="IEnumerator" /> of regions.</returns>
        public IEnumerator<TextureRegion> GetEnumerator()
        {
            return _regionsByIndex.GetEnumerator();
        }

        /// <summary>
        ///     Gets the enumerator of the <see cref="TextureAtlas" />' list of regions.
        /// </summary>
        /// <returns>The <see cref="IEnumerator" /> of regions</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether the texture atlas contains a region
        /// </summary>
        /// <param name="name">Name of the texture region.</param>
        /// <returns></returns>
        public bool ContainsRegion(string name)
        {
            return _regionsByName.ContainsKey(name);
        }

        /// <summary>
        /// Internal method for adding region
        /// </summary>
        /// <param name="region">Texture region.</param>
        private void AddRegion(TextureRegion region)
        {
            _regionsByIndex.Add(region);
            _regionsByName.Add(region.Name, region);
        }

        /// <summary>
        ///     Creates a new texture region and adds it to the list of the <see cref="TextureAtlas" />' regions.
        /// </summary>
        /// <param name="name">Name of the texture region.</param>
        /// <param name="x">X coordinate of the region's top left corner.</param>
        /// <param name="y">Y coordinate of the region's top left corner.</param>
        /// <param name="width">Width of the texture region.</param>
        /// <param name="height">Height of the texture region.</param>
        /// <returns>Created texture region.</returns>
        public TextureRegion CreateRegion(string name, int x, int y, int width, int height)
        {
            if (_regionsByName.ContainsKey(name))
                throw new InvalidOperationException($"Region {name} already exists in the texture atlas");

            var region = new TextureRegion(Texture, name, x, y, width, height);
            AddRegion(region);
            return region;
        }

        /// <summary>
        ///     Removes a texture region from the <see cref="TextureAtlas" />
        /// </summary>
        /// <param name="index">An index of the <see cref="TextureRegion" /> in <see cref="Region" /> to remove</param>
        public void RemoveRegion(int index)
        {
            var region = _regionsByIndex[index];
            _regionsByIndex.RemoveAt(index);

            if(region.Name != null)
                _regionsByName.Remove(region.Name);
        }

        /// <summary>
        ///     Removes a texture region from the <see cref="TextureAtlas" />
        /// </summary>
        /// <param name="name">Name of the <see cref="TextureRegion" /> to remove</param>
        public void RemoveRegion(string name)
        {
            if (_regionsByName.TryGetValue(name, out var region))
            {
                _regionsByName.Remove(name);
				_regionsByIndex.Remove(region);
            }
        }

        /// <summary>
        ///     Gets a <see cref="TextureRegion" /> from the <see cref="TextureAtlas" />' list.
        /// </summary>
        /// <param name="index">An index of the <see cref="TextureRegion" /> in <see cref="Region" /> to get.</param>
        /// <returns>The <see cref="TextureRegion" />.</returns>
        public TextureRegion GetRegion(int index)
        {
            if (index < 0 || index >= _regionsByIndex.Count)
                throw new IndexOutOfRangeException();

            return _regionsByIndex[index];
        }

        /// <summary>
        /// Gets a <see cref="TextureRegion" /> from the <see cref="TextureAtlas" />' list.
        /// </summary>
        /// <param name="name">Name of the <see cref="TextureRegion" /> to get.</param>
        /// <returns>The <see cref="TextureRegion" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// Throw if <paramref name="name"/> is <see langword="null"/> or an empty string.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if this atlas does not contain a texture region with name that matches <paramref name="name"/>.
        /// </exception>
        public TextureRegion GetRegion(string name) => _regionsByName[name];


        /// <summary>
        /// Creates a new <see cref="TextureAtlas" /> and populates it with a grid of <see cref="TextureRegion" />.
        /// </summary>
        /// <param name="name">The name of this texture atlas</param>
        /// <param name="texture">Source <see cref="Texture2D" /> image used to draw on screen</param>
        /// <param name="regionWidth">Width of the <see cref="TextureRegion" />.</param>
        /// <param name="regionHeight">Height of the <see cref="TextureRegion" />.</param>
        /// <param name="maxRegionCount">The number of <see cref="TextureRegion" /> to create.</param>
        /// <param name="margin">Minimum distance of the regions from the border of the source <see cref="Texture2D" /> image.</param>
        /// <param name="spacing">Horizontal and vertical space between regions.</param>
        /// <returns>A created and populated <see cref="TextureAtlas" />.</returns>
        public static TextureAtlas Create(string name, Texture2D texture, int regionWidth, int regionHeight,
            int maxRegionCount = int.MaxValue, int margin = 0, int spacing = 0)
        {
            var textureAtlas = new TextureAtlas(name, texture);
            var count = 0;
            var width = texture.Width - margin;
            var height = texture.Height - margin;
            var xIncrement = regionWidth + spacing;
            var yIncrement = regionHeight + spacing;

            for (var y = margin; y < height; y += yIncrement)
            {
                for (var x = margin; x < width; x += xIncrement)
                {
                    var regionName = $"{texture.Name ?? "region"}{count}";
                    textureAtlas.CreateRegion(regionName, x, y, regionWidth, regionHeight);
                    count++;

                    if (count >= maxRegionCount)
                        return textureAtlas;
                }
            }

            return textureAtlas;
        }
    }
}
