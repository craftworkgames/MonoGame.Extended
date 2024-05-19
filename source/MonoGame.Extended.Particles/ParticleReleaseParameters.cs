using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoGame.Extended.Particles
{
    public class ParticleReleaseParameters
    {
        public ParticleReleaseParameters()
        {
            Quantity = 1;
            Speed = new Range<float>(-1f, 1f);
            Color = new Range<HslColor>(Microsoft.Xna.Framework.Color.White.ToHsl(), Microsoft.Xna.Framework.Color.White.ToHsl());
            Opacity = new Range<float>(0f, 1f);
            Scale = new Range<float>(1f, 1f);
            Rotation = new Range<float>(-MathHelper.Pi, MathHelper.Pi);
            Mass = 1f;
            MaintainAspectRatioOnScale = true;
            ScaleX = new Range<float>(1f, 1f);
            ScaleY = new Range<float>(1f, 1f);
        }

        public Range<int> Quantity { get; set; }
        public Range<float> Speed { get; set; }
        public Range<HslColor> Color { get; set; }
        public Range<float> Opacity { get; set; }
        public Range<float> Scale { get; set; }
        public Range<float> Rotation { get; set; }
        public Range<float> Mass { get; set; }
        public bool MaintainAspectRatioOnScale { get; set; }
        public Range<float> ScaleX { get; set; }
        public Range<float> ScaleY { get; set; }

    }
}
