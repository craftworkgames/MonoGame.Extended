// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Serialization.Xml;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Represents a reader that deserializes a <see cref="ParticleEffect"/> from an XML configuration.
/// </summary>
[Obsolete("Use ParticleEffectSerializer.Deserialize.  ParticleEffectReader will be removed in 6.0.0")]
public sealed class ParticleEffectReader : IDisposable
{
    private readonly XmlReader _reader;
    private readonly ContentManager _content;
    private readonly string _baseDirectory;

    /// <summary>
    /// Gets a value that indicates whether this <see cref="ParticleEffectReader"/> has been disposed of.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffectReader"/> class that reads from a file.
    /// </summary>
    /// <param name="fileName">The file path to read the XMl from.</param>
    /// <param name="content">The <see cref="ContentManager"/> to use for loading textures.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is <see langword="null"/> or empty.</exception>
    public ParticleEffectReader(string fileName, ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.CloseInput = true;
        settings.IgnoreComments = true;
        settings.IgnoreWhitespace = true;

        _content = content;
        _reader = XmlReader.Create(fileName, settings);

        _baseDirectory = Path.GetDirectoryName(Path.GetFullPath(fileName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffectReader"/> class that reads from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="content">The <see cref="ContentManager"/> to use for loading textures.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="content"/> is <see langword="null"/></exception>
    public ParticleEffectReader(Stream stream, ContentManager content)
        : this(stream, content, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffectReader"/> class that reads from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="content">The <see cref="ContentManager"/> to use for loading textures.</param>
    /// <param name="baseDirectory">The base directory to use for resolving relative texture paths. If null, uses the ContentManager's RootDirectory.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="content"/> is <see langword="null"/></exception>
    public ParticleEffectReader(Stream stream, ContentManager content, string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(content);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        settings.IgnoreWhitespace = true;
        settings.CloseInput = false;

        _content = content;
        _reader = XmlReader.Create(stream, settings);

        _baseDirectory = baseDirectory ?? content.RootDirectory;
    }

    /// <summary/>
    ~ParticleEffectReader()
    {
        Dispose();
    }

    /// <summary>
    /// Reads a <see cref="ParticleEffect"/> from the XML input.
    /// </summary>
    /// <returns>The deserialized <see cref="ParticleEffect"/>.</returns>
    /// <exception cref="XmlException">The Xml format is invalid.</exception>
    public ParticleEffect ReadParticleEffect()
    {
        _reader.MoveToContent();

        if (_reader.NodeType != XmlNodeType.Element || _reader.LocalName != nameof(ParticleEffect))
        {
            throw new XmlException($"Expected {nameof(ParticleEffect)} root element");
        }

        string name = _reader.GetAttribute(nameof(ParticleEffect.Name)) ?? "Unnamed";
        ParticleEffect effect = new ParticleEffect(name);

        effect.Position = _reader.GetAttributeVector2(nameof(ParticleEffect.Position));
        effect.Rotation = _reader.GetAttributeFloat(nameof(ParticleEffect.Rotation));
        effect.Scale = _reader.GetAttributeVector2(nameof(ParticleEffect.Scale));
        effect.AutoTrigger = _reader.GetAttributeBool(nameof(ParticleEffect.AutoTrigger));
        effect.AutoTriggerFrequency = _reader.GetAttributeFloat(nameof(ParticleEffect.AutoTriggerFrequency));

        if (_reader.ReadToDescendant(nameof(ParticleEffect.Emitters)))
        {
            if (_reader.ReadToDescendant(nameof(ParticleEmitter)))
            {
                do
                {
                    ParticleEmitter emitter = ReadParticleEmitter();
                    effect.Emitters.Add(emitter);
                } while (_reader.ReadToNextSibling(nameof(ParticleEmitter)));
            }
        }

        return effect;
    }

    private ParticleEmitter ReadParticleEmitter()
    {
        int capacity = _reader.GetAttributeInt(nameof(ParticleEmitter.Capacity));

        ParticleEmitter emitter = new ParticleEmitter(capacity);
        emitter.Name = _reader.GetAttribute(nameof(ParticleEmitter.Name)) ?? nameof(ParticleEmitter.Name);
        emitter.LifeSpan = _reader.GetAttributeFloat(nameof(ParticleEmitter.LifeSpan));
        emitter.Offset = _reader.GetAttributeVector2(nameof(ParticleEmitter.Offset));
        emitter.LayerDepth = _reader.GetAttributeFloat(nameof(ParticleEmitter.LayerDepth));
        emitter.ReclaimFrequency = _reader.GetAttributeFloat(nameof(ParticleEmitter.ReclaimFrequency));

        string strategy = _reader.GetAttribute(nameof(ParticleEmitter.ModifierExecutionStrategy));

        if (strategy.Equals(nameof(ModifierExecutionStrategy.Serial)))
        {
            emitter.ModifierExecutionStrategy = ModifierExecutionStrategy.Serial;
        }
        else if (strategy.Equals(nameof(ModifierExecutionStrategy.Parallel)))
        {
            emitter.ModifierExecutionStrategy = ModifierExecutionStrategy.Parallel;
        }
        else
        {
            emitter.ModifierExecutionStrategy = ModifierExecutionStrategy.Serial;
        }

        emitter.RenderingOrder = _reader.GetAttributeEnum<ParticleRenderingOrder>(nameof(ParticleEmitter.RenderingOrder));

        using XmlReader subtree = _reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element)
            {
                switch (subtree.LocalName)
                {
                    case nameof(ParticleEmitter.TextureRegion):
                        emitter.TextureRegion = ReadTexture2DRegion(subtree);
                        break;

                    case nameof(ParticleEmitter.Parameters):
                        emitter.Parameters = ReadParticleReleaseParameters(subtree);
                        break;

                    case nameof(ParticleEmitter.Profile):
                        emitter.Profile = ReadProfile(subtree);
                        break;

                    case nameof(ParticleEmitter.Modifiers):
                        ReadModifiers(subtree, emitter.Modifiers);
                        break;
                }
            }
        }

        return emitter;
    }

    private Texture2DRegion ReadTexture2DRegion(XmlReader reader)
    {
        string name = reader.GetAttribute(nameof(Texture2DRegion.Texture.Name));

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        Rectangle bounds = _reader.GetAttributeRectangle(nameof(Texture2DRegion.Bounds));

        try
        {
            // Try loading directly though the content manager.  This should work, but there is a bug
            // for relative paths which has been documented at
            // https://github.com/MonoGame/MonoGame/issues/8786
            // And a propsed fix at
            // https://github.com/MonoGame/MonoGame/pull/8787
            // But until that is merged and released, we'll attempt to load here, then fall back to direct load in
            // the catch
            Texture2D texture = _content.Load<Texture2D>(name);
            return new Texture2DRegion(texture, bounds);
        }
        catch (ContentLoadException)
        {
            return TryLoadTextureDirectly(name, bounds);
        }
    }

    private Texture2DRegion TryLoadTextureDirectly(string name, Rectangle bounds)
    {
        if(_content?.ServiceProvider == null)
        {
            return null;
        }

        IGraphicsDeviceService graphicsDeviceService = _content.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
        if(graphicsDeviceService?.GraphicsDevice == null)
        {
            return null;
        }

        // Try common image extensions
        string filePath = Path.Combine(_baseDirectory, name);

        if (File.Exists(filePath))
        {
            try
            {
#if KNI || FNA
                using FileStream stream = File.OpenRead(filePath);
                Texture2D texture = Texture2D.FromStream(graphicsDeviceService.GraphicsDevice, stream);
#else
                Texture2D texture = Texture2D.FromFile(graphicsDeviceService.GraphicsDevice, filePath);
#endif
                texture.Name = name;
                return new Texture2DRegion(texture, bounds);
            }
            catch
            {
                // TODO: 6.0.0
                // Since the file name is baked into the .ember file including
                // the extension, we no longer check extensions in a for each
                // loop.  This means we can't just "continue" in the catch.
                // We'll need to throw an exception, but doing so would be
                // a breaking change, so we'll return null for now, document
                // it and update it for 6.0.0
                return null;
            }
        }

        return null;
    }

    private ParticleReleaseParameters ReadParticleReleaseParameters(XmlReader reader)
    {
        ParticleReleaseParameters parameters = new ParticleReleaseParameters();

        using XmlReader subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element)
            {
                switch (subtree.LocalName)
                {
                    case nameof(ParticleReleaseParameters.Quantity):
                        parameters.Quantity = ReadParticleInt32Parameter(subtree);
                        break;

                    case nameof(ParticleReleaseParameters.Speed):
                        parameters.Speed = ReadParticleFloatParameter(subtree);
                        break;

                    case nameof(ParticleReleaseParameters.Color):
                        parameters.Color = ReadParticleColorParameter(subtree);
                        break;

                    case nameof(ParticleReleaseParameters.Opacity):
                        parameters.Opacity = ReadParticleFloatParameter(subtree);
                        break;

                    case nameof(ParticleReleaseParameters.Scale):
                        parameters.Scale = ReadParticleVector2Parameter(subtree);
                        break;

                    case nameof(ParticleReleaseParameters.Rotation):
                        parameters.Rotation = ReadParticleFloatParameter(subtree);
                        break;

                    case nameof(ParticleReleaseParameters.Mass):
                        parameters.Mass = ReadParticleFloatParameter(subtree);
                        break;
                }
            }
        }

        return parameters;
    }

