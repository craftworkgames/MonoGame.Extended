namespace MonoGame.Extended.Collisions
{
    public class CollisionFixture
    {
        internal int ProxyIndex;

        public CollisionBody Body { get; internal set; }
        public CollisionShape2D Shape { get; internal set; }
        public CollisionFixtureFlags Flags { get; internal set; }

        internal CollisionFixture()
        {
            ResetState();
        }

        public void ResetState()
        {
            Flags = CollisionFixtureFlags.None;
            ProxyIndex = -1;
            Body = null;
            Shape = null;
        }
    }
}
