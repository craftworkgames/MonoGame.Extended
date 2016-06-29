//using MonoGame.Extended.Shapes.Explicit;
//
//namespace MonoGame.Extended.Collisions
//{
//    public class CollisionFixture<TShapeVertexType, TTransform>
//        where TShapeVertexType : struct, IShapeVertexType<TShapeVertexType>
//        where TTransform : struct, ITransform
//    {
//        internal int ProxyIndex;
//
//        public CollisionBody<TTransform> Body { get; internal set; }
//        public Shape<TShapeVertexType, TTransform> Shape { get; internal set; }
//        public CollisionFixtureFlags Flags { get; internal set; }
//
//        internal CollisionFixture()
//        {
//            ResetState();
//        }
//
//        public void ResetState()
//        {
//            ProxyIndex = -1;
//            Body = null;
//            Shape = null;
//            Flags = CollisionFixtureFlags.None;
//        }
//    }
//}
