using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class Transform2D : EntityComponent, IBaseTransform<Matrix2D, Transform2D>
    {
        private readonly Transform _transform;

        internal Transform2D()
        {
            _transform = new Transform(this);
        }

        internal Entity Parent
        {
            get { return _transform.Parent.Component.Entity; }
            set { _transform.Parent = value.GetComponent<Transform2D>()._transform; }
        }

        public Vector2 Position
        {
            get { return _transform.Position; }
            set { _transform.Position = value; }
        }

        public float Rotation
        {
            get { return _transform.Rotation; }
            set { _transform.Rotation = value; }
        }

        public Vector2 Scale
        {
            get { return _transform.Scale; }
            set { _transform.Scale = value; }
        }

        public Vector2 WorldPosition => _transform.WorldPosition;
        public float WorldRotation => _transform.WorldRotation;
        public Vector2 WorldScale => _transform.WorldScale;

        Matrix2D IBaseTransform<Matrix2D, Transform2D>.LocalMatrix => _transform.LocalMatrix;
        Matrix2D IBaseTransform<Matrix2D, Transform2D>.WorldMatrix => _transform.WorldMatrix;

        Transform2D IBaseTransform<Matrix2D, Transform2D>.Parent
        {
            get { return _transform.Parent.Component; }
            set { _transform.Parent = value._transform; }
        }

        event Action IBaseTransform<Matrix2D, Transform2D>.TranformUpdated
        {
            add { _transform.TranformUpdated += value; }
            remove { _transform.TranformUpdated -= value; }
        }

        event Action IBaseTransform<Matrix2D, Transform2D>.TransformBecameDirty
        {
            add { _transform.TransformBecameDirty += value; }
            remove { _transform.TransformBecameDirty -= value; }
        }

        void IBaseTransform<Matrix2D, Transform2D>.GetLocalMatrix(out Matrix2D matrix)
        {
            _transform.GetLocalMatrix(out matrix);
        }

        void IBaseTransform<Matrix2D, Transform2D>.GetWorldMatrix(out Matrix2D matrix)
        {
            _transform.GetWorldMatrix(out matrix);
        }

        private class Transform : Transform2D<Transform>
        {
            public Transform2D Component { get; }

            public Transform(Transform2D component)
            {
                Component = component;

                Position = Vector2.Zero;
                Rotation = 0;
                Scale = Vector2.One;
            }
        }
    }
}