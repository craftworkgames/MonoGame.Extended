using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    [ContentTypeWriter]
    public class TexturePackerWriter : ContentTypeWriter<TexturePackerProcessorResult>
    {
        protected override void Write(ContentWriter writer, TexturePackerProcessorResult result)
        {
            var data = result.Data;
            var metadata = data.Metadata;

            var assetName = Path.GetFileNameWithoutExtension(metadata.Image);
            Debug.Assert(assetName != null, "assetName != null");

            writer.Write(assetName);
            writer.Write(data.Regions.Count);

            foreach (var region in data.Regions)
            {
                var regionName = Path.ChangeExtension(region.Filename, null);
                Debug.Assert(regionName != null, "regionName != null");

                writer.Write(regionName);
                writer.Write(region.Frame.X);
                writer.Write(region.Frame.Y);
                writer.Write(region.Frame.Width);
                writer.Write(region.Frame.Height);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TextureAtlases.TextureAtlas, MonoGame.Extended";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TextureAtlases.TextureAtlasReader, MonoGame.Extended";
        }
    }
}