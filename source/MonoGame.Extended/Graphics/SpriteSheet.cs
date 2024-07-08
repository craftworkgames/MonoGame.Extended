// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents a sprite sheet containing textures and animations.
/// </summary>
public class SpriteSheet
{
    private readonly Dictionary<string, SpriteSheetAnimation> _animations = new Dictionary<string, SpriteSheetAnimation>();

    /// <summary>
    /// Gets the number of animations defined in the sprite sheet.
    /// </summary>
    public int AnimationCount => _animations.Count;

    /// <summary>
    /// Gets the name of the sprite sheet.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the texture atlas associated with the sprite sheet.
    /// </summary>
    public Texture2DAtlas TextureAtlas { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheet"/> class.
    /// </summary>
    /// <param name="name">The name of the sprite sheet.</param>
    /// <param name="textureAtlas">The texture atlas to use for the sprite sheet.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="textureAtlas"/> is <c>null</c>.</exception>
    public SpriteSheet(string name, Texture2DAtlas textureAtlas)
    {
        ArgumentNullException.ThrowIfNull(textureAtlas);

        TextureAtlas = textureAtlas;
        Name = name;
    }

    /// <summary>
    /// Creates a sprite from the specified region index.
    /// </summary>
    /// <param name="regionIndex">The index of the region in the texture atlas.</param>
    /// <returns>A new <see cref="Sprite"/> instance.</returns>
    public Sprite CreateSprite(int regionIndex) => TextureAtlas.CreateSprite(regionIndex);

    /// <summary>
    /// Creates a sprite from the specified region name.
    /// </summary>
    /// <param name="regionName">The name of the region in the texture atlas.</param>
    /// <returns>A new <see cref="Sprite"/> instance.</returns>
    public Sprite CreateSprite(string regionName) => TextureAtlas.CreateSprite(regionName);


    /// <summary>
    /// Defines a new animation for the sprite sheet.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <param name="buildAction">The action to build the animation definition.</param>
    public void DefineAnimation(string name, Action<SpriteSheetAnimationBuilder> buildAction)
    {
        SpriteSheetAnimationBuilder builder = new SpriteSheetAnimationBuilder(name, this);
        buildAction(builder);
        SpriteSheetAnimation definition = builder.Build();
        _animations.Add(name, definition);
    }

    public SpriteSheetAnimation GetAnimation(string name) => _animations[name];

    /// <summary>
    /// Removes the animation definition with the specified name.
    /// </summary>
    /// <param name="name">The name of the animation to remove.</param>
    /// <returns><c>true</c> if the animation was successfully removed; otherwise, <c>false</c>.</returns>
    public bool RemoveAnimationDefinition(string name) => _animations.Remove(name);
}

