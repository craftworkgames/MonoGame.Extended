// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Data;

/// <summary>
/// Defines the parameters used when releasing particles from an emitter.
/// </summary>
/// <remarks>
/// This class encapsulates all the configurable properties that control how particles are initialized when they are
/// created by the particle system. Each property can be set as either a constant value or a random range.
/// </remarks>
public class ParticleReleaseParameters
{
    /// <summary>
    /// Gets or sets the number of particles to release in a single emission.
    /// </summary>
    /// <remarks>
    /// Defaults to a random value between 5 and 100 particles per emission.
    /// </remarks>
    public ParticleInt32Parameter Quantity = new ParticleInt32Parameter(5, 100);

    /// <summary>
    /// Gets or sets the initial speed of particles when released.
    /// </summary>
    /// <remarks>
    /// Defaults to a random value between 50.0 and 100.0 units per second.
    /// </remarks>
    public ParticleFloatParameter Speed = new ParticleFloatParameter(50.0f, 100.0f);

    /// <summary>
    /// Gets or sets the initial color of particles when released.
    /// </summary>
    /// <remarks>
    /// Defaults to white (1.0f, 1.0f, 1.0f).
    /// </remarks>
    public ParticleColorParameter Color = new ParticleColorParameter(new Vector3(1.0f, 1.0f, 1.0f));

    /// <summary>
    /// Gets or sets the initial opacity of particles when released.
    /// </summary>
    /// <remarks>
    /// Defaults to a random value between 0.0 (transparent) and 1.0 (opaque).
    /// </remarks>
    public ParticleFloatParameter Opacity = new ParticleFloatParameter(0.0f, 1.0f);

    /// <summary>
    /// Gets or sets the initial scale of particles when released.
    /// </summary>
    /// <remarks>
    /// Defaults to a random value between 0.0 (half scale) and 1.0 (full scale)
    /// </remarks>
    public ParticleVector2Parameter Scale = new ParticleVector2Parameter(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 1.0f));

    /// <summary>
    /// Gets or sets the initial rotation (in radians) of particles when released.
    /// </summary>
    /// <remarks>
    /// Defaults to a random value between -π and π radians (a full 360° range).
    /// </remarks>
    public ParticleFloatParameter Rotation = new ParticleFloatParameter(-MathF.PI, MathF.PI);

    /// <summary>
    /// Gets or sets the mass of particles when released.
    /// </summary>
    /// <remarks>
    /// Defaults to a constant value of 1.0.
    /// </remarks>
    public ParticleFloatParameter Mass = new ParticleFloatParameter(1.0f);


    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleReleaseParameters"/> class with default values.
    /// </summary>
    /// <remarks>
    /// Default values for properties:
    /// <para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>Property</term>
    ///     <description>Default Value</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Quantity</term>
    ///     <description>Random: 5-100 particles</description>
    ///   </item>
    ///   <item>
    ///     <term>Speed</term>
    ///     <description>Random: 50.0-100.0 units/second</description>
    ///   </item>
    ///   <item>
    ///     <term>Color</term>
    ///     <description>Constant: White (1.0, 1.0, 1.0)</description>
    ///   </item>
    ///   <item>
    ///     <term>Opacity</term>
    ///     <description>Random: 0.0-1.0</description>
    ///   </item>
    ///   <item>
    ///     <term>Scale</term>
    ///     <description>Random: 0.5-1.0</description>
    ///   </item>
    ///   <item>
    ///     <term>Rotation</term>
    ///     <description>Random: -π to π radians</description>
    ///   </item>
    ///   <item>
    ///     <term>Mass</term>
    ///     <description>Constant: 1.0</description>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    public ParticleReleaseParameters() { }
}
