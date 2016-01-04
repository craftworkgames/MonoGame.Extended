using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace SpaceGame.Entities
{
    public class Meteor : Entity
    {
        public Meteor(TextureRegion2D textureRegion, Vector2 velocity)
        {
            _sprite = new Sprite(textureRegion);
            _shape = new CircleF(_sprite.Position, _radius);
            Velocity = velocity;
            HealthPoints = 10;
        }

        private const float _radius = 55f;
        private readonly Sprite _sprite;
        private CircleF _shape;

        public int HealthPoints { get; private set; }
        public Vector2 Velocity { get; set; }

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set
            {
                _sprite.Position = value;
                _shape.Center = _sprite.Position;
            }
        }

        public void Damage(int damage)
        {
            HealthPoints -= damage;

            if (HealthPoints <= 0)
                Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * deltaTime;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }

        public bool Contains(Vector2 position)
        {
            return _shape.Contains(position);
        }
    }
}