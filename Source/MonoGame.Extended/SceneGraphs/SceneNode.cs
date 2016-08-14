using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneNode : IMovable, IRotatable, IScalable
    {
        public string Name { get; set; }
        public Transform2D Transform { get; }
        public SceneNode Parent { get; internal set; }
        public SceneNodeCollection Children { get; }
        public SceneEntityCollection Entities { get; }
        public object Tag { get; set; }

        public Vector2 Position
        {
            get { return Transform.Position; }
            set { Transform.Position = value; }
        }

        public float Rotation
        {
            get { return Transform.RotationAngle; }
            set { Transform.RotationAngle = value; }
        }

        public Vector2 Scale
        {
            get { return Transform.Scale; }
            set { Transform.Scale = value; }
        }

        public SceneNode(string name)
            : this(name, Vector2.Zero, 0, Vector2.One)
        {
        }

        public SceneNode(string name, Vector2 position, float rotation = 0)
            : this(name, position, rotation, Vector2.One)
        {
        }

        public SceneNode(string name, Vector2 position, float rotation, Vector2 scale)
        {
            Transform = new Transform2D();

            Name = name;
            Position = position;
            Rotation = rotation;
            Scale = scale;

            Children = new SceneNodeCollection(this);
            Entities = new SceneEntityCollection();
        }

        public SceneNode()
            : this(null, Vector2.Zero, 0, Vector2.One)
        {
        }

        public RectangleF GetBoundingRectangle()
        {
            Vector2 position, scale;
            float rotation;
            Matrix worldMatrix;
            Transform.GetWorldMatrix(out worldMatrix);
            worldMatrix.Decompose(out position, out rotation, out scale);

            var rectangles = Entities
                .Select(e =>
                {
                    var r = e.GetBoundingRectangle();
                    r.Offset(position);
                    return r;
                })
                .Concat(Children.Select(i => i.GetBoundingRectangle()))
                .ToArray();
            var x0 = rectangles.Min(r => r.Left);
            var y0 = rectangles.Min(r => r.Top);
            var x1 = rectangles.Max(r => r.Right);
            var y1 = rectangles.Max(r => r.Bottom);
            
            return new RectangleF(x0, y0, x1 - x0, y1 - y0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 offsetPosition, offsetScale;
            float offsetRotation;
            Matrix worldMatrix;
            Transform.GetWorldMatrix(out worldMatrix);
            worldMatrix.Decompose(out offsetPosition, out offsetRotation, out offsetScale);

            foreach (var drawable in Entities.OfType<ISpriteBatchDrawable>())
            {
                if (drawable.IsVisible)
                {
                    var texture = drawable.TextureRegion.Texture;
                    var sourceRectangle = drawable.TextureRegion.Bounds;
                    var position = offsetPosition + drawable.Position;
                    var rotation = offsetRotation + drawable.Rotation;
                    var scale = offsetScale * drawable.Scale;

                    spriteBatch.Draw(texture, position, sourceRectangle, drawable.Color, rotation, drawable.Origin, scale, drawable.Effect, 0);
                }
            }

            foreach (var child in Children)
                child.Draw(spriteBatch);
        }

        public override string ToString()
        {
            return $"name: {Name}, position: {Position}, rotation: {Rotation}, scale: {Scale}";
        }
    }
}
