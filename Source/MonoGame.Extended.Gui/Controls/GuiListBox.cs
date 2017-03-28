using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiListBox : GuiControl
    {
        public GuiListBox()
        {
        }

        public GuiListBox(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        public List<string> Items { get; set; } = new List<string> {"Item 1", "Item 2", "Item 3"};

        //protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        //{
        //    base.DrawBackground(context, renderer, deltaSeconds);

        //    var targetRectangle = ClippingRectangle;

        //    foreach (var item in Items)
        //    {
        //        var itemTextInfo = GetTextInfo(context, item, targetRectangle, HorizontalAlignment.Left, VerticalAlignment.Top);
        //        var rectangle = new Rectangle(itemTextInfo.Position.ToPoint(), itemTextInfo.Size.ToPoint());
        //        renderer.DrawRectangle(rectangle, Color.LightBlue);
        //        targetRectangle.Y += (int)itemTextInfo.Size.Y;
        //    }
        //}

        protected override void DrawText(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            var targetRectangle = ClippingRectangle;

            foreach (var item in Items)
            {
                var itemTextInfo = GetTextInfo(context, item, targetRectangle, HorizontalAlignment.Left, VerticalAlignment.Top);
                renderer.DrawText(itemTextInfo.Font, itemTextInfo.Text, itemTextInfo.Position + TextOffset, itemTextInfo.Color, itemTextInfo.ClippingRectangle);
                targetRectangle.Y += (int)itemTextInfo.Size.Y;
            }
        }
    }
}