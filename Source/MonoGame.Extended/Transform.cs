using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Code derived from top answer: http://gamedev.stackexchange.com/questions/113977/should-i-store-local-forward-right-up-vector-or-calculate-when-necessary

    [Flags]
    internal enum TransformFlags : byte
    {
        WorldMatrixIsDirty = 1 << 0,
        LocalMatrixIsDirty = 1 << 1,
        All = WorldMatrixIsDirty | LocalMatrixIsDirty
    }

    /// <summary>
    ///     Represents the base class for the position, rotation, and scale of a game object in two-dimensions or
    ///     three-dimensions.
    /// </summary>
    /// <typeparam name="TMatrix">The type of the matrix.</typeparam>
    /// <typeparam name="TParentTransform">The type of the parent transform.</typeparam>
    /// <remarks>
    ///     <para>
    ///         Every game object has a transform which is used to store and manipulate the position, rotation and scale
    ///         of the object. Every transform can have a parent, which allows to apply position, rotation and scale to game
    ///         objects hierarchically.
    ///     </para>
    ///     <para>
    ///         This class shouldn't be used directly. Instead use either of the derived classes; <see cref="Transform2D" /> or
    ///         Transform3D.
    ///     </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseTransform<TMatrix, TParentTransform>
        where TMatrix : struct where TParentTransform : BaseTransform<TMatrix, TParentTransform>
    {
        private TransformFlags _flags = TransformFlags.All; // dirty flags, set all dirty flags when created
        private TMatrix _localMatrix; // model space to local space
        private TMatrix _worldMatrix; // local space to world space
        private TParentTransform _parentTransform; // parent

        public event Action TransformBecameDirty; // observer pattern for when the world (or local) matrix became dirty
        public event Action TranformUpdated; // observer pattern for after the world (or local) matrix was re-calculated

        /// <summary>
        ///     Gets the model-to-local space <see cref="Matrix2D" />.
        /// </summary>
        /// <value>
        ///     The model-to-local space <see cref="Matrix2D" />.
        /// </value>
        public TMatrix LocalMatrix
        {
            get
            {
                RecalculateLocalMatrixIfNecessary(); // attempt to update local matrix upon request if it is dirty
                return _localMatrix;
            }
        }

        /// <summary>
        ///     Gets the local-to-world space <see cref="Matrix2D" />.
        /// </summary>
        /// <value>
        ///     The local-to-world space <see cref="Matrix2D" />.
        /// </value>
        public TMatrix WorldMatrix
        {
            get
            {
                RecalculateWorldMatrixIfNecessary(); // attempt to update world matrix upon request if it is dirty
                return _worldMatrix;
            }
        }

        /// <summary>
        ///     Gets or sets the parent transform.
        /// </summary>
        /// <value>
        ///     The parent transform.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Setting <see cref="ParentTransform" /> to a non-null instance enables this instance to
        ///         inherit the position, rotation, and scale of the parent instance. Setting <see cref="ParentTransform" /> to
        ///         <code>null</code> disables the inheritance altogether for this instance.
        ///     </para>
        /// </remarks>
        public TParentTransform ParentTransform
        {
            get { return _parentTransform; }
            set
            {
                if (_parentTransform == value)
                    return;

                var oldParentTransform = ParentTransform;
                _parentTransform = value;
                OnParentChanged(oldParentTransform, value);
            }
        }

        // internal contructor because people should not be using this class directly; they should use Transform2D or Transform3D
        internal BaseTransform()
        {
        }

        /// <summary>
        ///     Gets model-to-local space <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The model-to-local space <see cref="Matrix2D" />.</param>
        public void GetLocalMatrix(out TMatrix matrix)
        {
            RecalculateLocalMatrixIfNecessary();
            matrix = _localMatrix;
        }

        /// <summary>
        ///     Gets local-to-world space <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The local-to-world space <see cref="Matrix2D" />.</param>
        public void GetWorldMatrix(out TMatrix matrix)
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
            TransformBecameDirty?.Invoke();
        }

        private void OnParentChanged(TParentTransform oldParent, TParentTransform newParent)
        {
            var parent = oldParent;
            while (parent != null)
            {
                parent.TransformBecameDirty -= ParentTransformOnTransformBecameDirty;
                parent = parent.ParentTransform;
            }

            parent = newParent;
            while (parent != null)
            {
                parent.TransformBecameDirty += ParentTransformOnTransformBecameDirty;
                parent = parent.ParentTransform;
            }
        }

        private void ParentTransformOnTransformBecameDirty()
        {
            _flags |= TransformFlags.WorldMatrixIsDirty;
        }

        private void RecalculateWorldMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.WorldMatrixIsDirty) == 0)
                return;

            RecalculateLocalMatrixIfNecessary();
            RecalculateWorldMatrix(ref _localMatrix, out _worldMatrix);
            _flags &= ~TransformFlags.WorldMatrixIsDirty;
            TranformUpdated?.Invoke();
        }

        protected abstract void RecalculateWorldMatrix(ref TMatrix localMatrix, out TMatrix matrix);

        private void RecalculateLocalMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.LocalMatrixIsDirty) == 0)
                return;

            RecalculateLocalMatrix(out _localMatrix);

            _flags &= ~TransformFlags.LocalMatrixIsDirty;
            WorldMatrixBecameDirty();
        }

        protected abstract void RecalculateLocalMatrix(out TMatrix matrix);
    }

    /// <summary>
    ///     Represents the position, rotation, and scale of a two-dimensional game object.
    /// </summary>
    /// <seealso cref="Extended.BaseTransform{Matrix2D, Transform2D}" />
    /// <remarks>
    ///     <para>
    ///         Every game object has a transform which is used to store and manipulate the position, rotation and scale
    ///         of the object. Every transform can have a parent, which allows to apply position, rotation and scale to game
    ///         objects hierarchically.
    ///     </para>
    /// </remarks>
    public class Transform2D : BaseTransform<Matrix2D, Transform2D>
    {
        private Vector2 _position;
        private Vector2 _scale = Vector2.One;
        private float _rotation;

        /// <summary>
        ///     Gets or sets the local position.
        /// </summary>
        /// <value>
        ///     The local position.
        /// </value>
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

        /// <summary>
        ///     Gets or sets the local scale.
        /// </summary>
        /// <value>
        ///     The local scale.
        /// </value>
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

        /// <summary>
        ///     Gets or sets the local rotation angle in radians.
        /// </summary>
        /// <value>
        ///     The local rotation angle in radians.
        /// </value>
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

        protected override void RecalculateWorldMatrix(ref Matrix2D localMatrix, out Matrix2D matrix)
        {
            if (ParentTransform != null)
            {
                ParentTransform.GetWorldMatrix(out matrix);
                Matrix2D.Multiply(ref matrix, ref localMatrix, out matrix);
            }
            else
            {
                matrix = localMatrix;
            }
        }

        protected override void RecalculateLocalMatrix(out Matrix2D matrix)
        {
            if (ParentTransform != null)
            {
                var parentPosition = ParentTransform.Position;
                matrix = Matrix2D.CreateTranslation(-parentPosition) * Matrix2D.CreateScale(_scale) * Matrix2D.CreateRotationZ(_rotation) * Matrix2D.CreateTranslation(parentPosition) * Matrix2D.CreateTranslation(_position);
            }
            else
            {
                matrix = Matrix2D.CreateScale(_scale) * Matrix2D.CreateRotationZ(_rotation) * Matrix2D.CreateTranslation(_position);
            }
        }
    }
}
