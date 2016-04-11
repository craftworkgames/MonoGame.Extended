namespace MonoGame.Extended.Animations.Tracks
{
    public interface IAnimationTrackGroup
    {
        void Update(double time);
        double MaxEndtime { get; }
        void Clear();
    }
}