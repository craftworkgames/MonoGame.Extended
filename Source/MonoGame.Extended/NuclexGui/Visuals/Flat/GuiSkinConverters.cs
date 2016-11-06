using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    public class GuiFrameJsonConverter : JsonConverter
    {
        /// <summary>Width of the frame's left border regions</summary>
        private int leftBorderWidth;
        /// <summary>Width of the frame's top border regions</summary>
        private int topBorderWidth;
        /// <summary>Width of the frame's right border regions</summary>
        private int rightBorderWidth;
        /// <summary>Width of the frame's bottom border regions</summary>
        private int bottomBorderWidth;

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Frame.Region))
                return true;

            if (objectType.Name == "name")
                return true;

            if (objectType.Name == "region")
                return true;

            if (objectType.Name == "text")
                return true;

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Frame.Region))
                return ParseRegion(JToken.Load(reader));

            if (objectType.Name == "region")
                return ParseRegions(JArray.Load(reader));

            if (objectType.Name == "text")
                return ParseTexts(JArray.Load(reader));

            return (string)reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }

        private Frame.Region[] ParseRegions(JArray jArray)
        {
            Frame.Region[] regions = new Frame.Region[jArray.Count];

            // Detect borders
            foreach (var token in jArray)
            {
                string hPlacement = token.Value<string>("hplacement");
                string vPlacement = token.Value<string>("vplacement");

                if (hPlacement == "left")
                    leftBorderWidth = Math.Max(leftBorderWidth, token.Value<int>("w"));
                else if (hPlacement == "right")
                    rightBorderWidth = Math.Max(rightBorderWidth, token.Value<int>("w"));

                if (vPlacement == "top")
                    topBorderWidth = Math.Max(topBorderWidth, token.Value<int>("h"));
                else if (vPlacement == "bottom")
                    bottomBorderWidth = Math.Max(bottomBorderWidth, token.Value<int>("h"));
            }

            // Parse each region
            for (int i = 0; i < regions.Length; i++)
                regions[i] = ParseRegion(jArray[i]);

            return regions;
        }

        private Frame.Region ParseRegion(JToken token)
        {
            Frame.Region region = new Frame.Region();
            
            string hAlignment = token.Value<string>("hplacement");
            string vAlignment = token.Value<string>("vplacement");
            int x = token.Value<int>("x");
            int y = token.Value<int>("y");
            int w = token.Value<int>("w");
            int h = token.Value<int>("h");

            // Assign trivialities
            region.Source = token.Value<string>("source");
            region.Id = token.Value<string>("id");
            region.SourceRegion.X = x;
            region.SourceRegion.Y = y;
            region.SourceRegion.Width = w;
            region.SourceRegion.Height = h;

            // Process each region's placement and set up the unified coordinates
            calculateRegionPlacement(getHorizontalPlacementIndex(hAlignment), w, leftBorderWidth, rightBorderWidth, ref region.DestinationRegion.Location.X, ref region.DestinationRegion.Size.X);
            calculateRegionPlacement(getVerticalPlacementIndex(vAlignment), h, topBorderWidth, bottomBorderWidth, ref region.DestinationRegion.Location.Y, ref region.DestinationRegion.Size.Y);

            return region;
        }

        /// <summary>
        ///   Calculates the unified coordinates a region needs to be placed at
        /// </summary>
        /// <param name="placementIndex">
        ///   Placement index indicating where in a frame the region will be located
        /// </param>
        /// <param name="width">Width of the region in pixels</param>
        /// <param name="lowBorderWidth">
        ///   Width of the border on the lower end of the coordinate range
        /// </param>
        /// <param name="highBorderWidth">
        ///   Width of the border on the higher end of the coordinate range
        /// </param>
        /// <param name="location">
        ///   Receives the target location of the region in unified coordinates
        /// </param>
        /// <param name="size">
        ///   Receives the size of the region in unified coordinates
        /// </param>
        private void calculateRegionPlacement(
          int placementIndex, int width,
          int lowBorderWidth, int highBorderWidth,
          ref UniScalar location, ref UniScalar size
        )
        {
            switch (placementIndex)
            {
                case (-1):
                    { // left or top
                        int gapWidth = lowBorderWidth - width;

                        location.Fraction = 0.0f;
                        location.Offset = gapWidth;
                        size.Fraction = 0.0f;
                        size.Offset = width;
                        break;
                    }
                case (+1):
                    { // right or bottom
                        location.Fraction = 1.0f;
                        location.Offset = -highBorderWidth;
                        size.Fraction = 0.0f;
                        size.Offset = width;
                        break;
                    }
                case (0):
                    { // stretch
                        location.Fraction = 0.0f;
                        location.Offset = lowBorderWidth;
                        size.Fraction = 1.0f;
                        size.Offset = (highBorderWidth + lowBorderWidth);
                        break;
                    }
            }
        }

        /// <summary>Converts a horizontal placement string into a placement index</summary>
        /// <param name="placement">String containing the horizontal placement</param>
        /// <returns>A placement index that is equivalent to the provided string</returns>
        private int getHorizontalPlacementIndex(string placement)
        {
            switch (placement)
            {
                case "left": { return -1; }
                case "right": { return +1; }
                case "stretch":
                default: { return 0; }
            }
        }

        /// <summary>Converts a vertical placement string into a placement index</summary>
        /// <param name="placement">String containing the vertical placement</param>
        /// <returns>A placement index that is equivalent to the provided string</returns>
        private int getVerticalPlacementIndex(string placement)
        {
            switch (placement)
            {
                case "top": { return -1; }
                case "bottom": { return +1; }
                case "stretch":
                default: { return 0; }
            }
        }

        private Frame.Text[] ParseTexts(JArray jArray)
        {
            Frame.Text[] texts = new Frame.Text[jArray.Count];

            // Parse each text
            for (int i = 0; i < texts.Length; i++)
                texts[i] = ParseText(jArray[i]);

            return texts;
        }

        private Frame.Text ParseText(JToken token)
        {
            Frame.Text text = new Frame.Text();

            text.Offset = new Microsoft.Xna.Framework.Point(token.Value<int>("xoffset"), token.Value<int>("yoffset"));
            text.Color = ColorExtensions.FromHex(token.Value<string>("color"));
            Enum.TryParse(token.Value<string>("hplacement"), out text.HorizontalPlacement);
            Enum.TryParse(token.Value<string>("vplacement"), out text.VerticalPlacement);
            text.Source = token.Value<string>("font");

            return text;
        }
    }

    public class GuiRegionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            


            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class GuiTextJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
