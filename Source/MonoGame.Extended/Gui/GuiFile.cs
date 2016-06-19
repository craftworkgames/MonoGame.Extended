using System.Collections.Generic;
using System.IO;
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
        private readonly List<GuiControlData> _controls;

        public GuiFile(IEnumerable<GuiControl> controls)
        {
            _controls = BuildControlDataList(controls);
        }

        private static List<GuiControlData> BuildControlDataList(IEnumerable<GuiControl> controls)
        {
            var list = new List<GuiControlData>();

            foreach (var control in controls)
            {
                var data = new GuiControlData
                {
                    Name = control.Name,
                    Style = control.Style.Name,
                    X = control.Position.X,
                    Y = control.Position.Y,
                    Width = control.Size.X,
                    Height = control.Size.Y
                };
                list.Add(data);
            }

            return list;
        }

        public static IEnumerable<GuiControlData> Load(StreamReader streamReader)
        {
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                var data = serializer.Deserialize<List<GuiControlData>>(jsonReader);

                return data;
            }
        }

        public void Save(StreamWriter streamWriter)
        {
            using (var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, _controls);
            }
        }
    }
}
