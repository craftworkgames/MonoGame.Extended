namespace MonoGame.Extended.Tiled.Renderers
{
    public class TiledMapModel
    {
        public TiledMapModel(TiledMapLayerModel[] layers)
        {
            Layers = layers;
        }

        public TiledMapLayerModel[] Layers { get; }
    }
}