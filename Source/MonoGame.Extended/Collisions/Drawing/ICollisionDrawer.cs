using MonoGame.Extended.Shapes.BoundingVolumes;

namespace MonoGame.Extended.Collisions.Drawing
{
    public interface ICollisionDrawer
    {
        void Begin();
        void DrawBoundingVolume(ref AxisAlignedBoundingBox2D boundingVolume);
        void End();
    }
}
