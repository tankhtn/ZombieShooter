using System;

namespace ZombieShooter
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ZombieShooterGame game = new ZombieShooterGame())
            {
                game.Run();
            }
        }
    }
#endif
}

