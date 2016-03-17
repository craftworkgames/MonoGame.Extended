namespace MonoGame.Extended.Particles.Modifiers.Containers
{
    public class RectLoopContainerModifier : IModifier
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                var left =   particle->TriggerPos.X + Width * -0.5f;
                var right =  particle->TriggerPos.X + Width * 0.5f;
                var top =    particle->TriggerPos.Y + Height * -0.5f;
                var bottom = particle->TriggerPos.Y + Height * 0.5f;

                float xPos = particle->Position.X;
                float yPos = particle->Position.Y;

                if ((int)particle->Position.X < left)
                {
                    xPos = particle->Position.X + Width;
                }
                else if ((int)particle->Position.X > right)
                {
                    xPos = particle->Position.X - Width;
                }

                if ((int)particle->Position.Y < top)
                {
                    yPos = particle->Position.Y + Height;
                }
                else if ((int)particle->Position.Y > bottom)
                {
                    yPos = particle->Position.Y - Height;
                }
                particle->Position = new Vector(xPos, yPos);
            }
        }
    }
}