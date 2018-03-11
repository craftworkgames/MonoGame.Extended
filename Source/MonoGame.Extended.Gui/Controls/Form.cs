using System.Linq;
using MonoGame.Extended.Input.InputListeners;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Gui.Controls
{
    //public class Form : StackPanel
    //{
    //    public Form()
    //    {
    //    }

    //    public override bool OnKeyPressed(IGuiContext context, KeyboardEventArgs args)
    //    {
    //        if (args.Key == Keys.Tab)
    //        {
    //            var controls = FindControls<Control>();
    //            var index = controls.IndexOf(context.FocusedControl);
    //            if (index > -1)
    //            {
    //                index++;
    //                if (index >= controls.Count) index = 0;
    //                context.SetFocus(controls[index]);
    //                return true;
    //            }
    //        }

    //        if (args.Key == Keys.Enter)
    //        {
    //            var controls = FindControls<Submit>();
    //            if (controls.Count > 0)
    //            {
    //                var submit = controls.FirstOrDefault();
    //                submit.TriggerClicked();
    //                return true;
    //            }
    //        }

    //        return base.OnKeyPressed(context, args);
    //    }
    //}
}
