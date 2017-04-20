using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
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

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public List<object> Items { get; } = new List<object>();
        public Color SelectedTextColor { get; set; } = Color.White;
        public Thickness ItemPadding { get; set; } = new Thickness(4, 2);

        public event EventHandler SelectedIndexChanged;

        private int _firstIndex;

        public object SelectedItem
        {
            get { return SelectedIndex >= 0 && SelectedIndex <= Items.Count - 1 ? Items[SelectedIndex] : null; }
            set { SelectedIndex = Items.IndexOf(value); }
        }

        public override void OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            base.OnKeyPressed(context, args);

            if (args.Key == Keys.Down) ScrollDown();
            if (args.Key == Keys.Up) ScrollUp();
        }

        public override void OnScrolled(int delta)
        {
            base.OnScrolled(delta);

            if (delta < 0) ScrollDown();
            if (delta > 0) ScrollUp();
        }

        private void ScrollDown()
        {
            if (SelectedIndex < Items.Count - 1)
                SelectedIndex++;
        }

        private void ScrollUp()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
        }

        public override void OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerDown(context, args);

            for (var i = _firstIndex; i < Items.Count; i++)
            {
                var itemRectangle = GetItemRectangle(context, i - _firstIndex);

                if (itemRectangle.Contains(args.Position))
                {
                    SelectedIndex = i;
                    break;
                }
            }
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            ScrollIntoView(context);

            for (var i = _firstIndex; i < Items.Count; i++)
            {
                var itemRectangle = GetItemRectangle(context, i - _firstIndex);

                if (SelectedIndex == i)
                    renderer.FillRectangle(itemRectangle, Color.CornflowerBlue, ClippingRectangle);

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

        private void ScrollIntoView(IGuiContext context)
        {
            var selectedItemRectangle = GetItemRectangle(context, SelectedIndex - _firstIndex);

            if (selectedItemRectangle.Bottom > ClippingRectangle.Bottom)
                _firstIndex++;

            if (selectedItemRectangle.Top < ClippingRectangle.Top && _firstIndex > 0)
                _firstIndex--;
        }
    }
}