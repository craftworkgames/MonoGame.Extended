using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.BitmapFonts;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MonoGame.Extended.Content.Pipeline
{
    public class BsonData
    {
        public BsonData(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; private set; }
    }

    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<FontFile, BsonData>
    {
        public override BsonData Process(FontFile input, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing BMFont");

                using (var memoryStream = new MemoryStream())
                {
                    var jsonSerialzier = new JsonSerializer();
                    var bsonWriter = new BsonWriter(memoryStream);
                    jsonSerialzier.Serialize(bsonWriter, input);
                    return new BsonData(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}