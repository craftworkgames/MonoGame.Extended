using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentTypeWriter]
    public class BitmapFontWriter : ContentTypeWriter<FileFileData>
    {
        protected override void Write(ContentWriter output, FileFileData value)
        {
            output.Write(value.TextureAssets.Count);

            foreach (var textureAsset in value.TextureAssets)
                output.Write(textureAsset);
            
            output.Write(value.Json);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(BitmapFont).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.BitmapFonts.BitmapFontReader, MonoGame.Extended";
        }
    }
}