namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public class RotatableTrack<TTransformable> : Track<TTransformable, float>
        where TTransformable : class, IRotatable
    {
        protected override void Set(float value) => Transformable.Rotation = value;
    }
}