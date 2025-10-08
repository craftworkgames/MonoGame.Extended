// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;

namespace MonoGame.Extended.Tests.Particles;

public class ParticleEffectWriterTests
{
    [Fact]
    public void WriteParticleEffect_NulLEffect_ThrowsArgumentNullException()
    {
        using MemoryStream stream = new MemoryStream();
        using ParticleEffectWriter writer = new ParticleEffectWriter(stream);
        Assert.Throws<ArgumentNullException>(() => writer.WriteParticleEffect(null));
    }

    [Fact]
    public void WriteParticleEffect_EmptyEffect_WritesMinimalXml()
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
    public void WriteParticleEffect_EmptyModifiers_WritesMinimalXml()
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
    public void WriteParticleEffect_EmptyInterpolators_WritesMinimalXml()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_BoxFillProfile_WritesExpected()
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
    public void WriteParticleEffect_BoxProfile_WritesExpected()
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
    public void WriteParticleEffect_BoxUniformProfile_WritesExpected()
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
    public void WriteParticleEffect_CircleProfile_WritesExpected()
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
    public void WriteParticleEffect_LineProfile_WritesExpected()
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
                  <Profile Type="LineProfile" Axis="1,1" Length="1" />
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_PointProfile_WritesExpected()
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
    public void WriteParticleEffect_RingProfile_WritesExpected()
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
    public void WriteParticleEffect_SprayProfile_WritesExpected()
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
    public void WriteParticleEffect_AgeModifier_WritesExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_CircleContainerModifier_WritesExpected()
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
                    <Modifier Name="CircleContainerModifier" Frequency="60" Type="CircleContainerModifier" Radius="0" Inside="True" RestitutionCoefficient="1" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_DragModifier_WritesExpected()
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
                    <Modifier Name="DragModifier" Frequency="60" Type="DragModifier" DragCoefficient="0.47" Density="0.5" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_LinearGravityModifier_WritesExpected()
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
                    <Modifier Name="LinearGravityModifier" Frequency="60" Type="LinearGravityModifier" Direction="0,0" Strength="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_OpacityFastFadeModifier_WritesExpected()
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
                    <Modifier Name="OpacityFastFadeModifier" Frequency="60" Type="OpacityFastFadeModifier" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_RectangleContainerModifier_WritesExpected()
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
                    <Modifier Name="RectangleContainerModifier" Frequency="60" Type="RectangleContainerModifier" Width="0" Height="0" RestitutionCoefficient="1" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_RectangleLoopContainerModifier_WritesExpected()
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
                    <Modifier Name="RectangleLoopContainerModifier" Frequency="60" Type="RectangleLoopContainerModifier" Width="0" Height="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_RotationModifier_WritesExpected()
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
                    <Modifier Name="RotationModifier" Frequency="60" Type="RotationModifier" RotationRate="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_VelocityColorModifier_WritesExpected()
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
                    <Modifier Name="VelocityColorModifier" Frequency="60" Type="VelocityColorModifier" StationaryColor="0,0,0" VelocityColor="0,0,0" VelocityThreshold="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_VelocityModifier_WritesExpected()
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
                    <Modifier Name="VelocityModifier" Frequency="60" Type="VelocityModifier" VelocityThreshold="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_VortexModifier_WritesExpected()
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
                    <Modifier Name="VortexModifier" Frequency="60" Type="VortexModifier" Position="0,0" Strength="0" OuterRadius="0" InnerRadius="0" MaxVelocity="0" RotationAngle="0" />
                  </Modifiers>
                </ParticleEmitter>
              </Emitters>
            </ParticleEffect>
            """;

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_ColorInterpolator_WritesExpected()
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

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_HueInterpolator_WritesExpected()
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

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_OpacityInterpolator_WritesExpected()
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

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_RotationInterpolator_WritesExpected()
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

        AssertParticleEffect(effect, expected);
    }

    [Fact]
    public void WriteParticleEffect_ScaleInterpolator_WritesExpected()
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
                    <Modifier Name="AgeModifier" Frequency="60" Type="AgeModifier">
                      <Interpolators>
                        <Interpolator Name="ScaleInterpolator" Type="ScaleInterpolator" StartValue="0,0" EndValue="0,0" />
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
    public void WriteParticleEffect_VelocityInterpolator_WritesExpected()
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

        AssertParticleEffect(effect, expected);
    }

    private void AssertParticleEffect(ParticleEffect effect, string expected)
    {
        string actual = string.Empty;
        using (MemoryStream stream = new MemoryStream())
        {
            using (ParticleEffectWriter writer = new ParticleEffectWriter(stream))
            {
                writer.WriteParticleEffect(effect);
                stream.Position = 0;
            }

            using StreamReader reader = new StreamReader(stream);
            actual = reader.ReadToEnd();
        }

        Assert.Equal(expected, actual);
    }
}
