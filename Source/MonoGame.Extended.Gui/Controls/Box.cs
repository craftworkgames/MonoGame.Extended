using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class Box : Control
    {
        public override IEnumerable<Control> Children { get; } = Enumerable.Empty<Control>();

        public override Size GetContentSize(IGuiContext context)
        {
            return new Size(Width, Height);
        }

        public Color FillColor { get; set; } = Color.White;

        private Rectangle _fillRectangle;
        public Rectangle FillRectangle
        {
            get
            {
                if (_fillRectangle.IsEmpty)
                {
                    var r = ContentRectangle;
                    return new Rectangle(r.Left + 4, r.Top + 4, r.Width - 8, r.Height - 8);
                }
                else
                    return _fillRectangle;
            }
            set
            {
                _fillRectangle = value;
            }
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);
            renderer.FillRectangle(FillRectangle, FillColor);
        }

    }
}