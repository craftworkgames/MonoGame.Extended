using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.BitmapFonts
{

    public class BitmapFontContentLoader : ContentLoader<BitmapFont>
    {
        public override BitmapFont LoadContentFromStream(ContentManager contentManager, Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(FontFile));
            var fontFile = (FontFile) deserializer.Deserialize(stream);
            var fileName = fontFile.Pages[0].File;
            var dotIndex = fileName.LastIndexOf('.');
            var assetName = dotIndex > 0 ? fileName.Substring(0, dotIndex) : fileName;
            var texture = contentManager.Load<Texture2D>(assetName);
            return new BitmapFont(texture, fontFile);
        }
    }
}
