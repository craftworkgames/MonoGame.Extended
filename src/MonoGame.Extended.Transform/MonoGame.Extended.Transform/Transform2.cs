// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended.Transform
{
    /// <summary>
    ///     Provides the position, rotation, and scale of a two-dimensional game object.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Setting the <see cref="Parent" /> allows to apply position, rotation and scale to
    ///         game objects hierarchically.
    ///     </para>
    /// </remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
    [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = "Public API.")]
    public class Transform2
    {
        private readonly Action _parentDirtyAction;
        private TransformFlags _flags = TransformFlags.All;
        private Matrix3x2 _localMatrix; // model space to local space
        private Transform2? _parent;

        private Vector2 _position;
        private float _rotation;
        private Vector2 _scale = Vector2.One;
        private Matrix3x2 _worldMatrix; // local space to world space

        /// <summary>
        ///     Occurs when the <see cref="WorldMatrix" /> has been re-calculated.
        /// </summary>
        public event Action? Updated;

        private event Action? Dirty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Transform2" /> class.
        /// </summary>
        public Transform2()
        {
            _parentDirtyAction = MatricesAreDirty;
        }

        /// <summary>
        ///     Gets or sets the local position.
        /// </summary>
        /// <value>
        ///     The local position.
        /// </value>
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                LocalMatrixIsDirty = true;
                WorldMatrixIsDirty = true;
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
            get => _rotation;
            set
            {
                _rotation = value;
                LocalMatrixIsDirty = true;
                WorldMatrixIsDirty = true;
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
            get => _scale;
            set
            {
                _scale = value;
                LocalMatrixIsDirty = true;
                WorldMatrixIsDirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the parent <see cref="Transform2" />.
        /// </summary>
        /// <value>
        ///     The parent <see cref="Transform2" />.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Setting <see cref="Parent" /> to a non-null <see cref="Transform2" /> instance enables the
        ///         <see cref="Position" />, <see cref="Rotation" />, and <see cref="Scale" /> to be inherited from said
        ///         <see cref="Transform2" />. Setting <see cref="Parent" /> to <c>null</c> disables the inheritance.
        ///     </para>
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Transform2? Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                {
                    return;
                }

                var oldParentTransform = Parent;
                _parent = value;
                OnParentChanged(oldParentTransform, value);
            }
        }

        /// <summary>
        ///     Gets the model-to-local space <see cref="Matrix3x2" />.
        /// </summary>
        /// <returns>The model-to-local space <see cref="Matrix3x2" />.</returns>
        public ref readonly Matrix3x2 LocalMatrix()
        {
            // NOTE: This method uses the `ref readonly` keyword because the return value is a struct larger than a pointer.
            //     See https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code for more details.

            RecalculateLocalMatrix();
            return ref _localMatrix;
        }

        /// <summary>
        ///     Gets the local-to-world space <see cref="Matrix3x2" />.
        /// </summary>
        /// <returns>The local-to-world space <see cref="Matrix3x2" />.</returns>
        public ref readonly Matrix3x2 WorldMatrix()
        {
            // NOTE: This method uses the `ref readonly` keyword because the return value is a struct larger than a pointer.
            //     See https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code for more details.

            RecalculateWorldMatrix();
            return ref _worldMatrix;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Position: {Position}, Rotation: {Rotation}, Scale: {Scale}";
        }

        private void OnParentChanged(Transform2? oldParent, Transform2? newParent)
        {
            var parent = oldParent;
            while (parent != null)
            {
                parent.Dirty -= _parentDirtyAction;
                parent = parent.Parent;
            }

            parent = newParent;
            while (parent != null)
            {
                parent.Dirty += _parentDirtyAction;
                parent = parent.Parent;
            }
        }

        private void MatricesAreDirty()
        {
            _flags |= TransformFlags.All;
        }

        private void RecalculateWorldMatrix()
        {
            if (!WorldMatrixIsDirty)
            {
                return;
            }

            RecalculateLocalMatrix();

            if (Parent != null)
            {
                var worldMatrix = Parent._worldMatrix;
                _worldMatrix = _localMatrix * worldMatrix;
            }
            else
            {
                _worldMatrix = _localMatrix;
            }

            WorldMatrixIsDirty = false;
            Updated?.Invoke();
        }

        private void RecalculateLocalMatrix()
        {
            if (!LocalMatrixIsDirty)
            {
                return;
            }

            _localMatrix = Matrix3x2.CreateScale(_scale) *
                           Matrix3x2.CreateRotation(_rotation) *
                           Matrix3x2.CreateTranslation(_position);

            LocalMatrixIsDirty = false;
            WorldMatrixIsDirty = true;
        }

        private bool LocalMatrixIsDirty
        {
            // NOTE: This is a helper property to make the code more readable; it is intended to act like a C macro.

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_flags & TransformFlags.LocalMatrixIsDirty) == 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    _flags |= TransformFlags.LocalMatrixIsDirty;
                }
                else
                {
                    _flags &= ~TransformFlags.LocalMatrixIsDirty;
                }
            }
        }

        private bool WorldMatrixIsDirty
        {
            // NOTE: This is a helper property to make the code more readable; it is intended to act like a C macro.

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_flags & TransformFlags.WorldMatrixIsDirty) == 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    _flags |= TransformFlags.WorldMatrixIsDirty;
                }
                else
                {
                    _flags &= ~TransformFlags.WorldMatrixIsDirty;
                    Dirty?.Invoke();
                }
            }
        }
    }
}
