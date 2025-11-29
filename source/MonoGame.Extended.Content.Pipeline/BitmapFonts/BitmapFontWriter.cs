using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content.ContentReaders;

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
            writer.Write(fontFile.FontName);
            writer.Write(fontFile.Info.FontSize);
            writer.Write(fontFile.Common.LineHeight);
            writer.Write(fontFile.Info.SpacingHoriz);
            writer.Write(fontFile.Info.SpacingVert);
            writer.Write(fontFile.Characters.Count);

            foreach (var c in fontFile.Characters)
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
            return typeof(BitmapFont).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BitmapFontContentReader).AssemblyQualifiedName;
        }
    }
}
