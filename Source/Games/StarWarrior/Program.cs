using System;

namespace StarWarrior
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
