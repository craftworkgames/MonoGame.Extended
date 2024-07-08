using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTilesetAnimatedTile : TiledMapTilesetTile
    {
        private TimeSpan _timer = TimeSpan.Zero;
        private int _frameIndex;

        public ReadOnlyCollection<TiledMapTilesetTileAnimationFrame> AnimationFrames { get; }
        public TiledMapTilesetTileAnimationFrame CurrentAnimationFrame { get; private set; }

        public TiledMapTilesetAnimatedTile(int localTileIdentifier,
            TiledMapTilesetTileAnimationFrame[] frames, string type = null, TiledMapObject[] objects = null, Texture2D texture = null)
            : base(localTileIdentifier, type, objects, texture)
        {
            if (frames.Length == 0) throw new InvalidOperationException("There must be at least one tileset animation frame");

            AnimationFrames = new ReadOnlyCollection<TiledMapTilesetTileAnimationFrame>(frames);
            CurrentAnimationFrame = AnimationFrames[0];
        }

        public void CreateTextureRotations(TiledMapTileset tileset, TiledMapTileFlipFlags flipFlags)
        {
            for (int i = 0; i < AnimationFrames.Count; i++)
            {
                AnimationFrames[i].CreateTextureRotations(tileset, flipFlags);
            }
        }

        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime;

            if (_timer <= CurrentAnimationFrame.Duration)
                return;

            _timer -= CurrentAnimationFrame.Duration;
            _frameIndex = (_frameIndex + 1) % AnimationFrames.Count;
            CurrentAnimationFrame = AnimationFrames[_frameIndex];
        }
    }
}
