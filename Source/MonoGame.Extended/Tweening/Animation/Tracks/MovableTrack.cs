using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public class MovableTrack<TTransformable> : Track<TTransformable, Vector2>
        where TTransformable : class, IMovable
    {
        protected override void Set(Vector2 value) => Transformable.Position = value;
    }
}