using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Graphics.Effects
{
    public class ShapeBatchEffect : Effect
    {
        public static readonly uint WorldProjectionViewDirtyBitMask;

        static ShapeBatchEffect()
        {
            WorldProjectionViewDirtyBitMask = BitVector32.CreateMask();
        }

        protected BitVector32 DirtyFlags = uint.MaxValue; // set all the 32-bits to 1 regardless if they are used or not

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

        public ShapeBatchEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, EffectResource.BatchEffect.Bytecode)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            CacheEffectParameters();
        }

        public ShapeBatchEffect(Effect cloneSource)
            : base(cloneSource)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            CacheEffectParameters();
        }

        protected virtual void CacheEffectParameters()
        {
            _worldViewProjectionParameter = Parameters[name: "WorldViewProjection"];
        }

        protected override bool OnApply()
        {
            base.OnApply();

            // ReSharper disable once InvertIf
            if (DirtyFlags[WorldProjectionViewDirtyBitMask])
            {
                DirtyFlags[WorldProjectionViewDirtyBitMask] = false;

                Matrix worldViewProjection;
                Matrix.Multiply(ref _world, ref _view, out worldViewProjection);
                Matrix.Multiply(ref worldViewProjection, ref _projection, out worldViewProjection);
                _worldViewProjectionParameter.SetValue(worldViewProjection);
            }

            return false;
        }

        public void SetWorld(ref Matrix world)
        {
            _world = world;
            DirtyFlags[WorldProjectionViewDirtyBitMask] = true;
        }

        public void SetView(ref Matrix view)
        {
            _view = view;
            DirtyFlags[WorldProjectionViewDirtyBitMask] = true;
        }

        public void SetProjection(ref Matrix projection)
        {
            _projection = projection;
            DirtyFlags[WorldProjectionViewDirtyBitMask] = true;
        }
    }
}
