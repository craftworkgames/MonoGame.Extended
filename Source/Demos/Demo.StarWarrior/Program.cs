using System;

namespace Demo.StarWarrior
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (var game = new GameMain())
            {
                game.Run();
            }
        }
    }
}
