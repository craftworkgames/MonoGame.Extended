using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public class ScalableTrack<TTransformable> : Track<TTransformable, Vector2>
        where TTransformable : class, IScalable
    {
        protected override void Set(Vector2 value) => Transformable.Scale = value;
    }
}