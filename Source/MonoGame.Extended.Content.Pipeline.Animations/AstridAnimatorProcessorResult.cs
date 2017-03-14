using System.Collections.Generic;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    public class AstridAnimatorProcessorResult
    {
        public string TextureAtlasAssetName { get; private set; }
        public string Directory { get; private set; }
        public AstridAnimatorFile Data { get; private set; }
        public List<string> Frames { get; private set; }

        public AstridAnimatorProcessorResult(string directory, AstridAnimatorFile data, IEnumerable<string> frames)
        {
            Directory = directory;
            Data = data;
            Frames = new List<string>(frames);
            TextureAtlasAssetName = Path.GetFileNameWithoutExtension(data.TextureAtlas);
        }
    }
}