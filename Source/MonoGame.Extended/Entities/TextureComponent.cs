using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Entities
{
    public class TextureComponent : VisualComponent
    {
        public Texture2D Texture { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Color Color { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public float LayerDepth { get; set; }
        public override void Draw(Vector2 position, Vector2 scale, float rotation) {
            SpriteBatch.Draw(Texture, position, null, SourceRectangle, Origin, rotation, scale, Color, SpriteEffects, LayerDepth);
        }
    }
    public class MeshComponent : VisualComponent
    {
        public Vertex2D[] Vertices { get; set; }
        public override void Draw(Vector2 position, Vector2 scale, float rotation) {
            throw new System.NotImplementedException();
        }
    }

}