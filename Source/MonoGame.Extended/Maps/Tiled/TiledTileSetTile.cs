using System.Collections.Generic;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTileSetTile
    {
        public TiledTileSetTile(int id)
        {
            Id = id;
            Animation = new List<TiledTileSetTileFrame>();
            Properties = new TiledProperties();
            _currentTimeInMilliseconds = 0.0;
        }
        public int Id { get; set; }
        public List<TiledTileSetTileFrame> Animation { get; private set; }
        public TiledProperties Properties { get; private set; }
        public int? CurrentTileId
        {
            get { return (_currentFrame == null) ? null :(int?)_currentFrame.TileId; }
        }

        private double _currentTimeInMilliseconds;
        private TiledTileSetTileFrame _currentFrame;

        public TiledTileSetTileFrame CreateTileSetTileFrame(int order, int tileId, int duration)
        {
            var tileSetTileFrame = new TiledTileSetTileFrame(order, tileId, duration);
            Animation.Add(tileSetTileFrame);
            _currentFrame = Animation[0];
            return tileSetTileFrame;
        }

        public void Update(double deltaTime)
        {
            if (Animation.Count == 0) return;
            _currentTimeInMilliseconds += deltaTime;
            if (_currentTimeInMilliseconds >= _currentFrame.Duration)
            {
                _currentTimeInMilliseconds = 0.0;

                int nextFrame = (_currentFrame.Order + 1) % Animation.Count;
                _currentFrame = Animation[nextFrame];
            }
        }
    }
}