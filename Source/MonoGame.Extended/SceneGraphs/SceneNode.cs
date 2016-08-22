using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneNode : IMovable, IRotatable, IScalable
    {
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

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public SceneNode Parent { get; internal set; }
        public SceneNodeCollection Children { get; }
        public SceneEntityCollection Entities { get; }
        public object Tag { get; set; }

        public RectangleF GetBoundingRectangle()
        {
            Vector2 position, scale;
            float rotation;
            GetWorldTransform().Decompose(out position, out rotation, out scale);

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

        public Matrix GetWorldTransform()
        {
            return Parent == null ? Matrix.Identity : Matrix.Multiply(GetLocalTransform(), Parent.GetWorldTransform());
        }

        public Matrix GetLocalTransform()
        {
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1));
            var translationMatrix = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            var tempMatrix = Matrix.Multiply(scaleMatrix, rotationMatrix);
            return Matrix.Multiply(tempMatrix, translationMatrix);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 offsetPosition, offsetScale;
            float offsetRotation;
            var worldTransform = GetWorldTransform();
            worldTransform.Decompose(out offsetPosition, out offsetRotation, out offsetScale);

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
