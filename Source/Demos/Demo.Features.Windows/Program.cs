using System;

namespace Demo.Features.Windows
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
