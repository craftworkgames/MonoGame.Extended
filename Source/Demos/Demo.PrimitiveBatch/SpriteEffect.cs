using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace Demo.PrimitiveBatch
{
    public class SpriteEffect : Effect
    {
        private static readonly uint _worldProjectionViewDirtyBitMask;
        private static readonly uint _textureDirtyBitMask;

        static SpriteEffect()
        {
            _worldProjectionViewDirtyBitMask = BitVector32.CreateMask();
            _textureDirtyBitMask = BitVector32.CreateMask(_worldProjectionViewDirtyBitMask);
        }

        private BitVector32 _dirtyFlags = _worldProjectionViewDirtyBitMask | _textureDirtyBitMask;
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
            set { SetTexture(ref value); }
        }

        public SpriteEffect(Effect effect)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTexture(ref Texture2D texture)
        {
            _texture = texture;
            _dirtyFlags[_textureDirtyBitMask] = true;
        }

        private void CacheEffectParameters()
        {
            _worldViewProjectionParameter = Parameters["WorldViewProjection"];
            _textureParameter = Parameters["Texture"];
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

            if (_dirtyFlags[_textureDirtyBitMask])
            {
                _dirtyFlags[_textureDirtyBitMask] = false;
                _textureParameter.SetValue(_texture);
            }

            return false;
        }
    }
}
