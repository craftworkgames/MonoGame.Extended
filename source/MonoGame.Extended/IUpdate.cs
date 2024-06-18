using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public interface IUpdate
    {
        /// <summary>
        /// Updates this instance based on the elapsed time, in seconds, specified.
        /// </summary>
        /// <param name="deltaTimeInSeonds">The elapsed time in seconds, since the previous frame.</param>
        void Update(double deltaTime);

        /// <summary>
        /// Updates this instance based on the game time specified.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Updates this instance based on the elapsed time specified.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the previous frame..</param>
        void Update(in TimeSpan elapsedTime);
    }
}
