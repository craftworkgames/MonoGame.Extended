using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentImporter(".aa", DefaultProcessor = "AstridAnimatorProcessor", 
        DisplayName = "Astrid Animator Importer - MonoGame.Extended")]
    public class AstridAnimatorImporter : JsonContentImporter<AstridAnimatorFile>
    {
    }
}