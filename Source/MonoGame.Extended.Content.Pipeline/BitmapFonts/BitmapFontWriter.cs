using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

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
                writer.Write(c.Id);
                writer.Write(c.Page);
                writer.Write(c.X);
                writer.Write(c.Y);
                writer.Write(c.Width);
                writer.Write(c.Height);
                writer.Write(c.XOffset);
                writer.Write(c.YOffset);
                writer.Write(c.XAdvance);
            }

            writer.Write(fontFile.Kernings.Count);
            foreach(var k in fontFile.Kernings)
            {
                writer.Write(k.First);
                writer.Write(k.Second);
                writer.Write(k.Amount);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.BitmapFonts.BitmapFont, MonoGame.Extended";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.BitmapFonts.BitmapFontReader, MonoGame.Extended";
        }
    }
}