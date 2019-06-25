using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Json
{
    [ContentProcessor(DisplayName = "JSON Processor - MonoGame.Extended")]
    public class JsonContentProcessor : ContentProcessor<ContentImporterResult<string>, JsonContentProcessorResult>
    {
        public override JsonContentProcessorResult Process(ContentImporterResult<string> input, ContentProcessorContext context)
        {
            try
            {
                var output = new JsonContentProcessorResult { Json = input.Data };
                return output;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}