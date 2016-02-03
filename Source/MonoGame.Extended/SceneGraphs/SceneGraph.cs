using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneGraph : IDraw, IUpdate
    {
        public SceneGraph(GraphicsDevice graphicsDevice, ViewportAdapter viewportAdapter, Camera2D camera)
        {
            _viewportAdapter = viewportAdapter;
            _camera = camera;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        private readonly ViewportAdapter _viewportAdapter;
        private readonly Camera2D _camera;
        private readonly SpriteBatch _spriteBatch;

        public SceneNode RootNode { get; set; }

        public void Draw(GameTime gameTime)
        {
            if (RootNode != null)
            {
                _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
                RootNode.Draw(_spriteBatch);
                _spriteBatch.End();
            }
        }

        public void Update(GameTime gameTime)
        {
            RootNode?.Update(gameTime);
        }
    }
}