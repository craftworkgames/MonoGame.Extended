using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>Vertical slider that can be moved using the mouse</summary>
    public class GuiVerticalSliderControl : GuiSliderControl
    {
        /// <summary>Obtains the region covered by the slider's thumb</summary>
        /// <returns>The region covered by the slider's thumb</returns>
        protected override RectangleF GetThumbRegion()
        {
            var bounds = GetAbsoluteBounds();

            if (ThumbLocator != null)
                return ThumbLocator.GetThumbPosition(bounds, ThumbPosition, ThumbSize);
            var thumbHeight = bounds.Height*ThumbSize;
            var thumbY = (bounds.Height - thumbHeight)*ThumbPosition;

            return new RectangleF(0, thumbY, bounds.Width, thumbHeight);
        }

        /// <summary>Moves the thumb to the specified location</summary>
        /// <param name="x">X coordinate for the new left border of the thumb</param>
        /// <param name="y">Y coordinate for the new upper border of the thumb</param>
        protected override void MoveThumb(float x, float y)
        {
            var bounds = GetAbsoluteBounds();

            var thumbHeight = bounds.Height*ThumbSize;
            var maxY = bounds.Height - thumbHeight;

            // Prevent divide-by-zero if the thumb fills out the whole rail
            if (maxY > 0.0f)
                ThumbPosition = MathHelper.Clamp(y/maxY, 0.0f, 1.0f);
            else ThumbPosition = 0.0f;

            OnMoved();
        }
    }
}