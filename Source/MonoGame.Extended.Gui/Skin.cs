using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Extended.BitmapFonts;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class Skin
    {
        public Skin()
        {
            TextureAtlases = new List<TextureAtlas>();
            Fonts = new List<BitmapFont>();
            NinePatches = new List<NinePatchRegion2D>();
            Styles = new KeyedCollection<string, ControlStyle>(s => s.Name ?? s.TargetType.Name);
        }

        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        [JsonProperty(Order = 1)]
        public IList<TextureAtlas> TextureAtlases { get; set; }

        [JsonProperty(Order = 2)]
        public IList<BitmapFont> Fonts { get; set; }

        [JsonProperty(Order = 3)]
        public IList<NinePatchRegion2D> NinePatches { get; set; }

        [JsonProperty(Order = 4)]
        public BitmapFont DefaultFont => Fonts.FirstOrDefault();

        [JsonProperty(Order = 5)]
        public Cursor Cursor { get; set; }

        [JsonProperty(Order = 6)]
        public KeyedCollection<string, ControlStyle> Styles { get; private set; }

        public ControlStyle GetStyle(string name)
        {
            return Styles[name];
        }

        public ControlStyle GetStyle(Type controlType)
        {
            ControlStyle controlStyle = null;

            while (controlStyle == null && controlType != null)
            {
                controlStyle = Styles.FirstOrDefault(s => s.TargetType == controlType);
                controlType = controlType.GetTypeInfo().BaseType;
            }

            return controlStyle;
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
            var skinSerializer = new GuiJsonSerializer(contentManager, customControlTypes);

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return skinSerializer.Deserialize<Skin>(jsonReader);
            }
        }


        public T Create<T>(string template, string name = null, string text = null)
            where T : Control, new()
        {
            return Create<T>(template, Vector2.Zero, name, text);
        }

        public T Create<T>(string template, Vector2 position, string name = null, string text = null)
            where T : Control, new()
        {
            var control = new T();
            GetStyle(template).Apply(control);
            control.Name = name;
            control.Position = position;
            control.Text = text;
            return control;
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
                GetStyle(template).Apply(control);

            return control;
        }

        public static Skin FromDefault(BitmapFont font)
        {
            var skin = new Skin
            {
                Fonts = { font },
                Styles =
                {
                    new ControlStyle(typeof(Control))
                    {
                        {nameof(Control.Color), new Color(51, 51, 55)},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                        {nameof(Control.BorderThickness), 1},
                        {nameof(Control.TextColor), new Color(241, 241, 241)},
                        {nameof(StackPanel.Padding), new Thickness(5)}
                    },
                    new ControlStyle(typeof(ComboBox))
                    {
                        {nameof(Control.Color), new Color(51, 51, 55)},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                        {nameof(Control.BorderThickness), 1},
                        {nameof(Control.TextColor), new Color(241, 241, 241)},
                        {nameof(StackPanel.Padding), new Thickness(5)},
                        {nameof(ComboBox.DropDownColor) , new Color(51, 51, 55)}
                    },
                    new ControlStyle(typeof(Label))
                    {
                        {nameof(Control.Color), Color.Transparent},
                        {nameof(Control.TextColor), Color.White}
                    },
                    new ControlStyle(typeof(TextBox))
                    {
                        {nameof(Control.Color), Color.LightGray},
                        {nameof(Control.TextColor), Color.Black},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                    },
                    new ControlStyle(typeof(Button))
                    {
                        {nameof(Control.Color), new Color(51, 51, 55)},
                        {nameof(Control.BorderColor), new Color(67, 67, 70)},
                        {nameof(Control.BorderThickness), 1},
                        {nameof(Control.TextColor), new Color(241, 241, 241)},
                        {
                            nameof(Control.HoverStyle),
                            new ControlStyle
                            {
                                {nameof(Button.Color), new Color(62, 62, 64)}
                            }
                        },
                        {
                            nameof(Button.PressedStyle),
                            new ControlStyle
                            {
                                {nameof(Button.Color), new Color(0, 122, 204)}
                            }
                        }
                    }
                }
            };
            return skin;
        }
    }
}