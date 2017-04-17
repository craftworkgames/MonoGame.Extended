using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities
{
    [EntityComponent]
    public class TransformComponent2D : EntityComponent
    {
        private TransformFlags _flags = TransformFlags.All; // dirty flags, set all dirty flags when created
        private Matrix2D _localMatrix; // model space to local space
        private TransformComponent2D _parent; // parent
        private Matrix2D _worldMatrix; // local space to world space
        private Vector2 _position;
        private float _rotation;
        private Vector2 _scale = Vector2.One;

        public Vector2 WorldPosition => WorldMatrix.Translation;
        public Vector2 WorldScale => WorldMatrix.Scale;
        public float WorldRotation => WorldMatrix.Rotation;

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

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
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

        public Matrix2D LocalMatrix
        {
            get
            {
                RecalculateLocalMatrixIfNecessary();
                return _localMatrix;
            }
        }

        public Matrix2D WorldMatrix
        {
            get
            {
                RecalculateWorldMatrixIfNecessary();
                return _worldMatrix;
            }
        }

        public TransformComponent2D Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                    return;

                var oldParentTransform = Parent;
                _parent = value;
                OnParentChanged(oldParentTransform, value);
            }
        }

        internal event Action TransformBecameDirty;

        public override void Reset()
        {
            Parent = null;
            _flags = TransformFlags.All;
            _localMatrix = Matrix2D.Identity;
            _worldMatrix = Matrix2D.Identity;
            _position = Vector2.Zero;
            _rotation = 0;
            _scale = Vector2.One;
        }

        public void GetLocalMatrix(out Matrix2D matrix)
        {
            RecalculateLocalMatrixIfNecessary();
            matrix = _localMatrix;
        }

        public void GetWorldMatrix(out Matrix2D matrix)
        {
            RecalculateWorldMatrixIfNecessary();
            matrix = _worldMatrix;
        }

        protected internal void LocalMatrixBecameDirty()
        {
            _flags |= TransformFlags.LocalMatrixIsDirty;
        }

        protected internal void WorldMatrixBecameDirty()
        {
            _flags |= TransformFlags.WorldMatrixIsDirty;
            TransformBecameDirty?.Invoke();
        }

        private void OnParentChanged(TransformComponent2D oldParent, TransformComponent2D newParent)
        {
            var parent = oldParent;
            while (parent != null)
            {
                parent.TransformBecameDirty -= ParentOnTransformBecameDirty;
                parent = parent.Parent;
            }

            parent = newParent;
            while (parent != null)
            {
                parent.TransformBecameDirty += ParentOnTransformBecameDirty;
                parent = parent.Parent;
            }
        }

        private void ParentOnTransformBecameDirty()
        {
            _flags |= TransformFlags.All;
        }

        private void RecalculateWorldMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.WorldMatrixIsDirty) == 0)
                return;

            RecalculateLocalMatrixIfNecessary();
            RecalculateWorldMatrix(ref _localMatrix, out _worldMatrix);
            _flags &= ~TransformFlags.WorldMatrixIsDirty;
        }

        private void RecalculateLocalMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.LocalMatrixIsDirty) == 0)
                return;

            RecalculateLocalMatrix(out _localMatrix);

            _flags &= ~TransformFlags.LocalMatrixIsDirty;
            WorldMatrixBecameDirty();
        }

        private void RecalculateWorldMatrix(ref Matrix2D localMatrix, out Matrix2D matrix)
        {
            if (Parent != null)
            {
                Parent.GetWorldMatrix(out matrix);
                Matrix2D.Multiply(ref matrix, ref localMatrix, out matrix);
            }
            else
            {
                matrix = localMatrix;
            }
        }

        private void RecalculateLocalMatrix(out Matrix2D matrix)
        {
            if (Parent != null)
            {
                var parentPosition = Parent.Position;
                matrix = Matrix2D.CreateTranslation(-parentPosition) * Matrix2D.CreateScale(_scale) *
                         Matrix2D.CreateRotationZ(-_rotation) * Matrix2D.CreateTranslation(parentPosition) *
                         Matrix2D.CreateTranslation(_position);
            }
            else
            {
                matrix = Matrix2D.CreateScale(_scale) * Matrix2D.CreateRotationZ(-_rotation) *
                         Matrix2D.CreateTranslation(_position);
            }
        }

        [Flags]
        private enum TransformFlags : byte
        {
            WorldMatrixIsDirty = 1 << 0,
            LocalMatrixIsDirty = 1 << 1,
            All = WorldMatrixIsDirty | LocalMatrixIsDirty
        }
    }
}
