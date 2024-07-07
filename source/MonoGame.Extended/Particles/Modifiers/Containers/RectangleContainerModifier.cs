using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles.Modifiers.Containers
{
    public sealed class RectangleContainerModifier : Modifier
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float RestitutionCoefficient { get; set; } = 1;

        public override unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                var left = particle->TriggerPos.X + Width*-0.5f;
                var right = particle->TriggerPos.X + Width*0.5f;
                var top = particle->TriggerPos.Y + Height*-0.5f;
                var bottom = particle->TriggerPos.Y + Height*0.5f;

                var xPos = particle->Position.X;
                var xVel = particle->Velocity.X;
                var yPos = particle->Position.Y;
                var yVel = particle->Velocity.Y;

                if ((int) particle->Position.X < left)
                {
                    xPos = left + (left - xPos);
                    xVel = -xVel*RestitutionCoefficient;
                }
                else
                {
                    if (particle->Position.X > right)
                    {
                        xPos = right - (xPos - right);
                        xVel = -xVel*RestitutionCoefficient;
                    }
                }

                if (particle->Position.Y < top)
                {
                    yPos = top + (top - yPos);
                    yVel = -yVel*RestitutionCoefficient;
                }
                else
                {
                    if ((int) particle->Position.Y > bottom)
                    {
                        yPos = bottom - (yPos - bottom);
                        yVel = -yVel*RestitutionCoefficient;
                    }
                }
                particle->Position = new Vector2(xPos, yPos);
                particle->Velocity = new Vector2(xVel, yVel);
            }
        }
    }
}