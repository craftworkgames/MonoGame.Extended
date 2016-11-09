using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneNode : Transform2D<SceneNode>
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

        public SceneNodeCollection Children { get; }
        public SceneEntityCollection Entities { get; }
        public object Tag { get; set; }

        public RectangleF BoundingRectangle
        {
            get
            {
                var rectangles = Entities
                    .Select(e =>
                    {
                        var r = e.BoundingRectangle;
                        r.Offset(WorldPosition);
                        return r;
                    })
                    .Concat(Children.Select(i => i.BoundingRectangle))
                    .ToArray();
                var x0 = rectangles.Min(r => r.Left);
                var y0 = rectangles.Min(r => r.Top);
                var x1 = rectangles.Max(r => r.Right);
                var y1 = rectangles.Max(r => r.Bottom);

                return new RectangleF(x0, y0, x1 - x0, y1 - y0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var drawable in Entities.OfType<ISpriteBatchDrawable>())
                if (drawable.IsVisible)
                {
                    var texture = drawable.TextureRegion.Texture;
                    var sourceRectangle = drawable.TextureRegion.Bounds;
                    var position = WorldPosition + drawable.Position;
                    var rotation = WorldRotation + drawable.Rotation;
                    var scale = WorldScale*drawable.Scale;

                    spriteBatch.Draw(texture, position, sourceRectangle, drawable.Color, rotation, drawable.Origin,
                        scale, drawable.Effect, 0);
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