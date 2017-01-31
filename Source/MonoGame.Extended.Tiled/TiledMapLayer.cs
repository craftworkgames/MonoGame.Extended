using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled.Graphics;

namespace MonoGame.Extended.Tiled
{
    public abstract class TiledMapLayer : IDisposable
    {
        internal readonly List<TiledMapLayerModel> Models;
        internal readonly List<TiledMapLayerAnimatedModel> AnimatedModels;

        public string Name { get; }
        public TiledMapProperties Properties { get; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }
        public float OffsetX { get; }
        public float OffsetY { get; }

        internal TiledMapLayer(ContentReader input)
        {
            Models = new List<TiledMapLayerModel>();
            AnimatedModels = new List<TiledMapLayerAnimatedModel>();

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
            foreach (var model in Models)
                model.Dispose();
        }

        internal void AddModel(TiledMapLayerModel model)
        {
            Models.Add(model);

            var animatedModel = model as TiledMapLayerAnimatedModel;
            if (animatedModel != null)
                AnimatedModels.Add(animatedModel);
        }
    }
}