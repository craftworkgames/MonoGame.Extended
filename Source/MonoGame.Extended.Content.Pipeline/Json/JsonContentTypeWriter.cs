using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Extended.Content.Pipeline.Json
{
    [ContentTypeWriter]
    public class JsonContentTypeWriter : ContentTypeWriter<JsonContentProcessorResult>
    {
        private string _runtimeType;

        protected override void Write(ContentWriter writer, JsonContentProcessorResult result)
        {
            _runtimeType = result.ContentType;
            writer.Write(result.Json);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return _runtimeType;// "MonoGame.Extended.Serialization.SpriteFactoryContentTypeReader, MonoGame.Extended";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return _runtimeType;// "MonoGame.Extended.Serialization.SpriteFactoryContentTypeReader, MonoGame.Extended";
        }
    }
}