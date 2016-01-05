using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace SpaceGame
{
    public class MeteorFactory
    {
        private readonly EntityManager _entityManager;
        private readonly List<TextureRegion2D> _bigRegions = new List<TextureRegion2D>();
        private readonly List<TextureRegion2D> _mediumRegions = new List<TextureRegion2D>();
        private readonly List<TextureRegion2D> _smallRegions = new List<TextureRegion2D>();
        private readonly List<TextureRegion2D> _tinyRegions = new List<TextureRegion2D>();
         
        public MeteorFactory(EntityManager entityManager, ContentManager contentManager)
        {
            _entityManager = entityManager;
            var contentManager1 = contentManager;

            _bigRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_big1")));
            _bigRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_big2")));
            _bigRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_big3")));
            _bigRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_big4")));

            _mediumRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_med1")));
            _mediumRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_med2")));

            _smallRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_small1")));
            _smallRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_small2")));

            _tinyRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_tiny1")));
            _tinyRegions.Add(new TextureRegion2D(contentManager1.Load<Texture2D>("meteorBrown_tiny2")));
        }
    }
}