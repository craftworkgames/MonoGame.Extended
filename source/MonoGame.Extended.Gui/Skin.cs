using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;

namespace MonoGame.Extended.Gui
{
    public class Skin
    {
        public Skin()
        {
            TextureAtlases = new List<Texture2DAtlas>();
            Fonts = new List<BitmapFont>();
            NinePatches = new List<NinePatch>();
            Styles = new KeyedCollection<string, ControlStyle>(s => s.Name ?? s.TargetType.Name);
        }

        [JsonPropertyOrder(0)]
        public string Name { get; set; }

        [JsonPropertyOrder(1)]
        public IList<Texture2DAtlas> TextureAtlases { get; set; }

        [JsonPropertyOrder(2)]
        public IList<BitmapFont> Fonts { get; set; }

        [JsonPropertyOrder(3)]
        public IList<NinePatch> NinePatches { get; set; }

        [JsonPropertyOrder(4)]
        public BitmapFont DefaultFont => Fonts.FirstOrDefault();

        [JsonPropertyOrder(5)]
        public Cursor Cursor { get; set; }

        [JsonPropertyOrder(6)]
        public KeyedCollection<string, ControlStyle> Styles { get; private set; }

        public ControlStyle GetStyle(string name)
        {
            if (Styles.TryGetValue(name, out var controlStyle))
                return controlStyle;

            return null;
        }

        public ControlStyle GetStyle(Type controlType)
        {
            return GetStyle(controlType.FullName);
        }

        public void Apply(Control control)
        {
            // TODO: This allocates memory on each apply because it needs to apply styles in reverse
            var types = new List<Type>();
            var controlType = control.GetType();

            while (controlType != null)
            {
                types.Add(controlType);
                controlType = controlType.GetTypeInfo().BaseType;
            }

            for (var i = types.Count - 1; i >= 0; i--)
            {
                var style = GetStyle(types[i]);
                style?.Apply(control);
            }
        }

        public static Skin FromFile(ContentManager contentManager, string path, params Type[] customControlTypes)
        {
            using (var stream = TitleContainer.OpenStream(path))
            {
                return FromStream(contentManager, stream, customControlTypes);
            }
        }

        public static Skin FromStream(ContentManager contentManager, Stream stream, params Type[] customControlTypes)
        {
            var options = GuiJsonSerializerOptionsProvider.GetOptions(contentManager, customControlTypes);
            return JsonSerializer.Deserialize<Skin>(stream, options);
        }


        public T Create<T>(string template, Action<T> onCreate)
            where T : Control, new()
        {
            var control = new T();
            GetStyle(template).Apply(control);
            onCreate(control);
            return control;
        }

        public Control Create(Type type, string template)
        {
            var control = (Control)Activator.CreateInstance(type);

            if (template != null)
            {
                var style = GetStyle(template);
                if (style != null)
                    style.Apply(control);
                else
                    throw new FormatException($"invalid style {template} for control {type.Name}");
            }

            return control;
        }

        public static Skin Default { get; set; }

        public static Skin CreateDefault(BitmapFont font)
        {
            Default = new Skin
            {
                Fonts = { font },
                Styles =
                {
                    new ControlStyle(typeof(Control)) {
                        {nameof(Control.BackgroundColor), new Color(51, 51, 55)},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                        {nameof(Control.BorderThickness), 1},
                        {nameof(Control.TextColor), new Color(241, 241, 241)},
                        {nameof(Control.Padding), new Thickness(5)},
                        {nameof(Control.DisabledStyle), new ControlStyle(typeof(Control)) {
                                { nameof(Control.TextColor), new Color(78,78,80) }
                            }
                        }
                    },
                    new ControlStyle(typeof(LayoutControl)) {
                        {nameof(Control.BackgroundColor), Color.Transparent},
                        {nameof(Control.BorderColor), Color.Transparent },
                        {nameof(Control.BorderThickness), 0},
                        {nameof(Control.Padding), new Thickness(0)},
                        {nameof(Control.Margin), new Thickness(0)},
                    },
                    new ControlStyle(typeof(ComboBox)) {
                        {nameof(ComboBox.DropDownColor), new Color(71, 71, 75)},
                        {nameof(ComboBox.SelectedItemColor), new Color(0, 122, 204)},
                        {nameof(ComboBox.HorizontalTextAlignment), HorizontalAlignment.Left }
                    },
                    new ControlStyle(typeof(CheckBox))
                    {
                        {nameof(CheckBox.HorizontalTextAlignment), HorizontalAlignment.Left },
                        {nameof(CheckBox.BorderThickness), 0},
                        {nameof(CheckBox.BackgroundColor), Color.Transparent},
                    },
                    new ControlStyle(typeof(ListBox))
                    {
                        {nameof(ListBox.SelectedItemColor), new Color(0, 122, 204)},
                        {nameof(ListBox.HorizontalTextAlignment), HorizontalAlignment.Left }
                    },
                    new ControlStyle(typeof(Label)) {
                        {nameof(Label.BackgroundColor), Color.Transparent},
                        {nameof(Label.TextColor), Color.White},
                        {nameof(Label.BorderColor), Color.Transparent},
                        {nameof(Label.BorderThickness), 0},
                        {nameof(Label.HorizontalTextAlignment), HorizontalAlignment.Left},
                        {nameof(Label.VerticalTextAlignment), VerticalAlignment.Bottom},
                        {nameof(Control.Margin), new Thickness(5,0)},
                        {nameof(Control.Padding), new Thickness(0)},
                    },
                    new ControlStyle(typeof(TextBox)) {
                        {nameof(Control.BackgroundColor), Color.DarkGray},
                        {nameof(Control.TextColor), Color.Black},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                        {nameof(Control.BorderThickness), 2},
                    },
                    new ControlStyle(typeof(TextBox2)) {
                        {nameof(Control.BackgroundColor), Color.DarkGray},
                        {nameof(Control.TextColor), Color.Black},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                        {nameof(Control.BorderThickness), 2},
                    },
                    new ControlStyle(typeof(Button)) {
                        {
                            nameof(Button.HoverStyle), new ControlStyle {
                                {nameof(Button.BackgroundColor), new Color(62, 62, 64)},
                                {nameof(Button.BorderColor), Color.WhiteSmoke }
                            }
                        },
                        {
                            nameof(Button.PressedStyle), new ControlStyle {
                                {nameof(Button.BackgroundColor), new Color(0, 122, 204)}
                            }
                        }
                    },
                    new ControlStyle(typeof(ToggleButton)) {
                        {
                            nameof(ToggleButton.CheckedStyle), new ControlStyle {
                                {nameof(Button.BackgroundColor), new Color(0, 122, 204)}
                            }
                        },
                        {
                            nameof(ToggleButton.CheckedHoverStyle), new ControlStyle {
                                {nameof(Button.BorderColor), Color.WhiteSmoke}
                            }
                        }
                    },
                    new ControlStyle(typeof(ProgressBar)) {
                        {nameof(ProgressBar.BarColor), new Color(0, 122, 204) },
                        {nameof(ProgressBar.Height), 32 },
                        {nameof(ProgressBar.Padding), new Thickness(5, 4)},
                    }
                }
            };
            return Default;
        }
    }
}
