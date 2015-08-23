namespace MonoGame.Extended.InputListeners
{
    public class MouseEventListenerSettings : EventListenerSettings<MouseEventListener>
    {
        public int DragThreshold { get; set; }
        public int DoubleClickMilliseconds { get; set; }

        internal override MouseEventListener CreateListener()
        {
            return new MouseEventListener(DoubleClickMilliseconds, DragThreshold);
        }
    }
}