using System.Windows.Forms;

namespace ContentExplorer.WindowsGL
{
    public class WindowsPlatform : IPlatformSpecific
    {
        public string OpenFile()
        {
            var dialog = new OpenFileDialog();

            if(dialog.ShowDialog() == DialogResult.OK)
                return dialog.FileName;

            return null;
        }
    }
}