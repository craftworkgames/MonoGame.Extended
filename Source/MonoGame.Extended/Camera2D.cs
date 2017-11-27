using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended
{
    public class Camera2D : IMovable, IRotatable
    {
        private readonly ViewportAdapter _viewportAdapter;
        private float _maximumZoom = float.MaxValue;
        private float _minimumZoom;
        private float _zoom;

        public Camera2D(GraphicsDevice graphicsDevice)
            : this(new DefaultViewportAdapter(graphicsDevice))
        {
        }

        public Camera2D(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;

            Rotation = 0;
            Zoom = 1;
            zPosition = 1;
            Origin = new Vector2(viewportAdapter.VirtualWidth/2f, viewportAdapter.VirtualHeight/2f);
            Position = Vector2.Zero;
        }

        public Vector2 Origin { get; set; }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if ((value < MinimumZoom) || (value > MaximumZoom))
                    throw new ArgumentException("Zoom must be between MinimumZoom and MaximumZoom");

                _zoom = value;
            }
        }

        public float MinimumZoom
        {
            get { return _minimumZoom; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("MinimumZoom must be greater than zero");

                if (Zoom < value)
                    Zoom = MinimumZoom;

                _minimumZoom = value;
            }
        }

        public float MaximumZoom
        {
            get { return _maximumZoom; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("MaximumZoom must be greater than zero");

                if (Zoom > value)
                    Zoom = value;

                _maximumZoom = value;
            }
        }
        public float zPosition { get; set;}
        public RectangleF BoundingRectangle
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

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
        }

        public void Rotate(float deltaRadians)
        {
            Rotation += deltaRadians;
        }

        public void ZoomIn(float deltaZoom)
        {
            ClampZoom(Zoom + deltaZoom);
        }

        public void ZoomOut(float deltaZoom)
        {
            ClampZoom(Zoom - deltaZoom);
        }

        private void ClampZoom(float value)
        {
            if (value < MinimumZoom)
                Zoom = MinimumZoom;
            else
                Zoom = value > MaximumZoom ? MaximumZoom : value;
        }

        public void DollyIn(float deltaDolly)
        {
            zPosition -= deltaDolly;
        }

        public void DollyOut(float deltaDolly)
        {
            zPosition += deltaDolly;
        }

        /// <summary>
        /// Outputs a Z dolly value.
        /// Let's say you want a layer with a Z Position of 2 to be drawn at a scale of 0.5.
        /// You'd call this function with 0.5 as the zoom parameter and 2 as the zOffset parameter:
        ///
        /// GetZPositionFromZoom(0.5, 2);
        ///
        /// The result would be 4. In other words, you'd have to set the camera's zPosition to 4.
        /// <seealso cref="GetZoomFromZPosition(float, float)"/>
        /// </summary>
        public float GetZPositionFromZoom(float zoom, float zOffset)
        {
            return (1 / (zoom)) + zOffset;
        }

        /// <summary>
        /// Outputs a zoom value.
        /// This is the sister function to GetZPositionFromZoom.
        /// Finds a layer's zoom value relative to an other layer.
        /// <seealso cref="GetZPositionFromZoom(float, float)"/>
        /// </summary>
        public float GetZoomFromZPosition(float zPosition, float zOffset)
        {
            //Might be a good idea to return 0 in case of a division by 0.
            return (1 / (zPosition - zOffset));
        }
        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(_viewportAdapter.VirtualWidth/2f, _viewportAdapter.VirtualHeight/2f);
        }

        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            var viewport = _viewportAdapter.Viewport;
            return Vector2.Transform(worldPosition + new Vector2(viewport.X, viewport.Y), GetViewMatrix());
        }

        public Vector2 ScreenToWorld(float x, float y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            var viewport = _viewportAdapter.Viewport;
            return Vector2.Transform(screenPosition - new Vector2(viewport.X, viewport.Y),
                Matrix.Invert(GetViewMatrix()));
        }

        public bool IsVisible(float zOffset)
        {
            float zoom = GetZoomFromZPosition(zPosition, zOffset);
            //10 Mean we can only get within 0.1 Z distance from a layer before we are considered behind it.
            return zoom > 0 && zoom < 10;
        }
        
        public Matrix GetViewMatrix(float zOffset)
        {
            return GetVirtualViewMatrix(zOffset) * _viewportAdapter.GetScaleMatrix();
        }

        private Matrix GetVirtualViewMatrix(float zOffset)
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f))*
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f))*
                Matrix.CreateRotationZ(Rotation)*
                Matrix.CreateScale(Zoom, Zoom, 1)*
                Matrix.CreateScale(GetZoomFromZPosition(zPosition, zOffset), GetZoomFromZPosition(zPosition, zOffset), 1)*
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        private Matrix GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(0);
        }

        public Matrix GetViewMatrix()
        {
            return GetViewMatrix(0);
        }

        public Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, _viewportAdapter.VirtualWidth,
                _viewportAdapter.VirtualHeight, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        public BoundingFrustum GetBoundingFrustum()
        {
            var viewMatrix = GetVirtualViewMatrix();
            var projectionMatrix = GetProjectionMatrix(viewMatrix);
            return new BoundingFrustum(projectionMatrix);
        }

        public ContainmentType Contains(Point point)
        {
            return Contains(point.ToVector2());
        }

        public ContainmentType Contains(Vector2 vector2)
        {
            return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
        }

        public ContainmentType Contains(Rectangle rectangle)
        {
            var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            var boundingBox = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(boundingBox);
        }
    }
}
