using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Input;
using System;
using System.Linq;

namespace MonoGame.Extended.Gui.Controls
{
    public class SliderPanel : LayoutControl
    {
        public SliderPanel()
            : base()
        {
            HorizontalAlignment = HorizontalAlignment.Centre;
            VerticalAlignment = VerticalAlignment.Centre;
            Items.ItemAdded += OnItemAdded;
        }

        private Point _pointerDown;
        private Vector2 _velocity;
        private DateTime _lastMove;
        private bool _isDown;

        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public bool IsSliding { get; private set; }

        private Point _sliderPosition;
        public Point SliderPosition
        {
            get { return _sliderPosition; }
            set
            {
                if (_sliderPosition == value)
                    return;
                _sliderPosition = value;
                if (DoesSnap)
                    DoSnap();
                IsLayoutRequired = true;
                InvalidateMeasure();
            }
        }

        public bool DoesSnap { get; set; }
        public bool PreventSlide { get; set; }

        public Size ItemSize { get; private set; }

        public override Size GetContentSize(IGuiContext context)
        {
            GuiSystem guiSystem = (GuiSystem)context;
            guiSystem.ActiveScreen.PointerUp -= SliderUp;
            guiSystem.ActiveScreen.PointerMoved -= SliderMoved;

            var width = 0;
            var height = 0;

            for (int i = 0; i < Items.Count; i++)
            {
                var control = Items[i];
                var actualSize = control.CalculateActualSize(context)+new Size(control.Margin.Width, control.Margin.Height);

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        width += actualSize.Width;
                        height = actualSize.Height > this.Height ? actualSize.Height : this.Height;
                        break;
                    case Orientation.Vertical:
                        width = actualSize.Width > this.Width ? actualSize.Width : this.Height;
                        height += actualSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }


            guiSystem.ActiveScreen.PointerUp += SliderUp;
            guiSystem.ActiveScreen.PointerMoved += SliderMoved;

            var desiredSize = new Size(Width != 0 ? Width : width, Height != 0 ? Height : height);
            //ActualSize = new Size(desiredSize.Width!=0?desiredSize.Width:width, desiredSize.Height != 0 ? desiredSize.Height : height);
            ItemSize = new Size(Orientation == Orientation.Horizontal ? width : desiredSize.Width,
                               Orientation == Orientation.Vertical ? height : desiredSize.Height);
            return new Size(desiredSize.Width, desiredSize.Height);
        }


        public void OnItemAdded(Control control)
        {
            IsLayoutRequired = true;
            AddEvents(control);
            control.PointerDown += (s, e) => OnPointerDown((IGuiContext)s, e);
            control.PointerUp += (s, e) => OnPointerUp((IGuiContext)s, e);
            control.PointerMoved += (s, e) => OnPointerMoved((IGuiContext)s, e);
            control.PointerDrag += (s, e) => OnPointerDrag((IGuiContext)s, e);

        }

        private void AddEvents(Control parentCtrl)
        {
            foreach (Control control in parentCtrl.Children)
            {
                control.PointerDown += (s, e) => OnPointerDown((IGuiContext)s, e);
                control.PointerUp += (s, e) => OnPointerUp((IGuiContext)s, e);
                control.PointerMoved += (s, e) => OnPointerMoved((IGuiContext)s, e);
                control.PointerDrag += (s, e) => OnPointerDrag((IGuiContext)s, e);

                AddEvents(control);
            }
            //parentCtrl.Children.ItemAdded += (s) => OnItemAdded(s);
        }

        protected override void Layout(IGuiContext context, Rectangle rectangle)
        {
            if (Items.Count == 0)
                return;

            float x = Orientation == Orientation.Horizontal ? SliderPosition.X : 0;
            float y = Orientation == Orientation.Vertical ? SliderPosition.Y : 0;
            x = x > 100 ? 100 : x;
            y = y > 100 ? 100 : y;

            if (x > 0 && !_isDown)
                _velocity = new Vector2((0 - x) / 10, 0);
            if (y > 0 && !_isDown)
                _velocity = new Vector2(0, (0 - y) / 10);

            int maxX = ItemSize.Width - this.ClippingRectangle.Width;
            int maxY = ItemSize.Height - this.ClippingRectangle.Height;
            if (x != 0 && -x > maxX && !_isDown)
                _velocity = new Vector2((-x - maxX) / 10, 0);
            if (y != 0 && -y > maxY && !_isDown)
                _velocity = new Vector2(0, (-y - maxY) / 10);

            _sliderPosition = new Point((int)x, (int)y);
            var desiredSize = new Size(Width == 0 ? ItemSize.Width : Width, Height == 0 ? ItemSize.Height : Height);

            for (int i = 0; i < Items.Count; i++)
            {
                var control = Items[i];
                var controlSize = control.CalculateActualSize(context);

                if (Orientation == Orientation.Vertical)
                {
                    if (HorizontalAlignment == HorizontalAlignment.Centre)
                        x = (int)(desiredSize.Width / 2 - controlSize.Width / 2);
                    else if (HorizontalAlignment == HorizontalAlignment.Right)
                        x = (int)(desiredSize.Width - desiredSize.Width);
                    else if (HorizontalAlignment == HorizontalAlignment.Stretch)
                        x = (int)(desiredSize.Width / 2 - (desiredSize.Width + control.Margin.Width) / 2);
                }
                else
                {
                    if (VerticalAlignment == VerticalAlignment.Centre)
                        y = (int)(desiredSize.Height / 2 - desiredSize.Height / 2);
                    else if (VerticalAlignment == VerticalAlignment.Bottom)
                        y = (int)(desiredSize.Height - desiredSize.Height);
                    else if (VerticalAlignment == VerticalAlignment.Stretch)
                        y = (int)(desiredSize.Height / 2 - desiredSize.Height / 2);
                }

                switch (Orientation)
                {
                    case Orientation.Vertical:

                        PlaceControl(context, control, Position.X + x , Position.Y + y , desiredSize.Width, controlSize.Height);
                        y += controlSize.Height + control.Margin.Height;
                        rectangle.Height -= controlSize.Height;
                        break;
                    case Orientation.Horizontal:
                        PlaceControl(context, control, Position.X + x , Position.Y + y + y , controlSize.Width, desiredSize.Height);
                        x += controlSize.Width + control.Margin.Width;
                        rectangle.Height -= controlSize.Height;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected orientation {Orientation}");
                }
            }
            IsLayoutRequired = false;
        }

