using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public abstract class Screen : IDisposable
    {
        protected Screen()
        {
        }

        public IScreenManager ScreenManager { get; internal set; }
        public bool IsInitialized { get; private set; }
        public bool IsVisible { get; set; }

        public virtual void Dispose()
        {
        }

        public T FindScreen<T>() where T : Screen
        {
            return ScreenManager?.FindScreen<T>();
        }

        public void Show<T>(bool hideThis) where T : Screen
        {
            var screen = FindScreen<T>();
            screen.Show();

            if (hideThis)
                Hide();
        }

        public void Show<T>() where T : Screen
        {
            Show<T>(true);
        }

        public void Show()
        {
            if (!IsInitialized)
                Initialize();

            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}