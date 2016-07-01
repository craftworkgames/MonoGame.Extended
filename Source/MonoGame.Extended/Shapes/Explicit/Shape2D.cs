//using Microsoft.Xna.Framework;
//
//namespace MonoGame.Extended.Shapes.Explicit
//{
//    public class Shape2D : Shape<ShapeVertex2D, Transform2D>
//    {
//        public Shape2D(ShapeVertex2D[] vertices)
//            : base(vertices)
//        {
//        }
//
//        public Vector2 Project(ref Vector2 axis)
//        {
//            var min = axis.Dot(Vertices[0]);
//            var max = min;
//
//            for (var i = 1; i < Vertices.Count; i++)
//            {
//                var vertex = Vertices[i];
//
//                var dotProduct = axis.Dot(vertex);
//                if (dotProduct < min)
//                {
//                    min = dotProduct;
//                }
//                else if (dotProduct > max)
//                {
//                    max = dotProduct;
//                }
//            }
//
//            return new Vector2(min, max);
//        }
//    }
//}
