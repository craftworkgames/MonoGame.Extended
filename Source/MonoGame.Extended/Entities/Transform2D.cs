using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities
{
    public class Transform2D : TransformComponent, IMovable, IRotatable, IScalable
    {

        public override Matrix TransformMatrix
            => MatrixExtensions.Create2DTransformation(Position, Scale, Rotation);
        public Vector2 Position { get; set; }
        public float XPosition
        {
            get
            {
                return Position.X;
            }
            set { Position = new Vector2(value, Position.Y); }
        }
        public float YPosition
        {
            get
            {
                return Position.Y;
            }
            set { Position = new Vector2(Position.X, value); }
        }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public float XScale
        {
            get
            {
                return Scale.X;
            }
            set { Scale = new Vector2(value, Scale.Y); }
        }
        public float YScale
        {
            get
            {
                return Scale.Y;
            }
            set { Scale = new Vector2(Scale.X, value); }
        }
    }


}