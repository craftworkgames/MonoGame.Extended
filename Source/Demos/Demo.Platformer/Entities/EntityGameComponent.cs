using System.Collections.Generic;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Demo.Platformer.Entities
{
    public interface IEntityManager
    {
        void AttachComponent(EntityComponent component);
        void DetachComponent(EntityComponent component);
    }

    public class EntityGameComponent : DrawableGameComponent, IEntityManager
    {
        public EntityGameComponent(Game game) 
            : base(game)
        {
            _entities = new List<Entity>();
            _components = new List<EntityComponent>();
            _nextEntityId = 1;
        }

        private SpriteBatch _spriteBatch;
        private readonly List<Entity> _entities;
        private readonly List<EntityComponent> _components;
        private long _nextEntityId;

        public override void Initialize()
        {
            base.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this, _nextEntityId);
            _entities.Add(entity);
            _nextEntityId++;
            return entity;
        }

        public void DestroyEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void AttachComponent(EntityComponent component)
        {
            _components.Add(component);
        }

        public void DetachComponent(EntityComponent component)
        {
            _components.Remove(component);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var deltaTime = gameTime.GetElapsedSeconds();

            foreach (var entityComponent in _components)
                entityComponent.Update(deltaTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            foreach (var entityComponent in _components)
                entityComponent.Draw(_spriteBatch);

            _spriteBatch.End();
        }
    }
}