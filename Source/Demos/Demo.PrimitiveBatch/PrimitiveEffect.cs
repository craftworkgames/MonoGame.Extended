using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace Demo.PrimitiveBatch
{
    public class PrimitiveEffect : Effect
    {
        private static readonly uint _worldProjectionViewDirtyBitMask;

        static PrimitiveEffect()
        {
            _worldProjectionViewDirtyBitMask = BitVector32.CreateMask();
        }

        private BitVector32 _dirtyFlags = _worldProjectionViewDirtyBitMask;
        private EffectParameter _worldViewProjectionParameter;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        public Matrix World
        {
            get { return _world; }
            set { SetWorld(ref value); }
        }

        public Matrix View
        {
            get { return _view; }
            set { SetView(ref value); }
        }

        public Matrix Projection
        {
            get { return _projection; }
            set { SetProjection(ref value); }
        }

        public PrimitiveEffect(Effect effect)
            : base(effect)
        {
            CacheEffectParameters();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWorld(ref Matrix world)
        {
            _world = world;
            _dirtyFlags[_worldProjectionViewDirtyBitMask] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetView(ref Matrix view)
        {
            _view = view;
            _dirtyFlags[_worldProjectionViewDirtyBitMask] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetProjection(ref Matrix projection)
        {
            _projection = projection;
            _dirtyFlags[_worldProjectionViewDirtyBitMask] = true;
        }

        private void CacheEffectParameters()
        {
            _worldViewProjectionParameter = Parameters["WorldViewProjection"];
        }

        protected override bool OnApply()
        {
            base.OnApply();

            if (_dirtyFlags[_worldProjectionViewDirtyBitMask])
            {
                _dirtyFlags[_worldProjectionViewDirtyBitMask] = false;

                Matrix worldViewProjection;
                Matrix.Multiply(ref _world, ref _view, out worldViewProjection);
                Matrix.Multiply(ref worldViewProjection, ref _projection, out worldViewProjection);
                _worldViewProjectionParameter.SetValue(worldViewProjection);
            }

            return false;
        }
    }
}
