using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiNinePatchRegion2DJsonConverter : NinePatchRegion2DJsonConverter
    {
        private readonly IGuiTextureRegionService _textureRegionService;

        public GuiNinePatchRegion2DJsonConverter(IGuiTextureRegionService textureRegionService)
            : base(textureRegionService)
        {
            _textureRegionService = textureRegionService;
        }
    }
}
