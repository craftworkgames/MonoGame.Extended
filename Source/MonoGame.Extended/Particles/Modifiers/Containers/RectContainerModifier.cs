namespace MonoGame.Extended.Particles.Modifiers.Containers
{
    public sealed class RectContainerModifier : IModifier
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float RestitutionCoefficient { get; set; } = 1;

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator) {
            
            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                var left =   particle->TriggerPos.X + Width * -0.5f;
                var right =  particle->TriggerPos.X + Width * 0.5f;
                var top =    particle->TriggerPos.Y + Height * -0.5f;
                var bottom = particle->TriggerPos.Y + Height * 0.5f;

                float xPos = particle->Position.X;
                float xVel = particle->Velocity.X;
                float yPos = particle->Position.Y;
                float yVel = particle->Velocity.Y;

                if ((int)particle->Position.X < left) {
                    xPos = left + (left - xPos);
                    xVel = -xVel * RestitutionCoefficient;
                }
                else if (particle->Position.X > right) {
                    xPos = right - (xPos - right);
                    xVel = -xVel * RestitutionCoefficient;
                }

                if (particle->Position.Y < top) {
                    yPos = top + (top - yPos);
                    yVel = -yVel * RestitutionCoefficient;
                }
                else if ((int)particle->Position.Y > bottom) {
                    yPos = bottom - (yPos - bottom);
                    yVel = -yVel * RestitutionCoefficient;
                }
                particle->Position = new Vector(xPos, yPos);
                particle->Velocity = new Vector(xVel, yVel);
            }
        }
    }
}