using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Drawables;
using MonoGame.Extended.InputListeners;

namespace MonoGame.Extended.Gui.Layouts
{
    public class GuiLayoutDrawable : IGuiDrawable
    {
        private readonly GuiLayoutControl _parent;

        public GuiLayoutDrawable(GuiLayoutControl parent)
        {
            _parent = parent;
        }

        public Size DesiredSize
        {
            get { return new Size(int.MaxValue, int.MaxValue); }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            foreach (var child in _parent.Children)
                child.Draw(spriteBatch, rectangle);
        }
    }

    public abstract class GuiLayoutControl : GuiControl
    {
        protected GuiLayoutControl()
        {
            Children = new List<GuiControl>();
        }

        public List<GuiControl> Children { get; private set; }

        protected override IGuiDrawable GetCurrentDrawable()
        {
            return new GuiLayoutDrawable(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var child in Children)
                child.Update(gameTime);
        }
        
        private void ForEachChildAtPoint(Point point, Action<GuiControl> action)
        {
            foreach (var control in Children.Where(c => c.Contains(point)))
                action(control);
        }

        public override void OnMouseMoved(object sender, MouseEventArgs args)
        {
            base.OnMouseMoved(sender, args);
            ForEachChildAtPoint(args.Position, c => c.OnMouseMoved(this, args));
        }

        public override void OnMouseDown(object sender, MouseEventArgs args)
        {
            base.OnMouseDown(sender, args);
            ForEachChildAtPoint(args.Position, c => c.OnMouseDown(this, args));
        }

        public override void OnMouseUp(object sender, MouseEventArgs args)
        {
            base.OnMouseUp(sender, args);
            ForEachChildAtPoint(args.Position, c => c.OnMouseUp(this, args));
        }

        protected int GetHorizontalAlignment(GuiControl control, Rectangle rectangle)
        {
            switch (control.HorizontalAlignment)
            {
                case GuiHorizontalAlignment.Stretch:
                case GuiHorizontalAlignment.Left:
                    return rectangle.Left;
                case GuiHorizontalAlignment.Right:
                    return rectangle.Right - control.Width;
                case GuiHorizontalAlignment.Center:
                    return rectangle.Left + rectangle.Width / 2 - control.Width / 2;
            }

            throw new NotSupportedException(string.Format("{0} is not supported", control.HorizontalAlignment));
        }

        protected int GetVerticalAlignment(GuiControl control, Rectangle rectangle)
        {
            switch (control.VerticalAlignment)
            {
                case GuiVerticalAlignment.Stretch:
                case GuiVerticalAlignment.Top:
                    return rectangle.Top;
                case GuiVerticalAlignment.Bottom:
                    return rectangle.Bottom - control.Height;
                case GuiVerticalAlignment.Center:
                    return rectangle.Top + rectangle.Height / 2 - control.Height / 2;
            }

            throw new NotSupportedException(string.Format("{0} is not supported", control.VerticalAlignment));
        }
    }
}