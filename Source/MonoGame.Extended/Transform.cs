using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    [Flags]
    internal enum TransformFlags : byte
    {
        WorldMatrixIsDirty = 1 << 0,
        LocalMatrixIsDirty = 1 << 1,
        All = WorldMatrixIsDirty | LocalMatrixIsDirty
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseTransform<TTransform> where TTransform : BaseTransform<TTransform>
    {
        private TransformFlags _flags = TransformFlags.All;
        private Matrix _worldMatrix;
        private Matrix _localMatrix;
        private TTransform _parentTransform;

        private event Action WorldMatrixIsDirty;
        public event Action WorldMatrixUpdated;

        public Matrix LocalMatrix
        {
            get
            {
                RecalculateLocalMatrixIfNecessary();
                return _localMatrix;
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                RecalculateWorldMatrixIfNecessary();
                return _worldMatrix;
            }
        }

        public TTransform ParentTransform
        {
            get { return _parentTransform; }
            set
            {
                if (_parentTransform == value)
                {
                    return;
                }

                var oldParentTransform = ParentTransform;
                _parentTransform = value;
                OnParentChanged(oldParentTransform, value);
            }
        }

        internal BaseTransform()
        {
        }

        public void GetLocalMatrix(out Matrix matrix)
        {
            RecalculateLocalMatrixIfNecessary();
            matrix = _localMatrix;
        }

        public void GetWorldMatrix(out Matrix matrix)
        {
            RecalculateWorldMatrixIfNecessary();
            matrix = _worldMatrix;
        }

        protected void LocalMatrixBecameDirty()
        {
            _flags |= TransformFlags.LocalMatrixIsDirty;
        }

        protected void WorldMatrixBecameDirty()
        {
            _flags |= TransformFlags.WorldMatrixIsDirty;
            WorldMatrixIsDirty?.Invoke();
        }

        private void OnParentChanged(TTransform oldParentTranform, TTransform newParentTransform)
        {
            var parentTransform = oldParentTranform;
            while (parentTransform != null)
            {
                parentTransform.WorldMatrixIsDirty -= ParentTransformOnWorldMatrixIsDirty;
                parentTransform = parentTransform.ParentTransform;
            }

            parentTransform = newParentTransform;
            while (parentTransform != null)
            {
                parentTransform.WorldMatrixIsDirty += ParentTransformOnWorldMatrixIsDirty;
                parentTransform = parentTransform.ParentTransform;
            }
        }

        private void ParentTransformOnWorldMatrixIsDirty()
        {
            _flags |= TransformFlags.WorldMatrixIsDirty;
        }

        private void RecalculateWorldMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.WorldMatrixIsDirty) == 0)
            {
                return;
            }

            RecalculateWorldMatrix(out _worldMatrix);
            _flags &= ~TransformFlags.WorldMatrixIsDirty;
            WorldMatrixUpdated?.Invoke();
        }

        protected abstract void RecalculateWorldMatrix(out Matrix matrix);

        private void RecalculateLocalMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.LocalMatrixIsDirty) == 0)
            {
                return;
            }

            RecalculateLocalMatrix(out _localMatrix);

            _flags &= ~TransformFlags.LocalMatrixIsDirty;
            WorldMatrixBecameDirty();
        }

        protected abstract void RecalculateLocalMatrix(out Matrix matrix);
    }

    public class Transform2D : BaseTransform<Transform2D>
    {
        private Vector2 _position;
        private Vector2 _scale = Vector2.One;
        private float _rotationAngle;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        public float RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        protected override void RecalculateWorldMatrix(out Matrix matrix)
        {
            matrix = ParentTransform != null ? ParentTransform.WorldMatrix * LocalMatrix : LocalMatrix;
        }

        protected override void RecalculateLocalMatrix(out Matrix matrix)
        {
            var scale = new Vector3(_scale, 1);
            var translation = new Vector3(_position, 0);

            if (ParentTransform != null)
            {
                var parentPosition = new Vector3(ParentTransform.Position, 0);
                matrix = Matrix.CreateTranslation(-parentPosition) * Matrix.CreateScale(scale) * Matrix.CreateRotationZ(_rotationAngle) * Matrix.CreateTranslation(parentPosition) * Matrix.CreateTranslation(translation);
            }
            else
            {
                matrix = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(_rotationAngle) * Matrix.CreateTranslation(translation);
            }
        }
    }

    public class Transform3D : BaseTransform<Transform3D>
    {
        private Vector3 _position;
        private Vector3 _scale = Vector3.One;
        private float _rotationAngle;
        private Vector3 _rotationAxis = Vector3.Up;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        public float RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        public Vector3 RotationAxis
        {
            get { return _rotationAxis; }
            set
            {
                _rotationAxis = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        protected override void RecalculateWorldMatrix(out Matrix matrix)
        {
            matrix = ParentTransform != null ? ParentTransform.WorldMatrix * LocalMatrix : LocalMatrix;
        }

        protected override void RecalculateLocalMatrix(out Matrix matrix)
        {
            if (ParentTransform != null)
            {
                var parentPosition = ParentTransform.Position;
                matrix = Matrix.CreateTranslation(-parentPosition) * Matrix.CreateScale(_scale) * Matrix.CreateFromAxisAngle(_rotationAxis, _rotationAngle) * Matrix.CreateTranslation(parentPosition) * Matrix.CreateTranslation(new Vector3(_position.X, _position.Y, 0));
            }
            else
            {
                matrix = Matrix.CreateScale(_scale) * Matrix.CreateFromAxisAngle(_rotationAxis, _rotationAngle) * Matrix.CreateTranslation(new Vector3(_position.X, _position.Y, 0));
            }
        }
    }
}
