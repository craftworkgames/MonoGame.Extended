using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiComboBox : GuiItemsControl
    {
        public GuiComboBox()
            : this(null)
        {
        }

        public GuiComboBox(GuiSkin skin)
            : base(skin)
        {
        }

        public bool IsOpen { get; set; }
        public TextureRegion2D DropDownRegion { get; set; }
        public Color DropDownColor { get; set; } = Color.White;

        public override bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Enter)
                IsOpen = false;

            return base.OnKeyPressed(context, args);
        }

        public override bool OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            IsOpen = !IsOpen;
            return base.OnPointerDown(context, args);
        }

        protected override Rectangle GetContentRectangle(IGuiContext context)
        {
            return GetDropDownRectangle(context);
        }

        public override bool Contains(IGuiContext context, Point point)
        {
            return base.Contains(context, point) || IsOpen && GetContentRectangle(context).Contains(point);
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

                if (itemSize.Height > height)
                    height = itemSize.Height;
            }

            return new Size2(width + ClipPadding.Size.Width, height + ClipPadding.Size.Height);
        }

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.DrawBackground(context, renderer, deltaSeconds);

            if (IsOpen)
            {
                var dropDownRectangle = GetContentRectangle(context);

                if (DropDownRegion != null)
                    renderer.DrawRegion(DropDownRegion, dropDownRectangle, DropDownColor);
                else
                    renderer.FillRectangle(dropDownRectangle, DropDownColor);
            }
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            var selectedTextInfo = GetItemTextInfo(context, ClippingRectangle, SelectedItem, ClippingRectangle);
            base.DrawForeground(context, renderer, deltaSeconds, selectedTextInfo);

            if (IsOpen)
                DrawItemList(context, renderer);
        }

        private Rectangle GetDropDownRectangle(IGuiContext context)
        {
            var dropDownRectangle = BoundingRectangle;
            dropDownRectangle.Y = dropDownRectangle.Y + dropDownRectangle.Height;
            var height = 0f;

            foreach (var item in Items)
            {
                var itemSize = GetItemSize(context, new Size2(BoundingRectangle.Width, 100), item);
                height += itemSize.Height;
            }

            dropDownRectangle.Height = (int)height;
            return dropDownRectangle;
        }
    }
}