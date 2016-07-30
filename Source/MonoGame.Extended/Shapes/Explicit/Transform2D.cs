//using Microsoft.Xna.Framework;
//
//namespace MonoGame.Extended.Shapes.Explicit
//{
//    public struct Transform2D : ITransform
//    {
//        public Vector2 Position { get; set; }
//        public float Rotation { get; set; }
//        public Vector2 Scale { get; set; }
//
//        public void GetMatrix(out Matrix transformMatrix)
//        {
//            transformMatrix = Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(new Vector3(Scale, z: 1f)) * Matrix.CreateTranslation(new Vector3(Position, z: 0));
//        }
//    }
//}
