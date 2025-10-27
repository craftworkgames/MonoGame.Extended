using System;
using System.Collections.Generic;
using System.IO;
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
/// Provides static methods for serializing and deserializing <see cref="ParticleEffect"/> instances to and from XML.
/// </summary>
public static class ParticleEffectSerializer
{
    #region Deserialize

    /// <summary>
    /// Deserializes a <see cref="ParticleEffect"/> from an XML file.
    /// </summary>
    /// <param name="fileName">The file path to read the XML from</param>
    /// <param name="content">The <see cref="ContentManager"/> to use for loading texture.</param>
    /// <returns>The deserialized <see cref="ParticleEffect"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="content"/> or <paramref name="fileName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fileName"/> is empty.</exception>
    /// <exception cref="XmlException">Thrown when the XMl format is invalid.</exception>
    public static ParticleEffect Deserialize(string fileName, ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.CloseInput = true;
        settings.IgnoreComments = true;
        settings.IgnoreWhitespace = true;

        string fullPath = Path.GetFullPath(fileName);
        string baseDirectory = Path.GetDirectoryName(fullPath);

        using XmlReader reader = XmlReader.Create(fileName, settings);
        return Deserialize(reader, content, baseDirectory);
    }

    /// <summary>
    /// Deserializes a <see cref="ParticleEffect"/> from a stream containing XML data.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="content">The <see cref="ContentManager"/> to use for loading textures.</param>
    /// <param name="baseDirectory">
    /// The base directory to use for resolving relative texture paths.
    /// If <see langword="null"/>, uses the <see cref="ContentManager.RootDirectory"/> property of <paramref name="content"/>.
    /// </param>
    /// <returns>The deserialized <see cref="ParticleEffect"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="stream"/> or <paramref name="content"/> are <see langword="null"/>.
    /// </exception>
    /// <exception cref="XmlException">Thrown when the XMl format is invalid.</exception>
    public static ParticleEffect Deserialize(Stream stream, ContentManager content, string baseDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(content);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.CloseInput = false;
        settings.IgnoreComments = true;
        settings.IgnoreWhitespace = true;

        if (string.IsNullOrEmpty(baseDirectory))
        {
            baseDirectory = content.RootDirectory;
        }

        using XmlReader reader = XmlReader.Create(stream, settings);
        return Deserialize(reader, content, baseDirectory);
    }

    private static ParticleEffect Deserialize(XmlReader reader, ContentManager content, string baseDirectory)
    {
        reader.MoveToContent();

        if (reader.NodeType != XmlNodeType.Element || reader.LocalName != nameof(ParticleEffect))
        {
            throw new XmlException($"Expected {nameof(ParticleEffect)} root element");
        }

        string name = reader.GetAttribute(nameof(ParticleEffect.Name)) ?? nameof(ParticleEffect);
        ParticleEffect effect = new ParticleEffect(name);

        effect.Position = reader.GetAttributeVector2(nameof(ParticleEffect.Position), default);
        effect.Rotation = reader.GetAttributeFloat(nameof(ParticleEffect.Rotation), default);
        effect.Scale = reader.GetAttributeVector2(nameof(ParticleEffect.Scale), default);
        effect.AutoTrigger = reader.GetAttributeBool(nameof(ParticleEffect.AutoTrigger), default);
        effect.AutoTriggerFrequency = reader.GetAttributeFloat(nameof(ParticleEffect.AutoTriggerFrequency), default);

        if (reader.ReadToDescendant(nameof(ParticleEffect.Emitters)))
        {
            if (reader.ReadToDescendant(nameof(ParticleEmitter)))
            {
                do
                {
                    ParticleEmitter emitter = ReadParticleEmitter(reader, content, baseDirectory);
                    effect.Emitters.Add(emitter);
                } while (reader.ReadToNextSibling(nameof(ParticleEmitter)));
            }
        }

        return effect;
    }

    private static ParticleEmitter ReadParticleEmitter(XmlReader reader, ContentManager content, string baseDirectory)
    {
        int capacity = reader.GetAttributeInt(nameof(ParticleEmitter.Capacity), default);

        ParticleEmitter emitter = new ParticleEmitter(capacity);
        emitter.Name = reader.GetAttribute(nameof(ParticleEmitter.Name)) ?? nameof(ParticleEmitter);
        emitter.LifeSpan = reader.GetAttributeFloat(nameof(ParticleEmitter.LifeSpan), default);
        emitter.Offset = reader.GetAttributeVector2(nameof(ParticleEmitter.Offset), default);
        emitter.LayerDepth = reader.GetAttributeFloat(nameof(ParticleEmitter.LayerDepth), default);
        emitter.ReclaimFrequency = reader.GetAttributeFloat(nameof(ParticleEmitter.ReclaimFrequency), default);

        string strategy = reader.GetAttribute(nameof(ParticleEmitter.ModifierExecutionStrategy));

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

        emitter.RenderingOrder = reader.GetAttributeEnum<ParticleRenderingOrder>(nameof(ParticleEmitter.RenderingOrder), default);

        using XmlReader subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
            if (subtree.NodeType == XmlNodeType.Element)
            {
                switch (subtree.LocalName)
                {
                    case nameof(ParticleEmitter.TextureRegion):
                        emitter.TextureRegion = ReadTexture2DRegion(subtree, content, baseDirectory);
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

    private static Texture2DRegion ReadTexture2DRegion(XmlReader reader, ContentManager content, string baseDirectory)
    {
        string name = reader.GetAttribute(nameof(Texture2DRegion.Texture.Name));

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        string path = Path.Combine(baseDirectory, name);

        // Content manager will throw exception if the path given is a rooted path
        if (Path.IsPathRooted(path))
        {
            path = Path.GetRelativePath(baseDirectory, path);
        }

        Texture2D texture = content.Load<Texture2D>(path);
        if(string.IsNullOrEmpty(texture.Name))
        {
            texture.Name = Path.GetFileName(path);
        }

        Rectangle bounds = reader.GetAttributeRectangle(nameof(Texture2DRegion.Bounds), default);

        if (bounds.IsEmpty)
        {
            bounds = texture.Bounds;
        }

        return new Texture2DRegion(texture, bounds);
    }

    private static ParticleReleaseParameters ReadParticleReleaseParameters(XmlReader reader)
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

    private static ParticleInt32Parameter ReadParticleInt32Parameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleInt32Parameter.Kind), default);

        if (kind == ParticleValueKind.Constant)
        {
            int value = reader.GetAttributeInt(nameof(ParticleInt32Parameter.Constant), default);
            return new ParticleInt32Parameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            int min = reader.GetAttributeInt(nameof(ParticleInt32Parameter.RandomMin), default);
            int max = reader.GetAttributeInt(nameof(ParticleInt32Parameter.RandomMax), default);
            return new ParticleInt32Parameter(min, max);
        }

        return new ParticleInt32Parameter(0);
    }