        private void DoSnap()
        {
            if (SliderPosition == Point2.Zero)
                return;
            switch (Orientation)
            {
                case Orientation.Vertical:
                    var y = Math.Round((double)SliderPosition.Y / (-ItemSize.Height / Items.Count), 0);
                    _sliderPosition = new Point(SliderPosition.X, (int)((-ItemSize.Height / Items.Count) * y));
                    break;
                case Orientation.Horizontal:
                    var x = Math.Round((double)SliderPosition.X / (-ItemSize.Width / Items.Count), 0);
                    _sliderPosition = new Point((int)(x * (-ItemSize.Width / Items.Count)), SliderPosition.Y);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected orientation {Orientation}");
            }
            IsLayoutRequired = true;
        }

        public override void Update(IGuiContext context, float deltaSeconds)
        {
            if (_velocity != Vector2.Zero || IsLayoutRequired)
                ScrollMomentum(context);

            base.Update(context, deltaSeconds);
        }

        public override bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        {
            IsSliding = false;
            _lastMove = DateTime.Now;
            _pointerDown = args.Position;
            _isDown = true;
            return base.OnPointerDown(context, args);
        }

        public override bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        {
            SliderUp(context, args);
            return base.OnPointerUp(context, args);
        }

        public override bool OnPointerMoved(IGuiContext context, PointerEventArgs args)
        {
            SliderMoved(context, args);
            return base.OnPointerDrag(context, args);
        }

        private void SliderMoved(object sender, PointerEventArgs args)
        {
            if (args.Button == MouseButton.Left && !PreventSlide && _isDown)
            {
                //Dont allow sliding if our content already fits
                if (Orientation == Orientation.Vertical && this.ActualSize.Height >= ItemSize.Height)
                    return;
                if (Orientation == Orientation.Horizontal && this.ActualSize.Width >= ItemSize.Width)
                    return;

                var deltaMove = args.Position - _pointerDown;
                if (args.Position.X == int.MinValue)
                    return;

                if (deltaMove.X == 0 && deltaMove.Y == 0)
                    return;
                if (Math.Abs(deltaMove.X) > 5 || Math.Abs(deltaMove.Y) > 5)
                {
                    IsSliding = true;
                }
                var slide = _sliderPosition + deltaMove;

                //Dont allow sliding too far past either end
                if (Orientation == Orientation.Vertical && (slide.Y > 100 || slide.Y < this.ClippingRectangle.Height - ItemSize.Height - 100))
                    return;
                if (Orientation == Orientation.Horizontal && (slide.X > 100 || slide.X < this.ClippingRectangle.Width - ItemSize.Width - 100))
                    return;

                SliderPosition = slide;

                float deltaTime = (float)(DateTime.Now - _lastMove).TotalSeconds;
                _velocity = new Vector2(deltaMove.X / deltaTime / 10, deltaMove.Y / deltaTime / 10);

                _lastMove = DateTime.Now;
                _pointerDown = args.Position;
            }

        }



        private void SliderUp(object sender, PointerEventArgs args)
        {
            _isDown = false;
            _pointerDown = Point.Zero;
            IsLayoutRequired = true;
            if (DoesSnap)
                DoSnap();
        }

        public override bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (!this.BoundingRectangle.Contains(args.Position))
                _isDown = false;
            return base.OnPointerLeave(context, args);
        }

        public void ScrollMomentum(IGuiContext context)
        {
            if (!_isDown)
            {
                if (Math.Abs(_velocity.X) > 0 || Math.Abs(_velocity.Y) > 0)
                {
                    Point moveAmount = new Point(_velocity.X > 10 ? 10 : (int)_velocity.X, _velocity.Y > 10 ? 10 : (int)_velocity.Y);
                    moveAmount = new Point(_velocity.X < -10 ? -10 : (int)_velocity.X, _velocity.Y < -10 ? -10 : (int)_velocity.Y);
                    _sliderPosition = new Point((int)(SliderPosition.X + moveAmount.X), (int)(SliderPosition.Y + moveAmount.Y));
                    _velocity *= 0.85f;
                };
            }

            Layout(context, ContentRectangle);

            if (Math.Abs(_velocity.X) < 1 && Math.Abs(_velocity.Y) < 1)
                _velocity = Vector2.Zero;
        }


        public override void InvalidateMeasure()
        {
            IsLayoutRequired = true;
            base.InvalidateMeasure();
        }

    }
}
