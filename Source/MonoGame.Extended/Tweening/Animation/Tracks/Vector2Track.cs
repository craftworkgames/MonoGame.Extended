using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening.Easing;

namespace MonoGame.Extended.Tweening.Animation.Tracks
{
    internal class FloatTrack<T> : Track<T, float> where T : class
    {
        protected override void SetValue(float value) {
            Float = value;
        }

        protected override float GetValue() {
            throw new System.NotImplementedException();
        }

        public float Float { get; set; }
    }

    public abstract class Vector2Track<T> : Track<T, Vector2> where T : class
    {
        internal FloatTrack<T> XTrack { get; }
        internal FloatTrack<T> YTrack { get; }

        protected Vector2Track() {
            XTrack = new FloatTrack<T>();
            YTrack = new FloatTrack<T>();
        }

        //TODO
        public override void Update(double time, double interpolation) {
            base.Update(time, interpolation);
            XTrack.Float = float.NaN;
            XTrack.Update(time, 1);
            YTrack.Float = float.NaN;
            YTrack.Update(time, 1);
            if (float.IsNaN(XTrack.Float) && float.IsNaN(YTrack.Float)) return;
            SetValue(new Vector2(XTrack.Float, YTrack.Float));
        }
    }
}