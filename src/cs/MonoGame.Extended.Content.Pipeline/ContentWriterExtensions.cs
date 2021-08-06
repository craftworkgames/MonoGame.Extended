using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Extended.Content.Pipeline
{
    public static class ContentWriterExtensions
    {
        public static void Write(this ContentWriter contentWriter, Color value)
        {
            contentWriter.Write(value.R);
            contentWriter.Write(value.G);
            contentWriter.Write(value.B);
            contentWriter.Write(value.A);
        }

        public static void Write(this ContentWriter contentWriter, Matrix value)
        {
            contentWriter.Write(value.M11);
            contentWriter.Write(value.M12);
            contentWriter.Write(value.M13);
            contentWriter.Write(value.M14);
            contentWriter.Write(value.M21);
            contentWriter.Write(value.M22);
            contentWriter.Write(value.M23);
            contentWriter.Write(value.M24);
            contentWriter.Write(value.M31);
            contentWriter.Write(value.M32);
            contentWriter.Write(value.M33);
            contentWriter.Write(value.M34);
            contentWriter.Write(value.M41);
            contentWriter.Write(value.M42);
            contentWriter.Write(value.M43);
            contentWriter.Write(value.M44);
        }

        public static void Write(this ContentWriter contentWriter, Quaternion value)
        {
            contentWriter.Write(value.X);
            contentWriter.Write(value.Y);
            contentWriter.Write(value.Z);
            contentWriter.Write(value.W);
        }

        public static void Write(this ContentWriter contentWriter, Vector2 value)
        {
            contentWriter.Write(value.X);
            contentWriter.Write(value.Y);
        }

        public static void Write(this ContentWriter contentWriter, Vector3 value)
        {
            contentWriter.Write(value.X);
            contentWriter.Write(value.Y);
            contentWriter.Write(value.Z);
        }

        public static void Write(this ContentWriter contentWriter, Vector4 value)
        {
            contentWriter.Write(value.X);
            contentWriter.Write(value.Y);
            contentWriter.Write(value.Z);
            contentWriter.Write(value.W);
        }

        public static void Write(this ContentWriter contentWriter, BoundingSphere value)
        {
            contentWriter.Write(value.Center);
            contentWriter.Write(value.Radius);
        }

        public static void Write(this ContentWriter contentWriter, Rectangle value)
        {
            contentWriter.Write(value.X);
            contentWriter.Write(value.Y);
            contentWriter.Write(value.Width);
            contentWriter.Write(value.Height);
        }
    }
}