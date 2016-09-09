using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collision.BoundingVolumes;
using MonoGame.Extended.Collision.Shapes;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Collision
{
    public class CollisionRenderer2D
    {
        private readonly DynamicBatch2D _batch;

        public CollisionSimulation2D CollisionSimulation { get; }
        public Color BoundingVolumeFillColor = Color.AliceBlue * 0.5f;
        public Color BoundingVolumeStrokeColor = Color.AliceBlue;
        public Color ShapeFillColor = Color.Cornsilk * 0.5f;
        public Color ShapeStrokeColor = Color.Cornsilk;

        public CollisionRenderer2D(CollisionSimulation2D collisionSimulation, GraphicsDevice graphicsDevice)
        {

            if (collisionSimulation == null)
            {
                throw new ArgumentNullException(nameof(collisionSimulation));
            }

            CollisionSimulation = collisionSimulation;

            _batch = new DynamicBatch2D(graphicsDevice);
        }

        public void Begin()
        {
            _batch.Begin(rasterizerState: RasterizerState.CullNone);
        }

        public void End()
        {
            _batch.End();
        }

        public void DrawBoundingVolume(BoundingVolume2D boundingVolume)
        {
            switch (boundingVolume.Type)
            {
                case BoundingVolumeType2D.BoundingBox:
                    DrawBoundingBox((BoundingBox2D)boundingVolume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawBoundingBox(BoundingBox2D boundingBox)
        {
            var size = boundingBox.Radius * 2;
            _batch.DrawRectangle(boundingBox.Centre, size, BoundingVolumeFillColor);
        }

        public void DrawShape(CollisionShape2D shape, ref Matrix2D worldMatrix)
        {
            _batch.DrawConvexPolygon(shape.Vertices, ref worldMatrix, ShapeFillColor);
        }
    }
}
