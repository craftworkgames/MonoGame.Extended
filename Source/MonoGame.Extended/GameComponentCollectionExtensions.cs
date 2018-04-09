using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class GameComponentCollectionExtensions
    {
        public static T Add<T>(this GameComponentCollection collection)
            where T : IGameComponent, new()
        {
            var gameComponent = new T();
            collection.Add(gameComponent);
            return gameComponent;
        }

        public static T Add<T>(this GameComponentCollection collection, Func<T> createGameComponent)
            where T : IGameComponent
        {
            var gameComponent = createGameComponent();
            collection.Add(gameComponent);
            return gameComponent;
        }
    }
}
