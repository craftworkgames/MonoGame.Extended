// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content.BitmapFonts;
using MonoGame.Extended.Content.TexturePacker;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Content;

/// <summary>
/// Extends the <see cref="ContentManager"/> to provide additional loading functionality and additional asset types
/// </summary>
public class ExtendedContentManager : ContentManager
{
    private List<IDisposable> _disposableAssets;
    private readonly IGraphicsDeviceService _graphicsDeviceService;

    /// <summary>
    /// Gets the collection of loaded disposable assets that will be disposed when this content manager is disposed.
    /// </summary>
    public List<IDisposable> DisposeableAssets
    {
        get
        {
            if (_disposableAssets is null)
            {
                //  MonoGame please make this protected so subclass have access plz
                FieldInfo field = typeof(ContentManager).GetField(nameof(_disposableAssets), BindingFlags.NonPublic | BindingFlags.Instance);
                if (field is null)
                {
                    throw new InvalidOperationException("Unable to get source disposable assets field");
                }
                _disposableAssets = field.GetValue(this) as List<IDisposable>;
            }

            return _disposableAssets;
        }
    }


#if KNI || FNA
    private Dictionary<string, object> _loadedAssets;

    /// <summary>
    /// Gets the dictionary of loaded assets keyed by asset name.
    /// </summary>
    public Dictionary<string, object> LoadedAssets
    {
        get
        {
            if(_loadedAssets is null)
            {
                //  KNI please make this public so I don't have to use reflection
                FieldInfo field = typeof(ContentManager).GetField(nameof(_loadedAssets), BindingFlags.NonPublic | BindingFlags.Instance);
                if (field is null)
                {
                    throw new InvalidOperationException("Unable to get source loaded assets field");
                }
                _loadedAssets = field.GetValue(this) as Dictionary<string, object>;
            }

            return _loadedAssets;
        }
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedContentManager"/> class.
    /// </summary>
    /// <remarks>
    /// By default, the content manager searches for content in the directory where the executable is located.
    /// </remarks>
    /// <param name="serviceProvider">The service provided used for resolving services.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceProvider"/> is <see langword="null"/>.
    /// </exception>
    public ExtendedContentManager(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _graphicsDeviceService = serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedContentManager"/> class with a specified root directory.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use for resolving services.</param>
    /// <param name="rootDirectory">The root directory the content manager will search for content in.</param>
    public ExtendedContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
    {
        _graphicsDeviceService = serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
    }

#if KNI || FNA
    /// <summary>
    /// Loads a <see cref="Texture2D"/> asset.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="path"/> parameter is a relative path, it must be relative to the
    /// <see cref="ContentManager.RootDirectory"/> path.
    /// </remarks>
    /// <param name="path">The path to the asset to load</param>
    /// <param name="premultiplyAlpha">
    /// Specifies whether the color data of the texture should be premultiplied by its alpha value.
    /// </param>
    /// <returns>The <see cref="Texture2D"/> loaded. </returns>
    public Texture2D LoadTexture2D(string path)
    {
        if (TryGetCachedAsset<Texture2D>(path, out Texture2D texture))
        {
            return texture;
        }

        if (NoExtension(path))
        {
            return Load<Texture2D>(path);
        }

        using Stream stream = GetStream(path);
        texture = Texture2D.FromStream(_graphicsDeviceService.GraphicsDevice, stream);
        texture.Name = path;
        CacheAsset(path, texture);
        return texture;
    }

#else
    /// <summary>
    /// Loads a <see cref="Texture2D"/> asset.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="path"/> parameter is a relative path, it must be relative to the
    /// <see cref="ContentManager.RootDirectory"/> path.
    /// </remarks>
    /// <param name="path">The path to the asset to load</param>
    /// <returns>The <see cref="Texture2D"/> loaded.</returns>
    public Texture2D LoadTexture2D(string path) => LoadTexture2D(path, true);

    /// <summary>
    /// Loads a <see cref="Texture2D"/> asset.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="path"/> parameter is a relative path, it must be relative to the
    /// <see cref="ContentManager.RootDirectory"/> path.
    /// </remarks>
    /// <param name="path">The path to the asset to load</param>
    /// <param name="premultiplyAlpha">
    /// Specifies whether the color data of the texture should be premultiplied by its alpha value.
    /// </param>
    /// <returns>The <see cref="Texture2D"/> loaded.</returns>
    public Texture2D LoadTexture2D(string path, bool premultiplyAlpha)
    {
        if (TryGetCachedAsset<Texture2D>(path, out Texture2D texture))
        {
            return texture;
        }

        if (NoExtension(path))
        {
            return Load<Texture2D>(path);
        }

        using Stream stream = GetStream(path);
        texture = premultiplyAlpha
            ? Texture2D.FromStream(_graphicsDeviceService.GraphicsDevice, stream)
            : Texture2D.FromStream(_graphicsDeviceService.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
        texture.Name = path;
        CacheAsset(path, texture);
        return texture;
    }
#endif

    /// <summary>
    /// Loads a <see cref="SoundEffect"/> asset.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="path"/> parameter is a relative path, it must be relative to the
    /// <see cref="ContentManager.RootDirectory"/> path.
    /// </remarks>
    /// <param name="path">The path to the asset to load</param>
    /// <returns>The <see cref="SoundEffect"/> loaded.</returns>
    public SoundEffect LoadSoundEffect(string path)
    {
        if (TryGetCachedAsset<SoundEffect>(path, out SoundEffect soundEffect))
        {
            return soundEffect;
        }

        if (NoExtension(path))
        {
            return Load<SoundEffect>(path);
        }

        using Stream stream = GetStream(path);
        soundEffect = SoundEffect.FromStream(stream);
        soundEffect.Name = path;
        CacheAsset(path, soundEffect);
        return soundEffect;
    }

    /// <summary>
    /// Loads a <see cref="BitmapFont"/> asset.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="path"/> parameter is a relative path, it must be relative to the
    /// <see cref="ContentManager.RootDirectory"/> path.
    /// </remarks>
    /// <param name="path">The path to the asset to load.</param>
    /// <returns>The <see cref="BitmapFont"/> loaded.</returns>
    public BitmapFont LoadBitmapFont(string path)
    {
        if (TryGetCachedAsset<BitmapFont>(path, out BitmapFont font))
        {
            return font;
        }

        if (NoExtension(path))
        {
            return Load<BitmapFont>(path);
        }

        using FileStream stream = GetStream(path);
        var bmfFile = BitmapFontFileReader.Read(stream);

        var textures =
            bmfFile.Pages.Select(page => LoadTexture2D(Path.GetRelativePath(path, page)))
            .ToArray();

        var characters = new Dictionary<int, BitmapFontCharacter>();
        foreach (var charBlock in bmfFile.Characters)
        {
            var texture = textures[charBlock.Page];
            var region = new Texture2DRegion(texture, charBlock.X, charBlock.Y, charBlock.Width, charBlock.Height);
            var character = new BitmapFontCharacter((int)charBlock.ID, region, charBlock.XOffset, charBlock.YOffset, charBlock.XAdvance);
            characters.Add(character.Character, character);
        }

        foreach (var kerningBlock in bmfFile.Kernings)
        {
            if (characters.TryGetValue((int)kerningBlock.First, out var character))
            {
                character.Kernings.Add((int)kerningBlock.Second, kerningBlock.Amount);
            }
        }

        var bmfFont = new BitmapFont(bmfFile.FontName, bmfFile.Info.FontSize, bmfFile.Common.LineHeight, characters.Values);
        CacheAsset(path, font);
        return font;
    }

#if KNI || FNA
    /// <summary>
    /// Loads a <see cref="Texture2DAtlas"/> from a TexturePacker JSON file.
    /// <summary>
    /// <param name="path">The path to the TexturePacker JSON file</param>
    /// <returns>The <see cref="Texture2DAtlas"/> created from the TexturePacker JSON file content.</returns>
    public Texture2DAtlas LoadTexturePacker(string path)
#else
    /// <summary>
    /// Loads a <see cref="Texture2DAtlas"/> from a TexturePacker JSON file.
    /// </summary>
    /// <param name="path">The path to the TexturePacker JSON file</param>
    /// <param name="premultiplyAlpha">
    /// Specifies whether the color data of the texture should be premultiplied by its alpha value.
    /// </param>
    /// <returns>The <see cref="Texture2DAtlas"/> created from the TexturePacker JSON file content.</returns>
    public Texture2DAtlas LoadTexturePacker(string path, bool premultiplyAlpha)
#endif
    {
        if (TryGetCachedAsset<Texture2DAtlas>(path, out var atlas))
        {
            return atlas;
        }

        if (NoExtension(path))
        {
            return Load<Texture2DAtlas>(path);
        }

        using var stream = GetStream(path);

        var tpFile = TexturePackerFileReader.Read(stream);
        var dir = Path.GetDirectoryName(path);
        var imageAssetPath = Path.Combine(dir, tpFile.Meta.Image);

#if KNI || FNA
        var texture = LoadTexture2D(imageAssetPath);
#else
        var texture = LoadTexture2D(imageAssetPath, premultiplyAlpha);
#endif
        atlas = new Texture2DAtlas(Path.GetFileNameWithoutExtension(tpFile.Meta.Image), texture);

        foreach (var region in tpFile.Regions)
        {
            var frame = region.Frame;
            atlas.CreateRegion(frame.X, frame.Y, frame.Width, frame.Height, Path.GetFileNameWithoutExtension(region.FileName));
        }

        CacheAsset(path, atlas);
        return atlas;
    }

    /// <summary>
    /// Opens a file stream for the specified asset path.
    /// </summary>
    /// <remarks>
    /// The <paramref name="path"/> argument can be either an absolute or relative path.  When it is a relative path, it
    /// is resolved using <see cref="TitleContainer"/>.
    /// </remarks>
    /// <param name="path">The path to the asset file.</param>
    /// <returns>A <see cref="FileStream"/> for reading the asset file.</returns>
    protected FileStream GetStream(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return File.OpenRead(path);
        }

        return (FileStream)TitleContainer.OpenStream(path);
    }

    /// <summary>
    /// Caches a loaded asset in the loaded assets dictionary/
    /// </summary>
    /// <remarks>
    /// Assets that implement <see cref="IDisposable"/> are registered with the <see cref="DisposeableAssets"/>
    /// collection for automatic disposal.
    /// </remarks>
    /// <param name="name">The cache key for the asset, typically the asset path.</param>
    /// <param name="obj">The asset instance to cache.</param>
    protected void CacheAsset(string name, object obj)
    {
        LoadedAssets.Add(name, obj);
        if (obj is IDisposable disposable)
        {
            DisposeableAssets.Add(disposable);
        }
    }

    /// <summary>
    /// Determines whether the specified asset name has no file extension.
    /// </summary>
    /// <param name="name">The asset name to check.</param>
    /// <returns>
    /// <see langword="true"/> if the name has no extension or the extension is empty; otherwise, <see langword="false"/>.
    /// </returns>
    protected bool NoExtension(string name) => string.IsNullOrEmpty(Path.GetExtension(name));

    /// <summary>
    /// Attempts to retrieve a previously cached asset of the specified type.
    /// </summary>
    /// <typeparam name="T">The expected type of the cached asset.</typeparam>
    /// <param name="name">The cache key for the asset, typically the asset path.</param>
    /// <param name="asset">
    /// When this method returns, contains the cached asset if found and of type <typeparamref name="T"/>; otherwise,
    /// the default value for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a cached asset of type <typeparamref name="T"/> was found; otherwise, <see langword="false"/>.
    /// </returns>
    protected bool TryGetCachedAsset<T>(string name, out T asset)
    {
        asset = default;

        if (LoadedAssets.TryGetValue(name, out object value))
        {
            if (value is T)
            {
                asset = (T)value;
                return true;
            }
        }

        return false;
    }
}
