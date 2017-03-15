using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer : IDisposable
    {
        internal TiledMapLayerModel[] Models;
        internal TiledMapLayerAnimatedModel[] AnimatedModels;

        public string Name { get; }
        public TiledMapProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float OffsetX { get; }
        public float OffsetY { get; }

        internal TiledMapLayer(ContentReader input)
        {
            Models = null;
            AnimatedModels = null;

            Name = input.ReadString();
            Properties = new TiledMapProperties();
            IsVisible = input.ReadBoolean();
            Opacity = input.ReadSingle();
            OffsetX = input.ReadSingle();
            OffsetY = input.ReadSingle();

            input.ReadTiledMapProperties(Properties);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool diposing)
        {
            if (!diposing)
                return;
            if (Models == null)
                return;
            foreach (var model in Models)
                model.Dispose();
        }
    }
}