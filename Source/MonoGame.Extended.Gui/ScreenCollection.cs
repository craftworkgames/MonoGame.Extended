using System;
using System.Linq;

namespace MonoGame.Extended.Gui
{
    public class ScreenCollection : ElementCollection<Screen, GuiSystem>
    {
        private GuiSystem _parent;
        public ScreenCollection(GuiSystem parent)
            : base(parent)
        {
            _parent = parent;
        }

        public Screen GetScreen(string name)
        {
            foreach (Screen screen in this.ToList())
            {
                if (screen.Name == name)
                    return screen;
            }
            return null;
        }
    }
}