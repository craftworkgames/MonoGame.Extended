using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Cameras;

public static class CameraExtensions
{
    public static Vector2 ScreenToWorld(this Camera<Vector2> camera, Point point)
    {
        return camera.ScreenToWorld(point.ToVector2());
    }

}
