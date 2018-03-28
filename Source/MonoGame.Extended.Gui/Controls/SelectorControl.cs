using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class SelectorControl : Control
    {
        protected SelectorControl()
        {
        }

        private int _selectedIndex = -1;
        public virtual int SelectedIndex
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

        public override IEnumerable<Control> Children => Items.OfType<Control>();

        public virtual List<object> Items { get; } = new List<object>();
        public virtual Color SelectedTextColor { get; set; } = Color.White;
        public virtual Color SelectedItemColor { get; set; } = Color.CornflowerBlue;
        public virtual Thickness ItemPadding { get; set; } = new Thickness(4, 2);
        public virtual string NameProperty { get; set; }

        public event EventHandler SelectedIndexChanged;

        protected int FirstIndex;

        public object SelectedItem
        {
            get { return SelectedIndex >= 0 && SelectedIndex <= Items.Count - 1 ? Items[SelectedIndex] : null; }
            set { SelectedIndex = Items.IndexOf(value); }
        }

        public override bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Down) ScrollDown();
            if (args.Key == Keys.Up) ScrollUp();

            return base.OnKeyPressed(context, args);
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

        public override bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        {
            var contentRectangle = GetListAreaRectangle(context);

            for (var i = FirstIndex; i < Items.Count; i++)
            {
                var itemRectangle = GetItemRectangle(context, i - FirstIndex, contentRectangle);

                if (itemRectangle.Contains(args.Position))
                {
                    SelectedIndex = i;
                    OnItemClicked(context, args);
                    break;
                }
            }

            return base.OnPointerDown(context, args);
        }

        protected virtual void OnItemClicked(IGuiContext context, PointerEventArgs args) { }

        protected TextInfo GetItemTextInfo(IGuiContext context, Rectangle itemRectangle, object item)
        {
            var textRectangle = new Rectangle(itemRectangle.X + ItemPadding.Left, itemRectangle.Y + ItemPadding.Top,
                itemRectangle.Width - ItemPadding.Right, itemRectangle.Height - ItemPadding.Bottom);
            var itemTextInfo = GetTextInfo(context, GetItemName(item), textRectangle, HorizontalTextAlignment, VerticalTextAlignment);
            return itemTextInfo;
        }

        private string GetItemName(object item)
        {
            if (item != null)
            {
                if (NameProperty != null)
                {
                    return item.GetType()
                        .GetRuntimeProperty(NameProperty)
                        .GetValue(item)
                        ?.ToString() ?? string.Empty;
                }

                return item.ToString();
            }

            return string.Empty;
        }

        protected Rectangle GetItemRectangle(IGuiContext context, int index, Rectangle contentRectangle)
        {
            var font = Font ?? context.DefaultFont;
            var itemHeight = font.LineHeight + ItemPadding.Top + ItemPadding.Bottom;
            return new Rectangle(contentRectangle.X, contentRectangle.Y + itemHeight * index, contentRectangle.Width, itemHeight);
        }

        protected void ScrollIntoView(IGuiContext context)
        {
            var contentRectangle = GetListAreaRectangle(context);
            var selectedItemRectangle = GetItemRectangle(context, SelectedIndex - FirstIndex, contentRectangle);

            if (selectedItemRectangle.Bottom > ClippingRectangle.Bottom)
                FirstIndex++;

            if (selectedItemRectangle.Top < ClippingRectangle.Top && FirstIndex > 0)
                FirstIndex--;
        }

        protected Size GetItemSize(IGuiContext context, object item)
        {
            var text = GetItemName(item);
            var font = Font ?? context.DefaultFont;
            var textSize = (Size)font.MeasureString(text ?? string.Empty);
            var itemWidth = textSize.Width + ItemPadding.Width;
            var itemHeight = textSize.Height + ItemPadding.Height;
            return new Size(itemWidth, itemHeight);
        }

        protected abstract Rectangle GetListAreaRectangle(IGuiContext context);

        protected void DrawItemList(IGuiContext context, IGuiRenderer renderer)
        {
            var listRectangle = GetListAreaRectangle(context);

            for (var i = FirstIndex; i < Items.Count; i++)
            {
                var item = Items[i];
                var itemRectangle = GetItemRectangle(context, i - FirstIndex, listRectangle);
                var itemTextInfo = GetItemTextInfo(context, itemRectangle, item);
                var textColor = i == SelectedIndex ? SelectedTextColor : itemTextInfo.Color;

                if (SelectedIndex == i)
                    renderer.FillRectangle(itemRectangle, SelectedItemColor, listRectangle);

                renderer.DrawText(itemTextInfo.Font, itemTextInfo.Text, itemTextInfo.Position + TextOffset, textColor, itemTextInfo.ClippingRectangle);
            }
        }
    }
}