using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui
{
    public interface IGuiContentService
    {
        TextureRegion2D GetTextureRegion(string name);
    }

    public class GuiContentService : IGuiContentService
    {
        private readonly ContentManager _contentManager;

        public GuiContentService(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public TextureRegion2D GetTextureRegion(string name)
        {
            var texture = _contentManager.Load<Texture2D>(name);
            return new TextureRegion2D(texture);
        }
    }
}