    private static ParticleFloatParameter ReadParticleFloatParameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleFloatParameter.Kind), default);

        if (kind == ParticleValueKind.Constant)
        {
            float value = reader.GetAttributeFloat(nameof(ParticleFloatParameter.Constant), default);
            return new ParticleFloatParameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            float min = reader.GetAttributeFloat(nameof(ParticleFloatParameter.RandomMin), default);
            float max = reader.GetAttributeFloat(nameof(ParticleFloatParameter.RandomMax), default);
            return new ParticleFloatParameter(min, max);
        }

        return new ParticleFloatParameter(0);
    }

    private static ParticleVector2Parameter ReadParticleVector2Parameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleVector2Parameter.Kind), default);

        if (kind == ParticleValueKind.Constant)
        {
            Vector2 value = reader.GetAttributeVector2(nameof(ParticleVector2Parameter.Constant), default);
            return new ParticleVector2Parameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            Vector2 min = reader.GetAttributeVector2(nameof(ParticleVector2Parameter.RandomMin), default);
            Vector2 max = reader.GetAttributeVector2(nameof(ParticleVector2Parameter.RandomMax), default);
            return new ParticleVector2Parameter(min, max);
        }

        return new ParticleVector2Parameter(Vector2.Zero);
    }

    private static ParticleColorParameter ReadParticleColorParameter(XmlReader reader)
    {
        ParticleValueKind kind = reader.GetAttributeEnum<ParticleValueKind>(nameof(ParticleColorParameter.Kind), default);

        if (kind == ParticleValueKind.Constant)
        {
            Vector3 value = reader.GetAttributeVector3(nameof(ParticleColorParameter.Constant), default);
            return new ParticleColorParameter(value);
        }
        else if (kind == ParticleValueKind.Random)
        {
            Vector3 min = reader.GetAttributeVector3(nameof(ParticleColorParameter.RandomMin), default);
            Vector3 max = reader.GetAttributeVector3(nameof(ParticleColorParameter.RandomMax), default);
            return new ParticleColorParameter(min, max);
        }

        return new ParticleColorParameter(Vector3.Zero);
    }

    private static Profile ReadProfile(XmlReader reader)
    {
        string type = reader.GetAttribute(nameof(Type));

        return type switch
        {
            nameof(BoxProfile) => ReadBoxProfile(reader),
            nameof(BoxFillProfile) => ReadBoxFillProfile(reader),
            nameof(BoxUniformProfile) => ReadBoxUniformProfile(reader),
            nameof(CircleProfile) => ReadCircleProfile(reader),
            nameof(LineProfile) => ReadLineProfile(reader),
            nameof(PointProfile) => ReadPointProfile(reader),
            nameof(RingProfile) => ReadRingProfile(reader),
            nameof(SprayProfile) => ReadSprayProfile(reader),
            _ => ReadPointProfile(reader)
        };
    }

    private static BoxProfile ReadBoxProfile(XmlReader reader)
    {
        float width = reader.GetAttributeFloat(nameof(BoxProfile.Width), default);
        float height = reader.GetAttributeFloat(nameof(BoxProfile.Height), default);

        return new BoxProfile { Width = width, Height = height };
    }

    private static BoxFillProfile ReadBoxFillProfile(XmlReader reader)
    {
        float width = reader.GetAttributeFloat(nameof(BoxFillProfile.Width), default);
        float height = reader.GetAttributeFloat(nameof(BoxFillProfile.Height), default);

        return new BoxFillProfile { Width = width, Height = height };
    }

    private static BoxUniformProfile ReadBoxUniformProfile(XmlReader reader)
    {
        float width = reader.GetAttributeFloat(nameof(BoxUniformProfile.Width), default);
        float height = reader.GetAttributeFloat(nameof(BoxUniformProfile.Height), default);

        return new BoxUniformProfile { Width = width, Height = height };
    }

    private static CircleProfile ReadCircleProfile(XmlReader reader)
    {
        float radius = reader.GetAttributeFloat(nameof(CircleProfile.Radius), default);
        CircleRadiation radiation = reader.GetAttributeEnum<CircleRadiation>(nameof(CircleProfile.Radiate), default);

        return new CircleProfile { Radius = radius, Radiate = radiation };
    }

    private static LineProfile ReadLineProfile(XmlReader reader)
    {
        Vector2 axis = reader.GetAttributeVector2(nameof(LineProfile.Axis), default);
        float length = reader.GetAttributeFloat(nameof(LineProfile.Length), default);
        Vector2 direction = reader.GetAttributeVector2(nameof(LineProfile.Direction), default);
        LineRadiation radiate = reader.GetAttributeEnum<LineRadiation>(nameof(LineProfile.Radiate), default);

        return new LineProfile { Axis = axis, Length = length, Direction = direction, Radiate = radiate };
    }

    private static PointProfile ReadPointProfile(XmlReader reader)
    {
        return new PointProfile();
    }

    private static RingProfile ReadRingProfile(XmlReader reader)
    {
        float radius = reader.GetAttributeFloat(nameof(RingProfile.Radius), default);
        CircleRadiation radiation = reader.GetAttributeEnum<CircleRadiation>(nameof(RingProfile.Radiate), default);

        return new RingProfile { Radius = radius, Radiate = radiation };
    }

    private static SprayProfile ReadSprayProfile(XmlReader reader)
    {
        Vector2 direction = reader.GetAttributeVector2(nameof(SprayProfile.Direction), default);
        float spread = reader.GetAttributeFloat(nameof(SprayProfile.Spread), default);

        return new SprayProfile { Direction = direction, Spread = spread };
    }

    private static void ReadModifiers(XmlReader reader, List<Modifier> modifiers)
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

    private static Modifier ReadModifier(XmlReader reader)
    {
        string type = reader.GetAttribute(nameof(Type));
        string name = reader.GetAttribute(nameof(Modifier.Name));
        float frequency = reader.GetAttributeFloat(nameof(Modifier.Frequency), default);
        bool enabled = reader.GetAttributeBool(nameof(Modifier.Enabled), default);

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
            modifier.Enabled = enabled;
        }

        return modifier;
    }

    private static AgeModifier ReadAgeModifier(XmlReader reader)
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

    private static DragModifier ReadDragModifier(XmlReader reader)
    {
        float dragCoefficient = reader.GetAttributeFloat(nameof(DragModifier.DragCoefficient), default);
        float density = reader.GetAttributeFloat(nameof(DragModifier.Density), default);

        return new DragModifier() { DragCoefficient = dragCoefficient, Density = density };
    }

    private static LinearGravityModifier ReadLinearGravityModifier(XmlReader reader)
    {
        Vector2 direction = reader.GetAttributeVector2(nameof(LinearGravityModifier.Direction), default);
        float strength = reader.GetAttributeFloat(nameof(LinearGravityModifier.Strength), default);

        return new LinearGravityModifier() { Direction = direction, Strength = strength };
    }

    private static RotationModifier ReadRotationModifier(XmlReader reader)
    {
        float rotationRate = reader.GetAttributeFloat(nameof(RotationModifier.RotationRate), default);

        return new RotationModifier() { RotationRate = rotationRate };
    }

    private static VelocityColorModifier ReadVelocityColorModifier(XmlReader reader)
    {
        Vector3 stationaryColorValue = reader.GetAttributeVector3(nameof(VelocityColorModifier.StationaryColor), default);
        Vector3 velocityColorValue = reader.GetAttributeVector3(nameof(VelocityColorModifier.VelocityColor), default);
        float velocityThreshold = reader.GetAttributeFloat(nameof(VelocityColorModifier.VelocityThreshold), default);

        HslColor stationaryColor = new HslColor(stationaryColorValue.X, stationaryColorValue.Y, stationaryColorValue.Z);
        HslColor velocityColor = new HslColor(velocityColorValue.X, velocityColorValue.Y, velocityColorValue.Z);

        return new VelocityColorModifier() { StationaryColor = stationaryColor, VelocityColor = velocityColor, VelocityThreshold = velocityThreshold };
    }

    private static VelocityModifier ReadVelocityModifier(XmlReader reader)
    {
        float velocityThreshold = reader.GetAttributeFloat(nameof(VelocityModifier.VelocityThreshold), default);

        VelocityModifier modifier = new VelocityModifier() { VelocityThreshold = velocityThreshold };

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

    private static VortexModifier ReadVortexModifier(XmlReader reader)
    {
        Vector2 position = reader.GetAttributeVector2(nameof(VortexModifier.Position), default);
        float strength = reader.GetAttributeFloat(nameof(VortexModifier.Strength), default);
        float outerRadius = reader.GetAttributeFloat(nameof(VortexModifier.OuterRadius), default);
        float innerRadius = reader.GetAttributeFloat(nameof(VortexModifier.InnerRadius), default);
        float maxVelocity = reader.GetAttributeFloat(nameof(VortexModifier.MaxVelocity), default);
        float rotationAngle = reader.GetAttributeFloat(nameof(VortexModifier.RotationAngle), default);

        return new VortexModifier { Position = position, Strength = strength, OuterRadius = outerRadius, InnerRadius = innerRadius, MaxVelocity = maxVelocity, RotationAngle = rotationAngle };
    }

    private static CircleContainerModifier ReadCircleContainerModifier(XmlReader reader)
    {
        float radius = reader.GetAttributeFloat(nameof(CircleContainerModifier.Radius), default);
        bool inside = reader.GetAttributeBool(nameof(CircleContainerModifier.Inside), default);
        float restitutionCoefficient = reader.GetAttributeFloat(nameof(CircleContainerModifier.RestitutionCoefficient), default);

        return new CircleContainerModifier() { Radius = radius, Inside = inside, RestitutionCoefficient = restitutionCoefficient };
    }

    private static RectangleContainerModifier ReadRectangleContainerModifier(XmlReader reader)
    {
        int width = reader.GetAttributeInt(nameof(RectangleContainerModifier.Width), default);
        int height = reader.GetAttributeInt(nameof(RectangleContainerModifier.Height), default);
        float restitutionCoefficient = reader.GetAttributeFloat(nameof(RectangleContainerModifier.RestitutionCoefficient), default);

        return new RectangleContainerModifier() { Width = width, Height = height, RestitutionCoefficient = restitutionCoefficient };
    }

    private static RectangleLoopContainerModifier ReadRectangleLoopContainerModifier(XmlReader reader)
    {
        int width = reader.GetAttributeInt(nameof(RectangleLoopContainerModifier.Width), default);
        int height = reader.GetAttributeInt(nameof(RectangleLoopContainerModifier.Height), default);

        return new RectangleLoopContainerModifier() { Width = width, Height = height };
    }

    private static void ReadInterpolators(XmlReader reader, List<Interpolator> interpolators)
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

    private static Interpolator ReadInterpolator(XmlReader reader)
    {
        string type = reader.GetAttribute(nameof(Type));
        string name = reader.GetAttribute(nameof(Interpolator.Name));
        bool enabled = reader.GetAttributeBool(nameof(Interpolator.Enabled), default);

        Interpolator interpolator = type switch
        {
            nameof(ColorInterpolator) => ReadColorInterpolator(reader),
            nameof(HueInterpolator) => ReadHueInterpolator(reader),
            nameof(OpacityInterpolator) => ReadOpacityInterpolator(reader),
            nameof(RotationInterpolator) => ReadRotationInterpolator(reader),
            nameof(ScaleInterpolator) => ReadScaleInterpolator(reader),
            nameof(VelocityInterpolator) => ReadVelocityInterpolator(reader),
            _ => null
        };

        if (interpolator != null)
        {
            interpolator.Name = name;
        }

        interpolator.Enabled = enabled;

        return interpolator;
    }

    private static ColorInterpolator ReadColorInterpolator(XmlReader reader)
    {
        Vector3 start = reader.GetAttributeVector3(nameof(ColorInterpolator.StartValue), default);
        Vector3 end = reader.GetAttributeVector3(nameof(ColorInterpolator.EndValue), default);

        HslColor startValue = new HslColor(start.X, start.Y, start.Z);
        HslColor endValue = new HslColor(end.X, end.Y, end.Z);

        return new ColorInterpolator() { StartValue = startValue, EndValue = endValue };
    }

    private static HueInterpolator ReadHueInterpolator(XmlReader reader)
    {
        float startValue = reader.GetAttributeFloat(nameof(HueInterpolator.StartValue), default);
        float endValue = reader.GetAttributeFloat(nameof(HueInterpolator.EndValue), default);

        return new HueInterpolator() { StartValue = startValue, EndValue = endValue };
    }

    private static OpacityInterpolator ReadOpacityInterpolator(XmlReader reader)
    {
        float startValue = reader.GetAttributeFloat(nameof(OpacityInterpolator.StartValue), default);
        float endValue = reader.GetAttributeFloat(nameof(OpacityInterpolator.EndValue), default);

        return new OpacityInterpolator() { StartValue = startValue, EndValue = endValue };
    }

    private static RotationInterpolator ReadRotationInterpolator(XmlReader reader)
    {
        float startValue = reader.GetAttributeFloat(nameof(RotationInterpolator.StartValue), default);
        float endValue = reader.GetAttributeFloat(nameof(RotationInterpolator.EndValue), default);

        return new RotationInterpolator() { StartValue = startValue, EndValue = endValue };
    }

    private static ScaleInterpolator ReadScaleInterpolator(XmlReader reader)
    {
        Vector2 startValue = reader.GetAttributeVector2(nameof(ScaleInterpolator.StartValue), default);
        Vector2 endValue = reader.GetAttributeVector2(nameof(ScaleInterpolator.EndValue), default);

        return new ScaleInterpolator() { StartValue = startValue, EndValue = endValue };
    }

    private static VelocityInterpolator ReadVelocityInterpolator(XmlReader reader)
    {
        Vector2 startValue = reader.GetAttributeVector2(nameof(VelocityInterpolator.StartValue), default);
        Vector2 endValue = reader.GetAttributeVector2(nameof(VelocityInterpolator.EndValue), default);

        return new VelocityInterpolator() { StartValue = startValue, EndValue = endValue };
    }

    #endregion Deserialize

    #region Serialize

    /// <summary>
    /// Serializes a <see cref="ParticleEffect"/> to an XML file.
    /// </summary>
    /// <param name="fileName">The file path to write the XML to.</param>
    /// <param name="effect">The <see cref="ParticleEffect"/> to serialize.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fileName"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException">
    /// Throw if either <paramref name="fileName"/> or <paramref name="effect"/> are <see langword="null"/>.
    /// </exception>
    public static void Serialize(string fileName, ParticleEffect effect)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        ArgumentNullException.ThrowIfNull(effect);

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CloseOutput = true;
        settings.NewLineChars = "\n";

        using XmlWriter writer = XmlWriter.Create(fileName, settings);
        Serialize(writer, effect);
    }

    /// <summary>
    /// Serializes a <see cref="ParticleEffect"/> to a stream as XML data.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="effect">The <see cref="ParticleEffect"/> to serialize.</param>
    /// <exception cref="ArgumentNullException">
    /// Throw if either <paramref name="stream"/> or <paramref name="effect"/> are <see langword="null"/>.
    /// </exception>
    public static void Serialize(Stream stream, ParticleEffect effect)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(effect);

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CloseOutput = false;
        settings.NewLineChars = "\n";

        using XmlWriter writer = XmlWriter.Create(stream, settings);
        Serialize(writer, effect);
    }

    private static void Serialize(XmlWriter writer, ParticleEffect effect)
    {
        writer.WriteStartDocument();
        writer.WriteStartElement(nameof(ParticleEffect));

        writer.WriteAttributeString(nameof(ParticleEffect.Name), effect.Name);
        writer.WriteAttributeVector2(nameof(ParticleEffect.Position), effect.Position);
        writer.WriteAttributeFloat(nameof(ParticleEffect.Rotation), effect.Rotation);
        writer.WriteAttributeVector2(nameof(ParticleEffect.Scale), effect.Scale);
        writer.WriteAttributeBool(nameof(ParticleEffect.AutoTrigger), effect.AutoTrigger);
        writer.WriteAttributeFloat(nameof(ParticleEffect.AutoTriggerFrequency), effect.AutoTriggerFrequency);

        if (effect.Emitters.Count > 0)
        {
            writer.WriteStartElement(nameof(ParticleEffect.Emitters));
            foreach (ParticleEmitter emitter in effect.Emitters)
            {
                writer.WriteStartElement(nameof(ParticleEmitter));
                WriteParticleEmitter(writer, emitter);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();

        writer.Flush();
    }

    private static void WriteParticleEmitter(XmlWriter writer, ParticleEmitter emitter)
    {
        writer.WriteAttributeString(nameof(ParticleEmitter.Name), emitter.Name);
        writer.WriteAttributeFloat(nameof(ParticleEmitter.LifeSpan), emitter.LifeSpan);
        writer.WriteAttributeVector2(nameof(ParticleEmitter.Offset), emitter.Offset);
        writer.WriteAttributeFloat(nameof(ParticleEmitter.LayerDepth), emitter.LayerDepth);
        writer.WriteAttributeFloat(nameof(ParticleEmitter.ReclaimFrequency), emitter.ReclaimFrequency);
        writer.WriteAttributeInt(nameof(ParticleEmitter.Capacity), emitter.Capacity);
        writer.WriteAttributeString(nameof(ParticleEmitter.ModifierExecutionStrategy), emitter.ModifierExecutionStrategy.ToString());
        writer.WriteAttributeString(nameof(ParticleEmitter.RenderingOrder), emitter.RenderingOrder.ToString());

        if (emitter.TextureRegion is Texture2DRegion region)
        {
            writer.WriteStartElement(nameof(ParticleEmitter.TextureRegion));
            WriteTexture2DRegion(writer, region);
            writer.WriteEndElement();
        }

        writer.WriteStartElement(nameof(ParticleEmitter.Parameters));
        WriteParticleReleaseParameters(writer, emitter.Parameters);
        writer.WriteEndElement();

        writer.WriteStartElement(nameof(ParticleEmitter.Profile));
        WriteProfile(writer, emitter.Profile);
        writer.WriteEndElement();


        if (emitter.Modifiers.Count > 0)
        {
            writer.WriteStartElement(nameof(ParticleEmitter.Modifiers));
            foreach (Modifier modifier in emitter.Modifiers)
            {
                writer.WriteStartElement(nameof(Modifier));
                WriteModifier(writer, modifier);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

    private static void WriteTexture2DRegion(XmlWriter writer, Texture2DRegion region)
    {
        writer.WriteAttributeString(nameof(Texture2DRegion.Texture.Name), region.Texture.Name);
        writer.WriteAttributeRectangle(nameof(Texture2DRegion.Bounds), region.Bounds);
    }

    private static void WriteParticleReleaseParameters(XmlWriter writer, ParticleReleaseParameters parameters)
    {
        WriteParticleInt32Parameter(writer, nameof(ParticleReleaseParameters.Quantity), parameters.Quantity);
        WriteParticleFloatParameter(writer, nameof(ParticleReleaseParameters.Speed), parameters.Speed);
        WriteParticleColorParameter(writer, nameof(ParticleReleaseParameters.Color), parameters.Color);
        WriteParticleFloatParameter(writer, nameof(ParticleReleaseParameters.Opacity), parameters.Opacity);
        WriteParticleVector2Parameter(writer, nameof(ParticleReleaseParameters.Scale), parameters.Scale);
        WriteParticleFloatParameter(writer, nameof(ParticleReleaseParameters.Rotation), parameters.Rotation);
        WriteParticleFloatParameter(writer, nameof(ParticleReleaseParameters.Mass), parameters.Mass);
    }

    private static void WriteParticleInt32Parameter(XmlWriter writer, string name, ParticleInt32Parameter parameter)
    {
        writer.WriteStartElement(name);
        writer.WriteAttributeString(nameof(ParticleInt32Parameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            writer.WriteAttributeInt(nameof(ParticleInt32Parameter.Constant), parameter.Constant);
        }
        else
        {
            writer.WriteAttributeInt(nameof(ParticleInt32Parameter.RandomMin), parameter.RandomMin);
            writer.WriteAttributeInt(nameof(ParticleInt32Parameter.RandomMax), parameter.RandomMax);
        }

        writer.WriteEndElement();
    }

    private static void WriteParticleFloatParameter(XmlWriter writer, string name, ParticleFloatParameter parameter)
    {
        writer.WriteStartElement(name);
        writer.WriteAttributeString(nameof(ParticleFloatParameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            writer.WriteAttributeFloat(nameof(ParticleFloatParameter.Constant), parameter.Constant);
        }
        else
        {
            writer.WriteAttributeFloat(nameof(ParticleFloatParameter.RandomMin), parameter.RandomMin);
            writer.WriteAttributeFloat(nameof(ParticleFloatParameter.RandomMax), parameter.RandomMax);
        }

        writer.WriteEndElement();
    }

    private static void WriteParticleVector2Parameter(XmlWriter writer, string name, ParticleVector2Parameter parameter)
    {
        writer.WriteStartElement(name);
        writer.WriteAttributeString(nameof(ParticleVector2Parameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            writer.WriteAttributeVector2(nameof(ParticleVector2Parameter.Constant), parameter.Constant);
        }
        else
        {
            writer.WriteAttributeVector2(nameof(ParticleVector2Parameter.RandomMin), parameter.RandomMin);
            writer.WriteAttributeVector2(nameof(ParticleVector2Parameter.RandomMax), parameter.RandomMax);
        }

        writer.WriteEndElement();
    }

    private static void WriteParticleColorParameter(XmlWriter writer, string name, ParticleColorParameter parameter)
    {
        writer.WriteStartElement(name);
        writer.WriteAttributeString(nameof(ParticleColorParameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            writer.WriteAttributeVector3(nameof(ParticleColorParameter.Constant), parameter.Constant);
        }
        else
        {
            writer.WriteAttributeVector3(nameof(ParticleColorParameter.RandomMin), parameter.RandomMin);
            writer.WriteAttributeVector3(nameof(ParticleColorParameter.RandomMax), parameter.RandomMax);
        }

        writer.WriteEndElement();
    }

    private static void WriteProfile(XmlWriter writer, Profile profile)
    {
        switch (profile)
        {
            case BoxFillProfile boxFillProfile:
                WriteBoxFillProfile(writer, boxFillProfile);
                break;

            case BoxProfile boxProfile:
                WriteBoxProfile(writer, boxProfile);
                break;

            case BoxUniformProfile boxUniformProfile:
                WriteBoxUniformProfile(writer, boxUniformProfile);
                break;

            case CircleProfile circleProfile:
                WriteCircleProfile(writer, circleProfile);
                break;

            case LineProfile lineProfile:
                WriteLineProfile(writer, lineProfile);
                break;

            case PointProfile pointProfile:
                WritePointProfile(writer, pointProfile);
                break;

            case RingProfile ringProfile:
                WriteRingProfile(writer, ringProfile);
                break;

            case SprayProfile sprayProfile:
                WriteSprayProfile(writer, sprayProfile);
                break;

            default:
                writer.WriteAttributeString(nameof(Type), profile.GetType().ToString());
                break;
        }
    }

    private static void WriteBoxFillProfile(XmlWriter writer, BoxFillProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(BoxFillProfile));
        writer.WriteAttributeFloat(nameof(BoxFillProfile.Width), profile.Width);
        writer.WriteAttributeFloat(nameof(BoxFillProfile.Height), profile.Height);
    }

    private static void WriteBoxProfile(XmlWriter writer, BoxProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(BoxProfile));
        writer.WriteAttributeFloat(nameof(BoxProfile.Width), profile.Width);
        writer.WriteAttributeFloat(nameof(BoxProfile.Height), profile.Height);
    }

    private static void WriteBoxUniformProfile(XmlWriter writer, BoxUniformProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(BoxUniformProfile));
        writer.WriteAttributeFloat(nameof(BoxUniformProfile.Width), profile.Width);
        writer.WriteAttributeFloat(nameof(BoxUniformProfile.Height), profile.Height);
    }

    private static void WriteCircleProfile(XmlWriter writer, CircleProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(CircleProfile));
        writer.WriteAttributeFloat(nameof(CircleProfile.Radius), profile.Radius);
        writer.WriteAttributeString(nameof(CircleProfile.Radiate), profile.Radiate.ToString());
    }

    private static void WriteLineProfile(XmlWriter writer, LineProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(LineProfile));
        writer.WriteAttributeVector2(nameof(LineProfile.Axis), profile.Axis);
        writer.WriteAttributeFloat(nameof(LineProfile.Length), profile.Length);
        writer.WriteAttributeString(nameof(LineProfile.Radiate), profile.Radiate.ToString());
        writer.WriteAttributeVector2(nameof(LineProfile.Direction), profile.Direction);
    }

    private static void WritePointProfile(XmlWriter writer, PointProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(PointProfile));
    }

    private static void WriteRingProfile(XmlWriter writer, RingProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(RingProfile));
        writer.WriteAttributeFloat(nameof(RingProfile.Radius), profile.Radius);
        writer.WriteAttributeString(nameof(RingProfile.Radiate), profile.Radiate.ToString());
    }

    private static void WriteSprayProfile(XmlWriter writer, SprayProfile profile)
    {
        writer.WriteAttributeString(nameof(Type), nameof(SprayProfile));
        writer.WriteAttributeVector2(nameof(SprayProfile.Direction), profile.Direction);
        writer.WriteAttributeFloat(nameof(SprayProfile.Spread), profile.Spread);
    }

    private static void WriteModifier(XmlWriter writer, Modifier modifier)
    {
        writer.WriteAttributeString(nameof(Modifier.Name), modifier.Name);
        writer.WriteAttributeBool(nameof(Modifier.Enabled), modifier.Enabled);
        writer.WriteAttributeFloat(nameof(Modifier.Frequency), modifier.Frequency);

        switch (modifier)
        {
            case AgeModifier ageModifier:
                WriteAgeModifier(writer, ageModifier);
                break;

            case CircleContainerModifier circleContainerModifier:
                WriteCircleContainerModifier(writer, circleContainerModifier);
                break;

            case DragModifier dragModifier:
                WriteDragModifier(writer, dragModifier);
                break;

            case LinearGravityModifier linearGravityModifier:
                WriteLinearGravityModifier(writer, linearGravityModifier);
                break;

            case OpacityFastFadeModifier opacityFastFadeModifier:
                WriteOpacityFastFadeModifier(writer, opacityFastFadeModifier);
                break;

            case RectangleContainerModifier rectangleContainerModifier:
                WriteRectangleContainerModifier(writer, rectangleContainerModifier);
                break;

            case RectangleLoopContainerModifier rectangleLoopContainerModifier:
                WriteRectangleLoopContainerModifier(writer, rectangleLoopContainerModifier);
                break;

            case RotationModifier rotationModifier:
                WriteRotationModifier(writer, rotationModifier);
                break;

            case VelocityColorModifier velocityColorModifier:
                WriteVelocityColorModifier(writer, velocityColorModifier);
                break;

            case VelocityModifier velocityModifier:
                WriteVelocityModifier(writer, velocityModifier);
                break;

            case VortexModifier vortexModifier:
                WriteVortexModifier(writer, vortexModifier);
                break;

            default:
                writer.WriteAttributeString(nameof(Type), modifier.GetType().Name);
                break;
        }
    }

    private static void WriteAgeModifier(XmlWriter writer, AgeModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(AgeModifier));
        if (modifier.Interpolators.Count > 0)
        {
            writer.WriteStartElement(nameof(AgeModifier.Interpolators));
            foreach (Interpolator interpolator in modifier.Interpolators)
            {
                writer.WriteStartElement(nameof(Interpolator));
                WriteInterpolator(writer, interpolator);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

    private static void WriteCircleContainerModifier(XmlWriter writer, CircleContainerModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(CircleContainerModifier));
        writer.WriteAttributeFloat(nameof(CircleContainerModifier.Radius), modifier.Radius);
        writer.WriteAttributeBool(nameof(CircleContainerModifier.Inside), modifier.Inside);
        writer.WriteAttributeFloat(nameof(CircleContainerModifier.RestitutionCoefficient), modifier.RestitutionCoefficient);
    }

    private static void WriteDragModifier(XmlWriter writer, DragModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(DragModifier));
        writer.WriteAttributeFloat(nameof(DragModifier.DragCoefficient), modifier.DragCoefficient);
        writer.WriteAttributeFloat(nameof(DragModifier.Density), modifier.Density);
    }

    private static void WriteLinearGravityModifier(XmlWriter writer, LinearGravityModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(LinearGravityModifier));
        writer.WriteAttributeVector2(nameof(LinearGravityModifier.Direction), modifier.Direction);
        writer.WriteAttributeFloat(nameof(LinearGravityModifier.Strength), modifier.Strength);
    }

    private static void WriteOpacityFastFadeModifier(XmlWriter writer, OpacityFastFadeModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(OpacityFastFadeModifier));
    }

    private static void WriteRectangleContainerModifier(XmlWriter writer, RectangleContainerModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(RectangleContainerModifier));
        writer.WriteAttributeInt(nameof(RectangleContainerModifier.Width), modifier.Width);
        writer.WriteAttributeInt(nameof(RectangleContainerModifier.Height), modifier.Height);
        writer.WriteAttributeFloat(nameof(RectangleContainerModifier.RestitutionCoefficient), modifier.RestitutionCoefficient);
    }

    private static void WriteRectangleLoopContainerModifier(XmlWriter writer, RectangleLoopContainerModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(RectangleLoopContainerModifier));
        writer.WriteAttributeInt(nameof(RectangleLoopContainerModifier.Width), modifier.Width);
        writer.WriteAttributeInt(nameof(RectangleLoopContainerModifier.Height), modifier.Height);
    }

    private static void WriteRotationModifier(XmlWriter writer, RotationModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(RotationModifier));
        writer.WriteAttributeFloat(nameof(RotationModifier.RotationRate), modifier.RotationRate);
    }

    private static void WriteVelocityColorModifier(XmlWriter writer, VelocityColorModifier modifier)
    {
        Vector3 stationaryColor = new Vector3(modifier.StationaryColor.H, modifier.StationaryColor.S, modifier.StationaryColor.L);
        Vector3 velocityColor = new Vector3(modifier.VelocityColor.H, modifier.VelocityColor.S, modifier.VelocityColor.L);

        writer.WriteAttributeString(nameof(Type), nameof(VelocityColorModifier));
        writer.WriteAttributeVector3(nameof(VelocityColorModifier.StationaryColor), stationaryColor);
        writer.WriteAttributeVector3(nameof(VelocityColorModifier.VelocityColor), velocityColor);
        writer.WriteAttributeFloat(nameof(VelocityColorModifier.VelocityThreshold), modifier.VelocityThreshold);
    }

    private static void WriteVelocityModifier(XmlWriter writer, VelocityModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(VelocityModifier));
        writer.WriteAttributeFloat(nameof(VelocityModifier.VelocityThreshold), modifier.VelocityThreshold);
        if (modifier.Interpolators.Count > 0)
        {
            writer.WriteStartElement(nameof(VelocityModifier.Interpolators));
            foreach (Interpolator interpolator in modifier.Interpolators)
            {
                writer.WriteStartElement(nameof(Interpolator));
                WriteInterpolator(writer, interpolator);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

    private static void WriteVortexModifier(XmlWriter writer, VortexModifier modifier)
    {
        writer.WriteAttributeString(nameof(Type), nameof(VortexModifier));
        writer.WriteAttributeVector2(nameof(VortexModifier.Position), modifier.Position);
        writer.WriteAttributeFloat(nameof(VortexModifier.Strength), modifier.Strength);
        writer.WriteAttributeFloat(nameof(VortexModifier.OuterRadius), modifier.OuterRadius);
        writer.WriteAttributeFloat(nameof(VortexModifier.InnerRadius), modifier.InnerRadius);
        writer.WriteAttributeFloat(nameof(VortexModifier.MaxVelocity), modifier.MaxVelocity);
        writer.WriteAttributeFloat(nameof(VortexModifier.RotationAngle), modifier.RotationAngle);
    }

    private static void WriteInterpolator(XmlWriter writer, Interpolator interpolator)
    {
        writer.WriteAttributeString(nameof(Interpolator.Name), interpolator.Name);
        writer.WriteAttributeBool(nameof(Interpolator.Enabled), interpolator.Enabled);

        switch (interpolator)
        {
            case ColorInterpolator colorInterpolator:
                WriteColorInterpolator(writer, colorInterpolator);
                break;

            case HueInterpolator hueInterpolator:
                WriteHueInterpolator(writer, hueInterpolator);
                break;

            case OpacityInterpolator opacityInterpolator:
                WriteOpacityInterpolator(writer, opacityInterpolator);
                break;

            case RotationInterpolator rotationInterpolator:
                WriteRotationInterpolator(writer, rotationInterpolator);
                break;

            case ScaleInterpolator scaleInterpolator:
                WriteScaleInterpolator(writer, scaleInterpolator);
                break;

            case VelocityInterpolator velocityInterpolator:
                WriteVelocityInterpolator(writer, velocityInterpolator);
                break;

            default:
                writer.WriteAttributeString(nameof(Type), interpolator.GetType().Name);
                break;
        }
    }

    private static void WriteColorInterpolator(XmlWriter writer, ColorInterpolator interpolator)
    {
        Vector3 startValue = new Vector3(interpolator.StartValue.H, interpolator.StartValue.S, interpolator.StartValue.L);
        Vector3 endValue = new Vector3(interpolator.EndValue.H, interpolator.EndValue.S, interpolator.EndValue.L);

        writer.WriteAttributeString(nameof(Type), nameof(ColorInterpolator));
        writer.WriteAttributeVector3(nameof(ColorInterpolator.StartValue), startValue);
        writer.WriteAttributeVector3(nameof(ColorInterpolator.EndValue), endValue);
    }

    private static void WriteHueInterpolator(XmlWriter writer, HueInterpolator interpolator)
    {
        writer.WriteAttributeString(nameof(Type), nameof(HueInterpolator));
        writer.WriteAttributeFloat(nameof(HueInterpolator.StartValue), interpolator.StartValue);
        writer.WriteAttributeFloat(nameof(HueInterpolator.EndValue), interpolator.EndValue);
    }

    private static void WriteOpacityInterpolator(XmlWriter writer, OpacityInterpolator interpolator)
    {
        writer.WriteAttributeString(nameof(Type), nameof(OpacityInterpolator));
        writer.WriteAttributeFloat(nameof(OpacityInterpolator.StartValue), interpolator.StartValue);
        writer.WriteAttributeFloat(nameof(OpacityInterpolator.EndValue), interpolator.EndValue);
    }

    private static void WriteRotationInterpolator(XmlWriter writer, RotationInterpolator interpolator)
    {
        writer.WriteAttributeString(nameof(Type), nameof(RotationInterpolator));
        writer.WriteAttributeFloat(nameof(RotationInterpolator.StartValue), interpolator.StartValue);
        writer.WriteAttributeFloat(nameof(RotationInterpolator.EndValue), interpolator.EndValue);
    }

    private static void WriteScaleInterpolator(XmlWriter writer, ScaleInterpolator interpolator)
    {
        writer.WriteAttributeString(nameof(Type), nameof(ScaleInterpolator));
        writer.WriteAttributeVector2(nameof(ScaleInterpolator.StartValue), interpolator.StartValue);
        writer.WriteAttributeVector2(nameof(ScaleInterpolator.EndValue), interpolator.EndValue);
    }

    private static void WriteVelocityInterpolator(XmlWriter writer, VelocityInterpolator interpolator)
    {
        writer.WriteAttributeString(nameof(Type), nameof(VelocityInterpolator));
        writer.WriteAttributeVector2(nameof(VelocityInterpolator.StartValue), interpolator.StartValue);
        writer.WriteAttributeVector2(nameof(VelocityInterpolator.EndValue), interpolator.EndValue);
    }

    #endregion Serialize
}
