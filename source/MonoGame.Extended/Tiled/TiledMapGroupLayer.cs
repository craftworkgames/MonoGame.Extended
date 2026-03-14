using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
	[Obsolete("The MonoGame.Extended.Tiled namespace is deprecated. Use MonoGame.Extended.Tilemaps instead. This will be removed in the next major SemVer release.")]
	public class TiledMapGroupLayer : TiledMapLayer
	{
		public List<TiledMapLayer> Layers { get; }
		public TiledMapGroupLayer(string name, string type, List<TiledMapLayer> layers, Vector2? offset = null, Vector2? parallaxFactor = null, float opacity = 1, bool isVisible = true)
			: base(name, type, offset, parallaxFactor, opacity, isVisible)
		{
			Layers = layers;
		}
	}
}
