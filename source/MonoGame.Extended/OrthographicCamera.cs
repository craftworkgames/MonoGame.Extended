using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended
{
    /// <summary>
    /// Represents an orthographic (2D) camera that provides view and projection transformations for rendering
    /// within a 2D world.
    /// </summary>
    public sealed class OrthographicCamera : Camera<Vector2>, IMovable, IRotatable
    {
        private readonly ViewportAdapter _viewportAdapter;
        private float _maximumZoom = float.MaxValue;
        private float _minimumZoom;
        private float _zoom;
        private float _pitch;
        private float _maximumPitch = float.MaxValue;
        private float _minimumPitch;
        private Vector2 _position;
        private Rectangle _worldBounds;
        private bool _clampZoomToWorldBounds;

        /// <inheritdoc/>
        /// <remarks>
        /// When <see cref="IsClampedToWorldBounds"/> is <see langword="true"/>, the camera position is clamped so that its
        /// view remains within the defined <see cref="WorldBounds"/>.
        /// </remarks>
        public override Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;

                if (IsClampedToWorldBounds)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        /// <inheritdoc/>
        public override float Rotation { get; set; }

        /// <inheritdoc/>
        /// <remarks>
        /// When <see cref="IsClampedToWorldBounds"/> is <see langword="true"/>, the camera zoom is clamped so that its
        /// view remains within the defined <see cref="WorldBounds"/>.
        /// </remarks>
        public override float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;

                bool canClampToWorldBounds = CanClampToWorldBounds();

                if (IsZoomClampedToWorldBounds && canClampToWorldBounds)
                {
                    ClampZoomToWorldBounds();
                }

                _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);

                if (canClampToWorldBounds)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        /// <inheritdoc/>
        public override float MinimumZoom
        {
            get => _minimumZoom;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                _minimumZoom = value;

                bool canClampToWorldBounds = CanClampToWorldBounds();

                if (IsZoomClampedToWorldBounds && canClampToWorldBounds)
                {
                    ClampZoomToWorldBounds();
                }

                _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);

                if (canClampToWorldBounds)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        /// <inheritdoc/>
        public override float MaximumZoom
        {
            get => _maximumZoom;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                _maximumZoom = value;
                bool canClampToWorldBounds = CanClampToWorldBounds();

                if (IsZoomClampedToWorldBounds && canClampToWorldBounds)
                {
                    ClampZoomToWorldBounds();
                }

                _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);

                if (canClampToWorldBounds)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        /// <inheritdoc/>
        [Obsolete("Pitch will be removed in the next major version")]
        public override float Pitch
        {
            get => _pitch;
            set => _pitch = MathHelper.Clamp(value, _minimumPitch, _maximumPitch);
        }

        /// <inheritdoc/>
        [Obsolete("Pitch will be removed in the next major version")]
        public override float MinimumPitch
        {
            get => _minimumPitch;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                _minimumPitch = value;
                _pitch = MathHelper.Clamp(_pitch, _minimumPitch, _maximumPitch);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Pitch will be removed in the next major version")]
        public override float MaximumPitch
        {
            get => _maximumPitch;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                _maximumPitch = value;
                _pitch = MathHelper.Clamp(_pitch, _minimumPitch, _maximumPitch);
            }
        }

        /// <inheritdoc/>
        public override RectangleF BoundingRectangle
        {
            get
            {
                var frustum = GetBoundingFrustum();
                var corners = frustum.GetCorners();
                var topLeft = corners[0];
                var bottomRight = corners[2];
                var width = bottomRight.X - topLeft.X;
                var height = bottomRight.Y - topLeft.Y;
                return new RectangleF(topLeft.X, topLeft.Y, width, height);
            }
        }

        /// <inheritdoc/>
        public override Vector2 Origin { get; set; }

        /// <inheritdoc/>
        public override Vector2 Center => Position + Origin;

        /// <summary>
        /// Gets the bounding rectangle that defines the limits of the camera's movement.
        /// </summary>
        /// <remarks>
        /// Use <see cref="EnableWorldBounds(Rectangle)"/> to set world bounds and enable constraints,
        /// or <see cref="DisableWorldBounds()"/> to remove constraints.
        /// </remarks>
        public Rectangle WorldBounds => _worldBounds;

        /// <summary>
        /// Gets a value indicating whether the camera is currently constrained within world bounds.
        /// </summary>
        /// <remarks>
        /// Use <see cref="EnableWorldBounds(Rectangle)"/> to enable world bounds constraints,
        /// or <see cref="DisableWorldBounds()"/> to disable them.
        /// </remarks>
        public bool IsClampedToWorldBounds { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera zoom should be clamped to world bounds.
        /// </summary>
        /// <remarks>
        /// When <see langword="true"/>, the camera zoom is constrained so that the view cannot extend
        /// beyond the world bounds. When <see langword="false"/>, zoom is only constrained by
        /// <see cref="MinimumZoom"/> and <see cref="MaximumZoom"/>.
        /// This property only has effect when <see cref="IsClampedToWorldBounds"/> is <see langword="true"/>.
        /// </remarks>
        public bool IsZoomClampedToWorldBounds
        {
            get => _clampZoomToWorldBounds;
            set
            {
                _clampZoomToWorldBounds = value;

                if (value)
                {
                    ClampZoomToWorldBounds();
                    _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);
                    ClampPositionToWorldBounds();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthographicCamera"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor uses the <see cref="DefaultViewportAdapter"/>.
        /// </remarks>
        /// <param name="graphicsDevice">The graphics device to associate with this camera.</param>
        public OrthographicCamera(GraphicsDevice graphicsDevice)
            : this(new DefaultViewportAdapter(graphicsDevice))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthographicCamera"/> class using the specified viewport adapter.
        /// </summary>
        /// <param name="viewportAdapter">
        /// The viewport adapter that defines how world and screen coordinates are transformed.
        /// </param>
        public OrthographicCamera(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;

            Rotation = 0;
            Zoom = 1;
            Pitch = 1;
            Origin = new Vector2(viewportAdapter.VirtualWidth / 2f, viewportAdapter.VirtualHeight / 2f);
            Position = Vector2.Zero;
        }

        /// <inheritdoc/>
        public override void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
        }

        /// <inheritdoc/>
        public override void Rotate(float deltaRadians)
        {
            Rotation += deltaRadians;
        }

        /// <inheritdoc/>
        public override void ZoomIn(float deltaZoom)
        {
            Zoom += deltaZoom;
        }

        /// <summary>
        /// Increases the camera's zoom level while maintaining a specified world position as the zoom center.
        /// </summary>
        /// <param name="deltaZoom">The amount to increase the zoom by.</param>
        /// <param name="zoomCenter">
        /// The world position to use as the zoom center. This point will remain fixed in screen space
        /// as the zoom changes.
        /// </param>
        public void ZoomIn(float deltaZoom, Vector2 zoomCenter)
        {
            float previousZoom = Zoom;
            Zoom += deltaZoom;

            if (Zoom != previousZoom)
            {
                Position += (zoomCenter - Origin - Position) * ((Zoom - previousZoom) / Zoom);
            }
        }

        /// <inheritdoc/>
        public override void ZoomOut(float deltaZoom)
        {
            Zoom -= deltaZoom;
        }

        /// <summary>
        /// Decreases the camera's zoom level while maintaining a specified world position as the zoom center.
        /// </summary>
        /// <param name="deltaZoom">The amount to decrease the zoom by.</param>
        /// <param name="zoomCenter">
        /// The world position to use as the zoom center. This point will remain fixed in screen space
        /// as the zoom changes.
        /// </param>
        public void ZoomOut(float deltaZoom, Vector2 zoomCenter)
        {
            float previousZoom = Zoom;
            Zoom -= deltaZoom;

            if (Zoom != previousZoom)
            {
                Position += (zoomCenter - Origin - Position) * ((Zoom - previousZoom) / Zoom);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Pitch will be removed in the next major version")]
        public override void PitchUp(float deltaPitch)
        {
            Pitch += deltaPitch;
        }

        /// <inheritdoc/>
        [Obsolete("Pitch will be removed in the next major version")]
        public override void PitchDown(float deltaPitch)
        {
            Pitch -= deltaPitch;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The camera is positioned so that the specified <paramref name="position"/> appears at the center of
        /// the viewport.
        /// </remarks>
        public override void LookAt(Vector2 position)
        {
            Position = position - new Vector2(_viewportAdapter.VirtualWidth / 2f, _viewportAdapter.VirtualHeight / 2f);
        }

        /// <summary>
        /// Converts a position from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="x">The x-position in world coordinates.</param>
        /// <param name="y">The y-position in world coordinates.</param>
        /// <returns>The corresponding position in screen coordinates.</returns>
        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        /// <inheritdoc/>
        public override Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Vector2 screenPosition = Vector2.Transform(worldPosition, GetViewMatrix());

            // For scaling viewport adapters, the viewport offset is part of the coordinate transformation
            if (_viewportAdapter is ScalingViewportAdapter)
            {
                var viewport = _viewportAdapter.Viewport;
                screenPosition += new Vector2(viewport.X, viewport.Y);
            }

            return screenPosition;
        }

        /// <summary>
        /// Converts a position from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="x">The x-position in screen coordinates.</param>
        /// <param name="y">The y-position in screen coordinates.</param>
        /// <returns>The corresponding position in world coordinates.</returns>
        public Vector2 ScreenToWorld(float x, float y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        /// <inheritdoc/>
        public override Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            // For scaling viewport adapters, the viewport offset is part of the coordinate transformation
            if (_viewportAdapter is ScalingViewportAdapter)
            {
                var viewport = _viewportAdapter.Viewport;
                screenPosition -= new Vector2(viewport.X, viewport.Y);
            }

            return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix()));
        }

        /// <summary>
        /// Gets the view transformation matrix for the camera, applying a parallax factor.
        /// </summary>
        /// <param name="parallaxFactor">
        /// The parallax factor to apply to the camera position. A value of (1,1) applies no parallax,
        /// while values closer to (0,0) create a stronger parallax effect for background layers.
        /// </param>
        /// <returns>
        /// A <see cref="Matrix"/> representing the camera's view transformation with the specified
        /// parallax factor applied.
        /// </returns>
        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            return GetVirtualViewMatrix(parallaxFactor) * _viewportAdapter.GetScaleMatrix();
        }

        private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom * Pitch, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        private Matrix GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(Vector2.One);
        }

        /// <inheritdoc/>
        public override Matrix GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }

        /// <inheritdoc/>
        public override Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        /// <inheritdoc/>
        public override BoundingFrustum GetBoundingFrustum()
        {
            var viewMatrix = GetVirtualViewMatrix();
            var projectionMatrix = GetProjectionMatrix(viewMatrix);
            return new BoundingFrustum(projectionMatrix);
        }

        /// <summary>
        /// Determines whether the camera's view contains the specified point.
        /// </summary>
        /// <param name="point">The point to test, in world coordinates.</param>
        /// <returns>
        /// A <see cref="ContainmentType"/> indicating whether the point is inside, outside, or
        /// intersects the camera's view.
        /// </returns>
        public ContainmentType Contains(Point point)
        {
            return Contains(point.ToVector2());
        }

        /// <inheritdoc/>
        public override ContainmentType Contains(Vector2 vector2)
        {
            return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
        }

        /// <inheritdoc/>
        public override ContainmentType Contains(Rectangle rectangle)
        {
            var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            var boundingBox = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(boundingBox);
        }

        /// <summary>
        /// Enables world bounds constraint for the camera and sets the bounding rectangle.
        /// </summary>
        /// <param name="worldBounds">
        /// The bounding rectangle that defines the limits of the camera's movement and zoom.
        /// </param>
        /// <remarks>
        /// When world bounds are enabled, the camera position and zoom are automatically clamped to
        /// ensure the visible area does not extend beyond the specified bounds. This only applies
        /// when the camera has no rotation and the pitch is 1.0.
        /// </remarks>
        public void EnableWorldBounds(Rectangle worldBounds)
        {
            _worldBounds = worldBounds;
            IsClampedToWorldBounds = true;
            ClampPositionToWorldBounds();
        }

        /// <summary>
        /// Disables world bounds constraint for the camera.
        /// </summary>
        /// <remarks>
        /// When world bounds are disabled, the camera can move and zoom freely without any constraints.
        /// The world bounds rectangle is reset to <see cref="Rectangle.Empty"/>.
        /// </remarks>
        public void DisableWorldBounds()
        {
            _worldBounds = Rectangle.Empty;
            IsClampedToWorldBounds = false;
        }

        private void ClampZoomToWorldBounds()
        {
            // Calculate the size of the area the camera can see
            Vector2 cameraSize = new Vector2(_viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight) / _zoom;

            // Only enforce minimum zoom if the camera view is larger than world bounds
            if (cameraSize.X > _worldBounds.Width || cameraSize.Y > _worldBounds.Height)
            {
                float minZoomX = (float)_viewportAdapter.VirtualWidth / _worldBounds.Width;
                float minZoomY = (float)_viewportAdapter.VirtualHeight / _worldBounds.Height;
                float minZoom = MathHelper.Max(minZoomX, minZoomY);

                if (_zoom < minZoom)
                {
                    _zoom = minZoom;
                }
            }
        }

        private void ClampPositionToWorldBounds()
        {
            // Calculate the size of the area the camera can see
            Vector2 cameraSize = new Vector2(_viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight) / _zoom;

            // If the world bounds are smaller than the camera view, then we center the camera in the world bounds.
            if (_worldBounds.Width < cameraSize.X || _worldBounds.Height < cameraSize.Y)
            {
                _position = _worldBounds.Center.ToVector2() - Origin;
                return;
            }

            // Get the camera's top-left corner in world space
            Matrix inverseViewMatrix = GetInverseViewMatrix();
            Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, inverseViewMatrix);

            Vector2 worldBoundsMin = new Vector2(_worldBounds.Left, _worldBounds.Top);
            Vector2 worldBoundsMax = new Vector2(_worldBounds.Right, _worldBounds.Bottom);

            // Calculate difference between position and world-space top-left.
            Vector2 positionOffset = _position - cameraWorldMin;

            // Clamp the camera's world-space top-left corner, then apply the offset
            _position = Vector2.Clamp(cameraWorldMin, worldBoundsMin, worldBoundsMax - cameraSize) + positionOffset;
        }

        private bool CanClampToWorldBounds()
        {
            if (!IsClampedToWorldBounds || _worldBounds.Width <= 0 || _worldBounds.Height <= 0)
            {
                return false;
            }

            if (MathHelper.Distance(Rotation, 0.0f) >= 0.001f || MathHelper.Distance(Pitch, 1.0f) >= 0.001f)
            {
                return false;
            }

            return true;
        }
    }
}
