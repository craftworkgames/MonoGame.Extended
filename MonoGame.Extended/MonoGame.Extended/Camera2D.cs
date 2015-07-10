using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public class Camera2D
    {
        private readonly Viewport _viewport;

        public Camera2D(Viewport viewport)
        {
            _viewport = viewport;

            Rotation = 0;
            Zoom = 1;
            Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
            Position = Vector2.Zero;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }

        public void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
        }

        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, GetViewMatrix());
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix()));
        }

        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            return 
                Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f)); 
        }

        public Matrix GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }
    }
}
