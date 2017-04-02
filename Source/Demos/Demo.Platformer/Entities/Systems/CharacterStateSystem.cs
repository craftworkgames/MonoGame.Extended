//using Demo.Platformer.Entities.Components;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended.Entities;

//namespace Demo.Platformer.Entities.Systems
//{
//    public class CharacterStateSystem : MonoGame.Extended.Entities.System
//    {
//        private readonly EntityFactory _entityFactory;
//        private readonly Vector2 _spawnPoint;

//        public CharacterStateSystem(EntityFactory entityFactory, Vector2 spawnPoint)
//            : base(Aspect.Contains(typeof(CharacterComponent)))
//        {
//            _entityFactory = entityFactory;
//            _spawnPoint = spawnPoint;
//        }

//        protected override void Process(GameTime gameTime, Entity entity)
//        {
//            var state = entity.GetEntityByTag<CharacterComponent>();
//            if (state.IsActive)
//                return;

//            _entityFactory.CreateBloodExplosion(state.Entity.Position);
//            state.Entity.Destroy();

//            if (state.Entity.Name == Entities.Player)
//                _entityFactory.CreatePlayer(_spawnPoint);
//        }
//    }
//}