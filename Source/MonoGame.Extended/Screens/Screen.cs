using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public abstract class Screen : IDisposable
    {
        public virtual void Dispose() { }
        public virtual void Show() { }
        public virtual void Hide() { }
        public virtual void Resize(int width, int height) { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}