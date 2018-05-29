using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Screens
{
    public abstract class GameScreen : Screen
    {
        protected GameScreen(Game game)
        {
            Game = game;
        }

        public Game Game { get; }
        public ContentManager Content => Game.Content;
        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        public GameServiceContainer Services => Game.Services;
    }
}