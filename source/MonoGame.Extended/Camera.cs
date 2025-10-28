using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    /// Represents an abstract camera.
    /// </summary>
    /// <typeparam name="T">
    /// The position type for the camera.
    /// Typically <see cref="Vector2"/> for 2D cameras or <see cref="Vector3"/> for 3D cameras
    /// </typeparam>
    public abstract class Camera<T> where T : struct
    {
        /// <summary>
        /// Gets or sets the position fo the camera in world coordinates.
        /// </summary>
        public abstract T Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the camera in radians.
        /// </summary>
        public abstract float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the zoom level of the camera.
        /// </summary>
        /// <value>
        /// The zoom factor where <c>1.0</c> represents the default zoom level.
        /// Values greater than <c>1.0</c> in and values less than <c>1.0</c> zoom out.
        /// </value>
        public abstract float Zoom { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed zoom level.
        /// </summary>
        public abstract float MinimumZoom { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed zoom level.
        /// </summary>
        public abstract float MaximumZoom { get; set; }

        /// <summary>
        /// Gets or sets the vertical scale multiplier applied to the camera's zoom.
        /// </summary>
        /// <remarks>
        /// This property is deprecated and will be removed in the next major version.
        /// </remarks>
        [Obsolete("Pitch will be removed in the next major version")]
        public abstract float Pitch { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed pitch value.
        /// </summary>
        /// <remarks>
        /// This property is deprecated and will be removed in the next major version.
        /// </remarks>
        [Obsolete("Pitch will be removed in the next major version")]
        public abstract float MinimumPitch { get; set; }

        /// <summary>
        /// Gets or Sets the maximum allowed pitch value.
        /// </summary>
        /// <remarks>
        /// This property is deprecated and will be removed in the next major version.
        /// </remarks>
        [Obsolete("Pitch will be removed in the next major version")]
        public abstract float MaximumPitch { get; set; }

        /// <summary>
        /// Gets the axis-aligned bounding rectangle of the camera's view in world coordinates.
        /// </summary>
        public abstract RectangleF BoundingRectangle { get; }

        /// <summary>
        /// Gets or sets the origin point for rotation and zoom transformations.
        /// </summary>
        /// <remarks>
        /// The origin defines the center point around which rotation and zoom are applied.
        /// Typically set to the center of the viewport.
        /// </remarks>
        public abstract T Origin { get; set; }

        /// <summary>
        /// Gets the center position of the camera's view in world coordinates.
        /// </summary>
        public abstract T Center { get; }

        /// <summary>
        /// Moves the camera by the specified direction vector.
        /// </summary>
        /// <param name="direction">The direction and distance to move the camera.</param>
        public abstract void Move(T direction);

        /// <summary>
        /// Rotates the camera by the specified amount
        /// </summary>
        /// <param name="deltaRadians">The rotation amount in radians to apply.</param>
        public abstract void Rotate(float deltaRadians);

        /// <summary>
        /// Increases the camera's zoom level by the specified amount.
        /// </summary>
        /// <param name="deltaZoom">The amount to increase the zoom by.</param>
        public abstract void ZoomIn(float deltaZoom);

        /// <summary>
        /// Decreases the camera's zoom level by the specified amount.
        /// </summary>
        /// <param name="deltaZoom">The amount to decrease the zoom by.</param>
        public abstract void ZoomOut(float deltaZoom);

        /// <summary>
        /// Increases the pitch value by the specified amount.
        /// </summary>
        /// <param name="deltaZoom">The amount to increase the pitch by.</param>
        /// <remarks>
        /// This method is deprecated and will be removed in the next major version.
        /// </remarks>
        [Obsolete("Pitch will be removed in the next major version")]
        public abstract void PitchUp(float deltaZoom);

        /// <summary>
        /// Decreases the pitch value by the specified amount.
        /// </summary>
        /// <param name="deltaZoom">The amount to decrease the pitch by.</param>
        /// <remarks>
        /// This method is deprecated and will be removed in the next major version.
        /// </remarks>
        [Obsolete("Pitch will be removed in the next major version")]
        public abstract void PitchDown(float deltaZoom);

        /// <summary>
        /// Positions the camera to look at the specified position.
        /// </summary>
        /// <param name="position">The world position to center the camera on.</param>
        public abstract void LookAt(T position);

        /// <summary>
        /// Converts a position from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="worldPosition">The position in world coordinates.</param>
        /// <returns>The corresponding position in screen coordinates.</returns>
        public abstract T WorldToScreen(T worldPosition);

        /// <summary>
        /// Converts a position from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="screenPosition">The position in screen coordinates.</param>
        /// <returns>The corresponding position in world coordinates.</returns>
        public abstract T ScreenToWorld(T screenPosition);

        /// <summary>
        /// Gets the view transformation matrix for the camera.
        /// </summary>
        /// <returns>A <see cref="Matrix"/> representing the camera's view transformation.</returns>
        public abstract Matrix GetViewMatrix();

        /// <summary>
        /// Gets the inverse of the view transformation matrix for the camera.
        /// </summary>
        /// <returns>A <see cref="Matrix"/> representing the inverse of the camera's view transformation.</returns>
        public abstract Matrix GetInverseViewMatrix();

        /// <summary>
        /// Gets the bounding frustum for the camera's view volume.
        /// </summary>
        /// <returns>A <see cref="BoundingFrustum"/> representing the camera's view volume.</returns>
        public abstract BoundingFrustum GetBoundingFrustum();

        /// <summary>
        /// Determines whether the camera's view contains the specified point.
        /// </summary>
        /// <param name="vector2">The point to test, in world coordinates.</param>
        /// <returns>
        /// A <see cref="ContainmentType"/> indicating whether the point is inside, outside, or
        /// intersects the camera's view.
        /// </returns>
        public abstract ContainmentType Contains(Vector2 vector2);

        /// <summary>
        /// Determines whether the camera's view contains the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to test, in world coordinates.</param>
        /// /// <returns>
        /// A <see cref="ContainmentType"/> indicating whether the rectangle is inside, outside, or
        /// intersects the camera's view.
        /// </returns>
        public abstract ContainmentType Contains(Rectangle rectangle);
    }
}
