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
            Animation = new List<TiledTilesetTileFrame>();
            ObjectGroups = new List<TiledObjectGroup>();
            Properties = new TiledProperties();
            _currentTimeInMilliseconds = 0.0;
        }

        public int Id { get; set; }
        public List<TiledTilesetTileFrame> Animation { get; }
        public List<TiledObjectGroup> ObjectGroups { get; private set; }
        public TiledProperties Properties { get; private set; }

        public int? CurrentTileId => _currentFrame == null ? null : (int?) _currentFrame.TileId;

        public TiledTilesetTileFrame CreateTileSetTileFrame(int order, int tileId, int duration)
        {
            var tileSetTileFrame = new TiledTilesetTileFrame(order, tileId, duration);
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

                var nextFrame = (_currentFrame.Order + 1)%Animation.Count;
                _currentFrame = Animation[nextFrame];
            }
        }
    }
}