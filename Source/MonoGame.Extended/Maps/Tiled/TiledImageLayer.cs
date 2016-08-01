using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledImageLayer : TiledLayer, IMovable
    {
        private readonly Texture2D _texture;
        private readonly VertexPositionTexture[] _imageVertices;
        private readonly short[] _verticesIndexes;

        public TiledImageLayer(string name, Texture2D texture, Vector2 position, int z)
            : base(name, z)
        {
            Position = position;
            _texture = texture;

            _imageVertices = new VertexPositionTexture[4];
            _imageVertices[0] = new VertexPositionTexture(new Vector3(position, z), new Vector2(0f, 0f));
            _imageVertices[1] = new VertexPositionTexture(new Vector3(position.X + texture.Width, position.Y, z), new Vector2(1f, 0f));
            _imageVertices[2] = new VertexPositionTexture(new Vector3(position.X, position.Y + texture.Height, z), new Vector2(0f, 1f));
            _imageVertices[3] = new VertexPositionTexture(new Vector3(position.X + texture.Width, position.Y + texture.Height, z), new Vector2(1f, 1f));

            _verticesIndexes = new short[6];
            _verticesIndexes[0] = 0; _verticesIndexes[1] = 1; _verticesIndexes[2] = 2;
            _verticesIndexes[3] = 1; _verticesIndexes[4] = 3; _verticesIndexes[5] = 2;
        }

        public override void Dispose()
        {
        }

        public Vector2 Position { get; set; }
        public Texture2D Texture => _texture;
        public VertexPositionTexture[] ImageVertices => _imageVertices;
        public short[] ImageVerticesIndexes => _verticesIndexes;

        public override void Draw(SpriteBatch spriteBatch, Rectangle? visibleRectangle = null, Color? backgroundColor = null, GameTime gameTime = null)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(_texture, Position, Color.White * Opacity);
        }
    }
}