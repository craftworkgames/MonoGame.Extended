//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended.Shapes.BoundingVolumes;
//
//namespace MonoGame.Extended.Shapes.Explicit
//{
//    public abstract class Shape<TShapeVertex, TTransform>
//        where TShapeVertex : struct, IShapeVertexType<TShapeVertex> where TTransform : struct, ITransform
//    {
//        private TShapeVertex[] _vertices;
//        private TShapeVertex[] _transformedVertices;
//        protected TTransform Transform { get; }
//
//        public IReadOnlyList<TShapeVertex> Vertices
//        {
//            get { return _vertices; }
//        }
//
//        public IReadOnlyList<TShapeVertex> TransformedVertices
//        {
//            get { return _transformedVertices; }
//        }
//
//        protected Shape(TShapeVertex[] vertices)
//        {
//            Transform = new TTransform();
//            SetVertices(vertices);
//        }
//
//        public void SetVertices(TShapeVertex[] vertices)
//        {
//            _vertices = vertices;
//            _transformedVertices = new TShapeVertex[vertices.Length];
//            ApplyTransformMatrix();
//        }
//
//        public void ApplyTransformMatrix()
//        {
//            Matrix transformMatrix;
//            Transform.GetMatrix(out transformMatrix);
//
//            for (var i = 0; i < _vertices.Length; i++)
//            {
//                _vertices[i].Transform(ref transformMatrix, out _transformedVertices[i]);
//            }
//        }
//
//        public void GetBoundingVolume<TBoundingVolume>(ref TBoundingVolume boundingVolume) where TBoundingVolume : struct, IBoundingVolume<TShapeVertex, TBoundingVolume>
//        {
//            boundingVolume.UpdateFrom(_vertices);
//        }
//    }
//}
