namespace MonoGame.Extended.Tweening
{
    /// <summary>
    /// Holds functions for basic math operations for generic types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Interpolator<T> //where T : struct
    {
        public abstract T Add(T t1, T t2);
        public abstract T Mult(T value, double amount);
        public abstract T Substract(T t1, T t2);
        public virtual T Interpolate(T t1, T t2, double amount) {
            return Add(t1, Mult(Substract(t2, t1), amount));
        }


    }
}