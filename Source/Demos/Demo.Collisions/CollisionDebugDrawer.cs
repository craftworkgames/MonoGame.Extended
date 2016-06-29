using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Drawing;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Shapes.BoundingVolumes;

namespace Demo.Collisions
{
    public class CollisionDrawer : ICollisionDrawer
    {
        private readonly ShapeBatch _shapeBatch;

        public CollisionDrawer(GraphicsDevice graphicsDevice)
        {
            _shapeBatch = new ShapeBatch(graphicsDevice);
        }

        public void Begin()
        {
            _shapeBatch.Begin(BatchMode.Deferred);
        }

        public void DrawBoundingVolume(ref AxisAlignedBoundingBox2D boundingVolume)
        {
            //_shapeBatch.DrawAxisAlignedBoundingBox(_effect, ref boundingVolume);
        }

        public void End()
        {
            _shapeBatch.End();
        }
    }
}
