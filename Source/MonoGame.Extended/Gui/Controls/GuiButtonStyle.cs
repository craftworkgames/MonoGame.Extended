namespace MonoGame.Extended.Gui.Controls
{
    public class GuiButtonStyle : GuiControlStyle
    {
        public GuiButtonStyle()
        {
            Hovered = new GuiSpriteStyle();
            Disabled = new GuiSpriteStyle();
            Pressed = new GuiSpriteStyle();
        }

        public GuiSpriteStyle Hovered { get; set; }
        public GuiSpriteStyle Disabled { get; set; }
        public GuiSpriteStyle Pressed { get; set; }

        public override GuiControl CreateControl(IGuiContentService contentService)
        {
            return new GuiButton(contentService, this);
        }
    }
}