﻿using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles
{
    public class ParticleReleaseParameters
    {
        public ParticleReleaseParameters()
        {
            Quantity = 1;
            Speed = new Range<float>(-1f, 1f);
            Color = new Range<HslColor>(new HslColor(0f, 0.5f, 0.5f), new HslColor(360f, 0.5f, 0.5f));
            Opacity = new Range<float>(0f, 1f);
            Scale = new Range<float>(1f, 10f);
            Rotation = new Range<float>(-MathHelper.Pi, MathHelper.Pi);
            Mass = 1f;
        }

        public Range<int> Quantity { get; set; }
        public Range<float> Speed { get; set; }
        public Range<HslColor> Color { get; set; }
        public Range<float> Opacity { get; set; }
        public Range<float> Scale { get; set; }
        public Range<float> Rotation { get; set; }
        public Range<float> Mass { get; set; }
    }
}