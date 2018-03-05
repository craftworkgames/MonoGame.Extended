using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class Binding
    {
        public Binding(object viewModel, string viewModelProperty, string viewProperty)
        {
            ViewModel = viewModel;
            ViewModelProperty = viewModelProperty;
            ViewProperty = viewProperty;
        }

        public object ViewModel { get; }
        public string ViewModelProperty { get; }
        public string ViewProperty { get; }
    }

    public abstract class Element
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public Color BorderColor { get; set; } = Color.White;
        public int BorderThickness { get; set; } = 0;

        private TextureRegion2D _backgroundRegion;
        public TextureRegion2D BackgroundRegion
        {
            get { return _backgroundRegion; }
            set
            {
                _backgroundRegion = value;

                if (_backgroundRegion != null && Color == Color.Transparent)
                    Color = Color.White;
            }
        }

        public List<Binding> Bindings { get; } = new List<Binding>();

        protected void OnPropertyChanged(string propertyName)
        {
            foreach (var binding in Bindings)
            {
                if (binding.ViewProperty == propertyName)
                {
                    var value = GetType()
                        .GetTypeInfo()
                        .GetDeclaredProperty(binding.ViewProperty)
                        .GetValue(this);

                    binding.ViewModel
                        .GetType()
                        .GetTypeInfo()
                        .GetDeclaredProperty(binding.ViewModelProperty)
                        .SetValue(binding.ViewModel, value);
                }
            }
        }

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

        protected virtual void OnSizeChanged() {}

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

    public abstract class Element<TParent> : Element, IRectangular
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
