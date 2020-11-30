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
    /// <remarks>
    ///     <para>
    ///         Every game object has a transform which is used to store and manipulate the position, rotation and scale
    ///         of the object. Every transform can have a parent, which allows to apply position, rotation and scale to game
    ///         objects hierarchically.
    ///     </para>
    ///     <para>
    ///         This class shouldn't be used directly. Instead use either of the derived classes; <see cref="Transform2" /> or
    ///         Transform3D.
    ///     </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseTransform<TMatrix>
        where TMatrix : struct
    {
        private TransformFlags _flags = TransformFlags.All; // dirty flags, set all dirty flags when created
        private TMatrix _localMatrix; // model space to local space
        private BaseTransform<TMatrix> _parent; // parent
        private TMatrix _worldMatrix; // local space to world space

        // internal contructor because people should not be using this class directly; they should use Transform2D or Transform3D
        internal BaseTransform()
        {
        }

        /// <summary>
        ///     Gets the model-to-local space <see cref="Matrix2" />.
        /// </summary>
        /// <value>
        ///     The model-to-local space <see cref="Matrix2" />.
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
        ///     Gets the local-to-world space <see cref="Matrix2" />.
        /// </summary>
        /// <value>
        ///     The local-to-world space <see cref="Matrix2" />.
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BaseTransform<TMatrix> Parent
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

        public event Action TransformBecameDirty; // observer pattern for when the world (or local) matrix became dirty
        public event Action TranformUpdated; // observer pattern for after the world (or local) matrix was re-calculated

        /// <summary>
        ///     Gets the model-to-local space <see cref="Matrix2" />.
        /// </summary>
        /// <param name="matrix">The model-to-local space <see cref="Matrix2" />.</param>
        public void GetLocalMatrix(out TMatrix matrix)
        {
            RecalculateLocalMatrixIfNecessary();
            matrix = _localMatrix;
        }

        /// <summary>
        ///     Gets the local-to-world space <see cref="Matrix2" />.
        /// </summary>
        /// <param name="matrix">The local-to-world space <see cref="Matrix2" />.</param>
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
            TransformBecameDirty?.Invoke();
        }

        private void OnParentChanged(BaseTransform<TMatrix> oldParent, BaseTransform<TMatrix> newParent)
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
            TranformUpdated?.Invoke();
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

    /// <summary>
    ///     Represents the position, rotation, and scale of a two-dimensional game object.
    /// </summary>
    /// <seealso cref="BaseTransform{Matrix2D}" />
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
    public class Transform2 : BaseTransform<Matrix2>, IMovable, IRotatable, IScalable
    {
        private Vector2 _position;
        private float _rotation;
        private Vector2 _scale = Vector2.One;

        public Transform2()
            : this(Vector2.Zero, 0, Vector2.One)
        {
        }

        public Transform2(float x, float y, float rotation = 0, float scaleX = 1, float scaleY = 1)
            : this(new Vector2(x, y), rotation, new Vector2(scaleX, scaleY))
        {
        }

        public Transform2(Vector2? position = null, float rotation = 0, Vector2? scale = null)
        {
            Position = position ?? Vector2.Zero;
            Rotation = rotation;
            Scale = scale ?? Vector2.One;
        }

        /// <summary>
        ///     Gets the world position.
        /// </summary>
        /// <value>
        ///     The world position.
        /// </value>
        public Vector2 WorldPosition => WorldMatrix.Translation;

        /// <summary>
        ///     Gets the world scale.
        /// </summary>
        /// <value>
        ///     The world scale.
        /// </value>
        public Vector2 WorldScale => WorldMatrix.Scale;

        /// <summary>
        ///     Gets the world rotation angle in radians.
        /// </summary>
        /// <value>
        ///     The world rotation angle in radians.
        /// </value>
        public float WorldRotation => WorldMatrix.Rotation;

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

        protected internal override void RecalculateWorldMatrix(ref Matrix2 localMatrix, out Matrix2 matrix)
        {
            if (Parent != null)
            {
                Parent.GetWorldMatrix(out matrix);
                Matrix2.Multiply(ref localMatrix, ref matrix, out matrix);
            }
            else
            {
                matrix = localMatrix;
            }
        }

        protected internal override void RecalculateLocalMatrix(out Matrix2 matrix)
        {
            matrix = Matrix2.CreateScale(_scale) *
                     Matrix2.CreateRotationZ(_rotation) *
                     Matrix2.CreateTranslation(_position);
        }

        public override string ToString()
        {
            return $"Position: {Position}, Rotation: {Rotation}, Scale: {Scale}";
        }
    }


	/// <summary>
	///     Represents the position, rotation, and scale of a three-dimensional game object.
	/// </summary>
	/// <seealso cref="BaseTransform{Matrix}" />
	/// <remarks>
	///     <para>
	///         Every game object has a transform which is used to store and manipulate the position, rotation and scale
	///         of the object. Every transform can have a parent, which allows to apply position, rotation and scale to game
	///         objects hierarchically.
	///     </para>
	/// </remarks>
	public class Transform3 : BaseTransform<Matrix> {
		private Vector3 _position;
		private Quaternion _rotation;
		private Vector3 _scale = Vector3.One;

		public Transform3(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null) {
			Position = position ?? Vector3.Zero;
			Rotation = rotation ?? Quaternion.Identity;
			Scale = scale ?? Vector3.One;
		}

		/// <summary>
		///     Gets the world position.
		/// </summary>
		/// <value>
		///     The world position.
		/// </value>
		public Vector3 WorldPosition => WorldMatrix.Translation;

		/// <summary>
		///     Gets the world scale.
		/// </summary>
		/// <value>
		///     The world scale.
		/// </value>
		public Vector3 WorldScale {
			get {
				Vector3 scale = Vector3.Zero;
				Quaternion rotation = Quaternion.Identity;
				Vector3 translation = Vector3.Zero;
				WorldMatrix.Decompose(out scale, out rotation, out translation);
				return scale;
			}
		}


		/// <summary>
		///     Gets the world rotation quaternion in radians.
		/// </summary>
		/// <value>
		///     The world rotation quaternion in radians.
		/// </value>
		public Quaternion WorldRotation {
			get {
				Vector3 scale = Vector3.Zero;
				Quaternion rotation = Quaternion.Identity;
				Vector3 translation = Vector3.Zero;
				WorldMatrix.Decompose(out scale, out rotation, out translation);
				return rotation;
			}
		}

		/// <summary>
		///     Gets or sets the local position.
		/// </summary>
		/// <value>
		///     The local position.
		/// </value>
		public Vector3 Position {
			get { return _position; }
			set {
				_position = value;
				LocalMatrixBecameDirty();
				WorldMatrixBecameDirty();
			}
		}

		/// <summary>
		///     Gets or sets the local rotation quaternion in radians.
		/// </summary>
		/// <value>
		///     The local rotation quaternion in radians.
		/// </value>
		public Quaternion Rotation {
			get { return _rotation; }
			set {
				_rotation = value;
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
		public Vector3 Scale {
			get { return _scale; }
			set {
				_scale = value;
				LocalMatrixBecameDirty();
				WorldMatrixBecameDirty();
			}
		}

		protected internal override void RecalculateWorldMatrix(ref Matrix localMatrix, out Matrix matrix) {
			if (Parent != null) {
				Parent.GetWorldMatrix(out matrix);
				Matrix.Multiply(ref localMatrix, ref matrix, out matrix);
			}
			else {
				matrix = localMatrix;
			}
		}

		protected internal override void RecalculateLocalMatrix(out Matrix matrix) {
			matrix = Matrix.CreateScale(_scale) *
					 Matrix.CreateFromQuaternion(_rotation) *
					 Matrix.CreateTranslation(_position);
		}

		public override string ToString() {
			return $"Position: {Position}, Rotation: {Rotation}, Scale: {Scale}";
		}
	}
}