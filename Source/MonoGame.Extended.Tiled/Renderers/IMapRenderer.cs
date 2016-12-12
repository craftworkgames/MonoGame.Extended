using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled.Renderers
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