using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    //public class Window : Element<Screen>
    //{
    //    public Window(Screen parent)
    //    {
    //        Parent = parent;
    //    }

    //    public ControlCollection Controls { get; } = new ControlCollection();

    //    public void Show()
    //    {
    //        Parent.Windows.Add(this);
    //    }

    //    public void Hide()
    //    {
    //        Parent.Windows.Remove(this);
    //    }

    //    public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
    //    {
    //        renderer.FillRectangle(BoundingRectangle, Color.Magenta);
    //    }

    //    public Size2 GetDesiredSize(IGuiContext context, Size2 availableSize)
    //    {
    //        return new Size2(Width, Height);
    //    }

    //    public void Layout(IGuiContext context, RectangleF rectangle)
    //    {
    //        foreach (var control in Controls)
    //            LayoutHelper.PlaceControl(context, control, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    //    }
    //}
}