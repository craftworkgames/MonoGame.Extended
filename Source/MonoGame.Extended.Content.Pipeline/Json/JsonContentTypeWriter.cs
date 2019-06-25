using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Content.Pipeline.Json
{
    [ContentTypeWriter]
    public class JsonContentTypeWriter : ContentTypeWriter<JsonContentProcessorResult>
    {
        protected override void Write(ContentWriter writer, JsonContentProcessorResult result)
        {
            writer.Write(result.Json);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.Serialization.JsonContentTypeReader, MonoGame.Extended";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            var type = typeof(Sprite);
            var readerType = type.Namespace + ".JsonContentTypeReader, " + type.AssemblyQualifiedName;
            return "GetRuntimeType";

            //return typeof(object).AssemblyQualifiedName;// "MonoGame.Extended.Serialization.JsonContentTypeReader, MonoGame.Extended";
        }
    }
}