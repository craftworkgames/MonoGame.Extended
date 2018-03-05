using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class Image : Control
    {
        public Image()
        {
        }

        public Image(TextureRegion2D image)
        {
            BackgroundRegion = image;
        }
    }
}