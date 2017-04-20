using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapTilesetAnimatedTile : TiledMapTilesetTile
    {
        private readonly List<TiledMapTilesetTileAnimationFrame> _animationFrames =
            new List<TiledMapTilesetTileAnimationFrame>();

        private int _frameIndex;
        private TimeSpan _timer;

        public ReadOnlyCollection<TiledMapTilesetTileAnimationFrame> AnimationFrames { get; }
        public TiledMapTilesetTileAnimationFrame CurrentAnimationFrame { get; private set; }

        // ReSharper disable once SuggestBaseTypeForParameter
        internal TiledMapTilesetAnimatedTile(TiledMapTileset tileset, ContentReader input, int localTileIdentifier, int animationFramesCount)
            : base(localTileIdentifier,input)
        {
            AnimationFrames = new ReadOnlyCollection<TiledMapTilesetTileAnimationFrame>(_animationFrames);
            _timer = TimeSpan.Zero;

            for (var i = 0; i < animationFramesCount; i++)
            {
                var localTileIdentifierForFrame = input.ReadInt32();
                var frameDurationInMilliseconds = input.ReadInt32();
                var tileSetTileFrame = new TiledMapTilesetTileAnimationFrame(tileset, localTileIdentifierForFrame, frameDurationInMilliseconds);
                _animationFrames.Add(tileSetTileFrame);
                CurrentAnimationFrame = AnimationFrames[0];
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_animationFrames.Count == 0)
                return;

            _timer += gameTime.ElapsedGameTime;
            if (_timer <= CurrentAnimationFrame.Duration)
                return;

            _timer -= CurrentAnimationFrame.Duration;
            _frameIndex = (_frameIndex + 1) % AnimationFrames.Count;
            CurrentAnimationFrame = AnimationFrames[_frameIndex];
        }
    }
}