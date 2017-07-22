namespace MonoGame.Extended.Particles.Modifiers
{
    public abstract class Modifier
    {
        protected Modifier()
        {
            Name = GetType().Name;
        }

        public string Name { get; set; }
        public abstract void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator);

        public override string ToString()
        {
            return $"{Name} [{GetType().Name}]";
        }
    }
}