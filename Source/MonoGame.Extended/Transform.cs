using System;
using System.ComponentModel;

namespace MonoGame.Extended
{
    // Code derived from top answer: http://gamedev.stackexchange.com/questions/113977/should-i-store-local-forward-right-up-vector-or-calculate-when-necessary

    /// <summary>
    ///     Represents the base class for the position, rotation, and scale of a game object in two-dimensions or
    ///     three-dimensions.
    /// </summary>
    /// <typeparam name="TMatrix">The type of the matrix.</typeparam>
    /// <typeparam name="TParent">The type of the parent transform.</typeparam>
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
    public abstract class Transform<TMatrix, TParent>
        where TMatrix : struct where TParent : Transform<TMatrix, TParent>
    {
        private TransformFlags _flags = TransformFlags.All; // dirty flags, set all dirty flags when created
        private TMatrix _localMatrix; // model space to local space
        private TMatrix _worldMatrix; // local space to world space
        private TParent _parent; // parent

        public event Action BecameDirty; // observer pattern for when the world (or local) matrix became dirty
        public event Action Updated; // observer pattern for after the world (or local) matrix was re-calculated

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
        ///     Gets or sets the parent instance.
        /// </summary>
        /// <value>
        ///     The parent instance.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Setting <see cref="Parent" /> to a non-null instance enables this instance to
        ///         inherit the position, rotation, and scale of the parent instance. Setting <see cref="Parent" /> to
        ///         <code>null</code> disables the inheritance altogether for this instance.
        ///     </para>
        /// </remarks>
        public TParent Parent
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

        // internal contructor because people should not be using this class directly; they should use Transform2D or Transform3D
        internal Transform()
        {
        }

        /// <summary>
        ///     Gets the model-to-local space <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The model-to-local space <see cref="Matrix2D" />.</param>
        public void GetLocalMatrix(out TMatrix matrix)
        {
            RecalculateLocalMatrixIfNecessary();
            matrix = _localMatrix;
        }

        /// <summary>
        ///     Gets the local-to-world space <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="matrix">The local-to-world space <see cref="Matrix2D" />.</param>
        public void GetWorldMatrix(out TMatrix matrix)
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
            BecameDirty?.Invoke();
        }

        private void OnParentChanged(TParent oldParent, TParent newParent)
        {
            var parent = oldParent;
            while (parent != null)
            {
                parent.BecameDirty -= ParentOnBecameDirty;
                parent = parent.Parent;
            }

            parent = newParent;
            while (parent != null)
            {
                parent.BecameDirty += ParentOnBecameDirty;
                parent = parent.Parent;
            }
        }

        private void ParentOnBecameDirty()
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
            Updated?.Invoke();
        }

        protected internal abstract void RecalculateWorldMatrix(ref TMatrix localMatrix, out TMatrix matrix);

        private void RecalculateLocalMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.LocalMatrixIsDirty) == 0)
                return;

            RecalculateLocalMatrix(out _localMatrix);

            _flags &= ~TransformFlags.LocalMatrixIsDirty;
            WorldMatrixBecameDirty();
        }

        protected internal abstract void RecalculateLocalMatrix(out TMatrix matrix);
    }
}
