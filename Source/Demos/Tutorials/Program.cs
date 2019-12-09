using System;

namespace Tutorials
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new GameMain(new PlatformConfig { IsFullScreen = false }))
                game.Run();
        }
    }
}
