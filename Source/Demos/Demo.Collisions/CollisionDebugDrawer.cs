using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Drawing;
using MonoGame.Extended.Graphics.Batching;
using MonoGame.Extended.Shapes.BoundingVolumes;

namespace Demo.Collisions
{
    public class CollisionDrawer : ICollisionDrawer
    {
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Effect _effect;

        public CollisionDrawer(GraphicsDevice graphicsDevice, Effect effect)
        {
            _primitiveBatch = new PrimitiveBatch<VertexPositionColor>(graphicsDevice);
            _effect = effect;
        }

        public void Begin()
        {
            _primitiveBatch.Begin();
        }

        public void DrawBoundingVolume(ref AxisAlignedBoundingBox2D boundingVolume)
        {
            _primitiveBatch.DrawAxisAlignedBoundingBox(_effect, ref boundingVolume);
        }

        public void End()
        {
            _primitiveBatch.End();
        }
    }
}