    private ParticleInt32Parameter ReadParticleInt32Parameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleInt32Parameter.Kind));

        if (kind == ParticleValueKind.Constant)
        {
            int value = reader.GetAttributeInt(nameof(ParticleInt32Parameter.Constant));
            return new ParticleInt32Parameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            int min = reader.GetAttributeInt(nameof(ParticleInt32Parameter.RandomMin));
            int max = reader.GetAttributeInt(nameof(ParticleInt32Parameter.RandomMax));
            return new ParticleInt32Parameter(min, max);
        }

        return new ParticleInt32Parameter(0);
    }

    private ParticleFloatParameter ReadParticleFloatParameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleFloatParameter.Kind));

        if (kind == ParticleValueKind.Constant)
        {
            float value = reader.GetAttributeFloat(nameof(ParticleFloatParameter.Constant));
            return new ParticleFloatParameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            float min = reader.GetAttributeFloat(nameof(ParticleFloatParameter.RandomMin));
            float max = reader.GetAttributeFloat(nameof(ParticleFloatParameter.RandomMax));
            return new ParticleFloatParameter(min, max);
        }

        return new ParticleFloatParameter(0);
    }

    private ParticleVector2Parameter ReadParticleVector2Parameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleVector2Parameter.Kind));

        if(kind == ParticleValueKind.Constant)
        {
            Vector2 value = reader.GetAttributeVector2(nameof(ParticleVector2Parameter.Constant));
            return new ParticleVector2Parameter(value);
        }
        else if(kind == ParticleValueKind.Random)
        {
            Vector2 min = reader.GetAttributeVector2(nameof(ParticleVector2Parameter.RandomMin));
            Vector2 max = reader.GetAttributeVector2(nameof(ParticleVector2Parameter.RandomMax));
            return new ParticleVector2Parameter(min, max);
        }

        return new ParticleVector2Parameter(Vector2.Zero);
    }

    private ParticleColorParameter ReadParticleColorParameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleColorParameter.Kind));

        if (kind == ParticleValueKind.Constant)
        {
            Vector3 value = reader.GetAttributeVector3(nameof(ParticleColorParameter.Constant));
            return new ParticleColorParameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            Vector3 min = reader.GetAttributeVector3(nameof(ParticleColorParameter.RandomMin));
            Vector3 max = reader.GetAttributeVector3(nameof(ParticleColorParameter.RandomMax));
            return new ParticleColorParameter(min, max);
        }

        return new ParticleColorParameter(Vector3.Zero);
    }

    private Profile ReadProfile(XmlReader reader)
    {
        string type = reader.GetAttribute(nameof(Type));

        switch (type)
        {
            case nameof(BoxProfile):
                BoxProfile boxProfile = new BoxProfile();
                boxProfile.Width = reader.GetAttributeFloat(nameof(BoxProfile.Width));
                boxProfile.Height = reader.GetAttributeFloat(nameof(BoxProfile.Height));
                return boxProfile;

            case nameof(BoxFillProfile):
                BoxFillProfile boxFillProfile = new BoxFillProfile();
                boxFillProfile.Width = reader.GetAttributeFloat(nameof(BoxFillProfile.Width));
                boxFillProfile.Height = reader.GetAttributeFloat(nameof(BoxFillProfile.Height));
                return boxFillProfile;

            case nameof(BoxUniformProfile):
                BoxUniformProfile boxUniformProfile = new BoxUniformProfile();
                boxUniformProfile.Width = reader.GetAttributeFloat(nameof(BoxUniformProfile.Width));
                boxUniformProfile.Height = reader.GetAttributeFloat(nameof(BoxUniformProfile.Height));
                return boxUniformProfile;

            case nameof(CircleProfile):
                CircleProfile circleProfile = new CircleProfile();
                circleProfile.Radius = reader.GetAttributeFloat(nameof(CircleProfile.Radius));
                circleProfile.Radiate = reader.GetAttributeEnum<CircleRadiation>(nameof(CircleProfile.Radiate));
                return circleProfile;

            case nameof(LineProfile):
                LineProfile lineProfile = new LineProfile();
                lineProfile.Axis = reader.GetAttributeVector2(nameof(LineProfile.Axis));
                lineProfile.Length = reader.GetAttributeFloat(nameof(LineProfile.Length));
                return lineProfile;

            case nameof(PointProfile):
                return Profile.Point();

            case nameof(RingProfile):
                RingProfile ringProfile = new RingProfile();
                ringProfile.Radius = reader.GetAttributeFloat(nameof(RingProfile.Radius));
                ringProfile.Radiate = reader.GetAttributeEnum<CircleRadiation>(nameof(RingProfile.Radiate));
                return ringProfile;

            case nameof(SprayProfile):
                SprayProfile sprayProfile = new SprayProfile();
                sprayProfile.Direction = reader.GetAttributeVector2(nameof(SprayProfile.Direction));
                sprayProfile.Spread = reader.GetAttributeFloat(nameof(SprayProfile.Spread));
                return sprayProfile;

            default:
                return new PointProfile();
        }
    }

    private void ReadModifiers(XmlReader reader, List<Modifier> modifiers)
    {
        using XmlReader subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element && subtree.LocalName == nameof(Modifier))
            {
                Modifier modifier = ReadModifier(subtree);

                if (modifier != null)
                {
                    modifiers.Add(modifier);
                }
            }
        }
    }

    private Modifier ReadModifier(XmlReader reader)
    {
        string type = reader.GetAttribute(nameof(Type));
        string name = reader.GetAttribute(nameof(Modifier.Name));
        float frequency = reader.GetAttributeFloat(nameof(Modifier.Frequency));

        Modifier modifier = type switch
        {
            nameof(AgeModifier) => ReadAgeModifier(reader),
            nameof(DragModifier) => ReadDragModifier(reader),
            nameof(LinearGravityModifier) => ReadLinearGravityModifier(reader),
            nameof(OpacityFastFadeModifier) => new OpacityFastFadeModifier(),
            nameof(RotationModifier) => ReadRotationModifier(reader),
            nameof(VelocityColorModifier) => ReadVelocityColorModifier(reader),
            nameof(VelocityModifier) => ReadVelocityModifier(reader),
            nameof(VortexModifier) => ReadVortexModifier(reader),
            nameof(CircleContainerModifier) => ReadCircleContainerModifier(reader),
            nameof(RectangleContainerModifier) => ReadRectangleContainerModifier(reader),
            nameof(RectangleLoopContainerModifier) => ReadRectangleLoopContainerModifier(reader),
            _ => null
        };

        if (modifier != null)
        {
            modifier.Name = name;
            modifier.Frequency = frequency;
        }

        return modifier;
    }

    private Modifier ReadAgeModifier(XmlReader reader)
    {
        AgeModifier modifier = new AgeModifier();

        using XmlReader subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element && subtree.LocalName == nameof(AgeModifier.Interpolators))
            {
                ReadInterpolators(subtree, modifier.Interpolators);
            }
        }

        return modifier;
    }

    private Modifier ReadDragModifier(XmlReader reader)
    {
        DragModifier modifier = new DragModifier();
        modifier.DragCoefficient = reader.GetAttributeFloat(nameof(DragModifier.DragCoefficient));
        modifier.Density = reader.GetAttributeFloat(nameof(DragModifier.Density));
        return modifier;
    }

    private LinearGravityModifier ReadLinearGravityModifier(XmlReader reader)
    {
        LinearGravityModifier modifier = new LinearGravityModifier();
        modifier.Direction = reader.GetAttributeVector2(nameof(LinearGravityModifier.Direction));
        modifier.Strength = reader.GetAttributeFloat(nameof(LinearGravityModifier.Strength));
        return modifier;
    }

    private Modifier ReadRotationModifier(XmlReader reader)
    {
        RotationModifier modifier = new RotationModifier();
        modifier.RotationRate = reader.GetAttributeFloat(nameof(RotationModifier.RotationRate));
        return modifier;
    }

    private Modifier ReadVelocityColorModifier(XmlReader reader)
    {
        VelocityColorModifier modifier = new VelocityColorModifier();
        Vector3 stationaryColor = reader.GetAttributeVector3(nameof(VelocityColorModifier.StationaryColor));
        modifier.StationaryColor = new HslColor(stationaryColor.X, stationaryColor.Y, stationaryColor.Z);
        Vector3 velocityColor = reader.GetAttributeVector3(nameof(VelocityColorModifier.VelocityColor));
        modifier.VelocityColor = new HslColor(velocityColor.X, velocityColor.Y, velocityColor.Z);
        modifier.VelocityThreshold = reader.GetAttributeFloat(nameof(VelocityColorModifier.VelocityThreshold));
        return modifier;
    }

    private Modifier ReadVelocityModifier(XmlReader reader)
    {
        VelocityModifier modifier = new VelocityModifier();
        modifier.VelocityThreshold = reader.GetAttributeFloat(nameof(VelocityModifier.VelocityThreshold));

        using XmlReader subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element && subtree.LocalName == nameof(VelocityModifier.Interpolators))
            {
                ReadInterpolators(subtree, modifier.Interpolators);
            }
        }

        return modifier;
    }

    private Modifier ReadVortexModifier(XmlReader reader)
    {
        VortexModifier modifier = new VortexModifier();
        modifier.Position = reader.GetAttributeVector2(nameof(VortexModifier.Position));
        modifier.Strength = reader.GetAttributeFloat(nameof(VortexModifier.Strength));
        modifier.OuterRadius = reader.GetAttributeFloat(nameof(VortexModifier.OuterRadius));
        modifier.InnerRadius = reader.GetAttributeFloat(nameof(VortexModifier.InnerRadius));
        modifier.MaxVelocity = reader.GetAttributeFloat(nameof(VortexModifier.MaxVelocity));
        modifier.RotationAngle = reader.GetAttributeFloat(nameof(VortexModifier.RotationAngle));
        return modifier;
    }

    private CircleContainerModifier ReadCircleContainerModifier(XmlReader reader)
    {
        CircleContainerModifier modifier = new CircleContainerModifier();
        modifier.Radius = reader.GetAttributeFloat(nameof(CircleContainerModifier.Radius));
        modifier.Inside = reader.GetAttributeBool(nameof(CircleContainerModifier.Inside));
        modifier.RestitutionCoefficient = reader.GetAttributeFloat(nameof(CircleContainerModifier.RestitutionCoefficient));
        return modifier;
    }

    private Modifier ReadRectangleContainerModifier(XmlReader reader)
    {
        RectangleContainerModifier modifier = new RectangleContainerModifier();
        modifier.Width = reader.GetAttributeInt(nameof(RectangleContainerModifier.Width));
        modifier.Height = reader.GetAttributeInt(nameof(RectangleContainerModifier.Height));
        modifier.RestitutionCoefficient = reader.GetAttributeFloat(nameof(RectangleContainerModifier.RestitutionCoefficient));
        return modifier;
    }

    private Modifier ReadRectangleLoopContainerModifier(XmlReader reader)
    {
        RectangleLoopContainerModifier modifier = new RectangleLoopContainerModifier();
        modifier.Width = reader.GetAttributeInt(nameof(RectangleLoopContainerModifier.Width));
        modifier.Height = reader.GetAttributeInt(nameof(RectangleLoopContainerModifier.Height));
        return modifier;
    }

    private void ReadInterpolators(XmlReader reader, List<Interpolator> interpolators)
    {
        using XmlReader subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element && subtree.LocalName == nameof(Interpolator))
            {
                Interpolator interpolator = ReadInterpolator(subtree);

                if (interpolator != null)
                {
                    interpolators.Add(interpolator);
                }
            }
        }
    }

    private Interpolator ReadInterpolator(XmlReader reader)
    {
        string type = reader.GetAttribute(nameof(Type));
        string name = reader.GetAttribute(nameof(Interpolator.Name));

        switch (type)
        {
            case nameof(ColorInterpolator):
                ColorInterpolator colorInterpolator = new ColorInterpolator();
                Vector3 startValue = reader.GetAttributeVector3(nameof(ColorInterpolator.StartValue));
                Vector3 endValue = reader.GetAttributeVector3(nameof(ColorInterpolator.EndValue));
                colorInterpolator.StartValue = new HslColor(startValue.X, startValue.Y, startValue.Z);
                colorInterpolator.EndValue = new HslColor(endValue.X, endValue.Y, endValue.Z);
                return colorInterpolator;

            case nameof(HueInterpolator):
                HueInterpolator hueInterpolator = new HueInterpolator();
                hueInterpolator.StartValue = reader.GetAttributeFloat(nameof(HueInterpolator.StartValue));
                hueInterpolator.EndValue = reader.GetAttributeFloat(nameof(HueInterpolator.EndValue));
                return hueInterpolator;

            case nameof(OpacityInterpolator):
                OpacityInterpolator opacityInterpolator = new OpacityInterpolator();
                opacityInterpolator.StartValue = reader.GetAttributeFloat(nameof(OpacityInterpolator.StartValue));
                opacityInterpolator.EndValue = reader.GetAttributeFloat(nameof(OpacityInterpolator.EndValue));
                return opacityInterpolator;

            case nameof(RotationInterpolator):
                RotationInterpolator rotationInterpolator = new RotationInterpolator();
                rotationInterpolator.StartValue = reader.GetAttributeFloat(nameof(RotationInterpolator.StartValue));
                rotationInterpolator.EndValue = reader.GetAttributeFloat(nameof(RotationInterpolator.EndValue));
                return rotationInterpolator;

            case nameof(ScaleInterpolator):
                ScaleInterpolator scaleInterpolator = new ScaleInterpolator();
                scaleInterpolator.StartValue = reader.GetAttributeVector2(nameof(ScaleInterpolator.StartValue));
                scaleInterpolator.EndValue = reader.GetAttributeVector2(nameof(ScaleInterpolator.EndValue));
                return scaleInterpolator;

            case nameof(VelocityInterpolator):
                VelocityInterpolator velocityInterpolator = new VelocityInterpolator();
                velocityInterpolator.StartValue = reader.GetAttributeVector2(nameof(VelocityInterpolator.StartValue));
                velocityInterpolator.EndValue = reader.GetAttributeVector2(nameof(VelocityInterpolator.EndValue));
                return velocityInterpolator;

            default:
                return null;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        _reader?.Dispose();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}
