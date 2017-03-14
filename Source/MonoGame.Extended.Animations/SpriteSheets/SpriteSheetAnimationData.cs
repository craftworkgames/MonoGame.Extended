namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheetAnimationData
    {
        public SpriteSheetAnimationData(int[] frameIndicies, float frameDuration = 0.2f, bool isLooping = true,
            bool isReversed = false, bool isPingPong = false)
        {
            FrameIndicies = frameIndicies;
            FrameDuration = frameDuration;
            IsLooping = isLooping;
            IsReversed = isReversed;
            IsPingPong = isPingPong;
        }

        public int[] FrameIndicies { get; }
        public float FrameDuration { get; }
        public bool IsLooping { get; }
        public bool IsReversed { get; }
        public bool IsPingPong { get; }
    }
}