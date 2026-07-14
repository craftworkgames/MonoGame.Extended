using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Tests.Fixtures;

namespace MonoGame.Extended.Tests.Particles;

[Collection("GraphicsTest")]
public sealed class ParticleEffectSerializerTextureTests
{
    private readonly GraphicsTestFixture _graphicsFixture;

    public ParticleEffectSerializerTextureTests(GraphicsTestFixture graphicsFixture)
    {
        _graphicsFixture = graphicsFixture;
    }

    [Fact]
    public void Deserialize_TextureRegion_OverwritesLoadedTextureNameWithRawXmlValue()
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();
        texture.Name = "StaleCachedName";

        PresetTextureContentManager content = new PresetTextureContentManager(texture);

        // Effect lives under "particles/", texture lives under a sibling "textures/" folder.
        const string textureAttribute = "../textures/particle";
        const string expectedLoadedAssetName = "textures/particle";

        string xml =
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <TextureRegion Name="{textureAttribute}" />
                  <Parameters />
                  <Profile Type="PointProfile" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        using MemoryStream stream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, leaveOpen: true))
        {
            writer.Write(xml);
        }
        stream.Position = 0;

        ParticleEffect effect = ParticleEffectSerializer.Deserialize(stream, content, baseDirectory: "particles");

        // Regression for #1162 (85142e2d): the content path actually loaded is normalized.
        Assert.Equal(expectedLoadedAssetName, content.LastRequestedAssetName);

        // Regression for #1165: Texture.Name is unconditionally set to the raw XML
        // attribute value, distinct from both the normalized load path and any stale name
        // the loaded texture instance already had.
        Assert.Equal(textureAttribute, effect.Emitters[0].TextureRegion.Texture.Name);
        Assert.NotEqual(content.LastRequestedAssetName, effect.Emitters[0].TextureRegion.Texture.Name);
    }

    private sealed class PresetTextureContentManager : ContentManager
    {
        private readonly Texture2D _texture;

        public string LastRequestedAssetName { get; private set; }

        public PresetTextureContentManager(Texture2D texture) : base(new GameServiceContainer())
        {
            _texture = texture;
        }

        public override T Load<T>(string assetName)
        {
            LastRequestedAssetName = assetName;

            if (typeof(T) == typeof(Texture2D))
            {
                return (T)(object)_texture;
            }

            return default;
        }
    }
}
