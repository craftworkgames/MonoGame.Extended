using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

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
        public Point Position { get; set; }
        public Point Origin { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; } = Color.White;
        public int BorderThickness { get; set; } = 0;

        private Texture2DRegion _backgroundRegion;
        public Texture2DRegion BackgroundRegion
        {
            get => _backgroundRegion;
            set
            {
                _backgroundRegion = value;

                if (_backgroundRegion != null && BackgroundColor == Color.Transparent)
                    BackgroundColor = Color.White;
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

        private Size _size;
        public Size Size
        {
            get => _size;
            set
            {
                _size = value;
                OnSizeChanged();
            }
        }

        protected virtual void OnSizeChanged() { }

        public int MinWidth { get; set; }
        public int MinHeight { get; set; }
        public int MaxWidth { get; set; } = int.MaxValue;
        public int MaxHeight { get; set; } = int.MaxValue;

        public int Width
        {
            get => Size.Width;
            set => Size = new Size(value, Size.Height);
        }

        public int Height
        {
            get => Size.Height;
            set => Size = new Size(Size.Width, value);
        }

        public Size ActualSize { get; internal set; }

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
                var offset = Point.Zero;

                if (Parent != null)
                    offset = Parent.BoundingRectangle.Location;

                var position = offset + Position - ActualSize * Origin;
                return new Rectangle(position.X, position.Y, ActualSize.Width, ActualSize.Height);
            }
        }
    }
}
