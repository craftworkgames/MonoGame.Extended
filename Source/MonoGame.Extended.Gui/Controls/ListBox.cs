using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class ListBox : SelectorControl
    {
        public ListBox()
        {
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var width = 0;
            var height = 0;

            foreach (var item in Items)
            {
                var itemSize = GetItemSize(context, item);

                if (itemSize.Width > width)
                    width = itemSize.Width;

                height += itemSize.Height;
            }

            return new Size(width + ClipPadding.Width, height + ClipPadding.Height);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            ScrollIntoView(context);
            DrawItemList(context, renderer);
        }

        protected override Rectangle GetListAreaRectangle(IGuiContext context)
        {
            return ContentRectangle;
        }
    }
}