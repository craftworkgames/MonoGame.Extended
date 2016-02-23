using System;

namespace Demo.SpriteSheetAnimations
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SandboxGame())
                game.Run();
        }
    }
#endif
}
