using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Extended.Content.Pipeline
{
    [ContentTypeWriter]
    public class BitmapFontWriter : ContentTypeWriter<BitmapFontOut>
    {
        protected override void Write(ContentWriter output, BitmapFontOut value)
        {
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(BitmapFontOut).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.Content.Pipeline.BitmapFontReader, MonoGame.Extended.Content.Pipeline, Version=1.0.0.0, Culture=neutral";
        }
    }
}