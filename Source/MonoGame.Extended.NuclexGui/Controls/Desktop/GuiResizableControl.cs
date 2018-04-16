using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended;
using System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace MonoGame.Extended.NuclexGui.Controls.Desktop
{
    [Flags]
    public enum ResizingSides
    {
        None = 0,
        Top = 1,
        Right = 2,
        TopRight = 3,
        Bottom = 4,
        BottomRight = 6,
        Left = 8,
        TopLeft = 9,
        BottomLeft = 12,

    }

    [Flags]
    public enum ResizingDirections
    {
        None = 0,
        Vertical = 1,
        Horizontal = 2,
        Both = 3
    }

    // Properties:
    //   Boundaries (for constraining a control to a region)
    //   Resizable (turn moveability on or off)

    /// <summary>Control the user can drag around with the mouse</summary>
    public abstract class GuiResizableControl : GuiDraggableControl
    {
        /// <summary>Which sides are being resized</summary>
        private ResizingSides _resizingSides = ResizingSides.None;

        /// <summary>Which Directions are being resized/summary>
        private ResizingDirections _resizingDirections = ResizingDirections.None;

        /// <summary>Whether the control is currently being dragged</summary>
        private bool _beingResized;

        /// <summary>Whether the control can be dragged</summary>
        private bool _enableResizing;

        /// <summary>X coordinate at which the control was grabbed</summary>
        private float _grabX;

        /// <summary>Y coordinate at which the control was grabbed</summary>
        private float _grabY;

        /// <summary>Width of Border</summary>
        private float _borderThickness = 8;

        /// <summary>Initializes a new draggable control</summary>
        public GuiResizableControl()
        {
            EnableResizing = true;
        }

        /// <summary>Initializes a new draggable control</summary>
        /// <param name="canGetFocus">Whether the control can obtain the input focus</param>
        public GuiResizableControl(bool canGetFocus) : base(canGetFocus)
        {
            EnableResizing = true;
        }



        /// <summary>Whether the control can be dragged with the mouse</summary>
        protected bool EnableResizing
        {
            get { return _enableResizing; }
            set
            {
                _enableResizing = value;
                _beingResized &= value;
            }
        }

        protected override void OnMouseLeft(float x, float y)
        {
            if (_beingResized)
            {
                ResizeTo(x, y);
            }
            else
            {
                base.OnMouseLeft(x, y);
                _resizingSides = ResizingSides.None;
                SetCursorForResize(_resizingSides);
            }

        }

        /// <summary>Called when the mouse position is updated</summary>
        /// <param name="x">X coordinate of the mouse cursor on the GUI</param>
        /// <param name="y">Y coordinate of the mouse cursor on the GUI</param>
        protected override void OnMouseMoved(float x, float y)
        {
            base.OnMouseMoved(x, y);
            if (_beingResized)
            {
                ResizeTo(x, y);
            }
            else
            {
                // Remember the current mouse position so we know where the user picked
                // up the control when a drag operation begins
                _grabX = x;
                _grabY = y;
            }

            if (IsMouseOnBorder(x, y) && !_beingResized && _enableResizing)
            {
                SetResizingDirection();
            }
            else if (_resizingSides != ResizingSides.None && !_beingResized)
            {
                _resizingSides = ResizingSides.None;
                SetCursorForResize(_resizingSides);
            }
        }

        private void ResizeTo(float x, float y)
        {
            var OldBounds = Bounds;
            var diffX = x - _grabX;
            var diffY = y - _grabY;
            var boundsX = Bounds.Size.X;
            var boundsY = Bounds.Size.Y;
            var locX = Bounds.Location.X;
            var locY = Bounds.Location.Y;

            if (_resizingSides.HasFlag(ResizingSides.Right))
            {
                boundsX = new UniScalar(boundsX.Fraction, boundsX.Offset + diffX);
            }
            if (_resizingSides.HasFlag(ResizingSides.Left))
            {
                boundsX = new UniScalar(boundsX.Fraction, boundsX.Offset - diffX);
                locX = new UniScalar(locX.Fraction, locX.Offset + diffX);
            }
            if (_resizingSides.HasFlag(ResizingSides.Bottom))
            {
                boundsY = new UniScalar(boundsY.Fraction, boundsY.Offset + diffY);
            }
            if (_resizingSides.HasFlag(ResizingSides.Top))
            {
                boundsY = new UniScalar(boundsY.Fraction, boundsY.Offset - diffY);
                locY = new UniScalar(locY.Fraction, locY.Offset + diffY);
            }

            Bounds = new UniRectangle(new UniVector(locX, locY), new UniVector(boundsX, boundsY));

            if (GetAbsoluteBounds().Size.Width < 20 || GetAbsoluteBounds().Size.Height < 20) //Too Small
            {
                Bounds = OldBounds;
            }
            else
            {
                if (_resizingSides.HasFlag(ResizingSides.Right))
                    _grabX = x;
                if (_resizingSides.HasFlag(ResizingSides.Bottom))
                    _grabY = y;
                OnResize(OldBounds);
            }


        }

        public virtual void OnResize(UniRectangle oldBounds)
        {

        }

        public virtual void SetCursorForResize(ResizingSides resizingSides)
        {
            //No Default behavior
        }

        private void SetResizingDirection()
        {
            var _oldResizingSides = _resizingSides;
            _resizingDirections = ResizingDirections.None;
            _resizingSides = ResizingSides.None;
            if (_grabX >= GetAbsoluteBounds().Size.Width - _borderThickness)
            {
                _resizingDirections |= ResizingDirections.Horizontal;
                _resizingSides |= ResizingSides.Right;
            }
            if (_grabX <= _borderThickness)
            {
                _resizingDirections |= ResizingDirections.Horizontal;
                _resizingSides |= ResizingSides.Left;
            }
            if (_grabY >= GetAbsoluteBounds().Size.Height - _borderThickness)
            {
                _resizingDirections |= ResizingDirections.Vertical;
                _resizingSides |= ResizingSides.Bottom;
            }
            if (_grabY <= _borderThickness)
            {
                _resizingDirections |= ResizingDirections.Vertical;
                _resizingSides |= ResizingSides.Top;
            }
            if (_oldResizingSides != _resizingSides)
            {
                SetCursorForResize(_resizingSides);
            }
        }

        private bool IsMouseOnBorder(float x, float y)
        {
            var absoluteSize = GetAbsoluteBounds().Size;
            return x < _borderThickness || y < _borderThickness || x > absoluteSize.Width - _borderThickness || y > absoluteSize.Height - _borderThickness;
        }

        /// <summary>Called when a mouse button has been pressed down</summary>
        /// <param name="button">Index of the button that has been pressed</param>
        protected override void OnMousePressed(MouseButton button)
        {
            if (button == MouseButton.Left && IsMouseOnBorder(_grabX, _grabY))
            {
                _beingResized = EnableResizing;
            }
            else
            {
                base.OnMousePressed(button);
            }
        }

        /// <summary>Called when a mouse button has been released again</summary>
        /// <param name="button">Index of the button that has been released</param>
        protected override void OnMouseReleased(MouseButton button)
        {
            base.OnMouseReleased(button);
            if (button == MouseButton.Left)
                _beingResized = false;
        }
    }
}