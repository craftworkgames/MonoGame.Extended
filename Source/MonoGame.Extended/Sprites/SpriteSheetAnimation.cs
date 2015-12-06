namespace MonoGame.Extended.Sprites
{
    public class SpriteSheetAnimation
    {
        internal SpriteSheetAnimation(string name, int framesPerSecond, int[] frameIndicies)
        {
            Name = name;
            FramesPerSecond = framesPerSecond;
            FrameIndicies = frameIndicies;
        }

        public string Name { get; private set; }
        public int FramesPerSecond { get; set; }
        public int[] FrameIndicies { get; private set; }
    }
}