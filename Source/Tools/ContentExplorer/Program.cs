using System;

namespace ContentExplorer
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
