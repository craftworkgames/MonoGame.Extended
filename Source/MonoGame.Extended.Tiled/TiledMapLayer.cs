using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled.Graphics;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer : IDisposable
    {
        internal TiledMapLayerModel[] Models;
        //internal TiledMapLayerAnimatedModel[] AnimatedModels;

        public string Name { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public TiledMapProperties Properties { get; }

        protected TiledMapLayer(string name, Vector2? offset = null, float opacity = 1.0f, bool isVisible = true)
        {
            Name = name;
            Offset = offset ?? Vector2.Zero;
            Opacity = opacity;
            IsVisible = isVisible;
            Properties = new TiledMapProperties();
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