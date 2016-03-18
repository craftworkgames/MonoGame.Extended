using System.Runtime.InteropServices;

namespace MonoGame.Extended.Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle
    {
        public float Inception;
        public float Age;
        public Vector Position;
        public Vector TriggerPos;
        public Vector Velocity;
        public HslColor Colour;
        public float Opacity;
        public Vector Scale;
        public float Rotation;
        public float Mass;

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Particle));
    }
}