using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.SceneGraphs
{
    public static class SceneGraphExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, SceneGraph sceneGraph)
        {
            sceneGraph.Draw(spriteBatch);
        }
    }
}