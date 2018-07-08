using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
	public class TiledMapGroupLayer : TiledMapLayer
	{
		public List<TiledMapLayer> Layers { get; }
		public TiledMapGroupLayer(string name, List<TiledMapLayer> layers, Vector2? offset = null, float opacity = 1, bool isVisible = true) 
			: base(name, offset, opacity, isVisible)
		{
			Layers = layers;
		}
	}
}
