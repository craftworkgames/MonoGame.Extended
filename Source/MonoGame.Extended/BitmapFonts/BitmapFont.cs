using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFont 
    {
        public BitmapFont(Texture2D[] textures, BitmapFontFile fontFile)
        {
            _fontFile = fontFile;
            _characterMap = BuildCharacterMap(textures, _fontFile);
        }

        public BitmapFont(Texture2D texture, BitmapFontFile fontFile)
        {
            _fontFile = fontFile;
            _characterMap = BuildCharacterMap(new [] { texture }, _fontFile);
        }

        private readonly BitmapFontFile _fontFile;
        private readonly Dictionary<char, BitmapFontRegion> _characterMap;

        public int LineHeight
        {
            get { return _fontFile.Common.LineHeight; }
        }

        private static Dictionary<char, BitmapFontRegion> BuildCharacterMap(Texture2D[] textures, BitmapFontFile fontFile)
        {
            var characterMap = new Dictionary<char, BitmapFontRegion>();

            foreach (var fontChar in fontFile.Chars)
            {
                var pageIndex = fontChar.Page;
                var character = (char)fontChar.ID;
                var texture = textures[pageIndex];
                var region = new TextureRegion2D(texture, fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
                var fontRegion = new BitmapFontRegion(region, fontChar);
                characterMap.Add(character, fontRegion);
            }

            return characterMap;
        }

        public BitmapFontRegion GetCharacterRegion(char character)
        {
            BitmapFontRegion region;
            return _characterMap.TryGetValue(character, out region) ? region : null;
        }

        public Rectangle GetStringRectangle(string text, Vector2 position)
        {
            var width = 0;
            var height = 0;

            foreach (var c in text)
            {
                BitmapFontRegion fontRegion;

                if (_characterMap.TryGetValue(c, out fontRegion))
                {
                    var fc = fontRegion.FontCharacter;
                    width += fc.XAdvance;

                    if (fc.Height + fc.YOffset > height)
                        height = fc.Height + fc.YOffset;
                }
            }

            var p = position.ToPoint();
            return new Rectangle(p.X, p.Y, width, height);
        }
    }
}
