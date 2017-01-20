using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class TransformComponent : EntityComponent, IBaseTransform<Matrix2D, TransformComponent>
    {
        private readonly Transform _transform;

        internal TransformComponent()
        {
            _transform = new Transform(this);
        }

        internal Entity Parent
        {
            get { return _transform.Parent.Component.Entity; }
            set { _transform.Parent = value.GetComponent<TransformComponent>()._transform; }
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

        Matrix2D IBaseTransform<Matrix2D, TransformComponent>.LocalMatrix => _transform.LocalMatrix;
        Matrix2D IBaseTransform<Matrix2D, TransformComponent>.WorldMatrix => _transform.WorldMatrix;

        TransformComponent IBaseTransform<Matrix2D, TransformComponent>.Parent
        {
            get { return _transform.Parent.Component; }
            set { _transform.Parent = value._transform; }
        }

        event Action IBaseTransform<Matrix2D, TransformComponent>.TranformUpdated
        {
            add { _transform.TranformUpdated += value; }
            remove { _transform.TranformUpdated -= value; }
        }

        event Action IBaseTransform<Matrix2D, TransformComponent>.TransformBecameDirty
        {
            add { _transform.TransformBecameDirty += value; }
            remove { _transform.TransformBecameDirty -= value; }
        }

        void IBaseTransform<Matrix2D, TransformComponent>.GetLocalMatrix(out Matrix2D matrix)
        {
            _transform.GetLocalMatrix(out matrix);
        }

        void IBaseTransform<Matrix2D, TransformComponent>.GetWorldMatrix(out Matrix2D matrix)
        {
            _transform.GetWorldMatrix(out matrix);
        }

        private class Transform : Transform2D<Transform>
        {
            public TransformComponent Component { get; }

            public Transform(TransformComponent component)
            {
                Component = component;

                Position = Vector2.Zero;
                Rotation = 0;
                Scale = Vector2.One;
            }
        }
    }
}