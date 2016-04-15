using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Interpolation.Easing;

namespace MonoGame.Extended.Animations.Transformations
{
    //these use interfaces to transform, no reflection needed -> fastest
    public class MoveableTransform<T> : TweenTransformBase<T, Vector2> where T : class, IMovable
    {
        public MoveableTransform(double time, Vector2 value, EasingFunction easing = null)
            : base(time, value, easing) { }

        protected override void SetValue(double t, T transformable, Vector2 previous) {
            transformable.Position = (float)t * (Value - previous) + previous;
        }
        
    }
    public class RotationTransform<T> : TweenTransformBase<T, float> where T : class, IRotatable
    {
        public RotationTransform(double time, float value, EasingFunction easing = null)
            : base(time, value, easing) { }
        protected override void SetValue(double t, T transformable, float previous) {
            transformable.Rotation = (float)t * (Value - previous) + previous;
        }
    }
    public class ScaleTransform<T> : TweenTransformBase<T, Vector2> where T : class, IScalable
    {
        public ScaleTransform(double time, Vector2 value, EasingFunction easing = null)
            : base(time, value, easing) { }
        protected override void SetValue(double t, T transformable, Vector2 previous) {
            transformable.Scale = (float)t * (Value - previous) + previous;
        }
    }
    //public class ColorTransform<T> : TweenTransformBase<T, Vector4> where T : class, IColorable
    //{
    //    public ColorTransform(double time, Color value, Easing easing = null)
    //        : base(time, value.ToVector4(), easing) { }
    //    protected override void SetValue(double t, T transformable, Vector4 previous) {
    //        transformable.Color = new Color((float)t * (Value - previous) + previous);
    //    }
    //}
    //TODO make an IMesh interface which holds Vertex2D vertices
    public class MeshTransform<TVertex> : TweenTransformBase<IList<TVertex>, IList<TVertex>>
        where TVertex : IMovable, IVertexType
    {
        public MeshTransform(double time, IList<TVertex> value, EasingFunction easing = null) : base(time, value, easing) { }
        protected override void SetValue(double t, IList<TVertex> transformable, IList<TVertex> previous) {
            var n = Math.Min(transformable.Count, Math.Min(Value.Count, previous.Count));
            for (var i = 0; i < n; i++) {
                transformable[i].Position = (float)t * (previous[i].Position - Value[i].Position) + Value[i].Position;
            }
        }
        public override string ToString() => $"Mesh-Transform: {Time}ms, {Value}";
    }
}