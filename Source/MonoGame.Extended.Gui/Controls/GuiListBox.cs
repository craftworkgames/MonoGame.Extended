using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiListBox : GuiControl
    {
        public GuiListBox()
            : this(null)
        {
        }

        public GuiListBox(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        public int SelectedIndex { get; set; } = -1;
        public List<object> Items { get; } = new List<object>();
        public Color SelectedTextColor { get; set; } = Color.White;
        public Thickness ItemPadding { get; set; } = new Thickness(4, 2);

        public object SelectedItem
        {
            get { return SelectedIndex >= 0 && SelectedIndex <= Items.Count - 1 ? Items[SelectedIndex] : null; }
            set { SelectedIndex = Items.IndexOf(value); }
        }

        public override void OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerDown(context, args);

            for (var i = 0; i < Items.Count; i++)
            {
                var itemRectangle = GetItemRectangle(context, i);

                if (itemRectangle.Contains(args.Position))
                {
                    SelectedIndex = i;
                    break;
                }
            }
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var itemRectangle = GetItemRectangle(context, i);

                if (SelectedIndex == i)
                    renderer.FillRectangle(itemRectangle, Color.CornflowerBlue);

                var item = Items[i];
                var textRectangle = new Rectangle(itemRectangle.X + ItemPadding.Left, itemRectangle.Y + ItemPadding.Top,
                    itemRectangle.Width - ItemPadding.Right, itemRectangle.Height - ItemPadding.Bottom);
                var itemTextInfo = GetTextInfo(context, item?.ToString() ?? string.Empty, textRectangle, HorizontalAlignment.Left, VerticalAlignment.Top);
                var textColor = i == SelectedIndex ? SelectedTextColor : itemTextInfo.Color;

                renderer.DrawText(itemTextInfo.Font, itemTextInfo.Text, itemTextInfo.Position + TextOffset, textColor, itemTextInfo.ClippingRectangle);
            }
        }

        private Rectangle GetItemRectangle(IGuiContext context, int index)
        {
            var font = Font ?? context.DefaultFont;
            var contentRectangle = ClippingRectangle;
            var itemHeight = font.LineHeight + ItemPadding.Top + ItemPadding.Bottom;
            return new Rectangle(contentRectangle.X, contentRectangle.Y + itemHeight * index, contentRectangle.Width, itemHeight);
        }
    }
}