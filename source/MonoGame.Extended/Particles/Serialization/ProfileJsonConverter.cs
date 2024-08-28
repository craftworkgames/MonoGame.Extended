using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Serialization.Json;

namespace MonoGame.Extended.Particles.Serialization
{
    public class ProfileJsonConverter : JsonConverter<Profile>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Profile);

        /// <inheritdoc />
        public override Profile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected {nameof(JsonTokenType.StartObject)} token");
            }

            Profile profile = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    if (propertyName.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var type = reader.GetString();
                        profile = type switch
                        {
                            nameof(Profile.Point) => Profile.Point(),
                            nameof(Profile.Line) => ReadLineProfile(ref reader),
                            nameof(Profile.Ring) => ReadRingProfile(ref reader),
                            nameof(Profile.Box) => ReadBoxProfile(ref reader),
                            nameof(Profile.BoxFill) => ReadBoxFillProfile(ref reader),
                            nameof(Profile.BoxUniform) => ReadBoxUniformProfile(ref reader),
                            nameof(Profile.Circle) => ReadCircleProfile(ref reader),
                            nameof(Profile.Spray) => ReadSprayProfile(ref reader),

                            _ => throw new NotSupportedException($"The profile type {type} is not supported at this time")
                        };
                    }
                }
            }

            return profile;
        }

        private static Profile ReadLineProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("axis", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            string[] split = reader.GetString().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(split.Length == 2);
            Vector2 axis = new Vector2(float.Parse(split[0]), float.Parse(split[1]));

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("length", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float length = reader.GetSingle();

            return Profile.Line(axis, length);
        }

        private static Profile ReadRingProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("radius", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float radius = reader.GetSingle();

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("radiate", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            int radiate = reader.GetInt32();

            return Profile.Ring(radius, (Profile.CircleRadiation)radiate);
        }

        private static Profile ReadBoxProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("width", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float width = reader.GetSingle();

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("height", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float height = reader.GetSingle();

            return Profile.Box(width, height);
        }

        private static Profile ReadBoxFillProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("width", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float width = reader.GetSingle();

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("height", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float height = reader.GetSingle();

            return Profile.BoxFill(width, height);
        }

        private static Profile ReadBoxUniformProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("width", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float width = reader.GetSingle();

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("height", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float height = reader.GetSingle();

            return Profile.BoxUniform(width, height);
        }

        private static Profile ReadCircleProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("radius", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float radius = reader.GetSingle();

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("radiate", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            int radiate = reader.GetInt32();

            return Profile.Circle(radius, (Profile.CircleRadiation)radiate);

        }

        private static Profile ReadSprayProfile(ref Utf8JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("direction", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            string[] split = reader.GetString().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(split.Length == 2);
            Vector2 direction = new Vector2(float.Parse(split[0]), float.Parse(split[1]));

            reader.Read();
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString().Equals("spread", StringComparison.InvariantCultureIgnoreCase));
            reader.Read();
            float spread = reader.GetSingle();

            return Profile.Spray(direction, spread);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Profile value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            var type = value.GetType().ToString();
            switch (type)
            {
                case nameof(PointProfile):
                    WritePointProfile(ref writer, (PointProfile)value);
                    break;

                case nameof(LineProfile):
                    WriteLineProfile(ref writer, (LineProfile)value);
                    break;

                case nameof(RingProfile):
                    WriteRingProfile(ref writer, (RingProfile)value);
                    break;

                case nameof(BoxProfile):
                    WriteBoxProfile(ref writer, (BoxProfile)value);
                    break;

                case nameof(BoxFillProfile):
                    WriteBoxFillProfile(ref writer, (BoxFillProfile)value);
                    break;

                case nameof(BoxUniformProfile):
                    WriteBoxUniformProfile(ref writer, (BoxUniformProfile)value);
                    break;

                case nameof(CircleProfile):
                    WriteCircleProfile(ref writer, (CircleProfile)value);
                    break;

                case nameof(SprayProfile):
                    WriteSprayProfile(ref writer, (SprayProfile)value);
                    break;

                default:
                    throw new InvalidOperationException("Unknown profile type");
            }
        }

        private static void WritePointProfile(ref Utf8JsonWriter writer, PointProfile value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteStringValue(value.GetType().ToString());
            writer.WriteEndObject();
        }

        private static void WriteLineProfile(ref Utf8JsonWriter writer, LineProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteString("axis", $"{value.Axis.X} {value.Axis.Y}");
            writer.WriteNumber("length", value.Length);
            writer.WriteEndObject();
        }

        private static void WriteRingProfile(ref Utf8JsonWriter writer, RingProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteNumber("radius", value.Radius);
            writer.WriteNumber("radiate", (int)value.Radiate);
            writer.WriteEndObject();
        }

        private static void WriteBoxProfile(ref Utf8JsonWriter writer, BoxProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteNumber("width", value.Width);
            writer.WriteNumber("height", value.Height);
            writer.WriteEndObject();
        }

        private static void WriteBoxFillProfile(ref Utf8JsonWriter writer, BoxFillProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteNumber("width", value.Width);
            writer.WriteNumber("height", value.Height);
            writer.WriteEndObject();
        }

        private static void WriteBoxUniformProfile(ref Utf8JsonWriter writer, BoxUniformProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteNumber("width", value.Width);
            writer.WriteNumber("height", value.Height);
            writer.WriteEndObject();
        }

        private static void WriteCircleProfile(ref Utf8JsonWriter writer, CircleProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteNumber("radius", value.Radius);
            writer.WriteNumber("radiate", (int)value.Radiate);
            writer.WriteEndObject();

        }

        private static void WriteSprayProfile(ref Utf8JsonWriter writer, SprayProfile value)
        {
            writer.WriteStartObject();
            writer.WriteString("type", value.GetType().ToString());
            writer.WriteString("direction", $"{value.Direction.X} {value.Direction.Y}");
            writer.WriteNumber("spread", value.Spread);
            writer.WriteEndObject();
        }
    }
}
