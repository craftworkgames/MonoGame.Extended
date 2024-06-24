using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Graphics;

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

                writer.Write(region.Frame.X);
                writer.Write(region.Frame.Y);
                writer.Write(region.Frame.Width);
                writer.Write(region.Frame.Height);
                writer.Write(regionName);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Texture2DAtlas).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ContentReaders.Texture2DAtlasReader).AssemblyQualifiedName;
        }
    }
}
