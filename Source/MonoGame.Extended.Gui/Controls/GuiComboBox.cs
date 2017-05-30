using Microsoft.Xna.Framework;
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

        public TextureRegion2D DropDownRegion { get; set; }

        public bool IsOpen { get; set; }

        public override void OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerDown(context, args);

            IsOpen = !IsOpen;
        }

        protected override Rectangle GetContentRectangle(IGuiContext context)
        {
            return GetDropDownRectangle(context);
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

                if(itemSize.Height > height)
                    height = itemSize.Height;
            }

            return new Size2(width + ClipPadding.Size.Width, height + ClipPadding.Size.Height);
        }

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.DrawBackground(context, renderer, deltaSeconds);

            if (IsOpen)
            {
                var dropDownRectangle = GetDropDownRectangle(context);

                if (DropDownRegion != null)
                    renderer.DrawRegion(DropDownRegion, dropDownRectangle, Color.White);
                else
                    renderer.FillRectangle(dropDownRectangle, Color.White);
            }
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            var selectedTextInfo = GetItemTextInfo(context, ClippingRectangle, SelectedItem);
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