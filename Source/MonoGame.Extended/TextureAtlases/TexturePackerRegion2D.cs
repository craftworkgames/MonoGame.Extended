using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.TextureAtlases
{
    public class TexturePackerRegion2D : TextureRegion2D
    {

        public TextureRegion2D[] Regions2D { get; private set; }
        public Color[] RegionColors { get; private set; }

        protected TexturePackerRegion2D(Texture2D texture, int x, int y, int width, int height)
            : base(null, texture, x, y, width, height)
        {
        }

        public TexturePackerRegion2D(string name, TextureRegion2D[] regions, Color[] colors, Texture2D texture, int x, int y, int width, int height)
             : this(texture, x, y, width, height)
        {
            TextureRegion2D textureRegion = new TextureRegion2D(texture, x, y, width, height);
            if (colors.Length != regions.Length)
                throw new Exception("Cannot create TexturePackerRegions2D: regions and colors must be the same length.");

            Regions2D = regions;
            RegionColors = colors;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
        {
            for (int i = 0; i < Regions2D.Length; i++)
            {
                spriteBatch.Draw(Regions2D[i], destinationRectangle, RegionColors[i], clippingRectangle);
            }
        }

    }
}
