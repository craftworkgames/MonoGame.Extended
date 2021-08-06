using System;
using MonoGame.Extended.NuclexGui.Controls.Desktop;

namespace MonoGame.Extended.NuclexGui.Visuals.Flat.Renderers
{
    /// <summary>Renders text input controls in a traditional flat style</summary>
    public class FlatListControlRenderer : IFlatControlRenderer<GuiListControl>, IListRowLocator
    {
        /// <summary>Style used to draw this control</summary>
        private const string _style = "list";

        //   Otherwise the renderer could try to renderer when no frame is being drawn.
        //   Also, this way the renderer makes the assumption that all drawing happens
        //   through one graphics interface only.

        /// <summary>Graphics interface we used for the last draw call</summary>
        private IFlatGuiGraphics _graphics;

        /// <summary>Height of a single row in the list</summary>
        private float _rowHeight = float.NaN;

        /// <summary>
        ///     Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///     Graphics interface that will be used to draw the control
        /// </param>
        public void Render(GuiListControl control, IFlatGuiGraphics graphics)
        {
            _graphics = graphics;

            var controlBounds = control.GetAbsoluteBounds();

            graphics.DrawElement(_style, controlBounds);

            // Cache the number of items in the list (as a float, this comes in handy later)
            // and the height of a single item when rendered
            float totalItems = control.Items.Count;
            var rowHeight = GetRowHeight(controlBounds);

            // Number of items (+fraction) fitting into the list's height
            var itemsInView = controlBounds.Height/rowHeight;

            // Number of items by which the slider can move up and down
            var scrollableArea = Math.Max(totalItems - itemsInView, 0.0f);

            // Index (+fraction) of the item at the top of the list given
            // the slider's current position
            var scrollPosition = control.Slider.ThumbPosition*scrollableArea;

            // Determine the first and the last item we need to draw
            // (no need to draw the whole of the list when only a small subset
            // will end up in the clipping area)
            var firstItem = (int) scrollPosition;
            var lastItem = (int) Math.Ceiling(scrollPosition + itemsInView);
            lastItem = Math.Min(lastItem, control.Items.Count);

            // Set up a rectangle we can use to track the bounds of the item
            // currently being rendered
            var itemBounds = controlBounds;
            itemBounds.Y -= (scrollPosition - firstItem)*rowHeight;
            itemBounds.Height = rowHeight;

            using (graphics.SetClipRegion(controlBounds))
            {
                for (var item = firstItem; item < lastItem; ++item)
                {
                    if (control.SelectedItems.Contains(item))
                        graphics.DrawElement("list.selection", itemBounds);

                    graphics.DrawString(_style, itemBounds, control.Items[item]);
                    itemBounds.Y += rowHeight;
                }
            }

            control.ListRowLocator = this;
        }

        /// <summary>Calculates the list row the cursor is in</summary>
        /// <param name="bounds">
        ///     Boundaries of the control, should be in absolute coordinates
        /// </param>
        /// <param name="thumbPosition">
        ///     Position of the thumb in the list's slider
        /// </param>
        /// <param name="itemCount">
        ///     Number of items contained in the list
        /// </param>
        /// <param name="y">Vertical position of the cursor</param>
        /// <returns>The row the cursor is over</returns>
        public int GetRow(RectangleF bounds, float thumbPosition, int itemCount, float y)
        {
            float totalItems = itemCount;
            var rowHeight = GetRowHeight(bounds);

            // Number of items (+fraction) fitting into the list's height
            var itemsInView = bounds.Height/rowHeight;

            // Number of items by which the slider can move up and down
            var scrollableArea = totalItems - itemsInView;

            // Index (+fraction) of the item at the top of the list given
            // the slider's current position
            var scrollPosition = thumbPosition*scrollableArea;

            // Calculate the item that should be under the requested Y coordinate
            return (int) (y/GetRowHeight(bounds) + scrollPosition);
        }

        /// <summary>Determines the height of a row displayed in the list</summary>
        /// <param name="bounds">
        ///     Boundaries of the control, should be in absolute coordinates
        /// </param>
        /// <returns>The height of a single row in the list</returns>
        public float GetRowHeight(RectangleF bounds)
        {
            //   The code below is not optimal, but the XNA SpriteFont isn't very talkative
            //   when it comes to providing informations about itself ;)

            if (float.IsNaN(_rowHeight))
            {
                _rowHeight = _graphics.MeasureString("list", bounds, "qyjpMAW!").Height;
                _rowHeight += 2.0f;
            }

            return _rowHeight;
        }
    }
}