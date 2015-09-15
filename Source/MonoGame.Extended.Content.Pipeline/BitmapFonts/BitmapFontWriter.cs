using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentTypeWriter]
    public class BitmapFontWriter : ContentTypeWriter<BitmapFontProcessorResult>
    {
        protected override void Write(ContentWriter writer, BitmapFontProcessorResult result)
        {
            writer.Write(result.TextureAssets.Count);

            foreach (var textureAsset in result.TextureAssets)
                writer.Write(textureAsset);

            var fontFile = result.FontFile;
            writer.Write(fontFile.Common.LineHeight);
            writer.Write(fontFile.Chars.Count);

            foreach (var c in fontFile.Chars)
            {
                writer.Write(c.ID);
                writer.Write(c.Page);
                writer.Write(c.X);
                writer.Write(c.Y);
                writer.Write(c.Width);
                writer.Write(c.Height);
                writer.Write(c.XOffset);
                writer.Write(c.YOffset);
                writer.Write(c.XAdvance);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(BitmapFont).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BitmapFontReader).AssemblyQualifiedName;
        }
    }
}