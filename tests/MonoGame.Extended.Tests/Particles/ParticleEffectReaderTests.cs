// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

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

public class ParticleEffectReaderTests
{
    private readonly MockContentManager _mockContentManager;

    public ParticleEffectReaderTests()
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

        using ParticleEffectReader reader = new ParticleEffectReader(stream, _mockContentManager);
        return reader.ReadParticleEffect();
    }

    [Fact]
    public void ReadParticleEffect_NulLStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ParticleEffectReader((Stream)null, _mockContentManager));
    }

    [Fact]
    public void ReadParticleEffect_NullContentManager_ThrowsArgumentNullException()
    {
        using MemoryStream stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => new ParticleEffectReader(stream, null));
    }

    [Fact]
    public void ReadParticleEffect_InvalidXmlRoot_ThrowsXmlException()
    {
        string xml = "<InvalidRoot />";
        Assert.Throws<XmlException>(() => ReadParticleEffectFromXml(xml));
    }

    [Fact]
    public void ReadParticleEffect_EmptyEffect_ReturnsExpected()
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
        Assert.Equal(effect.AutoTriggerFrequency, 1.0f);
    }

    [Fact]
    public void ReadParticleEffect_EmptyModifiers_ReturnsExpected()
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
    public void ReadParticleEffect_EmptyInterpolators_ReturnsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        ParticleEffect effect = ReadParticleEffectFromXml(xml);

        Assert.Single(effect.Emitters);
        ParticleEmitter emitter = effect.Emitters[0];

        AgeModifier modifier = Assert.IsType<AgeModifier>(emitter.Modifiers[0]);
        Assert.Empty(modifier.Interpolators);
    }

    [Fact]
    public void ReadParticleEffect_BoxFillProfile_ReadsExpected()
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
    public void ReadParticleEffect_BoxProfile_ReadsExpected()
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
    public void ReadParticleEffect_BoxUniformProfile_ReadsExpected()
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
    public void ReadParticleEffect_CircleProfile_ReadsExpected()
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
    public void ReadParticleEffect_LineProfile_ReadsExpected()
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
                  <Profile Type="LineProfile" Axis="1,1" Length="1" />
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
    public void ReadParticleEffect_PointProfile_ReadsExpected()
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

        PointProfile profile = Assert.IsType<PointProfile>(emitter.Profile);
    }

    [Fact]
    public void ReadParticleEffect_RingProfile_ReadsExpected()
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
    public void ReadParticleEffect_SprayProfile_ReadsExpected()
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
    public void ReadParticleEffect_AgeModifier_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier" />
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
        Assert.Equal(60.0f, modifier.Frequency);
        Assert.Equal("AgeModifier", modifier.Name);
    }

    [Fact]
    public void ReadParticleEffect_CircleContainerModifier_ReadsExpected()
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
                    <Modifier Name="CircleContainerModifier" Frequency="60" Type="CircleContainerModifier" Radius="0" Inside="True" RestitutionCoefficient="1" />
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
        Assert.Equal("CircleContainerModifier", modifier.Name);
        Assert.Equal(0.0f, modifier.Radius);
        Assert.True(modifier.Inside);
        Assert.Equal(1.0f, modifier.RestitutionCoefficient);
    }

    [Fact]
    public void ReadParticleEffect_DragModifier_ReadsExpected()
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
                    <Modifier Name="DragModifier" Frequency="60" Type="DragModifier" DragCoefficient="0.47" Density="0.5" />
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
        Assert.Equal("DragModifier", modifier.Name);
        Assert.Equal(0.47f, modifier.DragCoefficient);
        Assert.Equal(0.5f, modifier.Density);
    }

    [Fact]
    public void ReadParticleEffect_LinearGravityModifier_ReadsExpected()
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
                    <Modifier Name="LinearGravityModifier" Frequency="60" Type="LinearGravityModifier" Direction="0,0" Strength="0" />
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
        Assert.Equal("LinearGravityModifier", modifier.Name);
        Assert.Equal(Vector2.Zero, modifier.Direction);
        Assert.Equal(0.0f, modifier.Strength);
    }

    [Fact]
    public void ReadParticleEffect_OpacityFastFadeModifier_ReadsExpected()
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
                    <Modifier Name="OpacityFastFadeModifier" Frequency="60" Type="OpacityFastFadeModifier" />
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
        Assert.Equal("OpacityFastFadeModifier", modifier.Name);
    }

    [Fact]
    public void ReadParticleEffect_RectangleContainerModifier_ReadsExpected()
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
                    <Modifier Name="RectangleContainerModifier" Frequency="60" Type="RectangleContainerModifier" Width="0" Height="0" RestitutionCoefficient="1" />
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
        Assert.Equal("RectangleContainerModifier", modifier.Name);
        Assert.Equal(0.0f, modifier.Width);
        Assert.Equal(0.0f, modifier.Height);
        Assert.Equal(1.0f, modifier.RestitutionCoefficient);
    }

    [Fact]
    public void ReadParticleEffect_RectangleLoopContainerModifier_ReadsExpected()
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
                    <Modifier Name="RectangleLoopContainerModifier" Frequency="60" Type="RectangleLoopContainerModifier" Width="0" Height="0" />
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
        Assert.Equal("RectangleLoopContainerModifier", modifier.Name);
        Assert.Equal(0.0f, modifier.Width);
        Assert.Equal(0.0f, modifier.Height);
    }

    [Fact]
    public void ReadParticleEffect_RotationModifier_ReadsExpected()
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
                    <Modifier Name="RotationModifier" Frequency="60" Type="RotationModifier" RotationRate="0" />
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
        Assert.Equal("RotationModifier", modifier.Name);
        Assert.Equal(0.0f, modifier.RotationRate);
    }

    [Fact]
    public void ReadParticleEffect_VelocityColorModifier_ReadsExpected()
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
                    <Modifier Name="VelocityColorModifier" Frequency="60" Type="VelocityColorModifier" StationaryColor="0,0,0" VelocityColor="0,0,0" VelocityThreshold="0" />
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
        Assert.Equal("VelocityColorModifier", modifier.Name);
        Assert.Equal(new HslColor(0, 0, 0), modifier.StationaryColor);
        Assert.Equal(new HslColor(0, 0, 0), modifier.VelocityColor);
        Assert.Equal(0.0f, modifier.VelocityThreshold);
    }

    [Fact]
    public void ReadParticleEffect_VelocityModifier_ReadsExpected()
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
                    <Modifier Name="VelocityModifier" Frequency="60" Type="VelocityModifier" VelocityThreshold="0" />
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
        Assert.Equal("VelocityModifier", modifier.Name);
        Assert.Equal(0.0f, modifier.VelocityThreshold);
    }

    [Fact]
    public void ReadParticleEffect_VortexModifier_ReadsExpected()
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
                    <Modifier Name="VortexModifier" Frequency="60" Type="VortexModifier" Position="0,0" Strength="1" OuterRadius="2" InnerRadius="3" MaxVelocity="4" RotationAngle="5" />
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
        Assert.Equal("VortexModifier", modifier.Name);
        Assert.Equal(Vector2.Zero, modifier.Position);
        Assert.Equal(1.0f, modifier.Strength);
        Assert.Equal(2.0f, modifier.OuterRadius);
        Assert.Equal(3.0f, modifier.InnerRadius);
        Assert.Equal(4.0f, modifier.MaxVelocity);
        Assert.Equal(5.0f, modifier.RotationAngle);
    }

    [Fact]
    public void ReadParticleEffect_ColorInterpolator_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ColorInterpolator" Type="ColorInterpolator" StartValue="0,0,0" EndValue="0,0,0" />
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
        Assert.Equal(new HslColor(0, 0, 0), interpolator.StartValue);
        Assert.Equal(new HslColor(0, 0, 0), interpolator.EndValue);
    }

    [Fact]
    public void ReadParticleEffect_HueInterpolator_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="HueInterpolator" Type="HueInterpolator" StartValue="0" EndValue="0" />
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
        Assert.Equal(0.0f, interpolator.StartValue);
        Assert.Equal(0.0f, interpolator.EndValue);
    }

    [Fact]
    public void ReadParticleEffect_OpacityInterpolator_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="OpacityInterpolator" Type="OpacityInterpolator" StartValue="0" EndValue="0" />
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
        Assert.Equal(0.0f, interpolator.StartValue);
        Assert.Equal(0.0f, interpolator.EndValue);
    }

    [Fact]
    public void ReadParticleEffect_RotationInterpolator_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="RotationInterpolator" Type="RotationInterpolator" StartValue="0" EndValue="0" />
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
        Assert.Equal(0.0f, interpolator.StartValue);
        Assert.Equal(0.0f, interpolator.EndValue);
    }

    [Fact]
    public void ReadParticleEffect_ScaleInterpolator_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ScaleInterpolator" Type="ScaleInterpolator" StartValue="0,0" EndValue="1,1" />
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
        Assert.Equal(Vector2.Zero, interpolator.StartValue);
        Assert.Equal(Vector2.One, interpolator.EndValue);
    }

    [Fact]
    public void ReadParticleEffect_VelocityInterpolator_ReadsExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="VelocityInterpolator" Type="VelocityInterpolator" StartValue="0,0" EndValue="0,0" />
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
        Assert.Equal(Vector2.Zero, interpolator.StartValue);
        Assert.Equal(Vector2.Zero, interpolator.EndValue);
    }
}
