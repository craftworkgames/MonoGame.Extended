using System.Collections.Generic;

namespace MonoGame.Extended.Gui.Controls
{
    public enum GuiButtonState
    {
        Normal,
        Pressed,
        Disabled,
        Hovered
    }

    public class GuiButton : GuiControl
    {
        private readonly GuiButtonStyle _style;
        private readonly Dictionary<GuiButtonState, GuiSpriteStyle> _stateStyleMap;

        public GuiButton(GuiContentService contentService, GuiButtonStyle style)
            : base(contentService, style)
        {
            _style = style;
            _stateStyleMap = new Dictionary<GuiButtonState, GuiSpriteStyle>
            {
                {GuiButtonState.Normal, _style.Normal},
                {GuiButtonState.Hovered, _style.Hovered},
                {GuiButtonState.Pressed, _style.Pressed},
                {GuiButtonState.Disabled, _style.Disabled},
            };
        }

        public override void OnMouseMove()
        {
            ApplySpriteStyle(_style.Hovered);
        }

        private GuiButtonState _state;
        public GuiButtonState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    ApplySpriteStyle(_stateStyleMap[_state]);
                }
            }
        }
    }
}