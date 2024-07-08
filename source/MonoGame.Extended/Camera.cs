using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class Camera<T> where T : struct
    {
        public abstract T Position { get; set; }
        public abstract float Rotation { get; set; }
        public abstract float Zoom { get; set; }
        public abstract float MinimumZoom { get; set; }
        public abstract float MaximumZoom { get; set; }
        public abstract float Pitch { get; set; }
        public abstract float MinimumPitch { get; set; }
        public abstract float MaximumPitch { get; set; }
        public abstract RectangleF BoundingRectangle { get; }
        public abstract T Origin { get; set; }
        public abstract T Center { get; }

        public abstract void Move(T direction);
        public abstract void Rotate(float deltaRadians);
        public abstract void ZoomIn(float deltaZoom);
        public abstract void ZoomOut(float deltaZoom);
        public abstract void PitchUp(float deltaZoom);
        public abstract void PitchDown(float deltaZoom);
        public abstract void LookAt(T position);

        public abstract T WorldToScreen(T worldPosition);
        public abstract T ScreenToWorld(T screenPosition);

        public abstract Matrix GetViewMatrix();
        public abstract Matrix GetInverseViewMatrix();

        public abstract BoundingFrustum GetBoundingFrustum();
        public abstract ContainmentType Contains(Vector2 vector2);
        public abstract ContainmentType Contains(Rectangle rectangle);
    }
}