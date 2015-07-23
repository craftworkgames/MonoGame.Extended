using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.BitmapFonts;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline
{
    [ContentTypeWriter]
    public class BitmapFontWriter : ContentTypeWriter<BitmapFont>
    {
        protected override void Write(ContentWriter output, BitmapFont value)
        {
            output.Write("Hello world");
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(BitmapFont).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.BitmapFonts.BitmapFontReader, MonoGame.Extended";//, Version=1.0.0.0, Culture=neutral";
        }
    }
}