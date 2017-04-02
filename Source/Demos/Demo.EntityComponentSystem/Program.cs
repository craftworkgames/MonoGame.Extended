using System;

namespace Demo.StarWarriorGame
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}
