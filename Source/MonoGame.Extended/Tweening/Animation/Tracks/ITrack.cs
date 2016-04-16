namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    public interface ITrack<TTransformable> where TTransformable : class
    {
        bool Interpolate { get; set; }
        TTransformable Transformable { get; set; }
        void Update(double time);
    }
}