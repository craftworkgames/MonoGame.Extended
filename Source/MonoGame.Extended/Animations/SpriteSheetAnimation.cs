namespace MonoGame.Extended.Animations
{
    public class SpriteSheetAnimation
    {
        public int[] FrameIndicies { get; private set; }
        public int FramesPerSecond { get; set; }

        public string Name { get; private set; }

        internal SpriteSheetAnimation(string name, int framesPerSecond, int[] frameIndicies)
        {
            Name = name;
            FramesPerSecond = framesPerSecond;
            FrameIndicies = frameIndicies;
        }
    }
}