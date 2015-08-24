namespace MonoGame.Extended.InputListeners
{
    public class MouseListenerSettings : InputListenerSettings<MouseListener>
    {
        public MouseListenerSettings()
        {
            // initial values are windows defaults
            DoubleClickMilliseconds = 500;
            DragThreshold = 2;
        }

        public int DragThreshold { get; set; }
        public int DoubleClickMilliseconds { get; set; }

        internal override MouseListener CreateListener()
        {
            return new MouseListener(this);
        }
    }
}