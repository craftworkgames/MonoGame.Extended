namespace MonoGame.Extended.Interpolation
{
    public interface IInterpolator<T>
    {
        T Add(T t1, T t2);
        T Interpolate(T t1, T t2, double amount);
        T Mult(T value, double amount);
        T Substract(T t1, T t2);
    }

    public abstract class Interpolator<T> : IInterpolator<T> //where T : struct
    {
        public abstract T Add(T t1, T t2);
        public abstract T Mult(T value, double amount);
        public abstract T Substract(T t1, T t2);
        public virtual T Interpolate(T t1, T t2, double amount) {
            return Add(t1, Mult(Substract(t2, t1), amount));
        }
    }
}