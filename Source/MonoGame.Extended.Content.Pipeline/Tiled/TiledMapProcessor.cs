using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Utilities;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TmxMap, TiledMapProcessorResult>
    {
        public override TiledMapProcessorResult Process(TmxMap map, ContentProcessorContext context)
        {
            var tileLayers = map.Layers.OfType<TmxTileLayer>().ToArray();
            context.Logger.LogMessage($"Processing {tileLayers.Length} layers");

            foreach (var tileLayer in tileLayers)
            {
                var data = tileLayer.Data;
                context.Logger.LogMessage($"layer: '{tileLayer.Name}', encoding: '{data.Encoding}', compression: '{data.Compression}'");

                if (data.Encoding == "csv")
                    data.Tiles = DecodeCsvData(data);

                if (data.Encoding == "base64")
                    data.Tiles = DecodeBase64Data(data, tileLayer.Width, tileLayer.Height);

                context.Logger.LogMessage($"{data.Tiles.Count} tiles processed");
            }

            return new TiledMapProcessorResult(map, context.Logger);
        }

        private static List<TmxDataTile> DecodeBase64Data(TmxData data, int width, int height)
        {
            var tileList = new List<TmxDataTile>();
            var encodedData = data.Value.Trim();
            var decodedData = Convert.FromBase64String(encodedData);

            using (var stream = OpenStream(decodedData, data.Compression))
            {
                using (var reader = new BinaryReader(stream))
                {
                    data.Tiles = new List<TmxDataTile>();

                    for (var y = 0; y < width; y++)
                    {
                        for (var x = 0; x < height; x++)
                        {
                            var gid = reader.ReadUInt32();
                            tileList.Add(new TmxDataTile((int) gid));
                        }
                    }
                }
            }

            return tileList;
        }

        private static List<TmxDataTile> DecodeCsvData(TmxData data)
        {
            return data.Value
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .Select(gid => new TmxDataTile(gid))
                .ToList();
        }

        private static Stream OpenStream(byte[] decodedData, string compressionMode)
        {
            var memoryStream = new MemoryStream(decodedData, writable: false);
            
            if (compressionMode == "gzip")
                return new GZipStream(memoryStream, CompressionMode.Decompress);

            if (compressionMode == "zlib")
                return new ZlibStream(memoryStream, Utilities.CompressionMode.Decompress);

            return memoryStream;
        }
    }
}