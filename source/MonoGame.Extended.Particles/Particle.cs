using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoGame.Extended.Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle
    {
        public float Inception;
        public float Age;
        public Vector2 Position;
        public Vector2 TriggerPos;
        public Vector2 Velocity;
        public HslColor Color;
        public float Opacity;
        public Vector2 Scale;
        public float Rotation;
        public float Mass;
        public float LayerDepth;

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Particle));
    }
}