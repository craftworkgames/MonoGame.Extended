using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiListBox : GuiItemsControl
    {
        public GuiListBox()
            : this(null)
        {
        }

        public GuiListBox(GuiSkin skin)
            : base(skin)
        {
        }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var width = 0f;
            var height = 0f;

            foreach (var item in Items)
            {
                var itemSize = GetItemSize(context, availableSize, item);

                if (itemSize.Width > width)
                    width = itemSize.Width;

                height += itemSize.Height;
            }

            return new Size2(width + ClipPadding.Size.Width, height + ClipPadding.Size.Height);
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            ScrollIntoView(context);
            DrawItemList(context, renderer);
        }

        protected override Rectangle GetContentRectangle(IGuiContext context)
        {
            return ClippingRectangle;
        }
    }
}