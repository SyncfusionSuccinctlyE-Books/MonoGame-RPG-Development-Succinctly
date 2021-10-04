using System;

namespace MonoGameRPG
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            //read game config


            using (var game = new Game1())
                game.Run();
        }
    }
}
