using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
        {
            var textureAssetCount = reader.ReadInt32();
            var assets = new List<string>();

            for (var i = 0; i < textureAssetCount; i++)
            {
                var assetName = reader.ReadString();
                assets.Add(assetName);
            }

            var json = reader.ReadString();
            var fontFile = JsonConvert.DeserializeObject<BitmapFontFile>(json);
            var textures = assets
                .Select(i => reader.ContentManager.Load<Texture2D>(i))
                .ToArray();
            return new BitmapFont(textures, fontFile);
        }
    }
}