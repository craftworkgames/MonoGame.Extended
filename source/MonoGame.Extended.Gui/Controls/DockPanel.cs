using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public enum Dock
    {
        Left, Right, Top, Bottom
    }

    public class DockPanel : LayoutControl
    {
        public override Size GetContentSize(IGuiContext context)
        {
            var size = new Size();

            for (var i = 0; i < Items.Count; i++)
            {
                var control = Items[i];
                var actualSize = control.CalculateActualSize(context);

                if (LastChildFill && i == Items.Count - 1)
                {
                    size.Width += actualSize.Width;
                    size.Height += actualSize.Height;
                }
                else
                {
                    var dock = control.GetAttachedProperty(DockProperty) as Dock? ?? Dock.Left;

                    switch (dock)
                    {
                        case Dock.Left:
                        case Dock.Right:
                            size.Width += actualSize.Width;
                            break;
                        case Dock.Top:
                        case Dock.Bottom:
                            size.Height += actualSize.Height;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return size;
        }

        protected override void Layout(IGuiContext context, Rectangle rectangle)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var control = Items[i];

                if (LastChildFill && i == Items.Count - 1)
                {
                    PlaceControl(context, control, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                }
                else
                {
                    var actualSize = control.CalculateActualSize(context);
                    var dock = control.GetAttachedProperty(DockProperty) as Dock? ?? Dock.Left;

                    switch (dock)
                    {
                        case Dock.Left:
                            PlaceControl(context, control, rectangle.Left, rectangle.Top, actualSize.Width, rectangle.Height);
                            rectangle.X += actualSize.Width;
                            rectangle.Width -= actualSize.Width;
                            break;
                        case Dock.Right:
                            PlaceControl(context, control, rectangle.Right - actualSize.Width, rectangle.Top, actualSize.Width, rectangle.Height);
                            rectangle.Width -= actualSize.Width;
                            break;
                        case Dock.Top:
                            PlaceControl(context, control, rectangle.Left, rectangle.Top, rectangle.Width, actualSize.Height);
                            rectangle.Y += actualSize.Height;
                            rectangle.Height -= actualSize.Height;
                            break;
                        case Dock.Bottom:
                            PlaceControl(context, control, rectangle.Left, rectangle.Bottom - actualSize.Height, rectangle.Width, actualSize.Height);
                            rectangle.Height -= actualSize.Height;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public const string DockProperty = "Dock";
        
        public override Type GetAttachedPropertyType(string propertyName)
        {
            if (string.Equals(DockProperty, propertyName, StringComparison.OrdinalIgnoreCase))
                return typeof(Dock);

            return base.GetAttachedPropertyType(propertyName);
        }

        public bool LastChildFill { get; set; } = true;
    }
}