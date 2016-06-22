using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
{
    public class GuiControlData
    {
        public string Name { get; set; }
        public string Style { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class GuiFile
    {
        public GuiFile()
        {
            Controls = new List<GuiControlData>();
        }

        public string Styles { get; set; }
        public List<GuiControlData> Controls { get; private set; }

        public static GuiFile Build(string styles, IEnumerable<GuiControl> controls)
        {
            return new GuiFile
            {
                Styles = styles,
                Controls = controls.Select(control => new GuiControlData
                {
                    Name = control.Name,
                    Style = control.Style.Name,
                    X = control.Position.X,
                    Y = control.Position.Y,
                    Width = control.Size.Width,
                    Height = control.Size.Height
                }).ToList()
            };
        }

        public static GuiFile Load(StreamReader streamReader)
        {
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<GuiFile>(jsonReader);
            }
        }

        public void Save(StreamWriter streamWriter)
        {
            using (var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, this);
            }
        }
    }
}
