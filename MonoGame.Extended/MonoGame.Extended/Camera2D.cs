using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended
{
    public class Camera2D : IMovable, IRotatable
    {
        public Camera2D(Viewport viewport)
            : this(viewport.Width, viewport.Height)
        {
        }

        public Camera2D(int viewportWidth, int viewportHeight)
        {
            Rotation = 0;
            Zoom = 1;
            Origin = new Vector2(viewportWidth / 2f, viewportHeight / 2f);
            Position = Origin;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }

        public Vector2 ToWorldSpace(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(GetViewMatrix()));
        }

        public Vector2 ToWorldSpace(float x, float y)
        {
            return ToWorldSpace(new Vector2(x, y));
        }

        public Vector2 ToScreenSpace(Vector2 position)
        {
            return Vector2.Transform(position, GetViewMatrix());
        }

        public Vector2 ToScreenSpace(float x, float y)
        {
            return ToScreenSpace(new Vector2(x, y));
        }

        //public Rectangle GetVisibileRectangle(int screenWidth, int screenHeight)
        //{
        //    // NOT TESTED - Source: http://gamedev.stackexchange.com/questions/59301/xna-2d-camera-scrolling-why-use-matrix-transform
        //    var viewMatrix = GetViewMatrix();
        //    var inverseViewMatrix = Matrix.Invert(viewMatrix);
        //    var topLeft = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        //    var topRight = Vector2.Transform(new Vector2(screenWidth, 0), inverseViewMatrix);
        //    var bottomLeft = Vector2.Transform(new Vector2(0, screenHeight), inverseViewMatrix);
        //    var bottomRight = Vector2.Transform(new Vector2(screenWidth, screenHeight), inverseViewMatrix);
        //    var min = new Vector2(
        //        MathHelper.Min(topLeft.X, MathHelper.Min(topRight.X, MathHelper.Min(bottomLeft.X, bottomRight.X))),
        //        MathHelper.Min(topLeft.Y, MathHelper.Min(topRight.Y, MathHelper.Min(bottomLeft.Y, bottomRight.Y))));
        //    var max = new Vector2(
        //        MathHelper.Max(topLeft.X, MathHelper.Max(topRight.X, MathHelper.Max(bottomLeft.X, bottomRight.X))),
        //        MathHelper.Max(topLeft.Y, MathHelper.Max(topRight.Y, MathHelper.Max(bottomLeft.Y, bottomRight.Y))));

        //    return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        //}

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
