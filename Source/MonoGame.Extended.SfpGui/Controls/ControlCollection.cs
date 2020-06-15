namespace MonoGame.Extended.SfpGui.Controls {
    public class ControlCollection : ElementCollection<Control, Control> {
        public ControlCollection () : base (null) { }

        public ControlCollection (Control parent) : base (parent) { }
    }
}