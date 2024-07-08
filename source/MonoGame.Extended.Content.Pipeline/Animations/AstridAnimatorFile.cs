using System.Collections.Generic;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    public class AstridAnimatorFile
    {
        public string TextureAtlas { get; set; }
        public List<AstridAnimatorAnimation> Animations { get; set; }

        public AstridAnimatorFile()
        {
            Animations = new List<AstridAnimatorAnimation>();
        }
    }
}