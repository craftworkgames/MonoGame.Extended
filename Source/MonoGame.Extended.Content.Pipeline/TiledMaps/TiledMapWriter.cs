using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.TileMaps;

namespace MonoGame.Extended.Content.Pipeline.TiledMaps
{
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapProcessorResult>
    {
        protected override void Write(ContentWriter output, TiledMapProcessorResult value)
        {
            var map = value.Map;
            output.Write(map.Width);
            output.Write(map.Height);
            output.Write(map.TileWidth);
            output.Write(map.TileHeight);
            
            output.Write(map.Tilesets.Count);

            foreach (var tileset in map.Tilesets)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                output.Write(Path.GetFileNameWithoutExtension(tileset.Image.Source));
                output.Write(tileset.FirstGid);
                output.Write(tileset.TileWidth);
                output.Write(tileset.TileHeight);
                output.Write(tileset.Spacing);
                output.Write(tileset.Margin);
            }

            output.Write(map.Layers.Count);

            foreach (var layer in map.Layers)
            {
                output.Write(layer.Tiles.Count);

                foreach (var tile in layer.Tiles)
                    output.Write(tile.Gid);

                output.Write(layer.Name);
                output.Write(map.Width);   // layers should have separate width and height
                output.Write(map.Height);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TileMap).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TileMaps.TileMapReader, MonoGame.Extended";
        }
    }
}