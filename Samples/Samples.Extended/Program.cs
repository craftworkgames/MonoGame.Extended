using System;
using System.Windows.Forms;

namespace Samples.Extended
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var form = new MainForm();
            var dialogResult = form.ShowDialog();

            while (dialogResult == DialogResult.OK)
            {
                using (var game = form.Sample)
                    game.Run();

                dialogResult = form.ShowDialog();
            }
        }
    }
#endif
}
