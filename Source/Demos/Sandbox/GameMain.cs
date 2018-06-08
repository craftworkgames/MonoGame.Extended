using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using Sandbox.Screens;

namespace Sandbox
{
    public class EcsAspect
    {
        private readonly BitArray _allSet;
        private readonly BitArray _exclusionSet;
        private readonly BitArray _oneSet;

        private const int _defaultSize = 32;

        public EcsAspect()
        {
            _allSet = new BitArray(_defaultSize);
            _exclusionSet = new BitArray(_defaultSize);
            _oneSet = new BitArray(_defaultSize);
        }

        public void All(bool[] set)
        {
            for (var i = 0; i < set.Length; i++)
                _allSet.Set(i, set[i]);
        }

        public bool IsInterested(EcsEntity entity)
        {
            return IsInterested(entity.ComponentBits);
        }

        public bool IsInterested(BitArray componentBits)
        {
            if (componentBits.And(_allSet) == componentBits)
                return true;

            return false;
        }
    }

    public abstract class EcsSystem
    {
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }

    public class EcsEntity
    {
        internal EcsEntity()
        {
            ComponentBits = new BitArray(32);
        }

        public BitArray ComponentBits { get; }
        
        public void Attach<T>() 
            where T : class
        {
        }

        public void Attach<T>(T component) 
            where T : class
        {
        }

        public void Detach<T>()
        {
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }
    }

    public class EcsWorld : DrawableGameComponent
    {
        private readonly List<EcsEntity> _entities = new List<EcsEntity>();
        private readonly List<EcsSystem> _systems = new List<EcsSystem>();

        public EcsWorld(Game game) 
            : base(game)
        {
        }

        public EcsEntity CreateEntity()
        {
            var entity = new EcsEntity();
            _entities.Add(entity);
            return entity;
        }

        public void DestroyEntity(EcsEntity entity)
        {
            _entities.Remove(entity);
        }

        public void AddSystem(EcsSystem system)
        {
            _systems.Add(system);
        }

        public void RemoveSystem(EcsSystem system)
        {
            _systems.Remove(system);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var system in _systems)
                system.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var system in _systems)
                system.Draw(gameTime);
        }
    }

    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 480,
                SynchronizeWithVerticalRetrace = false
            };

            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);

            _screenManager = Components.Add<ScreenManager>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _screenManager.LoadScreen(new TitleScreen(this), new FadeTransition(GraphicsDevice, Color.Black, 0.5f));
        }
    }
}
