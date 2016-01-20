using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui.Layouts
{
    public abstract class GuiLayoutControl : GuiControl
    {
        protected GuiLayoutControl()
        {
            Children = new List<GuiControl>();
        }

        protected abstract void LayoutChildren(Rectangle bounds);

        public List<GuiControl> Children { get; private set; }
    }
}