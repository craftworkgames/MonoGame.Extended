using System;

namespace ContentExplorer.WindowsGL
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new MainGame(new WindowsPlatform()))
                game.Run();
        }
    }
}
