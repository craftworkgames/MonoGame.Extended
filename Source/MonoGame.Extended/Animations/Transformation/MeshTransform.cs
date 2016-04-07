using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Animations.Transformation
{
    //TODO make an IMesh interface which holds Vertex2D vertices
    public class MeshTransform<TVertex> : Transform<IList<TVertex>, IList<TVertex>>
        where TVertex : IMovable, IVertexType
    {
        public MeshTransform(double time, IList<TVertex> value, Easing easing = null) : base(time, value, easing) { }
        protected override void SetValue(double t, IList<TVertex> transformable, IList<TVertex> previous) {
            var n = Math.Min(transformable.Count, Math.Min(Value.Count, previous.Count));
            for (var i = 0; i < n; i++) {
                transformable[i].Position = (float)t * (previous[i].Position - Value[i].Position) + Value[i].Position;
            }
        }
        public override string ToString() => $"Mesh-Transform: {Time}ms, {Value}";
    }
}