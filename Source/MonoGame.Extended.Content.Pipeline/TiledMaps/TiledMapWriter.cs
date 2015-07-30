using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.TiledMaps;

namespace MonoGame.Extended.Content.Pipeline.TiledMaps
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapProcessorResult>
    {
        protected override void Write(ContentWriter output, TiledMapProcessorResult value)
        {
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TiledMap).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TiledMaps.TiledMapReader, MonoGame.Extended";
        }
    }
}