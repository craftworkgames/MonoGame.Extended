namespace Sandbox
{
    public class SpriteSheetAnimation
    {
        public SpriteSheetAnimation(string name, int framesPerSecond, int[] frameIndicies)
        {
            Name = name;
            FramesPerSecond = framesPerSecond;
            FrameIndicies = frameIndicies;
        }
        
        public string Name { get; private set; }
        public int FramesPerSecond { get; private set; }
        public int[] FrameIndicies { get; private set; }
    }
}