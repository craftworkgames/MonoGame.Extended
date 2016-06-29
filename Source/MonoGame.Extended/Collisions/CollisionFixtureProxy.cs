//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using MonoGame.Extended.Shapes.BoundingVolumes;
//using MonoGame.Extended.Shapes.Explicit;
//
//namespace MonoGame.Extended.Collisions
//{
//    [StructLayout(LayoutKind.Sequential, Pack = 1)]
//    public struct CollisionFixtureProxy<TShapeVertexType, TTransform>
//        where TShapeVertexType : struct, IShapeVertexType<TShapeVertexType>
//        where TTransform : struct, ITransform
//    {
//        public AxisAlignedBoundingBox2D BoundingVolume;
//        public readonly CollisionFixture<TShapeVertexType, TTransform> Fixture;
//        public readonly int BodyHashCode;
//
//        internal CollisionFixtureProxy(CollisionFixture<TShapeVertexType, TTransform> fixture)
//        { 
//            Fixture = fixture;
//            BoundingVolume = default(AxisAlignedBoundingBox2D);
//            BodyHashCode = RuntimeHelpers.GetHashCode(fixture.Body);
//        }
//    }
//}
