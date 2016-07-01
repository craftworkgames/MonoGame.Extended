using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Graphics.Effects
{
    public class ShapeEffect : Effect, ITextureEffect2D
    {
        public static readonly uint WorldProjectionViewDirtyBitMask;
        public static readonly uint TextureDirtyBitMask;

        static ShapeEffect()
        {
            WorldProjectionViewDirtyBitMask = BitVector32.CreateMask();
            TextureDirtyBitMask = BitVector32.CreateMask(WorldProjectionViewDirtyBitMask);
        }

        protected BitVector32 DirtyFlags = uint.MaxValue; // set all the 32-bits to 1 regardless if they are used or not

        private EffectParameter _worldViewProjectionParameter;
        private EffectParameter _textureParameter;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private Texture2D _texture;

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

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                DirtyFlags[TextureDirtyBitMask] = true;
            }
        }

        public ShapeEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, EffectResource.BatchEffect.Bytecode)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            CacheEffectParameters();
        }

        public ShapeEffect(Effect cloneSource)
            : base(cloneSource)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            CacheEffectParameters();
        }

        protected virtual void CacheEffectParameters()
        {
            _worldViewProjectionParameter = Parameters["WorldViewProjection"];
            _textureParameter = Parameters["Texture"];
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

        protected override bool OnApply()
        {
            base.OnApply();

            if (DirtyFlags[WorldProjectionViewDirtyBitMask])
            {
                DirtyFlags[WorldProjectionViewDirtyBitMask] = false;

                Matrix worldViewProjection;
                Matrix.Multiply(ref _world, ref _view, out worldViewProjection);
                Matrix.Multiply(ref worldViewProjection, ref _projection, out worldViewProjection);
                _worldViewProjectionParameter.SetValue(worldViewProjection);
            }

            // ReSharper disable once InvertIf
            if (DirtyFlags[TextureDirtyBitMask])
            {
                DirtyFlags[TextureDirtyBitMask] = false;

                _textureParameter.SetValue(Texture);
            }

            return false;
        }
    }
}
