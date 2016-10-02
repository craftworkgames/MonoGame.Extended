using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    /// <summary>Horizontal slider that can be moved using the mouse</summary>
    public class GuiHorizontalSliderControl : GuiSliderControl
    {
        /// <summary>Obtains the region covered by the slider's thumb</summary>
        /// <returns>The region covered by the slider's thumb</returns>
        protected override RectangleF GetThumbRegion()
        {
            RectangleF bounds = GetAbsoluteBounds();

            if (ThumbLocator != null)
            {
                return ThumbLocator.GetThumbPosition(bounds, ThumbPosition, ThumbSize);
            }
            else {
                float thumbWidth = bounds.Width * ThumbSize;
                float thumbX = (bounds.Width - thumbWidth) * ThumbPosition;

                return new RectangleF(thumbX, 0, thumbWidth, bounds.Height);
            }
        }

        /// <summary>Moves the thumb to the specified location</summary>
        /// <returns>Location the thumb will be moved to</returns>
        protected override void MoveThumb(float x, float y)
        {
            RectangleF bounds = GetAbsoluteBounds();

            float thumbWidth = bounds.Width * ThumbSize;
            float maxX = bounds.Width - thumbWidth;

            // Prevent divide-by-zero if the thumb fills out the whole rail
            if (maxX > 0.0f)
            {
                ThumbPosition = MathHelper.Clamp(x / maxX, 0.0f, 1.0f);
            }
            else
            {
                ThumbPosition = 0.0f;
            }

            OnMoved();
        }

    }
}
}
