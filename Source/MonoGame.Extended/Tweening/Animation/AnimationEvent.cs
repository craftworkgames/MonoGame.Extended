namespace MonoGame.Extended.Tweening.Animation
{
    public class AnimationEvent
    {
        public float FloatValue { get; set; }
        public int IntValue { get; set; }
        public string Name { get; set; }
        public string StringValue { get; set; }
        public double Time { get; set; }

        public AnimationEvent Copy() {
            return (AnimationEvent)MemberwiseClone();
        }
    }
}