namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButtonStyle : GuiControlStyle
    {
        public GuiSpriteStyle Hovered { get; set; }
        public GuiSpriteStyle Disabled { get; set; }
        public GuiSpriteStyle Pressed { get; set; }

        public override GuiControl CreateControl(GuiContentService contentService)
        {
            return new GuiButton(contentService, this);
        }
    }
}