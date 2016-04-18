using System.Diagnostics.Contracts;

namespace MonoGame.Extended.Tweening.Animation
{
    public interface IAnimation
    {
        string Name { get; set; }
        double Duration { get; }
        void Update(double time);
        void End();
        void AddChildAnimation(IAnimation sub);
        IAnimation Copy();
    }
}