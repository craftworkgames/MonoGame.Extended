using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.SpaceGame.Entities
{
    public class Spaceship : Entity
    {
        private readonly IBulletFactory _bulletFactory;

        private readonly Sprite _sprite;
        private readonly Transform2 _transform;

        private float _fireCooldown;

        public int ShieldHealth { get; private set; } = 10;
        public float ShieldRadius { get; private set; } = 50;

        public CircleF BoundingCircle;

        public Vector2 Direction => Vector2.UnitX.Rotate(Rotation);

        public Vector2 Position
        {
            get => _transform.Position;
            set
            {
                _transform.Position = value;
                BoundingCircle.Center = value;
            }
        }

        public float Rotation
        {
            get => _transform.Rotation - MathHelper.ToRadians(90);
            set => _transform.Rotation = value + MathHelper.ToRadians(90);
        }

        public Vector2 Velocity { get; set; }

        public Spaceship(TextureRegion2D textureRegion, IBulletFactory bulletFactory)
        {
            _bulletFactory = bulletFactory;
            _sprite = new Sprite(textureRegion);
            _transform = new Transform2
            {
                Scale = Vector2.One * 0.5f,
                Position = new Vector2(400, 240)
            };
            BoundingCircle = new CircleF(_transform.Position, 20);
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            Velocity *= 0.98f;

            if (_fireCooldown > 0)
            {
                _fireCooldown -= deltaTime;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _transform);

            if (ShieldHealth > 0)
                spriteBatch.DrawCircle(Position, ShieldRadius, 32, Color.Green * 0.2f, ShieldHealth);
        }

        public void Accelerate(float acceleration)
        {
            Velocity += Direction * acceleration;
        }

        public void LookAt(Vector2 point)
        {
            Rotation = (float)Math.Atan2(point.Y - Position.Y, point.X - Position.X);
        }

        public void Fire()
        {
            if (_fireCooldown > 0)
            {
                return;
            }

            var angle = Rotation + MathHelper.ToRadians(90);
            var muzzle1Position = Position + new Vector2(14, 0).Rotate(angle);
            var muzzle2Position = Position + new Vector2(-14, 0).Rotate(angle);
            _bulletFactory.Create(muzzle1Position, Direction, angle, 550f);
            _bulletFactory.Create(muzzle2Position, Direction, angle, 550f);
            _fireCooldown = 0.2f;
        }

        public void HitShield()
        {
            ShieldHealth--;
            ShieldRadius--;
        }
    }
}