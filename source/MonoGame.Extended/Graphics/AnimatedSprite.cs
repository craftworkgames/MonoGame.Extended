// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents an animated sprite that can play, pause, and update animations.
/// </summary>
public class AnimatedSprite : Sprite
{
    private readonly Texture2DRegion[] _regions;

    /// <summary>
    /// Gets the animation used by this animated sprite.
    /// </summary>
    public IAnimation Animation { get; }

    internal AnimatedSprite(SpriteSheetAnimationDefinition definition, Texture2DRegion[] regions)
        : base(regions[0])
    {
        Animation = new Animation(definition);
        _regions = regions;
    }

    /// <inheritdoc />
    public void Update(GameTime gameTime)
    {
        int index = Animation.CurrentFrame;
        Animation.Update(gameTime);

        //  If the current frame changed during the update, change the texture region
        if (index != Animation.CurrentFrame)
        {
            TextureRegion = _regions[Animation.CurrentFrame];
        }
    }
}
