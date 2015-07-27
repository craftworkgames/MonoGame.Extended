using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader input, BitmapFont existingInstance)
        {
            var textureAssetCount = input.ReadInt32();
            var assets = new List<string>();

            for (var i = 0; i < textureAssetCount; i++)
            {
                var assetName = input.ReadString();
                assets.Add(assetName);
            }

            var json = input.ReadString();
            var fontFile = JsonConvert.DeserializeObject<BitmapFontFile>(json);
            var textures = assets
                .Select(i => input.ContentManager.Load<Texture2D>(i))
                .ToArray();
            return new BitmapFont(textures, fontFile);
        }
    }
}