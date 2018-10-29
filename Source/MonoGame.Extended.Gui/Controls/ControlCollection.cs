using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Extended.Gui.Controls
{
    public class ControlCollection : ElementCollection<Control, Control>
    {
        public ControlCollection()
            : base(null)
        {
        }

        public ControlCollection(Control parent)
            : base(parent)
        {
        }
        public ControlCollection(IEnumerable<Control> controls)
            : base(null)
        {
            foreach (Control control in controls)
                Add(control);
        }

        internal IEnumerable<Control> GetList()
        {
            return this.ToList();
        }
    }
}