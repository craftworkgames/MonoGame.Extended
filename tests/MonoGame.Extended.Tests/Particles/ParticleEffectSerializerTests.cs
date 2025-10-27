using System;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;

namespace MonoGame.Extended.Tests.Particles;

public sealed class ParticleEffectSerializerTests
{
    private readonly MockContentManager _mockContentManager;

    public ParticleEffectSerializerTests()
    {
        _mockContentManager = new MockContentManager();
    }

    private ParticleEffect ReadParticleEffectFromXml(string xml)
    {
        using MemoryStream stream = new MemoryStream();
        using StreamWriter writer = new StreamWriter(stream);
        writer.Write(xml);
        writer.Flush();
        stream.Position = 0;

        return ParticleEffectSerializer.Deserialize(stream, _mockContentManager);
    }

    [Fact]
    public void Deserialize_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ParticleEffectSerializer.Deserialize((Stream)null!, _mockContentManager));
    }

    [Fact]
    public void Deserialize_NullContentManager_ThrowsArgumentNullException()
    {
        using MemoryStream stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => ParticleEffectSerializer.Deserialize(stream, null));
    }

    [Fact]
    public void Deserialize_InvalidXmlRoot_ThrowsXmlException()
    {
        string xml = "<InvalidRoot />";
        Assert.Throws<XmlException>(() => ReadParticleEffectFromXml(xml));
    }

    [Fact]
    public void Deserialize_EmptyEffect_ReturnsExpected()
    {
        string xml =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="EmptyEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1" />
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Equal("EmptyEffect", effect.Name);
        Assert.Equal(Vector2.Zero, effect.Position);
        Assert.Equal(0.0f, effect.Rotation);
        Assert.Equal(Vector2.One, effect.Scale);
        Assert.Empty(effect.Emitters);
        Assert.True(effect.AutoTrigger);
        Assert.Equal(1.0f, effect.AutoTriggerFrequency);
    }

    [Fact]
    public void Deserialize_EmptyModifiers_ReturnsExpected()
    {
        string xml =
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="EmptyModifiers" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Empty(emitter.Modifiers);
    }

    [Fact]
    public void Deserialize_BoxFillProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="BoxFillProfile" Width="1" Height="1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        BoxFillProfile profile = Assert.IsType<BoxFillProfile>(emitter.Profile);
        Assert.Equal(1.0f, profile.Height);
        Assert.Equal(1.0f, profile.Height);
    }

    [Fact]
    public void Deserialize_BoxProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="BoxProfile" Width="1" Height="1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        BoxProfile profile = Assert.IsType<BoxProfile>(emitter.Profile);
        Assert.Equal(1.0f, profile.Height);
        Assert.Equal(1.0f, profile.Height);
    }

    [Fact]
    public void DeserializeBoxUniformProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="BoxUniformProfile" Width="1" Height="1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        BoxUniformProfile profile = Assert.IsType<BoxUniformProfile>(emitter.Profile);
        Assert.Equal(1.0f, profile.Height);
        Assert.Equal(1.0f, profile.Height);
    }

    [Fact]
    public void DeserializeCircleProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="CircleProfile" Radius="1" Radiate="Out" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        CircleProfile profile = Assert.IsType<CircleProfile>(emitter.Profile);
        Assert.Equal(1.0f, profile.Radius);
        Assert.Equal(CircleRadiation.Out, profile.Radiate);
    }

    [Fact]
    public void DeserializeLineProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="LineProfile" Axis="1,1" Length="1" Direction="0,0" Radiate="None" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        LineProfile profile = Assert.IsType<LineProfile>(emitter.Profile);
        Assert.Equal(Vector2.One, profile.Axis);
        Assert.Equal(1.0f, profile.Length);
    }

    [Fact]
    public void DeserializePointProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.IsType<PointProfile>(emitter.Profile);
    }

    [Fact]
    public void DeserializeRingProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="RingProfile" Radius="1" Radiate="In" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        RingProfile profile = Assert.IsType<RingProfile>(emitter.Profile);
        Assert.Equal(1.0f, profile.Radius);
        Assert.Equal(CircleRadiation.In, profile.Radiate);
    }

    [Fact]
    public void DeserializeSprayProfile_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="SprayProfile" Direction="1,1" Spread="1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        SprayProfile profile = Assert.IsType<SprayProfile>(emitter.Profile);
        Assert.Equal(Vector2.One, profile.Direction);
        Assert.Equal(1.0f, profile.Spread);
    }

    [Fact]
    public void DeserializeAgeModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        AgeModifier modifier = Assert.IsType<AgeModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.True(modifier.Enabled);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.Equal(nameof(AgeModifier), modifier.Name);
    }

    [Fact]
    public void DeserializeCircleContainerModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="CircleContainerModifier" Enabled="True" Frequency="60" Type="CircleContainerModifier" Radius="0" Inside="True" RestitutionCoefficient="1" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        CircleContainerModifier modifier = Assert.IsType<CircleContainerModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(CircleContainerModifier), modifier.Name);
        Assert.Equal(0.0f, modifier.Radius);
        Assert.True(modifier.Inside);
        Assert.Equal(1.0f, modifier.RestitutionCoefficient);
    }

    [Fact]
    public void DeserializeDragModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="DragModifier" Enabled="True" Frequency="60" Type="DragModifier" DragCoefficient="0.47" Density="0.5" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        DragModifier modifier = Assert.IsType<DragModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(DragModifier), modifier.Name);
        Assert.Equal(0.47f, modifier.DragCoefficient);
        Assert.Equal(0.5f, modifier.Density);
    }

    [Fact]
    public void DeserializeLinearGravityModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="LinearGravityModifier" Enabled="True" Frequency="60" Type="LinearGravityModifier" Direction="0,0" Strength="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        LinearGravityModifier modifier = Assert.IsType<LinearGravityModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(LinearGravityModifier), modifier.Name);
        Assert.Equal(Vector2.Zero, modifier.Direction);
        Assert.Equal(0.0f, modifier.Strength);
    }

    [Fact]
    public void DeserializeOpacityFastFadeModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="OpacityFastFadeModifier" Enabled="True" Frequency="60" Type="OpacityFastFadeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        OpacityFastFadeModifier modifier = Assert.IsType<OpacityFastFadeModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(OpacityFastFadeModifier), modifier.Name);
    }

    [Fact]
    public void DeserializeRectangleContainerModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="RectangleContainerModifier" Enabled="True" Frequency="60" Type="RectangleContainerModifier" Width="0" Height="0" RestitutionCoefficient="1" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        RectangleContainerModifier modifier = Assert.IsType<RectangleContainerModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(RectangleContainerModifier), modifier.Name);
        Assert.Equal(0.0f, modifier.Width);
        Assert.Equal(0.0f, modifier.Height);
        Assert.Equal(1.0f, modifier.RestitutionCoefficient);
    }

    [Fact]
    public void DeserializeRectangleLoopContainerModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="RectangleLoopContainerModifier" Enabled="True" Frequency="60" Type="RectangleLoopContainerModifier" Width="0" Height="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        RectangleLoopContainerModifier modifier = Assert.IsType<RectangleLoopContainerModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(RectangleLoopContainerModifier), modifier.Name);
        Assert.Equal(0.0f, modifier.Width);
        Assert.Equal(0.0f, modifier.Height);
    }

    [Fact]
    public void DeserializeRotationModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="RotationModifier" Enabled="True" Frequency="60" Type="RotationModifier" RotationRate="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        RotationModifier modifier = Assert.IsType<RotationModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(RotationModifier), modifier.Name);
        Assert.Equal(0.0f, modifier.RotationRate);
    }

    [Fact]
    public void DeserializeVelocityColorModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="VelocityColorModifier" Enabled="True" Frequency="60" Type="VelocityColorModifier" StationaryColor="0,0,0" VelocityColor="0,0,0" VelocityThreshold="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        VelocityColorModifier modifier = Assert.IsType<VelocityColorModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.True(modifier.Enabled);
        Assert.Equal(nameof(VelocityColorModifier), modifier.Name);
        Assert.Equal(new HslColor(0, 0, 0), modifier.StationaryColor);
        Assert.Equal(new HslColor(0, 0, 0), modifier.VelocityColor);
        Assert.Equal(0.0f, modifier.VelocityThreshold);
    }

    [Fact]
    public void DeserializeVelocityModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="VelocityModifier" Enabled="True" Frequency="60" Type="VelocityModifier" VelocityThreshold="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        VelocityModifier modifier = Assert.IsType<VelocityModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.Equal(nameof(VelocityModifier), modifier.Name);
        Assert.Equal(0.0f, modifier.VelocityThreshold);
    }

    [Fact]
    public void DeserializeVortexModifier_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="VortexModifier" Enabled="True" Frequency="60" Type="VortexModifier" Position="0,0" Strength="1" OuterRadius="2" InnerRadius="3" MaxVelocity="4" RotationAngle="5" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);

        VortexModifier modifier = Assert.IsType<VortexModifier>(effect.Emitters[0].Modifiers[0]);
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.Equal(nameof(VortexModifier), modifier.Name);
        Assert.Equal(Vector2.Zero, modifier.Position);
        Assert.Equal(1.0f, modifier.Strength);
        Assert.Equal(2.0f, modifier.OuterRadius);
        Assert.Equal(3.0f, modifier.InnerRadius);
        Assert.Equal(4.0f, modifier.MaxVelocity);
        Assert.Equal(5.0f, modifier.RotationAngle);
    }

    [Fact]
    public void DeserializeColorInterpolator_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ColorInterpolator" Type="ColorInterpolator" Enabled="True" StartValue="0,0,0" EndValue="0,0,0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);
        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);

        Assert.Single(modifier.Interpolators);

        ColorInterpolator interpolator = Assert.IsType<ColorInterpolator>(modifier.Interpolators[0]);
        Assert.True(interpolator.Enabled);
        Assert.Equal(new HslColor(0, 0, 0), interpolator.StartValue);
        Assert.Equal(new HslColor(0, 0, 0), interpolator.EndValue);
    }

    [Fact]
    public void DeserializeHueInterpolator_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="HueInterpolator" Type="HueInterpolator" Enabled="True" StartValue="0" EndValue="0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);
        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);

        Assert.Single(modifier.Interpolators);

        HueInterpolator interpolator = Assert.IsType<HueInterpolator>(modifier.Interpolators[0]);
        Assert.True(interpolator.Enabled);
        Assert.Equal(0.0f, interpolator.StartValue);
        Assert.Equal(0.0f, interpolator.EndValue);
    }

    [Fact]
    public void DeserializeOpacityInterpolator_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="OpacityInterpolator" Type="OpacityInterpolator" Enabled="True" StartValue="0" EndValue="0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);
        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);

        Assert.Single(modifier.Interpolators);

        OpacityInterpolator interpolator = Assert.IsType<OpacityInterpolator>(modifier.Interpolators[0]);
        Assert.True(interpolator.Enabled);
        Assert.Equal(0.0f, interpolator.StartValue);
        Assert.Equal(0.0f, interpolator.EndValue);
    }

    [Fact]
    public void DeserializeRotationInterpolator_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="RotationInterpolator" Type="RotationInterpolator" Enabled="True" StartValue="0" EndValue="0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);
        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);

        Assert.Single(modifier.Interpolators);

        RotationInterpolator interpolator = Assert.IsType<RotationInterpolator>(modifier.Interpolators[0]);
        Assert.True(interpolator.Enabled);
        Assert.Equal(0.0f, interpolator.StartValue);
        Assert.Equal(0.0f, interpolator.EndValue);
    }

    [Fact]
    public void DeserializeScaleInterpolator_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0,0" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ScaleInterpolator" Type="ScaleInterpolator" Enabled="True" StartValue="0,0" EndValue="1,1" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);
        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);

        Assert.Single(modifier.Interpolators);

        ScaleInterpolator interpolator = Assert.IsType<ScaleInterpolator>(modifier.Interpolators[0]);
        Assert.True(interpolator.Enabled);
        Assert.Equal(Vector2.Zero, interpolator.StartValue);
        Assert.Equal(Vector2.One, interpolator.EndValue);
    }

    [Fact]
    public void DeserializeVelocityInterpolator_ReadsExpected()
    {
        string xml =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="VelocityInterpolator" Type="VelocityInterpolator" Enabled="True" StartValue="0,0" EndValue="0,0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        Assert.Single(emitter.Modifiers);
        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);

        Assert.Single(modifier.Interpolators);

        VelocityInterpolator interpolator = Assert.IsType<VelocityInterpolator>(modifier.Interpolators[0]);
        Assert.True(interpolator.Enabled);
        Assert.Equal(Vector2.Zero, interpolator.StartValue);
        Assert.Equal(Vector2.Zero, interpolator.EndValue);
    }

    [Fact]
    public void SerializeNulLEffect_ThrowsArgumentNullException()
    {
        using MemoryStream stream = new MemoryStream();
        using ParticleEffectWriter writer = new ParticleEffectWriter(stream);
        Assert.Throws<ArgumentNullException>(() => writer.WriteParticleEffect(null));
    }

    [Fact]
    public void SerializeEmptyEffect_WritesMinimalXml()
    {
        ParticleEffect effect = new ParticleEffect("EmptyEffect");

        string expected =
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="EmptyEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1" />
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeEmptyModifiers_WritesMinimalXml()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "EmptyModifiers";
        effect.Emitters.Add(emitter);

        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="EmptyModifiers" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeEmptyInterpolators_WritesMinimalXml()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        effect.Emitters.Add(emitter);
        AgeModifier ageModifier = new AgeModifier();
        emitter.Modifiers.Add(ageModifier);

        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeBoxFillProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.BoxFill(1.0f, 2.0f);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="BoxFillProfile" Width="1" Height="2" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeBoxProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Box(1.0f, 2.0f);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="BoxProfile" Width="1" Height="2" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeBoxUniformProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.BoxUniform(1.0f, 2.0f);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="BoxUniformProfile" Width="1" Height="2" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeCircleProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Circle(1.0f, CircleRadiation.Out);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="CircleProfile" Radius="1" Radiate="Out" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeLineProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Line(Vector2.One, 1.0f);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="LineProfile" Axis="1,1" Length="1" Radiate="None" Direction="0,1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializePointProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeRingProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Ring(1.0f, CircleRadiation.In);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="RingProfile" Radius="1" Radiate="In" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeSprayProfile_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Spray(Vector2.One, 1.0f);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="SprayProfile" Direction="1,1" Spread="1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeAgeModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeCircleContainerModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        CircleContainerModifier modifier = new CircleContainerModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="CircleContainerModifier" Enabled="True" Frequency="60" Type="CircleContainerModifier" Radius="0" Inside="True" RestitutionCoefficient="1" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeDragModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        DragModifier modifier = new DragModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="DragModifier" Enabled="True" Frequency="60" Type="DragModifier" DragCoefficient="0.47" Density="0.5" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeLinearGravityModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        LinearGravityModifier modifier = new LinearGravityModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="LinearGravityModifier" Enabled="True" Frequency="60" Type="LinearGravityModifier" Direction="0,0" Strength="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeOpacityFastFadeModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        OpacityFastFadeModifier modifier = new OpacityFastFadeModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="OpacityFastFadeModifier" Enabled="True" Frequency="60" Type="OpacityFastFadeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeRectangleContainerModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        RectangleContainerModifier modifier = new RectangleContainerModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="RectangleContainerModifier" Enabled="True" Frequency="60" Type="RectangleContainerModifier" Width="0" Height="0" RestitutionCoefficient="1" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeRectangleLoopContainerModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        RectangleLoopContainerModifier modifier = new RectangleLoopContainerModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="RectangleLoopContainerModifier" Enabled="True" Frequency="60" Type="RectangleLoopContainerModifier" Width="0" Height="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeRotationModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        RotationModifier modifier = new RotationModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="RotationModifier" Enabled="True" Frequency="60" Type="RotationModifier" RotationRate="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeVelocityColorModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        VelocityColorModifier modifier = new VelocityColorModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="VelocityColorModifier" Enabled="True" Frequency="60" Type="VelocityColorModifier" StationaryColor="0,0,0" VelocityColor="0,0,0" VelocityThreshold="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeVelocityModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        VelocityModifier modifier = new VelocityModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="VelocityModifier" Enabled="True" Frequency="60" Type="VelocityModifier" VelocityThreshold="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeVortexModifier_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        VortexModifier modifier = new VortexModifier();
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="VortexModifier" Enabled="True" Frequency="60" Type="VortexModifier" Position="0,0" Strength="0" OuterRadius="0" InnerRadius="0" MaxVelocity="0" RotationAngle="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeColorInterpolator_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        ColorInterpolator interpolator = new ColorInterpolator();
        modifier.Interpolators.Add(interpolator);
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ColorInterpolator" Enabled="True" Type="ColorInterpolator" StartValue="0,0,0" EndValue="0,0,0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeHueInterpolator_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        HueInterpolator interpolator = new HueInterpolator();
        modifier.Interpolators.Add(interpolator);
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="HueInterpolator" Enabled="True" Type="HueInterpolator" StartValue="0" EndValue="0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeOpacityInterpolator_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        OpacityInterpolator interpolator = new OpacityInterpolator();
        modifier.Interpolators.Add(interpolator);
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="OpacityInterpolator" Enabled="True" Type="OpacityInterpolator" StartValue="0" EndValue="0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeRotationInterpolator_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        RotationInterpolator interpolator = new RotationInterpolator();
        modifier.Interpolators.Add(interpolator);
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="RotationInterpolator" Enabled="True" Type="RotationInterpolator" StartValue="0" EndValue="0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeScaleInterpolator_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        ScaleInterpolator interpolator = new ScaleInterpolator();
        modifier.Interpolators.Add(interpolator);
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ScaleInterpolator" Enabled="True" Type="ScaleInterpolator" StartValue="0,0" EndValue="0,0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void SerializeVelocityInterpolator_WritesExpected()
    {
        ParticleEffect effect = new ParticleEffect("TestEffect");
        ParticleEmitter emitter = new ParticleEmitter(1);
        emitter.Name = "TestEmitter";
        emitter.Profile = Profile.Point();
        AgeModifier modifier = new AgeModifier();
        VelocityInterpolator interpolator = new VelocityInterpolator();
        modifier.Interpolators.Add(interpolator);
        emitter.Modifiers.Add(modifier);
        effect.Emitters.Add(emitter);


        string expected =
           $"""
            <?xml version="1.0" encoding="utf-8"?>
            <ParticleEffect Name="TestEffect" Position="0,0" Rotation="0" Scale="1,1" AutoTrigger="True" AutoTriggerFrequency="1">
              <Emitters>
                <ParticleEmitter Name="TestEmitter" LifeSpan="1" Offset="0,0" LayerDepth="0" ReclaimFrequency="60" Capacity="1" ModifierExecutionStrategy="Serial" RenderingOrder="FrontToBack">
                  <Parameters>
                    <Quantity Kind="Random" RandomMin="5" RandomMax="100" />
                    <Speed Kind="Random" RandomMin="50" RandomMax="100" />
                    <Color Kind="Constant" Constant="1,1,1" />
                    <Opacity Kind="Random" RandomMin="0" RandomMax="1" />
                    <Scale Kind="Random" RandomMin="0.5,0.5" RandomMax="1,1" />
                    <Rotation Kind="Random" RandomMin="{-MathF.PI}" RandomMax="{MathF.PI}" />
                    <Mass Kind="Constant" Constant="1" />
                  </Parameters>
                  <Profile Type="PointProfile" />
                  <Modifiers>
                    <Modifier Name="AgeModifier" Enabled="True" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="VelocityInterpolator" Enabled="True" Type="VelocityInterpolator" StartValue="0,0" EndValue="0,0" />
                      </Interpolators>
                    </Modifier>
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    private static void AssertParticleEffect(ParticleEffect effect, string expected)
    {
        using MemoryStream stream = new MemoryStream();
        ParticleEffectSerializer.Serialize(stream, effect);

        stream.Position = 0;
        using StreamReader reader = new StreamReader(stream);
        string actual = reader.ReadToEnd();

        Assert.Equal(expected, actual);
    }
}
