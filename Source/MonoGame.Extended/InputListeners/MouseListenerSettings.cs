namespace MonoGame.Extended.InputListeners
{
    public class MouseListenerSettings : InputListenerSettings<MouseListener>
    {
        public int DoubleClickMilliseconds { get; set; }

        public int DragThreshold { get; set; }

        public MouseListenerSettings()
        {
            // initial values are windows defaults
            DoubleClickMilliseconds = 500;
            DragThreshold = 2;
        }

        internal override MouseListener CreateListener()
        {
            return new MouseListener(this);
        }
    }
}