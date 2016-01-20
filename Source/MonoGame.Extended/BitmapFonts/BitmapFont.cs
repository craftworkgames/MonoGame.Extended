using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont 
    {
        internal BitmapFont(IEnumerable<BitmapFontRegion> regions, int lineHeight)
        {
            _characterMap = regions.ToDictionary(r => r.Character);// BuildCharacterMap(textures, _fontFile);
            LineHeight = lineHeight;
        }

        private readonly Dictionary<char, BitmapFontRegion> _characterMap;

        public int LineHeight { get; private set; }

        public BitmapFontRegion GetCharacterRegion(char character)
        {
            BitmapFontRegion region;
            return _characterMap.TryGetValue(character, out region) ? region : null;
        }

        public Size GetSize(string text)
        {
            var width = 0;
            var height = 0;

            foreach (var c in text)
            {
                BitmapFontRegion fontRegion;

                if (_characterMap.TryGetValue(c, out fontRegion))
                {
                    width += fontRegion.XAdvance;

                    if (fontRegion.Height + fontRegion.YOffset > height)
                        height = fontRegion.Height + fontRegion.YOffset;
                }
            }

            return new Size(width, height);
        }

        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            var size = GetSize(text);
            var p = position.ToPoint();
            return new Rectangle(p.X, p.Y, size.Width, size.Height);
        }
    }
}
