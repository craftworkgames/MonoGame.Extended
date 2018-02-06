using System;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled.Graphics;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer : IDisposable
    {
        internal TiledMapLayerModel[] Models;
        internal TiledMapLayerAnimatedModel[] AnimatedModels;

        public string Name { get; }
        public TiledMapGroupLayer Parent { get; }
        public TiledMapProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float OffsetX
        {
            get
            {
                return (Parent?.OffsetX ?? 0) + _offsetX;
            }
        }
        public float OffsetY
        {
            get
            {
                return (Parent?.OffsetY ?? 0) + _offsetY;
            }
        }

        private float _offsetX;
        private float _offsetY;

        internal TiledMapLayer(ContentReader input, TiledMapGroupLayer parent)
        {
            Models = null;
            AnimatedModels = null;

            Name = input.ReadString();
            Properties = new TiledMapProperties();
            IsVisible = input.ReadBoolean();
            Opacity = input.ReadSingle();
            _offsetX = input.ReadSingle();
            _offsetY = input.ReadSingle();
            Parent = parent;

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