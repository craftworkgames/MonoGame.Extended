using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiComboBoxList : GuiListBox
    {
        private readonly GuiComboBox _parent;

        public GuiComboBoxList(GuiComboBox parent)
            : base(null)
        {
            _parent = parent;
            TextColor = Color.Black;
        }

        protected override void OnItemClicked(IGuiContext context, GuiPointerEventArgs args)
        {
            _parent.IsOpen = false;
        }

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            if (BackgroundRegion != null)
                renderer.DrawRegion(BackgroundRegion, BoundingRectangle, Color);
            else
                renderer.FillRectangle(BoundingRectangle, Color);        }
        }

        public class GuiComboBox : GuiItemsControl
    {
        private readonly GuiComboBoxList _dropDown;

        public GuiComboBox()
            : this(null)
        {
        }

        public GuiComboBox(GuiSkin skin)
            : base(null)
        {
            // we must initialize the dropdown before applying the skin
            _dropDown = new GuiComboBoxList(this);
            skin?.GetStyle(GetType())?.Apply(this);
            skin?.GetStyle(_dropDown.GetType())?.Apply(_dropDown);
        }

        public override List<object> Items => _dropDown.Items;

        public TextureRegion2D DropDownRegion
        {
            get { return _dropDown.BackgroundRegion; }
            set { _dropDown.BackgroundRegion = value; }
        }

        public override Thickness ItemPadding
        {
            get { return _dropDown.ItemPadding; }
            set { _dropDown.ItemPadding = value; }
        }

        public override int SelectedIndex
        {
            get { return _dropDown.SelectedIndex; }
            set { _dropDown.SelectedIndex = value; }
        }

        public override Color SelectedItemColor
        {
            get { return _dropDown.SelectedItemColor; }
            set { _dropDown.SelectedItemColor = value; }
        }

        public override Color SelectedTextColor
        {
            get { return _dropDown.SelectedTextColor; }
            set { _dropDown.SelectedTextColor = value; }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    var boundingRectangle = BoundingRectangle;

                    if (_isOpen)
                    {
                        _dropDown.Position = new Vector2(0, boundingRectangle.Height);
                        _dropDown.Width = BoundingRectangle.Width;
                        _dropDown.Height = 100;
                        Controls.Add(_dropDown);
                    }
                    else
                    {
                        Controls.Remove(_dropDown);
                    }
                }
            }
        }

        public override void OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
        {
            _dropDown.OnKeyPressed(context, args);

            if (args.Key == Keys.Enter)
                IsOpen = false;
        }

        public override void OnScrolled(int delta)
        {
            _dropDown.OnScrolled(delta);
        }

        public override void OnPointerDown(IGuiContext context, GuiPointerEventArgs args)
        {
            base.OnPointerDown(context, args);

            IsOpen = !IsOpen;
        }

        protected override Rectangle GetContentRectangle(IGuiContext context)
        {
            return _dropDown.BoundingRectangle;// GetDropDownRectangle(context);
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

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            var selectedTextInfo = GetItemTextInfo(context, ClippingRectangle, SelectedItem, ClippingRectangle);
            base.DrawForeground(context, renderer, deltaSeconds, selectedTextInfo);
        }

        //private Rectangle GetDropDownRectangle(IGuiContext context)
        //{
        //    var dropDownRectangle = BoundingRectangle;
        //    dropDownRectangle.Y = dropDownRectangle.Y + dropDownRectangle.Height;
        //    var height = 0f;

        //    foreach (var item in Items)
        //    {
        //        var itemSize = GetItemSize(context, new Size2(BoundingRectangle.Width, 100), item);
        //        height += itemSize.Height;
        //    }

        //    dropDownRectangle.Height = (int)height;
        //    return dropDownRectangle;
        //}
    }
}