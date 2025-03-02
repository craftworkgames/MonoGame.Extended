// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Graphics;

/// <summary>
/// Represents an animated sprite that can play, pause, and update animations.
/// </summary>
public class AnimatedSprite : Sprite
{
    private readonly SpriteSheet _spriteSheet;
    private IAnimation _animation;

    private readonly Texture2DRegion[] _regions;

    /// <summary>
    /// Gets the animation controller used to control the current animation of this animated sprite.
    /// </summary>
    public IAnimationController Controller { get; private set; }

    /// <summary>
    /// Gets the name of the current animation playing.
    /// </summary>
    public string CurrentAnimation => _animation.Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class with the specified
    /// <see cref="SpriteSheet"/>.
    /// </summary>
    /// <param name="spriteSheet">The <see cref="SpriteSheet"/> that contains the animations.</param>
    public AnimatedSprite(SpriteSheet spriteSheet)
        : base(spriteSheet.TextureAtlas[0])
    {
        ArgumentNullException.ThrowIfNull(spriteSheet);
        _spriteSheet = spriteSheet;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class with the specified
    /// <see cref="SpriteSheet"/>.
    /// </summary>
    /// <param name="spriteSheet">The <see cref="SpriteSheet"/> that contains the animations.</param>
    /// <param name="initialAnimation">The initial animation to play</param>
    public AnimatedSprite(SpriteSheet spriteSheet, string initialAnimation) :this(spriteSheet)
    {
        _animation = spriteSheet.GetAnimation(initialAnimation);
        Controller = new AnimationController(_animation);
        TextureRegion = _spriteSheet.TextureAtlas[Controller.CurrentFrame];
    }

    /// <summary>
    /// Sets the animation to use for this animated sprite.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <returns>The <see cref="IAnimationController"/> of the animation.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the source spritesheet does not contain an animation a name that matches the <paramref name="name"/> parameter.
    /// </exception>
    public IAnimationController SetAnimation(string name)
    {
        _animation = _spriteSheet.GetAnimation(name);
        Controller = new AnimationController(_animation);
        TextureRegion = _spriteSheet.TextureAtlas[Controller.CurrentFrame];

        return Controller;
    }

    /// <inheritdoc />
    public void Update(GameTime gameTime)
    {
        int index = Controller.CurrentFrame;
        Controller.Update(gameTime);

        //  If the current frame changed during the update, change the texture region
        if (index != Controller.CurrentFrame)
        {
            TextureRegion = _spriteSheet.TextureAtlas[Controller.CurrentFrame];
        }
    }
}
