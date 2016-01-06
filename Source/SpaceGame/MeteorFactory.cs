using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using SpaceGame.Entities;

namespace SpaceGame
{
    public class MeteorFactory
    {
        private readonly EntityManager _entityManager;
        private readonly List<TextureRegion2D> _bigRegions = new List<TextureRegion2D>();
        private readonly List<TextureRegion2D> _mediumRegions = new List<TextureRegion2D>();
        private readonly List<TextureRegion2D> _smallRegions = new List<TextureRegion2D>();
        private readonly List<TextureRegion2D> _tinyRegions = new List<TextureRegion2D>();
        private readonly Random _random = new Random();

        public MeteorFactory(EntityManager entityManager, ContentManager contentManager)
        {
            _entityManager = entityManager;

            _bigRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_big1")));
            _bigRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_big2")));
            _bigRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_big3")));
            _bigRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_big4")));

            _mediumRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_med1")));
            _mediumRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_med3")));

            _smallRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_small1")));
            _smallRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_small2")));

            _tinyRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_tiny1")));
            _tinyRegions.Add(new TextureRegion2D(contentManager.Load<Texture2D>("meteorBrown_tiny2")));
        }

        public void SpawnNewMeteor(Vector2 playerPosition)
        {
            var rotationSpeed = _random.Next(-10, 10) * 0.1f;
            var spawnCircle = new CircleF(playerPosition, 630);
            var spawnAngle = MathHelper.ToRadians(_random.Next(0, 360));
            var spawnPosition = spawnCircle.GetPointAlongEdge(spawnAngle);
            var velocity = (playerPosition - spawnPosition)
                .Rotate(MathHelper.ToRadians(_random.Next(-15, 15))) * _random.Next(3, 10) * 0.01f;
            var meteor = new Meteor(_bigRegions[_random.Next(0, _bigRegions.Count)], spawnPosition, velocity, rotationSpeed);

            _entityManager.AddEntity(meteor);
        }

    }
}