using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    ///     Represents the position, rotation, and scale of a two-dimensional game object.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent.</typeparam>
    /// <seealso cref="Transform2D" />
    /// <seealso cref="IMovable" />
    /// <seealso cref="IRotatable" />
    /// <seealso cref="IScalable" />
    /// <remarks>
    ///     <para>
    ///         Every game object has a transform which is used to store and manipulate the position, rotation and scale
    ///         of the object. Every transform can have a parent, which allows to apply position, rotation and scale to game
    ///         objects hierarchically.
    ///     </para>
    /// </remarks>
    public class Transform2D<TParent> : Transform2D
        where TParent : Transform2D<TParent>
    {
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
        public new TParent Parent
        {
            get { return (TParent)base.Parent; }
            set { base.Parent = value; }
        }
    }

    /// <summary>
    ///     Represents the position, rotation, and scale of a two-dimensional game object.
    /// </summary>
    /// <seealso cref="Transform{TMatrix,TParent}" />
    /// <seealso cref="IMovable" />
    /// <seealso cref="IRotatable" />
    /// <seealso cref="IScalable" />
    /// <remarks>
    ///     <para>
    ///         Every game object has a transform which is used to store and manipulate the position, rotation and scale
    ///         of the object. Every transform can have a parent, which allows to apply position, rotation and scale to game
    ///         objects hierarchically.
    ///     </para>
    /// </remarks>
    public class Transform2D : Transform<Matrix2D, Transform2D>, IMovable, IRotatable, IScalable
    {
        private Vector2 _position;
        private Vector2 _scale = Vector2.One;
        private float _rotation;

        /// <summary>
        ///     Gets the world position.
        /// </summary>
        /// <value>
        ///     The world position.
        /// </value>
        public Vector2 WorldPosition => WorldMatrix.Translation;

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
        ///     Gets the world scale.
        /// </summary>
        /// <value>
        ///     The world scale.
        /// </value>
        public Vector2 WorldScale => WorldMatrix.Scale;

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
        ///     Gets the world rotation angle in radians.
        /// </summary>
        /// <value>
        ///     The world rotation angle in radians.
        /// </value>
        public float WorldRotation => WorldMatrix.Rotation;

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

        protected internal override void RecalculateWorldMatrix(ref Matrix2D localMatrix, out Matrix2D matrix)
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

        protected internal override void RecalculateLocalMatrix(out Matrix2D matrix)
        {
            if (Parent != null)
            {
                var parentPosition = Parent.Position;
                matrix = Matrix2D.CreateTranslation(-parentPosition) * Matrix2D.CreateScale(_scale) * Matrix2D.CreateRotationZ(-_rotation) * Matrix2D.CreateTranslation(parentPosition) * Matrix2D.CreateTranslation(_position);
            }
            else
            {
                matrix = Matrix2D.CreateScale(_scale) * Matrix2D.CreateRotationZ(-_rotation) * Matrix2D.CreateTranslation(_position);
            }
        }
    }
}