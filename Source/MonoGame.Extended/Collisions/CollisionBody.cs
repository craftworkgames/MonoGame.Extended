//using MonoGame.Extended.Shapes.Explicit;
//
//namespace MonoGame.Extended.Collisions
//{
//    public abstract class CollisionBody<TTransform> where TTransform : struct, ITransform
//    {
//        internal CollisionSimulation CollisionSimulation;
//        internal int Index;
//
//        // NOT a property because structs members can't be set directory via a property, e.g. for 2D: Transform.Position = new Vector2(x, y);
//        public TTransform Transform;
//
//        public CollisionBodyFlags Flags { get; internal set; }
//
//        protected CollisionBody()
//        {
//            ResetState();
//        }
//
//        internal void ResetState()
//        {
//            CollisionSimulation = null;
//            Flags = CollisionBodyFlags.None;
//        }
//    }
//}
