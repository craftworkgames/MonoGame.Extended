﻿using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiCheckBox : GuiControl
    {
        public GuiCheckBox()
        {
        }

        public GuiCheckBox(GuiSkin skin) 
            : base(skin)
        {
        }

        public event EventHandler CheckStateChanged;

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    CheckedStyle?.ApplyIf(this, _isChecked);
                    CheckStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private GuiControlStyle _checkedStyle;
        public GuiControlStyle CheckedStyle
        {
            get { return _checkedStyle; }
            set
            {
                if (_checkedStyle != value)
                {
                    _checkedStyle = value;
                    CheckedStyle?.ApplyIf(this, _isChecked);
                }
            }
        }

        public override bool OnPointerUp(IGuiContext context, GuiPointerEventArgs args)
        {
            if (IsFocused && BoundingRectangle.Contains(args.Position))
                IsChecked = !IsChecked;

            return base.OnPointerUp(context, args);
        }

        protected override void DrawBackground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            var boundingRectangle = BoundingRectangle;
            var checkRectangle = new Rectangle(boundingRectangle.X, boundingRectangle.Y, BackgroundRegion.Width, BackgroundRegion.Height);

            renderer.DrawRegion(BackgroundRegion, checkRectangle, Color);
        }

        protected override void DrawForeground(IGuiContext context, IGuiRenderer renderer, float deltaSeconds, TextInfo textInfo)
        {
            textInfo.Position = BoundingRectangle.Location.ToVector2() +
                    new Vector2(BackgroundRegion.Width + 5, BackgroundRegion.Height * 0.5f - textInfo.Font.LineHeight * 0.5f);

            base.DrawForeground(context, renderer, deltaSeconds, textInfo);
        }
    }
}
