using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat
{
    public partial class FlatGuiGraphics : IFlatGuiGraphics, IDisposable
    {
        /// <summary>Width of the caret used for text input</summary>
        const float CaretWidth = 2.0f;

        #region class ScissorKeeper

        /// <summary>Manages the scissor rectangle for the GUI graphics interface</summary>
        private class ScissorKeeper : IDisposable
        {
            /// <summary>GUI graphics interface for which the scissor rectangle is managed</summary>
            private FlatGuiGraphics _flatGuiGraphics;

            /// <summary>Scissor rectangle that was previously assigned to the graphics device</summary>
            private Rectangle _oldScissorRectangle;

            /// <summary>Initializes a new scissor manager</summary>
            /// <param name="flatGuiGraphics">GUI graphics interface the scissor rectangle will be managed for</param>
            public ScissorKeeper(FlatGuiGraphics flatGuiGraphics)
            {
                _flatGuiGraphics = flatGuiGraphics;
            }

            /// <summary>Assigns the scissor rectangle to the graphics device</summary>
            /// <param name="clipRegion">Scissor rectangle that will be assigned</param>
            public void Assign(ref Rectangle clipRegion)
            {
                _flatGuiGraphics.endSpriteBatch();
                try
                {
                    GraphicsDevice graphics = _flatGuiGraphics._spriteBatch.GraphicsDevice;
                    _oldScissorRectangle = graphics.ScissorRectangle;
                    graphics.ScissorRectangle = clipRegion;
                }
                finally
                {
                    _flatGuiGraphics.beginSpriteBatch();
                }
            }

            /// <summary>Releases the currently assigned scissor rectangle again</summary>
            public void Dispose()
            {
                _flatGuiGraphics.endSpriteBatch();
                try
                {
                    GraphicsDevice graphics = _flatGuiGraphics._spriteBatch.GraphicsDevice;
                    graphics.ScissorRectangle = _oldScissorRectangle;
                }
                finally
                {
                    _flatGuiGraphics.beginSpriteBatch();
                }
            }
        }

        #endregion // class ScissorKeeper

        #region Fields

        /// <summary>String builder used for various purposes in this class</summary>
        private StringBuilder _stringBuilder;
        /// <summary>Locates openings between letters in strings</summary>
        private OpeningLocator _openingLocator;
        /// <summary>Manages the scissor rectangle and its assignment time</summary>
        private ScissorKeeper _scissorManager;
        /// <summary>Batches GUI elements for faster drawing</summary>
        private SpriteBatch _spriteBatch;
        /// <summary>Manages the content used to draw the GUI</summary>
        private ContentManager _contentManager;
        /// <summary>Font styles known to the GUI</summary>
        private Dictionary<string, SpriteFont> _fonts;
        /// <summary>Bitmaps containing resources for the GUI</summary>
        private Dictionary<string, Texture2D> _bitmaps;
        /// <summary>Types of frames the painter can draw</summary>
        private Dictionary<string, Frame> _frames;
        /// <summary>Rasterizer state used for drawing the GUI</summary>
        private RasterizerState _rasterizerState;

        #endregion

        /// <summary>Initializes a new gui painter</summary>
        /// <param name="contentManager">
        ///   Content manager containing the resources for the GUI. The instance takes
        ///   ownership of the content manager and will dispose it.
        /// </param>
        /// <param name="skinStream">
        ///   Stream from which the skin description will be read
        /// </param>
        public FlatGuiGraphics(ContentManager contentManager, Stream skinStream)
        {
            IGraphicsDeviceService graphicsDeviceService =
              (IGraphicsDeviceService)contentManager.ServiceProvider.GetService(
                typeof(IGraphicsDeviceService)
              );

            _spriteBatch = new SpriteBatch(graphicsDeviceService.GraphicsDevice);
            _contentManager = contentManager;
            _openingLocator = new OpeningLocator();
            _stringBuilder = new StringBuilder(64);
            _scissorManager = new ScissorKeeper(this);
            _rasterizerState = new RasterizerState()
            {
                ScissorTestEnable = true
            };
            _fonts = new Dictionary<string, SpriteFont>();
            _bitmaps = new Dictionary<string, Texture2D>();
            _frames = new Dictionary<string, Frame>();

            LoadSkin(skinStream);
        }

        private void LoadSkin(Stream skinStream)
        {
            GuiSkin skin = GuiSkin.FromStream(skinStream);

            foreach (var font in skin.resources.font)
                _fonts.Add(font.name, _contentManager.Load<SpriteFont>(font.contentPath));

            foreach (var texture in skin.resources.bitmap)
                _bitmaps.Add(texture.name, _contentManager.Load<Texture2D>(texture.contentPath));

            foreach (var frame in skin.frames)
            {
                if (frame.Regions != null)
                {
                    for (int i = 0; i < frame.Regions.Length; i++)
                        frame.Regions[i].Texture = _bitmaps[frame.Regions[i].Source];

                    _frames.Add(frame.Name, frame);
                }
            }
        }

        /// <summary>Immediately releases all resources owned by the instance</summary>
        public void Dispose()
        {
            if (_contentManager != null)
            {
                _contentManager.Dispose();
                _contentManager = null;
            }
            if (_spriteBatch != null)
            {
                _spriteBatch.Dispose();
                _spriteBatch = null;
            }
        }

        /// <summary>
        ///   Positions a string within a frame according to the positioning instructions
        ///   stored in the provided text anchor.
        /// </summary>
        /// <param name="anchor">Text anchor the string will be positioned for</param>
        /// <param name="bounds">Boundaries of the control the string is rendered in</param>
        /// <param name="text">String that will be positioned</param>
        /// <returns>The position of the string within the control</returns>
        private Vector2 positionText(ref Frame.Text anchor, RectangleF bounds, string text)
        {
            Vector2 textSize = anchor.Font.MeasureString(text);
            float x, y;

            switch (anchor.HorizontalPlacement)
            {
                case Frame.HorizontalTextAlignment.Left:
                    {
                        x = bounds.Left;
                        break;
                    }
                case Frame.HorizontalTextAlignment.Right:
                    {
                        x = bounds.Right - textSize.X;
                        break;
                    }
                case Frame.HorizontalTextAlignment.Center:
                default:
                    {
                        x = (bounds.Width - textSize.X) / 2.0f + bounds.Left;
                        break;
                    }
            }

            switch (anchor.VerticalPlacement)
            {
                case Frame.VerticalTextAlignment.Top:
                    {
                        y = bounds.Top;
                        break;
                    }
                case Frame.VerticalTextAlignment.Bottom:
                    {
                        y = bounds.Bottom - anchor.Font.LineSpacing;
                        break;
                    }
                case Frame.VerticalTextAlignment.Center:
                default:
                    {
                        y = (bounds.Height - anchor.Font.LineSpacing) / 2.0f + bounds.Top;
                        break;
                    }
            }

            return new Vector2(floor(x + anchor.Offset.X), floor(y + anchor.Offset.Y));
        }

        /// <summary>
        ///   Calculates the absolute pixel position of a rectangle in unified coordinates
        /// </summary>
        /// <param name="bounds">Bounds of the drawing area in pixels</param>
        /// <param name="destination">Destination rectangle in unified coordinates</param>
        /// <returns>
        ///   The destination rectangle converted to absolute pixel coordinates
        /// </returns>
        private static Rectangle calculateDestinationRectangle(ref RectangleF bounds, ref UniRectangle destination)
        {
            int x = (int)(bounds.X + destination.Location.X.Offset);
            x += (int)(bounds.Width * destination.Location.X.Fraction);

            int y = (int)(bounds.Y + destination.Location.Y.Offset);
            y += (int)(bounds.Height * destination.Location.Y.Fraction);

            int width = (int)(destination.Size.X.Offset);
            width += (int)(bounds.Width * destination.Size.X.Fraction);

            int height = (int)(destination.Size.Y.Offset);
            height += (int)(bounds.Height * destination.Size.Y.Fraction);

            return new Rectangle(x, y, width, height);
        }

        /// <summary>Looks up the frame with the specified name</summary>
        /// <param name="frameName">Frame that will be looked up</param>
        /// <returns>The frame with the specified name</returns>
        private Frame lookupFrame(string frameName)
        {

            // Make sure the renderer specified a valid frame name. If someone modifies
            // the skin or uses a skin which does not support all required controls,
            // this will provide the user with a clear error message.
            Frame frame;
            if (!_frames.TryGetValue(frameName, out frame))
            {
                throw new ArgumentException("Unknown frame type: '" + frameName + "'", "frameName");
            }

            return frame;

        }

        /// <summary>Removes the fractional part from the floating point value</summary>
        /// <param name="value">Value whose fractional part will be removed</param>
        /// <returns>The floating point value without its fractional part</returns>
        private static float floor(float value)
        {
            return (float)Math.Floor(value);
        }
    }
}
