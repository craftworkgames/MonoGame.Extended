using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input.InputListeners;

namespace MonoGame.Extended.Gui.Controls
{
    public class ComboBox : SelectorControl
    {
        public ComboBox()
        {
        }
        
        public bool IsOpen { get; set; }
        public Texture2DRegion DropDownRegion { get; set; }
        public Color DropDownColor { get; set; } = Color.White;

        public override bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Enter)
                IsOpen = false;

            return base.OnKeyPressed(context, args);
        }

        public override bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        {
            IsOpen = !IsOpen;
            return base.OnPointerUp(context, args);
        }

        protected override Rectangle GetListAreaRectangle(IGuiContext context)
        {
            return GetDropDownRectangle(context);
        }

        public override bool Contains(IGuiContext context, Point point)
        {
            return base.Contains(context, point) || IsOpen && GetListAreaRectangle(context).Contains(point);
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

                if (itemSize.Height > height)
                    height = itemSize.Height;
            }

            return new Size(width + ClipPadding.Width, height + ClipPadding.Height);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);

            if (IsOpen)
            {
                var dropDownRectangle = GetListAreaRectangle(context);

                if (DropDownRegion != null)
                {
                    renderer.DrawRegion(DropDownRegion, dropDownRectangle, DropDownColor);
                }
                else
                {
                    renderer.FillRectangle(dropDownRectangle, DropDownColor);
                    renderer.DrawRectangle(dropDownRectangle, BorderColor);
                }

                DrawItemList(context, renderer);
            }

            var selectedTextInfo = GetItemTextInfo(context, ContentRectangle, SelectedItem);

            if (!string.IsNullOrWhiteSpace(selectedTextInfo.Text))
                renderer.DrawText(selectedTextInfo.Font, selectedTextInfo.Text, selectedTextInfo.Position + TextOffset, selectedTextInfo.Color, selectedTextInfo.ClippingRectangle);
        }

        private Rectangle GetDropDownRectangle(IGuiContext context)
        {
            var dropDownRectangle = BoundingRectangle;

            dropDownRectangle.Y = dropDownRectangle.Y + dropDownRectangle.Height;
            dropDownRectangle.Height = (int) Items
                .Select(item => GetItemSize(context, item))
                .Select(itemSize => itemSize.Height)
                .Sum();

            return dropDownRectangle;
        }
    }
}