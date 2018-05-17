﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace Demo.Features.Demos
{
    public class CollisionDemo : DemoBase
    {
        private readonly CollisionComponent _collisionComponent =
            new CollisionComponent(new RectangleF(-1000, -500, 2000, 1000));
        private List<DemoActor> _actors = new List<DemoActor>();
        private SpriteBatch _spriteBatch;
        private Texture2D _spikeyBallTexture;
        private Texture2D _blankTexture;

        public CollisionDemo(GameMain game) : base(game)
        {
        }

        public override string Name { get; } = "Collisions";

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var spikeyBallTexture = Content.Load<Texture2D>("Textures/spike_ball");
            _spikeyBallTexture = spikeyBallTexture;

            _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.WhiteSmoke });

            var demoBall = new DemoBall(new Sprite(_spikeyBallTexture))
            {
                Position = new Vector2(100, 100),
                BoundingBox = new RectangleF(100, 100, 20, 20)
            };
            _actors.Add(demoBall);

            var wall1 = new DemoWall(new Sprite(_blankTexture));
            wall1.BoundingBox = new RectangleF(0, 0, 800, 20);
            _actors.Add(wall1);

            var wall2 = new DemoWall(new Sprite(_blankTexture));
            wall2.BoundingBox = new RectangleF(0, 460, 800, 20);
            _actors.Add(wall2);

            foreach (var actor in _actors)
            {
                _collisionComponent.Insert(actor);
            }

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var actor in _actors)
            {
                actor.Update(gameTime);
            }
            _collisionComponent.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            foreach (var actor in _actors)
            {
                _spriteBatch.Draw(_blankTexture, actor.BoundingBox.ToRectangle(), Color.WhiteSmoke);
                actor.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    #region Collision Demo Implementation

    class DemoActor : IActorTarget, IUpdate
    {
        private readonly Sprite _sprite;

        public DemoActor(Sprite sprite)
        {
            _sprite = sprite;
            BoundingBox = sprite.BoundingRectangle;
        }


        public Vector2 Position { get; set; }
        public RectangleF BoundingBox { get; set; }
        public Vector2 Velocity { get; set; }

        public virtual void OnCollision(CollisionInfo collisionInfo)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Position = Position;
            _sprite.Draw(spriteBatch);
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += gameTime.GetElapsedSeconds() * Velocity;
        }
    }

    class DemoWall : DemoActor
    {
        public DemoWall(Sprite sprite) : base(sprite)
        {
        }
    }

    /// <summary>
    /// Ball that bounces on wall
    /// </summary>
    class DemoBall : DemoActor
    {
        public DemoBall(Sprite sprite) : base(sprite)
        {
            Velocity = new Vector2(0f, 80f);
        }

        public override void Update(GameTime gameTime)
        {
            BoundingBox = new RectangleF(Position.X, Position.Y, BoundingBox.Width, BoundingBox.Height);
            base.Update(gameTime);
        }

        public override void OnCollision(CollisionInfo collisionInfo)
        {
//            throw new Exception("Collided with ball!");
            Velocity *= -1;
            base.OnCollision(collisionInfo);
        }
    }

    #endregion
}