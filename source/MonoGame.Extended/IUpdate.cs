using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public interface IUpdate
    {
        void Update(double deltaTime);
        void Update(GameTime gameTime);
        void Update(in TimeSpan elapsedTime);
    }
}
