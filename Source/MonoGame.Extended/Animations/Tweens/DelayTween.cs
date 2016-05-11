namespace MonoGame.Extended.Animations.Tweens
{
    public class DelayTween : Animation
    {
        public DelayTween(float duration)
            : base(null, true)
        {
            Duration = duration;
        }

        public float Duration { get; set; }

        protected override bool OnUpdate(float deltaTime)
        {
            return CurrentTime >= Duration;
        }
    }
}