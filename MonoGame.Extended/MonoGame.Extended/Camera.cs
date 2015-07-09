using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public abstract class Camera : IMovable, IRotatable
    {
        protected Camera()
        {
            Position = Vector2.Zero;
            Rotation = 0;
            Zoom = 1;
            Origin = Vector2.Zero;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }

        public Vector2 ToWorldSpace(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(CalculateTransformMatrix()));
        }

        public Vector2 ToWorldSpace(float x, float y)
        {
            return ToWorldSpace(new Vector2(x, y));
        }

        public Vector2 ToScreenSpace(Vector2 position)
        {
            return Vector2.Transform(position, CalculateTransformMatrix());
        }

        public Vector2 ToScreenSpace(float x, float y)
        {
            return ToScreenSpace(new Vector2(x, y));
        }

        //public Rectangle GetVisibileRectangle(int screenWidth, int screenHeight)
        //{
        //    // NOT TESTED - Source: http://gamedev.stackexchange.com/questions/59301/xna-2d-camera-scrolling-why-use-matrix-transform
        //    var viewMatrix = CalculateTransformMatrix();
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

        public Matrix CalculateTransformMatrix(Vector2 parallaxFactor)
        {
            var cx = (int)(Position.X * parallaxFactor.X);
            var cy = (int)(Position.Y * parallaxFactor.Y);
            return
                Matrix.CreateTranslation(new Vector3(-cx, -cy, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                //Matrix.CreateScale(_viewport.Scale.X, _viewport.Scale.Y, 1.0f) *
                Matrix.CreateScale(Zoom) *
                Matrix.CreateTranslation(Origin.X, Origin.Y, 0);
        }

        public Matrix CalculateTransformMatrix()
        {
            return CalculateTransformMatrix(Vector2.One);
        }
    }
}
