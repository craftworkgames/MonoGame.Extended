using System.Collections.Generic;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTilesetTile
    {
        private TiledTilesetTileFrame _currentFrame;
        private double _currentTimeInMilliseconds;

        public TiledTilesetTile(int id)
        {
            Id = id;
            Frames = new List<TiledTilesetTileFrame>();
            Properties = new TiledProperties();

            _currentTimeInMilliseconds = 0.0;
        }

        public int Id { get; set; }
        public List<TiledTilesetTileFrame> Frames { get; }
        public TiledProperties Properties { get; private set; }

        public int? CurrentTileId => _currentFrame?.TileId;

        public TiledTilesetTileFrame CreateTileSetTileFrame(int order, int tileId, int duration)
        {
            var tileSetTileFrame = new TiledTilesetTileFrame(order, tileId, duration);
            Frames.Add(tileSetTileFrame);
            _currentFrame = Frames[0];
            return tileSetTileFrame;
        }

        public void Update(double deltaTime)
        {
            if (Frames.Count == 0)
                return;

            _currentTimeInMilliseconds += deltaTime;

            if (_currentTimeInMilliseconds >= _currentFrame.Duration)
            {
                _currentTimeInMilliseconds = 0.0;

                var nextFrame = (_currentFrame.Order + 1)%Frames.Count;
                _currentFrame = Frames[nextFrame];
            }
        }
    }
}