using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiElement
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public TextureRegion2D BackgroundRegion { get; set; }

        private Size2 _size;
        public Size2 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnSizeChanged();
            }
        }

        private object _bindingContext;
        public virtual object BindingContext
        {
            get { return _bindingContext; }
            set
            {
                if (_bindingContext != value)
                {
                    _bindingContext = value;
                    OnBindingContextChanged();
                }
            }
        }

        private void OnBindingContextChanged()
        {
            UpdateElementBindings();

            var viewModel = BindingContext as INotifyPropertyChanged;

            if (viewModel != null)
            {
                viewModel.PropertyChanged += (sender, args) =>
                {
                    UpdateElementBindings();
                };
            }
        }

        protected virtual void OnSizeChanged() {}

        private void UpdateElementBindings()
        {
            foreach (var binding in _bindings)
            {
                var sourceValue = _bindingContext
                    .GetType()
                    .GetRuntimeProperty(binding.ViewModelProperty)
                    .GetValue(_bindingContext);

                var targetProperty = binding.Element
                    .GetType()
                    .GetRuntimeProperty(binding.ViewProperty);

                targetProperty.SetValue(binding.Element, sourceValue);
            }
        }

        protected struct Binding
        {
            public GuiElement Element;
            public string ViewProperty;
            public string ViewModelProperty;
        }

        protected readonly List<Binding> _bindings = new List<Binding>();

        public virtual void SetBinding(string viewProperty, string viewModelProperty)
        {
            _bindings.Add(new Binding
            {
                Element = this,
                ViewProperty = viewProperty,
                ViewModelProperty = viewModelProperty
            });
        }

        public float Width
        {
            get { return Size.Width; }
            set { Size = new Size2(value, Size.Height); }
        }

        public float Height
        {
            get { return Size.Height; }
            set { Size = new Size2(Size.Width, value); }
        }

        public abstract void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds);
    }

    public abstract class GuiElement<TParent> : GuiElement, IRectangular
        where TParent : IRectangular
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public TParent Parent { get; internal set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public Rectangle BoundingRectangle
        {
            get
            {
                var offset = Vector2.Zero;

                if (Parent != null)
                    offset = Parent.BoundingRectangle.Location.ToVector2();

                return new Rectangle((offset + Position - Size * Origin).ToPoint(), (Point)Size);
            }
        }
    }
}
