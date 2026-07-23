using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MonoGame.Extended.Tilemaps.Tiled.Document
{
    internal static class TiledXmlParser
    {
        public static TiledMapXml ParseMap(XElement element)
        {
            var map = new TiledMapXml
            {
                Version = (string)element.Attribute("version"),
                TiledVersion = (string)element.Attribute("tiledversion"),
                Orientation = (string)element.Attribute("orientation"),
                RenderOrder = (string)element.Attribute("renderorder"),
                Width = ParseInt((string)element.Attribute("width")),
                Height = ParseInt((string)element.Attribute("height")),
                TileWidth = ParseInt((string)element.Attribute("tilewidth")),
                TileHeight = ParseInt((string)element.Attribute("tileheight")),
                Infinite = ParseInt((string)element.Attribute("infinite")),
                BackgroundColor = (string)element.Attribute("backgroundcolor"),
                ParallaxOriginX = ParseFloat((string)element.Attribute("parallaxoriginx")),
                ParallaxOriginY = ParseFloat((string)element.Attribute("parallaxoriginy")),
                HexSideLength = ParseInt((string)element.Attribute("hexsidelength")),
                StaggerAxis = (string)element.Attribute("staggeraxis"),
                StaggerIndex = (string)element.Attribute("staggerindex"),
                NextLayerId = ParseInt((string)element.Attribute("nextlayerid")),
                NextObjectId = ParseInt((string)element.Attribute("nextobjectid")),
            };

            foreach (var child in element.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "properties":
                        {
                            map.Properties = ParseProperties(child);
                            break;
                        }
                    case "tileset":
                        {
                            map.Tilesets.Add(ParseTilesetReference(child));
                            break;
                        }
                    case "layer":
                        {
                            map.Layers.Add(ParseTileLayer(child));
                            break;
                        }
                    case "objectgroup":
                        {
                            map.Layers.Add(ParseObjectLayer(child));
                            break;
                        }
                    case "imagelayer":
                        {
                            map.Layers.Add(ParseImageLayer(child));
                            break;
                        }
                    case "group":
                        {
                            map.Layers.Add(ParseGroupLayer(child));
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"Ignoring element: {child.Name.LocalName}");
                            break;
                        }
                }
            }

            return map;
        }

        public static TiledTilesetXml ParseTileset(XElement element)
        {
            var tileset = new TiledTilesetXml();
            PopulateTileset(element, tileset);

            return tileset;
        }

        private static TiledTilesetRefXml ParseTilesetReference(XElement element)
        {
            var tilesetReference = new TiledTilesetRefXml
            {
                Source = (string)element.Attribute("source"),
            };
            PopulateTileset(element, tilesetReference);

            return tilesetReference;
        }

        private static void PopulateTileset(XElement element, TiledTilesetXml tileset)
        {
            tileset.FirstGlobalId = ParseInt((string)element.Attribute("firstgid"));
            tileset.Name = (string)element.Attribute("name");
            tileset.TileWidth = ParseInt((string)element.Attribute("tilewidth"));
            tileset.TileHeight = ParseInt((string)element.Attribute("tileheight"));
            tileset.TileCount = ParseInt((string)element.Attribute("tilecount"));
            tileset.Columns = ParseInt((string)element.Attribute("columns"));
            tileset.Spacing = ParseInt((string)element.Attribute("spacing"));
            tileset.Margin = ParseInt((string)element.Attribute("margin"));
            tileset.ObjectAlignment = (string)element.Attribute("objectalignment");

            foreach (var child in element.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "tileoffset":
                        {
                            tileset.TileOffset = new TiledTileOffsetXml
                            {
                                X = ParseInt((string)child.Attribute("x")),
                                Y = ParseInt((string)child.Attribute("y")),
                            };
                            break;
                        }
                    case "grid":
                        {
                            tileset.Grid = new TiledGridXml
                            {
                                Orientation = (string)child.Attribute("orientation"),
                                Width = ParseInt((string)child.Attribute("width")),
                                Height = ParseInt((string)child.Attribute("height")),
                            };
                            break;
                        }
                    case "image":
                        {
                            tileset.Image = ParseImage(child);
                            break;
                        }
                    case "tile":
                        {
                            tileset.Tiles.Add(ParseTile(child));
                            break;
                        }
                    case "properties":
                        {
                            tileset.Properties = ParseProperties(child);
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"Ignoring child of base tileset: {child.Name.LocalName}");
                            break;
                        }
                }
            }
        }

        private static TiledTileXml ParseTile(XElement element)
        {
            var tile = new TiledTileXml
            {
                Id = ParseInt((string)element.Attribute("id")),
                Type = (string)element.Attribute("type"),
                Class = (string)element.Attribute("class"),
                Probability = ParseFloat((string)element.Attribute("probability")),
            };

            foreach (var child in element.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "properties":
                        {
                            tile.Properties = ParseProperties(child);
                            break;
                        }
                    case "image":
                        {
                            tile.Image = ParseImage(child);
                            break;
                        }
                    case "objectgroup":
                        {
                            tile.ObjectGroup = new TiledObjectGroupXml
                            {
                                DrawOrder = (string)child.Attribute("draworder"),
                                Objects = child.Elements("object").Select(ParseObject).ToList(),
                            };
                            break;
                        }
                    case "animation":
                        {
                            tile.Animation = new TiledAnimationXml
                            {
                                Frames = child.Elements("frame").Select(f => new TiledAnimationFrameXml
                                {
                                    TileId = ParseInt((string)f.Attribute("tileid")),
                                    Duration = ParseInt((string)f.Attribute("duration")),
                                }).ToList()
                            };
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"Ignoring child of tile: {child.Name.LocalName}");
                            break;
                        }
                }
            }

            return tile;
        }

        private static TiledImageXml ParseImage(XElement element)
        {
            return new TiledImageXml
            {
                Source = (string)element.Attribute("source"),
                Trans = (string)element.Attribute("trans"),
                Width = ParseInt((string)element.Attribute("width")),
                Height = ParseInt((string)element.Attribute("height")),
            };
        }

        private static TiledPropertiesXml ParseProperties(XElement element)
        {
            return new TiledPropertiesXml
            {
                Properties = element.Elements("property").Select(ParseProperty).ToList(),
            };
        }

        private static TiledPropertyXml ParseProperty(XElement element)
        {
            return new TiledPropertyXml
            {
                Name = (string)element.Attribute("name"),
                Type = (string)element.Attribute("type"),
                Value = (string)element.Attribute("value") ?? element.Value,
            };
        }

        private static void ParseBaseLayer(XElement element, TiledLayerXml layer)
        {
            layer.Id = ParseInt((string)element.Attribute("id"));
            layer.Name = (string)element.Attribute("name");
            layer.Class = (string)element.Attribute("class");
            layer.OffsetX = ParseFloat((string)element.Attribute("offsetx"));
            layer.OffsetY = ParseFloat((string)element.Attribute("offsety"));
            layer.ParallaxX = element.Attribute("parallaxx") != null ? ParseFloat((string)element.Attribute("parallaxx")) : 1.0f;
            layer.ParallaxY = element.Attribute("parallaxy") != null ? ParseFloat((string)element.Attribute("parallaxy")) : 1.0f;
            layer.Opacity = element.Attribute("opacity") != null ? ParseFloat((string)element.Attribute("opacity")) : 1.0f;
            layer.Visible = element.Attribute("visible") != null ? ParseInt((string)element.Attribute("visible")) : 1;
            layer.TintColor = (string)element.Attribute("tintcolor");

            var propertiesElement = element.Element("properties");
            if (propertiesElement != null)
            {
                layer.Properties = ParseProperties(propertiesElement);
            }
        }

        private static TiledTileLayerXml ParseTileLayer(XElement element)
        {
            var layer = new TiledTileLayerXml
            {
                Width = ParseInt((string)element.Attribute("width")),
                Height = ParseInt((string)element.Attribute("height")),
            };
            ParseBaseLayer(element, layer);

            var dataElement = element.Element("data");
            if (dataElement != null)
            {
                layer.Data = new TiledTileLayerDataXml
                {
                    Encoding = (string)dataElement.Attribute("encoding"),
                    Compression = (string)dataElement.Attribute("compression"),
                    Value = dataElement.Value,
                    Tiles = dataElement.Elements("tile").Select(t => new TiledDataTileXml
                    {
                        Gid = ParseUInt((string)t.Attribute("gid")),
                    }).ToList(),
                    Chunks = dataElement.Elements("chunk").Select(c => new TiledChunkXml
                    {
                        X = ParseInt((string)c.Attribute("x")),
                        Y = ParseInt((string)c.Attribute("y")),
                        Width = ParseInt((string)c.Attribute("width")),
                        Height = ParseInt((string)c.Attribute("height")),
                        Value = c.Value,
                    }).ToList()
                };
            }

            return layer;
        }

        private static TiledObjectLayerXml ParseObjectLayer(XElement element)
        {
            var layer = new TiledObjectLayerXml
            {
                Color = (string)element.Attribute("color"),
                DrawOrder = (string)element.Attribute("draworder"),
                Objects = element.Elements("object").Select(ParseObject).ToList(),
            };
            ParseBaseLayer(element, layer);

            return layer;
        }

        private static TiledObjectXml ParseObject(XElement element)
        {
            var tiledObject = new TiledObjectXml
            {
                Id = ParseInt((string)element.Attribute("id")),
                Name = (string)element.Attribute("name"),
                Type = (string)element.Attribute("type"),
                Class = (string)element.Attribute("class"),
                X = ParseFloat((string)element.Attribute("x")),
                Y = ParseFloat((string)element.Attribute("y")),
                Width = ParseFloat((string)element.Attribute("width")),
                Height = ParseFloat((string)element.Attribute("height")),
                Rotation = ParseFloat((string)element.Attribute("rotation")),
                Gid = ParseUInt((string)element.Attribute("gid")),
                Visible = element.Attribute("visible") != null
                    ? ParseInt((string)element.Attribute("visible"))
                    : 1
            };

            foreach (var child in element.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "properties":
                        {
                            tiledObject.Properties = ParseProperties(child);
                            break;
                        }
                    case "ellipse":
                        {
                            tiledObject.Ellipse = new TiledEllipseXml();
                            break;
                        }
                    case "point":
                        {
                            tiledObject.Point = new TiledPointXml();
                            break;
                        }
                    case "polygon":
                        {
                            tiledObject.Polygon = new TiledPolygonXml { Points = (string)child.Attribute("points") };
                            break;
                        }
                    case "polyline":
                        {
                            tiledObject.Polyline = new TiledPolylineXml { Points = (string)child.Attribute("points") };
                            break;
                        }
                    case "text":
                        {
                            tiledObject.Text = new TiledTextXml
                            {
                                FontFamily = (string)child.Attribute("fontfamily"),
                                PixelSize =
                                    child.Attribute("pixelsize") != null
                                        ? ParseInt((string)child.Attribute("pixelsize"))
                                        : 16,
                                Wrap =
                                    child.Attribute("wrap") != null ? ParseInt((string)child.Attribute("wrap")) : 0,
                                Color = (string)child.Attribute("color") ?? "#000000",
                                Bold =
                                    child.Attribute("bold") != null ? ParseInt((string)child.Attribute("bold")) : 0,
                                Italic =
                                    child.Attribute("italic") != null
                                        ? ParseInt((string)child.Attribute("italic"))
                                        : 0,
                                Underline =
                                    child.Attribute("underline") != null
                                        ? ParseInt((string)child.Attribute("underline"))
                                        : 0,
                                Strikeout =
                                    child.Attribute("strikeout") != null
                                        ? ParseInt((string)child.Attribute("strikeout"))
                                        : 0,
                                Kerning =
                                    child.Attribute("kerning") != null
                                        ? ParseInt((string)child.Attribute("kerning"))
                                        : 1,
                                HAlign = (string)child.Attribute("halign") ?? "left",
                                VAlign = (string)child.Attribute("valign") ?? "top",
                                Value = child.Value
                            };
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"Ignoring child of object: {child.Name.LocalName}");
                            break;
                        }
                }
            }

            return tiledObject;
        }

        private static TiledImageLayerXml ParseImageLayer(XElement element)
        {
            var layer = new TiledImageLayerXml
            {
                RepeatX = element.Attribute("repeatx") != null
                    ? ParseInt((string)element.Attribute("repeatx"))
                    : 0,
                RepeatY = element.Attribute("repeaty") != null
                    ? ParseInt((string)element.Attribute("repeaty"))
                    : 0,
            };
            ParseBaseLayer(element, layer);

            var imageElement = element.Element("image");
            if (imageElement != null)
            {
                layer.Image = ParseImage(imageElement);
            }

            return layer;
        }

        private static TiledGroupLayerXml ParseGroupLayer(XElement element)
        {
            var group = new TiledGroupLayerXml();
            ParseBaseLayer(element, group);

            foreach (var child in element.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "layer":
                        {
                            group.Layers.Add(ParseTileLayer(child));
                            break;
                        }
                    case "objectgroup":
                        {
                            group.Layers.Add(ParseObjectLayer(child));
                            break;
                        }
                    case "imagelayer":
                        {
                            group.Layers.Add(ParseImageLayer(child));
                            break;
                        }
                    case "group":
                        {
                            group.Layers.Add(ParseGroupLayer(child));
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"Ignoring child of group: {child.Name.LocalName}");
                            break;
                        }
                }
            }

            return group;
        }

        #region Helpers

        private static float ParseFloat(string value)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                Debug.WriteLine($"Failed to parse float: {value}");
            }

            return result;
        }

        private static int ParseInt(string value)
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                Debug.WriteLine($"Failed to parse int: {value}");
            }

            return result;
        }

        private static uint ParseUInt(string value)
        {
            if (!uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint result))
            {
                Debug.WriteLine($"Failed to parse uint: {value}");
            }

            return result;
        }

        #endregion
    }
}
