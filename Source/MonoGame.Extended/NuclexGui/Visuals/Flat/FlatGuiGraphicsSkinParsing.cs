using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Support;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    partial class FlatGuiGraphics
    {
        #region class RegionListBuilder

        /// <summary>Builds a region list from the regions in an frame XML node</summary>
        private class RegionListBuilder
        {
            /// <summary>Width of the frame's left border regions</summary>
            private int _leftBorderWidth;
            /// <summary>Width of the frame's top border regions</summary>
            private int _topBorderWidth;
            /// <summary>Width of the frame's right border regions</summary>
            private int _rightBorderWidth;
            /// <summary>Width of the frame's bottom border regions</summary>
            private int _bottomBorderWidth;

            /// <summary>Initializes a new frame region list builder</summary>
            private RegionListBuilder() { }

            /// <summary>Builds a region list from the regions specified in the provided frame XML node</summary>
            /// <param name="frameElement">XML node for the frame whose regions wille be processed</param>
            /// <param name="bitmaps">Bitmap lookup table used to associate a region's bitmap id to the real bitmap</param>
            /// <returns>A list of the regions that have been extracted from the frame XML node</returns>
            public static Frame.Region[] Build(XElement frameElement, IDictionary<string, Texture2D> bitmaps)
            {
                RegionListBuilder builder = new RegionListBuilder();
                builder.retrieveBorderSizes(frameElement);
                return builder.createAndPlaceRegions(frameElement, bitmaps);
            }

            /// <summary>Retrieves the sizes of the border regions in a frame</summary>
            /// <param name="frameElement">XML node for the frame containing the region</param>
            private void retrieveBorderSizes(XElement frameElement)
            {
                foreach (XElement element in frameElement.Descendants("region"))
                {

                    // Left and right border width determination
                    string hplacement = element.Attribute("hplacement").Value;
                    string w = element.Attribute("w").Value;
                    if (hplacement == "left")
                    {
                        _leftBorderWidth = Math.Max(_leftBorderWidth, int.Parse(w));
                    }
                    else if (hplacement == "right")
                    {
                        _rightBorderWidth = Math.Max(_rightBorderWidth, int.Parse(w));
                    }

                    // Top and bottom border width determination
                    string vplacement = element.Attribute("vplacement").Value;
                    string h = element.Attribute("h").Value;
                    if (vplacement == "top")
                    {
                        _topBorderWidth = Math.Max(_topBorderWidth, int.Parse(h));
                    }
                    else if (vplacement == "bottom")
                    {
                        _bottomBorderWidth = Math.Max(_bottomBorderWidth, int.Parse(h));
                    }

                }
            }

            /// <summary>Creates and places the regions needed to be drawn to render the frame</summary>
            /// <param name="frameElement">XML node for the frame containing the region</param>
            /// <param name="bitmaps">Bitmap lookup table to associate a region's bitmap id to the real bitmap</param>
            /// <returns>The regions created for the frame</returns>
            private Frame.Region[] createAndPlaceRegions(
              XElement frameElement, IDictionary<string, Texture2D> bitmaps
            )
            {
                var regions = new List<Frame.Region>();

                // Fill all regions making up the current frame
                foreach (XElement element in frameElement.Descendants("region"))
                {
                    XAttribute idAttribute = element.Attribute("id");
                    string id = (idAttribute == null) ? null : idAttribute.Value;
                    string source = element.Attribute("source").Value;
                    string hplacement = element.Attribute("hplacement").Value;
                    string vplacement = element.Attribute("vplacement").Value;
                    string x = element.Attribute("x").Value;
                    string y = element.Attribute("y").Value;
                    string w = element.Attribute("w").Value;
                    string h = element.Attribute("h").Value;

                    // Assign the trivial attributes
                    var region = new Frame.Region()
                    {
                        Id = id,
                        Texture = bitmaps[source]
                    };
                    region.SourceRegion.X = int.Parse(x);
                    region.SourceRegion.Y = int.Parse(y);
                    region.SourceRegion.Width = int.Parse(w);
                    region.SourceRegion.Height = int.Parse(h);

                    // Process each region's placement and set up the unified coordinates
                    calculateRegionPlacement(
                      getHorizontalPlacementIndex(hplacement),
                      int.Parse(w),
                      _leftBorderWidth,
                      _rightBorderWidth,
                      ref region.DestinationRegion.Location.X,
                      ref region.DestinationRegion.Size.X
                    );
                    calculateRegionPlacement(
                      getVerticalPlacementIndex(vplacement),
                      int.Parse(h),
                      _topBorderWidth,
                      _bottomBorderWidth,
                      ref region.DestinationRegion.Location.Y,
                      ref region.DestinationRegion.Size.Y
                    );

                    regions.Add(region);
                }

                return regions.ToArray();
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
                            size.Offset = -(highBorderWidth + lowBorderWidth);
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
        }

        #endregion // class RegionListBuilder

        #region class TextListBuilder

        /// <summary>Builds a text list from the regions in an frame XML node</summary>
        private class TextListBuilder
        {
            /// <summary>
            ///   Builds a text list from the text placements specified in the provided node
            /// </summary>
            /// <param name="frameElement">
            ///   XML node for the frame whose text placements wille be processed
            /// </param>
            /// <param name="fonts">
            ///   Font lookup table used to associate a text's font id to the real font
            /// </param>
            /// <returns>
            ///   A list of the texts that have been extracted from the frame XML node
            /// </returns>
            public static Frame.Text[] Build(XElement frameElement, IDictionary<string, SpriteFont> fonts)
            {
                var texts = new List<Frame.Text>();

                foreach (XElement element in frameElement.Descendants("text"))
                {
                    string font = element.Attribute("font").Value;
                    string horizontalPlacement = element.Attribute("hplacement").Value;
                    string verticalPlacement = element.Attribute("vplacement").Value;

                    XAttribute xOffsetAttribute = element.Attribute("xoffset");
                    int xOffset = (xOffsetAttribute == null) ? 0 : int.Parse(xOffsetAttribute.Value);

                    XAttribute yOffsetAttribute = element.Attribute("yoffset");
                    int yOffset = (yOffsetAttribute == null) ? 0 : int.Parse(yOffsetAttribute.Value);

                    XAttribute colorAttribute = element.Attribute("color");
                    Color color;
                    if (colorAttribute == null)
                    {
                        color = Color.White;
                    }
                    else {
                        color = colorFromString(colorAttribute.Value);
                    }

                    var text = new Frame.Text()
                    {
                        Font = fonts[font],
                        HorizontalPlacement = horizontalPlacementFromString(
                        horizontalPlacement
                      ),
                        VerticalPlacement = verticalPlacementFromString(
                        verticalPlacement
                      ),
                        Offset = new Point(xOffset, yOffset),
                        Color = color
                    };
                    texts.Add(text);
                }

                return texts.ToArray();
            }

            /// <summary>Converts a string into a horizontal placement enumeration value</summary>
            /// <param name="placement">Placement string that will be converted</param>
            /// <returns>The horizontal placement enumeration value matching the string</returns>
            private static Frame.HorizontalTextAlignment horizontalPlacementFromString(string placement)
            {
                switch (placement)
                {
                    case "left": { return Frame.HorizontalTextAlignment.Left; }
                    case "right": { return Frame.HorizontalTextAlignment.Right; }
                    case "center":
                    default: { return Frame.HorizontalTextAlignment.Center; }
                }
            }

            /// <summary>Converts a string into a vertical placement enumeration value</summary>
            /// <param name="placement">Placement string that will be converted</param>
            /// <returns>The vertical placement enumeration value matching the string</returns>
            private static Frame.VerticalTextAlignment verticalPlacementFromString(string placement)
            {
                switch (placement)
                {
                    case "top": { return Frame.VerticalTextAlignment.Top; }
                    case "bottom": { return Frame.VerticalTextAlignment.Bottom; }
                    case "center":
                    default: { return Frame.VerticalTextAlignment.Center; }
                }
            }

        }

        #endregion // class TextListBuilder

        /// <summary>Loads a skin from the specified path</summary>
        /// <param name="skinStream">Stream containing the skin description</param>
        private void loadSkin(Stream skinStream)
        {

#if NO_XMLSCHEMA
      
      var skinDocument = XDocument.Load(skinStream);

#else

            // Load the schema
            XmlSchema schema;
            using (Stream schemaStream = getResourceStream("Resources.skin.xsd"))
            {
                schema = XmlHelper.LoadSchema(schemaStream);
            }

            // Load the XML document and validate it against the schema
            XDocument skinDocument = XmlHelper.LoadDocument(schema, skinStream);

#endif

            // The XML document is validated, we don't have to worry about the structure
            // of the thing anymore, only about the values it provides us with ;)
            // Load everything contained in the skin and set up our data structures
            // so we can efficiently render everything
            loadResources(skinDocument);
            loadFrames(skinDocument);

        }

        /// <summary>Loads the resources contained in a skin document</summary>
        /// <param name="skinDocument">
        ///   XML document containing a skin description whose resources will be loaded
        /// </param>
        private void loadResources(XDocument skinDocument)
        {

            // Get the resources node containing a list of all resources of the skin
            XElement resources = skinDocument.Element("skin").Element("resources");

            // Load all fonts used by the skin
            foreach (XElement element in resources.Descendants("font"))
            {
                string fontName = element.Attribute("name").Value;
                string contentPath = element.Attribute("contentPath").Value;

                SpriteFont spriteFont = this._contentManager.Load<SpriteFont>(contentPath);
                this._fonts.Add(fontName, spriteFont);
            }

            // Load all bitmaps used by the skin
            foreach (XElement element in resources.Descendants("bitmap"))
            {
                string bitmapName = element.Attribute("name").Value;
                string contentPath = element.Attribute("contentPath").Value;

                Texture2D bitmap = this._contentManager.Load<Texture2D>(contentPath);
                this._bitmaps.Add(bitmapName, bitmap);
            }

        }

        /// <summary>Loads the frames contained in a skin document</summary>
        /// <param name="skinDocument">
        ///   XML document containing a skin description whose frames will be loaded
        /// </param>
        private void loadFrames(XDocument skinDocument)
        {

            // Load all the frames specified by the skin
            XElement resources = skinDocument.Element("skin").Element("frames");
            foreach (XElement element in resources.Descendants("frame"))
            {
                string name = element.Attribute("name").Value;

                Frame.Region[] regions = RegionListBuilder.Build(element, this._bitmaps);
                Frame.Text[] texts = TextListBuilder.Build(element, this._fonts);
                this._frames.Add(name, new Frame(regions, texts));
            }

        }

        /// <summary>Returns a stream for a resource embedded in this assembly</summary>
        /// <param name="resourceName">Name of the resource for which to get a stream</param>
        /// <returns>A stream for the specified embedded resource</returns>
        private static Stream getResourceStream(string resourceName)
        {
            Assembly self = typeof(FlatGuiGraphics).GetTypeInfo().Assembly;
            string[] resources = self.GetManifestResourceNames();
            return self.GetManifestResourceStream(resourceName);
        }

        /// <summary>Converts a string in the style "#rrggbb" into a Color value</summary>
        /// <param name="color">String containing a hexadecimal color value</param>
        /// <returns>The equivalent color as a Color value</returns>
        private static Color colorFromString(string color)
        {
            string trimmedColor = color.Trim();

            int startIndex = 0;
            if (trimmedColor[0] == '#')
            {
                ++startIndex;
            }

            bool isValidColor =
              ((trimmedColor.Length - startIndex) == 6) ||
              ((trimmedColor.Length - startIndex) == 8);

            if (!isValidColor)
            {
                throw new ArgumentException("Invalid color format '" + color + "'", "color");
            }

            int r = Convert.ToInt32(trimmedColor.Substring(startIndex + 0, 2), 16);
            int g = Convert.ToInt32(trimmedColor.Substring(startIndex + 2, 2), 16);
            int b = Convert.ToInt32(trimmedColor.Substring(startIndex + 4, 2), 16);
            int a;
            if ((trimmedColor.Length - startIndex) == 8)
            {
                a = Convert.ToInt32(trimmedColor.Substring(startIndex + 6, 2), 16);
            }
            else {
                a = 255;
            }

            // No need to worry about overflows: two hexadecimal digits can
            // by definition not grow larger than 255 ;-)        
            return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }

    }
}