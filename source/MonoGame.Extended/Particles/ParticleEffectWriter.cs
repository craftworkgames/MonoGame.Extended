using System;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Serialization.Xml;

namespace MonoGame.Extended.Particles;

/// <summary>
/// Represents a writer that serializes a <see cref="ParticleEffect"/> to an XML configuration.
/// </summary>
[Obsolete("Use ParticleEffectSerializer.Serialize.  ParticleEffectWriter will be removed in 6.0.0")]
public class ParticleEffectWriter : IDisposable
{
    private readonly XmlWriter _writer;

    /// <summary>
    /// Gets a value that indicates if this <see cref="ParticleEffectWriter"/> has been disposed of.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffectWriter"/> class that writers to the writes to a file.
    /// </summary>
    /// <param name="fileName">The file path to write the XML to.</param>
    public ParticleEffectWriter(string fileName)
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CloseOutput = true;
        settings.NewLineChars = "\n";

        _writer = XmlWriter.Create(fileName, settings);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticleEffectWriter"/> class that writes to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public ParticleEffectWriter(Stream stream)
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CloseOutput = false;
        settings.NewLineChars = "\n";

        _writer = XmlWriter.Create(stream, settings);
    }

    /// <summary/>
    ~ParticleEffectWriter()
    {
        Dispose();
    }

    /// <summary>
    /// Write a <see cref="ParticleEffect"/> to the XML output.
    /// </summary>
    /// <param name="effect">The <see cref="ParticleEffect"/> to serialize.</param>
    /// <exception cref="ArgumentNullException"><paramref name="effect"/> is <see langword="null"/>.</exception>
    public void WriteParticleEffect(ParticleEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        _writer.WriteStartDocument();
        _writer.WriteStartElement("ParticleEffect");

        _writer.WriteAttributeString(nameof(ParticleEffect.Name), effect.Name);
        _writer.WriteAttributeVector2(nameof(ParticleEffect.Position), effect.Position);
        _writer.WriteAttributeFloat(nameof(ParticleEffect.Rotation), effect.Rotation);
        _writer.WriteAttributeVector2(nameof(ParticleEffect.Scale), effect.Scale);
        _writer.WriteAttributeBool(nameof(ParticleEffect.AutoTrigger), effect.AutoTrigger);
        _writer.WriteAttributeFloat(nameof(ParticleEffect.AutoTriggerFrequency), effect.AutoTriggerFrequency);

        if (effect.Emitters.Count > 0)
        {
            _writer.WriteStartElement(nameof(ParticleEffect.Emitters));
            foreach (ParticleEmitter emitter in effect.Emitters)
            {
                _writer.WriteStartElement(nameof(ParticleEmitter));
                WriteParticleEmitter(emitter);
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
        }

        _writer.WriteEndElement();
        _writer.WriteEndDocument();

        _writer.Flush();
    }

    private void WriteParticleEmitter(ParticleEmitter emitter)
    {
        _writer.WriteAttributeString(nameof(ParticleEmitter.Name), emitter.Name);
        _writer.WriteAttributeFloat(nameof(ParticleEmitter.LifeSpan), emitter.LifeSpan);
        _writer.WriteAttributeVector2(nameof(ParticleEmitter.Offset), emitter.Offset);
        _writer.WriteAttributeFloat(nameof(ParticleEmitter.LayerDepth), emitter.LayerDepth);
        _writer.WriteAttributeFloat(nameof(ParticleEmitter.ReclaimFrequency), emitter.ReclaimFrequency);
        _writer.WriteAttributeInt(nameof(ParticleEmitter.Capacity), emitter.Capacity);
        _writer.WriteAttributeString(nameof(ParticleEmitter.ModifierExecutionStrategy), emitter.ModifierExecutionStrategy.ToString());
        _writer.WriteAttributeString(nameof(ParticleEmitter.RenderingOrder), emitter.RenderingOrder.ToString());

        if (emitter.TextureRegion is Texture2DRegion region)
        {
            _writer.WriteStartElement(nameof(ParticleEmitter.TextureRegion));
            WriteTexture2DRegion(region);
            _writer.WriteEndElement();
        }

        _writer.WriteStartElement(nameof(ParticleEmitter.Parameters));
        WriteParticleReleaseParameters(emitter.Parameters);
        _writer.WriteEndElement();

        _writer.WriteStartElement(nameof(ParticleEmitter.Profile));
        WriteProfile(emitter.Profile);
        _writer.WriteEndElement();


        if (emitter.Modifiers.Count > 0)
        {
            _writer.WriteStartElement(nameof(ParticleEmitter.Modifiers));
            foreach (Modifier modifier in emitter.Modifiers)
            {
                _writer.WriteStartElement(nameof(Modifier));
                WriteModifier(modifier);
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
        }
    }

    private void WriteTexture2DRegion(Texture2DRegion region)
    {
        _writer.WriteAttributeString(nameof(Texture2DRegion.Texture.Name), region.Texture.Name);
        _writer.WriteAttributeRectangle(nameof(Texture2DRegion.Bounds), region.Bounds);

    }

    private void WriteParticleReleaseParameters(ParticleReleaseParameters parameters)
    {
        WriteParticleInt32Parameter(nameof(ParticleReleaseParameters.Quantity), parameters.Quantity);
        WriteParticleFloatParameter(nameof(ParticleReleaseParameters.Speed), parameters.Speed);
        WriteParticleColorParameter(nameof(ParticleReleaseParameters.Color), parameters.Color);
        WriteParticleFloatParameter(nameof(ParticleReleaseParameters.Opacity), parameters.Opacity);
        WriteParticleVector2Parameter(nameof(ParticleReleaseParameters.Scale), parameters.Scale);
        WriteParticleFloatParameter(nameof(ParticleReleaseParameters.Rotation), parameters.Rotation);
        WriteParticleFloatParameter(nameof(ParticleReleaseParameters.Mass), parameters.Mass);

    }

    private void WriteParticleInt32Parameter(string name, ParticleInt32Parameter parameter)
    {
        _writer.WriteStartElement(name);
        _writer.WriteAttributeString(nameof(ParticleInt32Parameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            _writer.WriteAttributeInt(nameof(ParticleInt32Parameter.Constant), parameter.Constant);
        }
        else
        {
            _writer.WriteAttributeInt(nameof(ParticleInt32Parameter.RandomMin), parameter.RandomMin);
            _writer.WriteAttributeInt(nameof(ParticleInt32Parameter.RandomMax), parameter.RandomMax);
        }

        _writer.WriteEndElement();
    }

    private void WriteParticleFloatParameter(string name, ParticleFloatParameter parameter)
    {
        _writer.WriteStartElement(name);
        _writer.WriteAttributeString(nameof(ParticleFloatParameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            _writer.WriteAttributeFloat(nameof(ParticleFloatParameter.Constant), parameter.Constant);
        }
        else
        {
            _writer.WriteAttributeFloat(nameof(ParticleFloatParameter.RandomMin), parameter.RandomMin);
            _writer.WriteAttributeFloat(nameof(ParticleFloatParameter.RandomMax), parameter.RandomMax);
        }

        _writer.WriteEndElement();
    }

    private void WriteParticleVector2Parameter(string name, ParticleVector2Parameter parameter)
    {
        _writer.WriteStartElement(name);
        _writer.WriteAttributeString(nameof(ParticleVector2Parameter.Kind), parameter.Kind.ToString());

        if(parameter.Kind == ParticleValueKind.Constant)
        {
            _writer.WriteAttributeVector2(nameof(ParticleVector2Parameter.Constant), parameter.Constant);
        }
        else
        {
            _writer.WriteAttributeVector2(nameof(ParticleVector2Parameter.RandomMin), parameter.RandomMin);
            _writer.WriteAttributeVector2(nameof(ParticleVector2Parameter.RandomMax), parameter.RandomMax);
        }

        _writer.WriteEndElement();
    }

    private void WriteParticleColorParameter(string name, ParticleColorParameter parameter)
    {
        _writer.WriteStartElement(name);
        _writer.WriteAttributeString(nameof(ParticleColorParameter.Kind), parameter.Kind.ToString());

        if (parameter.Kind == ParticleValueKind.Constant)
        {
            _writer.WriteAttributeVector3(nameof(ParticleColorParameter.Constant), parameter.Constant);
        }
        else
        {
            _writer.WriteAttributeVector3(nameof(ParticleColorParameter.RandomMin), parameter.RandomMin);
            _writer.WriteAttributeVector3(nameof(ParticleColorParameter.RandomMax), parameter.RandomMax);
        }

        _writer.WriteEndElement();
    }

    private void WriteProfile(Profile profile)
    {
        switch (profile)
        {
            case BoxFillProfile boxFillProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(BoxFillProfile));
                _writer.WriteAttributeFloat(nameof(BoxFillProfile.Width), boxFillProfile.Width);
                _writer.WriteAttributeFloat(nameof(BoxFillProfile.Height), boxFillProfile.Height);
                break;

            case BoxProfile boxProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(BoxProfile));
                _writer.WriteAttributeFloat(nameof(BoxProfile.Width), boxProfile.Width);
                _writer.WriteAttributeFloat(nameof(BoxProfile.Height), boxProfile.Height);
                break;

            case BoxUniformProfile boxUniformProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(BoxUniformProfile));
                _writer.WriteAttributeFloat(nameof(BoxUniformProfile.Width), boxUniformProfile.Width);
                _writer.WriteAttributeFloat(nameof(BoxUniformProfile.Height), boxUniformProfile.Height);
                break;

            case CircleProfile circleProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(CircleProfile));
                _writer.WriteAttributeFloat(nameof(CircleProfile.Radius), circleProfile.Radius);
                _writer.WriteAttributeString(nameof(CircleProfile.Radiate), circleProfile.Radiate.ToString());
                break;

            case LineProfile lineProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(LineProfile));
                _writer.WriteAttributeVector2(nameof(LineProfile.Axis), lineProfile.Axis);
                _writer.WriteAttributeFloat(nameof(LineProfile.Length), lineProfile.Length);
                break;

            case PointProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(PointProfile));
                break;

            case RingProfile ringProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(RingProfile));
                _writer.WriteAttributeFloat(nameof(RingProfile.Radius), ringProfile.Radius);
                _writer.WriteAttributeString(nameof(RingProfile.Radiate), ringProfile.Radiate.ToString());
                break;

            case SprayProfile sprayProfile:
                _writer.WriteAttributeString(nameof(Type), nameof(SprayProfile));
                _writer.WriteAttributeVector2(nameof(SprayProfile.Direction), sprayProfile.Direction);
                _writer.WriteAttributeFloat(nameof(SprayProfile.Spread), sprayProfile.Spread);
                break;

            default:
                _writer.WriteAttributeString(nameof(Type), nameof(PointProfile));
                break;
        }
    }

    private void WriteModifier(Modifier modifier)
    {
        _writer.WriteAttributeString(nameof(Modifier.Name), modifier.Name);
        _writer.WriteAttributeFloat(nameof(Modifier.Frequency), modifier.Frequency);

        switch (modifier)
        {
            case AgeModifier ageModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(AgeModifier));
                if (ageModifier.Interpolators.Count > 0)
                {
                    _writer.WriteStartElement(nameof(AgeModifier.Interpolators));
                    foreach (Interpolator interpolator in ageModifier.Interpolators)
                    {
                        _writer.WriteStartElement(nameof(Interpolator));
                        WriteInterpolator(interpolator);
                        _writer.WriteEndElement();
                    }
                    _writer.WriteEndElement();
                }
                break;

            case CircleContainerModifier circleContainerModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(CircleContainerModifier));
                _writer.WriteAttributeFloat(nameof(CircleContainerModifier.Radius), circleContainerModifier.Radius);
                _writer.WriteAttributeBool(nameof(CircleContainerModifier.Inside), circleContainerModifier.Inside);
                _writer.WriteAttributeFloat(nameof(CircleContainerModifier.RestitutionCoefficient), circleContainerModifier.RestitutionCoefficient);
                break;

            case DragModifier dragModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(DragModifier));
                _writer.WriteAttributeFloat(nameof(DragModifier.DragCoefficient), dragModifier.DragCoefficient);
                _writer.WriteAttributeFloat(nameof(DragModifier.Density), dragModifier.Density);
                break;

            case LinearGravityModifier linearGravityModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(LinearGravityModifier));
                _writer.WriteAttributeVector2(nameof(LinearGravityModifier.Direction), linearGravityModifier.Direction);
                _writer.WriteAttributeFloat(nameof(LinearGravityModifier.Strength), linearGravityModifier.Strength);
                break;

            case OpacityFastFadeModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(OpacityFastFadeModifier));
                break;

            case RectangleContainerModifier rectangleContainerModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(RectangleContainerModifier));
                _writer.WriteAttributeInt(nameof(RectangleContainerModifier.Width), rectangleContainerModifier.Width);
                _writer.WriteAttributeInt(nameof(RectangleContainerModifier.Height), rectangleContainerModifier.Height);
                _writer.WriteAttributeFloat(nameof(RectangleContainerModifier.RestitutionCoefficient), rectangleContainerModifier.RestitutionCoefficient);
                break;

            case RectangleLoopContainerModifier rectangleLoopContainerModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(RectangleLoopContainerModifier));
                _writer.WriteAttributeInt(nameof(RectangleLoopContainerModifier.Width), rectangleLoopContainerModifier.Width);
                _writer.WriteAttributeInt(nameof(RectangleLoopContainerModifier.Height), rectangleLoopContainerModifier.Height);
                break;

            case RotationModifier rotationModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(RotationModifier));
                _writer.WriteAttributeFloat(nameof(RotationModifier.RotationRate), rotationModifier.RotationRate);
                break;

            case VelocityColorModifier velocityColorModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(VelocityColorModifier));
                Vector3 stationaryColor = new(velocityColorModifier.StationaryColor.H, velocityColorModifier.StationaryColor.S, velocityColorModifier.StationaryColor.L);
                _writer.WriteAttributeVector3(nameof(VelocityColorModifier.StationaryColor), stationaryColor);
                Vector3 velocityColor = new(velocityColorModifier.VelocityColor.H, velocityColorModifier.VelocityColor.S, velocityColorModifier.VelocityColor.L);
                _writer.WriteAttributeVector3(nameof(VelocityColorModifier.VelocityColor), velocityColor);
                _writer.WriteAttributeFloat(nameof(VelocityColorModifier.VelocityThreshold), velocityColorModifier.VelocityThreshold);
                break;

            case VelocityModifier velocityModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(VelocityModifier));
                _writer.WriteAttributeFloat(nameof(VelocityModifier.VelocityThreshold), velocityModifier.VelocityThreshold);
                if (velocityModifier.Interpolators.Count > 0)
                {
                    _writer.WriteStartElement(nameof(VelocityModifier.Interpolators));
                    foreach (Interpolator interpolator in velocityModifier.Interpolators)
                    {
                        _writer.WriteStartElement(nameof(Interpolator));
                        WriteInterpolator(interpolator);
                        _writer.WriteEndElement();
                    }
                    _writer.WriteEndElement();
                }
                break;

            case VortexModifier vortexModifier:
                _writer.WriteAttributeString(nameof(Type), nameof(VortexModifier));
                _writer.WriteAttributeVector2(nameof(VortexModifier.Position), vortexModifier.Position);
                _writer.WriteAttributeFloat(nameof(VortexModifier.Strength), vortexModifier.Strength);
                _writer.WriteAttributeFloat(nameof(VortexModifier.OuterRadius), vortexModifier.OuterRadius);
                _writer.WriteAttributeFloat(nameof(VortexModifier.InnerRadius), vortexModifier.InnerRadius);
                _writer.WriteAttributeFloat(nameof(VortexModifier.MaxVelocity), vortexModifier.MaxVelocity);
                _writer.WriteAttributeFloat(nameof(VortexModifier.RotationAngle), vortexModifier.RotationAngle);
                break;

            default:
                _writer.WriteAttributeString(nameof(Type), "Unknown");
                break;
        }
    }

    private void WriteInterpolator(Interpolator interpolator)
    {
        _writer.WriteAttributeString(nameof(interpolator.Name), interpolator.Name);

        switch (interpolator)
        {
            case ColorInterpolator colorInterpolator:
                _writer.WriteAttributeString(nameof(Type), nameof(ColorInterpolator));
                Vector3 startValue = new Vector3(colorInterpolator.StartValue.H, colorInterpolator.StartValue.S, colorInterpolator.StartValue.L);
                Vector3 endValue = new Vector3(colorInterpolator.EndValue.H, colorInterpolator.EndValue.S, colorInterpolator.EndValue.L);
                _writer.WriteAttributeVector3(nameof(ColorInterpolator.StartValue), startValue);
                _writer.WriteAttributeVector3(nameof(ColorInterpolator.EndValue), endValue);
                break;

            case HueInterpolator hueInterpolator:
                _writer.WriteAttributeString(nameof(Type), nameof(HueInterpolator));
                _writer.WriteAttributeFloat(nameof(HueInterpolator.StartValue), hueInterpolator.StartValue);
                _writer.WriteAttributeFloat(nameof(HueInterpolator.EndValue), hueInterpolator.EndValue);
                break;

            case OpacityInterpolator opacityInterpolator:
                _writer.WriteAttributeString(nameof(Type), nameof(OpacityInterpolator));
                _writer.WriteAttributeFloat(nameof(OpacityInterpolator.StartValue), opacityInterpolator.StartValue);
                _writer.WriteAttributeFloat(nameof(OpacityInterpolator.EndValue), opacityInterpolator.EndValue);
                break;

            case RotationInterpolator rotationInterpolator:
                _writer.WriteAttributeString(nameof(Type), nameof(RotationInterpolator));
                _writer.WriteAttributeFloat(nameof(RotationInterpolator.StartValue), rotationInterpolator.StartValue);
                _writer.WriteAttributeFloat(nameof(RotationInterpolator.EndValue), rotationInterpolator.EndValue);
                break;

            case ScaleInterpolator scaleInterpolator:
                _writer.WriteAttributeString(nameof(Type), nameof(ScaleInterpolator));
                _writer.WriteAttributeVector2(nameof(ScaleInterpolator.StartValue), scaleInterpolator.StartValue);
                _writer.WriteAttributeVector2(nameof(ScaleInterpolator.EndValue), scaleInterpolator.EndValue);
                break;

            case VelocityInterpolator velocityInterpolator:
                _writer.WriteAttributeString(nameof(Type), nameof(VelocityInterpolator));
                _writer.WriteAttributeVector2(nameof(VelocityInterpolator.StartValue), velocityInterpolator.StartValue);
                _writer.WriteAttributeVector2(nameof(VelocityInterpolator.EndValue), velocityInterpolator.EndValue);
                break;

            default:
                _writer.WriteAttributeString(nameof(Type), "Unknown");
                break;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        _writer.Dispose();
        GC.SuppressFinalize(this);

        IsDisposed = true;
    }
}
