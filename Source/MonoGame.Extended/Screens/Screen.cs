using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public abstract class Screen : IDraw, IDisposable
    {
        public bool IsInitialized { get; private set; }
        public bool IsVisible { get; set; }

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

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        public virtual void Dispose()
        {
        }
    }
}