using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Graphics;

public class SpriteSheet
{
    private static readonly TimeSpan s_defaultDuration = TimeSpan.FromMinutes(100);
    private readonly Dictionary<string, SpriteSheetAnimationDefinition> _animations = new Dictionary<string, SpriteSheetAnimationDefinition>();

    public int AnimationCycleCount => _animations.Count;

    public string Name { get; }

    public Texture2DAtlas TextureAtlas { get; }

    public SpriteSheet(string name, Texture2DAtlas textureAtlas)
    {
        ArgumentNullException.ThrowIfNull(textureAtlas);

        TextureAtlas = textureAtlas;
        Name = name;
    }

    public Sprite CreateSprite(int regionIndex) => TextureAtlas.CreateSprite(regionIndex);
    public Sprite CreateSprite(string regionName) => TextureAtlas.CreateSprite(regionName);

    public AnimatedSprite CreateAniamtedSprite(string animationName)
    {
        SpriteSheetAnimationDefinition animationDefinition = _animations[animationName];
        Texture2DRegion[] regions = TextureAtlas.GetRegions(animationDefinition.Frames);
        return new AnimatedSprite(animationDefinition, regions);
    }

    public void DefineAnimation(string name, Action<SpriteSheetAnimationDefinitionBuilder> buildAction)
    {
        SpriteSheetAnimationDefinitionBuilder builder = new SpriteSheetAnimationDefinitionBuilder(name, this);
        buildAction(builder);
        SpriteSheetAnimationDefinition definition = builder.Build();
        _animations.Add(name, definition);
    }

    public bool RemoveAnimationDefinition(string name) => _animations.Remove(name);


}
