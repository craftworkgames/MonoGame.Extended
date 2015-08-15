using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zlib;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TmxMap, TiledMapProcessorResult>
    {
        public override TiledMapProcessorResult Process(TmxMap map, ContentProcessorContext context)
        {
            foreach (var layer in map.Layers.OfType<TmxTileLayer>())
            {
                var data = layer.Data;

                if (data.Encoding == "csv")
                {
                    data.Tiles = data.Value
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .Select(gid => new TmxDataTile(gid))
                        .ToList();
                }

                if (data.Encoding == "base64")
                {
                    var encodedData = data.Value.Trim();
                    var decodedData = Convert.FromBase64String(encodedData);

                    using (var stream = OpenStream(decodedData, data.Compression))
                    using(var reader = new BinaryReader(stream))
                    {
                        data.Tiles = new List<TmxDataTile>();

                        for (var y = 0; y < layer.Width; y++)
                        {
                            for (var x = 0; x < layer.Height; x++)
                            {
                                var gid = reader.ReadUInt32();
                                data.Tiles.Add(new TmxDataTile((int) gid));
                            }
                        }
                    }
                }
            }

            return new TiledMapProcessorResult(map);
        }

        private static Stream OpenStream(byte[] decodedData, string compressionMode)
        {
            var memoryStream = new MemoryStream(decodedData, writable: false);
            
            if (compressionMode == "gzip")
                return new GZipStream(memoryStream, CompressionMode.Decompress);

            if (compressionMode == "zlib")
                return new ZlibStream(memoryStream, CompressionMode.Decompress);

            return memoryStream;
        }
    }
}