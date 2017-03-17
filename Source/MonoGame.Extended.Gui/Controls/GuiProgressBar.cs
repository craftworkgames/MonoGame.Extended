using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiProgressBar : GuiControl
    {
        public GuiProgressBar()
        {
        }

        public GuiProgressBar(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        private float _progress = 1.0f;
        public float Progress
        {
            get { return _progress; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if(_progress != value)
                {
                    _progress = value;
                    ProgressChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TextureRegion2D BarRegion { get; set; }

        public event EventHandler ProgressChanged;

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.DrawBackground(context, renderer, deltaSeconds);

            var boundingRectangle = BoundingRectangle;
            var clippingRectangle = new Rectangle(boundingRectangle.X, boundingRectangle.Y, (int)(boundingRectangle.Width * Progress), boundingRectangle.Height);
            renderer.DrawRegion(BarRegion, BoundingRectangle, Color, clippingRectangle);
        }
    }
}