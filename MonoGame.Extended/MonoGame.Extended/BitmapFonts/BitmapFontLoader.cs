using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontLoader : ContentTypeReader<BitmapFont>
    {
        public BitmapFont Load(ContentManager contentManager, string assetPath)
        {
            using (var stream = TitleContainer.OpenStream("Content\\" + assetPath))
            {
                var deserializer = new XmlSerializer(typeof(FontFile));
                var fontFile = (FontFile)deserializer.Deserialize(stream);
                var texture = contentManager.Load<Texture2D>(fontFile.Pages[0].File);
                return new BitmapFont(assetPath, texture, fontFile);
            }
        }

        protected override BitmapFont Read(ContentReader input, BitmapFont existingInstance)
        {
            throw new System.NotImplementedException();
        }
    }
}
