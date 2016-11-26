using System;

namespace Demo.BitmapFonts
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new Game2())
            {
                game.Run();
            }
        }
    }
}