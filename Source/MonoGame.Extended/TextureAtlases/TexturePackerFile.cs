using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonoGame.Extended.TextureAtlases
{
    public class TexturePackerFile
    {
        [JsonProperty("frames")]
        public List<TexturePackerRegion> Regions { get; set; }

        [JsonProperty("meta")]
        public TexturePackerMeta Metadata { get; set; }
    }

    //public class TexturePackerToTextureAtlasConverter
    //{
    //    public TextureAtlas Convert(TexturePackerFile texturePackerFile)
    //    {
    //        var name = texturePackerFile.Metadata.Image;
    //        var textureAtlas = new TextureAtlas(name, texture);

    //        foreach (var region in texturePackerFile.Regions)
    //        {
    //            textureAtlas.CreateRegion(region.Filename, region.Frame.X, region.Frame.Y, region.Frame.Width, region.Frame.Height);
    //        }

    //        return textureAtlas;
    //    }
    //}
}