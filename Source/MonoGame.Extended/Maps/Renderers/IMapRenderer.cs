using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Maps.Tiled;

namespace MonoGame.Extended.Maps.Renderers
{
    public interface IMapRenderer
    {
        void SwapMap(TiledMap newMap);

        void Update(GameTime gameTime);

        [Obsolete]
        void Draw(Camera2D camera);

        void Draw(Matrix viewMatrix);
    }
}