using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Screens
{
    public abstract class Screen : IDisposable
    {
        public ScreenManager ScreenManager { get; internal set; }

        public virtual void Dispose() { }
        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}