using System;

namespace Demo.Platformer
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var game = new GameMain())
            {
                game.Run();
            }
        }
    }
}