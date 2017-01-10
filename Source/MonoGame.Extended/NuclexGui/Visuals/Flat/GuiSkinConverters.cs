using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    public class GuiSkinJsonConverter : JsonConverter
    {
        /// <summary>Width of the frame's bottom border regions</summary>
        private int _bottomBorderWidth;

        /// <summary>Width of the frame's left border regions</summary>
        private int _leftBorderWidth;

        /// <summary>Width of the frame's right border regions</summary>
        private int _rightBorderWidth;

        /// <summary>Width of the frame's top border regions</summary>
        private int _topBorderWidth;

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Frame.Region[]))
                return true;

            if (objectType == typeof(Frame.Text))
                return true;

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType == typeof(Frame.Region[]))
                return ParseRegions(JArray.Load(reader));

            if (objectType == typeof(Frame.Text))
                return ParseText(JToken.Load(reader));

            return (string) reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        private Frame.Region[] ParseRegions(JArray jArray)
        {
            var regions = new Frame.Region[jArray.Count];

            _leftBorderWidth = 0;
            _rightBorderWidth = 0;
            _topBorderWidth = 0;
            _bottomBorderWidth = 0;

            // Detect borders
            foreach (var token in jArray)
            {
                var hPlacement = token.Value<string>("hplacement");
                var vPlacement = token.Value<string>("vplacement");

                if (hPlacement == "left")
                    _leftBorderWidth = Math.Max(_leftBorderWidth, token.Value<int>("w"));
                else
                {
                    if (hPlacement == "right")
                        _rightBorderWidth = Math.Max(_rightBorderWidth, token.Value<int>("w"));
                }

                if (vPlacement == "top")
                    _topBorderWidth = Math.Max(_topBorderWidth, token.Value<int>("h"));
                else
                {
                    if (vPlacement == "bottom")
                        _bottomBorderWidth = Math.Max(_bottomBorderWidth, token.Value<int>("h"));
                }
            }

            // Parse each region
            for (var i = 0; i < regions.Length; i++)
                regions[i] = ParseRegion(jArray[i]);

            return regions;
        }

        private Frame.Region ParseRegion(JToken token)
        {
            var region = new Frame.Region();

            var hAlignment = token.Value<string>("hplacement");
            var vAlignment = token.Value<string>("vplacement");
            var x = token.Value<int>("x");
            var y = token.Value<int>("y");
            var w = token.Value<int>("w");
            var h = token.Value<int>("h");

            // Assign trivialities
            region.Source = token.Value<string>("source");
            region.Id = token.Value<string>("id");
            region.SourceRegion.X = x;
            region.SourceRegion.Y = y;
            region.SourceRegion.Width = w;
            region.SourceRegion.Height = h;

            // Process each region's placement and set up the unified coordinates
            CalculateRegionPlacement(GetHorizontalPlacementIndex(hAlignment), w, _leftBorderWidth, _rightBorderWidth,
                ref region.DestinationRegion.Location.X, ref region.DestinationRegion.Size.X);
            CalculateRegionPlacement(GetVerticalPlacementIndex(vAlignment), h, _topBorderWidth, _bottomBorderWidth,
                ref region.DestinationRegion.Location.Y, ref region.DestinationRegion.Size.Y);

            return region;
        }

        /// <summary>
        ///     Calculates the unified coordinates a region needs to be placed at
        /// </summary>
        /// <param name="placementIndex">
        ///     Placement index indicating where in a frame the region will be located
        /// </param>
        /// <param name="width">Width of the region in pixels</param>
        /// <param name="lowBorderWidth">
        ///     Width of the border on the lower end of the coordinate range
        /// </param>
        /// <param name="highBorderWidth">
        ///     Width of the border on the higher end of the coordinate range
        /// </param>
        /// <param name="location">
        ///     Receives the target location of the region in unified coordinates
        /// </param>
        /// <param name="size">
        ///     Receives the size of the region in unified coordinates
        /// </param>
        private void CalculateRegionPlacement(
            int placementIndex, int width,
            int lowBorderWidth, int highBorderWidth,
            ref UniScalar location, ref UniScalar size
        )
        {
            switch (placementIndex)
            {
                case -1:
                {
                    // left or top
                    var gapWidth = lowBorderWidth - width;

                    location.Fraction = 0.0f;
                    location.Offset = gapWidth;
                    size.Fraction = 0.0f;
                    size.Offset = width;
                    break;
                }
                case +1:
                {
                    // right or bottom
                    location.Fraction = 1.0f;
                    location.Offset = -highBorderWidth;
                    size.Fraction = 0.0f;
                    size.Offset = width;
                    break;
                }
                case 0:
                {
                    // stretch
                    location.Fraction = 0.0f;
                    location.Offset = lowBorderWidth;
                    size.Fraction = 1.0f;
                    size.Offset = -(highBorderWidth + lowBorderWidth);
                    break;
                }
            }
        }

        /// <summary>Converts a horizontal placement string into a placement index</summary>
        /// <param name="placement">String containing the horizontal placement</param>
        /// <returns>A placement index that is equivalent to the provided string</returns>
        private int GetHorizontalPlacementIndex(string placement)
        {
            switch (placement)
            {
                case "left":
                {
                    return -1;
                }
                case "right":
                {
                    return +1;
                }
                case "stretch":
                default:
                {
                    return 0;
                }
            }
        }

        /// <summary>Converts a vertical placement string into a placement index</summary>
        /// <param name="placement">String containing the vertical placement</param>
        /// <returns>A placement index that is equivalent to the provided string</returns>
        private int GetVerticalPlacementIndex(string placement)
        {
            switch (placement)
            {
                case "top":
                {
                    return -1;
                }
                case "bottom":
                {
                    return +1;
                }
                case "stretch":
                default:
                {
                    return 0;
                }
            }
        }

        private Frame.Text ParseText(JToken token)
        {
            var text = new Frame.Text
            {
                Offset = new Point(token.Value<int>("xoffset"), token.Value<int>("yoffset")),
                Color = ColorHelper.FromHex(token.Value<string>("color"))
            };

            Enum.TryParse(token.Value<string>("hplacement"), true, out text.HorizontalPlacement);
            Enum.TryParse(token.Value<string>("vplacement"), true, out text.VerticalPlacement);
            text.Source = token.Value<string>("font");

            return text;
        }
    }
}